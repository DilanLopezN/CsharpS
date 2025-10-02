var alteraPermissoes = false;
var GRUPO = 0, GRUPO_MASTER = 1;
var TIPO = new Number();

function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if ((tab.getAttribute('widgetId') == 'tabContainer_tablist_tabPermissoes' && !dijit.byId("gridPermissoes")))
            criarPermissao();
    } catch (e) {
        postGerarLog(e);
    }
}

function configurarGrupo() {
    try{
        dojo.ready(function () {
            var parametros = getParamterosURL();
            if (hasValue(parametros['tipo'])) {
                switch (eval(parametros['tipo'])) {
                    case GRUPO:
                        document.getElementById("labelTitulos").innerHTML = "Grupo";
                        document.getElementById("titulo").innerHTML = "Grupo";
                        document.getElementById("cadGrupo_title").innerHTML = "Cadastro de Grupo de Permissões";
                        TIPO = GRUPO;
                        break;
                    case GRUPO_MASTER:
                        TIPO = GRUPO_MASTER;
                        if (jQuery.parseJSON(MasterGeral())) {
                            document.getElementById("labelTitulos").innerHTML = "Grupo Master";
                            document.getElementById("titulo").innerHTML = "Grupo Master";
                            document.getElementById("cadGrupo_title").innerHTML = "Cadastro de Grupo Master de Permissões";
                        } else {
                            dojo.byId("painelPesquisa").style.display = "none";
                            showCarregando();
                            var mensagensWeb = [];
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTelaFunadacao);
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        }
                        break;
                }
                montarCadastroGrupo();
            }
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//#region funções para checkbox

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridGrupo';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_grupo", grid._by_idx[rowIndex].item.cd_grupo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_grupo', 'selecionadoGrupo', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_grupo', 'selecionadoGrupo', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}


function atualizaCheckBox(dijitObj, value) {
    try{
        if (hasValue(dijitObj) && !dijitObj.disabled) {
            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangePermissoes(rowIndex, campo, obj) {
    try{
        dijit.byId('ckIdAtualizarGrupo')._onChangeActive = false;

        if (TIPO == GRUPO && !$.parseJSON(MasterGeral()))
            dijit.byId('ckIdAtualizarGrupo').set('checked',false);

        var gridPermissoes = dijit.byId('gridPermissoes');
        marcarFilhosRecursivo(gridPermissoes._by_idx[rowIndex].item, campo, obj, false);

        dijit.byId('ckIdAtualizarGrupo')._onChangeActive = true;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPermissoes(value, rowIndex, obj, k) {
    try{
        var gridPermissoes = dijit.byId('gridPermissoes');
        var icon;
        var id = k.field + '_Selected_' + gridPermissoes._by_idx[rowIndex].item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";
        setTimeout("configuraCheckBoxPermissoes(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "', " + gridPermissoes._by_idx[rowIndex].item.ehPermitidoEditar + ")", 1);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxPermissoes(value, rowIndex, field, id, ehPermitidoEditar) {
    try{
        var gridPermissoes = dijit.byId("gridPermissoes");

        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value,
                onChange: function (b) { checkBoxChangePermissoes(rowIndex, field, this) }
            }, id);
            checkBox.set('disabled', !ehPermitidoEditar && field != "visualizar");
        }
        else {
            var dijitObj = dijit.byId(id);

            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
            dijitObj.onChange = function (b) { checkBoxChangePermissoes(rowIndex, field, this) };
            dijitObj.set('disabled', !ehPermitidoEditar && field != "visualizar");
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeUsuario(rowIndex) {
    try{
        var gridUsuarioGrupo = dijit.byId('gridUsuarioGrupo');
        gridUsuarioGrupo._by_idx[rowIndex].item.selecionado = !gridUsuarioGrupo._by_idx[rowIndex].item.selecionado;
    } catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region Inicio da grade Grupo
function montarCadastroGrupo() {
    require([
                "dojo/dom",
                "dojo/ready",
                "dojo/_base/xhr",
                "dojox/grid/EnhancedGrid",
                "dojox/grid/enhanced/plugins/Pagination",
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory",
                "dojo/query",
                "dijit/form/Button",
                "dijit/form/DropDownButton",
                "dijit/DropDownMenu",
                "dijit/MenuItem",
                "dojo/on",
                "dojo/domReady!",
                "dojo/parser"
    ], function (dom, ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, DropDownButton, DropDownMenu, MenuItem, on) {
        try{
            ready(function () {
                //if (hasValue(document.getElementById("pesquisaGrupo"))) {
                if (!jQuery.parseJSON(MasterGeral()) && TIPO == GRUPO_MASTER) {
                    var mensagensWeb = [];
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTelaFunadacao);
                    apresentaMensagem('apresentadorMensagem', mensagensWeb);
                    return false;
                }
                var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/permissao/getgruposearch?descricao=&inicio=false" + "&tipoGrupo=" + TIPO,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
            ), Memory({}));
                var gridGrupo = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        {
                            name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoGrupo", width: "5%",
                            styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox
                        },
                         //   { name: "Código", field: "cd_grupo", width: "9%",styles: "text-align: right; min-width:50px; max-width:50px;" },
                            { name: "Grupo de Permissões", field: "no_grupo", width: "95%" }
                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["18", "36", "72", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "18",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridGrupo"); // make sure you have a target HTML element with this id
                gridGrupo.startup();
                gridGrupo.canSort = function (col) { return Math.abs(col) != 1; };
                gridGrupo.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                        getLimpar('#formGrupo');
                        apresentaMensagem('apresentadorMensagem', null);
                        keepValues(item, gridGrupo, false);
                        var tabContainer = dijit.byId("tabContainer");
                        tabContainer.selectChild(tabContainer.getChildren()[0]);
                        dijit.byId("cadGrupo").show();
                        dijit.byId('paiGridEscolaMaster').resize();
                        IncluirAlterar(0, 'divAlterarGrupo', 'divIncluirGrupo', 'divExcluirGrupo', 'apresentadorMensagemGrupo', 'divCancelarGrupo', 'divLimparGrupo');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridGrupo.pagination.plugin._paginator.plugin.connect(gridGrupo.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridGrupo, 'cd_grupo', 'selecionaTodos');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridGrupo, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_grupo', 'selecionadoGrupo', -1, 'selecionaTodos', 'selecionaTodos', 'gridGrupo')", gridGrupo.rowsPerPage * 3);
                    });
                });

                if (TIPO == GRUPO) {
                    // Montando GridUsuarioGrupo
                    var gridUsuarioGrupo = new EnhancedGrid({
                        store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                        structure: [
                            { name: " ", field: "selecionadoUsuarios", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxUsuario },
                            //{ name: "Código", field: "cd_usuario", width: "75px", styles: "text-align: right; min-width:50px; max-width:50px;" },
                            { name: "Usuário", field: "no_login", width: "100%" }
                        ],
                        plugins: {}
                    }, "gridUsuarioGrupo");

                    gridUsuarioGrupo.startup();
                }

                if (TIPO == GRUPO_MASTER) {
                    // Montando GridUsuarioGrupo
                    var gridEscolas = new EnhancedGrid({
                        store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                        structure: [
                            { name: " ", field: "selecionado", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxEscola },
                            { name: "Escola", field: "no_pessoa", width: "98%" }
                        ],
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "6",
                                gotoButton: true,
                                maxPageStep: 5,
                                position: "button",
                                plugins: { nestedSorting: true }
                            }
                        }
                    }, "gridEscolas");

                    gridEscolas.startup();
                }

                IncluirAlterar(1, 'divAlterarGrupo', 'divIncluirGrupo', 'divExcluirGrupo', 'apresentadorMensagemGrupo', 'divCancelarGrupo', 'divLimparGrupo');

                //}
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        if (TIPO == GRUPO)
                            IncluirGrupo('apresentadorMensagemGrupo', null, 'formGrupo');
                        if (TIPO == GRUPO_MASTER)
                            incluirGrupoEscola('apresentadorMensagemGrupo', null, 'formGrupo');
                    }
                }, "incluirGrupo");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        if (TIPO == GRUPO)
                            AlterarGrupo();
                        if (TIPO == GRUPO_MASTER)
                            alterarGrupoEscola();
                    }
                }, "alterarGrupo");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '',
                            function executaRetorno() {
                                if (TIPO == GRUPO)
                                    DeletarGrupo();
                                if (TIPO == GRUPO_MASTER)
                                    DeletarGrupoEscola();
                            });
                    }
                }, "deleteGrupo");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        limparGrupo();
                    }
                }, "limparGrupo");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        try{
                            var tabContainer = dijit.byId("tabContainer");
                            tabContainer.selectChild(tabContainer.getChildren()[0]);
                            apresentaMensagem('apresentadorMensagem', null);
                            keepValues(null, gridGrupo, false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarGrupo");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadGrupo").hide();
                    }
                }, "fecharGrupo");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        PesquisarGrupo(true);
                    }
                }, "pesquisarGrupo");
                decreaseBtn(document.getElementById("pesquisarGrupo"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            showCarregando();
                            destroyCreateGridPermissao();
                            limparGrupo();
                            var tabContainer = dijit.byId("tabContainer");
                            tabContainer.selectChild(tabContainer.getChildren()[0]);
                            apresentaMensagem('apresentadorMensagem', null);
                            IncluirAlterar(1, 'divAlterarGrupo', 'divIncluirGrupo', 'divExcluirGrupo', 'apresentadorMensagemGrupo', 'divCancelarGrupo', 'divLimparGrupo');
                            alteraPermissoes = false;
                            dijit.byId("cadGrupo").show();
                            if (TIPO == GRUPO) {
                                limparGrupo();
                                dojo.byId('paiGridUsuarioGrupo').style.display = '';
                                dojo.byId('paiGridEscolaMaster').style.display = 'none';
                                xhr.get({
                                    url: Endereco() + "/api/permissao/getUsuarioByGrupo?cdGrupo=0",
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    try {
                                        vizualizarCheckBoxAtualizarGrupo(jQuery.parseJSON(data).retorno, GRUPO);
                                        destroyCreateGridUsuarioGrupo(jQuery.parseJSON(data).retorno, GRUPO);
                                        apresentaMensagem('apresentadorMensagemGrupo', null);
                                        showCarregando();
                                    }  catch (e) {
                                        postGerarLog(e);
                                    }
                                }, function (error) {
                                    apresentaMensagem('apresentadorMensagemGrupo', error);
                                    showCarregando();
                                });
                            }
                            else {
                                dojo.byId('paiGridUsuarioGrupo').style.display = 'none';
                                dojo.byId('paiGridEscolaMaster').style.display = '';
                                clearGrid(gridEscolas);
                                apresentaMensagem('apresentadorMensagemGrupo', null);
                                vizualizarCheckBoxAtualizarGrupo(0, TIPO);
                                showCarregando();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoGrupo");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        xhr.get({
                            url: !hasValue(document.getElementById("pesquisaGrupo").value) ? Endereco() + "/permissao/geturlrelatorioGrupo?" + getStrGridParameters('gridGrupo') + "descricao=&inicio=" + document.getElementById("inicioDescGrupo").checked + "&tipo=" + TIPO : Endereco() + "/permissao/geturlrelatorioGrupo?" + getStrGridParameters('gridGrupo') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaGrupo").value) + "&inicio=" + document.getElementById("inicioDescGrupo").checked + "&tipo=" + TIPO,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data.retorno, '765px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioGrupo");

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridGrupo.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        try{
                            if (TIPO == GRUPO)
                                eventoRemoverGrupo(gridGrupo.itensSelecionados, 'DeletarGrupo(itensSelecionados)');
                            if (TIPO == GRUPO_MASTER)
                                eventoRemoverGrupoMaster(gridGrupo.itensSelecionados, 'DeletarGrupoMaster(itensSelecionados)');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);


                //link grade Escola
                //Metodos para criação do link
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarEscolasFK();
                                    });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisarEscolasFK();
                                    });
                                    abrirPessoaFK(false);
                                });
                            else
                                abrirPessoaFK(false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirEscolaFK");

                var menuCurso = new DropDownMenu({ style: "height: 25px" });
                var acaoRemoverCurso = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_pessoa', gridEscolas);
                    }
                });
                menuCurso.addChild(acaoRemoverCurso);

                var buttonCurso = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasRelac",
                    dropDown: menuCurso,
                    id: "acoesRelacionadasEscola"
                });
                dojo.byId("linkAcoesEscola").appendChild(buttonCurso.domNode);

                //fim
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    dojo.query("#nomPessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarUsuarioFK();
                                    });
                                    abrirUsuarioFK(false);
                                });
                            else
                                abrirUsuarioFK(false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirUsuarioFK");

                var menuUsuario = new DropDownMenu({ style: "height: 25px" });
                var acaoRemoverUsuario = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_usuario', dijit.byId("gridUsuarioGrupo"));
                    }
                });
                menuUsuario.addChild(acaoRemoverUsuario);

                var buttonUsuario = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasUsuario",
                    dropDown: menuUsuario,
                    id: "acoesRelacionadasUsuario"
                });
                dojo.byId("linkAcoesUsuario").appendChild(buttonUsuario.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridGrupo, 'todosItens', ['pesquisarGrupo', 'relatorioGrupo']);
                        PesquisarGrupo(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridGrupo', 'selecionadoGrupo', 'cd_grupo', 'selecionaTodos', ['pesquisarGrupo', 'relatorioGrupo'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            try {
                                var parametros = getParamterosURL();
                                if (hasValue(parametros['tipo']))
                                    if (eval(parametros['tipo']) == GRUPO)
                                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322978',
                                            '765px',
                                            '771px');
                                    else
                                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322977',
                                            '765px',
                                            '771px');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        });
                }

                dijit.byId("ckIdAtualizarGrupo").on("change", function (e) {
                    try{
                        if (TIPO == GRUPO && !$.parseJSON(MasterGeral()) && (e == true)) {
                            var mensagensWeb = [];
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgPesonalizarGrupoMaster);
                            apresentaMensagem('apresentadorMensagemGrupo', mensagensWeb);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                adicionarAtalhoPesquisa(['pesquisaGrupo'], 'pesquisarGrupo', ready);
                showCarregando();

            })
        } catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region destroyCreateGridUsuarioGrupo

function destroyCreateGridUsuarioGrupo(dados, tipo) {
    try{
        dijit.byId('tabContainer').resize();
        if (tipo == GRUPO)
            dojo.ready(function () {
                var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
                dijit.byId("gridUsuarioGrupo").setStore(dataStore);
            });
    } catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region  funções recursivas
//Função não perfomática para buscar o pai e marcar ele. A função não é perfomática, pois é O(n*2) e poderia ser O(log n). Entretanto, ela percorre somente os itens da árvore mostrados na tela.
function marcarPaisRecursivo(item, campo, obj) {
    try{
        var gridPermissoes = dijit.byId('gridPermissoes');
        if (hasValue(item) && item.pai != 0) {
            var pai = null;

            for (var p = 0; p < gridPermissoes._by_idx.length && pai == null; p++)
                pai = marcarRecursivo(item.pai, gridPermissoes._by_idx[p].item, campo, obj);
            marcarPaisRecursivo(pai, campo, obj);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function marcarRecursivo(pai, item, campo, obj) {
    try{
        var retorno = null;

        if (hasValue(item))
            // Achou o item pai:
            if (item.id[0] == pai) {
                if (obj.checked || !verificaOutrosFilhosMarcadosRecursivo(item, campo, item.id, pai)) {
                    if (campo == 'alterar')
                        item.alterar[0] = obj.checked;
                    else if (campo == 'incluir')
                        item.incluir[0] = obj.checked;
                    else if (campo == 'excluir')
                        item.excluir[0] = obj.checked;
                    else
                        item.visualizar[0] = obj.checked;
                    atualizaCheckBox(dijit.byId(campo + '_Selected_' + item._0), obj.checked);
                }

                retorno = item;
            }
            else
                if (hasValue(item.children) && item.children.length > 0) {
                    for (var idx1 = 0; idx1 < item.children.length; idx1++) {
                        var retornoRecursivo = marcarRecursivo(pai, item.children[idx1], campo, obj);

                        if (retornoRecursivo != null) {
                            retorno = retornoRecursivo;
                            break;
                        }
                    }
                }

        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function marcarFilhosRecursivo(item, campo, obj, filho) {
    try{
        if (hasValue(item)) {
            if (campo == 'alterar') {
                if (hasValue(item.alterar) && item.ehPermitidoEditar[0]) {
                    item.alterar[0] = obj.checked;
                    if (!filho)
                        marcarPaisRecursivo(item, campo, obj);
                    if (obj.checked)
                        item.visualizar[0] = obj.checked;
                    if (hasValue(document.getElementById('alterar_Selected_' + item._0))) {
                        atualizaCheckBox(dijit.byId('alterar_Selected_' + item._0), obj.checked);
                        if (obj.checked) {
                            atualizaCheckBox(dijit.byId('visualizar_Selected_' + item._0), obj.checked);
                            if (!filho)
                                marcarPaisRecursivo(item, 'visualizar', obj);
                        }
                    }
                }
            }
            else if (campo == 'incluir') {
                if (hasValue(item.incluir) && item.ehPermitidoEditar[0]) {
                    item.incluir[0] = obj.checked;
                    if (!filho)
                        marcarPaisRecursivo(item, campo, obj);
                    if (obj.checked)
                        item.visualizar[0] = obj.checked;
                    if (hasValue(document.getElementById('incluir_Selected_' + item._0))) {
                        atualizaCheckBox(dijit.byId('incluir_Selected_' + item._0), obj.checked);
                        if (obj.checked) {
                            atualizaCheckBox(dijit.byId('visualizar_Selected_' + item._0), obj.checked);
                            if (!filho)
                                marcarPaisRecursivo(item, 'visualizar', obj);
                        }
                    }
                }
            }
            else if (campo == 'excluir') {
                if (hasValue(item.excluir) && item.ehPermitidoEditar[0]) {
                    item.excluir[0] = obj.checked;
                    if (!filho)
                        marcarPaisRecursivo(item, campo, obj);
                    if (obj.checked)
                        item.visualizar[0] = obj.checked;
                    if (hasValue(document.getElementById('excluir_Selected_' + item._0))) {
                        atualizaCheckBox(dijit.byId('excluir_Selected_' + item._0), obj.checked);
                        if (obj.checked) {
                            atualizaCheckBox(dijit.byId('visualizar_Selected_' + item._0), obj.checked);
                            if (!filho)
                                marcarPaisRecursivo(item, 'visualizar', obj);
                        }
                    }
                }
            }
            else {
                if (hasValue(item.visualizar)) {
                    item.visualizar[0] = obj.checked;
                    if (!filho)
                        marcarPaisRecursivo(item, campo, obj);
                    if (!obj.checked) {
                        item.incluir[0] = obj.checked;
                        marcarPaisRecursivo(item, 'incluir', obj);
                        item.alterar[0] = obj.checked;
                        marcarPaisRecursivo(item, 'alterar', obj);
                        item.excluir[0] = obj.checked;
                        marcarPaisRecursivo(item, 'excluir', obj);
                    }
                    if (hasValue(document.getElementById('visualizar_Selected_' + item._0))) {
                        atualizaCheckBox(dijit.byId('visualizar_Selected_' + item._0), obj.checked);
                        if (!obj.checked) {
                            atualizaCheckBox(dijit.byId('incluir_Selected_' + item._0), obj.checked);
                            atualizaCheckBox(dijit.byId('alterar_Selected_' + item._0), obj.checked);
                            atualizaCheckBox(dijit.byId('excluir_Selected_' + item._0), obj.checked);
                        }
                    }
                }
            }
            if (hasValue(item.children) && obj.checked)
                for (var idx1 = 0; idx1 < item.children.length; idx1++)
                    marcarFilhosRecursivo(item.children[idx1], campo, obj, true);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function verificaOutrosFilhosMarcadosRecursivo(item, campo, id, pai) {
    try{
        var retorno = false;
        var primeiroMarcado = true;

        // Realiza a recursão verificando se tem algum outro item marcado para o pai:
        for (var idx2 = 0; idx2 < item.children.length && !retorno; idx2++) {
            var itemMarcado = eval("item.children[idx2]." + campo + "[0]");

            if (itemMarcado)
                primeiroMarcado = false;
            if (!primeiroMarcado)
                retorno = retorno || itemMarcado;
        }
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function limparPermissoesRecursivo(storechildren) {
    try{
        if (hasValue(storechildren)) {
            for (var i = 0; i < storechildren.length; i++) {
                storechildren[i].visualizar[0] = false;
                storechildren[i].alterar[0] = false;
                storechildren[i].incluir[0] = false;
                storechildren[i].excluir[0] = false;
                if (hasValue(storechildren[i].children))
                    limparPermissoesRecursivo(storechildren[i].children);
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//#endregion

function montarUsuarioGrupoUsuarioSelecionada(dataItemGrupo, dataUsuariobyEscola) {
    var storeGridUsuarioGrupo;
    require(["dojo/ready"], function (ready) {
        ready(function () {
            try{
                if (dataItemGrupo != null && dataUsuariobyEscola != null) {

                    $.each(dataItemGrupo, function (index, val) {
                        $.each(dataUsuariobyEscola, function (idx, value) {
                            if (val.cd_usuario == value.cd_usuario) {
                                dataUsuariobyEscola[idx].selecionado = true;
                            }
                        })
                    })
                }
                else
                    if ((dataItemGrupo == null || dataItemGrupo.length <= 0) && (dataUsuariobyEscola != null || dataUsuariobyEscola.length > 0))
                        $.each(dataUsuariobyEscola, function (idx, value) {
                            dataUsuariobyEscola[idx].selecionado = false;
                        })
            } catch (e) {
                postGerarLog(e);
            }
        })
    })
    return dataUsuariobyEscola;
}

function destroyCreateGridPermissao() {
    try{
        if (hasValue(dijit.byId("gridPermissoes"))) {
            dijit.byId("gridPermissoes").destroy();
            $('<div>').attr('id', 'gridPermissoes').appendTo('#paiGridPermissoes');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//#region formats CheckBox das grades de cadastro



function formatCheckBoxUsuario(value, rowIndex, obj) {
    try{
        var gridName = 'gridUsuarioGrupo';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosUsuario');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_usuario", grid._by_idx[rowIndex].item.cd_usuario);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_usuario', 'selecionadoUsuarios', -1, 'selecionaTodosUsuario', 'selecionaTodosUsuario', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_usuario', 'selecionadoUsuarios', " + rowIndex + ", '" + id + "', 'selecionaTodosUsuario', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}


function formatCheckBoxEscola(value, rowIndex, obj) {
    try{
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  style='height: 16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//#endregion

function retirarChildrenLenthIgualZero(array) {
    try{
        for (var i = 0; hasValue(array) && i < array.length; i++)
            if (hasValue(array[i].children) && array[i].children.length > 0)
                retirarChildrenLenthIgualZero(array[i].children);
            else
                array[i] = {
                    cd_direito_grupo: array[i].cd_direito_grupo,
                    ehPermitidoEditar: array[i].ehPermitidoEditar,
                    alterar: array[i].alterar,
                    excluir: array[i].excluir,
                    id: array[i].id,
                    incluir: array[i].incluir,
                    pai: array[i].pai,
                    permissao: array[i].permissao,
                    visualizar: array[i].visualizar
                };

        return array;
    } catch (e) {
        postGerarLog(e);
    }
}

function limparGrupo() {
    try{
        var gridUsuarioGrupo = dijit.byId('gridUsuarioGrupo');
        var compGridEscolas = dijit.byId("gridEscolas");
        if (TIPO == GRUPO)
            dijit.byId("paiGridUsuarioGrupo").set("open", true);
        else {
            dijit.byId("paiGridEscolaMaster").set("open", true);
            clearGrid(compGridEscolas);
            if (hasValue(compGridEscolas.itensSelecionados))
                compGridEscolas.itensSelecionados = new Array();
        }

        getLimpar('#formGrupo')
        IncluirAlterar(1, 'divAlterarGrupo', 'divIncluirGrupo', 'divExcluirGrupo', 'apresentadorMensagemGrupo', 'divCancelarGrupo', 'divLimparGrupo');
        document.getElementById("cd_grupo").value = null;

        //Limpa as informações da grade de usuarios:
        if (hasValue(gridUsuarioGrupo)) {
            for (var i = 0; i < gridUsuarioGrupo._by_idx.length; i++)
                gridUsuarioGrupo._by_idx[i].item.selecionado = false;
            gridUsuarioGrupo.update();
        }

        //Limpa as informações da grade de permissões:
        var gridPermissoes = dijit.byId('gridPermissoes');

        if (hasValue(gridPermissoes)) {
            for (var i = 0; i < gridPermissoes._by_idx.length; i++) {
                gridPermissoes._by_idx[i].item.visualizar[0] = false;
                gridPermissoes._by_idx[i].item.alterar[0] = false;
                gridPermissoes._by_idx[i].item.incluir[0] = false;
                gridPermissoes._by_idx[i].item.excluir[0] = false;
                if (hasValue(gridPermissoes._by_idx[i].item.children))
                    limparPermissoesRecursivo(gridPermissoes._by_idx[i].item.children);
            }
            gridPermissoes.update();
            gridUsuarioGrupo._clearData();
            gridUsuarioGrupo._by_idx = [];
            gridUsuarioGrupo.update();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function montarUsuariosSelecionados() {
    try{
        var dados = [];
        if (hasValue(dijit.byId("gridUsuarioGrupo")) && hasValue(dijit.byId("gridUsuarioGrupo").store.objectStore.data)) {
            var storeGrupoUsuario = dijit.byId("gridUsuarioGrupo").store.objectStore.data;
            $.each(storeGrupoUsuario, function (index, val) {
                //if (val.selecionado == true)
                dados.push({ cd_usuario: val.cd_usuario });
            });
            return dados;
        }
        return null;
    } catch (e) {
        postGerarLog(e);
    }
}

//#region Permissões
function Permissoes(id, alterar, excluir, incluir, visualizar, permissao, cd_direito_grupo, children) {
    try{
        this.cd_direito_grupo = cd_direito_grupo;
        this.alterar = alterar;
        this.excluir = excluir;
        this.id = id;
        this.incluir = incluir;
        this.permissao = permissao;
        this.children = children;
        this.visualizar = visualizar;
    } catch (e) {
        postGerarLog(e);
    }
}

function getPermissoesFromGridRecursivo(array) {
    try{
        var arrayPermissoes = new Array();
        for (var i = 0; hasValue(array) && i < array.length; i++)
            arrayPermissoes[i] = new Permissoes(array[i].id[0], array[i].alterar[0], array[i].excluir[0], array[i].incluir[0], array[i].visualizar[0], array[i].permissao[0], array[i].cd_direito_grupo[0], getPermissoesFromGridRecursivo(array[i].children));
        return arrayPermissoes;
    } catch (e) {
        postGerarLog(e);
    }
}

function criarPermissao() {
    alteraPermissoes = true;
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/api/permissao/getfuncionalidadesgrupoarvore?cdGrupo=" + document.getElementById("cd_grupo").value,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                apresentaMensagem('apresentadorMensagemGrupo', data);
                var dataItens = jQuery.parseJSON(eval(data)).retorno;
                if (hasValue(dataItens))
                    dataItens = retirarChildrenLenthIgualZero(dataItens);
                /* set up data store */
                var data = {
                    identifier: 'id',
                    label: 'permissao',
                    items: dataItens
                };
                var store = new dojo.data.ItemFileWriteStore({ data: data });
                var model = new dijit.tree.ForestStoreModel({ store: store, childrenAttrs: ['children'] });

                /* set up layout */
                var layout = [
                  { name: 'Permissão', field: 'permissao', width: '56%' },
                  { name: 'Visualizar', field: 'visualizar', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
                  { name: 'Incluir', field: 'incluir', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
                  { name: 'Alterar', field: 'alterar', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
                  { name: 'Excluir', field: 'excluir', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
                  { name: '', field: 'id', width: '1%', styles: "display: none;" },
                  { name: '', field: 'pai', width: '1%', styles: "display: none;" }
                ];

                /* create a new grid: */
                if (dijit.byId("gridPermissoes"))
                    destroyCreateGridPermissao();
                var gridPermissoes = new dojox.grid.LazyTreeGrid({
                    id: 'gridPermissoes',
                    treeModel: model,
                    structure: layout
                }, document.createElement('div'));

                dojo.byId("gridPermissoes").appendChild(gridPermissoes.domNode);
                gridPermissoes.startup();
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemGrupo', error);
        });
    });
}

function GrupoPermissao(grupo, permissoes, usuariogrupo) {
    try{
        this.grupo = grupo;
        this.permissoes = permissoes;
        this.usuariogrupo = usuariogrupo;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  - abrirPessoaFK  - pesquisarEscolasFK() - limparPesquisaEscolaFK - retornarPessoa - clearGrid

function abrirPessoaFK(isPesquisa) {
    dojo.ready(function () {
        try{
            dijit.byId("fkPessoaPesq").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaPesq").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function abrirUsuarioFK(isPesquisa) {
    try{
        limparPesquisaUsuarioFK();
        dojo.byId("idOrigemUsuarioFK").value = CADGRUPO;
        pesquisarUsuarioFK();
        dijit.byId("proUsuario").show();
        apresentaMensagem('apresentadorMensagemProUsuario', null);
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEscolasFK() {
    require([
     "dojo/store/JsonRest",
     "dojo/data/ObjectStore",
     "dojo/store/Cache",
     "dojo/store/Memory",
     "dojo/ready",
     "dojo/domReady!",
     "dojo/parser"],
 function (JsonRest, ObjectStore, Cache, Memory, ready) {
     ready(function () {
         try{
             var myStore = Cache(
                  JsonRest({
                      target: Endereco() + "/api/escola/getEscolaSearchFK?nome=" + dojo.byId("_nomePessoaFK").value +
                                    "&fantasia=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                    "&cnpj=" + dojo.byId("CnpjCpf").value,
                      handleAs: "json",
                      headers: { "Accept": "application/json", "Authorization": Token() }
                  }), Memory({}));

             dataStore = new ObjectStore({ objectStore: myStore });
             var grid = dijit.byId("gridPesquisaPessoa");
             grid.setStore(dataStore);
             grid.layout.setColumnVisibility(5, false)
         } catch (e) {
             postGerarLog(e);
         }
     })
 });
}

function limparPesquisaEscolaFK() {
    try{
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    } catch (e) {
        postGerarLog(e);
    }
}

// função que retorna a escola selecionada.
function retornarPessoa() {
    try{
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadGrupo").open) {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_pessoa');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_pessoa", { cd_pessoa: value.cd_pessoa, no_pessoa: value.dc_reduzido_pessoa, cd_grupo:0 });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    } catch (e) {
        postGerarLog(e);
    }
}
function retornarUsuarioFK() {
    try{
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        var gridUsuarios = dijit.byId("gridUsuarioGrupo");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadGrupo").open) {
                var storeGridUsuarios = (hasValue(gridUsuarios) && hasValue(gridUsuarios.store.objectStore.data)) ? gridUsuarios.store.objectStore.data : [];
                quickSortObj(gridUsuarios.store.objectStore.data, 'cd_usuario');
                $.each(gridUsuarioSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridUsuarios.store.objectStore.data, "cd_usuario", { cd_usuario: value.cd_usuario, no_login: value.no_login });
                });
                gridUsuarios.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridUsuarios.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}


function clearGrid(nameGrid) {
    try{
        var limparGrid = dijit.byId(nameGrid);
        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) });
        limparGrid.setStore(dataStore);
        limparGrid.update();
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  Inicio da Percistência do Grupo

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formGrupo');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(null, dijit.byId('gridGrupo'), true);

            var tabContainer = dijit.byId("tabContainer");
            tabContainer.selectChild(tabContainer.getChildren()[0]);

            dijit.byId("cadGrupo").show();
            dijit.byId('paiGridEscolaMaster').resize();
            IncluirAlterar(0, 'divAlterarGrupo', 'divIncluirGrupo', 'divExcluirGrupo', 'apresentadorMensagemGrupo', 'divCancelarGrupo', 'divLimparGrupo');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverGrupo(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () {
                DeletarGrupo(itensSelecionados);
            });
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverGrupoMaster(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () {
                var listaSysGrupos = [];
                if (hasValue(itensSelecionados) && itensSelecionados.length > 0) {
                    for (var i = 0; i < itensSelecionados.length; i++) {
                        listaSysGrupos.push({
                            no_grupo: itensSelecionados[i].no_grupo,
                            cd_grupo: itensSelecionados[i].cd_grupo
                        });
                    }
                }
                DeletarGrupoEscola(listaSysGrupos);
            });
    } catch (e) {
        postGerarLog(e);
    }
}

function PesquisarGrupo(limparItens) {
    var descricao = encodeURIComponent(dijit.byId("pesquisaGrupo").get('value'));

    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            if (hasValue(document.getElementById("pesquisaGrupo"))) {
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/permissao/getgruposearch?descricao=" + descricao + "&inicio=" + document.getElementById("inicioDescGrupo").checked + "&tipoGrupo=" + TIPO,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_grupo"
                    }), Memory({ idProperty: "cd_grupo" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridGrupo = dijit.byId("gridGrupo");
                gridGrupo.store.objectStore.data = dataStore;
                gridGrupo.setStore(dataStore);
            }
            if (limparItens)
                gridGrupo.itensSelecionados = [];

            gridGrupo.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function verificarPermissoesSelecionadasFromGridRecursivo(array) {
    try{
        var retorno = true; // Não tem nenhuma permissão marcada.

        for (var i = 0; i < array.length && retorno; i++)
            if (array[i].visualizar[0])
                retorno = false; // Tem uma permissão marcada.
            else
                getPermissoesFromGridRecursivo(array[i].children);

        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarGruposFilhosByEscolasGrid() {
    try{
        var gridEscolas = dijit.byId('gridEscolas').store.objectStore.data;
        var listaSysGrupos = [];
        if (hasValue(gridEscolas) && gridEscolas.length > 0) {
            for (var i = 0; i < gridEscolas.length; i++) {
                listaSysGrupos.push({
                    cd_pessoa_empresa: gridEscolas[i].cd_pessoa,
                    no_grupo: dojo.byId("no_grupo").value,
                    cd_grupo_master: 0,
                    cd_grupo: gridEscolas[i].cd_grupo == null ? 0 : gridEscolas[i].cd_grupo
                });
            }
        }

        if (listaSysGrupos == null || listaSysGrupos.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgEscolaDeverSerInserida);
            apresentaMensagem('apresentadorMensagemGrupo', mensagensWeb);
            return false;
        }

        return listaSysGrupos;
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink) {
    try{
        alteraPermissoes = false;
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        getLimpar('#formGrupo');
        if (value != null) {
            dojo.byId("cd_grupo").value = value.cd_grupo;
            dojo.byId("no_grupo").value = value.no_grupo;

            dijit.byId('ckIdAtualizarGrupo')._onChangeActive = false;

            dijit.byId('ckIdAtualizarGrupo').set('value', value.id_atualizar_grupo);

            vizualizarCheckBoxAtualizarGrupo(value, TIPO);
            dijit.byId('ckIdAtualizarGrupo')._onChangeActive = true;

            if (TIPO == GRUPO) {
                dojo.byId('paiGridUsuarioGrupo').style.display = '';
                dojo.byId('paiGridEscolaMaster').style.display = 'none';
                dojo.ready(function () {
                    // Limpando dados da Grid
                    var gridUsuarioGrupo = dijit.byId("gridUsuarioGrupo");
                    if (hasValue(gridUsuarioGrupo)) {
                        var data = new Array();
                        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) });
                        gridUsuarioGrupo.setStore(dataStore);
                        gridUsuarioGrupo.update();
                    }

                    dojo.xhr.get({
                        url: Endereco() + "/api/permissao/getUsuarioByGrupo?cdGrupo=" + document.getElementById("cd_grupo").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            apresentaMensagem('apresentadorMensagemGrupo', data);
                            data = jQuery.parseJSON(data).retorno;
                            var data = montarUsuarioGrupoUsuarioSelecionada(value.Usuarios, data);
                            destroyCreateGridUsuarioGrupo(data, GRUPO);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }, function (error) {
                        apresentaMensagem('apresentadorMensagemGrupo', error);
                    });
                });
            }
            else {
                dojo.byId('paiGridUsuarioGrupo').style.display = 'none';
                dojo.byId('paiGridEscolaMaster').style.display = '';
                dojo.ready(function () {
                    // Limpando dados da Grid
                    dijit.byId("paiGridEscolaMaster").set("open", true);
                    var gridEscolas = dijit.byId("gridEscolas");
                    if (hasValue(gridEscolas)) {
                        var data = new Array();
                        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) });
                        gridEscolas.setStore(dataStore);
                        gridEscolas.update();
                    }

                    dojo.xhr.get({
                        url: Endereco() + "/api/empresa/getEmpresaHasGrupoMaster?cd_grupo_master=" + value.cd_grupo,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            apresentaMensagem('apresentadorMensagemGrupo', data);
                            var empresas = data.retorno;
                            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                            gridEscolas.update();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }, function (error) {
                        apresentaMensagem('apresentadorMensagemGrupo', error);
                    });
                });
            }
            destroyCreateGridPermissao();

            return false;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function vizualizarCheckBoxAtualizarGrupo(value, tipo) {
    try{
        //var cd_grupo_pai
        if (value.cd_grupo_master == null || value.cd_grupo_master == 0) {
            dijit.byId('ckIdAtualizarGrupo').set('disabled', true);
            dojo.byId('decAtualizar').style.display = 'none';
            dojo.byId('ckAtualizar').style.display = 'none';
        } else {
            dojo.byId('decAtualizar').style.display = '';
            dojo.byId('ckAtualizar').style.display = '';
            dijit.byId('ckIdAtualizarGrupo').set('disabled', false);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//#region Crud grupos usuarios
function IncluirGrupo(msg, type, form) {
    try{
        apresentaMensagem('apresentadorMensagem', type);
        apresentaMensagem(msg, type);

        if (!dijit.byId("cadGrupo").validate()) {
            dijit.byId("tabContainer").selectChild(dijit.byId("tabCadastro"));
            return false;
        }
        var existePermissao = false;
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            var permissoesWeb = null;
            if ((hasValue(dijit.byId("gridPermissoes"))) && (hasValue(dijit.byId("gridPermissoes").store))) {
                permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
            };
            if (hasValue(permissoesWeb))
                for (var i = 0; i < permissoesWeb.length; i++)
                    if (permissoesWeb[i].alterar == true || permissoesWeb[i].excluir == true || permissoesWeb[i].incluir == true || permissoesWeb[i].visualizar == true)
                        existePermissao = true;
            if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgLackPermissionUser);
                apresentaMensagem('apresentadorMensagemGrupo', mensagensWeb);
                return false;
            }

            var usuarios = montarUsuariosSelecionados();
            showCarregando();
            xhr.post(Endereco() + "/permissao/postgrupo", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    grupo: {
                        no_grupo: dom.byId("no_grupo").value,
                        id_atualizar_grupo: dijit.byId('ckIdAtualizarGrupo').checked,
                        alteraDireito: alteraPermissoes
                    },
                    permissoes: permissoesWeb,
                    usuariogrupo: usuarios
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridGrupo';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupo").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_grupo", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoGrupo', 'cd_grupo', 'selecionaTodos', ['pesquisarGrupo', 'relatorioGrupo'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_grupo");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemGrupo', data.erro);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupo', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function AlterarGrupo() {
    try{
        var gridName = 'gridGrupo';
        var existePermissao = false;
        var grid = dijit.byId(gridName);
        var existePer = 0;
        if (!dijit.byId("cadGrupo").validate()) {
            dijit.byId("tabContainer").selectChild(dijit.byId("tabCadastro"));
            return false;
        }
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            var permissoesWeb = null;
            if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
                permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
                for (var i = 0 ; i < permissoesWeb.length; i++) {
                    if (permissoesWeb[i].visualizar == true)
                        existePer = 1;
                }
            }
            else
                existePer = 1;

            if (existePer == 1) {
                var usuarios = montarUsuariosSelecionados();
                showCarregando();
                xhr.post(Endereco() + "/permissao/postalterargrupo", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    data: ref.toJson({
                        grupo: {
                            cd_grupo: parseInt(dom.byId("cd_grupo").value),
                            no_grupo: dom.byId("no_grupo").value,
                            id_atualizar_grupo: dijit.byId('ckIdAtualizarGrupo').checked,
                            alteraDireito: alteraPermissoes
                        },
                        permissoes: permissoesWeb,
                        usuariogrupo: (hasValue(usuarios) && usuarios.length > 0) ? usuarios : null

                    })
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var todos = dojo.byId("todosItens_label");

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];

                            apresentaMensagem('apresentadorMensagem', data);
                            dijit.byId("cadGrupo").hide();
                            removeObjSort(grid.itensSelecionados, "cd_grupo", dom.byId("cd_grupo").value);
                            insertObjSort(grid.itensSelecionados, "cd_grupo", itemAlterado);

                            buscarItensSelecionados(gridName, 'selecionadoGrupo', 'cd_grupo', 'selecionaTodos', ['pesquisarGrupo', 'relatorioGrupo'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_grupo");
                        }
                        else
                            apresentaMensagem('apresentadorMensagemGrupo', data);
                        showCarregando();
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemGrupo', error);
                });
            }
            else {

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgLackPermissionUser);
                apresentaMensagem('apresentadorMensagemGrupo', mensagensWeb);
            }
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarGrupo(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_grupo').value != 0)
                    itensSelecionados = [{
                        cd_grupo: dom.byId("cd_grupo").value,
                        no_grupo: dom.byId("no_grupo").value
                    }];
            xhr.post({
                url: Endereco() + "/api/permissao/postdeleteGrupo",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItens_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupo").hide();
                        dijit.byId("pesquisaGrupo").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridGrupo').itensSelecionados, "cd_grupo", itensSelecionados[r].cd_grupo);

                        PesquisarGrupo(true);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisaGrupo").set('disabled', false);
                        dijit.byId("relatorioGrupo").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        if (!hasValue(dojo.byId("cadGrupo").style.display))
                            apresentaMensagem('apresentadorMensagemGrupo', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadGrupo").style.display))
                    apresentaMensagem('apresentadorMensagemGrupo', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region crud grupo master

function getPermissoesForGruposEscola(isEdit) {
    try{
        var permissoesWeb = null;
        var existePermissao = false;

        if ((hasValue(dijit.byId("gridPermissoes"))) && (hasValue(dijit.byId("gridPermissoes").store))) {
            permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
        };

        if (hasValue(permissoesWeb)) {
            for (var i = 0; i < permissoesWeb.length; i++)
                if (!isEdit) {
                    if (permissoesWeb[i].alterar == true || permissoesWeb[i].excluir == true
                                                         || permissoesWeb[i].incluir == true
                                                         || permissoesWeb[i].visualizar == true)
                        existePermissao = true;
                }
                else {
                    if (permissoesWeb[i].visualizar == true)
                        existePermissao = true;
                }
        }

        if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0
                                     || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgLackPermissionUser);
            apresentaMensagem('apresentadorMensagemGrupo', mensagensWeb);
            return false;
        }

        return permissoesWeb;
    } catch (e) {
        postGerarLog(e);
    }
}

function incluirGrupoEscola(msg, type, form) {
    try{
        apresentaMensagem('apresentadorMensagem', type);
        apresentaMensagem(msg, type);


        if (!dijit.byId("cadGrupo").validate())
            return false;

        var listaSysGrupos = montarGruposFilhosByEscolasGrid();

        if (!listaSysGrupos)
            return false;

        var permissoesWeb = getPermissoesForGruposEscola(false);

        if (!permissoesWeb)
            return false;

        showCarregando();

        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {

            xhr.post(Endereco() + "/permissao/inserirGrupoEscola", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    grupo: {
                        no_grupo: dom.byId("no_grupo").value,
                        alteraDireito: alteraPermissoes,
                        SysGrupoFilho: listaSysGrupos
                    },
                    permissoes: permissoesWeb
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridGrupo';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupo").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_grupo", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoGrupo', 'cd_grupo', 'selecionaTodos', ['pesquisarGrupo', 'relatorioGrupo'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_grupo");

                        showCarregando();
                    }
                    else {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemGrupo', data.erro);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupo', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarGrupoEscola() {
    try{
        if (!dijit.byId("cadGrupo").validate())
            return false;

        var permissoesWeb = [];

        var listaSysGrupos = montarGruposFilhosByEscolasGrid();

        if (!listaSysGrupos)
            return false;

        if (alteraPermissoes)
            permissoesWeb = getPermissoesForGruposEscola(true);

        if (!permissoesWeb)
            return false;

        showCarregando();

        dojo.xhr.post({
            url: Endereco() + "/permissao/alterarGrupoEscola",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({
                grupo: {
                    cd_grupo: parseInt(dojo.byId("cd_grupo").value),
                    no_grupo: dojo.byId("no_grupo").value,
                    alteraDireito: alteraPermissoes,
                    SysGrupoFilho: listaSysGrupos
                },
                permissoes: permissoesWeb
            })
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    showCarregando();
                    var gridName = 'gridGrupo';
                    var grid = dijit.byId(gridName);
                    var todos = dojo.byId("todosItens_label");

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);

                    dijit.byId("cadGrupo").hide();
                    removeObjSort(grid.itensSelecionados, "cd_grupo", dojo.byId("cd_grupo").value);
                    insertObjSort(grid.itensSelecionados, "cd_grupo", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoGrupo', 'cd_grupo', 'selecionaTodos', ['pesquisarGrupo', 'relatorioGrupo'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_grupo");
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemGrupo', data);
                }
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemGrupo', error);
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarGrupoEscola(itensSelecionados) {
    try{
        showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_grupo').value != 0)
                    itensSelecionados = [{
                        cd_grupo: dom.byId("cd_grupo").value,
                        no_grupo: dom.byId("no_grupo").value
                    }];
            xhr.post({
                url: Endereco() + "/permissao/deletarGrupoEscola",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItens_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupo").hide();
                        dijit.byId("pesquisaGrupo").set("value", '');

                        PesquisarGrupo(true);

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridGrupo').itensSelecionados, "cd_grupo", itensSelecionados[r].cd_grupo);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisaGrupo").set('disabled', false);
                        dijit.byId("relatorioGrupo").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                        showCarregando();
                    }
                    else {
                        showCarregando();
                        if (!hasValue(dojo.byId("cadGrupo").style.display))
                            apresentaMensagem('apresentadorMensagemGrupo', data);
                        else
                            apresentaMensagem('apresentadorMensagem', data);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
             function (error) {
                 showCarregando();
                 if (!hasValue(dojo.byId("cadGrupo").style.display))
                     apresentaMensagem('apresentadorMensagemGrupo', error);
                 else
                     apresentaMensagem('apresentadorMensagem', error);
             });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#endregion  Inicio da Percistência do Grupo
