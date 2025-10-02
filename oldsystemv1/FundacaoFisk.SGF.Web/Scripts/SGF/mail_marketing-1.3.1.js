var TODOS = 0;
var  PROSPECT = 1, ALUNO = 2, CLIENTE = 3, PESSOARELACIONADA = 4;

function montarMailMarketing() {
    require([
    "dojo/_base/xhr",
    "dojo/dom",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/query",
    "dojo/dom-attr",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dojo/ready",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/MenuBar",
     "dijit/PopupMenuBarItem",
    "dijit/form/FilteringSelect",
    "dojo/_base/array",
    "dojox/json/ref"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button, ready, on, ready, DropDownButton, DropDownMenu, MenuItem, MenuBar, PopupMenuBarItem,
        FilteringSelect, array,ref) {
        ready(function () {

            var gridHistDocAnexo = new EnhancedGrid({
                store: dojo.data.ObjectStore({ objectStore: new Memory({ data: [] }) }),
                structure:
                [
                    { name: "Tipo Arquivo", field: "Extension", width: "15%" },
                    { name: "Nome", field: "Name", width: "45%", styles: "text-align: center;" },
                    { name: "Tamanho", field: "Length", width: "12%", styles: "text-align: center;" },
                    {
                        name: "Documento",
                        field: "_item",
                        width: '100px',
                        styles: "text-align: center;",
                        formatter: function (item) {
                            var btn = new dijit.form.Button({
                                label: "Baixar",
                                onClick: function () {
                                    try {
                                        var url = Endereco() + "/api/emailMarketing/DownloadArquivoAnexo?nome_arquivo=" + item.Name + "&cd_mala_direta=" + dijit.byId("gridHistorico").itensSelecionados[0].cd_mala_direta + "&cd_escola=" + item.cd_escola;
                                        window.location = url;
                                    } catch (e) {

                                    }
                                }
                            });
                            return btn;
                        }
                    }
                ]
            }, "gridHistDocAnexo");
            gridHistDocAnexo.startup();

            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                    dijit.byId("dlgHistDocAnexo").hide();
                }
            }, "fecharHistDocAnexo");

            if (!hasValue(dijit.byId("mensagemComporMsg"))) {
                montarCadastroMailMarketing(function () { montarNovaMensagem(); });
                montarCadastroMaladireta(function () { montarNovaViewMalaDireta(); });
            }
            //Desenho dos menus:

            //MENU MENSAGEM
            var pMenuBar = new MenuBar({
                id: "menusMensagemBar"
            });
            var pSubMenu = new DropDownMenu({});
            pSubMenu.addChild(new MenuItem({
                label: ""
            }));
            pMenuBar.addChild(new PopupMenuBarItem({
                onClick: function (obj) { },
                id: "menuComporMensagem",
                onClick: function (obj) { trocaVisao('compor_mensagem'); },
                label: "Compor Mensagem"
            }));
            pMenuBar.placeAt("menusMensagem");

            //MENU CONTROLE
            pMenuBar = new MenuBar({
                id: "menusControleBar"
            });
            pSubMenu = new DropDownMenu({});
            pSubMenu.addChild(new MenuItem({
                label: ""
            }));

            pMenuBar.addChild(new PopupMenuBarItem({
                onClick: function (obj) { trocaVisao('historico'); criarOpcaoHistorico(); },
                id: "menuHistorico",
                label: "Histórico"
            }));
            pMenuBar.addChild(new PopupMenuBarItem({
                onClick: function (obj) {
                    trocaVisao('listagem_enderecos');
                    criarOpcaoListagemEndereco(Button);
                },
                id: "menuListaEnderecos",
                label: "Lista de Endereços"
            }));
            pMenuBar.placeAt("menusControle");

            //MENU MALA DIRETA
            pMenuBar = new MenuBar({
                id: "menusMalaDiretaBar"
            });
            pSubMenu = new DropDownMenu({});
            pSubMenu.addChild(new MenuItem({
                label: ""
            }));

            pMenuBar.addChild(new PopupMenuBarItem({
                onClick: function (obj) {
                    trocaVisao('gerar_mala_direta');
                },
                id: "menuGerarMalaDireta",
                label: "Gerar Mala Direta"
            }));
            pMenuBar.addChild(new PopupMenuBarItem({
                onClick: function (obj) {
                    trocaVisao('historicoEtiqueta');
                    criarOpcaoHistoricoEtiqueta();
                },
                id: "menuHistoricoMalaDireta",
                label: "Histórico"
            }));
            pMenuBar.placeAt("menusMalaDireta");

        });
    });
}

function trocaVisao(nome_visao) {
    showP('compor_mensagem', false);
    showP('historico', false);
    showP('listagem_enderecos', false);
    showP('gerar_mala_direta', false);
    showP('historicoEtiqueta', false);

    showP(nome_visao, true);
}

//Métodos tag histórico / listagem endereços / cartão postal.
function criarOpcaoHistorico() {
    if (!hasValue(dijit.byId("gridHistorico"))) {
        try {
            decreaseBtn(document.getElementById("pesquisa_historico"), '32px');
            var myStore = dojo.store.Cache(dojo.store.JsonRest({
                target: Endereco() + "/api/emailMarketing/searchHistoricoMalaDireta?dc_assunto=&dta_historico=&id_tipo_mala=1",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }
                                ), dojo.store.Memory({}));
            var gridHistorico = new dojox.grid.EnhancedGrid({
                store: dojo.data.ObjectStore({ objectStore: myStore }),
                structure:
                [
                    { name: "<input id='selecionaTodosMalaDireta' style='display:none'/>", field: "malaDiretaSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMalaDireta },
                    { name: "Data", field: "dta_mala_direta", width: "10%", styles: "text-align: center;min-width:80px;" },
                    { name: "Assunto", field: "dc_assunto", width: "50%", styles: "min-width:80px;" },
                    { name: "Login do Usuário", field: "no_login", width: "25%", styles: "min-width:80px;" },
                    { name: "Enviados", field: "qtd_enderecos_enviados", width: "10%", styles: "min-width:70px;" }
                ],
                canSort: true,
                selectionMode: "single",
                noDataMessage: "Nenhum registro encontrado.",
                plugins: {
                    pagination: {
                        pageSizes: ["12", "24", "36", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "12",
                        gotoButton: true,
                        maxPageStep: 5,
                        position: "button"
                    }
                }
            }, "gridHistorico");
            gridHistorico.startup();
            gridHistorico.pagination.plugin._paginator.plugin.connect(gridHistorico.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridHistorico, 'cd_mala_direta', 'selecionaTodosMalaDireta');
            });

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridHistorico, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMalaDireta').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'malaDiretaSelecionado', -1, 'selecionaTodosMalaDireta', 'selecionaTodosMalaDireta', 'gridHistorico')", gridHistorico.rowsPerPage * 3);
                });
            });
            gridHistorico.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 4};
            gridHistorico.itensSelecionados = [];
            gridHistorico.on("RowDblClick", function (evt) {
                try {
                    var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                    //keepValuesCnab(item, gridHistorico, false);
                    //IncluirAlterar(0, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
                    //dijit.byId("cadCnab").show();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);
            var menu = new dijit.DropDownMenu({ id: "menuAcoesRelacionadas", style: "height: 25px" });

            var acaoEditar = new dijit.MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarHistoricoMalaDireta(gridHistorico.itensSelecionados) }
            });
            menu.addChild(acaoEditar);
            var acaoLogsHistorico = new dijit.MenuItem({
                label: "Logs",
                onClick: function () { eventoVisualizarLogs(gridHistorico.itensSelecionados) }
            });
            menu.addChild(acaoLogsHistorico);
            //Deivid
            var acaoEditar = new dijit.MenuItem({
                label: "Visualizar",
                onClick: function () { visualizarCartaoPostal(gridHistorico.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoVisualizarPdf = new dijit.MenuItem({
                label: "Visualizar Anexo",
                onClick: function () { visualizarDocumentoAnexo(gridHistorico.itensSelecionados); }
            });
            menu.addChild(acaoVisualizarPdf);

            var acaoGerarPDF = new dijit.MenuItem({
                label: "Gerar PDF",
                onClick: function () { baixarCartaoPostal(gridHistorico.itensSelecionados); }
            });
            menu.addChild(acaoGerarPDF);

            var acaoRptListagem = new dijit.MenuItem({
                label: "Relatório de Listagem de Endereços",
                onClick: function () { eventoVisualizarListagemEnderecos(gridHistorico.itensSelecionados) }
            });
            menu.addChild(acaoRptListagem);
            var button = new dijit.form.DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadas"
            });
            dojo.byId("linkAcoesHitorico").appendChild(button.domNode);


            // Adiciona link de selecionados:
            menu = new dijit.DropDownMenu({ style: "height: 25px" });

            var menuTodosItens = new dijit.MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridHistorico, 'todosItens', ['pesquisa_historico']); pesquisaHistoricoMalaDireto(); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new dijit.MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridHistorico', 'selecionadoMovto', 'cd_mala_direta', 'selecionaTodosMalaDireta', ['pesquisa_historico'], 'todosItens'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new dijit.form.DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItens"
            });
            dojo.byId("linkSelecionadosHistoricoMalaDireta").appendChild(button.domNode);

            dijit.byId('pesquisa_historico').on("click", function (evt) {
                apresentaMensagem('apresentadorMensagemHistoricoMalaDireta', null);
                pesquisaHistoricoMalaDireto();
            });
            adicionarAtalhoPesquisa(['dc_assunto_historico', 'dta_historico'], 'pesquisa_historico', dojo.ready);
            loadPesqSexo(dojo.store.Memory, dijit.byId("nm_sexo_historico"));
            loadMesNascimento(dojo.store.Memory, dijit.byId('nm_mes_nascimento_historico'));
            loadPeriodo(dojo.store.Memory, dojo.data.ItemFileReadStore, dijit.byId("cbPeriodoHistorico"));
            //Montagem do editor de html:
            var editor = new dijit.Editor({
                height: '315px',
                plugins: ['collapsibletoolbar', 'undo', 'redo', '|', 'cut', 'copy', 'paste', '|', 'bold', 'italic', 'underline', 'strikethrough', '|', 'insertOrderedList', 'insertUnorderedList', 'indent', 'outdent',
                                          '|', 'justifyLeft', 'justifyRight', 'justifyCenter', 'justifyFull', '|', 'foreColor', 'hiliteColor', '|', 'createLink', 'insertImage', '|',
                                          'newpage', /*'save',*/'showBlockNodes', 'preview', 'findreplace',
                                          { name: 'viewSource', stripScripts: true, stripComments: true }, 'selectAll', '|', 'fontName', 'fontSize', { name: 'formatBlock', plainText: true }]
            }, "mensagemComporMsgHistorico");
            editor.startup();
            dijit.byId("mensagemComporMsgHistorico").set("required", true);

            //Redimensiona os botões do editor de texto:
            var botoes_editor = dojo.byId('mensagemComporMsgHistorico').children[0].children[0];
            for (var i = 0; i < botoes_editor.children[0].children[0].children[2].children[0].children[0].children.length; i++)
                if (hasValue(botoes_editor.children[0].children) && hasValue(botoes_editor.children[0].children[0])
                        && hasValue(botoes_editor.children[0].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2])
                        && hasValue(botoes_editor.children[0].children[0].children[2].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0])
                        && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children) && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0])
                        && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children)
                        && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i])
                        && botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].localName != "div") {
                    botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].style.minWidth = '32px';
                    if (hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children)
                            && hasValue(botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0]))
                        botoes_editor.children[0].children[0].children[2].children[0].children[0].children[i].children[0].style.minWidth = '32px';
                }


            // Declare my own so I can pass in my plugins list without resoring to globals.
            dojo.declare("custom.Editor", [dijit.Editor], {
                constructor: function () {
                    this.plugins = plugins;
                    this.height = "100%";
                    this.inherited(arguments);
                }
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    }
}

function criarOpcaoHistoricoEtiqueta() {
    if (!hasValue(dijit.byId("gridHistoricoEtiqueta"))) {
        try {
            decreaseBtn(document.getElementById("pesquisa_historico_etiqueta"), '32px');
            var myStore = dojo.store.Cache(dojo.store.JsonRest({
                target: Endereco() + "/api/emailMarketing/searchHistoricoMalaDireta?dc_assunto=&dta_historico=&id_tipo_mala=2",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }
                                ), dojo.store.Memory({}));
            var gridHistoricoEtiqueta = new dojox.grid.EnhancedGrid({
                store: dojo.data.ObjectStore({ objectStore: myStore }),
                structure:
                [
                    { name: "<input id='selecionaTodosMalaDiretaEtiqueta' style='display:none'/>", field: "malaDiretaEtiquetaSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMalaDiretaEtiqueta },
                    { name: "Data", field: "dta_mala_direta", width: "10%", styles: "text-align: center;min-width:80px;" },
                    { name: "Descrição", field: "dc_assunto", width: "50%", styles: "min-width:80px;" },
                    { name: "Login do Usuário", field: "no_login", width: "25%", styles: "min-width:80px;" },
                    { name: "Enviados", field: "qtd_enderecos_enviados", width: "10%", styles: "min-width:70px;" }
                ],
                canSort: true,
                selectionMode: "single",
                noDataMessage: "Nenhum registro encontrado.",
                plugins: {
                    pagination: {
                        pageSizes: ["12", "24", "36", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "12",
                        gotoButton: true,
                        maxPageStep: 5,
                        position: "button"
                    }
                }
            }, "gridHistoricoEtiqueta");
            gridHistoricoEtiqueta.startup();
            gridHistoricoEtiqueta.pagination.plugin._paginator.plugin.connect(gridHistoricoEtiqueta.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridHistoricoEtiqueta, 'cd_mala_direta', 'selecionaTodosMalaDiretaEtiqueta');
            });

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridHistoricoEtiqueta, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosMalaDiretaEtiqueta').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'malaDiretaEtiquetaSelecionado', -1, 'selecionaTodosMalaDiretaEtiqueta', 'selecionaTodosMalaDiretaEtiqueta', 'gridHistoricoEtiqueta')", gridHistoricoEtiqueta.rowsPerPage * 3);
                });
            });
            gridHistoricoEtiqueta.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 4 };
            gridHistoricoEtiqueta.itensSelecionados = [];

            var menu = new dijit.DropDownMenu({ id: "menuAcoesRelacionadasEtiqueta", style: "height: 25px" });

            var acaoGerarPDF = new dijit.MenuItem({
                label: "Gerar PDF",
                onClick: function () {
                    //baixarCartaoPostal(gridHistoricoEtiqueta.itensSelecionados);
                    var cd_mala_direta = gridHistoricoEtiqueta.itensSelecionados[0].cd_mala_direta;
                    if (hasValue(cd_mala_direta))
                        gerarEtiqueta(cd_mala_direta)
                }
            });
            menu.addChild(acaoGerarPDF);
       
            var button = new dijit.form.DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadas",
                dropDown: menu,
                id: "acoesRelacionadasEtiqueta"
            });
            dojo.byId("linkAcoesHitoricoEtiqueta").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new dijit.DropDownMenu({ style: "height: 25px" });

            var menuTodosItens = new dijit.MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridHistoricoEtiqueta, 'todosItensEtiqueta', ['pesquisa_historico_etiqueta']);
                    pesquisaHistoricoEtiqueta();
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new dijit.MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridHistoricoEtiqueta', 'malaDiretaEtiquetaSelecionado', 'cd_mala_direta', 'selecionaTodosMalaDireta', ['pesquisa_historico_etiqueta'], 'todosItens'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new dijit.form.DropDownButton({
                label: "Todos Itens",
                name: "todosItens",
                dropDown: menu,
                id: "todosItensEtiqueta"
            });
            dojo.byId("linkSelecionadosHistoricoEtiqueta").appendChild(button.domNode);

            dijit.byId('pesquisa_historico_etiqueta').on("click", function (evt) {
                apresentaMensagem('apresentadorMensagemHistoricoMalaDireta', null);
                pesquisaHistoricoEtiqueta();
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    }
}

function eventoEditarHistoricoMalaDireta(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            //dojo.byId("paiMensagemEditView").innerHTML = dojo.byId("scrollComporMensagem").innerHTML;
            //dojo.byId("scrollComporMensagem").innerHTML = "";
            limparComporMensagemHistorico();
            dijit.byId("dialogMensagem").show();
            showEditMalaDireta(itensSelecionados[0].cd_mala_direta);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparComporMensagemHistorico() {
    clearForm("formComporMensagemHistorico");
    dijit.byId('mensagemComporMsgHistorico').set('value', "");
    if (hasValue(dijit.byId('cbPeriodoHistorico')) && hasValue(dijit.byId('cbPeriodoHistorico').store))
        dijit.byId('cbPeriodoHistorico').setStore(dijit.byId('cbPeriodoHistorico').store, [0]);
    if (hasValue(dijit.byId('cbProdutoAtualHistorico')) && hasValue(dijit.byId('cbProdutoAtualHistorico').store))
        dijit.byId('cbProdutoAtualHistorico').setStore(dijit.byId('cbProdutoAtualHistorico').store, [0]);
    if (hasValue(dijit.byId('cbCursoAtualHistorico')) && hasValue(dijit.byId('cbCursoAtualHistorico').store))
        dijit.byId('cbCursoAtualHistorico').setStore(dijit.byId('cbCursoAtualHistorico').store, [0]);
}

function eventoListarEnderecos(malaDireta) {
    try {
        var listaEnderecos = cloneArray(malaDireta.ListasEnderecoMala);

        if (!hasValue(listaEnderecos) || listaEnderecos.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotListaEnderecos, null);
            showCarregando();
        }
        else {
            showP('acoesListagemEnderecos', true);
            apresentaMensagem('apresentadorMensagemListagemEnderecos', "");
            require(["dojo/dom-style"], function (domStyle) {
                domStyle.set("dialogListagemEnderecos", "height", "640px");
                domStyle.set("panelListagemEnderecoFK", "height", "608px");

                if (!hasValue(dijit.byId("gridListagemEnderecoFK")))
                    criarEAtualizarFKListagemEnderecos(function () {
                        dijit.byId("dialogListagemEnderecos").show();
                        populaEnderecos(listaEnderecos);
                        dijit.byId('gridListagemEnderecoFK').malaDireta = clone(malaDireta);
                    }, null);
                else {
                    dijit.byId("dialogListagemEnderecos").show();
                    populaEnderecos(listaEnderecos);
                    dijit.byId('gridListagemEnderecoFK').malaDireta = clone(malaDireta);
                }
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoVisualizarLogs(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            showP('acoesListagemEnderecos', false);
            require(["dojo/dom-style"], function (domStyle) {
                domStyle.set("dialogListagemEnderecos", "height", "610px");
                domStyle.set("panelListagemEnderecoFK", "height", "603px");
                
                if (!hasValue(dijit.byId("gridListagemEnderecoFK")))
                    criarEAtualizarFKListagemEnderecos(function () {
                        dijit.byId("dialogListagemEnderecos").show();
                        new function () { pesquisarListaEnderecosMalaDireta(itensSelecionados[0].cd_mala_direta); }
                        dijit.byId('gridListagemEnderecoFK').layout.setColumnVisibility(0, false);
                    }, null);
                else {
                    dijit.byId("dialogListagemEnderecos").show();
                    pesquisarListaEnderecosMalaDireta(itensSelecionados[0].cd_mala_direta);
                    dijit.byId('gridListagemEnderecoFK').layout.setColumnVisibility(0, false);
                }
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoVisualizarListagemEnderecos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            try {
                dojo.xhr.get({
                    url: Endereco() + "/api/emailMarketing/getUrlRelatorioListagemEnderecosMMK?cd_mala_direta=" + itensSelecionados[0].cd_mala_direta + "&no_pessoa=&status=0&email=&id_tipo_cadastro=0",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    abrePopUp(Endereco() + '/Relatorio/RelatorioListagemEnderecosMMK?' + data, '1000px', '750px', 'popRelatorio');
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemListagemEndereco', error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarOpcaoListagemEndereco(Button) {
    showCarregando();
    if (!hasValue(dijit.byId("gridListagemEndereco"))) {
        decreaseBtn(document.getElementById("pesquisa_listagem_enderecos"), '32px');
        decreaseBtn(document.getElementById("pesquisa_historico"), '32px');
        criarEventosFiltroListagemEndereco();
        new Button({
            label: getNomeLabelRelatorio(),
            iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
            onClick: function () {
                var tipo_cadastro_listagem = hasValue(dijit.byId("tipo_cadastro_listagem").value) ? dijit.byId("tipo_cadastro_listagem").value : 0;
                var status_listagem = hasValue(dijit.byId("status_listagem").value) ? dijit.byId("status_listagem").value : 0;
                try {
                    dojo.xhr.get({
                        url: Endereco() + "/api/emailMarketing/getUrlRelatorioListagemEnderecosMMK?cd_mala_direta=0&no_pessoa=" + dojo.byId("no_pessoa_lista").value + "&status=" +
                            status_listagem + "&email=" + dojo.byId("email_listagem").value + "&id_tipo_cadastro=" + tipo_cadastro_listagem,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        abrePopUp(Endereco() + '/Relatorio/RelatorioListagemEnderecosMMK?' + data, '1000px', '750px', 'popRelatorio');
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagemListagemEndereco', error);
                    });
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "relatorioListagemEnderecos");

        menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new dijit.MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(dijit.byId("gridListagemEndereco"), 'todosItensEnderecos', ['pesquisa_listagem_enderecos']); pesquisaListagemEndereco(); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new dijit.MenuItem({
            label: "Lista Não Inscritos",
            onClick: function () {
                buscarItensSelecionados('gridListagemEndereco', 'enderecoSelecionado', 'cd_cadastro', 'selecionaTodosEnderecos', ['pesquisa_listagem_enderecos'], 'todosItensEnderecos');
                var todos = dojo.byId("todosItensEnderecos" + "_label");
                if (hasValue(todos))
                    todos.innerHTML = "Lista Não Inscritos";
            }
        });
        menu.addChild(menuItensSelecionados);

        button = new dijit.form.DropDownButton({
            label: "Todos Itens",
            name: "todosItensCnab",
            dropDown: menu,
            id: "todosItensEnderecos"
        });
        dojo.byId("linkSelecionadosEnderecos").appendChild(button.domNode);
        dijit.byId("cancelarListagemEnd").on("Click", function (e) {
            limparFiltrosListagemEndereco();
            apresentaMensagem('apresentadorMensagemListagemEndereco', null);
            pesquisaListagemEndereco();
        });
        dijit.byId("salvarListagemEnd").on("Click", function (e) {
            apresentaMensagem('apresentadorMensagemListagemEndereco', null);
            salvarListagemEndereco();
        });
        dijit.byId('pesquisa_listagem_enderecos').on("click", function (evt) {
            apresentaMensagem('apresentadorMensagemListagemEndereco', null);
            pesquisaListagemEndereco();
        });
        montarLegendaListagemEnderecos("chartListagemEnd", "legendListagemEnd");
        adicionarAtalhoPesquisa(['no_pessoa_lista', 'status_listagem', 'email_listagem', 'tipo_cadastro_listagem'], 'pesquisa_listagem_enderecos', dojo.ready);
        var gridListagemEndereco = new dojox.grid.EnhancedGrid({
            store: dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
            structure:
            [
                { name: "<input id='selecionaTodosEnderecos' style='display:none'/>", field: "enderecoSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxInscrito },
                //{ name: "Não Inscrito", field: "id_inscrito", width: "8%", styles: "text-align: center;min-width:80px;", formatter: formatCheckInscrito },
                { name: "Nome", field: "no_pessoa", width: "38%", styles: "min-width:70px;" },
                { name: "E-Mail", field: "dc_email_cadastro", width: "38%", styles: "min-width:70px;" },
                { name: "Cadastro", field: "tipo_cadastro", width: "16%", styles: "min-width:70px;" },
                { name: "cd_cadastro", field: "cd_cadastro", width: "16%", styles: "min-width:70px;display:none;" }
            ],
            canSort: true,
            selectionMode: "single",
            noDataMessage: "Nenhum registro encontrado.",
            plugins: {
                pagination: {
                    pageSizes: ["11", "22", "33", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "11",
                    gotoButton: true,
                    maxPageStep: 5,
                    position: "button"
                }
            }
        }, "gridListagemEndereco");
        gridListagemEndereco.on("StyleRow", function (row) {
            try {
                var inscrito = false;
                var itens = cloneArray(gridListagemEndereco.itensSelecionados);
                quickSortObj(itens, 'cd_cadastro');
                if (hasValue(itens && itens.length > 0) &&
                    row.node.children[0].children[0].children[0].children[4].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[4].innerHTML)) {
                    var posicao = binaryObjSearch(itens, 'cd_cadastro', parseInt(row.node.children[0].children[0].children[0].children[4].innerHTML));
                    if (hasValue(posicao, true))
                        inscrito = true;
                }
                if (inscrito)
                    row.customClasses += " RedRow"
                else
                    row.customClasses += " GreenRow"
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        gridListagemEndereco.startup();
        gridListagemEndereco.canSort = function (col) { return Math.abs(col) != 1 };
        gridListagemEndereco.itensSelecionados = [];
        gridListagemEndereco.on("RowDblClick", function (evt) {
            try {
                apresentaMensagem('apresentadorMensagemListagemEndereco', null);
                var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                //keepValuesCnab(item, gridListagemEndereco, false);
                //IncluirAlterar(0, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
                //dijit.byId("cadCnab").show();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, true);
        dojo.xhr.get({
            url: Endereco() + "/api/emailMarketing/getListagemEnderecosEscola?no_pessoa=&status=0&email=&id_tipo_cadastro=0",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (hasValue(data) && hasValue(data.ListaNaoInscritos))
                    gridListagemEndereco.itensSelecionados = data.ListaNaoInscritos;
                gridListagemEndereco.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.Enderecos }) }));
                showCarregando();
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
    } else
        showCarregando();
}

function formatCheckBoxInscrito(value, rowIndex, obj) {
    try {
        var gridName = 'gridListagemEndereco'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEnderecos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_cadastro", grid._by_idx[rowIndex].item.cd_cadastro);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_cadastro', 'enderecoSelecionado', -1, 'selecionaTodosEnderecos', 'selecionaTodosEnderecos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_cadastro', 'enderecoSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosEnderecos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMalaDireta(value, rowIndex, obj) {
    try {
        var gridName = 'gridHistorico'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMalaDireta');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_mala_direta", grid._by_idx[rowIndex].item.cd_mala_direta);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'malaDiretaSelecionado', -1, 'selecionaTodosMalaDireta', 'selecionaTodosMalaDireta', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_mala_direta', 'malaDiretaSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosMalaDireta', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMalaDiretaEtiqueta(value, rowIndex, obj) {
    try {
        var gridName = 'gridHistoricoEtiqueta'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMalaDiretaEtiqueta');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_mala_direta", grid._by_idx[rowIndex].item.cd_mala_direta);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'malaDiretaEtiquetaSelecionado', -1, 'selecionaTodosMalaDiretaEtiqueta', 'selecionaTodosMalaDiretaEtiqueta', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_mala_direta', 'malaDiretaEtiquetaSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosMalaDiretaEtiqueta', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaListagemEndereco() {
    var tipo_cadastro_listagem = hasValue(dijit.byId("tipo_cadastro_listagem").value) ? dijit.byId("tipo_cadastro_listagem").value : 0;
    var status_listagem = hasValue(dijit.byId("status_listagem").value) ? dijit.byId("status_listagem").value : 0;
    dojo.xhr.get({
        url: Endereco() + "/api/emailMarketing/getListagemEnderecosEscola?no_pessoa=" + dojo.byId("no_pessoa_lista").value + "&status=" + status_listagem + "&email=" + dojo.byId("email_listagem").value +
            "&id_tipo_cadastro=" + tipo_cadastro_listagem,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (hasValue(data) && hasValue(data.ListaNaoInscritos))
                dijit.byId("gridListagemEndereco").itensSelecionados = data.ListaNaoInscritos;
            dijit.byId("gridListagemEndereco").setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.Enderecos }) }));
            dijit.byId("gridListagemEndereco").update();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function pesquisaHistoricoMalaDireto() {
    try {
        var myStore = dojo.store.Cache(dojo.store.JsonRest({
            target: Endereco() + "/api/emailMarketing/searchHistoricoMalaDireta?dc_assunto=" + dojo.byId("dc_assunto_historico").value + "&dta_historico=" + dojo.byId("dta_historico").value + "&id_tipo_mala=1",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }
                    ), dojo.store.Memory({}));
        dijit.byId("gridHistorico").setStore(dojo.data.ObjectStore({ objectStore: myStore }));
        dijit.byId("gridHistorico").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaHistoricoEtiqueta() {
    try {
        var myStore = dojo.store.Cache(dojo.store.JsonRest({
            target: Endereco() + "/api/emailMarketing/searchHistoricoMalaDireta?dc_assunto=" + dojo.byId("dc_assunto_historico_etiqueta").value + "&dta_historico=" + dojo.byId("dta_historico_etiqueta").value + "&id_tipo_mala=2",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }
                    ), dojo.store.Memory({}));
        dijit.byId("gridHistoricoEtiqueta").setStore(dojo.data.ObjectStore({ objectStore: myStore }));
        dijit.byId("gridHistoricoEtiqueta").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarEventosFiltroListagemEndereco() {
    var dados = [
        { name: "Inscrito", id: "1" },
        { name: "Não Inscrito", id: "2" }
    ];
    criarOuCarregarCompFiltering("status_listagem", dados, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name', MASCULINO);

    dados = [
        { name: "Prospect", id: "1" },
        { name: "Aluno", id: "2" },
        { name: "Cliente", id: "3" },
        { name: "Pessoa Relacionada", id: "4" },
        { name: "Funcionário/Professor", id: "5" }
    ];
    criarOuCarregarCompFiltering("tipo_cadastro_listagem", dados, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name', MASCULINO);
}

function limparFiltrosListagemEndereco() {
    dijit.byId("no_pessoa_lista").reset();
    dijit.byId("status_listagem").set("value", TODOS);
    dijit.byId("email_listagem").reset();
    dijit.byId("tipo_cadastro_listagem").set("value", TODOS);
    dijit.byId("gridListagemEndereco").itensSelecionados = [];
}

function salvarListagemEndereco() {
    try {
        var itensSelecionados = dijit.byId("gridListagemEndereco").itensSelecionados;
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/emailMarketing/postAtualizarListaNaoInscrito",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(itensSelecionados)
        }).then(function (data) {
            if (!hasValue(data.erro)) {
                apresentaMensagem('apresentadorMensagemListagemEndereco', data);
                limparFiltrosListagemEndereco();
                pesquisaListagemEndereco();
                dijit.byId("gridListagemEndereco").currentPage(1);
                showCarregando();
            } else {
                apresentaMensagem('apresentadorMensagemListagemEndereco', data);
                showCarregando();
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemListagemEndereco', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaEnderecos(listaEnderecos) {
    try {
        dijit.byId('gridListagemEnderecoFK').layout.setColumnVisibility(0, true);

        //Ordena para colocar como itens selecionados:
        var listaEnderecosOrdenado = cloneArray(listaEnderecos)

        quickSortObj(listaEnderecosOrdenado, 'cd_cadastro');
        dijit.byId('gridListagemEnderecoFK').itensSelecionados = listaEnderecosOrdenado;
        dijit.byId("gridListagemEnderecoFK").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: listaEnderecos }) }));
        dijit.byId('gridListagemEnderecoFK').update();

        showCarregando();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarListaEnderecosMalaDireta(cd_mala_direta) {
    showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/emailMarketing/getListagemEnderecoMalaDireta?cd_mala_direta=" + cd_mala_direta,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            dijit.byId("gridListagemEnderecoFK").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
            showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function showEditMalaDireta(cd_mala_direta) {
    dojo.xhr.get({
        url: Endereco() + "/api/emailMarketing/getMalaDiretaEComponentesEdit?cd_mala_direta=" + cd_mala_direta,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            dijit.byId("tagDestinatarioHistorico").set("open", true);
            dijit.byId("tagFiltrosHistorico").set("open", true);
            dijit.byId("tagEscreverMensagemHistorico").set("open", true);
            if (hasValue(data.retorno))
                data = data.retorno;
            if (hasValue(data.tx_msg_footer))
                dojo.byId('rodape_fixo_msg_historico').innerHTML = data.tx_msg_footer;
            if (hasValue(data.id_sexo))
                dijit.byId("nm_sexo_historico").set("value", data.id_sexo);
            if (hasValue(data.dc_assunto))
                dijit.byId('dc_assunto_historico_mala').set('value', data.dc_assunto);
            if (hasValue(data.tx_msg_body))
                dijit.byId('mensagemComporMsgHistorico').set('value', data.tx_msg_body);
            if (hasValue(data.nm_mes_nascimento))
                dijit.byId('nm_mes_nascimento').set('value', data.nm_mes_nascimento);
            if (hasValue(data.hh_inicial))
                dijit.byId('timeIniHistorico').set('value', data.hh_inicial);
            if (hasValue(data.hh_final))
                dijit.byId('timeFim').set('value', data.hh_final);
            if (hasValue(data.MalasDiretaCadastro) && data.MalasDiretaCadastro.length > 0)
                $.each(data.MalasDiretaCadastro, function (idx, val) {
                    switch (val.id_cadastro) {
                        case PROSPECT:
                            dijit.byId("ckProspectHistorico").set("checked", true);
                            dijit.byId("ckProspectHistorico").onClick();
                            break;
                        case CLIENTE:
                            dijit.byId("ckClienteHistorico").set("checked", true);
                            dijit.byId("ckClienteHistorico").onClick();
                            break;
                        case ALUNO:
                            dijit.byId("ckAlunoHistorico").set("checked", true);
                            dijit.byId("ckAlunoHistorico").onClick();
                            break;
                        case PESSOARELACIONADA:
                            dijit.byId("ckPessoaRelacionadaHistorico").set("checked", true);
                            dijit.byId("ckPessoaRelacionadaHistorico").onClick();
                            break;
                    }
                });
            if (hasValue(data.MalasDiretaProduto) && data.MalasDiretaProduto.length > 0) {
                var w = dijit.byId("cbProdutoAtualHistorico");
                var produtoCb = [];
                var setCbProdutos = new Array();
                $.each(data.MalasDiretaProduto, function (index, val) {
                    produtoCb.push({
                        "cd_produto": val.cd_produto + "",
                        "no_produto": val.no_produto
                    });
                    setCbProdutos[index] = val.cd_produto;
                });
                var store = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "cd_produto",
                        label: "no_produto",
                        items: produtoCb
                    }
                });
                w.setStore(store, setCbProdutos);
            }
            if (hasValue(data.MalasDiretaCurso) && data.MalasDiretaCurso.length > 0) {
                var w = dijit.byId("cbCursoAtualHistorico");
                var cursosCb = [];
                var setCbCuros = new Array();
                $.each(data.MalasDiretaCurso, function (index, val) {
                    cursosCb.push({
                        "cd_curso": val.cd_curso + "",
                        "no_curso": val.no_curso
                    });
                    setCbCuros[index] = val.cd_curso;
                });
                var store = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: cursosCb
                    }
                });
                w.setStore(store, setCbCuros);

            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemInserirImagem", error);
        //showCarregando();
    });
}

function mostraPeriodoAlunoHistorico(obj) {
    if (dijit.byId('ckProspectHistorico').checked || dijit.byId('ckAlunoHistorico').checked)
        showP('trProdutoAtualHistorico', true);
    else
        showP('trProdutoAtualHistorico', false);
    show('tdCursoAtual1Historico');
    show('tdCursoAtual2Historico');
    show('tdPeriodoAluno1Historico');
    show('tdPeriodoAluno2Historico');
}

function mostraPeriodoProspectHistorico(obj) {
    if (dijit.byId('ckProspectHistorico').checked || dijit.byId('ckAlunoHistorico').checked)
        showP('trProdutoAtualHistorico', true);
    else
        showP('trProdutoAtualHistorico', false);
    show('tdPeriodoProspect1Historico');
    show('tdPeriodoProspect2Historico');
}

function visualizarCartaoPostal(itensSelecionados) {

    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
    else if (itensSelecionados.length > 1)
        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
    else 
        window.open(Endereco() + "/mailmarketing/VisualizadorCartaoPostal?cd_mala_direta=" + itensSelecionados[0].cd_mala_direta + "&enderecoWeb=" + EnderecoAbsoluto(), '_blank');
}

function visualizarDocumentoAnexo(itensSelecionados) {

    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
    else if (itensSelecionados.length > 1)
        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
    else {
        loadHistDocumentos(itensSelecionados[0].cd_mala_direta);
        dijit.byId("dlgHistDocAnexo").show();
    }
}

function loadHistDocumentos(cd_mala_direta) {
    require([
     "dojo/store/Memory",
     "dojo/data/ObjectStore",
      "dojo/ready",
    ], function (Memory, ObjectStore, ready) {
        ready(function () {
            if (hasValue(cd_mala_direta)) {
                $.ajax({
                    type: "GET",
                    url: Endereco() + "/api/emailMarketing/getHistDocumentos?cd_mala_direta=" + cd_mala_direta,
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    success: function (results) {                        

                        for (var i = 0; i < results.length; i++) {
                            results[i].Length = (Math.round(results[i].Length * 0.001) + " Kb");
                            results[i].Extension = "application/" + results[i].Extension;
                        }

                        var gridHistDocAnexo = dijit.byId("gridHistDocAnexo");

                        var dataStore = new ObjectStore({ objectStore: new Memory({ data: results }) });
                        gridHistDocAnexo.setStore(dataStore);
                        gridHistDocAnexo.update();
                    },
                    error: function (error) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Erro ao buscar registros.");
                        apresentaMensagem("apresentadorMensagemHistDocAnexo", mensagensWeb);
                    }
                });
            }
        });
    });
}

function baixarCartaoPostal(itensSelecionados) {
    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
    else if (itensSelecionados.length > 1)
        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
    else
        window.open(Endereco() + "/mailmarketing/BaixarCartaoPostal?cd_mala_direta=" + itensSelecionados[0].cd_mala_direta + "&enderecoWeb=" + EnderecoAbsoluto(), '_blank');
}
// fim
