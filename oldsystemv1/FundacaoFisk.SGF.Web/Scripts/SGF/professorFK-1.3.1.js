var FUNCIONARIO = 1, PESQUISAPROFESSOR = 2;

//#region Métodos auxiliares do formulario - carregarMascara() 
function carregarMascaraCPFProf() {
    try{
        $("#cpfProf").mask("999.999.999-99");
    } catch (e) {
        postGerarLog(e);
    }
}

function desabilitaCampo(status) {
    try{
        if (status == 0)
            dijit.byId("statusProf").set("disabled", true);
    } catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaProfessorFK() {
    try{
        dojo.byId("nomeProf").value = "";
        dojo.byId("nomeRed").value = "";
        dojo.byId("inicioProf").checked = false;
        dijit.byId("statusProf").set("value", 0);
        dijit.byId("nm_sexo_prof").set("value", 0);
        dojo.byId('cpfProf').value = "";
        if (hasValue(dijit.byId("gridPesquisaProfessor")) && hasValue(dijit.byId("gridPesquisaProfessor").itensSelecionados))
            dijit.byId("gridPesquisaProfessor").itensSelecionados = [];
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region criando grade da fk - montarGridPesquisaAluno() - formatCheckBoxAlunoFK

function formatCheckBoxProfessorFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaProfessor';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosProfessorFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_funcionario", grid._by_idx[rowIndex].item.cd_funcionario);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_funcionario', 'selecionadoProfessorFK', -1, 'selecionaTodosProfessorFK', 'selecionaTodosProfessorFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_funcionario', 'selecionadoProfessorFK', " + rowIndex + ", '" + id + "', 'selecionaTodosProfessorFK', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarGridPesquisaProfessor(especializada, pFuncao) {
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        "dijit/registry",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dijit/form/Button",
       "dojo/on"
    ], function (ready, xhr, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, on) {
        ready(function () {
            try{
                var store = null
                if (!especializada) {
                    var myStore = Cache(
                      JsonRest({
                          target: Endereco() + "/api/funcionario/GetFuncionarioSearch?nome=&apelido=&status=" + parseInt(0) + "&cpf=" + "&inicio=false&tipo=" + PESQUISAPROFESSOR + "&sexo=0",
                          handleAs: "json",
                          preventCache: true,
                          headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                      }
                  ), Memory({}));
                    store = new ObjectStore({ objectStore: myStore });
                } else
                    store = new ObjectStore({ objectStore: new Memory({ data: null }) });

            
                //*** Cria a grade de Cursos **\\
                var gridPesquisaProfessor = new EnhancedGrid({
                    store: store,
                    structure:
                            [
                                { name: "<input id='selecionaTodosProfessorFK' style='display:none'/>", field: "selecionadoProfessorFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProfessorFK },
                               //{ name: "Código", field: "cd_aluno", width: "50px", styles: "min-width:50px; text-align: left;" },
                                { name: "Nome", field: "no_pessoa", width: "25%", styles: "min-width:80px;" },
                                { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "15%" },
                                { name: "CPF", field: "nm_cpf_cgc", width: "5%px" },
                                { name: "Cargo", field: "des_cargo", width: "20%", styles: "120px" },
                                { name: "Data Admissão", field: "dta_cadastro", width: "15%", styles: "min-width:80px;" },
                                { name: "Ativo", field: "id_ativo", width: "8%", styles: "text-align: center; min-width:50px; max-width: 50px;" }
                            ],
                    noDataMessage: "Nenhum registro encontrado.",
                    selectionMode: "single",
                    plugins: {
                        //nestedSorting: true,
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
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridPesquisaProfessor");
           
                gridPesquisaProfessor.pagination.plugin._paginator.plugin.connect(gridPesquisaProfessor.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaProfessor, 'cd_professor', 'selecionaTodosProfessorFK');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaProfessor, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosProfessorFK').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_professor', 'selecionadoProfessorFK', -1, 'selecionaTodosProfessorFK', 'selecionaTodosProfessorFK', 'gridPesquisaProfessor')", gridPesquisaProfessor.rowsPerPage * 3);
                    });
                });

                gridPesquisaProfessor.canSort = function (col) { return Math.abs(col) == 2 };
                gridPesquisaProfessor.startup();

                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); if(!especializada) pesquisarProfessorFK(true); },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarProfessorFK");
                decreaseBtn(document.getElementById("pesquisarProfessorFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try{
                            if (hasValue(retornarProfessorFK)) {
                                dojo.byId("ehSelectGradeProfessorFK").value = false;
                                retornarProfessorFK();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaProfessorFK");
            
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proProfessor").hide(); }
                }, "fecharProfessorFK");
                loadPesqSexo(Memory, dijit.byId("nm_sexo_prof"));
            criarOuCarregarCompFiltering("statusProf", [{ name: "Ativos", id: "1" }, { name: "Inativos", id: "2" }], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                        'id', 'name', MASCULINO);
            //montarStatus("statusProf");
                carregarMascaraCPFProf();
                adicionarAtalhoPesquisa(['nomeProf', 'nomeRed', 'cpfProf', 'statusProf', 'nm_sexo_prof'], 'pesquisarProfessorFK', ready);
            if (hasValue(pFuncao))
                pFuncao.call();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#endregion

//#region métodos para  o aluno - pesquisarAlunoFK
function pesquisarProfessorFK(trazerProf) {
    try{
        //Caso tenha horários, verifica se existe horários marcados:
        var trHorarios = document.getElementById('trHorarios');

        if(trHorarios.style.display == 'none' || (trHorarios.style.display != 'none' && dijit.byId('cbHorarios').validate())){
            var sexo = hasValue(dijit.byId("nm_sexo_prof").value) ? dijit.byId("nm_sexo_prof").value : 0;
        var status = hasValue(dijit.byId("statusProf").value) ? dijit.byId("statusProf").value : 0;
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try{
                    var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/funcionario/GetFuncionarioSearch?nome=" + dojo.byId("nomeProf").value + "&apelido=" + dojo.byId("nomeRed").value + "&status=" + parseInt(status) +
                    "&cpf=" + dojo.byId("cpfProf").value + "&inicio=" + document.getElementById("inicioProf").checked + "&tipo=" + PESQUISAPROFESSOR + "&sexo=" + dijit.byId("nm_sexo_prof").value + "&cdAtividade=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_professor"
                    }), Memory({ idProperty: "cd_professor" }));
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var gridProfessor = dijit.byId("gridPesquisaProfessor");
                    gridProfessor.itensSelecionados = [];
                    gridProfessor.setStore(dataStore);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion