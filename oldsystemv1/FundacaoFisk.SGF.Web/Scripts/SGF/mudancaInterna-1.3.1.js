var MUDANCAINTERNAORI = 1, MUDANCAINTERNADESTINO = 2;
var produto_origem = 0, qtd_vagas_dest = 0, qtd_aluno_turma_dest, curso_origem, curso_dest, dt_inicio = "";
var verifica_vaga = false, id_ppt_origem = false, turma_origem_encerrada = false;
var MUDARTURMA = 1, RETORNARTURMA = 2;
var listaContratosAlunos = [];
var TIPOFINANCHEQUE = 4,  CHEQUEPREDATADO = 4, CHEQUEVISTA = 10;
var isViewChequeTransacao = null;
var dt_ultima_aula = new Date();
function montarCadastroMudanca(Permissoes) {
    //Criação da Grade de sala
    require([
        "dojo/_base/xhr",
        "dojo/store/JsonRest",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DateTextBox",
        "dojox/json/ref",
        "dojo/data/ObjectStore",
        "dojox/grid/EnhancedGrid"
    ], function (xhr, JsonRest, Memory, Button, ready, DateTextBox, ref, ObjectStore, EnhancedGrid) {
        ready(function () {
            try {
                document.getElementById("tgCheque").style.display = "none";
                this.loadAcaoDesistenciaTitulos();

                var gridTitulo = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodosTitulos' style='display:none' onclick=''/>", field: "tituloSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckboxTitulo },
                        { name: "Aluno", field: "nomeAluno", width: "15%", styles: "min-width:180px;" },
                        { name: "Responsável", field: "nomeResponsavel", width: "15%", styles: "min-width:180px;" },
                        { name: "Titulo", field: "nm_titulo", width: "8%", styles: "min-width:180px;" },
                        { name: "Tipo Finan.", field: "tipoDoc", width: "6%", styles: "min-width:180px;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:180px; text-align: center;" },
                        { name: "Emissão", field: "dt_emissao", width: "6%", styles: "min-width:180px;" },
                        { name: "Vencto.", field: "dt_vcto", width: "6%", styles: "min-width:180px;" },
                        { name: "Valor", field: "vlTitulo", width: "7%", styles: "min-width:100px;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "7%", styles: "min-width:100px;" },
                        {
                            name: "Cheque",
                            field: "_item",
                            width: '100px', 
                            styles: "text-align: center;",
                            formatter: function(item) {
                                var label = "Adicionar";

                                if(hasValue(item.Cheque) && hasValue(item.Cheque.dt_bom_para))
                                    label = "Alterar";    

                                if(hasValue(dijit.byId(item.cd_titulo+"_"+item.nm_parcela_titulo))){
                                    dijit.byId(item.cd_titulo+"_"+item.nm_parcela_titulo).destroy();
                                }

                                var btn = new dijit.form.Button({
                                    label: label,
                                    id: item.cd_titulo+"_"+item.nm_parcela_titulo,
                                    disabled: desabilitarBotaoIncluirCheque(null, null, true),
                                    onClick: function() { 
                                        try {
                                            dijit.byId('proChequeFK').set('title', 'Dados do cheque do Título: ' + item.nm_titulo + ' Parcela ' + item.nm_parcela_titulo);
                                            showViewCheque(item,true);
                                        } catch (e) {
    
                                        }
                                    }
                                });
                                return btn;
                            }
                        }
                    ],
                    canSort: true,
                    selectionMode: "single",
                }, "gridTitulo");
                gridTitulo.startup();
                gridTitulo.on("RowClick", function (evt) {                                                
                    EVENTO_GRID_CHEQUE = evt;
                }, true);
                gridTitulo.layout.setColumnVisibility(10, false);

                if (hasValue(dojo.byId("tabContainer_tablist_tabTitulo"))) {
                    dojo.byId("tabContainer_tablist_tabTitulo").parentElement.id = "paiTabTitulo";
                    dojo.byId("paiTabTitulo").style.display = "none";
                }
                dijit.byId("tabContainer").resize();
                dojo.byId("check_4").style.display = "none";
                new Button({
                    label: "Salvar", disabled: "true", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        salvarMudanca(xhr, ref, JsonRest);
                    }
                }, "salvarMudanca");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () {
                        limparMudancasInternas();
                    }
                }, "limparMudanca");

                new Button({
                    label: "", title: "Tranferir aluno selecionado", iconClass: 'dijitEditorIcon dijitEditorIconUndo', onClick: function () {
                        transfereUmAluno(xhr, ref, Permissoes);
                    },
                }, "btnIncluir");
                new Button({
                    label: "", title: "Retornar aluno selecionado", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                        retornarUmAluno();
                    }
                }, "btnExcluir");
                new Button({
                    label: "", title: "Tranferir todos os alunos selecionados", iconClass: 'dijitEditorIcon dijitEditorIconInsertAll', onClick: function () {
                        transfereVariosAluno(xhr, ref, Permissoes);
                    }
                }, "btnIncluirTodos");
                new Button({
                    label: "", title: "Retornar todos os alunos selecionados", iconClass: 'dijitEditorIcon dijitEditorIconRemoveAll', onClick: function () {
                        retornarVariosAluno();
                    }
                }, "btnExcluirTodos");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', title: "Turma de destino", onClick: function () {
                        try{
                            
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                montarGridPesquisaTurmaFK(function () {
                                    funcaoFKTurmaDestino(Memory);
                                    dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                            montarGridPesquisaAluno(false, function () {
                                                abrirAlunoFKTurmaFK(true);
                                            });
                                        }
                                        else
                                            abrirAlunoFKTurmaFK();
                                    });
                                });
                            else
                                funcaoFKTurmaDestino(Memory);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkNovaTurma");
                dijit.byId("fkNovaTurma").set("disabled", true);
                new Button({
                    label: "", title: "Turma origem", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try{
                            loadTipoTurmaFK(Memory);
                            
                            loadSituacaoTurmaFKTodas(Memory);
                            limparFiltrosTurmaFK();
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                montarGridPesquisaTurmaFK(function () {
                                    funcaoFKTurmaOrigem(Memory);
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
                                funcaoFKTurmaOrigem(Memory);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkTurmaAndamento");

                var buttonFkArray = ["btnIncluir", "btnExcluir", "btnIncluirTodos", "btnExcluirTodos", 'fkNovaTurma', 'fkTurmaAndamento'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                dijit.byId("cbManterContrato").on("change", function (e) {
                    try{
                        if (e) {
                            dijit.byId('ckRenegociacao').set('disabled', true);
                            if (hasValue(dojo.byId("tabContainer_tablist_tabTitulo"))) {
                                dojo.byId("tabContainer_tablist_tabTitulo").parentElement.id = "paiTabTitulo";
                                dojo.byId("paiTabTitulo").style.display = "none";
                                resetAbaTitulosAbertos();
                            }
                        }
                        else {
                            dijit.byId('ckRenegociacao').set('disabled', false);
                            if (dojo.byId("alunosMudar").options.length > 0) {
                                if (hasValue(dojo.byId("tabContainer_tablist_tabTitulo"))) {
                                    dojo.byId("tabContainer_tablist_tabTitulo").parentElement.id = "paiTabTitulo";
                                    dojo.byId("paiTabTitulo").style.display = "";
                                    resetAbaTitulosAbertos();
                                }
                            }
                        }
                        limparTurmasAlunos();
                    //    dijit.byId("cdNovaTurma").set("value", "");
                    //    dojo.byId("cd_NovaTurma").value = 0;
                    //    dojo.byId('dt_inicio').value = "";
                    //    dijit.byId('dt_movimentacao').reset();
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                new Button({
                    label: "Limpar", disabled: "true", iconClass: '', onClick: function () {
                        try{
                            dijit.byId('limparTurmaAndamentoPes').set("disabled", true);
                            limparTurmasAlunos();
                            limparTitulosAbertos();

                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparTurmaAndamentoPes");
                if (hasValue(document.getElementById("limparTurmaAndamentoPes"))) {
                    document.getElementById("limparTurmaAndamentoPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurmaAndamentoPes").parentNode.style.width = '40px';
                }

                new Button({
                    label: "Limpar", disabled: "true", iconClass: '', onClick: function () {
                        limparTurmaNova();
                    }
                }, "limparNovaTurmaPes");
                if (hasValue(document.getElementById("limparNovaTurmaPes"))) {
                    document.getElementById("limparNovaTurmaPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparNovaTurmaPes").parentNode.style.width = '40px';
                }
                dijit.byId("alunosPermanecerao").on("click", function () {
                    try{
                        if (dijit.byId("cbMudancasInternas").value == RETORNARTURMA)
                            verificaTurmaOrigem(dijit.byId("alunosPermanecerao").value[0], xhr, ref);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId('cbCurso').set("required", false);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323059', '765px', '771px');
                        });
                }
                dijit.byId("pesTurmasFilhasFK").set("checked", false);
                //dijit.byId("tipoTurmaFK").on("change", function (e) {
                //    try{
                //        if (dojo.byId("idOrigenPesquisaTurmaMudInterna").value == MUDANCAINTERNAORI)
                //            dijit.byId("pesTurmasFilhasFK").set("checked", false);
                //        else
                //            dijit.byId("pesTurmasFilhasFK").set("checked", true);
                //    } catch (e) {
                //        postGerarLog(e);
                //    }
                //});

                dijit.byId("cbLiquidacaoCad").on("change", function (e) {
                    //hide
                    var gridTitulos = dijit.byId("gridTitulo");
                    gridTitulos.layout.setColumnVisibility(10, false);
                    if(hasValue(e)){
                        tipoFinanTituloTransacao(e);                  
                    }
                });
                dijit.byId("cbTurmaPPT").set('disabled', true);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
};

function configuraCheckBoxTitulos(value, field, fieldTodos, rowIndex, id, idTodos, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            var dojoId = dojo.byId(id);
            var grid = dijit.byId(gridName);

            if (hasValue(grid)){
                if (id != idTodos || (hasValue(grid.pagination) && !grid.pagination.plugin._showAll)) {

                    // Se id for seleciona todos, verifica se todos estão marcados para marcá-lo:
                    if (id == idTodos) {
                        var j = 0;
                        var campo = dojo.byId(fieldTodos + '_Selected_' + j);
                        value = hasValue(campo);

                        while (hasValue(campo) && value) {
                            if (campo.type == 'text') {
                                setTimeout("configuraCheckBox(" + value + ", '" + field + "', '" + fieldTodos + "', " + rowIndex + ", '" + id + "', '" + idTodos + "', '" + gridName + "')", grid.rowsPerPage * 3);
                                return;
                            }
                            else {
                                value = value && campo.checked;
                                j += 1;
                                campo = dojo.byId(fieldTodos + '_Selected_' + j);
                            }
                        }
                    }

                    if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                        dijit.byId(id).destroy();
                    if (value == undefined)
                        value = false;
                    if (disabled == null || disabled == undefined) disabled = false;
                    if (hasValue(dojoId) && dojoId.type == 'text')
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            checked: value,
                            disabled: disabled,
                            onChange: function (b) { checkBoxChangeTitulos(rowIndex, field, fieldTodos, idTodos, this, grid); }
                        }, id);
                }
                else if (hasValue(dojo.byId(idTodos)))
                    dojo.byId(idTodos).parentNode.removeChild(dojo.byId(idTodos));

            }
        })
    });
}

/* Funções para componentização de checkbox na grid paginada sob demanda: */
function checkBoxChangeTitulos(rowIndex, field, fieldTodos, idTodos, obj, grid) {
    var itemTodos = dijit.byId(idTodos);

    if (rowIndex != -1 && hasValue(grid.getItem(rowIndex))) {
        if (!hasValue(grid.itensSelecionados))
            grid.itensSelecionados = [];

        if (obj.checked) {            
            insertObjSort(grid.itensSelecionados, field, grid.getItem(rowIndex));
            verificaMarcacaoTodos(function () { marcaItem(itemTodos, true); }, fieldTodos, grid);

            //função
            desabilitarBotaoIncluirCheque(obj.checked, grid.getItem(rowIndex), false);
            tipoFinanTituloTransacao(CHEQUEPREDATADO);
        }
        else {
            //função
            desabilitarBotaoIncluirCheque(obj.checked, grid.getItem(rowIndex), false);
            tipoFinanTituloTransacao(CHEQUEPREDATADO);
            removeObjSort(grid.itensSelecionados, field, eval('grid.getItem(rowIndex).' + field));
            marcaItem(itemTodos, false);
        }        
    }
    else
        // Checa todos:
        selecionaTodos(fieldTodos, itemTodos.checked);
}

function loadAcaoDesistenciaTitulos() {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        var statusStoreTipo = new Memory({
            data: [
                { name: "Deixar todos os títulos em Aberto", id: "0" },
                { name: "Liquidar os títulos selecionados", id: "1" }
            ]
        });
        dijit.byId("cbEscolhaCad").store = statusStoreTipo;
        dijit.byId("cbEscolhaCad").on("change", function (e) {
            apresentaMensagem('apresentadorMensagem', null);
            if (e == 1) {
                document.getElementById("localMovto").style.display = "";
                dijit.byId('des_local').set('required', true);
                dijit.byId('cbLiquidacaoCad').set('required', true);
            }
            else{
                document.getElementById("localMovto").style.display = "none";
            }
        });
    });
}

function formatCheckboxTitulo(value, rowIndex, obj) {
    var gridName = 'gridTitulo';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosTitulos');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px' id='" + id + "'/> ";

    setTimeout("configuraCheckBoxTitulos(" + value + ", 'cd_titulo', 'tituloSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosTitulos', '" + gridName + "')", 2);

    return icon;
}

function selecionaTab(e) {
    var tab = dojo.query(e.target).parents('.dijitTab')[0];

    if (!hasValue(tab)) // Clicou na borda da aba:
        tab = dojo.query(e.target)[0];
}

function mudaOpcao(opcao) {
    try{
        switch (parseInt(opcao.value)) {
            case MUDARTURMA:
                showP('obs_1', true);
                showP('obs_2', false);

                showP('lblDataInicio', true);
                showP(dojo.byId('dt_inicio').parentNode.parentNode.id, true);

                document.getElementById('fkNovaTurma').parentNode.parentNode.style.display = '';
                if (dojo.byId('cd_TurmaAndamento').value > 0)
                    dijit.byId("fkNovaTurma").set("disabled", false);
                dijit.byId('cbManterContrato').set('disabled', false);
                dijit.byId('cbManterContrato').set('checked', false);
                dijit.byId('ckRenegociacao').set('disabled', false);
                document.getElementById('lblNovaTurma').innerHTML = 'Turma em andamento existente:';
                document.getElementById('lblTurmaEmAndamento').innerHTML = 'Turma a alterar:';
                limparTurmasAlunos();

                break;
            case RETORNARTURMA:
                showP('obs_1', false);
                showP('obs_2', true);

                showP('lblDataInicio', false);
                showP(dojo.byId('dt_inicio').parentNode.parentNode.id, false);
                document.getElementById('fkNovaTurma').parentNode.parentNode.style.display = '';
                dijit.byId("fkNovaTurma").set("disabled", true);
                dijit.byId('cbManterContrato').set('checked', true);
                dijit.byId('cbManterContrato').set('disabled', true);
                mudarPPT(dijit.byId('cbTurmaPPT'));
                mudarManter(dijit.byId('cbManterContrato'));
                document.getElementById('lblNovaTurma').innerHTML = 'Turma Original:';
                document.getElementById('lblTurmaEmAndamento').innerHTML = 'Turma Atual:';
                limparTurmasAlunos();
                break;
            default:
                dijit.byId('cbTurmaPPT').set('disabled', true);
                dijit.byId('cbTurmaPPT').set('checked', false);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function mudarPPT(obj) {
    try{
        if (obj.checked) {
            var cd_curso = null;
            if (dijit.byId('cbManterContrato').checked)
                cd_curso = curso_origem;
            carregarCursoPorProdutoMudanca(produto_origem, cd_curso);
            document.getElementById('lblCurso').style.display = '';
            dojo.byId('cbCurso').parentNode.parentNode.style.display = '';
            dijit.byId('cbCurso').set("required", true);
            document.getElementById('lblNovaTurma').innerHTML = 'Turma Personalizada Pai:';
            dijit.byId("dt_inicio").set("disabled", true);
            limparTurmaNova();
            if (dijit.byId('cbManterContrato').checked) {
                dojo.byId('dt_inicio').value = dt_inicio;
                if (hasValue(dt_ultima_aula)) {
                    var time = new Date(dt_ultima_aula);
                    var d = time.getDate();
                    var dt = new Date(time.setDate(d + 1));
                    dijit.byId('dt_movimentacao').set("value", hasValue(dt_ultima_aula) ? dt : dojo.date.locale.parse(dt_inicio, { formatLength: 'short', selector: 'date', locale: 'pt-br' }))
                }
            }
            dijit.byId('fkTurmaAndamento').set('disabled',true)
        }
        else {
            document.getElementById('lblCurso').style.display = 'none';
            dojo.byId('cbCurso').parentNode.parentNode.style.display = 'none';
            dijit.byId('cbCurso').set("required", false);
            document.getElementById('lblNovaTurma').innerHTML = 'Turma em andamento existente:';
            dijit.byId("dt_inicio").set("disabled", true);
            dijit.byId('dt_inicio').reset();
            dijit.byId("cdNovaTurma").set("value", "");
            dojo.byId("cd_NovaTurma").value = 0;
            dijit.byId('fkTurmaAndamento').set('disabled', false)
            dijit.byId("dt_movimentacao").reset();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function mudarManter(obj) {
    try{
        if (obj.checked) {
            dijit.byId('ckRenegociacao').set('disabled', true);
            dijit.byId('ckRenegociacao').set('checked', false);
        }
        else {
            dijit.byId('ckRenegociacao').set('disabled', false);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKTurmaOrigem(Memory) {
    try{
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de turma.
        dojo.byId("idOrigemCadastro").value = CADMUDANCAINTERNAORI;
        //Configuração retorno fk de turma dentro da tela de mudança interna.
        dojo.byId("idOrigenPesquisaTurmaMudInterna").value = MUDANCAINTERNAORI;
        loadTipoTurmaFK(Memory);
        loadSituacaoTurmaFKTodas(Memory);
        limparFiltrosTurmaFK();
        limparTurmasAlunos();
        dijit.byId("proTurmaFK").show();
        dijit.byId('tipoTurmaFK').set('value', 1);
        dijit.byId('pesSituacaoFK').set('value', 1);
        dijit.byId('pesTurmasFilhasFK').set("disabled", true);
        dijit.byId('pesTurmasFilhasFK').set("checked", false);
        loadSituacaoTurmaFKTodas(dojo.store.Memory);
        dijit.byId("pesSituacaoFK").costumizado = true;
        pesquisarTurmaFKMudanca(MUDANCAINTERNAORI);
    } catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKTurmaDestino(Memory) {
    try{
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de turma dentro da tela de mudança interna.
        dojo.byId("idOrigenPesquisaTurmaMudInterna").value = MUDANCAINTERNADESTINO;
        //Configuração retorno fk de turma.
        dojo.byId("idOrigemCadastro").value = CADMUDANCAINTERNADES;
        limparFiltrosTurmaFK();
        limparTurmaNova();
        //dijit.byId('tipoTurmaFK')._onChangeActive = false;
        loadTipoTurmaFK(Memory);
        loadSituacaoTurmaFKAbertas(Memory);
        dijit.byId("pesSituacaoFK").costumizado = true;
        dojo.byId("lblTurmaFilhasFK").style.display = "none";
        dojo.byId("divPesTurmasFilhasFK").style.display = "none";
        dijit.byId("pesProdutoFK").set("disabled", true);
        dijit.byId('pesTurmasFilhasFK').set("checked", false);
        dijit.byId('pesProdutoFK').set('value', produto_origem);
        if (hasValue(curso_origem) && dijit.byId('cbManterContrato').checked && !dijit.byId("cbTurmaPPT").checked) {
            dijit.byId("pesCursoFK").set("disabled", true);
            dijit.byId('pesCursoFK').set('value', curso_origem);
        }
        else
            dijit.byId("pesCursoFK").set("disabled", false);

        if (!dijit.byId("cbTurmaPPT").checked)
            dijit.byId('tipoTurmaFK').set('value', 1);
        else
            dijit.byId('tipoTurmaFK').set('value', 3);
        dijit.byId("tipoTurmaFK").set("disabled", true);
        dijit.byId('pesSituacaoFK').set('value', 1);
       // dijit.byId("pesSituacaoFK").set("disabled", true);
      //  dijit.byId('tipoTurmaFK')._onChangeActive = true;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFKMudanca(MUDANCAINTERNADESTINO);

    } catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    try{
        //Configuração retorno fk de turma dentro da tela de mudança interna.
        if (dojo.byId("idOrigenPesquisaTurmaMudInterna").value == MUDANCAINTERNAORI)
            retornarTurmaFKMudancaInternaOri();
        else
            retornarTurmaFKMudancaInternaDest();
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKMudancaInternaOri() {
    try{
        apresentaMensagem("apresentadorMensagem", null);
        var itemTurma = dijit.byId("gridPesquisaTurmaFK").itensSelecionados[0];
        if (dijit.byId('cbMudancasInternas').value == MUDARTURMA)
        dijit.byId("cbTurmaPPT").set('disabled', false);


        require(["dojo/_base/xhr"],
            function (xhr) {
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
                if (dijit.byId("cbMudancasInternas").value == MUDARTURMA)
                    dijit.byId("fkNovaTurma").set("disabled", false);
                    xhr.get({
                        url: Endereco() + "/api/aluno/getAlunoPorTurma?cdTurma=" + itemTurma.cd_turma + "&opcao=" + dijit.byId('cbMudancasInternas').value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
                            listaContratosAlunos = [];

                            for (var i = 0; i < listaAlunosOri.length; i++)
                                listaAlunosOri[i] = null;
                            var dataAlunoMudar = jQuery.parseJSON(data).retorno;
                            dojo.byId('cd_TurmaAndamento').value = itemTurma.cd_turma;
                            //dijit.byId("cbTurmaAndamento").set("value", itemTurma.cd_turma);
                            dojo.byId("cbTurmaAndamento").value = itemTurma.no_turma;
                            turma_origem_encerrada = hasValue(itemTurma.dta_termino_turma) ? true : false;
                            id_ppt_origem = itemTurma.id_turma_ppt;
                            produto_origem = itemTurma.cd_produto;
                            curso_origem = itemTurma.cd_curso;
                            dt_inicio = itemTurma.dtaIniAula;
                            dt_ultima_aula = itemTurma.dt_ultima_aula;
                            // Ordernar lista pelo nome do aluno.
                            OrdernarListaAluno(dataAlunoMudar);

                            for (var i = 0; i < dataAlunoMudar.length; i++) {
                                var idade = "00";

                                if (hasValue(dataAlunoMudar[i].idade)) {
                                    if (dataAlunoMudar[i].idade < 10)
                                        idade = "0" + dataAlunoMudar[i].idade;
                                    else
                                        idade = dataAlunoMudar[i].idade;
                                }
                                listaAlunosOri[i] = new Option(idade + " - " + dataAlunoMudar[i].nomeAluno, dataAlunoMudar[i].cd_aluno);
                                listaContratosAlunos.push(
                                    {
                                        'cd_aluno': dataAlunoMudar[i].cd_aluno,
                                        'cd_contrato': dataAlunoMudar[i].cd_contrato,
                                        'cd_pessoa_aluno': dataAlunoMudar[i].cd_pessoa_aluno
                                    }
                                  );
                            }

                            //zerar lista de Alunos destinos
                            var listaAlunosMudar = dojo.byId("alunosMudar").options;
                            if (listaAlunosMudar.length > 0) {
                                for (var i = 0; i < listaAlunosMudar.length; i++)
                                    listaAlunosMudar[i] = null;
                                if (hasValue(dijit.byId("cdNovaTurma").value))
                                    dijit.byId("salvarMudanca").set('disabled', false);
                            }
                            dijit.byId('limparTurmaAndamentoPes').set("disabled", false);

                            if (dijit.byId("cbTurmaPPT").checked) {
                                var cd_curso = null;
                                if (dijit.byId('cbManterContrato').checked)
                                    cd_curso = curso_origem;
                                carregarCursoPorProdutoMudanca(produto_origem, cd_curso);
                            }

                            dijit.byId("proTurmaFK").hide();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem("apresentadorMensagem", error);
                    });
                }
                if (!valido)
                    return false;

            });
    } catch (e) {
        postGerarLog(e);
    }
}

function OrdernarListaAluno(dataAlunoMudar) {
    if (hasValue(dataAlunoMudar)) {
        dataAlunoMudar.sort(function (a, b) {
            if (a.nomeAluno > b.nomeAluno) {
                return 1;
            }
            if (a.nomeAluno < b.nomeAluno) {
                return -1;
            }
            return 0;
        });
    }
}

function retornarTurmaFKMudancaInternaDest() {
    try{
        var itemTurma = dijit.byId("gridPesquisaTurmaFK").itensSelecionados[0];
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
            dijit.byId('tipoTurmaFK')._onChangeActive = true;

            dojo.byId("cd_NovaTurma").value = itemTurma.cd_turma;
            dojo.byId("cdNovaTurma").value = itemTurma.no_turma;
            if (!dijit.byId('cbManterContrato').checked || !dijit.byId('cbTurmaPPT').checked)
                dojo.byId("dt_inicio").value = itemTurma.dtaIniAula;

            if (!dijit.byId('cbTurmaPPT').checked) {
                dijit.byId('cbManterContrato')._onChangeActive = false;
                dijit.byId('cbManterContrato').set("checked", (itemTurma.cd_curso == curso_origem));
                dijit.byId('cbManterContrato')._onChangeActive = true;
            }
            qtd_vagas_dest = itemTurma.vagas_disponiveis;
            qtd_aluno_turma_dest = itemTurma.nro_alunos;
            verifica_vaga = itemTurma.considera_vagas;

            if (itemTurma['cd_curso'] != null && itemTurma['cd_curso'] != undefined) {
                curso_dest = itemTurma.cd_curso;
            } else {
                curso_dest = null;
            }
            
            var listaAlunosMudar = dojo.byId("alunosMudar").options;
            if (listaAlunosMudar.length > 0) {
                for (var i = 0; i < listaAlunosMudar.length; i++)
                    listaAlunosMudar[i] = null;
                if (hasValue(dijit.byId("cbMudancasInternas").value))
                    dijit.byId("salvarMudanca").set('disabled', false);
            }
            dijit.byId('limparNovaTurmaPes').set("disabled", false);
            dijit.byId("proTurmaFK").hide();

        }
        if (!valido)
            return false;

    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaFKMudanca(origem_destino) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            var cdTurma = 0;
            if (origem_destino == MUDANCAINTERNADESTINO)
                cdTurma = dojo.byId("cd_TurmaAndamento").value;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)

            /*--combo_escola_fk
            //some a combo de escola
            dojo.byId("trEscolaTurmaFiltroFk").style.display = "none";
            dojo.byId("lbEscolaTurmaFiltroFk").style.display = "none";
            require(['dojo/dom-style', 'dijit/registry'],
                function (domStyle, registry) {

                    domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', 'none');
                });*/

            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchComAluno?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value + "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor + "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked
                                            + "&cdAluno=" + cdAluno + "&dtInicial=&dtFinal=&cd_turma_PPT=null&cdTurmaOri=" + cdTurma + "&opcao=" + dijit.byId('cbMudancasInternas').value + "&cd_escola_combo_fk=0",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];

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
            grid.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function transfereUmAluno(xhr, ref, Permissoes) {
    try{
        apresentaMensagem("apresentadorMensagem", null);
        var valorMudar = dijit.byId("alunosPermanecerao").value;
        var valido = true;
        if (dojo.byId("cd_NovaTurma").value <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgInfEscolherTurmaDest);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }
        if (!hasValue(valorMudar) || valorMudar.length <= 0) {
            if (valorMudar != null && valorMudar.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgNotSelectReg);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
            }
            return false;
        }

        verificaAlunosTurma(xhr, ref, function () {
            if (dojo.byId("cd_NovaTurma").value <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgInfEscolherTurmaDest);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }
            var listaAlunosDest = dojo.byId("alunosMudar").options;
            var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
            var cdAlunos = new Array();
            for (var i = 0; i < dijit.byId("alunosPermanecerao").value.length; i++) {
                cdAlunos[i] = dijit.byId("alunosPermanecerao").value[i];
            }
            if (dijit.byId("cbMudancasInternas").value == RETORNARTURMA)
                verificaTurmasOrigem(cdAlunos, xhr, ref, false);
            else
                for (var i = 0; i < listaAlunosOri.length; i++)
                    for (var j = 0; j < valorMudar.length; j++)
                        if (listaAlunosOri[i].value == valorMudar[j]) {
                            var alunoOri = listaAlunosOri[i];
                            var alunoPer = valorMudar[j];
                            var indice = i;
                            incluirNaTurmaDestino(listaAlunosDest, alunoOri, alunoPer, Option, listaAlunosOri, indice, Permissoes);

                        }
            if (listaAlunosDest.length > 0)
                dijit.byId("salvarMudanca").set('disabled', false);
            else
                dijit.byId("salvarMudanca").set('disabled', true);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function incluirNaTurmaDestino(listaAlunosDest, alunosOri, listaAluno, Option, listaAlunosOri, i, Permissoes) {
    try {
        //incluindo na turma de destino
        listaAlunosDest[listaAlunosDest.length] = new Option(alunosOri.label, listaAluno);

        if (!(dijit.byId('cbManterContrato').get('value') == "on")) {
            // Ao incluir alunos na lista de destino, aba 'Titulos Abertos' é apresentada.
            dojo.byId("paiTabTitulo").style.display = "";

            var aluno = listaContratosAlunos.find(x => x.cd_aluno == listaAluno);
            dojo.byId("cd_contrato").value = aluno.cd_contrato
            this.LoadDataTitulosAbertos(aluno.cd_pessoa_aluno, aluno.cd_contrato, Permissoes);
        }

        //saindo da turma de Origem
        listaAlunosOri[i] = null;
        dijit.byId("alunosPermanecerao").value = [];
        dijit.byId("alunosMudar").value = [];

    } catch (e) {
        postGerarLog(e);
    }
}

function LoadDataTitulosAbertos(cd_pessoa_aluno, cd_contrato, Permissoes) {
    require(["dojo/_base/xhr"], function (xhr) {
        if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getLocalMovtoAndTipoLiquidacaoGeral?cd_pessoa_titulo=" + cd_pessoa_aluno + "&cd_contrato=" + cd_contrato,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = $.parseJSON(data);

                    this.loadLocalMovimentoTitulosAbertos(data.locaMovto);
                    this.loadTipoLiquidacaoTitulosAbertos(data.tipoLiquidacao);
                    this.LoadGridTitulosAbertos(data.titulos);
                    criarOuCarregarCompFiltering("bancos", data.bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_banco', 'no_banco');

                    criarOuCarregarCompFiltering("bancosViewCheque", data.bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_banco', 'no_banco');
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        }
    })
}

function loadLocalMovimentoTitulosAbertos(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbMotivo = dijit.byId('des_local');
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_local_movto, name: value.nomeLocal });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbMotivo.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTipoLiquidacaoTitulosAbertos(registros) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbTipoLiquidacao = dijit.byId('cbLiquidacaoCad');
            Array.forEach(registros, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_liquidacao, name: value.dc_tipo_liquidacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipoLiquidacao.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function LoadGridTitulosAbertos(dataTitulos) {
    this.requererOpcaoTitulo(dataTitulos)
    require(["dojo/_base/array","dojo/store/Memory", "dojo/data/ObjectStore"],
    function (array, Memory, ObjectStore) {
        var gridTitulos = dijit.byId("gridTitulo");
        listGrid = gridTitulos.store.objectStore.data;

        array.forEach(dataTitulos, function(item)
        {
            // add new titulo in list grid.
            listGrid.push(item);
        });

        var memoryTitulos = new Memory({data:listGrid, idProperty: "cd_titulo"});
        gridTitulos.setStore(new ObjectStore({ objectStore: memoryTitulos }));

        gridTitulos.update();
    });
}



function requererOpcaoTitulo(titulos) {
    try {
        apresentaMensagem("apresentadorMensagemCadDesistencia", '');
        if (titulos != null && titulos.length > 0)
            dijit.byId("cbEscolhaCad").set("required", true);
        else
            dijit.byId("cbEscolhaCad").set("required", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function transfereVariosAluno(xhr, ref, Permissoes) {
    try{
        apresentaMensagem("apresentadorMensagem", null);
        if (dojo.byId("cd_NovaTurma").value <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgInfEscolherTurmaDest);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }

        //codigo dos alunos mover
        var cdAlunos = new Array();
        for (var i = 0; i < dojo.byId("alunosPermanecerao").options.length; i++) {
            cdAlunos[i] = dojo.byId("alunosPermanecerao").options[i].value;
        }
        verificaAlunosTurma(xhr, ref, function () {
            if (dojo.byId("cd_NovaTurma").value <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgInfEscolherTurmaDest);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }
            var listaAlunosDest = dojo.byId("alunosMudar").options;
            var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
            var qtdRegOri = listaAlunosOri.length;
            if (dijit.byId("cbMudancasInternas").value == RETORNARTURMA)
                verificaTurmasOrigem(cdAlunos, xhr, ref, true);
            else
                for (var i = qtdRegOri - 1; i >= 0; i--) {
                    //VERIFICA SE É DA MESMA TURMA DE DESTINO QUANDO FOR PELA OPÇÃO 2
                    var alunoOri = listaAlunosOri[i];
                    var alunoPer = listaAlunosOri[i].value;
                    var indice = i;
                    incluirNaTurmaDestino(listaAlunosDest, alunoOri, alunoPer, Option, listaAlunosOri, indice, Permissoes);
                }
            if (listaAlunosDest.length > 0)
                dijit.byId("salvarMudanca").set('disabled', false);
            else
                dijit.byId("salvarMudanca").set('disabled', true);

        });
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarUmAluno() {
    try {      

        var valorMudar = dijit.byId("alunosMudar").value;
        var valido = true;
        if (!hasValue(valorMudar) || valorMudar.length <= 0 || valorMudar.length > 1) {
            if (valorMudar != null && valorMudar.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgNotSelectReg);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
            }
            if (valorMudar != null && valorMudar.length > 1) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgMaisDeUmRegSelect);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
            }
            valido = false;
        }
        if (!valido)
            return false;
        var mudou = false;
        var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
        var listaAlunosMudar = dojo.byId("alunosMudar").options;
        for (var i = 0; i < listaAlunosMudar.length; i++)
            for (var j = 0; j < valorMudar.length; j++)
                if (listaAlunosMudar[i].value == dijit.byId("alunosMudar").value[j]) {
                    //incluindo na turma de destino
                    listaAlunosOri[listaAlunosOri.length] = new Option(listaAlunosMudar[i].label, dijit.byId("alunosMudar").value[j]);
                    //saindo da turma de Origem
                    listaAlunosMudar[i] = null;
                    mudou = true;

                    //removendo titulos da aba 'Titulos Abertos'.
                    this.removeTituloAlunoGrid(parseInt(dijit.byId("alunosMudar").value[j]), false);
                }
        if (listaAlunosMudar.length > 0)
            dijit.byId("salvarMudanca").set('disabled', false);
        else
            dijit.byId("salvarMudanca").set('disabled', true);
        if (mudou) {
            dijit.byId("alunosPermanecerao").value = [];
            dijit.byId("alunosMudar").value = [];
        }

        if (dojo.byId("alunosMudar").options.length == 0) {
            if (hasValue(dojo.byId("tabContainer_tablist_tabTitulo"))) {
                dojo.byId("tabContainer_tablist_tabTitulo").parentElement.id = "paiTabTitulo";
                dojo.byId("paiTabTitulo").style.display = "none";
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function retornarVariosAluno() {
    try {        

        var mudou = false;
        var listaAlunosMudar = dojo.byId("alunosMudar").options;
        var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
        if (listaAlunosMudar != null && listaAlunosMudar.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgNotSelectReg);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
        }
        var qtdRegOri = listaAlunosMudar.length;
        for (var i = qtdRegOri - 1; i >= 0; i--) {
            //incluindo na turma de destino
            listaAlunosOri[listaAlunosOri.length] = new Option(listaAlunosMudar[i].label, listaAlunosMudar[i].value);
            //saindo da turma de Origem
            listaAlunosMudar[i] = null;
            mudou = true;
        }

        //removendo titulos da aba 'Titulos Abertos'.
        this.removeTituloAlunoGrid(null, true);

        if (listaAlunosMudar.length > 0)
            dijit.byId("salvarMudanca").set('disabled', false);
        else
            dijit.byId("salvarMudanca").set('disabled', true);
        if (mudou) {
            dijit.byId("alunosPermanecerao").value = [];
            dijit.byId("alunosMudar").value = [];
        }

        if (dojo.byId("alunosMudar").options.length == 0) {
            if (hasValue(dojo.byId("tabContainer_tablist_tabTitulo"))) {
                dojo.byId("tabContainer_tablist_tabTitulo").parentElement.id = "paiTabTitulo";
                dojo.byId("paiTabTitulo").style.display = "none";
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function removeTituloAlunoGrid(paramCdAluno, removerTodos) {
    require(["dojo/_base/array","dojo/store/Memory", "dojo/data/ObjectStore"],
   function(array, Memory, ObjectStore) {

       var gridAluno = dijit.byId("gridTitulo");
       listGrid = removerTodos ? null : gridAluno.store.objectStore.data;
       var aluno = listaContratosAlunos.find(x => x.cd_aluno == paramCdAluno);

       var memoryTitulos = new Memory({data:listGrid, idProperty: "cd_pessoa_titulo"});
       array.forEach(listGrid, function(item)
       {
           memoryTitulos.remove(aluno.cd_pessoa_aluno);
           dijit.byId("gridTitulo").itensSelecionados.pop(aluno.cd_pessoa_aluno);
       });

       if (memoryTitulos.data.length == 0){
           dijit.byId("gridTitulo").itensSelecionados = [];

           // Limpar 'Local de Movimento' e 'Tipo Liquidação'
           dijit.byId('des_local').reset();
           dijit.byId('cbLiquidacaoCad').reset();
           document.getElementById("tgCheque").style.display = "none";
           isViewChequeTransacao = null;
           dijit.byId('gridTitulo').layout.setColumnVisibility(10, false);
       }
       gridAluno.setStore(new ObjectStore({ objectStore: memoryTitulos }));
       gridAluno.update();       
   });
}

function montaMudanca() {
    try{
        var alunos = dojo.byId("alunosMudar").options;
        var listaAlunosTurma = new Array();
        for (var i = 0; i < alunos.length; i++) {
            var dadosRetorno = {
                cd_aluno: alunos[i].value,
                cd_turma: dojo.byId("cd_NovaTurma").value
            };
            listaAlunosTurma.push(dadosRetorno);
        }
        var dtMovimentacao = dojo.byId("dt_movimentacao").value;
        if (hasValue(dtMovimentacao)){
            dojo.date.locale.parse(dojo.byId("dt_movimentacao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            }
        else{
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDtMovtoInvalida);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }

        var dados = {
            alunos: listaAlunosTurma,
            cd_produto: produto_origem,
            cd_turma_origem: dojo.byId("cd_TurmaAndamento").value,
            cd_turma_destino: dojo.byId("cd_NovaTurma").value,
            opcao: dijit.byId("cbMudancasInternas").value,
            id_manter_contrato: dijit.byId("cbManterContrato").checked,
            id_renegociacao: turma_origem_encerrada ? false : dijit.byId("ckRenegociacao").checked,
            id_ppt: dijit.byId("cbTurmaPPT").checked,
            cd_curso: dijit.byId("cbCurso").value,
            dt_movimentacao: hasValue(dojo.byId("dt_movimentacao").value) ? dojo.date.locale.parse(dojo.byId("dt_movimentacao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            dt_inicio: hasValue(dojo.byId("dt_inicio").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            id_ppt_origem: id_ppt_origem            
        }

        if (dijit.byId("cbEscolhaCad").get('value') > 0) {
            dados.cd_tipo_liquidacao = dijit.byId('cbLiquidacaoCad').get('value');
            dados.cd_local_movto = dijit.byId('des_local').get('value');
            
            dados.titulos = montarTitulosMudancaInterna();
            if(document.getElementById("tgCheque").style.display != "none") {
                dados.chequeTransacao = 
                    { 
                        Cheque:
                            {
                                cd_contrato:  dojo.byId("cd_contrato").value,
                                no_emitente_cheque: dijit.byId("emissorChequeName").get('value'),
                                no_agencia_cheque: dijit.byId("agenciaCheque").get('value'),

                                nm_agencia_cheque: dijit.byId("nroAgencia").get('value'),
                                nm_digito_agencia_cheque: dijit.byId("dgAgencia").get('value'),

                                nm_conta_corrente_cheque: dijit.byId("nroContaCorrente").get('value'),
                                nm_digito_cc_cheque: dijit.byId("dgContaCorrente").get('value'),

                                nm_primeiro_cheque: dijit.byId("nroPrimeiroCheque").get('value'),
                                cd_banco: dijit.byId("bancos").get('value'),
                            }, 
                        nm_cheque: dijit.byId("nroPrimeiroCheque").get('value'), 
                        dt_bom_para: dijit.byId("dtCheque").get('value') 
                    }
                }
        }

        return dados;
    } catch (e) {
        postGerarLog(e);
    }
}

//#region montarTitulos
function montarTitulosMudancaInterna() {
    try{
        var titulos = [];
        var gridTitulo = dijit.byId("gridTitulo");

        if (hasValue(gridTitulo))
            gridTitulo.store.save();

        if (hasValue(gridTitulo) && hasValue(gridTitulo.store.objectStore.data) && (gridTitulo.itensSelecionados != null) && (gridTitulo.itensSelecionados.length > 0))
            var data = gridTitulo.itensSelecionados;
        else {
            titulos = null;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);
            apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
            return false;
        }

        if (hasValue(gridTitulo) && hasValue(data) && data.length > 0) {
            $.each(data, function (idx, val) {
                val.cd_local_movto = dijit.byId('des_local').get('value');
                val.existe_titulo_transacao = document.getElementById("tgCheque").style.display != "none" ? true : false
            });
            return data;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarChequesMudancaInterna(cheques){
    if(document.getElementById("tgCheque").style.display != "none") {
        return cheques = {
            no_emitente_cheque: dijit.byId("emissorChequeName").get('value'),
            no_agencia_cheque: dijit.byId("nroAgencia").get('value'),
            nm_digito_agencia_cheque: dijit.byId("dgAgencia").get('value'),

            //nm_agencia_cheque: dijit.byId("").get('value'),

            nm_conta_corrente_cheque: dijit.byId("nroContaCorrente").get('value'),
            nm_digito_cc_cheque: dijit.byId("dgContaCorrente").get('value'),

            nm_primeiro_cheque: dijit.byId("nroPrimeiroCheque").get('value'),

            cd_banco: dijit.byId("bancos").get('value'),
            dt_bom_para: dijit.byId("dtCheque").get('value')
        };
    }else
        return cheques;
}

//#endregion
function salvarMudanca(xhr, ref, JsonRest) {
    require(["dojo/window"], function (windows) {
        try {

            //Valida curso turma origem e destino
            if (parseInt(dijit.byId("cbMudancasInternas").value) == 1) {
                if (!dijit.byId("cbManterContrato").checked && dijit.byId("cbTurmaPPT").checked == true) {
                    if (curso_origem == dijit.byId("cbCurso").value) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            msgErrorCursoTurmaOrigIgualCursoTurmaDestContratoNaoMarcado);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }
                } else if (!dijit.byId("cbManterContrato").checked && dijit.byId("cbTurmaPPT").checked == false) {
                    if (curso_origem == curso_dest) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            msgErrorCursoTurmaOrigIgualCursoTurmaDestContratoNaoMarcado);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }
                } else if (dijit.byId("cbManterContrato").checked && dijit.byId("cbTurmaPPT").checked == true) {
                    if (curso_origem != dijit.byId("cbCurso").value) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            msgErrorCursoTurmaOrigDiferenteCursoTurmaDestContratoMarcado);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }
                } else if (dijit.byId("cbManterContrato").checked && dijit.byId("cbTurmaPPT").checked == false) {
                    if (curso_origem != curso_dest) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            msgErrorCursoTurmaOrigDiferenteCursoTurmaDestContratoMarcado);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return false;
                    }
                }
            }


            if (dojo.byId("paiTabTitulo").style.display == "" && !dijit.byId("cbManterContrato").checked) {
                if(!ValidarTitulosAbertos(windows))
                    return false;
            }

            if (dojo.byId("alunosMudar").options.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMudancaSemAluno);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }

            if (!dijit.byId("formPrincipalMudanca").validate()){
                var tabs = dijit.byId("tabContainer");
                var pane = dijit.byId("tabPrincipal");
                tabs.selectChild(pane);
                return false;
            }

            if((dijit.byId('cbLiquidacaoCad').value ==  CHEQUEPREDATADO || dijit.byId('cbLiquidacaoCad').value == CHEQUEVISTA) && !dijit.byId("cbManterContrato").checked){
                if(!tipoFinanTituloTransacao(dijit.byId('cbLiquidacaoCad').value)){
                    dijit.byId("tgCheque").set("open", true);
                    return false;
                }
            }

            showCarregando();
            var mudanca = montaMudanca();
            xhr.post({
                url: Endereco() + "/api/escola/postMudancaInterna",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(mudanca)
            }).then(function (data) {
                try{
                    limparMudancasInternas();
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', data);
                    
                    resetAbaTitulosAbertos();
                } catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem("apresentadorMensagem", error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function resetAbaTitulosAbertos() {
    isViewChequeTransacao = null;
    
    dijit.byId('gridTitulo').itensSelecionados = [];
    dijit.byId('emissorChequeName').set("value","");
    dijit.byId('nroAgencia').set("value","");
    dijit.byId('dgAgencia').set("value","");
    dijit.byId('dtCheque').set("value",null);
    dijit.byId('bancos').set("value","");
    dijit.byId('nroContaCorrente').set("value","");
    dijit.byId('dgContaCorrente').set("value","");
    dijit.byId('nroPrimeiroCheque').set("value","");
    dijit.byId('agenciaCheque').set("value","");
    document.getElementById("tgCheque").style.display = "none";
}

function ValidarTitulosAbertos(windowUtils) {

    var escolhaTitulo = dijit.byId("cbEscolhaCad");
    var desLocal = dijit.byId('des_local');
    var liquidacao = dijit.byId('cbLiquidacaoCad');

    var emissorChequeName = dijit.byId('emissorChequeName');
    var nroAgencia = dijit.byId('nroAgencia');
    var dgAgencia = dijit.byId('dgAgencia');
    var dtCheque = dijit.byId('dtCheque');
    var banco = dijit.byId('bancos');
    var nroContaCorrente  = dijit.byId('nroContaCorrente');
    var dgContaCorrente = dijit.byId('dgContaCorrente');
    var nroPrimeiroCheque = dijit.byId('nroPrimeiroCheque');
    var agenciaCheque = dijit.byId('agenciaCheque');

    dijit.byId("cbEscolhaCad").set("required", true);
    var gridTitulo = dijit.byId('gridTitulo');

    var FormValido = true;

    if (!escolhaTitulo.validate()) {
        mostrarMensagemCampoValidado(windowUtils, escolhaTitulo);

        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTitulosAbertos);
        apresentaMensagem("apresentadorMensagem", mensagensWeb);
        FormValido = false;
    }

    if (dijit.byId("cbEscolhaCad").get('value') > 0) {
        if (!desLocal.validate()) {
            FormValido = false;
            mostrarMensagemCampoValidado(windowUtils, desLocal);
        }
        if (!liquidacao.validate()) {
            FormValido = false;
            mostrarMensagemCampoValidado(windowUtils, liquidacao);
        }
        //---- Validar: Titulos Abertos -> Tipo Cheque.
        if(document.getElementById("tgCheque").style.display == "") {
            if (!emissorChequeName.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, emissorChequeName);
            }
            if (!nroAgencia.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, nroAgencia);
            }
            if (!dgAgencia.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, dgAgencia);
            }
            if (!dtCheque.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, dtCheque);
            }
            if (!banco.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, banco);
            }
            if (!nroContaCorrente.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, nroContaCorrente);
            }
            if (!dgContaCorrente.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, dgContaCorrente);
            }
            if (!nroPrimeiroCheque.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, nroPrimeiroCheque);
            }
            if(!agenciaCheque.validate()) {
                FormValido = false;
                mostrarMensagemCampoValidado(windowUtils, agenciaCheque);
            }
        }
    //----

    if(!gridTitulo.itensSelecionados){
        
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);
        apresentaMensagem("apresentadorMensagem", mensagensWeb);
        FormValido = false;
    }
    }

    if(!FormValido){
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabTitulo");
        tabs.selectChild(pane);
    }

    return FormValido;
}

function setRequiredTipoCheque(isRequired) {
    dijit.byId("emissorChequeName").set("required", isRequired);
    dijit.byId("nroAgencia").set("required", isRequired);
    dijit.byId("dgAgencia").set("required", isRequired);
    dijit.byId("dtCheque").set("required", isRequired);
    dijit.byId("bancos").set("required", isRequired);
    dijit.byId("nroContaCorrente").set("required", isRequired);
    dijit.byId("dgContaCorrente").set("required", isRequired);
    dijit.byId("nroPrimeiroCheque").set("required", isRequired);
    dijit.byId("agenciaCheque").set("required", isRequired);
}

function carregarCursoPorProdutoMudanca(cd_produto, cd_curso) {
    try{
        dijit.byId('cbCurso').set("required", false);
        var params = "";
        if (hasValue(cd_curso)) {
            dijit.byId('cbCurso').set("disabled", true);
            params = "cd_curso=" + cd_curso + "&cd_produto=" + cd_produto;
        }
        else {
            dijit.byId('cbCurso').set("disabled", false);
            cd_curso = 0;
            params = "cd_curso=&cd_produto=" + cd_produto;
        }
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/curso/getCursosPorProduto?" + params,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            data = jQuery.parseJSON(data);
            if (hasValue(data.retorno) && data.retorno.length > 0)
                loadSelect(data.retorno, "cbCurso", 'cd_curso', 'no_curso', cd_curso);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}


function verificaAlunosTurma(xhr, ref, pFuncao) {
    try{
        if (parseInt(dijit.byId("cbMudancasInternas").value) == 1) {
            var alunos = hasValue(dijit.byId("alunosPermanecerao").value) ? dijit.byId("alunosPermanecerao").value : dojo.byId("alunosPermanecerao").options;
            var listaAlunosTurma = new Array();

            for (var i = 0; i < alunos.length; i++) {
                var dadosRetorno = {
                    cd_aluno: alunos[i].value > 0 ? alunos[i].value : alunos[i],
                    cd_turma: dojo.byId("cd_NovaTurma").value,
                    cd_turma_origem: dojo.byId("cd_TurmaAndamento").value
                };
                listaAlunosTurma.push(dadosRetorno);
            }

            xhr.post({
                url: Endereco() + "/api/coordenacao/postVerificaAlunosTurma",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(listaAlunosTurma)
            }).then(function (data) {
                try{
                    if (jQuery.parseJSON(data).retorno)
                        if (hasValue(pFuncao))
                            pFuncao.call();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagem", error);
            });
        }
        else
            if (hasValue(pFuncao))
                pFuncao.call();
    } catch (e) {
        postGerarLog(e);
    }
}

function limparTurmasAlunos() {
    try{
        dojo.byId("cd_TurmaAndamento").value = 0;
        dijit.byId("cbTurmaAndamento").set("value", "");
        dijit.byId("cdNovaTurma").set("value", "");
        dojo.byId("cd_NovaTurma").value = 0;
        dojo.byId('cbTurmaAndamento').value = "";
        dijit.byId('fkTurmaAndamento').set('disabled', false)
        dijit.byId("cbTurmaPPT").set('disabled', true);
        dijit.byId("cbTurmaPPT").set('checked', false);
        dijit.byId("cbCurso").set("value", "");
        dijit.byId("dt_movimentacao").reset();
        mudarPPT(dijit.byId('cbTurmaPPT'));

        var alunosPer = dojo.byId("alunosPermanecerao").options;
        var alunosMudar = dojo.byId("alunosMudar").options;

        for (var i = alunosPer.length - 1 ; i >= 0; i--)
            alunosPer[i] = null;
        for (var i = alunosMudar.length - 1; i >= 0; i--)
            alunosMudar[i] = null;

        dijit.byId("fkNovaTurma").set("disabled", true);
        dijit.byId('limparNovaTurmaPes').set("disabled", true);
        dijit.byId("dt_inicio").reset();
        dijit.byId("salvarMudanca").set('disabled', true);
    } catch (e) {
        postGerarLog(e);
    }
}

function limparMudancasInternas() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        dijit.byId('cbMudancasInternas').set("value", 1);
        limparTurmasAlunos();
        dijit.byId("dt_movimentacao").reset();
        dijit.byId("cbTurmaPPT").set('checked', false);
        dijit.byId("cbManterContrato").set('checked', false);
        dijit.byId("ckRenegociacao").set('checked', false);
        mudarPPT(dijit.byId("cbTurmaPPT"));
        dijit.byId('limparTurmaAndamentoPes').set("disabled", true);

        this.limparTitulosAbertos();
    } catch (e) {
        postGerarLog(e);
    }
}
function limparTurmaNova() {
    try{
        dojo.byId("cd_NovaTurma").value = 0;
        dijit.byId('cdNovaTurma').value = 0;
        dojo.byId('cdNovaTurma').value = "";
        dijit.byId('limparNovaTurmaPes').set("disabled", true);
        var alunosMudar = dojo.byId("alunosMudar").options;
        var alunosPer = dojo.byId("alunosPermanecerao").options;
        if (!hasValue(dojo.byId("cd_TurmaAndamento").value))
            for (var i = alunosMudar.length - 1; i >= 0; i--)
                alunosMudar[i] = null;
        else
            for (var i = alunosMudar.length - 1; i >= 0; i--) {
                //incluindo na turma de destino
                alunosPer[alunosPer.length] = new Option(alunosMudar[i].label, alunosMudar[i].value);
                //saindo da turma de Origem
                alunosMudar[i] = null;
            }
        dijit.byId("salvarMudanca").set('disabled', true);
        if (!dijit.byId('cbManterContrato').checked || !dijit.byId('cbTurmaPPT').checked) {
            dijit.byId("cbCurso").set("value", "");
            dijit.byId("dt_inicio").reset();
        }

        limparTitulosAbertos();
    } catch (e) {
        postGerarLog(e);
    }
}

function limparTitulosAbertos(){
    
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
   function (Memory, ObjectStore) {
     
       // Limpar grid de titulos
       var gridTitulos = dijit.byId("gridTitulo");
       gridTitulos.setStore(new ObjectStore({ objectStore: new Memory({data:null }) }));
       gridTitulos.update();      

       // Fecha aba 'Titulos Abertos'
       dojo.byId("paiTabTitulo").style.display = "none";

       // Limpar 'Local de Movimento', 'Tipo Liquidação' e 'O que deseja fazer?'.
       dijit.byId('des_local').reset();
       dijit.byId('cbLiquidacaoCad').reset();
       dijit.byId('cbEscolhaCad').reset();
       
       // Redirecionar para aba 'Principal'
       var tabs = dijit.byId("tabContainer");
       var pane = dijit.byId("tabPrincipal");
       tabs.selectChild(pane);
   });
}

function verificaTurmaOrigem(cdAluno, xhr, ref, pFuncao) {
    // VERIFICAR A TURMA DE ORIGEM DO ALUNO SELECIONADO E DA TURMA DE ORIGEM PARA ESCOLHER A TURMA DE DESTINO, SE TIVER SELECIONADO MAIS DE UM ALUNO, SEMPRE PEGARÁ
    // O PRIMEIRO

    xhr.get({
        preventCache: true,
        url: Endereco() + "/api/turma/getTurmaOrigem?cdTurma=" + dojo.byId("cd_TurmaAndamento").value + "&cdAluno=" + cdAluno,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            var retorna = true;
            if (hasValue(data.retorno) && data.retorno.cd_turma > 0)
                if (dojo.byId("cd_NovaTurma").value == 0 || !hasValue(dojo.byId("cd_NovaTurma").value)) {
                    dojo.byId("cd_NovaTurma").value = data.retorno.cd_turma;
                    //dijit.byId("cdNovaTurma").set("value", data.retorno.cd_turma);
                    dojo.byId("cdNovaTurma").value = data.retorno.no_turma;
                    dijit.byId('limparNovaTurmaPes').set("disabled", false);
                    if(hasValue(pFuncao))
                        pFuncao.call();
                }
                else
                    if (dojo.byId("cd_NovaTurma").value != data.retorno.cd_turma) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaOriginal);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    }
                    else
                        if (hasValue(pFuncao))
                            pFuncao.call();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });

}


function verificaTurmasOrigem(cdAlunos, xhr, ref, todos) {
    // VERIFICAR A TURMA DE ORIGEM DO ALUNO SELECIONADO E DA TURMA DE ORIGEM PARA ESCOLHER A TURMA DE DESTINO, SE TIVER SELECIONADO MAIS DE UM ALUNO, SEMPRE PEGARÁ
    // O PRIMEIRO
    try{
        var listaAlunoTurma = [];
        if (hasValue(cdAlunos) && cdAlunos.length > 0)
            for (var i = 0; i < cdAlunos.length; i++) {
                var alunosTurma = {
                    cd_turma: dojo.byId("cd_TurmaAndamento").value,
                    cd_aluno: parseInt(cdAlunos[i])
                };
                listaAlunoTurma.push(alunosTurma);
            }

        xhr.post({
            url: Endereco() + "/api/turma/postProcuraTurmasOrigem",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(listaAlunoTurma)
        }).then(function (data) {
            try{
                var retorna = true;
                var difirente = false;
                if (hasValue(data.retorno) && data.retorno.length > 0) {
                    //Verifica se todos cdTurma são iguais
                    var cdTurma1 = data.retorno[0].cd_turma;
                    for (var i = 1; i < data.retorno.length; i++)
                        if (data.retorno[i].cd_turma != cdTurma1) {
                            difirente = true;
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaOriginal);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            break;
                        }


                    if (dojo.byId("cd_NovaTurma").value == 0 || !hasValue(dojo.byId("cd_NovaTurma").value)) {
                        dojo.byId("cd_NovaTurma").value = data.retorno.cd_turma;
                        //dijit.byId("cdNovaTurma").set("value", data.retorno.cd_turma);
                        dojo.byId("cdNovaTurma").value = data.retorno.no_turma;
                        dijit.byId('limparNovaTurmaPes').set("disabled", false);
                        var listaAlunosDest = dojo.byId("alunosMudar").options;
                        var listaAlunosOri = dojo.byId("alunosPermanecerao").options;
                        if (todos) {
                            for (var i = 1; i < data.retorno.length; i++) {
                                //incluindo na turma de destino
                                listaAlunosDest[listaAlunosDest.length] = new Option(listaAlunosOri[i].label, listaAlunosOri[i].value);
                                //saindo da turma de Origem
                                listaAlunosOri[i] = null;
                                dijit.byId("alunosPermanecerao").value = [];
                                dijit.byId("alunosMudar").value = [];
                            }
                        }
                        else
                            for (var i = 1; i < data.retorno.length; i++)
                                for (var j = 0; j < cdAlunos.length; j++)
                                    if (parseInt(data.retorno[i].value) == parseInt(cdAlunos[j])) {
                                        //incluindo na turma de destino
                                        listaAlunosDest[listaAlunosDest.length] = new Option(listaAlunosOri[i].label, listaAlunosOri[i].value);
                                        //saindo da turma de Origem
                                        listaAlunosOri[i] = null;
                                        dijit.byId("alunosPermanecerao").value = [];
                                        dijit.byId("alunosMudar").value = [];
                                    }
                    }
                    else
                        if (!difirente)
                            if (todos) {
                                //////////////
                                //for (var i = 0; i < dojo.byId("alunosPermanecerao").options.length; i++) {
                                for (var i = dojo.byId("alunosPermanecerao").options.length - 1; i >= 0; i--) {
                                    //incluindo na turma de destino
                                    dojo.byId("alunosMudar").options[dojo.byId("alunosMudar").options.length] = new Option(dojo.byId("alunosPermanecerao").options[i].label, dojo.byId("alunosPermanecerao").options[i].value);
                                    //saindo da turma de Origem
                                    dojo.byId("alunosPermanecerao").options[i] = null;
                                }
                                dijit.byId("alunosPermanecerao").value = [];
                                dijit.byId("alunosMudar").value = [];
                                if (dojo.byId("alunosMudar").options.length > 0)
                                    dijit.byId("salvarMudanca").set('disabled', false);
                                else
                                    dijit.byId("salvarMudanca").set('disabled', true);
                            }
                            else {
                                for (var j = 0; j < dojo.byId("alunosPermanecerao").options.length; j++)
                                    for (var i = 0; i < cdAlunos.length; i++)
                                        if (parseInt(dojo.byId("alunosPermanecerao").options[j].value) == parseInt(cdAlunos[i])) {
                                            //incluindo na turma de destino
                                            dojo.byId("alunosMudar").options[dojo.byId("alunosMudar").options.length] = new Option(dojo.byId("alunosPermanecerao").options[j].label, dojo.byId("alunosPermanecerao").options[j].value);
                                            //saindo da turma de Origem
                                            dojo.byId("alunosPermanecerao").options[j] = null;

                                            dijit.byId("alunosPermanecerao").value = [];
                                            dijit.byId("alunosMudar").value = [];
                                        }
                                if (dojo.byId("alunosMudar").options.length > 0)
                                    dijit.byId("salvarMudanca").set('disabled', false);
                                else
                                    dijit.byId("salvarMudanca").set('disabled', true);

                            }
                }
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function chamarPesquisaMudancaInterna() {
    try{
        if (dojo.byId("idOrigenPesquisaTurmaMudInterna").value == MUDANCAINTERNAORI)
            pesquisarTurmaFKMudanca(MUDANCAINTERNAORI);
        else
            pesquisarTurmaFKMudanca(MUDANCAINTERNADESTINO);
    } catch (e) {
        postGerarLog(e);
    }
}

function tipoFinanTituloTransacao(event) {
    var gridTitulos = dijit.byId("gridTitulo");
    var mensagensWeb = new Array();

    if(!hasValue(gridTitulos.itensSelecionados)) {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);
        apresentaMensagem("apresentadorMensagem", mensagensWeb);
        dijit.byId("cbLiquidacaoCad").reset();
        return false;
    }    
    
    document.getElementById("tgCheque").style.display = "none";
    //setRequiredTipoCheque(false);

    var isCheque = [];
    var isChequeTransacao = [];
    if (event ==  CHEQUEPREDATADO || event == CHEQUEVISTA) {
        
        isCheque = jQuery.grep(gridTitulos.itensSelecionados, function (titulo) {
            if(titulo.cd_tipo_financeiro ==  CHEQUEPREDATADO)
                return true;
        });

        isChequeTransacao = jQuery.grep(gridTitulos.itensSelecionados, function (titulo) {
            if(titulo.cd_tipo_financeiro !=   CHEQUEPREDATADO)
                return true;
        });

        // Retorna mensagem de erro informando os titulos que não são cheque.
        if(isCheque.length > 0 && isChequeTransacao.length > 0){

            if (isViewChequeTransacao != null) {
                if (isViewChequeTransacao)
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoTransacao);
                else
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoCheque);
            } else
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }

        // Libera o botão adicionar cheque para cada titulo selecionado do grid.
        if(isCheque.length > 0 && isChequeTransacao.length == 0 && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0) ){
            isViewChequeTransacao = false;
            apresentaMensagem("apresentadorMensagem", "");
            gridTitulos.layout.setColumnVisibility(10, true);
            return true;
        }

        // Libera a opção de inserir um unico cheque para todos os titulos.
        if(isCheque.length == 0 && isChequeTransacao.length > 0 && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0) ){
            // show 
            isViewChequeTransacao = true;
            document.getElementById("tgCheque").style.display = "";
            apresentaMensagem("apresentadorMensagem", "");
            setRequiredTipoCheque(true);
            return true;
        }
    }else {
        document.getElementById("tgCheque").style.display = "none";
        dijit.byId('gridTitulo').layout.setColumnVisibility(10, false);
        isViewChequeTransacao = null;
        if (isCheque.length > 0 && isChequeTransacao.length > 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }
        //Não deixar liquidar os títulos financeiros do tipo cheque com outro tipo de liquidação que não seja cheque pré-datado ou avista
        if (isCheque.length > 0 && isChequeTransacao.length == 0 && event != CHEQUEPREDATADO) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }
    }
}

function desabilitarBotaoIncluirCheque(isChecked, tituloSelecionado, isCheckAoMontarGrid) {

    if(!hasValue(dijit.byId("cbLiquidacaoCad").value))
        dijit.byId('gridTitulo').layout.setColumnVisibility(10, false);

    if(isCheckAoMontarGrid){
        if(hasValue(dijit.byId("gridTitulo").itensSelecionados)) {
            var grid = dijit.byId("gridTitulo").itensSelecionados;
            for (var i = 0; i < grid.length; i++) {
                if (grid[i].cd_tipo_financeiro == TIPOFINANCHEQUE && hasValue(dijit.byId(grid[i].cd_titulo+"_"+grid[i].nm_parcela_titulo))) {
                    dijit.byId(grid[i].cd_titulo+"_"+grid[i].nm_parcela_titulo).set("disabled", false);
                }
            }
            if(grid.length == dijit.byId("gridTitulo").store.objectStore.data.length)
                return false;
        }
        return true;
    }else{
        if(hasValue(tituloSelecionado)) {
            if(tituloSelecionado.cd_tipo_financeiro ==   TIPOFINANCHEQUE && isChecked && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0)) {
                dijit.byId(tituloSelecionado.cd_titulo+"_"+tituloSelecionado.nm_parcela_titulo).set("disabled", false);
            }else{
                dijit.byId(tituloSelecionado.cd_titulo+"_"+tituloSelecionado.nm_parcela_titulo).set("disabled", true);
            }
        }
    }
}
