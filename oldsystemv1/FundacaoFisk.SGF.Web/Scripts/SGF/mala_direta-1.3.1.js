var TODOS = 0;
var NORMAL = 1, PPT = 3;
var SITUACAO = 3;
var STATUS_MIDIA_TODOS = 0;
var RECEBER = 0;
var TIPO_FUNCIONARIO_PROFESSOR = 4;

function montarCadastroMaladireta(funcao) {
    //Criação da Grade
    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/MenuBar",
        "dijit/PopupMenuBarItem",
        "dijit/MenuItem",
        "dijit/DropDownMenu",
        "dojo/data/ItemFileReadStore",
        "dijit/registry",
        "dijit/Editor",
        "dijit/_editor/plugins/FontChoice",
        "dijit/_editor/plugins/TextColor",
        "dijit/_editor/plugins/LinkDialog",
        "dijit/_editor/plugins/ViewSource",
        "dojox/editor/plugins/ShowBlockNodes",
        "dojox/editor/plugins/Preview",
        "dojox/editor/plugins/Blockquote",
        "dijit/_editor/plugins/NewPage",
        "dojox/editor/plugins/FindReplace",
        "dojox/editor/plugins/ToolbarLineBreak",
        "dojox/editor/plugins/CollapsibleToolbar",
        "dojo/window",
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom,
            MenuBar, PopupMenuBarItem, MenuItem, DropDownMenu, ItemFileReadStore, registry, Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar, windowUtils) {
        ready(function () {
            try {

                loadPesqSexo(Memory, dijit.byId("nm_sexo_md"));
                loadMesNascimento(Memory, dijit.byId('nm_mes_nascimento_md'));
                loadPeriodo(Memory, ItemFileReadStore, registry.byId('cbPeriodo_md'));
          
                //Criação dos botões:
                new Button({
                    label: "Visualizar", iconClass: 'dijitEditorIcon dijitEditorIconEmail', onClick: function () {                        
                        visualizarEtiqueta(windowUtils);
                    }
                }, "btnVisualizarMalaDireta");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        limparMalaDireta();
                    }
                }, "btnCancelarMalaDireta");

                new Button({
                    label: "Fechar",
                    iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        dijit.byId("dlgInserirImagem").hide();
                    }
                }, "btnFechar");

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        incluirImagemEditor();
                    }
                }, "btnIncluir");

                            
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFKMalaDireta(xhr, ObjectStore, Memory, Cache, JsonRest);
                            });
                        else
                            abrirTurmaFKMalaDireta(xhr, ObjectStore, Memory, Cache, JsonRest);
                    }
                }, "pesProTurmaFK_md");
                btnPesquisar(dojo.byId("pesProTurmaFK_md"));

                dijit.byId("cbTipoTurma_md").on("change", function (tipo) {
                    try {
                        if (!hasValue(tipo, true) || tipo < 0)
                            dijit.byId("cbTipoTurma_md").set("value", TODOS);
                        else {
                            var pesSituacao = dijit.byId("cbSituacaoTurma_md");
                            if (tipo == PPT) {
                                dojo.byId("trTurmaFilha_md").style.display = "";
                                dojo.byId("lblTurmaFilhas_md").style.display = "";
                                dojo.byId("divPesTurmasFilhas_md").style.display = "";
                                loadSituacaoTurmaPPTMalaDireta();
                                pesSituacao.set("value", 1);
                                pesSituacao.set("disabled", false);
                            } else {
                                dojo.byId("trTurmaFilha_md").style.display = "none";
                                dojo.byId("lblTurmaFilhas_md").style.display = "none";
                                dojo.byId("divPesTurmasFilhas_md").style.display = "none";
                                dijit.byId("pesTurmasFilhas_md").set("checked", false);
                                loadSituacaoTurma(Memory);
                                pesSituacao.set("value", 1);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pesTurmasFilhas_md").on("click", function (e) {
                    try {
                        var pesSituacao = dijit.byId("cbSituacaoTurma_md");
                        if (dijit.byId("cbTipoTurma_md").displayedValue == "Personalizada" && this.checked) {
                            loadSituacaoTurmaPPTMalaDireta();
                            loadSituacaoTurmaMalaDireta(Memory);
                            pesSituacao.set("value", 1);
                        } else if (dijit.byId("cbTipoTurma_md").displayedValue == "Personalizada" && !this.checked) {
                            loadSituacaoTurmaMalaDireta(dojo.store.Memory);
                            loadSituacaoTurmaPPTMalaDireta();
                            pesSituacao.set("value", 1);
                            pesSituacao.set("disabled", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //Configura que quando o usuario selecionar o tipo PPT marcara automaticamente turmas filhas.
                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    try {
                        dijit.byId("pesTurmasFilhasFK").set("checked", false);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        dojo.byId("cd_turma_pesquisa_md").value = 0;
                        dojo.byId("cbTurma_md").value = "";
                        dijit.byId('limparProTurmaFK_md').set("disabled", true);

                        dijit.byId('cbTipoTurma_md').set("disabled", false);
                        dijit.byId('cbSituacaoTurma_md').set("disabled", false);

                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparProTurmaFK_md");

                if (hasValue(document.getElementById("limparProTurmaFK_md"))) {
                    document.getElementById("limparProTurmaFK_md").parentNode.style.minWidth = '40px';
                    document.getElementById("limparProTurmaFK_md").parentNode.style.width = '40px';
                }

                dijit.byId("estado_md").on("change", function (cd_estado) {
                    dijit.byId("cidade_md").reset();
                    dijit.byId("bairro_md").reset();

                    if (hasValue(cd_estado))
                        getCidadesPorIdEstado(cd_estado);
                });

                dijit.byId("cidade_md").on("change", function (cd_cidade) {
                    dijit.byId("bairro_md").reset();

                    if (hasValue(cd_cidade))
                        getBairroPorIdCidade(cd_cidade);
                });

                if (hasValue(funcao))
                    funcao.call();
                //showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function loadMesNascimento(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Selecione", id: 0 },
                    { name: "Janeiro", id: 1 },
                    { name: "Fevereiro", id: 2 },
                    { name: "Março", id: 3 },
                    { name: "Abril", id: 4 },
                    { name: "Maio", id: 5 },
                    { name: "Junho", id: 6 },
                    { name: "Julho", id: 7 },
                    { name: "Agosto", id: 8 },
                    { name: "Setembro", id: 9 },
                    { name: "Outubro", id: 10 },
                    { name: "Novembro", id: 11 },
                    { name: "Dezembro", id: 12 }
            ]
        });
        id.store = stateStore;
        id.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadProdutoAtualMalaDireta() {
    require([
       "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore"
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=0&cd_produto=null",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    var produtos = jQuery.parseJSON(data).retorno;
                    var w = registry.byId("cbProdutoAtual_md");
                    var produtoCb = [];
                    if (produtos != null || produtos.length > 0)
                        $.each(produtos, function (index, val) {
                            produtoCb.push({
                                "cd_produto": val.cd_produto + "",
                                "no_produto": val.no_produto
                            });
                        });
                    var store = new ItemFileReadStore({
                        data: {
                            identifier: "cd_produto",
                            label: "no_produto",
                            items: produtoCb
                        }
                    });
                    w.setStore(store, []);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadCursoAtualMalaDireta() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore"
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId("cbCursoAtual_md");
                var produtoCb = [];
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: produtoCb
                    }
                });
                w.setStore(store, []);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadPeriodo(Memory, ItemFileReadStore, w) {
    try {
        var stateStore = [
                   { name: "Manhã", id: "1" },
                   { name: "Tarde", id: "2" },
                   { name: "Noite", id: "3" }
        ];
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: stateStore
            }
        });
        w.setStore(store, []);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostraPeriodoAlunoMalaDireta(obj) {
    showP('tr_sexo_e_mes_nasc', true);

    if (dijit.byId('ckAlunoMalaDireta').checked) {
       
        
        setTitlePanel();
        setMenssageMultiSelect(DIA, 'cbDiasMalaDireta_md');
        setMenssageMultiSelect(SITUACAO, 'cbSituacaoAluno_md');
        loadDiasSemanaMalaDireta();
        loadSituacaoAlunoMalaDireta();
        loadTipoTurmaMalaDireta();
        loadSituacaoTurmaMalaDireta();
        if (!hasValue(dijit.byId("cbComoConheceu_md").store.data))
            loadMidiaMalaDireta();

        if (!hasValue(dijit.byId("escolaridade_md").store.data))
            getEscolaridade();

        if (!hasValue(dijit.byId("estado_md").store.data))
            getEstados();

        showP('trMatricula_md', true);
        showP('trTurma_md', true);

        if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

            showP('trLocalidade_md', false);
        } else {
            //Bairro
            showP('trLocalidade_md', true);
        }

        //Nome
        showP('tdNome_pessoa1', true);
        showP('tdNome_pessoa2', true);

        //Idade
        showP('tdIdade1', true);
        showP('tdIdade2', true);

        //Escolaridade
        showP('tdEscolaridade1', true);
        showP('tdEscolaridade2', true);

        showP('trDiasMalaDireta', true);

        //Situação Aluno
        showP('tdSituacao_md', true);
    }
    else {
        setTitlePanel();
        if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked)
            hideInputSexoMesNasc();

        showP('trMatricula_md', false);
        showP('trTurma_md', false);

        if (dijit.byId('ckPendentesMalaDireta').checked ) {
            if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                showP('trLocalidade_md', false);
            } else {
                //Bairro
                showP('trLocalidade_md', true);
            }
        } else {
            //Bairro
            showP('trLocalidade_md', false);
        }
        

        //Nome
        hideInputNome();

        //Idade
        hideInputIdade();

        //Escolaridade
        showP('tdEscolaridade1', false);
        showP('tdEscolaridade2', false);

        hideInputDias();

        //Situação Aluno
        showP('tdSituacao_md', false);

    }

    if (dijit.byId('ckPendentesMalaDireta').checked || dijit.byId('ckAlunoMalaDireta').checked || dijit.byId('ckClienteMalaDireta').checked) {
        showP('trProdutoAtual_md', true);
        showP('trComoConheceu_md', true);
    }
    else {
        showP('trProdutoAtual_md', false);
        showP('trComoConheceu_md', false);
    }
    show('tdCursoAtual1_md');
    show('tdCursoAtual2_md');
    show('tdPeriodoAluno1_md');
    show('tdPeriodoAluno2_md');
}

function mostraAlunosInadimplentesMalaDireta(obj) {

    hideInputSexoMesNasc();
    if (!obj.checked) {
        setTitlePanel();
        showP('tr_sexo_e_mes_nasc', true);                
        showP('trProdutoAtual_md', false);
        
        //Pessoa responsavel
        showP('tdResponsavel1', false);
        showP('tdResponsavel2', false);

        //Idade
        hideInputIdade();

        //Nome
        hideInputNome();

        //Quebrar linha
        showP('quebrar_linha1', false);
        showP('quebrar_linha2', false);

        if (dijit.byId('ckPendentesMalaDireta').checked) {
            if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                showP('trLocalidade_md', false);
            } else {
                //Bairro
                showP('trLocalidade_md', true);
            }
        } else {
            //Bairro
            showP('trLocalidade_md', false);
        }

    } else {
        
        setTitlePanel();
        if (!hasValue(dijit.byId("responsavel_md").store.data))
            getPessoaResponsavel();

        showP('trProdutoAtual_md', true);

        //Pessoa responsavel
        showP('tdResponsavel1', true);
        showP('tdResponsavel2', true);

        //Idade
        showP('tdIdade1', true);
        showP('tdIdade2', true);

        //Nome
        showP('tdNome_pessoa1', true);
        showP('tdNome_pessoa2', true);

        //Quebrar linha
        showP('quebrar_linha1', true);
        showP('quebrar_linha2', true);

        //Bairro
        showP('trLocalidade_md', false);
    }
}

function mostraPeriodoProspectMalaDireta(obj) {
    showP('tr_sexo_e_mes_nasc', true);

    if (dijit.byId('ckPendentesMalaDireta').checked) {
        showP('mes_nascimento_md1', false);
        showP('mes_nascimento_md2', false);

        setTitlePanel();
        setMenssageMultiSelect(DIA, 'cbDiasMalaDireta_md');
        loadDiasSemanaMalaDireta();

        if (!hasValue(dijit.byId("cbComoConheceu_md").store.data))
            loadMidiaMalaDireta();

        if (!hasValue(dijit.byId("cbFaixaEtaria_md").store.data))
            loadFaixaEtariaMalaDireta();

        if (!hasValue(dijit.byId("estado_md").store.data))
            getEstados();

        showP('trComoConheceu_md', true);
        showP('trFaixaEtaria_md', true);
        
        //Nome
        showP('tdNome_pessoa1', true);
        showP('tdNome_pessoa2', true);

        showP('trDiasMalaDireta', true);

        if (dijit.byId('ckFuncionarioProfessorMalaDireta').checked || dijit.byId('ckAlunosInadimplentesMalaDireta').checked) {

            showP('trLocalidade_md', false);
        } else {
            if (dijit.byId('ckPendentesMalaDireta').checked) {
                if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                    (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                    (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                    showP('trLocalidade_md', false);
                } else {
                    //Bairro
                    showP('trLocalidade_md', true);
                }
            } else {
                //Bairro
                showP('trLocalidade_md', false);
            }
        }
    }
    else {
        setTitlePanel();
        if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked) {

            hideInputSexoMesNasc();

            showP('trLocalidade_md', false);
        } else {
            if (dijit.byId('ckPendentesMalaDireta').checked) {
                if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                    (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                    (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                        dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                    showP('trLocalidade_md', false);
                } else {
                    //Bairro
                    showP('trLocalidade_md', true);
                }
            } else {
                //Bairro
                showP('trLocalidade_md', false);
            }
        }

        showP('mes_nascimento_md1', true);
        showP('mes_nascimento_md2', true);

        showP('trComoConheceu_md', false);
        showP('trFaixaEtaria_md', false);
        
        //Nome
        //showP('tdNome_pessoa1', false);
        //showP('tdNome_pessoa2', false);
        hideInputNome();

        hideInputDias();

        dijit.byId("cbComoConheceu_md").set("value", "");
    }

    if (dijit.byId('ckPendentesMalaDireta').checked || dijit.byId('ckAlunoMalaDireta').checked || dijit.byId('ckClienteMalaDireta').checked) {
        showP('trProdutoAtual_md', true);
        showP('trComoConheceu_md', true);
    }
    else {
        showP('trProdutoAtual_md', false);
        showP('trComoConheceu_md', false);
    }

    show('tdPeriodoProspect1_md');
    show('tdPeriodoProspect2_md');
}

function mostraProfissaoMalaDireta(obj) {
    showP('tr_sexo_e_mes_nasc', true);

    require([
       "dojo/store/Memory",
        "dojo/ready",
    ], function (Memory, ready) {
        ready(function () {
            if (!obj.checked) {
                setTitlePanel();
                if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked)
                    hideInputSexoMesNasc();

                //Nome
                //showP('tdNome_pessoa1', false);
                //showP('tdNome_pessoa2', false);
                hideInputNome();

                //Idade
                //showP('tdIdade1', false);
                //showP('tdIdade2', false);
                hideInputIdade();

                if (dijit.byId('ckPendentesMalaDireta').checked) {

                    if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                            !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                        (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                            dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                        (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                            dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                        showP('trLocalidade_md', false);
                    } else {
                        //Bairro
                        showP('trLocalidade_md', true);
                    }
                    
                } else {
                    //Bairro
                    showP('trLocalidade_md', false);
                }
            
            } else {

                setTitlePanel();
                //Nome
                showP('tdNome_pessoa1', true);
                showP('tdNome_pessoa2', true);

                //Idade
                showP('tdIdade1', true);
                showP('tdIdade2', true);

               

                //Bairro
                showP('trLocalidade_md', false);

                loadProfissao(Memory, dijit.byId('tdTipoProfissao_md'));                
            }
        });
    });
}

function mostraCamposClienteMalaDireta(obj) {
    if (!hasValue(dijit.byId("cbComoConheceu_md").store.data))
        loadMidiaMalaDireta();
    
    if (!obj.checked) {
        setTitlePanel();
        showP('tr_sexo_e_mes_nasc', true);

        showP('trComoConheceu_md', false);      

        //Idade
        //showP('tdIdade1', false);
        //showP('tdIdade2', false);
        hideInputIdade();

        //Escolaridade
        showP('tdEscolaridade1', false);
        showP('tdEscolaridade2', false);

        //Nome
        //showP('tdNome_pessoa1', false);
        //showP('tdNome_pessoa2', false);
        hideInputNome();

        //Faixa etaria
        showP('trFaixaEtaria_md', false);

        if (dijit.byId('ckPendentesMalaDireta').checked) {
            if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
                (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                    dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

                showP('trLocalidade_md', false);
            } else {
                //Bairro
                showP('trLocalidade_md', true);
            }
        } else {
            //Bairro
            showP('trLocalidade_md', false);
        }
    } else {
        showP('tr_sexo_e_mes_nasc', false);
        
        setTitlePanel();
        if (!hasValue(dijit.byId("escolaridade_md").store.data))
            getEscolaridade();

        if (!hasValue(dijit.byId("estado_md").store.data))
            getEstados();

        if (!hasValue(dijit.byId("cbFaixaEtaria_md").store.data))
            loadFaixaEtariaMalaDireta();

        showP('trComoConheceu_md', true);

        //Idade
        showP('tdIdade1', true);
        showP('tdIdade2', true);

        //Escolaridade
        showP('tdEscolaridade1', true);
        showP('tdEscolaridade2', true);

        //Nome
        showP('tdNome_pessoa1', true);
        showP('tdNome_pessoa2', true);

        //Faixa etaria
        showP('trFaixaEtaria_md', true);

        if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

            showP('trLocalidade_md', false);
        } else {
            //Bairro
            showP('trLocalidade_md', true);
        }
       
    }
}

function loadProfissao(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Todos", id: 0 },
                    { name: "Colaborador Cyber", id: 1 },
                    { name: "Coordenador", id: 2 },
                    { name: "Professor", id: 3 }
            ]
        });
        id.store = stateStore;
        id.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostraCamposPessoaRelacionadaMalaDireta(obj) {

    showP('tr_sexo_e_mes_nasc', true);

    if (!obj.checked) {
        setTitlePanel();
        if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked)
            hideInputSexoMesNasc();

        //Idade
        //showP('tdIdade1', false);
        //showP('tdIdade2', false);
        hideInputIdade();

      
         
        if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

            showP('trLocalidade_md', false);
        } else {
            //Bairro
            showP('trLocalidade_md', true);
        }


        //Nome
        //showP('tdNome_pessoa1', false);
        //showP('tdNome_pessoa2', false);
        hideInputNome();

        //Grau Participação
        showP('trGrauParticipacao_md', false);
    }
    else {
        if (!hasValue(dijit.byId("cbGrauParentesco_md").store))
            loadQualifRelacionamentoMalaDireta();

        setTitlePanel();
        if (!hasValue(dijit.byId("estado_md").store.data))
            getEstados();

        //Idade
        showP('tdIdade1', true);
        showP('tdIdade2', true);

        if ((dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (!dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked) ||
            (dijit.byId('ckAlunosInadimplentesMalaDireta').checked &&
                dijit.byId('ckFuncionarioProfessorMalaDireta').checked)) {

            showP('trLocalidade_md', false);
        } else {
            //Bairro
            showP('trLocalidade_md', true);
        }

        //Nome
        showP('tdNome_pessoa1', true);
        showP('tdNome_pessoa2', true);

        //Grau Participação
        showP('trGrauParticipacao_md', true);
    }
}

function loadQualifRelacionamentoMalaDireta() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            xhr.get({
                url: Endereco() + "/api/pessoa/getQualifRelacionamento",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (response) {
                try {
                    dados = jQuery.parseJSON(response);
                    var domGrauParent = registry.byId("cbGrauParentesco_md");
                    var arrGrauParent = [];

                    if (hasValue(dados)) {
                        $.each(dados.retorno.qualifRelacionamentos, function (index, val) {
                            arrGrauParent.push({
                                "cd_qualif_relacionamento": val.cd_qualif_relacionamento,
                                "no_qualif_relacionamento": val.no_qualif_relacionamento
                            });
                        });
                    }
                    var store = new ItemFileReadStore({
                        data: {
                            identifier: "cd_qualif_relacionamento",
                            label: "no_qualif_relacionamento",
                            items: arrGrauParent
                        }
                    });
                    domGrauParent.setStore(store, []);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        });
    });
}

function loadTipoPapelMalaDireta() {
    require([
        "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var EDUCADORESFORNECEDORES = 1;
                var PAISEMPRESAS = 2;
                var papeis = new Array(2);
                papeis[0] = EDUCADORESFORNECEDORES;
                papeis[1] = PAISEMPRESAS;
                require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
                    xhr.post({
                        url: Endereco() + "/api/pessoa/getPapelByTipo?tipo=" + papeis,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        postData: ref.toJson(papeis)
                    }).then(function (data) {
                        try {
                            var domRelacao = registry.byId("cbRelacao_md");
                            var arrRelacao = [];

                            if (hasValue(data)) {
                                $.each(data.retorno, function (index, val) {
                                    arrRelacao.push({
                                        "cd_papel": val.cd_papel,
                                        "no_papel": val.no_papel
                                    });
                                });
                            }
                            var store = new ItemFileReadStore({
                                data: {
                                    identifier: "cd_papel",
                                    label: "no_papel",
                                    items: arrRelacao
                                }
                            });
                            domRelacao.setStore(store, []);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadMidiaMalaDireta() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMidia?status=" + STATUS_MIDIA_TODOS,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbComoConheceu_md', 'cd_midia', 'no_midia');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function loadDiasSemanaMalaDireta() {
    require([
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId("cbDiasMalaDireta_md");
                var retorno = [];
                retorno.push({ value_dia: 1 + "", no_dia: "Domingo" });
                retorno.push({ value_dia: 2 + "", no_dia: "Segunda" });
                retorno.push({ value_dia: 3 + "", no_dia: "Terça" });
                retorno.push({ value_dia: 4 + "", no_dia: "Quarta" });
                retorno.push({ value_dia: 5 + "", no_dia: "Quinta" });
                retorno.push({ value_dia: 6 + "", no_dia: "Sexta" });
                retorno.push({ value_dia: 7 + "", no_dia: "Sabado" });
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "value_dia",
                        label: "no_dia",
                        items: retorno
                    }
                });
                w.setStore(store, []);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoAlunoMalaDireta() {
    require([
       "dojo/ready",
       "dijit/registry",
       "dojo/data/ItemFileReadStore",
    ], function (ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var w = registry.byId('cbSituacaoAluno_md');
                var dados = [
                             { name: "Aguardando Matrícula", id: "9" },
                             { name: "Matriculado", id: "1" },
                             { name: "Rematriculado", id: "8" },
                             { name: "Desistente", id: "2" },
                             { name: "Encerrado", id: "4" },
                             { name: "Movido", id: "0" }
                ]
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "id",
                        label: "name",
                        items: dados
                    }
                });
                w.setStore(store, []);
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadTipoTurmaMalaDireta() {
    require([
        "dojo/store/Memory",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStoreTipo = new Memory({
                    data: [
                    { name: "Todas", id: 0 },
                    { name: "Regular", id: 1 },
                    { name: "Personalizada", id: 3 }
                    ]
                });
                dijit.byId("cbTipoTurma_md").store = statusStoreTipo;
                dijit.byId("cbTipoTurma_md").set("value", 0);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoTurmaMalaDireta() {
    require([
        "dojo/store/Memory",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStore = new Memory({
                    data: [
                    { name: "Turmas em Andamento", id: 1 },
                    { name: "Turmas em Formação", id: 3 },
                    { name: "Turmas Encerradas", id: 2 }
                    ]
                });

                dijit.byId("cbSituacaoTurma_md").store = statusStore;
                dijit.byId("cbSituacaoTurma_md").set("value", 1);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadSituacaoTurmaPPTMalaDireta() {
    require([
           "dojo/store/Memory",
           "dojo/ready",
           "dijit/registry",
           "dojo/data/ItemFileReadStore",
    ], function (Memory, ready, registry, ItemFileReadStore) {
        ready(function () {
            try {
                var statusStore = new Memory({
                    data: [
                    { name: "Turmas Ativas", id: 1 },
                    { name: "Turmas Inativas", id: 2 }
                    ]
                });

                dijit.byId("cbSituacaoTurma").store = statusStore;
                dijit.byId("cbSituacaoTurma").set("value", 1);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadFaixaEtariaMalaDireta() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/getModalidades?criterios=&cd_modalidade=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbFaixaEtaria_md', 'cd_modalidade', 'no_modalidade');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function atualizaCursosAtuaisMalaDireta(objProdutoAtual) {
    require([
        "dojo/dom",
        "dojo/dom-attr",
        "dojo/_base/xhr",
        "dojox/json/ref",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore",
        "dijit/layout/TabContainer",
        "dijit/layout/ContentPane",
        "dojox/form/CheckedMultiSelect"
    ], function (dom, domAttr, xhr, ref, ready, registry, ItemFileReadStore) {
        ready(function () {
            var cd_produtos = "";
            if (hasValue(dijit.byId('cbProdutoAtual_md').value))
                cd_produtos = dijit.byId('cbProdutoAtual_md').value.join();
            xhr.get({
                url: Endereco() + "/api/curso/getCursosProdutos?cd_produtos=" + cd_produtos,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var cursos = jQuery.parseJSON(data).retorno;
                var w = registry.byId("cbCursoAtual_md");
                var cursosCb = [];
                var valores_antigos = dijit.byId("cbCursoAtual_md").value;

                apresentaMensagem("apresentadorMensagem", data);
                if (cursos != null || cursos.length > 0)
                    $.each(cursos, function (index, val) {
                        cursosCb.push({
                            "cd_curso": val.cd_curso + "",
                            "no_curso": val.no_curso
                        });
                    });
                var store = new ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: cursosCb
                    }
                });
                w.setStore(store, []);
                dijit.byId("cbCursoAtual_md").set('value', valores_antigos);
            });
        });
    });
}

function montarMalaDiretaEtiqueta() {
    var cd_produtos = new Array();
    if (hasValue(dijit.byId('cbProdutoAtual_md').value))
        for (var i = 0; i < dijit.byId('cbProdutoAtual_md').value.length; i++)
            cd_produtos.push({ cd_produto: dijit.byId('cbProdutoAtual_md').value[i] });

    var cd_cursos = new Array();
    if (hasValue(dijit.byId('cbCursoAtual_md').value))
        for (var i = 0; i < dijit.byId('cbCursoAtual_md').value.length; i++)
            cd_cursos.push({ cd_curso: dijit.byId('cbCursoAtual_md').value[i] });

    var cd_periodos_prospects = new Array();
    if (hasValue(dijit.byId('cbPeriodo_md').value))
        for (var i = 0; i < dijit.byId('cbPeriodo_md').value.length; i++)
            cd_periodos_prospects.push({ id_periodo: parseInt(dijit.byId('cbPeriodo_md').value[i]) });

    var malasDiretaCadastro = new Array();

    if (dijit.byId('ckPendentesMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 1 });

    if (dijit.byId('ckAlunoMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 2 });

    if (dijit.byId('ckClienteMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 3 });

    if (dijit.byId('ckPessoaRelacionadaMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 4 });

    if (dijit.byId('ckFuncionarioProfessorMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 5 });

    if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked)
        malasDiretaCadastro.push({ id_cadastro: 6 });

    var dadosRetorno = {
        MalasDiretaCadastro: malasDiretaCadastro,
        id_sexo: dijit.byId('nm_sexo_md').value == 0 ? "" : dijit.byId('nm_sexo_md').value,
        nm_mes_nascimento: dijit.byId('nm_mes_nascimento_md').value == 0 ? "" : dijit.byId('nm_mes_nascimento_md').value,
        MalasDiretaCurso: cd_cursos,
        MalasDiretaProduto: cd_produtos,
        MalasDiretaPeriodo: cd_periodos_prospects,
        dc_hr_inicial: dojo.byId('timeIni_md').value,
        dc_hr_fim: dojo.byId('timeFim_md').value,
        dc_assunto: dojo.byId('descricao_md').value,
        id_tipo_profissao: TIPO_FUNCIONARIO_PROFESSOR,

        cd_tipo_papel: hasValue(dijit.byId("cbRelacao_md").value) ? convertArrInt(dijit.byId("cbRelacao_md").value) : [],
        cd_midia: hasValue(dijit.byId("cbComoConheceu_md").value) ? dijit.byId("cbComoConheceu_md").value : 0,
        dt_matricula: hasValue(dijit.byId("dtaMatricula_md").value) ? dijit.byId("dtaMatricula_md").value : null,
        dt_inicio_turma: hasValue(dijit.byId("dtaInicialTurm_md").value) ? dijit.byId("dtaInicialTurm_md").value : null,
        cd_tipo_turma: hasValue(dijit.byId("cbTipoTurma_md").value) ? dijit.byId("cbTipoTurma_md").value : 0,
        existe_turmas_filhas: document.getElementById("pesTurmasFilhas_md").checked,
        cd_situacao_turma: hasValue(dijit.byId("cbSituacaoTurma_md").value) ? dijit.byId("cbSituacaoTurma_md").value : 0,
        cd_situacao_aluno: hasValue(dijit.byId("cbSituacaoAluno_md").value) ? convertArrInt(dijit.byId("cbSituacaoAluno_md").value) : [],
        cd_faixa_etaria: hasValue(dijit.byId("cbFaixaEtaria_md").value) ? dijit.byId("cbFaixaEtaria_md").value : 0,
        cd_turma: hasValue(dojo.byId("cd_turma_pesquisa_md").value) ? parseInt(dojo.byId("cd_turma_pesquisa_md").value) : 0,
        cd_dia_semana: getDiasSemana(),
        cd_grau_parentesco: hasValue(dijit.byId("cbGrauParentesco_md").value) ? convertArrInt(dijit.byId("cbGrauParentesco_md").value) : [],

        dtIniPeriodo: getDataFormatada("dt_ini_periodo_cad_md"),
        dtFimPeriodo: getDataFormatada("dt_final_periodo_cad_md"),
        nome: hasValue(dijit.byId("nome_pessoa_md").value) ? dijit.byId("nome_pessoa_md").value : null,
        cd_escolaridade: hasValue(dijit.byId("escolaridade_md").get("value")) ? dijit.byId("escolaridade_md").get("value") : 0,
        cd_pessoa_responsavel: hasValue(dijit.byId("responsavel_md").get("value")) ? dijit.byId("responsavel_md").get("value") : 0,
        idade: hasValue(dijit.byId("idade_md").get("value")) ? dijit.byId("idade_md").get("value") : 0,
        cd_localidade: hasValue(dijit.byId("bairro_md").get("value")) ? dijit.byId("bairro_md").get("value") : 0,
        cd_estado: hasValue(dijit.byId("estado_md").get("value")) ? dijit.byId("estado_md").get("value") : 0,
        cd_cidade: hasValue(dijit.byId("cidade_md").get("value")) ? dijit.byId("cidade_md").get("value") : 0,

        nmAlunoInadimplente: !hasValue(dijit.byId('nome_pessoa_md').value) ? dijit.byId('nome_pessoa_md').value : "",
    }
    return dadosRetorno;
}

function convertArrInt(situacao) {
    var arrInt = [];

    if (!hasValue(situacao))
        return arrInt;

    for (var i = 0; i < situacao.length; i++) {
        arrInt.push(parseInt(situacao[i]));
    }
    return arrInt;
}

function abrirTurmaFKMalaDireta(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try {
        dojo.byId("trAluno").style.display = "none";
        dojo.byId("idOrigemCadastro").value = MALADIRETA;

        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKMalaDireta(fieldID, fieldNome) {
    try {
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
            dojo.byId("cd_turma_pesquisa_md").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dojo.byId("cbTurma_md").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dijit.byId('limparProTurmaFK_md').set("disabled", false);


            dijit.byId("cbTipoTurma_md").set("value", 0);
            dijit.byId('cbSituacaoTurma_md').set("value", 1);
            dijit.byId('cbTipoTurma_md').set("disabled", true);
            dijit.byId('cbSituacaoTurma_md').set("disabled", true);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarNovaViewMalaDireta() {
    try {
        limparMalaDireta();
        loadProdutoAtualMalaDireta();
        loadCursoAtualMalaDireta();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparMalaDireta() {
    clearForm("formGerarMalaDireta");
    //dijit.byId('mensagemComporMsg').set('value', "");
    if (hasValue(dijit.byId('cbPeriodo_md')) && hasValue(dijit.byId('cbPeriodo_md').store))
        dijit.byId('cbPeriodo_md').setStore(dijit.byId('cbPeriodo_md').store, [0]);
    if (hasValue(dijit.byId('cbProdutoAtual_md')) && hasValue(dijit.byId('cbProdutoAtual_md').store))
        dijit.byId('cbProdutoAtual_md').setStore(dijit.byId('cbProdutoAtual_md').store, [0]);
    if (hasValue(dijit.byId('cbCursoAtual_md')) && hasValue(dijit.byId('cbCursoAtual_md').store))
        dijit.byId('cbCursoAtual_md').setStore(dijit.byId('cbCursoAtual_md').store, [0]);
}

function visualizarEtiqueta(windowUtils) {
    try {
        apresentaMensagem('apresentadorMensagem', "");
        var mensagensWeb = new Array();

        if (!hasValue(dijit.byId('descricao_md').value))
        {
            dijit.byId('tagDestinatario').set('open', true);

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroSelecionarDescricaoEtiqueta);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            return false;
        }

        if (hasValue(dojo.byId("dt_final_periodo_cad_md").value) && !hasValue(dojo.byId("dt_ini_periodo_cad_md").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroSelecionarDataEtiqueta);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            return false;
        }

        //Valida os campos:
        if ((!dijit.byId('ckPendentesMalaDireta').checked && !dijit.byId('ckAlunoMalaDireta').checked &&
                !dijit.byId('ckClienteMalaDireta').checked && !dijit.byId('ckPessoaRelacionadaMalaDireta').checked &&
                !dijit.byId('ckFuncionarioProfessorMalaDireta').checked && !dijit.byId('ckAlunosInadimplentesMalaDireta').checked)) {

            dijit.byId('tagDestinatario').set('open', true);

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroSelecionarFiltroEtiqueta);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            return false;
        }
        if (hasValue(dojo.byId('timeIni_md').value) || hasValue(dojo.byId('timeFim_md').value))
            if (dijit.byId('timeIni_md').value > dijit.byId('timeFim_md').value || !hasValue(dojo.byId('timeIni_md').value) || !hasValue(dojo.byId('timeFim_md').value)) {
                dijit.byId('tagDestinatario').set('open', true);
                var mensagensWeb = new Array();

                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHoraInicialMaiorFinal);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

        var mala_direta = new montarMalaDiretaEtiqueta();

        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/emailMarketing/visualizarEtiqueta",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(mala_direta)
            }).then(function (data) {
                try {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', data);

                    var etiqueta = JSON.parse(data);
                    if (hasValue(etiqueta.retorno))
                        gerarEtiqueta(etiqueta.retorno);
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarEtiqueta(cd_mala_direta) {
    var url = Endereco() + "/Relatorio/GerarEtiqueta?cd_mala_direta=" + cd_mala_direta;

    dojo.xhr.get({
        url: url,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            window.open(data);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function getDiasSemana() {

    if(hasValue(dojo.byId("cbDiasMalaDireta_md").value))
        return convertArrInt(dojo.byId("cbDiasMalaDireta_md").value);

    return [];
}

function getEscolaridade() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getEscolaridade?status=" + 0,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'escolaridade_md', 'cd_escolaridade', 'no_escolaridade');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function getDataFormatada(component) {
    var dataFormatted = null;
    if (hasValue(dojo.byId(component).value)) {
        dataFormatted = dojo.date.locale.parse(dojo.byId(component).value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        dataFormatted = dojo.date.locale.format(dataFormatted, { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
    }
    return dataFormatted;
}

function getPessoaResponsavel() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/Pessoa/getPessoasResponsavel",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dados) {
            try {
                loadSelect(jQuery.parseJSON(dados).retorno, 'responsavel_md', 'cd_pessoa', 'no_pessoa');
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
        });
    } catch (exception) {

    }
}

function getEstados() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getAllEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try {
                    loadSelect(dataEstado.retorno, 'estado_md', 'cd_localidade', 'no_localidade');
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemCidade', error);
                });
        });
}

function getCidadesPorIdEstado(cd_estado) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/GetCidade?idEstado=" + cd_estado,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var cidades = JSON.parse(data);
                loadSelect(cidades.retorno, 'cidade_md', 'cd_localidade', 'no_localidade');
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(idApresentador, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getBairroPorIdCidade(cd_cidade) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getBairroPorCidade?cd_cidade=" + cd_cidade + "&cd_bairro=0",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var cidades = JSON.parse(data);
                loadSelect(cidades.retorno, 'bairro_md', 'cd_localidade', 'no_localidade');
            }
            catch (e) {
                postGerarLog(e);
            }

        },
        function (error) {
            apresentaMensagem(idApresentador, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function hideInputNome() {
    if (!dijit.byId('ckPendentesMalaDireta').checked &&
        !dijit.byId('ckAlunoMalaDireta').checked &&
        !dijit.byId('ckClienteMalaDireta').checked &&
        !dijit.byId('ckPessoaRelacionadaMalaDireta').checked &&
        !dijit.byId('ckFuncionarioProfessorMalaDireta').checked &&
        !dijit.byId('ckAlunosInadimplentesMalaDireta').checked) {

        //Nome
        showP('tdNome_pessoa1', false);
        showP('tdNome_pessoa2', false);
    }
}

function hideInputIdade() {
    if (!dijit.byId('ckAlunoMalaDireta').checked &&
        !dijit.byId('ckClienteMalaDireta').checked &&
        !dijit.byId('ckPessoaRelacionadaMalaDireta').checked &&
        !dijit.byId('ckFuncionarioProfessorMalaDireta').checked &&
        !dijit.byId('ckAlunosInadimplentesMalaDireta').checked) {

        //Idade
        showP('tdIdade1', false);
        showP('tdIdade2', false);
    }
}

function hideInputSexoMesNasc() {
    if (!dijit.byId('ckPendentesMalaDireta').checked &&
        !dijit.byId('ckAlunoMalaDireta').checked &&
        !dijit.byId('ckClienteMalaDireta').checked &&
        !dijit.byId('ckPessoaRelacionadaMalaDireta').checked &&
        !dijit.byId('ckFuncionarioProfessorMalaDireta').checked) {

        showP('tr_sexo_e_mes_nasc', false);
    }
}

function hideInputDias() {
    if (!dijit.byId('ckPendentesMalaDireta').checked &&
        !dijit.byId('ckAlunoMalaDireta').checked) {

        showP('trDiasMalaDireta', false);
    }
}

function setTitlePanel() {
    var titulo = "Período da Data Inicial/Data Final";

    if (dijit.byId('ckPendentesMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de cadastro do Prospect";

    if (dijit.byId('ckAlunoMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de cadastro do Aluno";

    if (dijit.byId('ckClienteMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de cadastro do Cliente";

    if (dijit.byId('ckPessoaRelacionadaMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de cadastro da Pessoa Relacionada";

    if (dijit.byId('ckFuncionarioProfessorMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de cadastro do Funcionário/Professor";

    if (dijit.byId('ckAlunosInadimplentesMalaDireta').checked)
        titulo = "Período da Data Inicial/Data Final de Contas a Receber";

    dojo.attr(dijit.byId('panelTituloPeriodoMalaDireta'), "title", titulo);
}