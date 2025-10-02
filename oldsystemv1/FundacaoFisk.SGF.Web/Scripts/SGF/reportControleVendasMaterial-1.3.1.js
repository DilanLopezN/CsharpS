var TODOS = 0;
var SAIDA = 2;

function montarControleVendasMaterial() {
    require([
"dojo/ready",
"dojo/_base/xhr",
"dojo/store/Memory",
"dojo/on",
"dijit/form/Button",
"dojo/window"
    ], function (ready, xhr, Memory, on, Button, window) {
        ready(function () {
            sugerirDatas();

            new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(window); } }, "rptContVendasMaterial");            
            showCarregando();
            
            // Partial Aluno
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
                        limparItemMaterial();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "fkAluno");

            new Button({
                label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                    dojo.byId("cdAlunoFK").value = 0;
                    dojo.byId("descAlunoFK").value = "";
                    dijit.byId("limparAluno").set('disabled', true);
                    limparItemMaterial();
                }
            }, "limparAluno");
            
            decreaseBtn(document.getElementById("fkAluno"), '18px');
            decreaseBtn(document.getElementById("limparAluno"), '40px');

            // Partial Material
            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    try {
                        apresentaMensagem('apresentadorMensagem', null);
                        if (!dijit.byId("formContVendasMaterial").validate())
                            return false;

                        document.getElementById("tipoFKItem").value = "pesq";
                        if (!hasValue(dijit.byId("pesquisarItemFK"))) {
                            montargridPesquisaItem(function () {
                                abrirItemFK();
                                dijit.byId("pesquisarItemFK").on("click", function (e) {
                                    apresentaMensagem("apresentadorMensagemItemFK", null);
                                    pesquisarItemFK(true);
                                });
                                dijit.byId("fecharItemFK").on("click", function (e) {
                                    dijit.byId("itemFK").hide();
                                });
                            }, true, true);
                        } else {
                            abrirItemFK();
                            dijit.byId("fecharItemFK").on("click", function (e) {
                                dijit.byId("itemFK").hide();
                            });
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "fkItem");

            new Button({
                label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                    limparItemMaterial();
                }
            }, "limparItemFK");

            decreaseBtn(document.getElementById("fkItem"), '18px');
            decreaseBtn(document.getElementById("limparItemFK"), '40px');

            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    try {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                funcaoFKTurma();
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
                            funcaoFKTurma();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "fkTurma");

            new Button({
                label: "Limpar", iconClass: '', type: "reset", disabled: true,
                onClick: function () {
                    dojo.byId("noTurmaPesq").value = "";
                    dojo.byId("cd_turma").value = 0;
                    dijit.byId("limparTurma").set('disabled', true);
                    if(!dijit.byId('semmaterial').checked)
                        dojo.byId('paneDataInicial').style.display = "";
                    apresentaMensagem('apresentadorMensagem', null);
                }
            }, "limparTurma");

            decreaseBtn(document.getElementById("fkTurma"), '18px');
            decreaseBtn(document.getElementById("limparTurma"), '40px');

            dijit.byId("semmaterial").on("change", function (e) {
                if(e){
                    dojo.byId('paneDataInicial').style.display = "none";
                    if(!hasValue(dijit.byId('dtInicial').value) || !hasValue(dijit.byId('dtFinal').value))
                        sugerirDatas();
                }
                else
                    if(dojo.byId("cd_turma").value == 0)
                        dojo.byId('paneDataInicial').style.display = "";
            });

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

        });
    });
}

function sugerirDatas() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataInicialEFinalMes",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            if (hasValue(dataCorrente) && hasValue(dataCorrente.dt_ini_mes)) {
                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", dataCorrente.dt_ini_mes);
                dijit.byId('dtInicial').oldValue = dijit.byId('dtInicial').value;
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", dataCorrente.dt_fim_mes);
                dijit.byId('dtFinal').oldValue = dijit.byId('dtFinal').value;
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function emitirRelatorio(windowUtils) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formContVendasMaterial").validate())
            return false;
        var cd_aluno = hasValue(dojo.byId("cdAlunoFK").value) ? dojo.byId("cdAlunoFK").value : 0;
        var cd_item = hasValue(dojo.byId("cd_item_pesq").value) ? dojo.byId("cd_item_pesq").value : 0;
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var no_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("noTurmaPesq").value : "";

        dojo.xhr.get({
            url: Endereco() + "/api/aluno/GetUrlRptContVendasMaterial?cd_aluno=" + cd_aluno + "&cd_item=" + cd_item +
                "&dt_inicial=" + dojo.byId('dtInicial').value + "&dt_final=" + dojo.byId('dtFinal').value +
                "&cd_turma=" + cd_turma + "&semmaterial=" + dijit.byId('semmaterial').checked +
                "&pNoTurma=" + no_turma,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioContVendasMaterial?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFK() {
    try {
        pesquisarItemFK();
        showP('comEstoqueTitulo', true);
        showP('comEstoqueCampo', true);
        dijit.byId("itemFK").show();
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("tipo").set("disabled", true);
        dijit.byId("statusItemFK").set("disabled", true);
        setTimeout(function () { dijit.byId("tipo").set('value', 1); }, 100);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemFK() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            var tipoItemInt = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : null;
            var id_natureza = 0;
            var dt_inicial = dojo.byId('dtInicial').value;
            var dt_final = dojo.byId('dtFinal').value;

            myStore = Cache(
               JsonRest({
                   target: Endereco() + "/fiscal/getItemMovimentoSearchVendaMaterial?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" +
                                        retornaStatus("statusItemFK") + "&tipoItemInt=" + 1 + "&grupoItem=" + grupoItem + "&id_tipo_movto=" + SAIDA + "&comEstoque=" + document.getElementById("comEstoque").checked + "&id_natureza_TPNF=" + SAIDA +
                                        "&cd_aluno=" + dojo.byId("cdAlunoFK").value + "&dt_inicial=" + dt_inicial + "&dt_final=" + dt_final,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_item"
               }
            ), Memory({ idProperty: "cd_item" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaItem");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirAlunoFK() {
    try {
        //Configuração retorno fk de aluno na matrícula
        dojo.byId('tipoRetornoAlunoFK').value = RELTURMAMATRICULAMATERIAL;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        getAlunoOrigemMaterial(true);
        dijit.byId("proAluno").show();
        apresentaMensagem('apresentadorMensagem', null);
    } catch (e) {
        postGerarLog(e);
    }
}

function getAlunoOrigemMaterial(pesquisaHabilitada) {
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
                    myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/getAlunoOrigemVendaMaterial?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_pessoa_responsavel=0",
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

//Aluno FK
function retornarAlunoFK() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else {
            dojo.byId("cdAlunoFK").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("descAlunoFK").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
            dijit.byId("proAluno").hide();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarItemFK() {
    try {
        var gridPesquisaItem = dijit.byId("gridPesquisaItem");
        if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesquisaItem.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {
            if (document.getElementById("tipoFKItem").value == "pesq") {
                dojo.byId("cd_item_pesq").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                dojo.byId("item_pesq").value = gridPesquisaItem.itensSelecionados[0].no_item;
            }
            else {
                dojo.byId("cd_item").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                dojo.byId("no_item").value = gridPesquisaItem.itensSelecionados[0].no_item;

                if (gridPesquisaItem.itensSelecionados[0].qt_estoque <= 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgAvisoItemSemEstoque);
                    apresentaMensagem("apresentadorMensagemEmprestimo", mensagensWeb);
                }
            }
            dijit.byId('limparItemFK').set('disabled', false);
            dijit.byId("itemFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparItemMaterial() {
    dojo.byId('cd_item_pesq').value = '';
    dojo.byId("item_pesq").value = "";
    dijit.byId('limparItemFK').set('disabled', true);
    limparItensSelecionados(function () { });
}

//Turma
function funcaoFKTurma() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = REPORT_TURMA_MATRICULA_MATERIAL;
        dijit.byId("proTurmaFK").show();
        dijit.byId("tipoTurmaFK").store.remove(0);

        pesquisarTurmaFKRelTurmaMatriculaMaterial();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == REPORT_TURMA_MATRICULA_MATERIAL) {
            var valido = true;
            var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
            if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            }
            else {
                dojo.byId("cd_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("noTurmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurma').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
            dojo.byId('paneDataInicial').style.display = "none";
            if(!hasValue(dijit.byId('dtInicial').value) || !hasValue(dijit.byId('dtFinal').value))
                sugerirDatas();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaFKRelTurmaMatriculaMaterial(cdProfDefault) {
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
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchFKRelTurmaMatriculaMaterial?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno +
                                            "&dtInicial=&dtFinal=&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=&semmaterial=" + dijit.byId('semmaterial').checked,
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
