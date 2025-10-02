var TIPOMOVIMENTO = null;
var VENDASSERVICO = 4, ORIGEMCHAMADONF = 48;
function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridEmprestimo';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_biblioteca", grid._by_idx[rowIndex].item.cd_biblioteca);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_biblioteca', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_biblioteca', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(grid) {
    try {
        var value = grid.itemSelecionado;
       // document.getElementById("divRenovar").style.display = "";
        dojo.byId('cd_biblioteca').value = value.cd_biblioteca;

        require(["dojo/_base/xhr", "dojo/dom", "dijit/registry", "dojox/grid/EnhancedGrid",
                   "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory", "dojo/query", "dojo/dom-attr",
                   "dijit/Dialog", "dojo/domReady!"
        ], function (xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr) {
            xhr.get({
                preventCache: true,
                url: Endereco() + "/api/escola/getemprestimo?cd_biblioteca=" + value.cd_biblioteca,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                value = jQuery.parseJSON(eval(data)).retorno;
                grid.EmprestimoBD = value;
                grid.EmprestimoBD.cd_biblioteca = dojo.byId('cd_biblioteca').value;
                dojo.byId('cd_item').value = value.cd_item;
                dojo.byId('no_pessoa').value = value.no_pessoa;
                dojo.byId('dtaEmprestimo').value = value.dta_emprestimo;
                dojo.byId('dt_prev_devolucao').value = value.dta_prevista_devolucao;
                dijit.byId('dt_prev_devolucao').oldValue = value.dta_prevista_devolucao
                dojo.byId('no_item').value = value.no_item;
                dojo.byId('cd_pessoa').value = value.cd_pessoa;
                dojo.byId('tx_obs_biblioteca').value = value.tx_obs_biblioteca;

                dijit.byId('dta_devolucao').set('value', dojo.date.locale.parse(value.dta_devolucao, { formatLength: 'short', selector: 'date', locale: 'pt-br' }));
                //dojo.byId('dta_devolucao').value = value.dta_devolucao;
                dijit.byId('vl_multa_emprestimo').set('value', value.vl_multa_emprestimo);
                dijit.byId('vl_taxa_emprestimo')._onChangeActive = false;
                dijit.byId('vl_taxa_emprestimo').set('value', value.vl_taxa_emprestimo);
                dijit.byId('vl_taxa_emprestimo')._onChangeActive = true;
                dojo.byId('nm_dias_biblioteca').value = value.nm_dias_biblioteca;

                if (value.id_bloquear_alt_dta_biblio)
                    dijit.byId('dta_devolucao').set('disabled', true);

                if (value.existe_devolucao) {
                    dijit.byId('dt_prev_devolucao').set('disabled', true);
                    dijit.byId('dtaEmprestimo').set('disabled', true);
                } else {
                    dijit.byId('dt_prev_devolucao').set('disabled', false);
                    dijit.byId('dtaEmprestimo').set('disabled', false);
                }
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugerirPrevDevolucao() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/api/escola/getParametrosPrevDevolucao?dataEmprestimo=" + dojo.byId("dtaEmprestimo").value,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                dijit.byId("dt_prev_devolucao").set("value", jQuery.parseJSON(eval(data)).retorno);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemEmprestimo', error);
        });
    })
}

function montarCadastroBiblioteca() {
    //Criação da Grade de sala
    require([
    "dojo/dom",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dojo/date",
    "dijit/Dialog",
    "dojo/domReady!"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, DropDownButton, DropDownMenu, MenuItem, date) {
        ready(function () {
            try {
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/biblioteca/getEmprestimoSearch?cd_pessoa=&cd_item=&pendentes=&dt_inicial=&dt_final=&emprestimos=&devolucao=",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                var gridEmprestimo = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        { name: "Data", field: "dta_emprestimo", width: "10%", styles: "min-width:80px;" },
                        { name: "Pessoa", field: "no_pessoa", width: "30%", styles: "min-width:80px;" },
                        { name: "Item", field: "no_item", width: "30%", styles: "min-width:80px;" },
                        { name: "Previsto", field: "dta_prevista_devolucao", width: "7%", styles: "min-width:80px;" },
                        { name: "Devolução", field: "dta_devolucao", width: "8%", styles: "min-width:80px;" },
                        { name: "Taxa", field: "vlTaxaEmprestimo", width: "5%", styles: "min-width:80px;text-align: center;" },
                        { name: "Multa", field: "vlMultaEmprestimo", width: "5%", styles: "text-align: center;" }
                    ],
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
                }, "gridEmprestimo");
                gridEmprestimo.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3 && Math.abs(col) != 4; };
                gridEmprestimo.startup();
                gridEmprestimo.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        gridEmprestimo.itemSelecionado = item;
                        limparEmprestimo(false);
                        keepValues(gridEmprestimo);
                        apresentaMensagem('apresentadorMensagem', '');
                        dojo.byId('tagDevolucao').style.display = 'block';
                        setarTabCad();
                        dijit.byId("cad").show();
                        dijit.byId('dta_devolucao').set('required', true);
                        IncluirAlterar(0, 'divAlterar', 'divIncluir', 'divExcluir', 'apresentadorMensagemEmprestimo', 'divCancelar', 'divLimpar');
                        dijit.byId('tabContainer').resize(true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridEmprestimo.EmprestimoBD = null;
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            document.getElementById("tipoFKPessoa").value = "pesq";
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaCadFK();
                                    });
                                    dijit.byId("fecharFK").on("click", function (e) {
                                        dijit.byId("fkPessoaPesq").hide();
                                    });
                                });
                            else
                                abrirPessoaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProPessoaFK");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            document.getElementById("tipoFKPessoa").value = "cad";
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaCadFK();
                                    });
                                    dijit.byId("fecharFK").on("click", function (e) {
                                        dijit.byId("fkPessoaPesq").hide();
                                    });
                                });
                            else
                                abrirPessoaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPessoaFKCad");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            document.getElementById("tipoFKItem").value = "pesq";
                            if (!hasValue(dijit.byId("pesquisarItemFK"))) {
                                montargridPesquisaItem(function () {
                                    abrirItemFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemEstoqueCadFK(true);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) {
                                        dijit.byId("fkItem").hide();
                                    });
                                }, true, true);
                            } else {
                                abrirItemFK();
                                dijit.byId("fecharItemFK").on("click", function (e) {
                                    dijit.byId("fkItem").hide();
                                });
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProItemFK");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            document.getElementById("tipoFKItem").value = "cad";
                            if (!hasValue(dijit.byId("pesquisarItemFK")))
                                montargridPesquisaItem(function () {
                                    abrirItemFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemEstoqueCadFK(true);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) {
                                        dijit.byId("fkItem").hide();
                                    });
                                }, true, true);
                            else
                                abrirItemFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesItemFKCad");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        incluirEmprestimo();
                    }
                }, "incluir");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        limparEmprestimo(false);
                        keepValues(dijit.byId('gridEmprestimo'));
                        apresentaMensagem('apresentadorMensagem', '');
                        setarTabCad();
                    }
                }, "cancelar");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { dijit.byId("cad").hide(); } }, "fechar");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        editarEmprestimo();
                    }
                }, "alterar");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarEmprestimo() }); } }, "delete");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparEmprestimo(true); } }, "limpar");
                //Fim
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        try {
                            apresentaMensagem('apresentadorMensagem', null);

                            var dtaInicio = hasValue(dojo.byId("dtInicial").value) ? dojo.date.locale.parse(dojo.byId("dtInicial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var dtaFinal = hasValue(dojo.byId("dtFinal").value) ? dojo.date.locale.parse(dojo.byId("dtFinal").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

                            //Verifica se tem alguma opção maracada para a pesquisa de períodos:
                            if (!dijit.byId('ckEmprestimo').checked && !dijit.byId('ckDevolucao').checked) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroOpcaoPesqBiblioteca);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                return false;
                            }

                            //Verifica se a data inicial é maior que a data final:
                            if (date.compare(dtaInicio, dtaFinal) > 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinalBiblioteca);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                return false;
                            }
                            pesquisarEmprestimo(true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesquisarEmprestimo");
                decreaseBtn(document.getElementById("pesquisarEmprestimo"), '32px');

                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            xhr.get({
                                url: Endereco() + "/api/biblioteca/geturlrelatorioemprestimo?" + getStrGridParameters('gridEmprestimo') + "cd_pessoa=" + pessoa_pesq
                                    + "&cd_item=" + item_pesq
                                    + "&pendentes=" + dijit.byId('ckPendentes').checked + "&dt_inicial=" + dojo.byId('dtInicial').value + "&dt_final=" + dojo.byId('dtFinal').value
                                    + "&emprestimos=" + dijit.byId('ckEmprestimo').checked + "&devolucao=" + dijit.byId('ckDevolucao').checked,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        })
                    }
                }, "relatorioEmprestimo");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                        function () {
                            try {
                                limparEmprestimo(true);
                                sugereDataCorrente('dtaEmprestimo');
                                dojo.byId('tagDevolucao').style.display = 'none';
                                dijit.byId("cad").show();
                                dijit.byId('tabContainer').resize(true);
                                dojo.byId('tabContainer_tablist').children[3].children[0].style.width = '100%';
                                dijit.byId('dta_devolucao').set('required', false);
                                IncluirAlterar(1, 'divAlterar', 'divIncluir', 'divExcluir', 'apresentadorMensagemEmprestimo', 'divCancelar', 'divLimpar');
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                }, "novo");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item_pesq').value = '';
                        dojo.byId("item_pesq").value = "";
                        dijit.byId('limparItemFK').set('disabled', true);
                    },
                    disabled: true
                }, "limparItemFK");
                if (hasValue(document.getElementById("limparItemFK"))) {
                    document.getElementById("limparItemFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparItemFK").parentNode.style.width = '40px';
                }
                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_pessoa_pesq').value = '';
                        dojo.byId("pessoa_pesq").value = "";
                        dijit.byId("limparPessoaFK").set('disabled', true);
                    },
                    disabled: true
                }, "limparPessoaFK");
                if (hasValue(document.getElementById("limparPessoaFK"))) {
                    document.getElementById("limparPessoaFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaFK").parentNode.style.width = '40px';
                }
                var buttonFkArray = ['pesProPessoaFK', 'pesProItemFK', 'pesItemFKCad', 'pesPessoaFKCad'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                //#region Links
                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridEmprestimo.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(dijit.byId('gridEmprestimo').itensSelecionados, 'deletarEmprestimo(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

                var acaoTaxa = new MenuItem({
                    label: "Taxa de Devolução",
                    onClick: function () { redirecionarTaxa(); }
                });
                menu.addChild(acaoTaxa);

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
                    onClick: function () {
                        buscarTodosItens(dijit.byId('gridEmprestimo'), 'todosItens', ['pesquisarEmprestimo', 'relatorioEmprestimo']);
                        pesquisarEmprestimo(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridEmprestimo', 'selecionado', 'cd_biblioteca', 'selecionaTodos', ['pesquisarEmprestimo', 'relatorioEmprestimo'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                dijit.byId("vl_taxa_emprestimo").on("change", function (e) {
                    calculaValorMulta();
                });
                dijit.byId("dta_devolucao").on("change", function (e) {
                    calculaValorMulta();
                });
                dijit.byId("dtaEmprestimo").on("change", function (e) {
                    try {
                        if (hasValue(e))
                            sugerirPrevDevolucao();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("renovar").on("click", function (e) {
                    try {
                        renovarEmprestimo();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("dt_prev_devolucao").on("change", function (data) {
                    try {
                        apresentaMensagem('apresentadorMensagemEmprestimo', null);
                        if (hasValue(dojo.byId("cd_biblioteca").value)) {
                            if (hasValue(data)) {
                                var compDtaMovto = dijit.byId("dtaMovto");
                                if (hasValue(dataHoje) && dojo.date.compare(dataHoje, data) > 0) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mmsgInfoDataPrevDevolRetroativa);
                                    apresentaMensagem('apresentadorMensagemEmprestimo', mensagensWeb);
                                    dijit.byId("dt_prev_devolucao")._onChangeActive = false;
                                    dijit.byId("dt_prev_devolucao").reset();
                                    dijit.byId("dt_prev_devolucao")._onChangeActive = true;
                                    return false;
                                }
                                if (dijit.byId("dt_prev_devolucao").oldValue != dojo.byId("dt_prev_devolucao").value) {
                                    dojo.byId('tagDevolucao').style.display = 'none';
                                    dojo.byId('divAlterar').style.display = 'none';
                                    dojo.byId('divRenovar').style.display = '';
                                } else {
                                    dojo.byId('tagDevolucao').style.display = '';
                                    dojo.byId('divAlterar').style.display = '';
                                    dojo.byId('divRenovar').style.display = 'none';
                                }
                            }
                            else {
                                dojo.byId('tagDevolucao').style.display = '';
                                dojo.byId('divAlterar').style.display = '';
                                dojo.byId('divRenovar').style.display = 'none';
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //#endregion
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323046', '765px', '771px');
                        });
                }
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function calculaValorMulta() {
    try {
        //var diferenca_datas = dijit.byId('dta_devolucao').value - dojo.date.locale.parse(dojo.byId('dtaEmprestimo').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        var diferenca_datas = dojo.date.locale.parse(dojo.byId('dta_devolucao').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) -
            dojo.date.locale.parse(dojo.byId('dt_prev_devolucao').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

        if (diferenca_datas > 0) {
            var diferenca_dias = Math.round(diferenca_datas / (1000 * 60 * 60 * 24));
            var valor_multa = unmaskFixed((diferenca_dias * dijit.byId('vl_taxa_emprestimo').value), 2);

            if (valor_multa > 0)
                dijit.byId('vl_multa_emprestimo').set('value', valor_multa);
            else // Caso o usuario volte para a data que não tem multa:
                dijit.byId('vl_multa_emprestimo').set('value', 0);
        }
        else
            dijit.byId('vl_multa_emprestimo').set('value', 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            var gridEmprestimo = dijit.byId('gridEmprestimo');

            limparEmprestimo(false);
            apresentaMensagem('apresentadorMensagem', '');
            gridEmprestimo.itemSelecionado = itensSelecionados[0];
            keepValues(gridEmprestimo);
            dojo.byId('tagDevolucao').style.display = 'block';
            setarTabCad();
            dijit.byId("cad").show();
            dijit.byId('dta_devolucao').set('required', true);
            IncluirAlterar(0, 'divAlterar', 'divIncluir', 'divExcluir', 'apresentadorMensagemEmprestimo', 'divCancelar', 'divLimpar');
            dijit.byId('tabContainer').resize(true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente(campo) {
    require(["dojo/_base/xhr"
    ], function (xhr) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/util/getdatacorrente?trazerHora=false",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            dojo.byId(campo).value = data;
            if (hasValue(data))
                sugerirPrevDevolucao();
        });
    });
}

function abrirPessoaFK() {
    try {
        limparPesquisaPessoaFK();
        pesquisaPessoaCadFK();
        dijit.byId("fkPessoaPesq").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemEstoqueCadFK() {
    if (document.getElementById("tipoFKItem").value == "pesq")
        return pesquisarItemEstoqueFK();

    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getItemEstoqueCadSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=1&tipoItemInt=" + dijit.byId("tipo").value + "&grupoItem=" + grupoItem + "&cEstoque=" + document.getElementById("comEstoque").checked,
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

function pesquisarItemEstoqueFK() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
        myStore = Cache(
           JsonRest({
               target: Endereco() + "/api/financeiro/getItemEstoqueSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" + retornaStatus("statusItemServico") + "&tipoItemInt=0&grupoItem=" + grupoItem + "&cEstoque=" + document.getElementById("comEstoque").checked,
               handleAs: "json",
               headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
               idProperty: "cd_item"
           }
        ), Memory({ idProperty: "cd_item" }));
        dataStore = new ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridPesquisaItem");
        grid.setStore(dataStore);
    });
}

function abrirItemFK() {
    try {
        limparPesquisaCursoFK(false);
        pesquisarItemEstoqueCadFK();
        showP('comEstoqueTitulo', true);
        showP('comEstoqueCampo', true);
        dijit.byId("fkItem").show();
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("tipo").set("disabled", true);
        dijit.byId("statusItemFK").set("disabled", true);
        setTimeout(function () { dijit.byId("tipo").set('value', 3); }, 100);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaCadFK() {
    if (document.getElementById("tipoFKPessoa").value == "pesq")
        return pesquisaPessoaBibliotecaFK();

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
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/biblioteca/getPessoaEmprestimoSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function pesquisaPessoaBibliotecaFK() {
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
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/biblioteca/getPessoaBibliotecaSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabPrincipal");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDialog(nome, e) {
    try {
        var offSetX = 20;
        var offSetY = 20;
        $("#" + nome).css("top", e.pageX + offSetX).css("left", e.pageY + offSetY);
        dijit.byId(nome).show();
        dojo.byId(nome).style.display = "block";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatImage(value, rowIndex, obj, k) {
    try {
        var gridAvalAluno = dijit.byId("gridAvalAluno");
        var icon;
        var id = k.field + '_Selected_' + gridAvalAluno._by_idx[rowIndex].item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(value) && rowIndex % 2 == 0)
            icon = "<span id='id' class='dijitReset dijitInline dijitIcon dijitEditorIcon dijitEditorIconNewSGF' data-dojo-attach-point='iconNode'></span>";
        else
            icon = '<span class="dijitReset dijitInline dijitIcon dijitEditorIcon dijitEditorIconNewPage" data-dojo-attach-point="iconNode"></span>';
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatImage(value, rowIndex, obj, k) {
    try {
        var gridEventoAluno = dijit.byId("gridEventoAluno");
        var icon;
        var id = k.field + '_IMG_' + gridEventoAluno._by_idx[rowIndex].item._0;
        var hasTexto = hasValue(value) ? true : false;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
        if ((gridEventoAluno._by_idx[rowIndex].item.pai[0] !== 1))
            setTimeout("configuraButtonImg(" + hasTexto + ", '" + id + "'," + gridEventoAluno._by_idx[rowIndex].item.id + "," + gridEventoAluno._by_idx[rowIndex].item.idPai + "," + false + ")", 1);
        else
            setTimeout("configuraButtonImg(" + hasTexto + ", '" + id + "'," + gridEventoAluno._by_idx[rowIndex].item.id + "," + gridEventoAluno._by_idx[rowIndex].item.idPai + "," + true + ")", 1);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraButtonImg(value, idfield, id, idPai, isPai) {
    try {
        var dialogGrid = '';
        if (hasValue(dijit.byId('dialogGrid'))) dialogGrid = dijit.byId('dialogGrid');
        if (!hasValue(dijit.byId(idfield))) {
            require(["dijit/form/Button", "dojo/dom", "dojo/domReady!"], function (Button, dom) {
                var myButton = new Button({
                    title: "Clique aqui para digitar uma obeservação",
                    iconClass: value == true ? 'dijitEditorIcon dijitEditorIconNewSGF' : 'dijitEditorIcon dijitEditorIconNewPage',
                    name: "button_" + idfield,
                    onClick: function () {
                        dialogGrid.show();
                    }
                }, idfield);
            });
            var buttonFkArray = [idfield];
            diminuirBotaoGrid(buttonFkArray);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function diminuirBotaoGrid(buttonFkArray) {
    try {
        for (var p = 0; p < buttonFkArray.length; p++) {
            var buttonFk = document.getElementById(buttonFkArray[p]);

            if (hasValue(buttonFk)) {
                buttonFk.parentNode.style.minWidth = '13px';
                buttonFk.parentNode.style.width = '15px';
                buttonFk.parentNode.style.height = '12px';
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var gridPesPessoaRel = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPesPessoaRel.itensSelecionados) || gridPesPessoaRel.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesPessoaRel.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {
            if (document.getElementById("tipoFKPessoa").value == "cad") {
                dojo.byId("cd_pessoa").value = gridPesPessoaRel.itensSelecionados[0].cd_pessoa;
                dojo.byId("no_pessoa").value = gridPesPessoaRel.itensSelecionados[0].no_pessoa;
            }
            else {
                dojo.byId("cd_pessoa_pesq").value = gridPesPessoaRel.itensSelecionados[0].cd_pessoa;
                dojo.byId("pessoa_pesq").value = gridPesPessoaRel.itensSelecionados[0].no_pessoa;
            }
            dijit.byId("limparPessoaFK").set('disabled', false);
            dijit.byId("fkPessoaPesq").hide();
        }
    }
    catch (e) {
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
            dijit.byId("fkItem").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarEmprestimo() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemEmprestimo', null);
    if (!dijit.byId("formEmprestimo").validate())
        return false;

    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postEditEmprestimo",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_biblioteca: dojo.byId('cd_biblioteca').value,
                cd_pessoa: dojo.byId("cd_pessoa").value,
                cd_item: dojo.byId("cd_item").value,
                dt_emprestimo: dojo.date.locale.parse(dojo.byId("dtaEmprestimo").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                dt_prevista_devolucao: dojo.date.locale.parse(dojo.byId("dt_prev_devolucao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                dt_devolucao: dojo.date.locale.parse(dojo.byId("dta_devolucao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                Pessoa: { no_pessoa: dojo.byId("no_pessoa").value },
                Item: { no_item: dojo.byId("no_item").value },
                tx_obs_biblioteca: dojo.byId('tx_obs_biblioteca').value,
                vl_taxa_emprestimo: dijit.byId('vl_taxa_emprestimo').value,
                vl_multa_emprestimo: dijit.byId('vl_multa_emprestimo').value
            })
        }).then(function (data) {
            try {
                var itemAlterado = data.retorno;
                var todos = dojo.byId("todosItens_label");
                var gridName = 'gridEmprestimo';
                var grid = dijit.byId(gridName);

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cad").hide();
                removeObjSort(grid.itensSelecionados, "cd_biblioteca", dom.byId("cd_biblioteca").value);
                insertObjSort(grid.itensSelecionados, "cd_biblioteca", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionado', 'cd_biblioteca', 'selecionaTodos', ['pesquisarEmprestimo', 'relatorioEmprestimo'], 'todosItens', 2);
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_biblioteca");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEmprestimo', error);
        });
    });
}

function incluirEmprestimo() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemEmprestimo', null);
    if (!dijit.byId("formEmprestimo").validate())
        return false;

    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postemprestimo",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_pessoa: dojo.byId("cd_pessoa").value,
                cd_item: dojo.byId("cd_item").value,
                str_dt_emprestimo: dojo.byId("dtaEmprestimo").value,
                str_dt_prevista_devolucao: dojo.byId("dt_prev_devolucao").value,
                //dt_devolucao: dom.byId("nomeCurso").value,
                Pessoa: { no_pessoa: dojo.byId("no_pessoa").value },
                Item: { no_item: dojo.byId("no_item").value },
                tx_obs_biblioteca: dojo.byId('tx_obs_biblioteca').value
            })
        }).then(function (data) {
            try {
                var itemAlterado = data.retorno;
                var gridName = 'gridEmprestimo';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cad").hide();

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                insertObjSort(grid.itensSelecionados, "cd_biblioteca", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionado', 'cd_biblioteca', 'selecionaTodos', ['pesquisarEmprestimo', 'relatorioEmprestimo'], 'todosItens', 2);
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_biblioteca");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEmprestimo', error);
        });
    });
}

function limparEmprestimo(sugerir_data) {
    try {
        apresentaMensagem('apresentadorMensagemEmprestimo', null);
        document.getElementById("divRenovar").style.display = "";
        getLimpar('#formEmprestimo');
        clearForm('formEmprestimo');
        document.getElementById("cd_emprestimo").value = '';
        document.getElementById("cd_pessoa").value = '';
        document.getElementById("cd_item").value = '';
        document.getElementById("divRenovar").style.display = "none";
        gridEmprestimo.EmprestimoBD = null;
        //Volta a data sugerida:
        if (sugerir_data)
            setTimeout(function () { sugereDataCorrente('dtaEmprestimo'); }, 10);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarEmprestimo(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var pessoa_pesq = hasValue(dojo.byId('cd_pessoa_pesq').value) ? dojo.byId('cd_pessoa_pesq').value : "";
            var item_pesq = hasValue(dojo.byId('cd_item_pesq').value) ? dojo.byId('cd_item_pesq').value : "";
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/biblioteca/getEmprestimoSearch?cd_pessoa=" + pessoa_pesq
                            + "&cd_item=" + item_pesq
                            + "&pendentes=" + dijit.byId('ckPendentes').checked + "&dt_inicial=" + dojo.byId('dtInicial').value + "&dt_final=" + dojo.byId('dtFinal').value
                            + "&emprestimos=" + dijit.byId('ckEmprestimo').checked + "&devolucao=" + dijit.byId('ckDevolucao').checked,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridEmprestimo");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function deletarEmprestimo(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_biblioteca').value != 0)
                itensSelecionados = [{
                    cd_biblioteca: dom.byId("cd_biblioteca").value
                }];
        xhr.post({
            url: Endereco() + "/api/escola/postDeleteEmprestimo",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cad").hide();

                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridEmprestimo').itensSelecionados, "cd_biblioteca", itensSelecionados[r].cd_biblioteca);

                pesquisarEmprestimo(false);

                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarEmprestimo").set('disabled', false);
                dijit.byId("relatorioEmprestimo").set('disabled', false);

                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cad").style.display))
                apresentaMensagem('apresentadorMensagemEmprestimo', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
}

function redirecionarTaxa() {
    try {

        var gridEmprestimo = dijit.byId('gridEmprestimo');

        if (!hasValue(gridEmprestimo.itensSelecionados) || (gridEmprestimo.itensSelecionados.length <= 0))
            caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
        else
            if (hasValue(gridEmprestimo.itensSelecionados) && (gridEmprestimo.itensSelecionados.length > 1))
                caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            else
                if (!hasValue(gridEmprestimo.itensSelecionados[0].dt_devolucao) || gridEmprestimo.itensSelecionados[0].dt_devolucao == null)
                    caixaDialogo(DIALOGO_ERRO, msgErroTaxaDevolucao, null);
                else
                    if (gridEmprestimo.itensSelecionados[0].vl_multa_emprestimo <= 0)
                        caixaDialogo(DIALOGO_ERRO, msgErroTaxaSemTaxa, null);
                    else {
                        showCarregando();
                        window.location = Endereco() + '/Secretaria/Movimentos?tipo=' + VENDASSERVICO + '&idOrigemNF=' + ORIGEMCHAMADONF + '&cdBiblioteca=' + gridEmprestimo.itensSelecionados[0].cd_biblioteca;
                    }
    } catch (e) {
        postGerarLog(e);
    }
}

function renovarEmprestimo() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemEmprestimo', null);
    var emprestimoBD = dijit.byId("gridEmprestimo").EmprestimoBD;
    if (!dijit.byId("formEmprestimo").validate())
        return false;
    if (emprestimoBD != null && hasValue(emprestimoBD) && emprestimoBD.cd_biblioteca == dojo.byId('cd_biblioteca').value &&
        (emprestimoBD.cd_item != dojo.byId("cd_item").value || emprestimoBD.cd_pessoa != dojo.byId("cd_pessoa").value ||
         emprestimoBD.dta_emprestimo != dojo.byId("dtaEmprestimo").value)) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mssgInfoRenovarDevolucao);
        apresentaMensagem('apresentadorMensagemEmprestimo', mensagensWeb);
        return false;
    }
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postRenovarEmprestimo",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson({
                cd_biblioteca: dojo.byId('cd_biblioteca').value,
                dt_prevista_devolucao: dojo.date.locale.parse(dojo.byId("dt_prev_devolucao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                Pessoa: { no_pessoa: dojo.byId("no_pessoa").value },
                Item: { no_item: dojo.byId("no_item").value }
            })
        }).then(function (data) {
            try {
                var itemAlterado = data.retorno;
                var todos = dojo.byId("todosItens_label");
                var gridName = 'gridEmprestimo';
                var grid = dijit.byId(gridName);

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cad").hide();
                removeObjSort(grid.itensSelecionados, "cd_biblioteca", dom.byId("cd_biblioteca").value);
                insertObjSort(grid.itensSelecionados, "cd_biblioteca", itemAlterado);

                buscarItensSelecionados(gridName, 'selecionado', 'cd_biblioteca', 'selecionaTodos', ['pesquisarEmprestimo', 'relatorioEmprestimo'], 'todosItens', 2);
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_biblioteca");
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemEmprestimo', error);
        });
    });
}