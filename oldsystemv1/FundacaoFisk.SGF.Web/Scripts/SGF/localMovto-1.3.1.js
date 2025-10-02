var CARTEIRA = 1, BANCO = 2, CAIXA = 3, CARTAO_CREDITO = 4, CARTAO_DEBITO = 5;
var FILTRAR_ESCOLA = 1;

function toglleTagConjunta(value) {
    try{
        if (value == true)
            dojo.byId('tConjunta').style.display = 'block';
        else
            dojo.byId('tConjunta').style.display = 'none';
    } catch (e) {
        postGerarLog(e);
    }

}

function formatCheckBoxLocalMovto(value, rowIndex, obj) {
    try{
        var gridName = 'gridLocalMovto';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosLocalMovto');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_local_movto", grid._by_idx[rowIndex].item.cd_local_movto);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_local_movto', 'selecionadoLocalMovto', -1, 'selecionaTodosLocalMovto', 'selecionaTodosLocalMovto', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_local_movto', 'selecionadoLocalMovto', " + rowIndex + ", '" + id + "', 'selecionaTodosLocalMovto', '" + gridName + "')", 2);


        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTaxaBancaria(value, rowIndex, obj) {
    try {
        var gridName = 'gridTaxa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasTaxas');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_taxa_bancaria", grid._by_idx[rowIndex].item.cd_taxa_bancaria);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_taxa_bancaria', 'selecionadoTaxaBancaria', -1, 'selecionaTodasTaxas', 'selecionaTodasTaxas', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_taxa_bancaria', 'selecionadoTaxaBancaria', " + rowIndex + ", '" + id + "', 'selecionaTodasTaxas', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region montarCadastroLocalMovto

function montarCadastroLocalMovto() {

    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dojo/dom",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dijit/form/DateTextBox"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, dom, DropDownButton, DropDownMenu, MenuItem, DateTextBox) {
        ready(function () {
            try{
                //*** Cria a grade de LocalMovtos **\\
                populaBanco(0, 'cd_banco');
                getAllLocalMovtoTipoBanco();
                $("#dc_cpf_pessoa_conjunta").mask("999.999.999-99");
                var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getLocalMovtoSearch?nome=&nmBanco=&inicio=false&status=1&tipo=0&pessoa=",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_local_movto"
                }
                    ), Memory({ idProperty: "cd_local_movto" }));
                var gridLocalMovto = new EnhancedGrid({
                    store: ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosLocalMovto' style='display:none'/>", field: "selecionadoLocalMovto", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxLocalMovto },
                        { name: "Tipo", field: "desc_tipo_local", width: "15%", styles: "min-width:80px;" },
                        { name: "Nome", field: "no_local_movto", width: "40%", styles: "min-width:80px;" },
                        { name: "Banco", field: "nm_banco", width: "8%", styles: "text-align: center; min-width:80px;" },
                        { name: "Agência", field: "nm_agencia", width: "8%", styles: "min-width:80px;" },
                        { name: "Dig", field: "nm_digito_agencia", width: "4%", styles: "text-align: center; min-width:80px;" },
                        { name: "C/C", field: "nm_conta_corrente", width: "8%", styles: "text-align: center; min-width:80px;" },
                        { name: "Dig", field: "nm_digito_conta_corrente", width: "4%", styles: "text-align: center; min-width:80px;" },
                        { name: "Pessoa", field: "no_pessoa_local", width: "35%", styles: "min-width:80px;" },
                        { name: "Ativo", field: "local_ativo", width: "6%", styles: "text-align: center; min-width:15px; max-width: 20px;"},
                        { name: "C.Conj.", field: "conta_conjunta", width: "7%", styles: "text-align: center; min-width:15px; max-width: 20px;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEncFiltro,
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridLocalMovto");
                gridLocalMovto.pagination.plugin._paginator.plugin.connect(gridLocalMovto.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridLocalMovto, 'cd_local_movto', 'selecionaTodosLocalMovto');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridLocalMovto, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosLocalMovto').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_local_movto', 'selecionadoLocalMovto', -1, 'selecionaTodosLocalMovto', 'selecionaTodosLocalMovto', 'gridLocalMovto')", gridLocalMovto.rowsPerPage * 3);
                    });
                });
                gridLocalMovto.canSort = function (col) { return Math.abs(col) != 1 };

                gridLocalMovto.startup();
                gridLocalMovto.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;

                        apresentaMensagem('apresentadorMensagem', '');
                        gridLocalMovto.itemSelecionado = item;
                        keepValuesLocalMovto(gridLocalMovto.itemSelecionado, null, xhr);
                        //TODO VERIFICAR DEPOIS
                        IncluirAlterar(0, 'divAlterarLocalMovto', 'divIncluirLocalMovto', 'divExcluirLocalMovto', 'apresentadorMensagemLocalMovto', 'divCancelarLocalMovto', 'divLimparLocalMovto');
                        dijit.byId("cadLocalMovto").show();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem("apresentadorMensagemLocalMovto", error);
                });

                dojo.byId('tConjunta').style.display = 'none';
                dijit.byId('cd_pessoa_local').setAttribute('disabled', true);

                dojo.byId('tBanco').style.display = 'none';
                dojo.byId('tCnab').style.display = 'none';

                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridLocalMovto, 'todosItensLocalMovto', ['pesquisarLocalMovto', 'relatorioLocalMovto']); PesquisarLocalMovto(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridLocalMovto', 'selecionadoLocalMovto', 'cd_local_movto', 'selecionaTodosLocalMovto', ['pesquisarLocalMovto', 'relatorioLocalMovto'], 'todosItensLocalMovto'); }
                });
                menu.addChild(menuItensSelecionados);


                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensLocalMovto",
                    dropDown: menu,
                    id: "todosItensLocalMovto"
                });
                dom.byId("linkSelecionadosLocalMovto").appendChild(button.domNode);

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridLocalMovto.itensSelecionados, 'DeletarLocalMovto(itensSelecionados)'); }
                });
                menu.addChild(acaoExcluir);

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarLocalMovto(dijit.byId("gridLocalMovto").itensSelecionados, xhr); }
                });
                menu.addChild(acaoEditar);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasLocalMovto",
                    dropDown: menu,
                    id: "acoesRelacionadasLocalMovto"
                });
                dom.byId("linkAcoesLocalMovto").appendChild(button.domNode);

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            limparPesquisaPessoaRelFK();
                            dijit.byId("proPessoaRel").show();
                            if (hasValue(dijit.byId("gridPesquisaPessoaRel")))
                                dijit.byId("gridPesquisaPessoaRel").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                            if (dijit.byId("nm_tipo_local").value == CAIXA) {
                                dijit.byId("tipoPessoaRelFK").set("value", 1);
                                dijit.byId("tipoPessoaRelFK").set("disabled", true);
                                dijit.byId("papelPessoaRelFK").set("value", 0);
                                dijit.byId("papelPessoaRelFK").set("disabled", true);
                            }
                            else {
                                dijit.byId("tipoPessoaRelFK").set("value", 0);
                                dijit.byId("tipoPessoaRelFK").set("disabled", false);
                                dijit.byId("papelPessoaRelFK").set("value", 0);
                                dijit.byId("papelPessoaRelFK").set("disabled", false);
                            }
                            pesquisaPessoaPorEscola();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadPessoaLocal");
                dijit.byId('cadPessoaLocal').setAttribute('disabled', true);
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFK(true, FILTRAR_ESCOLA);
                                    });
                                });
                            else
                                abrirPessoaFK();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadPessoaBco");

                dijit.byId("pesqPessoaRel").on("click", function (e) {
                    pesquisaPessoaPorEscola();
                });
                dijit.byId("dc_cpf_pessoa_conjunta").on("blur", function (evt) {
                    try{
                        apresentaMensagem('apresentadorMensagemProPessoa', null);
                        if (trim(dojo.byId("dc_cpf_pessoa_conjunta").value) != "" && dojo.byId("dc_cpf_pessoa_conjunta").value != "___.___.___-__")
                            validarCPF("#dc_cpf_pessoa_conjunta", "apresentadorMensagemLocalMovto");
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () { IncluirLocalMovto(); }
                }, "incluirLocalMovto");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { AlterarLocalMovto(); }
                }, "alterarLocalMovto");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarLocalMovto();
                        });
                    }
                }, "deleteLocalMovto");
                new Button({
                    label: "Limpar",  iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try{
                            getLimpar('#formLocalMovto');
                            clearForm('formLocalMovto');
                            getLimpar('#formBancoLocalMovto');
                            clearForm('formBancoLocalMovto');
                            clearGridTaxaBancaria();
                            padraoTela();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparLocalMovto");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValuesLocalMovto(null, gridLocalMovto, xhr);
                    }
                }, "cancelarLocalMovto");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("cadLocalMovto").hide(); }
                }, "fecharLocalMovto");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        PesquisarLocalMovto(true);
                    }
                }, "pesquisarLocalMovto");
                decreaseBtn(document.getElementById("pesquisarLocalMovto"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            dijit.byId("cadLocalMovto").show();
                            getLimpar('#formLocalMovto');
                            clearForm('formLocalMovto');
                            getLimpar('#formBancoLocalMovto');
                            clearForm('formBancoLocalMovto');
                            clearGridTaxaBancaria();
                            apresentaMensagem('apresentadorMensagem', null);
                            IncluirAlterar(1, 'divAlterarLocalMovto', 'divIncluirLocalMovto', 'divExcluirLocalMovto', 'apresentadorMensagemLocalMovto', 'divCancelarLocalMovto', 'divLimparLocalMovto');
                            padraoTela();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoLocalMovto");


                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        try{
                            dijit.byId('cd_pessoa_local').reset();
                            dijit.byId("cd_pessoa_local").value =  0;
                            dojo.byId("cd_pessoa_local").value = "";
                            dijit.byId('limparPessoaLocal').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaLocal");
                if (hasValue(document.getElementById("limparPessoaLocal"))) {
                    document.getElementById("limparPessoaLocal").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaLocal").parentNode.style.width = '40px';
                }

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        try{
                            dijit.byId('cd_pessoa_banco').reset();
                            dijit.byId("cd_pessoa_banco").value = 0;
                            dojo.byId("cd_pessoa_banco").value = "";
                            dijit.byId('limparPessoaBco').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaBco");

                if (hasValue(document.getElementById("limparPessoaBco"))) {
                    document.getElementById("limparPessoaBco").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaBco").parentNode.style.width = '40px';
                }

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        xhr.get({
                            url: Endereco() + "/api/financeiro/GetUrlRelatorioLocalMovto?" + getStrGridParameters('gridLocalMovto') + "nome=" + dojo.byId("descLocalMovto").value + "&nmBanco=" + dojo.byId("descBanco").value + "&inicio=" + dojo.byId("inicioLocalMovto").checked + "&status=" + retornaStatus("statusLocalMovto") + "&tipo=" + dijit.byId("tipoLocalMovto").value + "&pessoa=" + dijit.byId("noPessoaLocalMovto").value,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try{
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioLocalMovto");

                var buttonFkArray = ['cadPessoaLocal', 'cadPessoaBco'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                montarStatus("statusLocalMovto");
                montarTipoLocalC("tipoLocalMovto");
                montarTipoLocal("nm_tipo_local");

                dijit.byId("cd_banco").on("change", function (e) {
                    if (e > 0) {
                        montaDadosBanco(xhr);
                    }
                });

                dijit.byId("nm_tipo_local").on("change", function (e) {
                    try{
                        if (e != CARTEIRA && e != CARTAO_CREDITO && e != CARTAO_DEBITO) {
                            dijit.byId('cd_pessoa_local').setAttribute('disabled', false);
                            dijit.byId('cadPessoaLocal').setAttribute('disabled', false);
                            dojo.byId('tTaxa').style.display = 'none';

                            if (e == 2) {
                                //dojo.byId('corpoCad').style.height = '350px';
                                dojo.byId('tBanco').style.display = 'block';
                                dojo.byId('tCnab').style.display = 'block';
                            }                            
                            else {
                                //dojo.byId('corpoCad').style.height = '170px';
                                dojo.byId('tBanco').style.display = 'none';
                                dojo.byId('tCnab').style.display = 'none';
                                dojo.byId('tConjunta').style.display = 'none';                                
                            }
                        } else if (e == CARTAO_CREDITO || e == CARTAO_DEBITO) {

                            dijit.byId('cd_pessoa_local').setAttribute('disabled', true);
                            dijit.byId('cadPessoaLocal').setAttribute('disabled', true);
                            dojo.byId('tTaxa').style.display = 'block';
                            dijit.byId('tTaxa').set('open', true);

                            dojo.byId('tBanco').style.display = 'none';
                            dojo.byId('tCnab').style.display = 'none';
                            dojo.byId('tConjunta').style.display = 'none';
                        }
                        else {
                            //dojo.byId('corpoCad').style.height = '170px';
                            dijit.byId('cd_pessoa_local').setAttribute('disabled', true);
                            dijit.byId('cadPessoaLocal').setAttribute('disabled', true);
                            dojo.byId('tBanco').style.display = 'none';
                            dojo.byId('tCnab').style.display = 'none';
                            dojo.byId('tConjunta').style.display = 'none';
                            dojo.byId('tTaxa').style.display = 'none';
                        }
                        if (e != 2) {
                            getLimpar('#formBancoLocalMovto');
                            clearForm('formBancoLocalMovto');
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }

                });
                adicionarAtalhoPesquisa(['descLocalMovto', 'descBanco', 'statusLocalMovto', 'tipoLocalMovto', 'noPessoaLocalMovto'], 'pesquisarLocalMovto', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323063', '765px', '771px');
                        });
                }

                // Taxas Bancárias
                var gridTaxa = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodasTaxas' style='display:none'/>", field: "selecionadoTaxaBancaria", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTaxaBancaria },
                        { name: "N° Parcelas", field: "nm_parcela", width: "15%", styles: "text-align: center;min-width:80px;" },
                        { name: "Taxa(%)", field: "pc_taxa", width: "15%", styles: "text-align: center;min-width:80px;" },
                        { name: "Prazo(D+)", field: "nm_dias", width: "10%", styles: "text-align: center; min-width:80px;" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridTaxa");
                gridTaxa.canSort = function () { return true };
                gridTaxa.startup();
                gridTaxa.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        keepValuesTaxaBancaria(null, item);
                        IncluirAlterar(0, 'divAlterarTxBancaria', 'divIncluirTxBancaria', 'divExcluirTxBancaria', 'apresentadorMensagemTaxaBancaria', 'divCancelarTxBancaria', 'divLimparTxBancaria');
                        dijit.byId("dialogTaxaBancaria").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                // Crud Taxa Bancaria  
                new Button({
                    label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        incluirTaxaBancaria();
                    }
                }, "incluirTxBancaria");

                new Button({
                    label: "Alterar", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        alterarTaxaBancaria();
                    }
                }, "alterarTxBancaria");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        limparFormTaxaBancaria();
                    }
                }, "limparTxBancaria");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        getTaxaBancariaPorId();
                    }
                }, "cancelarTxBancaria");

                new Button({
                    label: "Fechar",
                    iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogTaxaBancaria").hide();
                    }
                }, "fecharTxBancaria");

                //Botao Incluir Taxa Bancaria:
                var buttonTaxaBancaria = new Button({
                    label: "Incluir",
                    name: "itemTaxaBancaria",
                    iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    id: "itemTaxaBancaria",
                    onClick: function () {
                        IncluirAlterar(1, 'divAlterarTxBancaria', 'divIncluirTxBancaria', 'divExcluirTxBancaria', 'apresentadorMensagemTaxaBancaria', 'divCancelarTxBancaria', 'divLimparTxBancaria');
                        limparFormTaxaBancaria();                       

                        dijit.byId("dialogTaxaBancaria").show();
                    }
                });
                dom.byId("btnAddTaxaBancaria").appendChild(buttonTaxaBancaria.domNode);

                // Adiciona link de ações Incluir Taxa Bancaria:
                var menuTaxaBancaria = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarTaxaBancaria(gridTaxa.itensSelecionados); }
                });
                menuTaxaBancaria.addChild(acaoEditar);

                var acaoRemoverTaxaBancaria = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_taxa_bancaria', gridTaxa);
                    }
                });
                menuTaxaBancaria.addChild(acaoRemoverTaxaBancaria);

                var buttonEscola = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPlano",
                    dropDown: menuTaxaBancaria,
                    id: "acoesRelacionadasTxBanc"
                });
                dom.byId("linkTaxaBancaria").appendChild(buttonEscola.domNode);

            } catch (e) {
                postGerarLog(e);
            }
        })

    });

    showCarregando();
};
//#endregion

function montarTipoLocalC(nomElement, mostraTodos) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try{
             var dados = [
                    { name: "Todos", id: "0" },
                    { name: "Banco", id: "2" },
                    { name: "Caixa", id: "3" },
                    { name: "Carteira", id: "1" },
                    { name: "Cartão de Crédito", id: "4" },
                    { name: "Cartão de Débito", id: "5" }


             ]
             var statusStore = new Memory({
                 data: dados
             });
             //ready(function () {
             var status = new filteringSelect({
                 id: nomElement,
                 name: "tipoLocalMovto",
                 value: "0",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 100%;"
             }, nomElement);
         } catch (e) {
             postGerarLog(e);
         }
     });
};

function montarTipoLocal(nomElement, mostraTodos) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try{
             var dados = [
                    { name: "Banco", id: "2" },
                    { name: "Caixa", id: "3" },
                    { name: "Carteira", id: "1" },
                    { name: "Cartão de Crédito", id: "4" },
                    { name: "Cartão de Débito", id: "5" }

             ]
             var statusStore = new Memory({
                 data: dados
             });
             //ready(function () {
             var status = new filteringSelect({
                 id: nomElement,
                 name: "nm_tipo_local",
             value: "1",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 100%;"
             }, nomElement);
			dijit.byId(nomElement).set("riquered", true);
         } catch (e) {
             postGerarLog(e);
         }
     });
};

function PesquisarLocalMovto(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getLocalMovtoSearch?nome=" + dojo.byId("descLocalMovto").value + "&nmBanco=" + dojo.byId("descBanco").value + "&inicio=" + dojo.byId("inicioLocalMovto").checked + "&status=" + retornaStatus("statusLocalMovto") + "&tipo=" + dijit.byId("tipoLocalMovto").value + "&pessoa=" + dijit.byId("noPessoaLocalMovto").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_local_movto"
                }
                    ), Memory({ idProperty: "cd_local_movto" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridLocalMovto = dijit.byId("gridLocalMovto");
            if (limparItens) {
                gridLocalMovto.itensSelecionados = [];
            }
            gridLocalMovto.noDataMessage = msgNotRegEnc;
            gridLocalMovto.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//TODO Karoline
function retornarPessoa() {
    try{
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
            dijit.byId("cd_pessoa_banco").value = gridPessoaSelec.itensSelecionados[0].cd_pessoa;
            dojo.byId("cd_pessoa_banco").value = gridPessoaSelec.itensSelecionados[0].no_pessoa;
            dijit.byId('limparPessoaBco').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaPorEscola() {
        require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"],
        function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                if (dijit.byId("nm_tipo_local").value == CAIXA)
                    var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaUsuarioSearch?nome=" + encodeURIComponent(dojo.byId("_nomePessoaRelFK").value) + "&apelido=" + encodeURIComponent(dojo.byId("_apelidoRel").value) + "&cnpjCpf=" + dojo.byId("CnpjCpfPessoaRel").value + "&sexo=" + parseInt(dijit.byId("sexoPessoaRelFK").value) + "&inicio=false",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));
                else
                    var myStore = Cache(
                         JsonRest({
                             target: Endereco() + "/api/aluno/getPessoaPapelSearchWithCPFCNPJ?nome=" + encodeURIComponent(dojo.byId("_nomePessoaRelFK").value) + "&apelido=" + encodeURIComponent(dojo.byId("_apelidoRel").value) + "&tipoPessoa=" + parseInt(dijit.byId("tipoPessoaRelFK").value) + "&cnpjCpf=" + dojo.byId("CnpjCpfPessoaRel").value + "&papel=" + parseInt(dijit.byId("papelPessoaRelFK").value) + "&sexo=" + parseInt(dijit.byId("sexoPessoaRelFK").value) + "&inicio=false",
                             handleAs: "json",
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoaRel");
                grid.setStore(dataStore);
            } catch (e) {
                postGerarLog(e);
            }
        })
}

function retornarPessoaRel() {
    try{
        var gridPesPessoaRel = dijit.byId("gridPesquisaPessoaRel");
            if (!hasValue(gridPesPessoaRel.itensSelecionados) || gridPesPessoaRel.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                return false;
            }
            else if (gridPesPessoaRel.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
                return false;
            }
        else {
            //TODO Melhorar esse código  HAS_RESPONSAVEL_TIT?? karol  não é muito usual setar na mesma variavél, seria bom criar uma para cada responsabilidade.
            dijit.byId("cd_pessoa_local").value = gridPesPessoaRel.itensSelecionados[0].cd_pessoa;
            dojo.byId("cd_pessoa_local").value = gridPesPessoaRel.itensSelecionados[0].no_pessoa;
            dijit.byId('limparPessoaLocal').set("disabled", false);
        }
        dijit.byId("proPessoaRel").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesLocalMovto(value, grid, xhr) {
    try{
        showCarregando();
        getLimpar('#formLocalMovto');
        clearForm('formLocalMovto');
        getLimpar('#formBancoLocalMovto');
        clearForm('formBancoLocalMovto');
        padraoTela();

        if (!hasValue(value) && grid != null)
            value = grid.itemSelecionado;
        if (value.cd_local_movto > 0)
            xhr.get({
                url: Endereco() + "/api/financeiro/getLocalMovtoById?cdLocalMovto=" + value.cd_local_movto,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    apresentaMensagem("apresentadorMensagemLocalMovto", null);
                    loadDataLocalMovto(jQuery.parseJSON(data).retorno, xhr);

                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                    showCarregando();
                }
            },
        function (error) {
            apresentaMensagem("apresentadorMensagemLocalMovto", error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
        showCarregando();
    }
}

function loadDataLocalMovto(local, xhr) {
    try {
        var gridTaxa = dijit.byId('gridTaxa');

        if (local.cd_banco > 0)
            loadCarteira(xhr, local.cd_banco, local.cd_local_movto, local.cd_carteira_cnab);
        dojo.byId("cd_local_movto").value = local.cd_local_movto;
        dijit.byId("cd_pessoa_local").set("value", local.cd_pessoa_local);
        dojo.byId("cd_pessoa_local").value = local.no_pessoa_local;

        dijit.byId("nm_tipo_local")._onChangeActive = false;
        dijit.byId("nm_tipo_local").set("value", local.nm_tipo_local);
        dijit.byId("nm_tipo_local")._onChangeActive = true;

        dijit.byId("cd_banco")._onChangeActive = false;
        dijit.byId("cd_banco").set("value", local.cd_banco);
        dijit.byId("cd_banco")._onChangeActive = true;

        dijit.byId("cd_pessoa_banco").set("value", local.cd_pessoa_banco);
        dojo.byId("cd_pessoa_banco").value = local.no_pessoa_banco;
        dijit.byId("dc_num_cliente_banco").set("value", local.dc_num_cliente_banco);
        dojo.byId("dc_nosso_numero").value = local.nossoNumero;
        dojo.byId("dc_pessoa_conjunta").value = local.dc_pessoa_conjunta;
        dojo.byId("dc_cpf_pessoa_conjunta").value = local.dc_cpf_pessoa_conjunta;
        dojo.byId("no_local_movto").value = local.no_local_movto;
        dojo.byId("nm_banco").value = local.nm_banco;
        dojo.byId("nm_agencia").value = local.nm_agencia;
        dojo.byId("nm_conta_corrente").value = local.nm_conta_corrente;
        dojo.byId("nm_digito_conta_corrente").value = local.nm_digito_conta_corrente
        dojo.byId("nm_digito_agencia").value = local.nm_digito_agencia;
        dijit.byId("id_local_ativo").set("checked", local.id_local_ativo);
        dijit.byId("id_conta_conjunta").set("checked", local.id_conta_conjunta);
        dojo.byId("dc_digito_cedente").value = local.nm_digito_cedente;
        dojo.byId("nm_operacao_conta_corrente").value = local.nm_op_conta;

        dojo.byId("nm_sequencia").value = local.nm_sequencia;
        dojo.byId("nm_transmissao").value = local.nm_transmissao;

        if (local.cd_pessoa_local)
            dijit.byId('limparPessoaLocal').set("disabled", false);
        else
            dijit.byId('limparPessoaLocal').set("disabled", true);
        if (local.cd_pessoa_banco)
            dijit.byId('limparPessoaBco').set("disabled", false);
        else
            dijit.byId('limparPessoaBco').set("disabled", true);

        toglleTagConjunta(local.id_conta_conjunta);

        if (local.nm_tipo_local != CARTEIRA && local.nm_tipo_local != CARTAO_CREDITO && local.nm_tipo_local != CARTAO_DEBITO) {
            dijit.byId('cd_pessoa_local').setAttribute('disabled', false);
            dijit.byId('cadPessoaLocal').setAttribute('disabled', false);
            dojo.byId('tTaxa').style.display = 'none';

            if (local.nm_tipo_local == 2) {
                //dojo.byId('corpoCad').style.height = '350px';
                dojo.byId('tBanco').style.display = 'block';
                dojo.byId('tCnab').style.display = 'block';
            }
            else {
                //dojo.byId('corpoCad').style.height = '170px';
                dojo.byId('tBanco').style.display = 'none';
                dojo.byId('tCnab').style.display = 'none';
                dojo.byId('tConjunta').style.display = 'none';
            }
        } else if (local.nm_tipo_local == CARTAO_CREDITO || local.nm_tipo_local == CARTAO_DEBITO) {

            dijit.byId('cd_pessoa_local').setAttribute('disabled', true);
            dijit.byId('cadPessoaLocal').setAttribute('disabled', true);
            dojo.byId('tTaxa').style.display = 'block';
            dijit.byId('tTaxa').set('open', true);

            dojo.byId('tBanco').style.display = 'none';
            dojo.byId('tCnab').style.display = 'none';
            dojo.byId('tConjunta').style.display = 'none';
            
            dijit.byId("lista_locais_movto_banco").set("value", local.cd_local_banco);
            gridTaxa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: local.taxaBancaria }) }));
            gridTaxa.update();
        }
        else {
            //dojo.byId('corpoCad').style.height = '170px';
            dijit.byId('cd_pessoa_local').setAttribute('disabled', true);
            dijit.byId('cadPessoaLocal').setAttribute('disabled', true);
            dojo.byId('tBanco').style.display = 'none';
            dojo.byId('tCnab').style.display = 'none';
            dojo.byId('tConjunta').style.display = 'none';
            dojo.byId('tTaxa').style.display = 'none';
        }
        
    } catch (e) {
        postGerarLog(e);
    }
}

function populaBanco(idBanco, field) {
    // Popula os produtos:
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getAllBanco",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataBanco) {
            try{
                loadBanco(dataBanco.retorno, field, idBanco);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemLocalMovto', error);
        });
    });
}

function loadBanco(items, linkBanco, idBanco) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbBanco = dijit.byId(linkBanco);

            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_banco, name: value.no_banco });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbBanco.store = stateStore;
            if (hasValue(idBanco)) {
                cbBanco._onChangeActive = false;
                cbBanco.set("value", idBanco);
                cbBanco._onChangeActive = true;
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoEditarLocalMovto(itensSelecionados, xhr) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridLocalMovto = dijit.byId('gridLocalMovto');

            apresentaMensagem('apresentadorMensagem', '');
            gridLocalMovto.itemSelecionado = itensSelecionados[0];
            keepValuesLocalMovto(gridLocalMovto.itemSelecionado, null, xhr);
            //TODO VERIFICAR DEPOIS
            IncluirAlterar(0, 'divAlterarLocalMovto', 'divIncluirLocalMovto', 'divExcluirLocalMovto', 'apresentadorMensagemLocalMovto', 'divCancelarLocalMovto', 'divLimparLocalMovto');
            dijit.byId("cadLocalMovto").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function padraoTela() {
    try{
        // dojo.byId('corpoCad').style.height = '170px';
        dojo.byId('tBanco').style.display = 'none';
        dojo.byId('tCnab').style.display = 'none';
        dojo.byId('tConjunta').style.display = 'none';
        dojo.byId('tTaxa').style.display = 'none';
    } catch (e) {
        postGerarLog(e);
    }
}

function montarLocalMovto() {
    try{
        var gridTaxa = dijit.byId('gridTaxa');
        var taxaBancaria = hasValue(gridTaxa.store.objectStore.data) ? gridTaxa.store.objectStore.data : null;

        var dadosRetorno = {
            cd_local_movto: dojo.byId("cd_local_movto").value,
            cd_pessoa_local: dijit.byId("cd_pessoa_local").value,
            no_pessoa_local: dojo.byId("cd_pessoa_local").value,
            nm_tipo_local: dijit.byId("nm_tipo_local").value,
            cd_banco: dijit.byId("cd_banco").value,
            cd_pessoa_banco: dijit.byId("cd_pessoa_banco").value,
            no_pessoa_banco: dojo.byId("cd_pessoa_banco").value,
            dc_num_cliente_banco: dijit.byId("dc_num_cliente_banco").value,
            dc_nosso_numero: dojo.byId("dc_nosso_numero").value,
            cd_carteira_cnab : dijit.byId("cbCarteira").value,
            dc_pessoa_conjunta: dojo.byId("dc_pessoa_conjunta").value,
            dc_cpf_pessoa_conjunta: dojo.byId("dc_cpf_pessoa_conjunta").value,
            no_local_movto: dojo.byId("no_local_movto").value,
            nm_banco: dojo.byId("nm_banco").value,
            nm_agencia: dojo.byId("nm_agencia").value,
            nm_digito_agencia: dojo.byId("nm_digito_agencia").value,
            nm_conta_corrente: dojo.byId("nm_conta_corrente").value,
            nm_digito_conta_corrente: dojo.byId("nm_digito_conta_corrente").value,
            id_local_ativo: dijit.byId("id_local_ativo").checked,
            id_conta_conjunta: dijit.byId("id_conta_conjunta").checked,
            nm_sequencia: dojo.byId("nm_sequencia").value,
            nm_transmissao: dojo.byId("nm_transmissao").value,
            nm_digito_cedente: dojo.byId("dc_digito_cedente").value,
            nm_op_conta: dojo.byId("nm_operacao_conta_corrente").value,
            taxaBancaria: taxaBancaria,
            cd_local_banco: dijit.byId("lista_locais_movto_banco").get("value"),
        }
        return dadosRetorno;
    } catch (e) {
        postGerarLog(e);
    }
}

//CRUD
function IncluirLocalMovto() {
    try{
        if (!dijit.byId('formLocalMovto').validate() || !dijit.byId('nm_tipo_local').validate()) {
            return false;
        }
        var localMovto = montarLocalMovto();
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemLocalMovto', null);
        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postLocalMovto",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(localMovto)
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridLocalMovto';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadLocalMovto").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_local_movto", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoLocalMovto', 'cd_local_movto', 'selecionaTodosLocalMovto', ['pesquisarLocalMovto', 'relatorioLocalMovto'], 'todosItensLocalMovto');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_local_movto");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagemLocalMovto', error);
    });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Local de Movimento
function AlterarLocalMovto() {
    try{
        var gridName = 'gridLocalMovto';
        var grid = dijit.byId(gridName);
    if (!dijit.byId('formLocalMovto').validate() || !dijit.byId('nm_tipo_local').validate())
            return false;
        var localMovto = montarLocalMovto();
        showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postAlterarLocalMovto",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(localMovto)
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensLocalMovto");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadLocalMovto").hide();
                    removeObjSort(grid.itensSelecionados, "cd_local_movto", dom.byId("cd_local_movto").value);
                    insertObjSort(grid.itensSelecionados, "cd_local_movto", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoLocalMovto', 'cd_local_movto', 'selecionaTodosLocalMovto', ['pesquisarLocalMovto', 'relatorioLocalMovto'], 'todosItensLocalMovto');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_local_movto");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemLocalMovto', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarLocalMovto(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_local_movto').value != 0)
                    itensSelecionados = [{
                        cd_local_movto: dojo.byId("cd_local_movto").value
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postDeleteLocalMovto",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensLocalMovto");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadLocalMovto").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridLocalMovto').itensSelecionados, "cd_local_movto", itensSelecionados[r].cd_local_movto);
                    PesquisarLocalMovto(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarLocalMovto").set('disabled', false);
                    dijit.byId("relatorioLocalMovto").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadLocalMovto").style.display))
                    apresentaMensagem('apresentadorMensagemLocalMovto', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}

function validarCPF(idCpf, idApreMsg) {
    try{
        var myCPF;
        myCPF = $(idCpf).val().replace('.', '').replace('.', '').replace('-', '');
        var numeros, digitos, soma, i, resultado, digitos_iguais;
        digitos_iguais = 1;

        if (myCPF.length < 11) {
            mostrarMensagenCPF(idApreMsg);
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
                mostrarMensagenCPF(idApreMsg);
                //$("#cpf").focus();
                return false;
            }
            numeros = myCPF.substring(0, 10);
            soma = 0;
            for (i = 11; i > 1; i--)
                soma += numeros.charAt(11 - i) * i;
            resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
            if (resultado != digitos.charAt(1)) {
                mostrarMensagenCPF(idApreMsg);
                // $("#cpf").focus();
                return false;
            }
            return true;
        }
        else {
            mostrarMensagenCPF(idApreMsg);
            return false;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function mostrarMensagenCPF(idApreMsg) {
    try{
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgCPFInvalid);
        apresentaMensagem(idApreMsg, mensagensWeb);
    } catch (e) {
        postGerarLog(e);
    }
}

function montaDadosBanco(xhr) {
    xhr.get({
        url: Endereco() + "/api/cnab/getBancobyId?cdBanco=" + dijit.byId('cd_banco').value + "&cdLocalMovto=" + dojo.byId("cd_local_movto").value,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataBanco) {
        try{
            dijit.byId("nm_banco").set("value", dataBanco.retorno.nm_banco);
        dijit.byId("no_local_movto").set("value", hasValue(dijit.byId("no_local_movto").value) ? dijit.byId("no_local_movto").value : dataBanco.retorno.no_banco.substring(0, 32));
            criarOuCarregarCompFiltering("cbCarteira", dataBanco.retorno.CarteirasCnab, "", dojo.byId("cd_local_movto").value, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_carteira_cnab', 'nomeCarteira', 0);
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemLocalMovto', error);
    });
}

function loadCarteira(xhr, cdBanco, cdLocalMovto, cdCarteira) {
    xhr.get({
        url: Endereco() + "/api/cnab/getCarteiraByBanco?cdLocalMovot=" + cdLocalMovto + "&cdBanco=" + cdBanco,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            criarOuCarregarCompFiltering("cbCarteira", jQuery.parseJSON(data).retorno, "", cdCarteira, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_carteira_cnab', 'nomeCarteira', 0);
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemLocalMovto", error);
    });
}

function abrirPessoaFK() {
    try{
        limparPesquisaPessoaFK();
        dijit.byId("tipoPessoaFK").set("value", 2);
        dijit.byId("tipoPessoaFK").set("disabled", true);
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        pesquisaPessoaFK(true, FILTRAR_ESCOLA);
        dijit.byId("proPessoa").show();
    } catch (e) {
        postGerarLog(e);
    }
}

function incluirTaxaBancaria() {
    try {
        if (!dijit.byId("dialogTaxaBancaria").validate())
            return false;

        //document.getElementById('divExcluirPlano').style.visibility = "hidden";
        var taxaBancaria = parseInt(dojo.byId("cd_taxa_bancaria").value);
        taxaBancaria = ++taxaBancaria;
        dojo.byId("cd_taxa_bancaria").value = taxaBancaria;
        var gridTaxaBancaria = dijit.byId("gridTaxa");

        var storegridTaxaBancarias = (hasValue(gridTaxaBancaria) && hasValue(gridTaxaBancaria.store.objectStore.data)) ? gridTaxaBancaria.store.objectStore.data : [];
        insertObjSort(gridTaxaBancaria.store.objectStore.data, "cd_taxa_bancaria", {
            cd_taxa_bancaria: dojo.byId('cd_taxa_bancaria').value,
            nm_parcela: dijit.byId("valParcela").get("value"),
            pc_taxa: dijit.byId('valTaxa').get("value"),
            nm_dias: dijit.byId('valPrazo').get("value"),
            dh_taxa_bancaria: montarDataHoraTaxaBancaria()

    });

        gridTaxaBancaria.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridTaxaBancaria.store.objectStore.data }) }));
        dijit.byId("dialogTaxaBancaria").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDataHoraTaxaBancaria() {
    try {
        var date = dojo.date.locale.format(new Date(),
            { selector: "date", datePattern: "MM/dd/yyyy HH:mm:ss", formatLength: "short", locale: "pt-br" });

        return date;
    } catch (e) {

    }
}

function limparFormTaxaBancaria() {
    try {
        dijit.byId("valParcela").set("value", null);
        dijit.byId("valTaxa").set("value", null);
        dijit.byId("valPrazo").set("value", null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearGridTaxaBancaria() {
    var gridTaxaBancaria = dijit.byId("gridTaxa");
    gridTaxaBancaria.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    gridTaxaBancaria.update();
}

function eventoEditarTaxaBancaria(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagemTaxaBancaria', '');
            keepValuesTaxaBancaria(dijit.byId('gridTaxa'), null);

            IncluirAlterar(0, 'divAlterarTxBancaria', 'divIncluirTxBancaria', 'divExcluirTxBancaria', 'apresentadorMensagemTaxaBancaria', 'divCancelarTxBancaria', 'divLimparTxBancaria');
            dijit.byId("dialogTaxaBancaria").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesTaxaBancaria(grid, taxa) {
    try {
        limparFormTaxaBancaria();
        var taxaBancaria = hasValue(grid) ? grid.itensSelecionados[0] : taxa;
       
        if (hasValue(taxaBancaria)) {
            document.getElementById("cd_taxa_bancaria").value = taxaBancaria.cd_taxa_bancaria;
            dijit.byId("valParcela").set("value", taxaBancaria.nm_parcela);
            dijit.byId("valTaxa").set("value", taxaBancaria.pc_taxa);
            dijit.byId("valPrazo").set("value", taxaBancaria.nm_dias);

            document.getElementById('divExcluirTxBancaria').style.visibility = "hidden";
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarTaxaBancaria() {
    try {
        if (!dijit.byId("dialogTaxaBancaria").validate())
            return false;
        var grid = dijit.byId("gridTaxa");
        for (var i = 0; i < grid._by_idx.length; i++)
            if (grid._by_idx[i].item.cd_taxa_bancaria == dojo.byId("cd_taxa_bancaria").value) {

                grid._by_idx[i].item.nm_parcela = dijit.byId("valParcela").get("value");
                grid._by_idx[i].item.pc_taxa = dijit.byId('valTaxa').get("value");
                grid._by_idx[i].item.nm_dias = dijit.byId('valPrazo').get("value");

                break;
            }
        grid.update();
        dijit.byId("dialogTaxaBancaria").hide();
        grid.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getTaxaBancariaPorId() {
    showCarregando();
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/financeiro/getTaxaBancariaPorId?cd_taxa_bancaria=" + dojo.byId("cd_taxa_bancaria").value,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var taxaBancaria = jQuery.parseJSON(data).retorno;
            keepValuesTaxaBancaria(null, taxaBancaria);
            showCarregando();
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagemMat', error);
    });
}

function getAllLocalMovtoTipoBanco() {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getAllLocalMovtoTipoBanco",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            response = jQuery.parseJSON(data);
            loadSelect(response.retorno, "lista_locais_movto_banco", 'cd_local_movto', 'nomeLocal');
        } catch (e) {
            postGerarLog(e);
        }

    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}