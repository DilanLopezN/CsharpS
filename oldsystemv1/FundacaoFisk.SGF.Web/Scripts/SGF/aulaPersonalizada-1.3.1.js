var PESQUISAAULAPERSONALIZADA = 18, CADAULAPERSONALIZADA = 19;
var HAS_ATIVO = 0;
function mascarar() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                //maskDate("#dataAtividade");
                //maskDate("#dtaIni");
                //maskDate("#dtaFim");
                $("#timeIni").mask("99:99");
                $("#timeFim").mask("99:99");
                $("#hrInicial").mask("99:99");
                $("#hrFinal").mask("99:99");
                
                $("#hrObsInicial").mask("99:99");
                $("#hrObsFinal").mask("99:99");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatInputParticipou(value, rowIndex, obj) {
    var grid = dijit.byId("gridAluno");
    var status = "";
    if (grid.store.objectStore.data[rowIndex].id_aula_dada)
        status = "Sim"
    else
        status = "Não"
    return status;
}

function formatCheckBoxParticipacao(value, rowIndex, obj) {
    try {
        var gridName = 'gridAluno';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input disabled='disabled' class='formatCheckBox' id='" + id + "'/> ";

        setTimeout("configuraCheckBoxParticipou(" + value + ", '" + id + "', '" + gridName + "', 'disabled')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxParticipou(value, id, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try {
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);


                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (disabled == null || disabled == undefined) disabled = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: disabled
                    }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function showEditAulaPersonalizada(cdAulaPersonalizada, cd_aluno) {
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getAulaPersonalizadaById?id=" + cdAulaPersonalizada + "&cd_aluno=" + cd_aluno,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagemAulaPersonalizada", null);
            var value = data.retorno;
            dijit.byId("pesCadProduto")._onChangeActive = false;
            dijit.byId("dtIniAula")._onChangeActive = false;
            dijit.byId("hrInicial")._onChangeActive = false;
            dijit.byId("hrFinal")._onChangeActive = false;
            dojo.byId("cd_aula_personalizada").value = value.cd_aula_personalizada;
            dojo.byId("cd_programacao_turma").value = value.AulaPersonalizadaAlunos[0].cd_programacao_turma;

            LimitarHrInicialFinal();
            dijit.byId("hrInicial").value = value.hh_inicial != null ? dojo.byId("hrInicial").value = value.hh_inicial.replace(":00", '') : "";
            dijit.byId("hrFinal").value = value.hh_final != null ? dojo.byId("hrFinal").value = value.hh_final.replace(":00", '') : "";


            dijit.byId("dtIniAula").value = value.dta_aula_personalizada != null ? dojo.byId("dtIniAula").value = value.dta_aula_personalizada : "";
            dojo.byId("pesCadProduto").value = value.cd_produto;
            dijit.byId("pesCadProduto").set("value", value.cd_produto);
            dojo.byId("cdTurma").value = value.cd_turma_personalizada;
            dojo.byId("cd_turma").value = value.cd_turma;
            dijit.byId("no_turma").set("value", value.no_turma_personalizada);
            
            var grid = dijit.byId("gridAluno");
            if (hasValue(value.AulaPersonalizadaAlunos)) {

               
                var count = 1;
                $.each(value.AulaPersonalizadaAlunos, function (idx, AlunoValue) {

                    AlunoValue.Id = count;

                    if (hasValue(AlunoValue.hh_inicial_aluno))
                        AlunoValue.hh_inicial_aluno = AlunoValue.hh_inicial_aluno.replace(":00", '');
                    if (hasValue(AlunoValue.hh_final_aluno))
                        AlunoValue.hh_final_aluno = AlunoValue.hh_final_aluno.replace(":00", '');
                    count++;
                });

                var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: value.AulaPersonalizadaAlunos, idProperty: "cd_aula_personalizada_aluno" }) });
                grid.setStore(dataStore);
                grid.itensSelecionados = [];
                grid.update();
            }
            dijit.byId("pesCadProduto")._onChangeActive = true;
            dijit.byId("dtIniAula")._onChangeActive = true;
            dijit.byId("hrInicial")._onChangeActive = true;
            dijit.byId("hrFinal")._onChangeActive = true;
            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemMat", error);
        showCarregando();
    });

}

function LimitarHrInicialFinal() {
    require(["dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr", "dojox/json/ref"],
      function (ready, Calendar, Observable, Memory, declare, on, xhr, ref) {
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

                      dijit.byId("hrInicial").constraints.min.setMinutes(minI);
                      dijit.byId("hrInicial").constraints.min.setHours(horaI);
                      dijit.byId("hrInicial").constraints.max.setHours(horaF);
                      dijit.byId("hrFinal").constraints.max.setMinutes(minF);
                      dijit.byId("hrFinal").constraints.min.setHours(horaI);
                      dijit.byId("hrFinal").constraints.max.setHours(horaF);

                  }
                      catch (e) {
                          showCarregando();
                          postGerarLog(e);
                      }
                  },
                  function (error) {
                      showCarregando();
                      apresentaMensagem('apresentadorMensagemTurma', error);
                  });
            });
      });
}

function keepValues(value, grid, ehLink) {
    try {
        limparAulaPersonalizada();
        if (!hasValue(value) && grid != null)
            value = grid.itemSelecionado;
        var cdAulaPersonalizada = hasValue(value) && hasValue(value.cd_aula_personalizada) ? value.cd_aula_personalizada : dojo.byId('cd_aula_personalizada').value;
        if (cdAulaPersonalizada > 0)
            showEditAulaPersonalizada(cdAulaPersonalizada, value.cd_aluno);
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAluno(value, rowIndex, obj) {
    var gridName = 'gridAluno'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosAluno');

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "Id", grid._by_idx[rowIndex].item.Id);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  Id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'Id', 'selecionadoAluno', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'Id', 'selecionadoAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxPRG(value, rowIndex, obj) {
    var gridName = 'gridTurmaPRG'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosPRG');

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_programacao_turma", grid._by_idx[rowIndex].item.cd_programacao_turma);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_programacao_turma', 'selecionadoPRG', -1, 'selecionaTodosPRG', 'selecionaTodosPRG', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_programacao_turma', 'selecionadoPRG', " + rowIndex + ", '" + id + "', 'selecionaTodosPRG', '" + gridName + "')", 2);

    return icon;
}
function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridAulaPersonalizada';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aula_personalizada_aluno", grid._by_idx[rowIndex].item.cd_aula_personalizada_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aula_personalizada_aluno', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aula_personalizada_aluno', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);


        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montaCadastroAulaPersonalizada() {
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
    "dijit/MenuSeparator",
    "dijit/Dialog",
    "dijit/form/DateTextBox",
    "dojo/domReady!"
    ], function (ready, domConstruct, array, xhr, registry, ItemFileReadStore, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, MenuSeparator) {
        ready(function () {
            mascarar();
            populaProduto();
            getPesqAulaPersonalizada();
            var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/coordenacao/getSearchAulaPersonalizada?dataIni=&dataFim=&hrInicial=&hrFinal=&cdProduto=&cdProfessor=&cdSala=&cdAluno=&participou=false",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }
               ), Memory({}));
            var gridAulaPersonalizada = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                    { name: "Aluno", field: "no_aluno", width: "25%", styles: "min-width:80px;" },
                    { name: "Produto", field: "no_produto", width: "5%", styles: "min-width:80px;" },
                    { name: "Data Aula", field: "dta_aula_personalizada", width: "8%" },
                    { name: "Hora Inicial", field: "hh_inicial", width: "5%", styles: "min-width:80px;text-align: center;" },
                    { name: "Hora Final", field: "hh_final", width: "5%", styles: "min-width:80px;text-align: center;" },
                    { name: "Sala", field: "no_sala", width: "7%" },
                    { name: "Professor", field: "no_professor", width: "10%" },
                    { name: "Personalizada", field: "no_turma_personalizada", width: "20%" },
                    { name: "Participou", field: "participou", width: "6%", styles: "text-align: center;" }
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
            }, "gridAulaPersonalizada");
            gridAulaPersonalizada.startup();
            gridAulaPersonalizada.canSort = function (col) { return Math.abs(col) != 1};
            gridAulaPersonalizada.on("RowDblClick", function (evt) {
                showCarregando();
                
                var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                // registroGrid = item
                
                apresentaMensagem('apresentadorMensagem', '');
                gridAulaPersonalizada.itemSelecionado = item;
                keepValues(null, gridAulaPersonalizada, false);
                dijit.byId("cadAulaPersonalizada").show();
                IncluirAlterar(0, 'divAlterarAulaPer', 'divIncluirAulaPer', 'divExcluirAulaPer', 'apresentadorMensagemAulaPersonalizada', 'divCancelarAulaPer', 'divLimparAulaPer');
            }, true);
            // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
            if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                var ieversion = new Number(RegExp.$1)
                if (ieversion == 7)
                    // Para IE7
                    dojo.byId('cadAulaPersonalizada').childNodes[1].style.height = '100%';
                dojo.byId('cadFilha').childNodes[1].style.height = '100%';
            }

            var gridAluno = new EnhancedGrid({
                structure:
                [
                    { name: "<input id='selecionaTodosAluno' style='display:none'/>", field: "selecionadoAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAluno },
                    { name: "Nome", field: "no_aluno", width: "40%" },
                    { name: "Participou", field: "id_aula_dada", width: "15%", styles: "text-align: center;", formatter: formatInputParticipou },
                    { name: "H.Inicial", field: "hh_inicial_aluno", width: "13%"},
                    { name: "H.Final", field: "hh_final_aluno", width: "12%"},
                    { name: "Aula", field: "tx_obs_aula", width: "42%" }
                ],
                canSort: true,
                noDataMessage: "Nenhum registro encontrado.",
                contentEditable: false,
            }, "gridAluno");
            gridAluno.startup();
            gridAluno.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3; };
            
            var gridTurmaPRG = new EnhancedGrid({
                structure:
                [
                    { name: "<input id='selecionaTodosPRG' style='display:none'/>", field: "selecionadoPRG", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPRG },
                    { name: "Turma", field: "no_turma", width: "45%" },
                    { name: "Aula", field: "nm_aula_programacao_turma", width: "10%", styles: "text-align: center;" },
                    { name: "Data", field: "dt_programacao_turma", width: "20%", styles: "text-align: center;" },
                    { name: "Conteúdo", field: "dc_programacao_turma", width: "20%" }
                ],
                canSort: true,
                selectionMode: "single",
                noDataMessage: "Nenhum registro encontrado.",
                contentEditable: false,
                plugins: {
                    pagination: {
                        pageSizes: ["7", "20", "50",  "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "7",
                        gotoButton: true,
                        maxPageStep: 7,
                        position: "button"
                    }
                }
            }, "gridTurmaPRG");
            gridTurmaPRG.startup();
            gridTurmaPRG.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 2 && Math.abs(col) != 3 && Math.abs(col) != 4 && Math.abs(col) != 5; };
            //visaoEvento
            new Button({
                label: "Incluir",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    if (!hasValue(dojo.byId('dtIniAula').value)){
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroAlunoAulaPersonalizada);
                        apresentaMensagem("apresentadorMensagemAulaPersonalizada", mensagensWeb);
                    }
                    else {
                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                            montarGridPesquisaAluno(false, function () {
                                abrirAlunoFk(CADAULAPERSONALIZADA);
                            });
                        }
                        else
                            abrirAlunoFk(CADAULAPERSONALIZADA);
                    }
                }
            }, "btnNovoAluno");


            new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem("apresentadorMensagem", ""); pesquisarAulaPersonalizada(true) } }, "pesquisarAulaPersonalizada");

            decreaseBtn(document.getElementById("pesquisarAulaPersonalizada"), '32px');
            //Crud Pessoa
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { IncluirAulaPersonalizada(); } }, "incluirAulaPer");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    try {
                        apresentaMensagem("apresentadorMensagemAulaPersonalizada", null);
                        showCarregando();
                        keepValues(null, gridAulaPersonalizada, false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }

            }, "cancelarAulaPer");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { dijit.byId("cadAulaPersonalizada").hide(); } }, "fecharAulaPer");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                    AlteraAulaPersonalizada(); 
                }
            }, "alterarAulaPer");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarAulaPersonalizada() }); } }, "deleteAulaPer");
            new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparAulaPersonalizada(); } }, "limparAulaPer");
            //Fim

            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                onClick: function () {
                    showCarregando();
                    if (!hasValue(dijit.byId("gridPesquisaTurmaPersonalizadaFK"))) {
                        montarGridPesquisaTurmaPersonalizadaFK(function () {
                            abrirTurmaPersonalizadaFk();
                            dijit.byId("proTurmaPersonalizada").on("show", function () { dijit.byId('gridPesquisaTurmaPersonalizadaFK').update(); });
                            
                        });
                    }
                    else
                        abrirTurmaPersonalizadaFk();
                }
            }, "pesTurmaFKCad");


            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    try {
                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                            montarGridPesquisaAluno(false, function () {
                                abrirAlunoFk(PESQUISAAULAPERSONALIZADA);
                            });
                        }
                        else
                            abrirAlunoFk(PESQUISAAULAPERSONALIZADA);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "pesAluno");

            new Button({
                label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                    try {
                        if (!validarHoraView('timeIni', 'timeFim', 'apresentadorMensagem'))
                            return false;

                        if (!validarDatasIniFim('dtInicial', 'dtFinal', 'apresentadorMensagem', dojo.date))
                            return false;
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/geturlrelatorioAulaPersonalizada?" + getStrGridParameters('gridAulaPersonalizada') + "dataIni=" + dojo.byId('dtInicial').value + "&dataFim=" + dojo.byId('dtFinal').value +
                                                            "&hrInicial=" + dojo.byId('timeIni').value + "&hrFinal=" + dojo.byId('timeFim').value + "&cdProduto=" + dijit.byId('pesProduto').value +
                                                            "&cdProfessor=" + dijit.byId('pesProfessor').value + "&cdSala=" + dijit.byId('pesSala').value + "&cdAluno=" + dojo.byId('cdAluno').value + "&participou=" + dijit.byId('ckParticipou').checked,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagemAulaPersonalizada', error);
                        });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "relatorioAulaPersonalizada");
            new Button({
                label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                    function () {
                        limparAulaPersonalizada();
                        IncluirAlterar(1, 'divAlterarAulaPer', 'divIncluirAulaPer', 'divExcluirAulaPer', 'apresentadorMensagemAulaPersonalizada', 'divCancelarAulaPer', 'divLimparAulaPer');
                        LimitarHrInicialFinal();
                        dijit.byId("cadAulaPersonalizada").show();

                    }
            }, "novaAulaPersonalizada");

            new Button({
                label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                onClick: function () { lancarDiario(); }
            }, "selecionaPRGFK");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () { dijit.byId("dialogTurmaPRG").hide(); }
            }, "fecharPRGFK");

            var buttonFkArray = ['pesAluno', 'pesTurmaFKCad'];

            for (var p = 0; p < buttonFkArray.length; p++) {
                var buttonFk = document.getElementById(buttonFkArray[p]);

                if (hasValue(buttonFk)) {
                    buttonFk.parentNode.style.minWidth = '18px';
                    buttonFk.parentNode.style.width = '18px';
                }
            }
            new Button({
                label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                    dojo.byId('cdAluno').value = 0;
                    dojo.byId("noAluno").value = "";
                    dijit.byId('limparAlunoPes').set("disabled", true);
                }
            }, "limparAlunoPes");
            if (hasValue(document.getElementById("limparAlunoPes"))) {
                document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
            }
            new Button({ label: "Editar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { postObsAulaLancada(); } }, "salvarObs");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogGrid").hide(); } }, "fecharObs");

            dijit.byId("tagAlunos").on("show", function (e) {
                dijit.byId("gridAluno").update();
            });

            //link turma
            var menu = new DropDownMenu({ id: "menuAcoesRelacionadas", style: "height: 25px" });

            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditar(gridAulaPersonalizada.itensSelecionados, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemoverAulaPersonalizada(gridAulaPersonalizada.itensSelecionados); }
            });
            menu.addChild(acaoRemover);


            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas3",
                dropDown: menu,
                id: "acoesRelacionadas3"
            });
            dojo.byId("linkAcoesAulaPersonalizada").appendChild(button.domNode);


            var menuMsim = new DropDownMenu({ style: "height: 25px" });

            var acaoEditarMsim = new MenuItem({
                label: "Excluir",
                onClick: function () { }
            });
            menuMsim.addChild(acaoEditarMsim);
            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridAulaPersonalizada, 'todosItens', ['pesquisarAulaPersonalizada', 'relatorioAulaPersonalizada']);
                    pesquisarAulaPersonalizada(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () {
                    buscarItensSelecionados('gridAulaPersonalizada', 'selecionado', 'cd_aula_personalizada_aluno', 'selecionaTodos', ['pesquisarAulaPersonalizada', 'relatorioAulaPersonalizada'], 'todosItens');
                }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItens"
            });
            dojo.byId("linkSelecionados").appendChild(button.domNode);

            var menuAluno = new DropDownMenu({ style: "height: 25px" });
            var acaoExcluirAluno = new MenuItem({
                label: "Excluir",
                onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'Id', dijit.byId("gridAluno")); }
            });
            menuAluno.addChild(acaoExcluirAluno);

            var acaoContratoAluno = new MenuItem({
                label: "Lançar Aula",
                onClick: function () {
                    pesquisarProgramacoesTurmaPorAluno();

                }
            });
            menuAluno.addChild(acaoContratoAluno);

            var acaoContratoAlunoA = new MenuItem({
                label: "Editar Aula",
                onClick: function () {
                    getObsDiarioAula();
                }
            });
            menuAluno.addChild(acaoContratoAlunoA);

            var buttonAluno = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasAluno",
                dropDown: menuAluno,
                id: "acoesRelacionadasAluno"
            });
            dojo.byId("linkAcoesAluno").appendChild(buttonAluno.domNode);
            adicionarAtalhoPesquisa(['dtInicial', 'dtFinal', 'timeIni', 'timeFim', 'pesProduto', 'pesProfessor', 'pesSala'], 'pesquisarAulaPersonalizada', ready);
            dijit.byId("pesCadProduto").on("change", function (e) {
                apresentaMensagem('apresentadorMensagemAulaPersonalizada', null);
                dojo.byId('cdTurma').value = 0;
                dijit.byId('no_turma').set("value", '');

                var gridAluno = dijit.byId("gridAluno");
                gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                gridAluno.itensSelecionados = [];
                gridAluno.update();
            });

            dijit.byId("dtIniAula").on("change", function (e) {
                dojo.byId('cdTurma').value = 0;
                dijit.byId('no_turma').set("value", '');

                var gridAluno = dijit.byId("gridAluno");
                gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                gridAluno.itensSelecionados = [];
                gridAluno.update();
            });
            dijit.byId("hrInicial").on("change", function (e) {
                dojo.byId('cdTurma').value = 0;
                dijit.byId('no_turma').set("value", '');

                var gridAluno = dijit.byId("gridAluno");
                gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                gridAluno.itensSelecionados = [];
                gridAluno.update();
            });
            dijit.byId("hrFinal").on("change", function (e) {
                dojo.byId('cdTurma').value = 0;
                dijit.byId('no_turma').set("value", '');

                var gridAluno = dijit.byId("gridAluno");
                gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                gridAluno.itensSelecionados = [];
                gridAluno.update();
            });

            showCarregando();
        });
    });
}


function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAlgumAlterar, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            showCarregando();            
            apresentaMensagem('apresentadorMensagem', '');
            var gridAulaPersonalizada = dijit.byId("gridAulaPersonalizada");
            gridAulaPersonalizada.itemSelecionado = itensSelecionados[0];
            keepValues(null, gridAulaPersonalizada, false);
            dijit.byId("cadAulaPersonalizada").show();
            IncluirAlterar(0, 'divAlterarAulaPer', 'divIncluirAulaPer', 'divExcluirAulaPer', 'apresentadorMensagemAulaPersonalizada', 'divCancelarAulaPer', 'divLimparAulaPer');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function loadProduto(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            if (field == 'pesProduto')
                itemsCb.push({ id: 0, name: "Todos" });
            var cbProduto = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_produto, name: value.no_produto });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbProduto.store = stateStore;
            if (field == 'pesProduto')
                cbProduto.set("value", 0);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function getPesqAulaPersonalizada() {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/getPesqAulaPersonalizada",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (hasValue(data.retorno)) {
                loadSala(data.retorno.salas, 'pesSala');
                loadProduto(data.retorno.produtos, 'pesProduto');
                loadProfessor(data.retorno.professores, 'pesProfessor');
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAulaPersonalizada', error);
    });
}

function loadSala(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            itemsCb.push({ id: 0, name: "Todos" });
            var cbSala = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_sala, name: value.no_sala });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbSala.store = stateStore;
            cbSala.set("value", 0);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function loadProfessor(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbProfessor = dijit.byId(field);

            itemsCb.push({ id: 0, name: "Todos" });

            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_funcionario, name: value.no_pessoa});
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbProfessor.store = stateStore;
            cbProfessor.set("value", 0);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarAulaPersonalizada(limparItens) {
    
    if (!validarHoraView('timeIni', 'timeFim', 'apresentadorMensagem'))
        return false;

    if (!validarDatasIniFim('dtInicial', 'dtFinal', 'apresentadorMensagem', dojo.date))
        return false;

    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getSearchAulaPersonalizada?dataIni=" + dojo.byId('dtInicial').value + "&dataFim=" + dojo.byId('dtFinal').value + "&hrInicial=" + dojo.byId('timeIni').value +
                                        "&hrFinal=" + dojo.byId('timeFim').value + "&cdProduto=" + dijit.byId('pesProduto').value + "&cdProfessor=" + dijit.byId('pesProfessor').value + "&cdSala=" + dijit.byId('pesSala').value +
                                        "&cdAluno=" + dojo.byId('cdAluno').value + "&participou=" +  dijit.byId('ckParticipou').checked,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_aula_personalizada"
                }
                ), Memory({ idProperty: "cd_aula_personalizada" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAulaPersonalizada = dijit.byId("gridAulaPersonalizada");
            if (limparItens) {
                gridAulaPersonalizada.itensSelecionados = [];
            }
            gridAulaPersonalizada.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirAlunoFk(tipo) {
    dojo.byId('tipoRetornoAlunoFK').value = tipo;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    if (tipo == PESQUISAAULAPERSONALIZADA)
        pesquisarAlunoFK(true);
    else
        pesquisarAlunoFKAulaPersonalizada();
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}


function abrirTurmaPersonalizadaFk() {
    apresentaMensagem("apresentadorMensagemAulaPersonalizada", null);
    if (!hasValue(dijit.byId('pesCadProduto').value) || dijit.byId('pesCadProduto').value <= 0 ||
        !hasValue(dojo.byId('dtIniAula').value) ||
        !hasValue(dojo.byId('hrInicial').value) ||
        !hasValue(dojo.byId('hrFinal').value)) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaAulaPersonalizada);
        apresentaMensagem("apresentadorMensagemAulaPersonalizada", mensagensWeb);
        showCarregando();
    }
    else {
        dijit.byId("gridPesquisaTurmaPersonalizadaFK").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        dijit.byId("gridPesquisaTurmaPersonalizadaFK").itensSelecionados = new Array();
        pesquisarTurmaFK();
        dijit.byId('gridPesquisaTurmaPersonalizadaFK').update();
        
    }
}

function retornarAlunoFKPesqAulaPersonalizada() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        /*if (hasValue(gridPesquisaAluno.itensSelecionados) && gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
            return false;
        }*/ else {
            dojo.byId('cdAluno').value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId('noAluno').value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAlunoPes').set("disabled", false);
        }
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaProduto() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=" + HAS_ATIVO + "&cd_produto=null",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (dataProdAtivo) {
        try {
            loadProduto(jQuery.parseJSON(dataProdAtivo).retorno, 'pesCadProduto');
        } catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem('apresentadorMensagemAulaPersonalizada', error);
    });
}


function retornarTurmaPersonalizadaFK() {
    try {
        var gridPesquisaTurmaPersonalizadaFK = dijit.byId("gridPesquisaTurmaPersonalizadaFK");

        if (!hasValue(gridPesquisaTurmaPersonalizadaFK.itensSelecionados) || gridPesquisaTurmaPersonalizadaFK.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        if (hasValue(gridPesquisaTurmaPersonalizadaFK.itensSelecionados) && gridPesquisaTurmaPersonalizadaFK.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
            return false;
        } else {
            dojo.byId('cdTurma').value = gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0].cd_turma;
            dojo.byId('no_turma').value = gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0].no_turma;
        }
        dijit.byId("proTurmaPersonalizada").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function limparAulaPersonalizada() {
    try {
        clearForm("formAulaPersonalizada");
        getLimpar("#formAulaPersonalizada");
        apresentaMensagem("apresentadorMensagemAulaPersonalizada", "");

        var gridAluno = dijit.byId("gridAluno");
        gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        gridAluno.itensSelecionados = [];
        gridAluno.update();

    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAlunoFKAulaPersonalizada() {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try {
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/aluno/GetAlunoSearchAulaPer?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&cdSituacao=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0" + "&dtaAula=" + dojo.byId("dtIniAula").value,
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
function retornarAlunoFKAulaPer() {
    
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridAluno = dijit.byId("gridAluno");
                var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridAluno = (hasValue(gridAluno) && hasValue(gridAluno.store) && hasValue(gridAluno.store.objectStore.data)) ? gridAluno.store.objectStore.data : [];

                    if (storeGridAluno != null && storeGridAluno.length > 0) {                       
                        $.each(gridPesquisaAluno.itensSelecionados, function (idx, value) {
                            var existeGrid = gridAluno.store.objectStore.data.some(function (item) {
                                return item.cd_aluno === value.cd_aluno;
                            });
                            if (!existeGrid) {
                                insertObjSort(gridAluno.store.objectStore.data, "Id", { Id: geradorIdAlunos(gridAluno), cd_aluno: value.cd_aluno, no_aluno: value.no_pessoa, id_aula_dada: false, tx_obs_aula: "" });
                            }
                            
                        });
                        gridAluno.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAluno.store.objectStore.data }) }));

                    } else {
                        gridAluno.store.objectStore.data = [];
                        $.each(gridPesquisaAluno.itensSelecionados, function (index, val) {

                            insertObjSort(gridAluno.store.objectStore.data, "Id", { Id: geradorIdAlunos(gridAluno), cd_aluno: val.cd_aluno, no_aluno: val.no_pessoa, id_aula_dada: false, tx_obs_aula: "" });
                        });
                        gridAluno.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAluno.store.objectStore.data }) }));

                    }
                    dijit.byId("proAluno").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function montarAlunoAulaPersonalizada() {
    try {
        var alunos = [];
        var gridAluno = dijit.byId("gridAluno");

        hasValue(gridAluno) ? gridAluno.store.save() : null;
        if (hasValue(gridAluno) && hasValue(gridAluno._by_idx))
            var data = gridAluno._by_idx;
        else alunos = null;
        if (hasValue(gridAluno) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                alunos.push({
                    cd_aula_personalizada_aluno: val.item.cd_aula_personalizada_aluno,
                    cd_aula_personalizada: val.item.cd_aula_personalizada,
                    cd_aluno: val.item.cd_aluno,
                    id_aula_dada: val.item.id_aula_dada,
                    tx_obs_aula: val.item.tx_obs_aula,
                    hh_inicial_aluno: val.item.hh_inicial_aluno,
                    hh_final_aluno: val.item.hh_final_aluno,
                    cd_diario_aula: val.item.cd_diario_aula,
                    cd_programacao_turma : val.item.cd_programacao_turma,
                    cd_turma: val.item.cd_turma,
                    cd_sala_prog: val.item.cd_sala_prog,
                    nm_aula_programacao_turma: val.item.nm_aula_programacao_turma,
                    dc_aula: val.item.dc_programacao_turma,
                    cd_professor: val.item.cd_professor
                });
            });
        return alunos;
    }
    catch (e) {
        postGerarLog(e);
    }
}
function IncluirAulaPersonalizada() {
    try {
        var horariosTurma = null;
        var gridPesquisaTurmaPersonalizadaFK = dijit.byId("gridPesquisaTurmaPersonalizadaFK");
        if (hasValue(gridPesquisaTurmaPersonalizadaFK) && hasValue(gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0]) &&
            hasValue(gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0].horariosTurma[0])) {

            horariosTurma = gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0].horariosTurma;
        }

        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAulaPersonalizada', null);
        if (!dijit.byId("formAulaPersonalizada").validate()) return false;
        var aulaAluno = montarAlunoAulaPersonalizada();
        if (!hasValue(aulaAluno) || aulaAluno.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroAulaPerSemAluno);
            apresentaMensagem("apresentadorMensagemAulaPersonalizada", mensagensWeb);
        } else {
            var dataAula = hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
            
            require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                showCarregando();
                xhr.post(Endereco() + "/api/coordenacao/postIncluirAulaPersonalizada", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        cd_aula_personalizada: dojo.byId("cd_aula_personalizada").value,
                        dt_aula_personalizada: dataAula,
                        hh_inicial: dojo.byId("hrInicial").value,
                        hh_final: dojo.byId("hrFinal").value,
                        cd_produto: dijit.byId("pesCadProduto").value,
                        cd_turma_personalizada: dojo.byId("cdTurma").value,                        
                        AulaPersonalizadaAlunos: aulaAluno,
                        Turma: { horariosTurma: horariosTurma }
                    })
                }).then(function (data) {
                    try {
                        showCarregando();
                        data = jQuery.parseJSON(data);
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridAulaPersonalizada';
                            var grid = dijit.byId(gridName);
                            apresentaMensagem('apresentadorMensagem', data);
                            dijit.byId("cadAulaPersonalizada").hide();
                            if (!hasValue(grid.itensSelecionados)) {
                                grid.itensSelecionados = [];
                            }
                            for (var i = 0; i < itemAlterado.length; i++) {
                                removeObjSort(grid.itensSelecionados, "cd_aula_personalizada_aluno", itemAlterado[i].cd_aula_personalizada_aluno);
                                insertObjSort(grid.itensSelecionados, "cd_aula_personalizada_aluno", itemAlterado[i]);
                                if (hasValue(grid.store.objectStore.data))
                                    setGridPagination(grid, itemAlterado[i], "cd_aula_personalizada_aluno");
                            }
                            buscarItensSelecionados(gridName, 'selecionado', 'cd_aula_personalizada_aluno', 'selecionaTodos', ['pesquisarAulaPersonalizada', 'relatorioAulaPersonalizada'], 'todosItens');
                        } else
                            apresentaMensagem('apresentadorMensagemAulaPersonalizada', data);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAulaPersonalizada', error.response.data);
                });
            });
        }
    }
    catch (er) {
        postGerarLog(er);
    }

}

function AlteraAulaPersonalizada() {
    try {
        var horariosTurma = null;
        if(hasValue(itemSelecionado) && hasValue(itemSelecionado.horariosTurma[0]))
            horariosTurma = itemSelecionado.horariosTurma;

        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAulaPersonalizada', null);
        var aulaAluno = montarAlunoAulaPersonalizada();
        if (!hasValue(aulaAluno) || aulaAluno.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroAulaPerSemAluno);
            apresentaMensagem("apresentadorMensagemAulaPersonalizada", mensagensWeb);

        } else {
            var dataAula = dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            if (!dijit.byId("formAulaPersonalizada").validate()) return false;
            require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                showCarregando();
                xhr.post(Endereco() + "/api/coordenacao/postAlterarAulaPersonalizada", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        cd_aula_personalizada: dojo.byId("cd_aula_personalizada").value,
                        dt_aula_personalizada: dataAula,
                        hh_inicial: dojo.byId("hrInicial").value,
                        hh_final: dojo.byId("hrFinal").value,
                        cd_produto: dijit.byId("pesCadProduto").value,
                        cd_turma_personalizada: dojo.byId("cdTurma").value,
                        Turma: { horariosTurma: horariosTurma },
                        AulaPersonalizadaAlunos: aulaAluno
                    })
                }).then(function (data) {
                    try {
                        showCarregando();
                        data = jQuery.parseJSON(data);
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridAulaPersonalizada';
                            var grid = dijit.byId(gridName);
                            apresentaMensagem('apresentadorMensagem', data);
                            dijit.byId("cadAulaPersonalizada").hide();
                            if (!hasValue(grid.itensSelecionados)) {
                                grid.itensSelecionados = [];
                            }
                            removeObjSort(grid.itensSelecionados, "cd_aula_personalizada", itemAlterado[0].cd_aula_personalizada);
                            for (var i = 0; i < itemAlterado.length; i++) {
                                insertObjSort(grid.itensSelecionados, "cd_aula_personalizada_aluno", itemAlterado[i]);
                                if (hasValue(grid.store.objectStore.data))
                                    setGridPagination(grid, itemAlterado[i], "cd_aula_personalizada_aluno");
                            }
                            buscarItensSelecionados(gridName, 'selecionado', 'cd_aula_personalizada_aluno', 'selecionaTodos', ['pesquisarAulaPersonalizada', 'relatorioAulaPersonalizada'], 'todosItens');
                        } else
                            apresentaMensagem('apresentadorMensagemAulaPersonalizada', data);
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAulaPersonalizada', error.response.data);
                });
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function pesquisarProgramacoesTurmaPorAluno() {
    try {
        var gridAluno = dijit.byId("gridAluno");
        if(!hasValue(dojo.byId("dtIniAula").value)){
            caixaDialogo(DIALOGO_AVISO, msgMensagensAulaPersonalizadaSemData, null);
            return false;
        }
        if (!hasValue(gridAluno.itensSelecionados) || gridAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        if (hasValue(gridAluno.itensSelecionados) && gridAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
            return false;
        } else {
            var listaProgAluno = "";
            if (hasValue(gridAluno.store.objectStore.data) && gridAluno.store.objectStore.data.length > 0) {
                var alunos = jQuery.grep(gridAluno.store.objectStore.data, function (value) {
                    return value.cd_aluno == gridAluno.itensSelecionados[0].cd_aluno && value.cd_programacao_turma > 0;
                });
                if (hasValue(alunos) && alunos.length > 0) {
                    $.each(alunos, function (index, value) {
                        listaProgAluno = listaProgAluno + value.cd_programacao_turma + ','
                    });
                    listaProgAluno = listaProgAluno.substring(0, listaProgAluno.length - 1);
                }
            }
            
            var myStore = dojo.store.Cache(
                        dojo.store.JsonRest({
                            target: Endereco() + "/api/coordenacao/getProgramacoesTurmaPorAluno?cd_aluno=" + gridAluno.itensSelecionados[0].cd_aluno + "&dt_inicial=" + dojo.byId("dtIniAula").value + "&cd_turma_principal=" + dojo.byId("cdTurma").value + "&listaProg=" + listaProgAluno,
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }
                ), dojo.store.Memory({}));
            var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridTurmaPRG");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

            dijit.byId("dialogTurmaPRG").show();

            //if (gridAluno.itensSelecionados[0].id_aula_dada) {
            //    caixaDialogo(DIALOGO_AVISO, msgMensagensAulaPersonalizadaLancada, null);
            //    return false;
            //}
            //else {
            //    var myStore = dojo.store.Cache(
            //            dojo.store.JsonRest({
            //                target: Endereco() + "/api/coordenacao/getProgramacoesTurmaPorAluno?cd_aluno=" + gridAluno.itensSelecionados[0].cd_aluno + "&dt_inicial=" + dojo.byId("dtIniAula").value + "&cd_turma_principal=" + dojo.byId("cdTurma").value,
            //                handleAs: "json",
            //                preventCache: true,
            //                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            //            }
            //    ), dojo.store.Memory({}));
            //    var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
            //    var grid = dijit.byId("gridTurmaPRG");
            //    grid.itensSelecionados = [];
            //    grid.setStore(dataStore);

            //    dijit.byId("dialogTurmaPRG").show();
            //}
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function lancarDiario() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
    var gridProg = dijit.byId("gridTurmaPRG");
    if (!hasValue(gridProg.itensSelecionados) || gridProg.itensSelecionados.length <= 0) {
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        return false;
    }
    if (hasValue(gridProg.itensSelecionados) && gridProg.itensSelecionados.length > 1) {
        caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
        return false;
    } else {
        var item = gridProg.itensSelecionados[0];
        if (!hasValue(item.cd_professor) || item.cd_professor <= 0) {
            caixaDialogo(DIALOGO_AVISO, "Turma de origem não tem professor.", null);
            return false;
        } else {
            var gridAluno = dijit.byId("gridAluno");
            var alunoSelecionado = gridAluno.itensSelecionados[0];

            var storeGridAluno = (hasValue(gridAluno) && hasValue(gridAluno.store) && hasValue(gridAluno.store.objectStore.data)) ? gridAluno.store.objectStore.data : [];

            if (storeGridAluno != null && storeGridAluno.length > 0) {
                if ((hasValue(alunoSelecionado.cd_programacao_turma) && item.cd_programacao_turma != alunoSelecionado.cd_programacao_turma) ||
                    (hasValue(item.Id) && item.Id != item.Id)) {
                    var Id = geradorIdAlunos(gridAluno);
                    $.each(alunoSelecionado, function (idx, value) {
                        insertObjSort(gridAluno.store.objectStore.data, "Id", {
                            Id: Id,
                            cd_aluno: alunoSelecionado.cd_aluno,
                            no_aluno: alunoSelecionado.no_aluno,
                            id_aula_dada: true,
                            tx_obs_aula: item.dc_programacao_turma,
                            cd_programacao_turma: item.cd_programacao_turma,
                            cd_turma: item.cd_turma,
                            cd_sala_prog: item.cd_sala_prog,
                            nm_aula_programacao_turma: item.nm_aula_programacao_turma,
                            dc_aula: item.dc_programacao_turma,
                            cd_professor: item.cd_professor
                        });
                    });
                    gridAluno.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAluno.store.objectStore.data }) }));
                } else {
                    alunoSelecionado.id_aula_dada = true;
                    alunoSelecionado.tx_obs_aula = item.dc_programacao_turma;
                    alunoSelecionado.cd_programacao_turma = item.cd_programacao_turma;
                    alunoSelecionado.cd_turma = item.cd_turma;
                    alunoSelecionado.cd_sala_prog = item.cd_sala_prog;
                    alunoSelecionado.nm_aula_programacao_turma = item.nm_aula_programacao_turma;
                    alunoSelecionado.dc_aula = item.dc_programacao_turma;
                    alunoSelecionado.cd_professor = item.cd_professor;
                }
            }

            gridAluno.update();
            dojo.byId("cd_programacao_turma").value = null;
            dijit.byId("dialogTurmaPRG").hide();
        }
    }

        });
}

function geradorIdAlunos(gridAlunos) {
    try {
        var id = gridAlunos.store.objectStore.data.length;
        var itensArray = gridAlunos.store.objectStore.data.sort(function byOrdem(a, b) { return a.Id - b.Id; });
        if (id == 0)
            id = 1;
        else if (id > 0)
            id = itensArray[id - 1].Id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getObsDiarioAula() {
    var gridAluno = dijit.byId("gridAluno");
   
    if (!hasValue(gridAluno.itensSelecionados) || gridAluno.itensSelecionados.length <= 0) {
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        return false;
    }
    if (hasValue(gridAluno.itensSelecionados) && gridAluno.itensSelecionados.length > 1) {
        caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
        return false;
    }
    else {        
        var item = gridAluno.itensSelecionados[0];
        document.getElementById("dialogGrid_title").innerHTML = "Aula Aluno " + item.no_aluno;
        if (!item.id_aula_dada) {
            caixaDialogo(DIALOGO_AVISO, msgMensagemAlunoSemDiario, null);
            return false;
        } else {
            showCarregando();
            if (!hasValue(item.cd_diario_aula) || item.cd_diario_aula <= 0) {
                dijit.byId("textAreaObs").set("value", item.tx_obs_aula);

                var alunoSelecionado = dijit.byId("gridAluno").itensSelecionados[0];

                LimitarObsHrInicialFinal();
                
                dijit.byId("hrObsInicial").value = hasValue(alunoSelecionado.hh_inicial_aluno) ? dijit.byId("hrObsInicial").set("value", "T" + alunoSelecionado.hh_inicial_aluno + ":00") : dijit.byId("hrObsInicial").reset();
                dijit.byId("hrObsFinal").value = hasValue(alunoSelecionado.hh_final_aluno) ? dijit.byId("hrObsFinal").set("value", "T" + alunoSelecionado.hh_final_aluno + ":00") : dijit.byId("hrObsFinal").reset();

                dijit.byId("dialogGrid").show();
                showCarregando();
            }
            else {
                dojo.xhr.get({
                    url: Endereco() + "/api/coordenacao/getObsDiarioAula?cd_diario_aula=" + item.cd_diario_aula,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data).retorno;
                        dijit.byId("textAreaObs").set("value", data);

                        var alunoSelecionado = dijit.byId("gridAluno").itensSelecionados[0];
                        
                        LimitarObsHrInicialFinal();
                        
                        dijit.byId("hrObsInicial").value = hasValue(alunoSelecionado.hh_inicial_aluno) ? dijit.byId("hrObsInicial").set("value", "T" + alunoSelecionado.hh_inicial_aluno + ":00") : dijit.byId("hrObsInicial").reset();
                        dijit.byId("hrObsFinal").value = hasValue(alunoSelecionado.hh_final_aluno) ? dijit.byId("hrObsFinal").set("value", "T" + alunoSelecionado.hh_final_aluno + ":00") : dijit.byId("hrObsFinal").reset();

                        dijit.byId("dialogGrid").show();
                        showCarregando();
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem("apresentadorMensagemMat", error);
                    showCarregando();
                });
            }
        }
    }
}

var itemSelecionado = "";
function LimitarObsHrInicialFinal() {
    try {
        var gridPesquisaTurmaPersonalizadaFK = dijit.byId("gridPesquisaTurmaPersonalizadaFK");

        var horaI = 0, horaF = 24;
        var minI = 00, minF = 00;

        if (hasValue(gridPesquisaTurmaPersonalizadaFK) && hasValue(gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0])) {
            itemSelecionado = gridPesquisaTurmaPersonalizadaFK.itensSelecionados[0];
        } else {
            // Preenche "itemSelecionado" com objeto turma vindo do banco de dados.
            pesquisarTurmaFKByIdTurma();
        }

        if (hasValue(itemSelecionado)) {
            if (hasValue(itemSelecionado.horariosTurma[0].dt_hora_ini)) {
                horaI = parseInt(itemSelecionado.horariosTurma[0].dt_hora_ini.substring(0, 2));
                minI = parseInt(itemSelecionado.horariosTurma[0].dt_hora_ini.substring(3, 5));
                minI = minI == 0 ? "00" : minI;
            }
            if (hasValue(itemSelecionado.horariosTurma[0].dt_hora_fim)) {
                horaF = parseInt(itemSelecionado.horariosTurma[0].dt_hora_fim.substring(0, 2));
                minF = parseInt(itemSelecionado.horariosTurma[0].dt_hora_fim.substring(3, 5));
                minF = minF == 0 ? "00" : minF;
            }
        } 

        dijit.byId("hrObsInicial").constraints.min.setMinutes(minI);
        dijit.byId("hrObsInicial").constraints.min.setHours(horaI);
        dijit.byId("hrObsInicial").constraints.max.setHours(horaF);
        dijit.byId("hrObsFinal").constraints.max.setMinutes(minF);
        dijit.byId("hrObsFinal").constraints.min.setHours(horaI);
        dijit.byId("hrObsFinal").constraints.max.setHours(horaF);

    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}


function pesquisarTurmaFKByIdTurma() {

    dojo.xhr.get({
        preventCache: true,
        handleAs: "json",
        url: Endereco() + "/api/turma/getTurmasPersonalizadas?cdProduto=" + dijit.byId('pesCadProduto').value + "&dtAula=" + dojo.byId("dtIniAula").value + "&hrIni=" + dojo.byId("hrInicial").value + "&hrFim=" + dojo.byId("hrFinal").value
        + "&cd_turma=" + dojo.byId("cdTurma").value,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        sync: true
    }).then(function (data) {
        try {
            if (hasValue(data)) {
                itemSelecionado =  data[0];
            }
            else {
                itemSelecionado = null;
            }
        }
        catch (e) {
            itemSelecionado = null;
            postGerarLog(e);
        }
    });
}


function postObsAulaLancada() {
    if (!dijit.byId("formDialog").validate()) return false;
    showCarregando();
    var alunoSelecionado = dijit.byId("gridAluno").itensSelecionados[0];
    var dataLancaDiario = {
        cd_diario_aula: alunoSelecionado.cd_diario_aula,
        tx_obs_aula: dijit.byId("textAreaObs").value
    }
    //if (!hasValue(alunoSelecionado.cd_diario_aula) || alunoSelecionado.cd_diario_aula <= 0) {
    alunoSelecionado.tx_obs_aula = dijit.byId("textAreaObs").value;
    alunoSelecionado.hh_inicial_aluno = dijit.byId("hrObsInicial").displayedValue.replace("__:__", "");
    alunoSelecionado.hh_final_aluno = dijit.byId("hrObsFinal").displayedValue.replace("__:__", "");

    dijit.byId("gridAluno").update();
    showCarregando();
    dijit.byId("dialogGrid").hide();
}


function DeletarAulaPersonalizada(itensSelecionados) {
    apresentaMensagem("apresentadorMensagemAulaPersonalizada", '');
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_aula_personalizada').value != 0)
                    itensSelecionados = [{
                        cd_aula_personalizada: dom.byId("cd_aula_personalizada").value
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteAulaPersonalizada",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItens");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadAulaPersonalizada").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridAulaPersonalizada').itensSelecionados, "cd_aula_personalizada", itensSelecionados[r].cd_aula_personalizada);

                    pesquisarAulaPersonalizada(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarAulaPersonalizada").set('disabled', false);
                    dijit.byId("relatorioAulaPersonalizada").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                //apresentaMensagem(msg, error);
                if (!hasValue(dojo.byId("cadAulaPersonalizada").style.display == ""))
                    apresentaMensagem('apresentadorMensagem', error);
                else
                    apresentaMensagem('apresentadorMensagemAulaPersonalizada', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}


function eventoRemoverAulaPersonalizada(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { DeletarAulaPersonalizada(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}