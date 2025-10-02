var idSysAdmin = false;
var ATIVOS = 1;
var CAD_ESCOLA_USUARIO = 2;


function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridUsuario';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "no_login", grid._by_idx[rowIndex].item.no_login);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        setTimeout("configuraCheckBox(" + value + ", 'no_login', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mascarar(ready) {
    ready(function () {
        try {
            maskHour('#timeIni');
            maskHour('#timeFim');
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formartExistEmpresa(value, rowIndex, obj) {
    try {
        var gridName = 'gridUsuario';
        var grid = dijit.byId(gridName);
        var retorno;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        retorno = grid._by_idx[rowIndex].item.dc_empresas;

        if (!grid._by_idx[rowIndex].item.possui_varias_escolas)
            dojo.byId(this.id).style.display = 'none';

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxEscola(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasEscolas');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxEscola'  id='" + id + "'/> ";

        setTimeout("configuraCheckBoxEscola(" + value + ", 'cd_pessoa', 'selecionaEscola', " + rowIndex + ", '" + id + "', 'selecionaTodasEscolas', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroUsuario() {
    require([
    "dojo/dom",
    "dijit/registry",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/DataGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dojo/ready",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dojo/data/ItemFileReadStore"
    ], function (dom, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, ready, DropDownButton, DropDownMenu, MenuItem, ItemFileReadStore) {
        ready(function () {
            try {
                loadEscolaPesq(jQuery.parseJSON(EscolasUsuario()), Memory);
//                loadEscolaPesq(null, Memory);
                mascarar(ready);
                setMenssageMultiSelect(DIA, 'cbDias');
                dijit.byId("cbDias").on("change", function (e) {
                    setMenssageMultiSelect(DIA, 'cbDias');
                });

                dijit.byId('tabContainer').resize();
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/Escola/GetUsuarioSearch?descricao=&nome=&inicio=false&status=1&escola=0&pesqSysAdmin=false",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }), Memory({}));

                var grid = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        //{ name: "Código", field: "cd_usuario", width: "50px", styles: "text-align: right; min-width:50px; max-width:50px;" },
                        { name: "Usuário", field: "no_login", width: "15%" },
                        { name: "Nome", field: "no_pessoa", width: "30%", styles: "min-width:10%; max-width: 30%;" },
                        { name: "Escola(s)", field: "possui_varias_escolas", width: "30%", styles: "min-width:10%; max-width: 30%;", formatter: formartExistEmpresa },
                        { name: "Ativo", field: "ativo", width: "8%", styles: "text-align: center;" },
                        { name: "Administrador", field: "Administrador", width: "90px", styles: "text-align: center; min-width:80px; max-width: 100px;" },
                        { name: "Supervisor", field: "Master", width: "80px", styles: "text-align: center; min-width:80px; max-width: 80px;" },
                        { name: "SysAdmin", field: "sysAdmin", width: "80px", styles: "text-align: center; min-width:80px; max-width: 80px;" },
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                }, "gridUsuario"); // make sure you have a target HTML element with this id
                grid.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 4; };
                //grid.canSort = function (col) { return true; };

                grid.on("StyleRow", function (row) {
                    try {
                        var item = grid.getItem(row.index);
                        if (hasValue(item) && !item.possui_varias_escolas)
                            row.node.children[0].children[0].children[0].children[3].style.display = 'none';
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                grid.startup();
                grid.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        destroyCreateGridHorario();
                        getLimpar();
                        apresentaMensagem('apresentadorMensagemUsuario', null);
                        IncluirAlterar(0, 'divAlterarUsuario', 'divIncluirUsuario', 'divExcluirUsuario', 'apresentadorMensagem', 'divCancelarUsuario', 'divClearUsuario');
                        keepValues(item, grid, false);
                        setarTabCad();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                grid.pagination.plugin._paginator.plugin.connect(grid.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, grid, 'no_login', 'selecionaTodos');
                });
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { incluir(); } }, "incluirUsuario");
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisar(true); } }, "pesquisar");
                decreaseBtn(document.getElementById("pesquisar"), '32px');

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            getLimpar();
                            destroyCreateGridHorario();
                            setarTabCad();
                            apresentaMensagem('apresentadorMensagemUsuario', null);
                            keepValues(null, dijit.byId('gridUsuario'), null);
                            criarGridPermissao();
                            if (jQuery.parseJSON(Master()) == true || idSysAdmin) {
                                criarGridGrupo();
                                criarGridEscola();
                            } else {
                                criarGridGrupoUsuario();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarUsuario");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formulario").hide(); } }, "fecharUsuario");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        try {
                            apresentaMensagem('apresentadorMensagemUsuario', null);
                            apresentaMensagem('apresentadorMensagem', null);
                            configurarLayoutUse(false, false);
                            destroyCreateGridPermissoes();
                            destroyCreateGridHorario();
                            getLimpar();
                            setarTabCad();
                            dijit.byId("ind_ativo").set("checked", true);
                            dijit.byId("id_bloqueado").set("checked", false);
                            dijit.byId("id_trocar_senha").set("checked", true);
                            IncluirAlterar(1, 'divAlterarUsuario', 'divIncluirUsuario', 'divExcluirUsuario', 'apresentadorMensagem', 'divCancelarUsuario', 'divClearUsuario');
                            if (jQuery.parseJSON(MasterGeral())) {
                                dijit.byId("ind_administrador").set("disabled", false);
                                destroyCreateGridEscolas();
                                //se DrowpEscola abilitada, desabilita
                                toggleDisabled(dijit.byId("codEscola"), true);
                                //se Permissoes desabilitada, abilita
                                enableDisableTabContainer("tabPermissoes", false);
                                //se TabEscolas desabilitada, abilita
                                if (hasValue(dijit.byId("tabEscolas")) && dijit.byId("tabEscolas").disabled == true)
                                    enableDisableTabContainer("tabEscolas", false);
                                dijit.byId("formulario").show();

                            } else if ((jQuery.parseJSON(Master()) == true || idSysAdmin) && jQuery.parseJSON(EscolasUsuario()).length > 0) {
                                destroyCreateGridEscolas();
                                toggleDisabled(dijit.byId("ind_administrador"), false);
                                //loadDadosEscola(data);
                                //se DrowpEscola desabilitada, abilita
                                toggleDisabled(dijit.byId("codEscola"), true);
                                //se Permissoes desabilitada, abilita
                                enableDisableTabContainer("tabPermissoes", false);
                                //se TabEscolas abilitada, desabilita
                                if (hasValue(dijit.byId("tabEscolas")) && dijit.byId("tabEscolas").disabled == true)
                                    enableDisableTabContainer("tabEscolas", false);
                                if (idSysAdmin) {
                                    toggleDisabled(dijit.byId("ind_administrador"), true);
                                    dijit.byId("ind_administrador").set("checked", true);
                                }
                                dijit.byId("formulario").show();
                            } else {
                                dojo.byId("paiGridGruposUsuario").style.display = "block";
                                dojo.byId("paiGridGrupos").style.display = "none";
                                dojo.byId("paiGridEscolas").style.display = "none";
                                toggleDisabled(dijit.byId("ind_administrador"), true);
                                toggleDisabled(dijit.byId("codEscola"), false);
                                //se Permissoes desabilitada, abilita
                                enableDisableTabContainer("tabPermissoes", false);
                                //se TabEscolas abilitada, desabilita
                                loadDadosEscola(jQuery.parseJSON(EscolasUsuario()));
                                dijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
                                toggleDisabled(dijit.byId("codEscola"), true);
                                //if (hasValue(dijit.byId("tabEscolas")) && dijit.byId("tabEscolas").disabled == false)
                                //    enableDisableTabContainer("tabEscolas", true);
                                dijit.byId("formulario").show();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novo");
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            var codEscolaSelecFiltro = hasValue(dijit.byId("escolaUsuario").value) ? dijit.byId("escolaUsuario").value : 0;
                            dojo.xhr.get({
                                url: Endereco() + "/Escola/GetUrlRelatorioUsuario?" + getStrGridParameters('gridUsuario') + "descricao=" + document.getElementById("pesquisatext").value + "&nome=" +
                                                dojo.byId("nomPessoa").value + "&inicio=" + document.getElementById("inicioUsuario").checked + "&status=" + retornaStatus("statusUsuario") + "&escola=" +
                                                codEscolaSelecFiltro + "&pesqSysAdmin=" + document.getElementById("sysAdminPesq").checked ,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data.retorno, '930px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioUsuario");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { updateUsuario(); } }, "alterarUsuario");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletar() }); } }, "deleteUsuario");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { getLimpar(); } }, "limparUsuario");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {                            
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK(false);
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFK(true, 0);
                                    });
                                });
                            else
                                abrirPessoaFK(false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPessoa");
                //Botões aba horário
                new Button({
                    label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconInsert",
                    onClick: function () { incluirItemHorario(); }
                }, "incluirHorario");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { excluiItemHorario(); }
                }, "excluirHorario");
                //fim botões aba horário
                //new
                var pesPessoa = document.getElementById('pesPessoa');

                if (hasValue(pesPessoa)) {
                    pesPessoa.parentNode.style.minWidth = '18px';
                    pesPessoa.parentNode.style.width = '18px';
                }
                if (hasValue(document.getElementById("limparPessoaRelac"))) {
                    document.getElementById("limparPessoaRelac").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaRelac").parentNode.style.width = '40px';
                }
                montarStatususuario();
                //configurar a FK de Pessoa
                var tipoPessoaFK = dijit.byId("tipoPessoaFK");

                if (hasValue(tipoPessoaFK))
                    tipoPessoaFK.set("value", 1);
                enableDisableTabContainer("tipoPessoaFK", true);
                //pesquisaPessoaFK();
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(grid, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'no_login', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridUsuario')", grid.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(grid.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverUsuario(grid.itensSelecionados); }
                });
                menu.addChild(acaoRemover);

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
                    onClick: function () { buscarTodosItens(grid, 'todosItens', ['pesquisar', 'relatorioUsuario']); pesquisar(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridUsuario', 'selecionado', 'no_login', 'selecionaTodos', ['pesquisar', 'relatorioUsuario'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);
                registry.byId("proPessoa").on("Show", function (e) {
                    try {
                        dijit.byId("gridPesquisaPessoa")._clearData();
                        dijit.byId("gridPesquisaPessoa")._by_idx = [];
                        dijit.byId("gridPesquisaPessoa").update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("limparPessoaRelac").on("click", function (e) {
                    try {
                        caixaDialogo(DIALOGO_CONFIRMAR, 'Deseja limpar esse(s) registro(s)?', function executaRetorno() {
                            try {
                                dojo.byId('cdPessoa').value = 0;
                                dojo.byId("noPessoa").value = "";
                                dojo.byId("cpf").value = "";
                                dojo.byId("sexo").value = "";
                                dijit.byId('noPessoa').set("disabled", false);
                                dijit.byId('cpf').set("disabled", false);
                                dijit.byId('sexo').set("disabled", false);
                                dijit.byId('limparPessoaRelac').set("disabled", true);
                                dijit.byId("email").set('value', "");
                                dijit.byId('email').set("disabled", false);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("id_trocar_senha").on("click", function (e) {
                    showMensageStateusuario();
                });
                registry.byId("id_bloqueado").on("click", function (e) {
                    showMensageStateusuario();
                });
                registry.byId("ind_ativo").on("click", function (e) {
                    showMensageStateusuario();
                });
                loadSexo('sexo', Memory);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322976', '765px', '771px');
                        });
                }
                if (!jQuery.parseJSON(MasterGeral())) {
                    dojo.byId("trSysAdmin").style.display = "none";
                    grid.layout.setColumnVisibility(6, false);
                    grid.layout.setColumnVisibility(7, false);
                    dojo.byId("tdLblSysAdmin").style.display = "none";
                    dojo.byId("tdSysAdmin").style.display = "none";
                }

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try {                            
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarEscolasFK();
                                    });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisarEscolasFK();
                                    });
                                    abrirPessoaFK(true);
                                });
                            else
                                abrirPessoaFK(true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirEscolaFK");

                // Adiciona link de ações aba ESCOLA:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function ()
                    {
                        var grid = dijit.byId("gridEscolas");
                        eventoRemoverEscola(grid.itensSelecionados);
                    }
                });
                menu.addChild(acaoRemover);

                var acaoFiltro = new MenuItem({
	                label: "Mostrar Filtro",
	                onClick: function () {
		                dijit.byId("gridEscolas").showFilterBar(true);
	                }
                });
                menu.addChild(acaoFiltro);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadasEscola"
                });
                dom.byId("linkSelecionadosEscola").appendChild(button.domNode);

                populaDiasSemana(registry, ItemFileReadStore);

                criarGridMenusAreaRestrita();

                montarLegenda();
                showCarregando();

                
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

//function inserirIdTabsCadastro() {
//    if (hasValue(dojo.byId("tabContainer_tablist_tabHorarios")))
//        dojo.byId("tabContainer_tablist_tabHorarios").parentElement.id = "paiTabHorarios";
//}

function clickTabAreaRestrita()
{
    if (hasValue(dojo.byId("noPessoa").value)) {
        dojo.byId("nomeAreaRestrita").value = dojo.byId("noPessoa").value;
    }

    if (hasValue(dojo.byId("email").value)) {
        dojo.byId("emailAreaRestrita").value = dojo.byId("email").value;
    }

}

function showMensageStateusuario() {
    try {
        var compTrocarSenha = dijit.byId("id_trocar_senha");
        var compBloqueado = dijit.byId("id_bloqueado");
        var compAtivo = dijit.byId("ind_ativo");
        if ((compTrocarSenha.get("checked") && compBloqueado.get("checked")) || (compTrocarSenha.get("checked") && !compAtivo.get("checked"))) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgInfoTrocarSenha);
            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
        } else
            apresentaMensagem('apresentadorMensagemUsuario', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSexo(idSexo, Memory) {
    try {
        var stateStore = new Memory({
            data: [
                    { name: "Feminino", id: 1 },
                    { name: "Masculino", id: 2 },
					{ name: "Não Binário", id: 3 },
					{ name: "Prefiro não responder ou Neutro", id: 4 },
            ]
        });
        dijit.byId(idSexo).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            getLimpar();
            destroyCreateGridHorario();
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(null, dijit.byId('gridUsuario'), true);
            IncluirAlterar(0, 'divAlterarUsuario', 'divIncluirUsuario', 'divExcluirUsuario', 'apresentadorMensagem', 'divCancelarUsuario', 'divClearUsuario');
            setarTabCad();
            dijit.byId("formulario").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaoInicial() {
    require([
           "dijit/registry",
           "dojo/ready",
           "dojo/on",
           "dojo/require",
           "dojo/domReady!"
    ], function (registry, ready, on) {
        ready(function () {
            try {
                $("#cpf").mask("999.999.999-99");
                if (hasValue(dijit.byId("cpf")))
                    dijit.byId("cpf").on("blur", function (evt) {
                        try {
                            if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__")
                                if (validarCPF())
                                    //validarCPF() ?
                                    verificarPessoCPF();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                VerificarSysAdmin();
                $('#nom_usuario').focusout(function () {
                    try {
                        if ((!jQuery.parseJSON(dojo.byId("usuarioValido").value) && hasValue(dojo.byId("nom_usuario").value)) || ((jQuery.parseJSON(dojo.byId("usuarioValido").value)) && (dojo.byId("nom_usuario").value != dojo.byId("nomUserValid").value))) {
                            dojo.xhr.get({
                                preventCache: true,
                                url: Endereco() + "/usuario/verifyExistLoginOK?login=" + document.getElementById("nom_usuario").value + "&nomePessoa=" + document.getElementById("noPessoa").value,
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try {
                                    if (hasValue(data) && data.indexOf('<') >= 0) {
                                        data = jQuery.parseJSON(data);
                                        if (data != null) {
                                            apresentaMensagem('apresentadorMensagemUsuario', data);
                                            dojo.byId("usuarioValido").value = false;
                                            dojo.byId("nomUserValid").value = "";
                                            $('#nom_usuario').focus();
                                        } else {
                                            dojo.byId("usuarioValido").value = true;
                                            dojo.byId("nomUserValid").value = dojo.byId("nom_usuario").value;
                                            apresentaMensagem('apresentadorMensagemUsuario', null);
                                        }
                                    }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemUsuario', error);
                            });
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("id_master").on("change", function (isChecked) {
                    try {
                        if (isChecked && dijit.byId("tabPermissoes").disabled == false)
                            enableDisableTabContainer("tabPermissoes", true);
                        if (isChecked == false && dijit.byId("tabPermissoes").disabled == true)
                            enableDisableTabContainer("tabPermissoes", false);
                        enableDisableTabContainer("tabHorarios", isChecked);
                        toggleDisabled(dijit.byId("tabEscolas"), isChecked);
                        enableDisableTabContainer("tabEscolas", isChecked);
                        
                        if (isChecked) {
                            var adm = dijit.byId("ind_administrador");
                            adm._onChangeActive = false;
                            adm.set("value", false);
                            adm._onChangeActive = true;
                        }

                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("ind_administrador").on("change", function (isChecked) {
                    try {
                        if (isChecked && dijit.byId("tabPermissoes").disabled == false)
                            enableDisableTabContainer("tabPermissoes", true);
                        if (isChecked == false && dijit.byId("tabPermissoes").disabled == true)
                            enableDisableTabContainer("tabPermissoes", false);
                        if (dijit.byId("id_sysAdmin").checked == true) {
                            enableDisableTabContainer("tabAreaRestrita", true);
                        } else {
                            enableDisableTabContainer("tabAreaRestrita", false);
                        }
                            
                        enableDisableTabContainer("tabHorarios", isChecked);
                        enableDisableTabContainer("tabEscolas", false);
                        if (isChecked) {
                            var adm = dijit.byId("id_master");
                            adm._onChangeActive = false;
                            adm.set("value", false);
                            adm._onChangeActive = true;
                        }

                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                registry.byId("id_sysAdmin").on("change", function (isChecked) {
                    try {
                        if (isChecked) {
                            limparPessoaUsuario();
                            dijit.byId("ind_administrador").set("checked", true);
                            dijit.byId("ind_administrador").set("disabled", true);
                            configurarLayoutUse(true, true);

                            enableDisableTabContainer("tabAreaRestrita", true);

                        } else {
                            dijit.byId("ind_administrador").set("checked", false);
                            dijit.byId("ind_administrador").set("disabled", false);
                            configurarLayoutUse(false, false);
                            enableDisableTabContainer("tabAreaRestrita", false);

                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function VerificarSysAdmin() {
    dojo.xhr.get({
        url: Endereco() + "/api/escola/verificarSysAdmin",
        sync: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            idSysAdmin = data.retorno;
            if (jQuery.parseJSON(Master()) || idSysAdmin) {
                //closeTabContainer("tabContainer", "tabEscolas");
                //enableDisableTabContainer("tabEscolas", true);
                //dojo.byId("trEscola").style("dislpay", "none");
                dojo.byId("trEscola").style.display = "none";
            }
            //Trocar o nome da aba de acordo com o usuario logado
            if (jQuery.parseJSON(Master()) || idSysAdmin)
                dojo.byId("tabContainer_tablist_tabEscolas").innerHTML = "Escolas";
            else
                dojo.byId("tabContainer_tablist_tabEscolas").innerHTML = "Grupos";
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function destroyCreateGridEscolas() {
    try {
        if (hasValue(dijit.byId("gridGrupoUsuario"))) {
            dijit.byId("gridGrupoUsuario").destroy();
            $('<div>').attr('id', 'gridGrupoUsuario').appendTo('#paiGridGruposUsuario');
        }
        if (hasValue(dijit.byId("gridEscolas"))) {
            dijit.byId("gridEscolas").destroy();
            $('<div>').attr('id', 'gridEscolas').appendTo('#paiGridEscolas');
        }
        if (hasValue(dijit.byId("gridGrupo"))) {
            dijit.byId("gridGrupo").destroy();
            $('<div>').attr('id', 'gridGrupo').appendTo('#paiGridGrupos');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridPermissoes() {
    try {
        if (hasValue(dijit.byId("gridPermissoes"))) {
            dijit.byId("gridPermissoes").destroy();
            $('<div>').attr('id', 'gridPermissoes').appendTo('#paiGridPermissoes');
            //dijit.byId("gridPermissoes").click();
            //$('#gridPermissoes').trigger('click');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink) {
    try {
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];
        if (value != null && value.cd_usuario > 0) {
            if (value.id_admin)
                configurarLayoutUse(true, true);
            else
                configurarLayoutUse(true, false);
            showEditUsuario(value.cd_usuario);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaOutrosFilhosMarcadosRecursivo(item, campo, id, pai) {
    try {
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
    }
    catch (e) {
        postGerarLog(e);
    }
}

function marcarRecursivo(pai, item, campo, obj) {
    try {
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
                if (hasValue(item.children))
                    for (var idx1 = 0; idx1 < item.children.length; idx1++) {
                        var retornoRecursivo = marcarRecursivo(pai, item.children[idx1], campo, obj);

                        if (retornoRecursivo != null) {
                            retorno = retornoRecursivo;
                            break;
                        }
                    }

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Função não perfomática para buscar o pai e marcar ele. A função não é perfomática, pois é O(n*2) e poderia ser O(log n). Entretanto, ela percorre somente os itens da árvore mostrados na tela.
function marcarPaisRecursivo(item, campo, obj) {
    try {
        var gridPermissoes = dijit.byId("gridPermissoes");
        if (hasValue(item) && item.pai != 0) {
            var pai = null;

            for (var p = 0; p < gridPermissoes._by_idx.length && pai == null; p++)
                pai = marcarRecursivo(item.pai, gridPermissoes._by_idx[p].item, campo, obj);
            marcarPaisRecursivo(pai, campo, obj);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaCheckBox(dijitObj, value) {
    try {
        if (hasValue(dijitObj) && !dijitObj.disabled) {
            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function marcarFilhosRecursivo(item, campo, obj, filho) {
    try {
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
                    if (document.getElementById('excluir_Selected_' + item._0)) {
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
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangePermissoes(rowIndex, campo, obj) {
    try {
        var gridPermissoes = dijit.byId("gridPermissoes");
        marcarFilhosRecursivo(gridPermissoes._by_idx[rowIndex].item, campo, obj, false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxGrupo(value, rowIndex, obj, k) {
    try {
        var gridGrupo = dijit.byId("gridGrupo");
        var icon;
        var id = k.field + '_Selected_' + gridGrupo._by_idx[rowIndex].item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1 && (hasValue(value) || value == false))
            icon = "<input id='" + id + "' class='formatCheckBox' /> ";
        setTimeout("configuraCheckBoxGrupo(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "')", 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxGrupo(value, rowIndex, field, id) {
    try {
        if (!hasValue(dijit.byId(id)))
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value,
                onChange: function (b) { checkBoxChangeGrupo(rowIndex, field, this) }
            }, id);
        else {
            var dijitObj = dijit.byId(id);

            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
            dijitObj.onChange = function (b) { checkBoxChangeGrupo(rowIndex, field, this) };
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeGrupo(rowIndex, campo, obj) {
    try {
        var gridGrupo = dijit.byId("gridGrupo");
        var gridEscolas = dijit.byId("gridEscolas");
        if (hasValue(gridGrupo) && hasValue(gridGrupo._by_idx[rowIndex].item)) {
            gridGrupo._by_idx[rowIndex].item.pertenceGrupo[0] = obj.checked;
            //var storeEscola = hasValue(gridEscolas.itensSelecionados) ? gridEscolas.itensSelecionados : null;

            var storeEscola = hasValue(dijit.byId("gridEscolas").store.objectStore.data) ? dijit.byId("gridEscolas").store.objectStore.data : null;
            if (storeEscola.length > 0) {
                $.each(storeEscola, function (index, value) {
                    if (hasValue(value.Grupos))
                        $.each(value.Grupos, function (idx, val) {
                            if (val.cd_grupo == gridGrupo._by_idx[rowIndex].item.cd_grupo[0]) {
                                storeEscola[index].Grupos[idx].ehSelecionado = obj.checked;
                                //dijit.byId("gridEscolas")._by_idx[index].item.Grupos[idx].ehSelecionado = obj.checked;
                                return
                            }
                        });
                });
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPermissoes(value, rowIndex, obj, k) {
    try {
        var gridPermissoes = dijit.byId("gridPermissoes");
        var icon;
        var id = k.field + '_Selected_' + gridPermissoes._by_idx[rowIndex].item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1)
            icon = "<input id='" + id + "' class='formatCheckBox' /> ";

        setTimeout("configuraCheckBoxPermissoes(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "', " + gridPermissoes._by_idx[rowIndex].item.ehPermitidoEditar + ")", 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxPermissoes(value, rowIndex, field, id, ehPermitidoEditar) {
    try {
        var gridPermissoes = dijit.byId("gridPermissoes");

        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value && (ehPermitidoEditar || field == "visualizar"),
                onChange: function (b) { checkBoxChangePermissoes(rowIndex, field, this) }
            }, id);
            checkBox.set('disabled', !ehPermitidoEditar && field != "visualizar");
        }
        else {
            var dijitObj = dijit.byId(id);

            dijitObj._onChangeActive = false;
            dijitObj.set('value', value && (ehPermitidoEditar || field == "visualizar"));
            dijitObj._onChangeActive = true;
            dijitObj.onChange = function (b) { checkBoxChangePermissoes(rowIndex, field, this) };
            dijitObj.set('disabled', !ehPermitidoEditar && field != "visualizar");
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxEscola(value, field, fieldTodos, rowIndex, id, idTodos, gridName) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try {
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);

                if (id != idTodos || (hasValue(grid.pagination) && !grid.pagination.plugin._showAll)) {

                    // Se id for seleciona todos, verifica se todos estão marcados para marcá-lo:
                    if (id == idTodos) {
                        var j = 0;
                        var campo = dojo.byId(fieldTodos + '_Selected_' + j);
                        value = hasValue(campo);

                        while (hasValue(campo) && value) {
                            if (campo.type == 'text') {
                                setTimeout("configuraCheckBoxEscola(" + value + ", '" + field + "', '" + fieldTodos + "', " + rowIndex + ", '" + id + "', '" + idTodos + "', '" + gridName + "')", grid.rowsPerPage * 3);
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
                    if (hasValue(dojoId) && dojoId.type == 'text')
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            checked: value,
                            onChange: function (b) {
                                checkBoxChange(rowIndex, field, fieldTodos, idTodos, this, grid);
                                //loadGridGrupo();
                            }
                        }, id);
                }
                else if (hasValue(dojo.byId(idTodos)))
                    dojo.byId(idTodos).parentNode.removeChild(dojo.byId(idTodos));
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxGrupoUsuario(value, rowIndex, obj) {
    try {
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input  id='" + id + "' class='formatCheckBox' /> ";

        setTimeout("configuraCheckBoxGrupoUsuario(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxGrupoUsuarioChange(rowIndex) {
    try {
        var gridGrupoUsuario = dijit.byId("gridGrupoUsuario");
        gridGrupoUsuario._by_idx[rowIndex].item.ehSelecionado = !gridGrupoUsuario._by_idx[rowIndex].item.ehSelecionado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxGrupoUsuario(value, rowIndex, id) {
    try {
        if (hasValue(dijit.byId(id)) && dojo.byId(id).type == 'text')
            dijit.byId(id).destroy();

        if (dojo.byId(id).type == 'text')
            require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
                ready(function () {
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        onChange: function (b) { checkBoxGrupoUsuarioChange(rowIndex); }
                    }, id);
                })
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}
//Depracated
function montarEscolasUsuarioSelecionada() {
    var storeGridEscolas;
    require(["dojo/data/ObjectStore", "dojo/store/Memory"], function (ObjectStore, Memory) {
        try {
            if (jQuery.parseJSON(Master())) {
                storeGridEscolas = hasValue(dijit.byId("gridEscolas").store.objectStore.data) ? dijit.byId("gridEscolas").store.objectStore.data : null;
                var storeDropEscolasUsuario = hasValue(dijit.byId("codEscola").store.data) ? dijit.byId("codEscola").store.data : null;
                if (storeDropEscolasUsuario != null && storeGridEscolas != null) {
                    var itemEncontrado = false;
                    $.each(storeDropEscolasUsuario, function (index, val) {
                        if (itemEncontrado)
                            return;
                        $.each(storeGridEscolas, function (idx, value) {
                            if (val.id == value.cd_pessoa) {
                                storeGridEscolas[idx].ehSelecionado = true;
                                itemEncontrado = true;
                                return;
                            }
                        })
                    })
                    //var dados = jQuery.parseJSON(storeGridEscolas);
                    storeGridEscolas = new ObjectStore({ objectStore: new Memory({ data: storeGridEscolas }) });
                    // dijit.byId("gridEscolas").setStore(dataStore);
                }

            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
    return storeGridEscolas;
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        //so vai criar a gridPermissao quando ela ainda nao estiver criado.
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabPermissoes' && !hasValue(dijit.byId("gridPermissoes"))) {
            criarGridPermissao();
            //so vai criar a gridEscolas quando ela ainda nao estiver criado.
        } else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabEscolas' && !hasValue(dijit.byId("gridEscolas"))) {
            //if (jQuery.parseJSON(Master()))
            if (jQuery.parseJSON(Master()) || idSysAdmin) {
                criarGridGrupo();
                criarGridEscola();
            } else if (!hasValue(dijit.byId("gridGrupoUsuario")))
                criarGridGrupoUsuario();
        } else if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAreaRestrita' && !hasValue(dijit.byId("gridAreaRestrita"))) {
            clickTabAreaRestrita();

        }


        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabHorarios' && !hasValue(dijit.byId("gridHorarios"))) {
            if (hasValue(dojo.byId("cod_usuario").value) && dojo.byId("cod_usuario").value > 0)
                loadHorarioUsuario(dojo.byId("cod_usuario").value);
            else {
                showCarregando();
                criacaoGradeHorario(null);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGridPermissao() {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/permissao/getfuncionalidadesusuarioarvore?cdUsuario=" + document.getElementById("cod_usuario").value,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            //apresentaMensagem('apresentadorMensagemUsuario', data);
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
            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            /* set up layout */
            var layout = [
              { name: 'Permissão', field: 'permissao', width: '54%' },
              { name: 'Visualizar', field: 'visualizar', width: '13%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
              { name: 'Incluir', field: 'incluir', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
              { name: 'Alterar', field: 'alterar', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
              { name: 'Excluir', field: 'excluir', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxPermissoes },
              { name: '', field: 'id', width: '0%', styles: "display: none;" },
              { name: '', field: 'pai', width: '0%', styles: "display: none;" }
            ];

            /* create a new grid: */
            if (hasValue(dijit.byId("gridPermissoes")))
                destroyCreateGridPermissoes();
            var gridPermissoes = new dojox.grid.LazyTreeGrid({
                id: 'gridPermissoes',
                treeModel: model,
                structure: layout
            }, document.createElement('div'));

            dojo.byId("gridPermissoes").appendChild(gridPermissoes.domNode);
            gridPermissoes.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemUsuario', error);
    });
}

function retirarChildrenLenthIgualZero(array) {
    try {
        for (var i = 0; hasValue(array) && i < array.length; i++)
            if (hasValue(array[i].children) && array[i].children.length > 0)
                retirarChildrenLenthIgualZero(array[i].children);
            else
                array[i] = {
                    id: array[i].id,
                    alterar: array[i].alterar,
                    excluir: array[i].excluir,
                    incluir: array[i].incluir,
                    visualizar: array[i].visualizar,
                    permissao: array[i].permissao,
                    ehPermitidoEditar: array[i].ehPermitidoEditar,
                    pai: array[i].pai
                };

        return array;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGridEscola() {
    require(["dojox/grid/EnhancedGrid", "dojo/store/JsonRest", "dojo/data/ItemFileWriteStore",
        "dojo/data/ObjectStore", "dojo/store/Memory", "dojo/store/Cache", "dojo/on", "dojox/grid/enhanced/plugins/Pagination", "dojox/grid/enhanced/plugins/Filter"
    ], function (EnhancedGrid, JsonRest, ItemFileWriteStore, ObjectStore, Memory, Cache, on, Pagination, Filter) {
        try {
	        
            var gridEscolas = new EnhancedGrid({
                store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                structure: [
                    { name: "<input id='selecionaTodasEscolas' style='display:none'/>", field: "selecionaEscola", width: "25px", styles: "text-align: center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEscola, filterable: false},
                    { name: "Nome", field: "dc_reduzido_pessoa", width: "98%" }
                ],                
                plugins: {
                    pagination: {
                        pageSizes: ["17", "34", "68", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "17",
                        gotoButton: true,
                        maxPageStep: 4,
                        position: "button",
                        plugins: {
	                        nestedSorting: true,
	                        
                        }
                    },
                    filter: {
	                    closeFilterbarButton: true,
	                    ruleCount: 1,
	                    itemsName: "Nome"
                    }
                }
            }, "gridEscolas");
            gridEscolas.canSort = function (col) { return Math.abs(col) != 1; };
            gridEscolas.sortInfo = 2;
            //gridEscolas.showFilterBar(true);
            gridEscolas.startup();
            
            if (dojo.byId("cod_usuario").value > 0) {
                
                montarEscolasEndGruposUsuarioEdit();
                gridEscolas.startup();
                //loadGridGrupo();
            } else
                gridEscolas.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function destroyCreateGridAreaRestrita() {
    try {
        if (hasValue(dijit.byId("gridMenusAreaRestrita"))) {
            dijit.byId("gridMenusAreaRestrita").destroyRecursive();
            $('<div>').attr('id', 'gridMenusAreaRestrita').attr('style', 'height:285px;').appendTo('#paigridAreaRestrita');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxEventoSelecionado(value, rowIndex, obj) {
    try {
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input  id='" + id + "' class='formatCheckBox' /> ";

        setTimeout("configuraCheckBoxEventoSelecionado(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxEventoSelecionado(value, rowIndex, id) {
    try {
        if (hasValue(id) && hasValue(dijit.byId(id)) && hasValue(dojo.byId(id).type) && dojo.byId(id).type == 'text')
            dijit.byId(id).destroy();
        if (hasValue(dojo.byId(id)) && dojo.byId(id).type == 'text')
            require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
                ready(function () {
                    try {
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            disabled: false,
                            checked: value,
                            onChange: function (b) { checkBoxEventoSelecionadoChange(rowIndex); }
                        }, id);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                })
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxEventoSelecionadoChange(rowIndex) {
    try {
        apresentaMensagem('apresentadorMensagemUsuario', null);
        if (jQuery.parseJSON(Master()) == false) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Somente usuário master pode cadastrar/alterar os menus da Área Restrita.");
            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            var gridMenusAreaRestrita = dijit.byId("gridMenusAreaRestrita");
            gridMenusAreaRestrita._by_idx[rowIndex].item.selecionadoMenu = gridMenusAreaRestrita._by_idx[rowIndex].item.selecionadoMenu;
            gridMenusAreaRestrita.refresh();
            return;
        }

        var gridMenusAreaRestrita = dijit.byId("gridMenusAreaRestrita");
        gridMenusAreaRestrita._by_idx[rowIndex].item.selecionadoMenu = !gridMenusAreaRestrita._by_idx[rowIndex].item.selecionadoMenu;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGridMenusAreaRestrita() {
    dojo.ready(function () {
        try {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/Empresa/getMenusAreaRestrita",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                preventCache: true,
                handleAs: "json",
            }).then(function (data) {
                try {
                    destroyCreateGridAreaRestrita();
                    apresentaMensagem("apresentadorMensagemUsuario", null);
                    
                    if (data != null &&
                        data.retorno != null &&
                        data.retorno.data != null &&
                        data.retorno.data.length > 0) {

                        data = data.retorno.data;
                        var gridMenusAreaRestrita = new dojox.grid.EnhancedGrid({
                                store: new dojo.data.ObjectStore({
                                    objectStore: new dojo.store.Memory({ data: data })
                                }),
                                structure: [
                                    { name: "Menu", field: "title", width: "90%", styles: "min-width:180px;" },
                                    {
                                        name: "-",
                                        field: 'selecionadoMenu',
                                        width: '10%',
                                        styles: "text-align: center;",
                                        formatter: formatCheckBoxEventoSelecionado
                                    },
                                ],
                                noDataMessage: msgNotRegEnc,
                                canSort: true,
                                selectionMode: "single"
                            },
                            "gridMenusAreaRestrita");
                        gridMenusAreaRestrita.startup();

                        
                    } else {
                        
                        var gridMenusAreaRestrita = new dojox.grid.EnhancedGrid({
                                store: new dojo.data.ObjectStore({
                                    objectStore: new dojo.store.Memory({ data: null })
                                }),
                                structure: [
                                    { name: "Menu", field: "title", width: "90%", styles: "min-width:180px;" },
                                    {
                                        name: "-",
                                        field: 'selecionadoMenu',
                                        width: '10%',
                                        styles: "text-align: center;",
                                        formatter: formatCheckBoxEventoSelecionado
                                    },
                                ],
                                noDataMessage: msgNotRegEnc,
                                canSort: true,
                                selectionMode: "single"
                            },
                            "gridMenusAreaRestrita");
                        gridMenusAreaRestrita.startup();

                        
                    }


                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem("apresentadorMensagemUsuario", error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarEscolasEndGruposUsuarioEdit() {
    require([
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dojo/ready"],
       function (JsonRest, ObjectStore, Cache, Memory, ready) {
           ready(function () {
               try {
                   var escolasUser = null;
                   var sysGrupoUser = null;
                   var gridEscola = dijit.byId("gridEscolas");

                   if (hasValue(dijit.byId("gridUsuario").itemSelecionado) && hasValue(dijit.byId("gridUsuario").itemSelecionado.SysGrupo))
                       sysGrupoUser = dijit.byId("gridUsuario").itemSelecionado.SysGrupo;

                   if (hasValue(dijit.byId("gridUsuario").itemSelecionado) && hasValue(dijit.byId("gridUsuario").itemSelecionado.Empresas)) {
                       escolasUser = dijit.byId("gridUsuario").itemSelecionado.Empresas;
                   }

                   dataStore = new ObjectStore({ objectStore: new Memory({ data: escolasUser }) });
                   gridEscola.setStore(dataStore);

                   //ação que marca os grupo do usuario editado. marca nos itensSelecionado na grade Escola.
                   var storeEscolasLogin = hasValue(dijit.byId("gridEscolas").store.objectStore.data) ? dijit.byId("gridEscolas").store.objectStore.data : null;
                   if (sysGrupoUser != null && storeEscolasLogin != null && storeEscolasLogin.length > 0)
                       for (var i = 0; i < sysGrupoUser.length; i++)
                           for (var k = 0; k < storeEscolasLogin.length; k++)
                               for (var j = 0; j < storeEscolasLogin[k].Grupos.length; j++)
                                   if (sysGrupoUser[i].cd_grupo == storeEscolasLogin[k].Grupos[j].cd_grupo)
                                       storeEscolasLogin[k].Grupos[j].ehSelecionado = true;

                   loadGridGrupo();
               }
               catch (e) {
                   postGerarLog(e);
               }
           });
       });
}

function loadGridGrupo() {
    try {
        // var itensGridEscola = dijit.byId("gridEscolas").selection.getSelected();
        var itensGridEscola = dijit.byId("gridEscolas").store.objectStore.data;
        //var storeGrupo = hasValue(gridUserEscolas.getItem(rowIndex)) ? gridUserEscolas.getItem(rowIndex) : null;
        var dataItens = [];
        var dataGrupo = [];
        if (itensGridEscola != null && itensGridEscola.length > 0) {
            $.each(itensGridEscola, function (index, value) {
                dataGrupo = [];
                if (hasValue(value.Grupos)) {
                    $.each(value.Grupos, function (idx, val) {
                        dataGrupo.push({
                            id: val.cd_grupo + '-' + val.cd_pessoa_escola,
                            cd_grupo: val.cd_grupo,
                            nome: val.no_grupo,
                            cd_pessoa_escola: val.cd_pessoa_escola,
                            pertenceGrupo: val.ehSelecionado
                        });
                    });
                }
                dataItens.push({
                    id: value.cd_pessoa,
                    cd_pessoa: value.cd_pessoa,
                    nome: hasValue(value.dc_reduzido_pessoa) ? value.dc_reduzido_pessoa : value.no_pessoa,
                    children: dataGrupo
                });
            });
        }
        var data = {
            identifier: 'id',
            label: 'nome',
            items: dataItens
        };
        var store = new dojo.data.ItemFileWriteStore({ data: data });
        var model = new dijit.tree.ForestStoreModel({
            store: store, childrenAttrs: ['children']
        });
        dijit.byId("gridGrupo").setModel(model);
        //dijit.byId("gridGrupo").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGridGrupo() {
    var dataItens = [];
    var data = {
        identifier: 'cd_pessoa',
        label: 'nome',
        items: dataItens
    };
    var store = new dojo.data.ItemFileWriteStore({ data: data });
    var model = new dijit.tree.ForestStoreModel({
        store: store, childrenAttrs: ['children']
    });

    /* set up layout */
    var layout = [
     { name: 'Escolas\\Grupo', field: 'nome', width: '30%' },
     { name: '&nbsp;', field: 'pertenceGrupo', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxGrupo }
    ];

    /* create a new grid: */
    var gridGrupo = new dojox.grid.LazyTreeGrid({
        id: 'gridGrupo',
        treeModel: model,
        selectionMode: "single",
        canSort: false,
        //noDataMessage: msgNotRegEnc,
        structure: layout
    }, document.createElement('div'));

    dojo.byId("paiGridGrupos").appendChild(gridGrupo.domNode);
    gridGrupo.startup();
}

function criarGridGrupoUsuario() {
    require(["dojo/_base/xhr", "dojox/grid/EnhancedGrid", "dojo/data/ObjectStore", "dojo/store/Memory", "dojo/query"
    ], function (xhr, EnhancedGrid, ObjectStore, Memory, query) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/permissao/getgrupo",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                var sysGrupoUser = null;
                apresentaMensagem('apresentadorMensagemUsuario', data);
                var storeGrid = [];
                if (hasValue(dijit.byId("gridUsuario").itemSelecionado) && hasValue(dijit.byId("gridUsuario").itemSelecionado.SysGrupo))
                    sysGrupoUser = dijit.byId("gridUsuario").itemSelecionado.SysGrupo;
                if (sysGrupoUser != null && sysGrupoUser.length > 0) {
                    $.each(sysGrupoUser, function (index, val) {
                        $.each(data, function (idx, value) {
                            if (val.cd_grupo == value.cd_grupo) {
                                data[idx].ehSelecionado = true;
                                return;
                            }
                        });
                    });
                    //var dados = jQuery.parseJSON(storeGridEscolas);
                    storeGrid = new ObjectStore({ objectStore: new Memory({ data: data }) });
                    //dijit.byId("gridEscolas").setStore(dataStore);
                } else {
                    storeGrid = new ObjectStore({ objectStore: new Memory({ data: data }) });
                }
                //var storeGrid = new ObjectStore({ objectStore: new Memory({ data: data }) });
                /* create a new grid: */
                var gridGrupoUsuario = new EnhancedGrid({
                    store: storeGrid,
                    selectionMode: "single",
                    //structure: gridLayout,
                    //keepSelection: true,
                    canSort: false,
                    noDataMessage: msgNotRegEnc,
                    structure: [
                       { name: 'Marcar', field: 'ehSelecionado', width: '11%', styles: "text-align: center;", formatter: formatCheckBoxGrupoUsuario },
                       //{ name: "Código", field: "cd_grupo", width: "10%" },
                       { name: "Descrição", field: "no_grupo", width: "83%" }
                    ]
                }, "gridGrupoUsuario");
                gridGrupoUsuario.canSort = function (col) { return false; };
                gridGrupoUsuario.startup();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemUsuario', error);
        });
    });
}

function getLimpar() {
    try {
        dijit.byId("id_sysAdmin")._onChangeActive = false;
        $("#formUsuario").get(0).reset();
        dijit.byId("id_sysAdmin")._onChangeActive = true;
        dojo.byId("cod_usuario").value = 0;
        dijit.byId("codEscola").reset();
        dijit.byId("codEscola").store.data = null;
        dijit.byId("ind_administrador").reset();
        dijit.byId("ind_ativo").reset();
        dijit.byId("id_bloqueado").reset();
        dijit.byId("id_trocar_senha").reset();

        dijit.byId("email").set('value', "");
        dijit.byId('noPessoa').set("disabled", false);
        dijit.byId('cpf').set("disabled", false);
        dijit.byId('email').set("disabled", false);
        dijit.byId("sexo").set("disabled", false);
        dijit.byId("gridUsuario").itemSelecionado = null;
        limparStoreEscolas();
        limparGrupoUsuario();
        dijit.byId('cbDias').setStore(dijit.byId('cbDias').store, []);
        limparUsuario();

        dojo.byId('nomeAreaRestrita').value = "";
        dojo.byId('emailAreaRestrita').value = "";
        var gridMenusAreaRestrita = dijit.byId("gridMenusAreaRestrita");

        if (gridMenusAreaRestrita != null && gridMenusAreaRestrita != undefined) {
            for (var i = 0; i < gridMenusAreaRestrita._by_idx.length; i++) {
                gridMenusAreaRestrita._by_idx[i].item.selecionadoMenu = false;
            }
        }



    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparUsuario() {
    try {
        if (hasValue(dijit.byId("gridPesquisaPessoa")))
            limparPesquisaPessoaFK();
        //document.getElementById("cod_usuario").value = null;
        //Configurando o cadastro para o usuário logado.
        apresentaMensagem('apresentadorMensagemUsuario', null);
        apresentaMensagem('apresentadorMensagem', null);
        configurarLayoutUse(false, false);
        destroyCreateGridPermissoes();
        destroyCreateGridHorario();
        setarTabCad();
        dijit.byId("ind_ativo").set("checked", true);
        dijit.byId("id_bloqueado").set("checked", false);
        dijit.byId("id_trocar_senha").set("checked", true);
        if (jQuery.parseJSON(MasterGeral())) {
            dijit.byId("ind_administrador").set("disabled", false);
            dijit.byId("ind_administrador").set("value", false);
            dijit.byId("id_sysAdmin").set("value", false);
            destroyCreateGridEscolas();
            //se DrowpEscola abilitada, desabilita
            toggleDisabled(dijit.byId("codEscola"), true);
            //se Permissoes desabilitada, abilita
            enableDisableTabContainer("tabPermissoes", false);
            //se TabEscolas desabilitada, abilita
            if (hasValue(dijit.byId("tabEscolas")) && dijit.byId("tabEscolas").disabled == true)
                enableDisableTabContainer("tabEscolas", false);
        }
        else if ((jQuery.parseJSON(Master()) == true || idSysAdmin) && jQuery.parseJSON(EscolasUsuario()).length > 0) {
            dijit.byId("id_master").set("value", false);
            destroyCreateGridEscolas();
            toggleDisabled(dijit.byId("id_master"), false);
            //loadDadosEscola(data);
            //se DrowpEscola desabilitada, abilita
            toggleDisabled(dijit.byId("codEscola"), true);
            //se Permissoes desabilitada, abilita
            enableDisableTabContainer("tabPermissoes", false);
            //se TabEscolas abilitada, desabilita
            if (hasValue(dijit.byId("tabEscolas")) && dijit.byId("tabEscolas").disabled == true)
                enableDisableTabContainer("tabEscolas", false);
            if (idSysAdmin) {
                toggleDisabled(dijit.byId("ind_administrador"), true);
                dijit.byId("ind_administrador").set("checked", true);
                enableDisableTabContainer("tabPermissoes", true);
            }
        } else {
            dojo.byId("paiGridGruposUsuario").style.display = "block";
            dojo.byId("paiGridGrupos").style.display = "none";
            dojo.byId("paiGridEscolas").style.display = "none";
            toggleDisabled(dijit.byId("ind_administrador"), true);
            toggleDisabled(dijit.byId("codEscola"), false);
            //se Permissoes desabilitada, abilita
            enableDisableTabContainer("tabPermissoes", false);
            //se TabEscolas abilitada, desabilita
            loadDadosEscola(jQuery.parseJSON(EscolasUsuario()));
            dijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
            toggleDisabled(dijit.byId("codEscola"), true);
        }

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
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPermissoesRecursivo(storechildren) {
    try {
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
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparStoreEscolas() {
    try {
        var gridEscolas = dijit.byId("gridEscolas");
        if (hasValue(gridEscolas) && hasValue(gridEscolas.itensSelecionados) && gridEscolas.itensSelecionados.length > 0) {
            gridEscolas.itensSelecionados = new Array();
            if (hasValue(gridEscolas._by_idx) && gridEscolas._by_idx.length > 0) {
                $.each(gridEscolas._by_idx, function (index, value) {
                    if (hasValue(value) && hasValue(value.item) && hasValue(value.item.Grupos)) {
                        $.each(value.item.Grupos, function (idx, val) {
                            if (val.ehSelecionado == true) {
                                gridEscolas._by_idx[index].item.Grupos[idx].ehSelecionado = false;
                            }
                        });
                    }
                });
            }
            gridEscolas.update();
            loadGridGrupo();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGrupoUsuario() {
    try {
        if (hasValue(dijit.byId("gridGrupoUsuario")) && hasValue(dijit.byId("gridGrupoUsuario").store.objectStore.data)) {
            var storeGridGrupoUsuario = dijit.byId("gridGrupoUsuario").store.objectStore.data;
            if (storeGridGrupoUsuario.length > 0) {
                $.each(storeGridGrupoUsuario, function (index, value) {
                    dijit.byId("gridGrupoUsuario").store.objectStore.data[index].ehSelecionado = false;
                });
            };
            dijit.byId("gridGrupoUsuario").update();
        };
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabCadastro");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function Permissoes(id, alterar, excluir, incluir, visualizar, permissao, children) {
    this.alterar = alterar;
    this.excluir = excluir;
    this.id = id;
    this.incluir = incluir;
    this.permissao = permissao;
    this.children = children;
    this.visualizar = visualizar;
}

function getPermissoesFromGridRecursivo(array) {
    try{
    var arrayPermissoes = new Array();
    if (hasValue(array))
        for (var i = 0; i < array.length; i++) {
            if (hasValue(array[i].children))
                arrayPermissoes[i] = new Permissoes(array[i].id[0], array[i].alterar[0], array[i].excluir[0], array[i].incluir[0], array[i].visualizar[0], array[i].permissao[0], getPermissoesFromGridRecursivo(array[i].children));
            else {
                arrayPermissoes[i] = new Permissoes(array[i].id[0], array[i].alterar[0], array[i].excluir[0], array[i].incluir[0], array[i].visualizar[0], array[i].permissao[0], null);
            }
        }

    return arrayPermissoes;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarPermissoesSelecionadasFromGridRecursivo(array) {
    try {
        var retorno = true; // Não tem nenhuma permissão marcada.

        for (var i = 0; i < array.length && retorno; i++)
            if (array[i].visualizar[0])
                retorno = false; // Tem uma permissão marcada.
            else
                getPermissoesFromGridRecursivo(array[i].children);

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGruposSelecionados() {
    try {
        var grupos = [];
        var gridEscolas = dijit.byId("gridEscolas");
        if (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data))
            var storeEscolas = gridEscolas.store.objectStore.data;
        if (hasValue(storeEscolas) && storeEscolas.length > 0)
            $.each(storeEscolas, function (idx, val) {
                $.each(val.Grupos, function (index, value) {
                    if (value.ehSelecionado == true)
                        // grupos.push(value);
                        grupos.push({
                            "cd_grupo": value.cd_grupo,
                            "cd_pessoa_escola": value.cd_pessoa_escola,
                            "ehSelecionado": value.ehSelecionado,
                            "no_grupo": value.no_grupo
                        });
                });
            });
        else
            grupos = null;

        return grupos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGruposUsuarioSelecionado() {
    try {
        var dados = [];
        if (hasValue(dijit.byId("gridGrupoUsuario")) && hasValue(dijit.byId("gridGrupoUsuario").store.objectStore.data)) {
            var storeGridGrupoUsuario = dijit.byId("gridGrupoUsuario").store.objectStore.data;
            if (storeGridGrupoUsuario.length > 0) {
                $.each(storeGridGrupoUsuario, function (index, value) {
                    if (value.ehSelecionado == true)
                        dados.push({
                            "cd_grupo": value.cd_grupo,
                            "cd_pessoa_escola": value.cd_pessoa_escola,
                            "ehSelecionado": value.ehSelecionado,
                            "no_grupo": value.no_grupo
                        });
                    //dados.push(value);
                });
            } else
                dados = null;
        }
        return dados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarEscolasSelecionadosForBeackEnd() {
    try {
        var dados = [];
        var gridEscola = dijit.byId("gridEscolas");
        if (hasValue(gridEscola) && hasValue(gridEscola.store.objectStore.data))
            dados = gridEscola.store.objectStore.data;
        return dados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//metodos C.R.U.D
function incluir() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            var permissoesWeb = null;
            var escolasSelecionadas = [];
            var grupoUsuario = null;
            var mensagensWeb = new Array();
            var atualizouPermissoes = false;
            var horarios = null;

            var gridMenusAreaRestrita = hasValue(dijit.byId("gridMenusAreaRestrita")) ? dijit.byId("gridMenusAreaRestrita").store.objectStore.data : [];

            var userAreaRestrita = (dijit.byId("id_sysAdmin").checked == false &&
                hasValue(dojo.byId('noPessoa').value) &&
                hasValue(dojo.byId('email').value)) ?  {
                name: !hasValue(dom.byId("nomeAreaRestrita").value) ? dom.byId("noPessoa").value : dom.byId("nomeAreaRestrita").value,
                email: !hasValue(dom.byId("emailAreaRestrita").value) ? dom.byId("email").value : dom.byId("emailAreaRestrita").value,
                menus: gridMenusAreaRestrita.filter(function (value) { return (value.selecionadoMenu != null && value.selecionadoMenu != undefined && value.selecionadoMenu == true) }).map(function (value) { return value.id })

            } : null;

            if (!dijit.byId("formUsuario").validate()) {
                setarTabCad();
                return false;
            }

            //Usuario Normal
            if (!document.getElementById("id_sysAdmin").checked && validarCPF() == false)
                return false;
            if (!hasValue(dojo.byId('des_senha').value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo senha é obrigatório.");
                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                return false;
            }

           /* if (dijit.byId("id_sysAdmin").checked == false) {

                if (!hasValue(dojo.byId('nomeAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo nome da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }

                if (!hasValue(dojo.byId('emailAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo email da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                } 


                if (!hasValue(dojo.byId('passwordAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo senha da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }
            }*/
            

            if (hasValue(document.getElementById("id_master")) && !document.getElementById("id_master").checked) {
                if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
                    permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
                    atualizouPermissoes = true;
                }


                if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx))
                    escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();


                if (hasValue(dijit.byId("codEscola")) && hasValue(dijit.byId("codEscola").get("value"))) {
                    escolasSelecionadas.push({
                        'cd_pessoa': dijit.byId("codEscola").get("value")
                    });
                } else if (escolasSelecionadas.length <= 0) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }


                if (escolasSelecionadas.length <= 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }


                if (hasValue(document.getElementById("ind_administrador")) && !document.getElementById("ind_administrador").checked && !idSysAdmin) {
                    if (hasValue(dijit.byId("gridGrupoUsuario"))) {
                        grupoUsuario = montarGruposUsuarioSelecionado();

                    }
                    if (!hasValue(grupoUsuario))
                        if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
                            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                            return false;
                        }
                    if (hasValue(dijit.byId("gridHorarios"))) {
                        atualizaHorarios = true;
                        if (dijit.byId("gridHorarios").items.length > 0)
                            horarios = dijit.byId('gridHorarios').params.store.data;
                        if (horarios == null || horarios.length == 0) {
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                            return false;
                        }
                    } else {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                        apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                        return false;
                    }
                }

            }    

            //if (!idSysAdmin && !jQuery.parseJSON(Master())) {
            //    if (hasValue(document.getElementById("id_master")) && !document.getElementById("id_master").checked) {
                    //if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
                    //    permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
                    //    atualizouPermissoes = true;
                    //}
                    //if (hasValue(dijit.byId("codEscola")) && hasValue(dijit.byId("codEscola").get("value"))) {
                    //    escolasSelecionadas.push({
                    //        'cd_pessoa': dijit.byId("codEscola").get("value")
                    //    });
                    //} else {
                    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
                    //    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    //    return false;
                    //}
                //    if (hasValue(document.getElementById("ind_administrador")) && !document.getElementById("ind_administrador").checked) {
                //        if (hasValue(dijit.byId("gridGrupoUsuario"))) {
                //            grupoUsuario = montarGruposUsuarioSelecionado();

                //        }
                //        if (!hasValue(grupoUsuario))
                //            if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
                //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
                //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                //                return false;
                //            }
                //        if (hasValue(dijit.byId("gridHorarios"))) {
                //            atualizaHorarios = true;
                //            if (dijit.byId("gridHorarios").items.length > 0)
                //                horarios = dijit.byId('gridHorarios').params.store.data;
                //            if (horarios == null || horarios.length == 0) {
                //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                //                return false;
                //            }
                //        } else {
                //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                //            return false;
                //        }
                //    }
            //    }
            //}
            //else if (!jQuery.parseJSON(MasterGeral()) && (jQuery.parseJSON(Master()) || idSysAdmin)) {
            //    if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx))
            //        escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
            //    if (hasValue(document.getElementById("ind_administrador")) && !document.getElementById("ind_administrador").checked) {
            //        if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
            //            permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
            //            atualizouPermissoes = true;
            //        }

            //        if (hasValue(dijit.byId("gridGrupo"))) {
            //            grupoUsuario = montarGruposSelecionados();
            //        }
            //        if (!hasValue(grupoUsuario))
            //            if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
            //                var mensagensWeb = new Array();
            //                mensagensWeb[0] = new MensagensWeb(2, msgLackPermissionUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }
            //        if (escolasSelecionadas.length <= 0) {
            //            var mensagensWeb = new Array();
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //        if (hasValue(dijit.byId("gridHorarios"))) {
            //            atualizaHorarios = true;
            //            if (dijit.byId("gridHorarios").items.length > 0)
            //                horarios = dijit.byId('gridHorarios').params.store.data;
            //            if (horarios == null || horarios.length == 0) {
            //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }
            //        } else {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    } else {
            //        if (escolasSelecionadas.length <= 0) {
            //            var mensagensWeb = new Array();
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    }
            //    //}

            //} else if (jQuery.parseJSON(MasterGeral())) {

            //    if (hasValue(document.getElementById("ind_administrador")) && !document.getElementById("ind_administrador").checked 
            //        && !document.getElementById("id_master").checked) {
            //        if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
            //            permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
            //            atualizouPermissoes = true;
            //        }
            //        if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx))
            //            escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
            //        if (hasValue(dijit.byId("gridGrupo")))
            //            grupoUsuario = montarGruposSelecionados();
            //        if (hasValue(escolasSelecionadas) && escolasSelecionadas.length > 0) {
            //            if (!hasValue(grupoUsuario))
            //                if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
            //                    var mensagensWeb = new Array();
            //                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
            //                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                    return false;
            //                }
            //        } else {
            //            var mensagensWeb = new Array();
            //            mensagensWeb[0] = new MensagensWeb(2, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //        if (hasValue(dijit.byId("gridHorarios"))) {
            //            atualizaHorarios = true;
            //            if (dijit.byId("gridHorarios").items.length > 0)
            //                horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
            //            if (horarios == null || horarios.length == 0) {
            //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }
            //        } else {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    } else {
            //        if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx))
            //            escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
            //        if (hasValue(dijit.byId("gridGrupo"))) {
            //            grupoUsuario = montarGruposSelecionados();
            //        }
            //        if (document.getElementById("id_sysAdmin").checked && escolasSelecionadas.length <= 0) {
            //            var mensagensWeb = new Array();
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    }
            //}
            showCarregando();
            xhr.post({
                url: Endereco() + "/usuario/PostInsertUsuario",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(
                    {
                        UsuarioWeb: {
                            cd_usuario: dom.byId("cod_usuario").value,
                            cd_pessoa: dojo.byId("cdPessoa").value,
                            no_login: dom.byId("nom_usuario").value,
                            dc_senha_usuario: dom.byId("des_senha").value,
                            id_administrador: domAttr.get("ind_administrador", "checked"),
                            id_master: domAttr.get("id_master", "checked"),
                            id_usuario_ativo: domAttr.get("ind_ativo", "checked"),
                            id_bloqueado: domAttr.get("id_bloqueado", "checked"),
                            id_trocar_senha: domAttr.get("id_trocar_senha", "checked"),
                            id_admin: domAttr.get("id_sysAdmin", "checked")
                        },
                        permissoes: permissoesWeb,
                        escolas: removerGruposEscolasSelecionadas(escolasSelecionadas),
                        gruposUsuario: grupoUsuario,
                        nm_pessoa_usuario: dom.byId("noPessoa").value,
                        nm_cpf: dom.byId("cpf").value,
                        nm_sexo: dijit.byId("sexo").value,
                        email: dom.byId("email").value,
                        horarios: horarios,
                        userAreaRestrita: userAreaRestrita,
                    }),
                load: function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridUsuario';
                            var grid = dijit.byId(gridName);

                            apresentaMensagem('apresentadorMensagem', data);
                            dijit.byId("formulario").hide();

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];

                            var ativo = dom.byId("ind_ativo").checked ? 1 : 2;
                            dijit.byId("statusUsuario").set("value", ativo);
                            insertObjSort(grid.itensSelecionados, "no_login", itemAlterado);

                            buscarItensSelecionados(gridName, 'selecionado', 'no_login', 'selecionaTodos', ['pesquisar', 'relatorioUsuario'], 'todosItens', 3);
                            //grid.sortInfo = 3;
                            setGridPagination(grid, itemAlterado, "no_login");
                            showCarregando();
                        } else {
                            apresentaMensagem('apresentadorMensagemUsuario', data.erro);
                            showCarregando();
                        }
                    }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }
                },
                error: function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemUsuario', error);
                    
                }
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });

}

function mountHorarios(dataHorarios) {
    try {
        var retorno = [];
        $.each(dataHorarios, function (index, value) {
            if (!hasValue(value.removeTimeZone)) {
                value.endTime = removeTimeZone(value.endTime);
                value.startTime = removeTimeZone(value.startTime);
                value.removeTimeZone = true;
            }
            retorno.push(value);
        });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function updateUsuario() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            var permissoesWeb = null;
            var escolasSelecionadas = [];
            var grupoUsuario = null;
            var atualizouPermissoes = false;
            var atualizarEscolas = false;
            var atualizarHorarios = false;
            var horarios = null;
            var mensagensWeb = new Array();

            var gridMenusAreaRestrita = hasValue(dijit.byId("gridMenusAreaRestrita")) ? dijit.byId("gridMenusAreaRestrita").store.objectStore.data : [];

            var userAreaRestrita = (dijit.byId("id_sysAdmin").checked == false &&
                hasValue(dojo.byId('noPessoa').value) &&
                hasValue(dojo.byId('email').value) )
                ?  {
                name: !hasValue(dom.byId("nomeAreaRestrita").value) ? dom.byId("noPessoa").value : dom.byId("nomeAreaRestrita").value,
                email: !hasValue(dom.byId("emailAreaRestrita").value) ? dom.byId("email").value : dom.byId("emailAreaRestrita").value,
                menus: gridMenusAreaRestrita.filter(function (value) { return (value.selecionadoMenu != null && value.selecionadoMenu != undefined && value.selecionadoMenu == true) }).map(function (value) { return value.id })

                } : null;

            /*if (dijit.byId("id_sysAdmin").checked == false )  {

                if (!hasValue(dojo.byId('nomeAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo nome da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }

                if (!hasValue(dojo.byId('emailAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo email da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }

                if (!hasValue(dojo.byId('passwordAreaRestrita').value)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O campo senha da aba Área Restrita é obrigatório quando o usuário não é sysAdmin.");
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }


                
            }*/


            if (!document.getElementById("id_sysAdmin").checked && validarCPF() == false)
                return false;
            if (!dijit.byId("formUsuario").validate()) {
                setarTabCad();
                return false;
            }
            var grid = dijit.byId('gridUsuario');
            var valEditUser = hasValue(grid.itemSelecionado) ? grid.itemSelecionado : null;
            //if (!idSysAdmin && jQuery.parseJSON(MasterGeral())) {
                if (hasValue(document.getElementById("id_master")) && !document.getElementById("id_master").checked) {
                    if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
                        permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
                        atualizouPermissoes = true;
                    }
                    if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx)) {
                        escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
                        atualizarEscolas = true;
                    }
                    if (escolasSelecionadas == null && (document.getElementById("ind_administrador").checked || idSysAdmin))
                        escolasSelecionadas = [];

                    if (hasValue(dijit.byId("gridGrupo"))) {
                        grupoUsuario = montarGruposSelecionados();
                        atualizarEscolas = true;
                    }
                    if ((document.getElementById("id_sysAdmin").checked || document.getElementById("ind_administrador").checked)
                        && hasValue(dijit.byId("gridEscolas")) && escolasSelecionadas.length <= 0) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
                        apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                        return false;
                    }

                    //if (((valEditUser.id_master == true && hasValue(valEditUser.SysGrupo) && valEditUser.SysGrupo.length <= 0) && !hasValue(grupoUsuario)) ||
                    //    (valEditUser.id_master == true && !hasValue(valEditUser.SysGrupo) && !hasValue(grupoUsuario)) ||
                    //    ((valEditUser.id_master == false && hasValue(valEditUser.SysGrupo) && valEditUser.SysGrupo.length <= 0) && !hasValue(grupoUsuario)) ||
                    //    (valEditUser.id_master == false && !hasValue(valEditUser.SysGrupo) && !hasValue(grupoUsuario)))
                    //    if (valEditUser.qtdPermissao <= 0 && (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) ||
                    //        (valEditUser.qtdPermissao == 0 && (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)))) {
                    //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
                    //        apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    //        return false;
                    //    }
                    //if ((!hasValue(valEditUser.Empresas || valEditUser.Empresas.length <= 0) && escolasSelecionadas.length <= 0)) {
                    //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
                    //    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    //    return false;
                    //}
                    if ((!document.getElementById("id_sysAdmin").checked && !document.getElementById("ind_administrador").checked)) {
                        if (hasValue(dijit.byId("gridHorarios"))) {
                            atualizarHorarios = true;
                            if (dijit.byId("gridHorarios").items.length > 0)
                                horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
                            if (horarios == null || horarios.length == 0) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                                return false;
                            }
                        } else {
                            if (valEditUser != null && (valEditUser.qtdHorarios == null || valEditUser.qtdHorarios == 0)) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
                                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                                return false;
                            }
                        }

                        if (hasValue(dijit.byId("gridPermissoes")) || (valEditUser != null && valEditUser.id_master == true && (valEditUser.SysGrupo == null || valEditUser.SysGrupo.length <= 0)))
                            if (!hasValue(grupoUsuario))
                                if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
                                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                                    return false;
                                }
                    }

                }
            //    else {
            //        if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx)) {
            //            escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
            //            atualizarEscolas = true;
            //        }
            //        if (escolasSelecionadas == null && document.getElementById("ind_administrador").checked)
            //            escolasSelecionadas = [];
            //        if (hasValue(dijit.byId("gridGrupo"))) {
            //            grupoUsuario = montarGruposSelecionados();
            //            atualizarEscolas = true;
            //        }
            //        if (document.getElementById("id_sysAdmin").checked && hasValue(dijit.byId("gridEscolas")) && escolasSelecionadas.length <= 0) {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    }
            //} else if (!jQuery.parseJSON(MasterGeral()) && (jQuery.parseJSON(Master()) || idSysAdmin)) {
            //    if (hasValue(dijit.byId("gridEscolas")) && hasValue(dijit.byId("gridEscolas")._by_idx)) {
            //        escolasSelecionadas = montarEscolasSelecionadosForBeackEnd();
            //        atualizarEscolas = true;
            //    }
            //    if (hasValue(document.getElementById("ind_administrador")) && !document.getElementById("ind_administrador").checked) {
            //        if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
            //            permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
            //            atualizouPermissoes = true;
            //        }
            //        if (hasValue(dijit.byId("gridGrupo"))) {
            //            grupoUsuario = montarGruposSelecionados();
            //            atualizarEscolas = true;
            //        }
            //        if (hasValue(dijit.byId("gridPermissoes")) || (valEditUser != null && valEditUser.id_master == true && (valEditUser.SysGrupo == null || valEditUser.SysGrupo.length <= 0)))
            //            if (!hasValue(grupoUsuario))
            //                if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
            //                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
            //                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                    return false;
            //                }
            //        if (hasValue(dijit.byId("gridEscolas")) && escolasSelecionadas.length <= 0) {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //        if (hasValue(dijit.byId("gridHorarios"))) {
            //            atualizarHorarios = true;
            //            if (dijit.byId("gridHorarios").items.length > 0)
            //                horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
            //            if (horarios == null || horarios.length == 0) {
            //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }
            //        } else {
            //            if (valEditUser != null && (valEditUser.qtdHorarios == null || valEditUser.qtdHorarios == 0)) {
            //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }
            //        }

            //    } else {
            //        if (hasValue(dijit.byId("gridGrupo"))) {
            //            grupoUsuario = montarGruposSelecionados();
            //            atualizarEscolas = true;
            //        }
            //        if (hasValue(dijit.byId("gridEscolas")) && escolasSelecionadas.length <= 0) {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    }
            //} else {
            //    if (hasValue(dijit.byId("codEscola")) && hasValue(dijit.byId("codEscola").get("value"))) {
            //        escolasSelecionadas.push({
            //            'cd_pessoa': dijit.byId("codEscola").get("value")
            //        });
            //    } else {
            //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "É obrigatório incluir pelo menos uma escola para o usuário.");
            //        apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //        return false;
            //    }
            //    if (hasValue(dijit.byId("gridPermissoes")) && hasValue(dijit.byId("gridPermissoes").store)) {
            //        permissoesWeb = getPermissoesFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems);
            //        atualizouPermissoes = true;
            //    }
            //    if (hasValue(dijit.byId("gridGrupoUsuario"))) {
            //        grupoUsuario = montarGruposUsuarioSelecionado();
            //        atualizarEscolas = true;
            //    }
            //    if (hasValue(dijit.byId("gridPermissoes")) || (valEditUser != null && valEditUser.id_master == true && (valEditUser.SysGrupo == null || valEditUser.SysGrupo.length <= 0)))
            //        if (!hasValue(grupoUsuario))
            //            if (!hasValue(permissoesWeb) || permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems)) {
            //                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
            //                apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //                return false;
            //            }


            //    //if (hasValue(dijit.byId("gridPermissoes")))
            //    //    if (!hasValue(grupoUsuario))
            //    //        if (hasValue(permissoesWeb) && (permissoesWeb.length <= 0 || verificarPermissoesSelecionadasFromGridRecursivo(dijit.byId("gridPermissoes").store._arrayOfTopLevelItems))) {
            //    //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgLackPermissionUser);
            //    //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //    //            return false;
            //    //        }
            //    if (hasValue(dijit.byId("gridHorarios"))) {
            //        atualizarHorarios = true;
            //        if (dijit.byId("gridHorarios").items.length > 0)
            //            horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
            //        if (horarios == null || horarios.length == 0) {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    } else {
            //        if (valEditUser != null && (valEditUser.qtdHorarios == null || valEditUser.qtdHorarios == 0)) {
            //            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroInfHorarioUser);
            //            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            //            return false;
            //        }
            //    }
            //}

            if (hasValue(dijit.byId("gridHorarios"))) {
                atualizarHorarios = true;
                if (dijit.byId("gridHorarios").items.length > 0)
                    horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
            }
            showCarregando();
            xhr.post({
                url: Endereco() + "/usuario/PostEditUsuario",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(
                    {
                        UsuarioWeb: {
                            cd_usuario: dom.byId("cod_usuario").value,
                            cd_pessoa: dojo.byId("cdPessoa").value,
                            no_login: dom.byId("nom_usuario").value,
                            dc_senha_usuario: dom.byId("des_senha").value,
                            id_master: domAttr.get("id_master", "checked"),
                            id_administrador: domAttr.get("ind_administrador", "checked"),
                            id_usuario_ativo: domAttr.get("ind_ativo", "checked"),
                            id_bloqueado: domAttr.get("id_bloqueado", "checked"),
                            id_trocar_senha: domAttr.get("id_trocar_senha", "checked"),
                            id_admin: domAttr.get("id_sysAdmin", "checked")
                        },
                        permissoes: permissoesWeb,
                        escolas: removerGruposEscolasSelecionadas(escolasSelecionadas),
                        gruposUsuario: grupoUsuario,
                        nm_pessoa_usuario: dom.byId("noPessoa").value,
                        nm_cpf: dom.byId("cpf").value,
                        nm_sexo: dijit.byId("sexo").value,
                        email: dom.byId("email").value,
                        atualizouPermissoes: atualizouPermissoes,
                        atualizouEscolasOrGrupos: atualizarEscolas,
                        atualizouHorarios: atualizarHorarios,
                        horarios: horarios,
                        userAreaRestrita: userAreaRestrita,
                    }),
                load: function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var todos = dojo.byId("todosItens_label");
                            var gridName = 'gridUsuario';
                            var grid = dijit.byId(gridName);


                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];
                            apresentaMensagem('apresentadorMensagem', data);
                            var ativo = dom.byId("ind_ativo").checked ? 1 : 2;
                            dijit.byId("statusUsuario").set("value", ativo);
                            dijit.byId("formulario").hide();

                            removeObjSort(grid.itensSelecionados, "no_login", dom.byId("nom_usuario").value);
                            insertObjSort(grid.itensSelecionados, "no_login", itemAlterado);

                            buscarItensSelecionados(gridName, 'selecionado', 'no_login', 'selecionaTodos', ['pesquisar', 'relatorioUsuario'], 'todosItens');
                            setGridPagination(grid, itemAlterado, "no_login");
                            showCarregando();
                        } else {
                            apresentaMensagem('apresentadorMensagemUsuario', data.erro);
                            showCarregando();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                error: function (error) {
                    apresentaMensagem('apresentadorMensagemUsuario', error);
                    showCarregando();
                }
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function removerGruposEscolasSelecionadas(escolasSelecionadas) {
    if (escolasSelecionadas != null && escolasSelecionadas.length > 0)
        $.each(escolasSelecionadas, function (idx, value) {
            if (hasValue(value.Grupos))
                delete value.Grupos;
        });
    return escolasSelecionadas;
}


function atualizar() {
    pesquisar();
}

function pesquisar(limparItens) {
    require([
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var codEscolaSelecFiltro = hasValue(dijit.byId("escolaUsuario").value) ? dijit.byId("escolaUsuario").value : 0;
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/Escola/GetUsuarioSearch?descricao=" + encodeURIComponent(document.getElementById("pesquisatext").value) + "&nome=" + encodeURIComponent(document.getElementById("nomPessoa").value) + "&inicio=" + document.getElementById("inicioUsuario").checked + "&status=" + retornaStatus("statusUsuario") + "&escola=" + codEscolaSelecFiltro + "&pesqSysAdmin=" + document.getElementById("sysAdminPesq").checked,
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridUsuario = dijit.byId("gridUsuario");
            gridUsuario.setStore(dataStore);

            if (limparItens)
                gridUsuario.itensSelecionados = [];

            gridUsuario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarStatususuario() {
    require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (ready, Memory, filteringSelect) {
         ready(function () {
             try {
                 var statusStore = new Memory({
                     data: [
                         { name: "Todos", id: "0" },
                         { name: "Ativos", id: "1" },
                         { name: "Inativos", id: "2" }
                     ]
                 });
                 dijit.byId("statusUsuario").store = statusStore;
                 dijit.byId("statusUsuario").set("value", ATIVOS);
             }
             catch (e) {
                 postGerarLog(e);
             }
         });
     });
}

function eventoRemoverUsuario(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletar(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletar(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cod_usuario').value != 0)
                    itensSelecionados = [{
                        cd_usuario: dom.byId("cod_usuario").value,
                        cd_pessoa: dojo.byId("cdPessoa").value,
                        no_login: dom.byId("nom_usuario").value,
                        dc_senha_usuario: dom.byId("des_senha").value,
                        id_master: dijit.byId("ind_administrador").get("value")
                    }];

            xhr.post({
                url: Endereco() + "/api/escola/postDeleteUsuario",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados),
                load: function (data) {
                    try {
                        var todos = dojo.byId("todosItens_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("pesquisatext").set("value", '');
                        dijit.byId("formulario").hide();

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridUsuario').itensSelecionados, "no_login", itensSelecionados[r].no_login);

                        pesquisar(false);

                        //dijit.byId('gridUsuario').itensSelecionados = null;

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisar").set('disabled', false);
                        dijit.byId("relatorioUsuario").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                error: function (error) {
                    if (!hasValue(dojo.byId("formulario").style.display))
                        apresentaMensagem('apresentadorMensagemUsuario', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                }
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadDadosEscola(data) {
    try {
        //apresentaMensagem("apresentadorMensagem", data);
        (hasValue(data.retorno)) ? data = jQuery.parseJSON(data).retorno : data;//jQuery.parseJSON(data);
        //data = jQuery.parseJSON(data).retorno;
        var dados = [];
        $.each(data, function (index, val) {
            dados.push({
                "name": val.dc_reduzido_pessoa,
                "id": val.cd_pessoa
            });
        });
        var stateStore = new dojo.store.Memory({
            data: eval(dados)
        });
        dijit.byId("codEscola").store = stateStore;

    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEscolaPesq(data, Memory) {
    require(["dojo/_base/array", "dojo/request/xhr", "dojo/domReady!"], function (Array, xhr) {
        try {
            if (!hasValue(data) || data.length == 0)
                xhr(Endereco() + "/api/escola/getAllEscolaByUsuarioLogin/", {
                    handleAs: "json",
                    preventCache: true,
                    headers: { Accept: "application/json", "Authorization": Token() }
                }).then(function (data) {
                    data = data.retorno;
                    populaEscolasUsuario(data, Memory, Array);
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
            else
                if (hasValue(data) && data.length == 1) {
                    document.getElementById("widget_escolaUsuario").style.display = "none";
                    document.getElementById("escolaUsuario_label").style.display = "none";
                }
                else
                    populaEscolasUsuario(data, Memory, Array);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaEscolasUsuario(data, Memory, Array) {
    try {
        var items = [];
        items.push({ id: 0, name: "Todas" });
        Array.forEach(data, function (value, i) {
            items.push({ id: value.cd_pessoa, name: value.dc_reduzido_pessoa == null ? value.no_pessoa : value.dc_reduzido_pessoa });
        });
        var stateStore = new Memory({
            data: items
        });

        dijit.byId("escolaUsuario").store = stateStore;
        dijit.byId("escolaUsuario").set("value", 0);
        //Esta sendo feito aqui, por causa de sincronização.
        adicionarAtalhoPesquisa(['pesquisatext', 'nomPessoa', 'statusUsuario', 'escolaUsuario'], 'pesquisar', dojo.ready);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadEscola() {
    require(["dojo/ready", "dojo/store/Memory", "dojo/data/ObjectStore", ],
     function (ready, Memory, ObjectStore) {
         ready(function () {
             try {
                 var data = null;
                 dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });

                 var status = new cancelaringSelect({
                     //autoComplete: autoComplete,
                     id: "codEscola",
                     name: "Pessoa",
                     value: "0",
                     store: dataStore,
                     searchAttr: "name",
                     style: "width: 553px;"
                 }, "codEscola");

             }
             catch (e) {
                 postGerarLog(e);
             }
         });
     });
}

function abilitarDesabilitarWidgets(bool) {
    dojo.ready(function () {
        setTimeout('toggleDisabled("codEscola",' + bool + ');', 1000);
    });
}

function loadGridPermissao() {
    try {
        var dados =
            {
                identifier: 'id',
                label: 'permissao',
                items: null
            };
        var store = new dojo.data.ItemFileWriteStore({ data: dados });
        var model = new dijit.tree.ForestStoreModel({
            store: store, childrenAttrs: ['children']
        });

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
        var gridPermissoes = new dojox.grid.LazyTreeGrid({
            id: 'gridPermissoes',
            treeModel: model,
            structure: layout,
            rowSelector: '20px'
        }, document.createElement('div'));

        dojo.byId("gridPermissoes").appendChild(gridPermissoes.domNode);
        gridPermissoes.startup();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function closeTabContainer(tabContainer, tabFilho) {
    dijit.byId(tabContainer).closeChild(dijit.byId(tabFilho));
}

function enableDisableTabContainer(tabFilho, bool) {
    try {
        var tab = dijit.byId(tabFilho)
        if (hasValue(tab))
            tab.set("disabled", bool);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarPessoCPF() {
    var mensagensWeb = new Array();
    if ($("#cpf").val()) {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/pessoa/VerificarPessoaByCpf?cpf=" + $("#cpf").val(),
            handleAs: "json",
            headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                apresentaMensagem('apresentadorMensagemUsuario', null);
                if (data.retorno != null) {
                    $("#cdPessoa").val(data.retorno.cd_pessoa);
                    $("#noPessoa").val(data.retorno.no_pessoa);
                    //$("#sexo").val(data.retorno.nm_sexo);
                    var sexo = dijit.byId("sexo");
                    if (data.retorno.nm_sexo > 0) {
                        sexo._onChangeActive = false;
                        data.retorno.nm_sexo != null ? sexo.set('value', data.retorno.nm_sexo) : "";
                        sexo._onChangeActive = true;
                    }
                    dijit.byId('noPessoa').set("disabled", true);
                    dijit.byId('cpf').set("disabled", true);
                    dijit.byId('sexo').set("disabled", true);
                    dijit.byId('limparPessoaRelac').set("disabled", false);
                } else
                    $("#cdPessoa").val(0);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemUsuario', error);
        });
    }
}

function validarCPF() {
    try {
        var myCPF;
        myCPF = $("#cpf").val().replace('.', '').replace('.', '').replace('-', '');
        var numeros, digitos, soma, i, resultado, digitos_iguais;
        digitos_iguais = 1;

        if (myCPF.length < 11) {
            mostrarMensagenCPF()
            //$("#cpf").focus();
            return false;
        }
        for (i = 0; i < myCPF.length - 1; i++)
            if (myCPF.charAt(i) != myCPF.charAt(i + 1)) {
                digitos_iguais = 0;
                break;
            }
        if (!digitos_iguais) {
            numeros = myCPF.substring(0, 9);
            digitos = myCPF.substring(9);
            soma = 0;
            for (i = 10; i > 1; i--)
                soma += numeros.charAt(10 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(0)) {
                mostrarMensagenCPF()
                //$("#cpf").focus();
                return false;
            }
            numeros = myCPF.substring(0, 10);
            soma = 0;
            for (i = 11; i > 1; i--)
                soma += numeros.charAt(11 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1)) {
                mostrarMensagenCPF()
                // $("#cpf").focus();
                return false;
            }
            return true;
        }
        else {
            mostrarMensagenCPF()
            //$("#cpf").focus();
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostrarMensagenCPF() {
    var mensagensWeb = new Array();
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "CPF inválido.");
    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
}

function showCarregandoAll(elemento) {
    try {
        var divCarregando = document.getElementById(elemento);
        var divWrapper = dojo.byId('paiGridGruposUsuario');

        if (hasValue(divCarregando) && hasValue(divWrapper))
            if (divCarregando.style.display == 'none') {
                divWrapper.style.opacity = '0.6';
                divCarregando.style.display = '';
            }
            else {
                divWrapper.style.opacity = '100';
                divCarregando.style.display = 'none';
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Retorno FKpessoa

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPessoaSelec.itensSelecionados;
            $("#cdPessoa").val(value[0].cd_pessoa);
            $("#noPessoa").val(value[0].no_pessoa);
            if (hasValue(dojo.byId("cpf")))
                $("#cpf").val(value[0].nm_cpf_cgc)
            dijit.byId('noPessoa').set("disabled", true);
            dijit.byId('cpf').set("disabled", true);
            dijit.byId('limparPessoaRelac').set("disabled", false);
            if (hasValue(value[0].email) != null && hasValue(value[0].email) != "")
                dijit.byId('email').set('value', value[0].email);
            else
                dijit.byId('email').reset();
            dijit.byId('email').set("disabled", true);
            apresentaMensagem('apresentadorMensagemUsuario', null);
            dijit.byId("proPessoa").hide();
            var sexo = dijit.byId("sexo");
            if (value[0].nm_sexo > 0) {
                sexo._onChangeActive = false;
                value[0].nm_sexo != null ? sexo.set('value', value[0].nm_sexo) : "";
                sexo._onChangeActive = true;
                sexo.set("disabled", true);
            }
        }
        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configurarLayoutUse(habilitar, sysAdmin) {
    try {
        dijit.byId('noPessoa').set("disabled", habilitar);
        dijit.byId('cpf').set("disabled", habilitar);
        dijit.byId('sexo').set("disabled", habilitar);
        dijit.byId('limparPessoaRelac').set("disabled", !habilitar);
        if (sysAdmin) {
            dijit.byId('limparPessoaRelac').set("disabled", habilitar);
            dijit.byId('pesPessoa').set("disabled", habilitar);
            dijit.byId('email').set("disabled", habilitar);
        } else {
            dijit.byId('pesPessoa').set("disabled", false);
            dijit.byId('email').set("disabled", false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditUsuario(cdUsuario) {
    dojo.xhr.get({
        url: Endereco() + "/api/escola/getAllDataUsuariobyId?cdUsuario=" + parseInt(cdUsuario),
        sync: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            value = data.retorno;
            if (value != null) {
                dijit.byId("gridUsuario").itemSelecionado = value;
                dijit.byId("ind_ativo").set("checked", false);
                dijit.byId("id_bloqueado").set("checked", false);
                dijit.byId("id_trocar_senha").set("checked", value.id_trocar_senha);
                dojo.byId("cod_usuario").value = value.cd_usuario;
                dojo.byId("nom_usuario").value = value.no_login;
                dojo.byId("des_senha").value = "";
                dojo.byId("usuarioValido").value = true;
                dojo.byId("nomUserValid").value = value.no_login;
                if (value.PessoaFisica != null)
                    dijit.byId("email").set('value', value.PessoaFisica.email);

                dijit.byId("email").set('disabled', true);

                if (value.id_usuario_ativo) {
                    dijit.byId("ind_ativo").set("checked", true);
                }
                if (value.id_bloqueado) {
                    dijit.byId("id_bloqueado").set("checked", true);
                }

                if (value.id_master) {
                    dijit.byId("id_master").set("checked", true);

                }
                if (value.id_admin) {
                    //configurarLayoutUse(true,false);
                    dijit.byId("ind_administrador").set("value", true);  //LBM estava ind_administrador
                    dijit.byId("ind_administrador").set("disabled", true);  //
                    dijit.byId("id_sysAdmin")._onChangeActive = false;
                    dijit.byId("id_sysAdmin").set("checked", true);
                    dijit.byId("id_sysAdmin")._onChangeActive = true;
                }

                if (value.id_administrador) {
                    dijit.byId("ind_administrador").set("checked", true);
                }

                if (hasValue(value.PessoaFisica)) {
                    dojo.byId("cdPessoa").value = value.PessoaFisica.cd_pessoa;
                    dojo.byId("cpf").value = hasValue(value.PessoaFisica.nm_cpf) ? value.PessoaFisica.nm_cpf : "";
                    dojo.byId("noPessoa").value = value.PessoaFisica.no_pessoa;

                    var sexo = dijit.byId("sexo");
                    if (value.PessoaFisica.nm_sexo > 0) {
                        sexo._onChangeActive = false;
                        value.PessoaFisica.nm_sexo != null ? sexo.set('value', value.PessoaFisica.nm_sexo) : "";
                        sexo._onChangeActive = true;
                        sexo.set("disabled", true);
                    }
                }
                //dojo.byId("cdPessoa").value = value[0].cd_pessoa;
                //Carrega os dados se usuario for master.
                if (hasValue(jQuery.parseJSON(MasterGeral())) && jQuery.parseJSON(MasterGeral())) {
                    destroyCreateGridEscolas();
                    //Valida se o usuario a ser editado e administrador geral.
                    // se master == true ele checa o valor (ind_administrador). seta o valor das escolas que tem acesso no dorpDown.
                    //LBMif (value.id_master && (!hasValue(value.Empresas) || value.Empresas.length == 0)) {
                        toggleDisabled(dijit.byId("tabEscolas"), true);
                    //LBM}
                    //LBMelse if (value.id_master) {
                        destroyCreateGridPermissoes();
                        //loadDadosEscola(value.Empresas);
                        //LBMdijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
                    //LBM} else {
                        //LBMdestroyCreateGridPermissoes();
                        //LBMloadDadosEscola(value.Empresas);
                        //toggleDisabled(dijit.byId("tabEscolas"), false);
                        //enableDisableTabContainer("tabEscolas", false);
                        enableDisableTabContainer("tabPermissoes", false);
                        //LBM dijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
                    //LBM}
                    toggleDisabled(dijit.byId("codEscola"), true);
                    dijit.byId("formulario").show();

                } else if ((jQuery.parseJSON(Master()) == true || idSysAdmin) && jQuery.parseJSON(EscolasUsuario()).length > 0) {
                    toggleDisabled(dijit.byId("ind_administrador"), false);
                    destroyCreateGridEscolas();
                    destroyCreateGridPermissoes();
                    //loadDadosEscola(value.Escolas);
                    //dijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
                    toggleDisabled(dijit.byId("codEscola"), true);
                    //dojo.byId("nom_completo").value = store.getValue(item, "no_completo");
                    dojo.byId("cdPessoa").value = value.cd_pessoa;
                    enableDisableTabContainer("tabEscolas", false);
                    if (idSysAdmin) {
                        toggleDisabled(dijit.byId("ind_administrador"), true);
                        dijit.byId("ind_administrador").set("checked", true);
                    }
                    dijit.byId("formulario").show();
                } else {
                    dojo.byId("paiGridGruposUsuario").style.display = "block";
                    dojo.byId("paiGridGrupos").style.display = "none";
                    dojo.byId("paiGridEscolas").style.display = "none";
                    toggleDisabled(dijit.byId("ind_administrador"), true);
                    destroyCreateGridPermissoes();
                    destroyCreateGridEscolas();
                    loadDadosEscola(value.Empresas);
                    dijit.byId("codEscola").set("value", jQuery.parseJSON(Escola()));
                    toggleDisabled(dijit.byId("codEscola"), true);
                    //dojo.byId("nom_completo").value = store.getValue(item, "no_completo");
                    // enableDisableTabContainer("tabEscolas", true);
                    enableDisableTabContainer("tabPermissoes", false);
                    dijit.byId("formulario").show();
                }
                var gridMenusAreaRestrita = dijit.byId("gridMenusAreaRestrita")

                if (hasValue(value.menusAreaRestrita) &&
                    gridMenusAreaRestrita != null &&
                    gridMenusAreaRestrita != undefined) {
                    for (var j = 0; j < value.menusAreaRestrita.length; j++) {
                        for (var i = 0; i < gridMenusAreaRestrita._by_idx.length; i++) {
                            if (value.menusAreaRestrita[j] == (gridMenusAreaRestrita._by_idx[i].item.id + "")) {
                                gridMenusAreaRestrita._by_idx[i].item.selecionadoMenu = true;
                            }

                        }
                    }
                        

                    
                } else {
                    for (var i = 0; i < gridMenusAreaRestrita._by_idx.length; i++) {
                        gridMenusAreaRestrita._by_idx[i].item.selecionadoMenu = false;
                    }
                }

                if (hasValue(value.emailAreaRestrita)) {
                    dojo.byId("emailAreaRestrita").value = value.emailAreaRestrita;
                }

                if (hasValue(value.nameAreaRestrita)) {
                    dojo.byId("nomeAreaRestrita").value = value.nameAreaRestrita;
                }


                
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function retornarItemEditUsuario() {
    try{
        var value = null;
        var grid = dijit.byId('gridUsuario');
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        var ehLink = eval(linkAnterior.value);
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];
        return value;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFK(isPessoaEscola) {
    try {

        if (isPessoaEscola) {
            dijit.byId("proPessoa").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia:";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            dojo.byId("idOrigemPessoaFK").value = CAD_ESCOLA_USUARIO;
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        else {
            limparPesquisaPessoaFK();
            dijit.byId("tipoPessoaFK").set("value", 1);
            dijit.byId("tipoPessoaFK").set("disabled", true);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Nome Reduzido:";
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Nome Reduzido";
            dojo.byId("idOrigemPessoaFK").value = "";
            dijit.byId("proPessoa").set("title", "Pesquisar Pessoa");

            pesquisaPessoaFK(true, 0);
            dijit.byId("proPessoa").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaEscolaFK() {
    try {
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEscolasFK() {

    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function () {
                try {
                   
                    var grid = dijit.byId("gridPesquisaPessoa");
                    var listaEmpresasGrid = getEmpresasGridUsuario();
                    
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/empresa/findAllEmpresaByLoginPag?&cd_usuario=" + dojo.byId("cod_usuario").value
                                + "&nome=" + dojo.byId("_nomePessoaFK").value + "&fantasia=" + dojo.byId("_apelido").value + "&cnpj=" + dojo.byId("CnpjCpf").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked ,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token(), "Range": "items=0-24" },
                            
                        }), Memory({}));
                    /*
                    myStore.put({
                        cd_usuario: dojo.byId("cod_usuario").value,
	                    nome: dojo.byId("_nomePessoaFK").value,
	                    fantasia: dojo.byId("_apelido").value,
	                    cnpj: dojo.byId("CnpjCpf").value,
	                    inicio: document.getElementById("inicioPessoaFK").checked
                    }).then(function (result) {
                        dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: result }) });
                        grid = dijit.byId("gridPesquisaPessoa");
                        grid.isServerSide = true;
                        grid.setStore(dataStore);
                        dijit.byId("gridPesquisaPessoa").pagination.plugin.prevPage = prevPageTeste;

                        //dataStore = new ObjectStore({ objectStore: result });
                    })*/

                    dataStore = new ObjectStore({ objectStore: myStore });
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false)

                   
                }
                catch (e) {
                    postGerarLog(e);
                }
            })
        });
}


function retornarPessoaEscola() {
    try {
	    showCarregando();
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("formulario").open) {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'dc_reduzido_pessoa');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "dc_reduzido_pessoa", {
                        cd_pessoa: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa,
                        Grupos: value.Grupos
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }
        
        if (!valido) {
	        showCarregando();
	        return false;

        }
            

         loadGridGrupo();

        dijit.byId("proPessoa").hide();
        showCarregando();
    }
    catch (e) {
	    showCarregando();
        postGerarLog(e);
    }
}

function getEmpresasGridUsuario() {
    var listaEmpresasGrid = "";

    if (dijit.byId("gridEscolas") != null && dijit.byId("gridEscolas")['store'] != null && hasValue(dijit.byId("gridEscolas").store.objectStore.data))
        $.each(dijit.byId("gridEscolas").store.objectStore.data, function (index, value) {
            listaEmpresasGrid += value.cd_pessoa + ",";
        });
    return listaEmpresasGrid;
}

function gerarEscolas() {
    try{
        var arrayEscolas = "";
        if (hasValue(dijit.byId("gridUsuario").itemSelecionado) && hasValue(dijit.byId("gridUsuario").itemSelecionado.Empresas))
            $.each(dijit.byId("gridUsuario").itemSelecionado.Empresas, function (index, value) {
                arrayEscolas = arrayEscolas + value.cd_pessoa + ",";
            });
        arrayEscolas = arrayEscolas.substring(0, arrayEscolas.length - 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPessoaUsuario() {
    try{
        dojo.byId('cdPessoa').value = 0;
        dojo.byId("noPessoa").value = "";
        dojo.byId("cpf").value = "";
        dojo.byId("sexo").value = "";
        dijit.byId('noPessoa').set("disabled", false);
        dijit.byId('cpf').set("disabled", false);
        dijit.byId('sexo').set("disabled", false);
        dijit.byId('limparPessoaRelac').set("disabled", true);
        dijit.byId("email").set('value', "");
        dijit.byId('email').set("disabled", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos aba horários

function loadHorarioUsuario(cdUser) {
    showCarregando();
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/escola/geHorarioByForUsuario?cdUser=" + parseInt(cdUser),
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataHorario) {
        try{
            var menorHorario = null;
            var menorData = null;

            if (hasValue(dataHorario.retorno) && dataHorario.retorno.length > 0) {
                $.each(dataHorario.retorno, function (idx, val) {
                    //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
                    val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                    val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });

                    //Pega o menor horário:
                    if (menorData == null || val.dt_hora_ini < menorHorario) {
                        menorData = val.startTime;
                        menorHorario = val.dt_hora_ini;
                    }
                });
                dijit.byId("gridUsuario").horarioUsuario = dataHorario.retorno.slice();

                //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                setTimeout(function () { posicionaPrimeiraLinhaHorarioUsuario(dataHorario.retorno.length, menorData); }, 100);
            }
            criacaoGradeHorario(dataHorario.retorno);
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemUsuario", error);
        showCarregando();
    });
}

function posicionaPrimeiraLinhaHorarioUsuario(tamanhoHorarios, menorData) {
    try{
        var gridHorarios = dijit.byId('gridHorarios');
        try{
            if (hasValue(gridHorarios) && hasValue(gridHorarios.items) && gridHorarios.items.length >= tamanhoHorarios)
                gridHorarios.columnView.set("startTimeOfDay", { hours: menorData.getHours(), duration: 500 });
            else
                setTimeout(function () { posicionaPrimeiraLinhaHorarioUsuario(tamanhoHorarios, menorData); }, 100);
        }
        catch (e) {
            setTimeout(function () { posicionaPrimeiraLinhaHorarioUsuario(tamanhoHorarios, menorData); }, 100);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaDiasSemana(registry, ItemFileReadStore) {
    try{
        var w = registry.byId("cbDias");
        var retorno = [];

        retorno.push({ value_dia: 0 + "", no_dia: getDiaSemana(0) });
        retorno.push({ value_dia: 1 + "", no_dia: getDiaSemana(1) });
        retorno.push({ value_dia: 2 + "", no_dia: getDiaSemana(2) });
        retorno.push({ value_dia: 3 + "", no_dia: getDiaSemana(3) });
        retorno.push({ value_dia: 4 + "", no_dia: getDiaSemana(4) });
        retorno.push({ value_dia: 5 + "", no_dia: getDiaSemana(5) });
        retorno.push({ value_dia: 6 + "", no_dia: getDiaSemana(6) });

        var store = new ItemFileReadStore({
            data: {
                identifier: "value_dia",
                label: "no_dia",
                items: retorno
            }
        });
        w.setStore(store, []);
        console.log(w.get("value_dia"));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridHorario() {
    try{
        if (hasValue(dijit.byId("gridHorarios"))) {
            dijit.byId("gridHorarios").destroyRecursive();
            $('<div>').attr('id', 'gridHorarios').appendTo('#paiGridHorarios');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarLegenda() {
    dojo.ready(function () {
        try{
            var chart = new dojox.charting.Chart("chart");
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legend");

            chart.addSeries("Horário de acesso ao sistema", [1], { fill: "#6ec2fd" }); //BlueRow
            //chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            chart.render();
            dijit.byId("legend").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function criacaoGradeHorario(dataHorario) {
    require([ "dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr"],
    function (ready, Calendar, Observable, Memory, declare, on, xhr) {
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
                            minI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(3, 5))
                            minI = minI == 0 ? "00" : minI;
                        }
                        if (hasValue(getHorarioFuncEscola.retorno.hr_final)) {
                            horaF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(0, 2));
                            minF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(3, 5))
                            minF = minF == 0 ? "00" : minF;
                        }
                    }
                    dijit.byId("timeIni").constraints.min.setMinutes(minI);
                    dijit.byId("timeIni").constraints.min.setHours(horaI);
                    dijit.byId("timeIni").constraints.max.setHours(horaF);
                    dijit.byId("timeFim").constraints.max.setMinutes(minF);
                    dijit.byId("timeFim").constraints.min.setHours(horaI);
                    dijit.byId("timeFim").constraints.max.setHours(horaF);

                    dijit.byId("timeIni").set("value", "T0" + horaI + ":" + minI);
                    dijit.byId("timeFim").set("value", "T" + horaF + ":" + minF);
                    horaF = minF > 1 ? horaF + 1 : horaF;
                    var ECalendar = declare("extended.Calendar", Calendar, {

                        isItemResizeEnabled: function (item, rendererKind) {
                            return false || (hasValue(item) && item.cssClass == 'Calendar1');
                        },

                        isItemMoveEnabled: function (item, rendererKind) {
                            return false || (hasValue(item) && item.cssClass == 'Calendar1');
                        }
                    });
                    var dataHoje = new Date(2012, 1, 7, 00, 00, 00);
                    //dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                    //var horarioD = null;
                    //if (dataHorario != null && hasValue(dataHorario) && dataHorario.length > 0) {
                    //    horarioD = getPrimeiroHorarioDisponivelAluno(dataHorario);
                    //    //calendar.columnView.set("startTimeOfDay", { hours: horarioD.startTime.getHours(), duration: 500 });
                    //}
                    if (hasValue(dijit.byId("gridHorarios")))
                        destroyCreateGridHorario();
                    var calendar = new ECalendar({
                        date: dojo.date.locale.parse("01/07/2012", { locale: 'pt-br', formatLength: "short", selector: "date" }),
                        cssClassFunc: function (item) {
                            return item.calendar;
                        },
                        store: new Observable(new Memory({ data: dataHorario })),
                        dateInterval: "week",
                        columnViewProps: { minHours: horaI, maxHours: horaF, timeSlotDuration: 5, hourSize: 50 },
                        style: "position:relative;max-width:650px;width:100%;height:295px"
                    }, "gridHorarios");
                    calendar.startup();


                    // Configura o calendario para poder excluir o item ao selecionar:

                    calendar.on("itemeditbegin", function (item) {
                        addItemHorariosEdit(item.item.id, calendar);
                    });
                    calendar.on("itemRollOver", function (e) {
                        try{
                            var minutosEnd = e.item.endTime.getMinutes() > 9 ? e.item.endTime.getMinutes() : "0" + e.item.endTime.getMinutes();
                            var horasEnd = e.item.endTime.getHours() > 9 ? e.item.endTime.getHours() : "0" + e.item.endTime.getHours();
                            var horarioEnd = horasEnd + ":" + minutosEnd;
                            var minutosStart = e.item.startTime.getMinutes() > 9 ? e.item.startTime.getMinutes() : "0" + e.item.startTime.getMinutes();
                            var horasStart = e.item.startTime.getHours() > 9 ? e.item.startTime.getHours() : "0" + e.item.startTime.getHours();
                            var horarioStart = horasStart + ":" + minutosStart;

                            if (e.item.calendar == "Calendar1")
                                dojo.attr(e.renderer.domNode.id, "title", "Horário de acesso ao sistema : " + horarioStart + " as " + horarioEnd);

                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                        //  dojo.byId('mostrarLabelHorario').value = true;
                    });


                    // Configura o calendário para poder incluir um item no click da grade:
                    var createItem = function (view, d, e) {
                        try{
                            // create item by maintaining control key
                            //if(!e.ctrlKey ||e.shiftKey || e.altKey){ //Usado para criar o item no componente somente com o click do mouse + a tecla <ctrl> pressionada.
                            if (e.shiftKey || e.altKey) {
                                return;
                            }

                            var dates = {
                                convert: function (dto) {
                                    return (
                                        dto.constructor === Date ? dto :
                                        dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                                        dto.constructor === Number ? new Date(dto) :
                                        dto.constructor === String ? new Date(dto) :
                                        typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                                        NaN
                                    );
                                },
                                compare: function (a, b) {
                                    return (
                                        isFinite(a = this.convert(a).valueOf()) &&
                                        isFinite(b = this.convert(b).valueOf()) ?
                                        (a > b) - (a < b) :
                                        NaN
                                    );
                                },
                                inRange: function (dto, start, end) {
                                    return (
                                         isFinite(dto = this.convert(dto).valueOf()) &&
                                         isFinite(start = this.convert(start).valueOf()) &&
                                         isFinite(end = this.convert(end).valueOf()) ?
                                         start <= dto && dto <= end :
                                         NaN
                                     );
                                },
                                interception: function (start1, end1, start2, end2) {
                                    return (
                                         isFinite(a1 = this.convert(start1).valueOf()) &&
                                         isFinite(a2 = this.convert(end1).valueOf()) &&
                                         isFinite(b1 = this.convert(start2).valueOf()) &&
                                         isFinite(b2 = this.convert(end2).valueOf()) ?
                                         ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                                         NaN
                                     );
                                },
                                include: function (start1, end1, start2, end2) {
                                    return (
                                         isFinite(a1 = this.convert(start1).valueOf()) &&
                                         isFinite(a2 = this.convert(end1).valueOf()) &&
                                         isFinite(b1 = this.convert(start2).valueOf()) &&
                                         isFinite(b2 = this.convert(end2).valueOf()) ?
                                         ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                                         NaN
                                     );
                                }
                            }

                            //var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                            var start, end;
                            var colView = calendar.columnView;
                            var cal = calendar.dateModule;
                            var gradeHorarios = dijit.byId('gridHorarios');
                            var itens = dijit.byId('gridHorarios').items;
                            var id = gradeHorarios.items.length;
                            var item = null;

                            if (id > 0) {
                                quickSortObj(gradeHorarios.items, 'id');
                                id = gradeHorarios.items[id - 1].id + 1;
                            }

                            if (view == colView) {
                                start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                                //start = d;
                                end = cal.add(start, "minute", colView.timeSlotDuration);
                                //end = new Date(d.getTime() + 15*60000);
                            }
                            if (start != null) {
                                var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                                var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                                var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                                var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                                var inicioDia = start.getDate();
                                if (itens.length == 0) {
                                    item = {
                                        id: id,
                                        cd_horario: 0,
                                        summary: "",
                                        dt_hora_ini: hIni + ":" + mIni + ":00",
                                        dt_hora_fim: hFim + ":" + mFim + ":00",
                                        startTime: start,
                                        endTime: end,
                                        calendar: "Calendar1",
                                        id_dia_semana: start.getDate(),
                                        id_disponivel: true
                                    };

                                } else {

                                    for (var j = itens.length - 1; j >= 0; j--) {
                                        //if (itens[j].id != evento.item.id) {
                                        //calendar.store.remove(itens[j].id);

                                        // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                                        if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                                            // Verifica se um item inclui totalmente o outro item e remove o incluído:
                                            var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                                            if (included == 1) {
                                                resolveuIntersecao = true;
                                                return false;
                                            }
                                            else
                                                if (included == 2)
                                                    calendar.store.remove(itens[j].id);

                                                    // Caso contrário, junta um item com o outro:
                                                else
                                                    if (included != NaN) {
                                                        if (dates.compare(start, itens[j].startTime) == 1)
                                                            start = itens[j].startTime;
                                                        else
                                                            end = itens[j].endTime;
                                                        calendar.store.remove(itens[j].id);

                                                    }
                                        }
                                    }

                                    item = {
                                        id: id,
                                        cd_horario: 0,
                                        summary: "",
                                        dt_hora_ini: hIni + ":" + mIni + ":00",
                                        dt_hora_fim: hFim + ":" + mFim + ":00",
                                        startTime: start,
                                        endTime: end,
                                        calendar: "Calendar1",
                                        id_dia_semana: start.getDate(),
                                        id_disponivel: true
                                    };

                                }
                                setTimeout("removeHorarioEditado(" + item.id + ")", 1500);
                                return item;
                            }
                            return null;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }

                    calendar.set("createOnGridClick", true);
                    calendar.set("createItemFunc", createItem);
                    //calendar.set("ItemEditEnd", ItemEditEnd);

                    dijit.byId("gridHorarios").on("ItemEditEnd", function (evt) {

                        setTimeout('mergeItemGridHorarios("' + evt.item.id + '", "' + evt.item.startTime + '", "' + evt.item.endTime + '", "' + evt.item.cd_horario + '")', 100);
                    });

                    var toolBar = document.getElementById('dijit_Toolbar_0');
                    $(".buttonContainer").css("display", "none");
                    $(".viewContainer").css("top", "5px");
                    if (hasValue(toolBar))
                        toolBar.style.display = 'none';
                    showCarregando();
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            },
             function (error) {
                 apresentaMensagem("apresentadorMensagemUsuario", error);
                 showCarregando();
             });
        });
    });

}

function mergeItemGridHorarios(id, startTime, endTime, cd_horario) {
    try{
        var calendar = dijit.byId('gridHorarios');
        var arraySemana = [];
        var itens = dijit.byId('gridHorarios').items;

        //Verificando se não está tirando horário que está ocupado
        var inicioHoras = (new Date(startTime)).getHours();
        var inicioMinutos = (new Date(startTime)).getMinutes();
        var fimHoras = (new Date(endTime)).getHours();
        var fimMinutos = (new Date(endTime)).getMinutes();
        var diaRegistro = (new Date(startTime));
        var inicioDia = diaRegistro.getDate();
        var diaRegistroFim = (new Date(endTime)).getDate();
        var ok = true;

        var endDia = new Date(endTime);

        var fimDia = endDia.getDate();
        //var diaRegistro = diaRegistro.getDate();

        if (inicioDia != fimDia) {
            calendar.store.remove(parseInt(id));
            return false;
        }
        var idOriginal = id;
        $.each(itens, function (idx, value) {
            if (value.id == parseInt(id)) {
                calendar.store.remove(value.id);
                return false;
            }
        });
        //if (hasValue(itens) && itens.length > 0) {
        //    itens = jQuery.grep(itens, function (value) {
        //        return value.startTime.getDate() == diaRegistro && value.id != parseInt(id);
        //    });

        //}


        var dates = {
            convert: function (dto) {
                return (
                    dto.constructor === Date ? dto :
                    dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                    dto.constructor === Number ? new Date(dto) :
                    dto.constructor === String ? new Date(dto) :
                    typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                    NaN
                );
            },
            compare: function (a, b) {
                return (
                    isFinite(a = this.convert(a).valueOf()) &&
                    isFinite(b = this.convert(b).valueOf()) ?
                    (a > b) - (a < b) :
                    NaN
                );
            },
            inRange: function (dto, start, end) {
                return (
                     isFinite(dto = this.convert(dto).valueOf()) &&
                     isFinite(start = this.convert(start).valueOf()) &&
                     isFinite(end = this.convert(end).valueOf()) ?
                     start <= dto && dto <= end :
                     NaN
                 );
            },
            interception: function (start1, end1, start2, end2) {
                return (
                     isFinite(a1 = this.convert(start1).valueOf()) &&
                     isFinite(a2 = this.convert(end1).valueOf()) &&
                     isFinite(b1 = this.convert(start2).valueOf()) &&
                     isFinite(b2 = this.convert(end2).valueOf()) ?
                     ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                     NaN
                 );
            },
            include: function (start1, end1, start2, end2) {
                return (
                     isFinite(a1 = this.convert(start1).valueOf()) &&
                     isFinite(a2 = this.convert(end1).valueOf()) &&
                     isFinite(b1 = this.convert(start2).valueOf()) &&
                     isFinite(b2 = this.convert(end2).valueOf()) ?
                     ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                     NaN
                 );
            }
        }
        var d;
        var start, end;
        var colView = calendar.columnView;
        var cal = calendar.dateModule;
        var gradeHorarios = dijit.byId('gridHorarios');
        var resolveuIntersecao = false;

        start = new Date(startTime);
        end = new Date(endTime);
        d = new Date(endTime);

        /****Verificar se existe interseção com um ou mais horários disponíveis:****/
        for (var j = itens.length - 1; j >= 0; j--) {
            //if (itens[j].id != evento.item.id) {
            //calendar.store.remove(itens[j].id);

            // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
            if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                // Verifica se um item inclui totalmente o outro item e remove o incluído:
                var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);
                $.each(calendar.store.data, function (idx, value) {
                    if (value.id == itens[j].id)
                        cd_horario = value.cd_horario;
                });
                if (included == 1)
                    resolveuIntersecao = true;
                else
                    if (included == 2)
                        calendar.store.remove(itens[j].id);

                        // Caso contrário, junta um item com o outro:
                    else
                        if (included != NaN) {
                            if (dates.compare(start, itens[j].startTime) == 1)
                                start = itens[j].startTime;
                            else
                                d = itens[j].endTime;
                            calendar.store.remove(itens[j].id);

                        }
            }
        }


        var id = gradeHorarios.items.length;

        if (id > 0) {
            quickSortObj(gradeHorarios.items, 'id');
            id = gradeHorarios.items[id - 1].id + 1;
        }

        if (!resolveuIntersecao) {
            if (start != null) {
                var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                var item = {
                    id: id,
                    cd_horario: parseInt(cd_horario),
                    summary: "",
                    startTime: start,
                    calendar: "Calendar1",
                    endTime: d,
                    dt_hora_ini: hIni + ":" + mIni + ":00",
                    dt_hora_fim: hFim + ":" + mFim + ":00",
                    id_dia_semana: start.getDate(),
                    id_disponivel: true
                };

                if (item != null)
                    calendar.store.add(item);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluiItemHorario() {
    try{
        var gradeHorarios = dijit.byId('gridHorarios');
        var store = gradeHorarios.items;

        if ((store.length > 0) && hasValue(gradeHorarios.selectedItems)) {
            apresentaMensagem('apresentadorMensagemUsuario', "");
            for (var i = gradeHorarios.selectedItems.length - 1; i >= 0; i--) {
                var itemSelected = gradeHorarios.selectedItems[i];

                if (itemSelected.calendar != "Calendar2")
                    gradeHorarios.store.remove(itemSelected.id);
            }
        }
        else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDeleteHorarioSeleciona);
            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            return false
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemHorario() {
    try {
        apresentaMensagem('apresentadorMensagemUsuario', null);
        var mensagensWeb = new Array();
        var calendar = dijit.byId('gridHorarios');
        var timeFim = dijit.byId('timeFim');
        var timeIni = dijit.byId('timeIni');
        var arraySemana = dijit.byId('cbDias').value;
        if (!hasValue(calendar)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNaoConstruidoGradeHorario);
            apresentaMensagem(msg, mensagensWeb);
            return false
        }
        var itens = dijit.byId('gridHorarios').items;
        if (arraySemana.length <= 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
            apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
            return false
        }
        else
            //if (timeIni.validate() && timeIni.validate()) {
            if (dijit.byId("formSemana").validate()) {
                if ((timeFim.value.getMinutes() % 5 != 0 || timeIni.value.getMinutes() % 5 != 0) || validarItemHorarioManual(timeIni, timeFim)) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotInclItemHorario);
                    apresentaMensagem('apresentadorMensagemUsuario', mensagensWeb);
                    return false;
                }
                var dates = {
                    convert: function (dto) {
                        // Converts the date in dto to a date-object. The input can be:
                        //   a date object: returned without modification
                        //  an array      : Interpreted as [year,month,day]. NOTE: month is 0-11.
                        //   a number     : Interpreted as number of milliseconds
                        //                  since 1 Jan 1970 (a timestamp)
                        //   a string     : Any format supported by the javascript engine, like
                        //                  "YYYY/MM/DD", "MM/DD/YYYY", "Jan 31 2009" etc.
                        //  an object     : Interpreted as an object with year, month and date
                        //                  attributes.  **NOTE** month is 0-11.
                        return (
                            dto.constructor === Date ? dto :
                            dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                            dto.constructor === Number ? new Date(dto) :
                            dto.constructor === String ? new Date(dto) :
                            typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                            NaN
                        );
                    },
                    compare: function (a, b) {
                        // Compare two dates (could be of any type supported by the convert
                        // function above) and returns:
                        //  -1 : if a < b
                        //   0 : if a = b
                        //   1 : if a > b
                        // NaN : if a or b is an illegal date
                        // NOTE: The code inside isFinite does an assignment (=).
                        return (
                            isFinite(a = this.convert(a).valueOf()) &&
                            isFinite(b = this.convert(b).valueOf()) ?
                            (a > b) - (a < b) :
                            NaN
                        );
                    },
                    inRange: function (dto, start, end) {
                        // Checks if date in dto is between dates in start and end.
                        // Returns a boolean or NaN:
                        //    true  : if dto is between start and end (inclusive)
                        //    false : if dto is before start or after end
                        //    NaN   : if one or more of the dates is illegal.
                        // NOTE: The code inside isFinite does an assignment (=).
                        return (
                             isFinite(dto = this.convert(dto).valueOf()) &&
                             isFinite(start = this.convert(start).valueOf()) &&
                             isFinite(end = this.convert(end).valueOf()) ?
                             start <= dto && dto <= end :
                             NaN
                         );
                    },
                    interception: function (start1, end1, start2, end2) {
                        return (
                             isFinite(a1 = this.convert(start1).valueOf()) &&
                             isFinite(a2 = this.convert(end1).valueOf()) &&
                             isFinite(b1 = this.convert(start2).valueOf()) &&
                             isFinite(b2 = this.convert(end2).valueOf()) ?
                             ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                             NaN
                         );
                    },
                    include: function (start1, end1, start2, end2) {
                        return (
                             isFinite(a1 = this.convert(start1).valueOf()) &&
                             isFinite(a2 = this.convert(end1).valueOf()) &&
                             isFinite(b1 = this.convert(start2).valueOf()) &&
                             isFinite(b2 = this.convert(end2).valueOf()) ?
                             ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                             NaN
                         );
                    }
                }

                for (var i = 0; i < arraySemana.length; i++) {
                    var diaSemana = parseInt(arraySemana[i]) + 1;
                    var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                    var start, end;
                    var colView = calendar.columnView;
                    var cal = calendar.dateModule;
                    var gradeHorarios = dijit.byId('gridHorarios');
                    var resolveuIntersecao = false;

                    start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                    d = new Date(2012, 6, diaSemana, timeFim.value.getHours(), timeFim.value.getMinutes());

                    /****Verificar se existe interseção com um ou mais horários disponíveis:****/
                    for (var j = itens.length - 1; j >= 0; j--) {
                        // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                        if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                            // Verifica se um item inclui totalmente o outro item e remove o incluído:
                            var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                            if (included == 1)
                                resolveuIntersecao = true;
                            else
                                if (included == 2)
                                    calendar.store.remove(itens[j].id);

                                    // Caso contrário, junta um item com o outro:
                                else
                                    if (included != NaN) {
                                        if (dates.compare(start, itens[j].startTime) == 1)
                                            start = itens[j].startTime;
                                        else
                                            d = itens[j].endTime;
                                        calendar.store.remove(itens[j].id);
                                    }
                        }
                    }

                    var id = gradeHorarios.items.length;

                    if (id > 0) {
                        quickSortObj(gradeHorarios.items, 'id');
                        id = gradeHorarios.items[id - 1].id + 1;
                    }

                    if (!resolveuIntersecao) {
                        if (start != null) {
                            var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                            var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                            var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                            var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                            var item = {
                                id: id,
                                cd_horario: 0,
                                summary: "",
                                dt_hora_ini: hIni + ":" + mIni + ":00",
                                dt_hora_fim: hFim + ":" + mFim + ":00",
                                startTime: start,
                                calendar: "Calendar1",
                                endTime: d,
                                id_dia_semana: start.getDate(),
                                id_disponivel: true
                            };

                            calendar.store.add(item);
                            colView.set("startTimeOfDay", { hours: hIni, duration: 500 });
                        }
                    }
                }

            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function removeHorarioEditado(id) {
    try{
        var gridHorarios = dijit.byId("gridHorarios");

        if (!hasValue(gridHorarios.novosItensEditados) || !binarySearch(gridHorarios.novosItensEditados, id))
            gridHorarios.store.remove(parseInt(id));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function addItemHorariosEdit(id, calendar) {
    try{
        var novosItensEditados = hasValue(calendar.novosItensEditados) ? calendar.novosItensEditados : new Array();

        //novosItensEditados.push(item.item.id);
        novosItensEditados = insertSort(novosItensEditados, id, false);
        calendar.novosItensEditados = novosItensEditados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarItemHorarioManual(timeIni, timeFim) {
    try {
        var retorno = false;
        var hIni = timeIni.value.getHours();
        var hFim = timeFim.value.getHours();
        var mIni = timeIni.value.getMinutes();
        var mFim = timeFim.value.getMinutes();

        var totalMinitosInicial = (parseInt(hIni) * 60) + parseInt(mIni);
        var totalMinutosFinal = (parseInt(hFim) * 60) + parseInt(mFim);
        if (totalMinutosFinal - totalMinitosInicial <= 5)
            retorno = true;
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverEscola(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () {                                
                removerEscolasGrid(itensSelecionados);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function removerEscolasGrid(escolasSelecionadas) {
    require(["dojox/grid/EnhancedGrid", "dojo/data/ObjectStore", "dojo/store/Memory", "dojo/query"
    ], function (EnhancedGrid, ObjectStore, Memory, query) {
        var gridEscolas = dijit.byId("gridEscolas");
        var listaEscolas = gridEscolas.store.objectStore.data;

        if (escolasSelecionadas != null && escolasSelecionadas.length > 0) {
            $.each(escolasSelecionadas, function (idx, escola) {
                removeObjSort(listaEscolas, "cd_pessoa", escola.cd_pessoa);
            });
            
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: listaEscolas }) });
            gridEscolas.setStore(dataStore);
            gridEscolas.startup();
            loadGridGrupo();
        }
        
    });
}