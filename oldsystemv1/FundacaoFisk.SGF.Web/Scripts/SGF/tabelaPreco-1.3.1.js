var HAS_TABELA_PRECO = 5;

function formatCheckBoxTabelaPreco(value, rowIndex, obj) {
    try{
        var gridName = 'gridTabelaPreco';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTabelaPreco');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tabela_preco", grid._by_idx[rowIndex].item.cd_tabela_preco);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tabela_preco', 'selecionadoTabelaPreco', -1, 'selecionaTodosTabelaPreco', 'selecionaTodosTabelaPreco', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tabela_preco', 'selecionadoTabelaPreco', " + rowIndex + ", '" + id + "', 'selecionaTodosTabelaPreco', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}


function keepValues(value, grid, ehLink) {
    try{
        apresentaMensagem("apresentadorMensagemTabelaPreco", null);
        dijit.byId("cd_curso").set('disabled', true);
        dijit.byId("cd_duracao").set('disabled', true);
        dijit.byId("cd_regime").set('disabled', true);
        dijit.byId("dta_tabela_preco").set('disabled', true);
        dijit.byId("nm_parcelas").set('disabled', true);
        dijit.byId("vl_aula").set('disabled', true);
        dijit.byId("vl_parcela").set('disabled', true);
        dijit.byId("vl_matricula").set('disabled', true);
        getLimpar('#formTabelaPreco');

        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        dijit.byId('tagHistorico').set('open', false);
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];
        if (hasValue(value)) {
            if (hasValue(value.cd_curso) && value.cd_curso > 0) {
                retornaHistorico(value.cd_curso, value.cd_duracao, value.cd_regime);
                var cdCurso = dijit.byId("cd_curso");
                cdCurso._onChangeActive = false;
                cdCurso.set('value', value.cd_curso);
                cdCurso._onChangeActive = true;
            }

            
            dojo.byId("cd_tabela_preco").value = value.cd_tabela_preco;
            dojo.byId("dta_tabela_preco").value = value.dt_tabela;
            dojo.byId("nm_parcelas").value = value.nm_parcelas;
            dojo.byId("vl_parcela").value = value.vl_parc;
            dojo.byId("vl_matricula").value = value.vl_mat;
            dojo.byId("vl_total").value = value.vl_ttl;
            dojo.byId("vl_aula").value = value.vlAula;

            

            var cdDuracao = dijit.byId("cd_duracao");
            cdDuracao._onChangeActive = false;
            cdDuracao.set('value', value.cd_duracao);
            cdDuracao._onChangeActive = true;

            var cdRegime = dijit.byId("cd_regime");
            cdRegime._onChangeActive = false;
            cdRegime.set('value', value.cd_regime);
            cdRegime._onChangeActive = true;
            dijit.byId("tagHistorico").on("click", function (e) {
                dijit.byId("gridHistorico").update();
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function loadProduto(data, linkProduto) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkProduto == 'codProduto') )
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_produto, name: value.no_produto });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("codProduto").set("value", 0);
		        dijit.byId(linkProduto).store = stateStore;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}


function retornaHistorico(cdCurso, cdDuracao, cdRegime) {
    cdRegime = cdRegime > 0 ? cdRegime : 0;
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
            JsonRest({
                handleAs: "json",
                target: Endereco() + "/api/financeiro/getHistoricoTabelaPreco?cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdRegime=" + cdRegime,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }), Memory({}))

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridHistorico = dijit.byId("gridHistorico");

            gridHistorico.setStore(dataStore);
            gridHistorico.update();
        } catch (e) {
            postGerarLog(e);
        }
    });
}
//Inicio da grade Tabela
function montarCadastroTabelaPreco() {

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
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom) {
        ready(function () {
            try {
                var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/financeiro/GetTabelaPrecoSearch?cdCurso=0&cdDuracao=0&cdRegime=0&dtaCad=null&codProduto=0",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }
               ), Memory({}));
                dijit.byId("codProduto").set("required", false);
                //*** Cria a grade de TabelaPreco **\\
                var gridTabelaPreco = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosTabelaPreco' style='display:none'/>", field: "selecionadoTabelaPreco", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTabelaPreco },
                       // { name: "Código", field: "cd_tabela_preco", width: "7%", styles: "width:75px; text-align: left;" },
                        { name: "Curso", field: "no_curso", width: "12%", styles: "min-width:50px;" },
                        { name: "Produto", field: "no_produto", width: "10%", styles: "min-width:50px;" },
                        { name: "Carga Horária", field: "dc_duracao", width: "12%", styles: "min-width:50px;" },
                        { name: "Modalidade", field: "no_regime", width: "12%", styles: "min-width:50px;" },
                        { name: "Data", field: "dt_tabela", width: "10%", styles: "min-width:50px;" },
                        { name: "Parcelas", field: "nm_parcelas", width: "6%", styles: "text-align: center;" },
                        { name: "Unitário", field: "vl_parc", width: "6%", styles: "text-align: center;" },
                        { name: "Total", field: "vl_ttl", width: "8%", styles: "text-align: center;", formatter: "n2" },
                        { name: "Matrícula", field: "vl_mat", width: "8%", styles: "text-align: center;" },
                        { name: "Aula", field: "vlAula", width: "7%", styles: "text-align: center;" }

                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
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
                }, "gridTabelaPreco");

                //Paginação
                gridTabelaPreco.pagination.plugin._paginator.plugin.connect(gridTabelaPreco.pagination.plugin._paginator, 'onSwitchPageSize',
                    function (evt) {
                        verificaMostrarTodos(evt, gridTabelaPreco, 'cd_tabela_preco', 'selecionaTodosTabelaPreco');
                    });
                //Seleciona todos e impede que a primeira coluna seja ordenada
                gridTabelaPreco.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 9; };
                gridTabelaPreco.startup();
                gridTabelaPreco.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        // Limpar os dados do form
                        getLimpar('#formTabelaPreco');
                        clearForm("formTabelaPreco");
                        apresentaMensagem('apresentadorMensagem', '');
                        componentesNovaTurma(xhr, function () {
                            keepValues(item, gridTabelaPreco, false);
                            dijit.byId("cad").show();
                            IncluirAlterar(0, 'divAlterarTabelaPreco', 'divIncluirTabelaPreco', 'divExcluirTabelaPreco', 'apresentadorMensagemTabelaPreco', 'divCancelarTabelaPreco', 'divLimparTabelaPreco');
                    
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }

                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTabelaPreco, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosTabelaPreco').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_tabela_preco', 'selecionado', -1, 'selecionaTodosTabelaPreco', 'selecionaTodosTabelaPreco', 'gridTabelaPreco')", gridTabelaPreco.rowsPerPage * 3);
                    });
                });


                //*** Cria a grade de Histórico **\\

                var data = new Array();
                var gridHistorico = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) }),
                    structure:
                      [
                        { name: "Data", field: "dt_tabela", width: "15%", styles: "min-width:50px;" },
                        { name: "Parcelas", field: "nm_parcelas", width: "80px", styles: "text-align: center;" },
                        { name: "Unitário", field: "vl_parc", width: "80px", styles: "text-align: center;" },
                        { name: "Total", field: "vl_ttl", width: "80px", styles: "text-align: center;" },
                        { name: "Matricula", field: "vl_mat", width: "80px", styles: "text-align: center;" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["6", "12", "24", "100", "All"],
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
                }, "gridHistorico");
                //Paginação
                gridHistorico.pagination.plugin._paginator.plugin.connect(gridHistorico.pagination.plugin._paginator, 'onSwitchPageSize',
                    function (evt) {
                        verificaMostrarTodos(evt, gridHistorico, 'cd_tabela_preco', 'selecionaTodosTabelaPreco');
                    });

                gridHistorico.startup();


                //////
                //*** Cria os botões do link de ações **\\


                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridTabelaPreco, 'todosItensTabelaPreco', ['pesquisarTbPreco', 'relatorioTabelaPreco']);
                        PesquisarTabelaPreco(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridTabelaPreco', 'selecionadoTabelaPreco', 'cd_tabela_preco', 'selecionaTodosTabelaPreco', ['pesquisarTbPreco', 'relatorioTabelaPreco'], 'todosItensTabelaPreco'); }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensTabelaPreco",
                    dropDown: menu,
                    id: "todosItensTabelaPreco"
                });
                dom.byId("linkSelecionadosTabelaPreco").appendChild(button.domNode);

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridTabelaPreco.itensSelecionados, 'DeletarTabelaPreco(itensSelecionados)'); }
                });
                menu.addChild(acaoExcluir);

                acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridTabelaPreco.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTabelaPreco",
                    dropDown: menu,
                    id: "acoesRelacionadasTabelaPreco"
                });
                dom.byId("linkAcoesTabelaPreco").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar",
                    iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        IncluirTabelaPreco();
                    }
                }, "incluirTabelaPreco");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { AlterarTabelaPreco(); }
                }, "alterarTabelaPreco");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { DeletarTabelaPreco(); }
                }, "deleteTabelaPreco");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        limparTabelaPreco();
                    }
                }, "limparTabelaPreco");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValues(null, gridTabelaPreco, false);
                    }
                }, "cancelarTabelaPreco");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cad").hide();
                    }
                }, "fecharTabelaPreco");
                new Button({
                    label: "",
                    onClick: function () { PesquisarTabelaPreco(); },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarTbPreco");
                decreaseBtn(document.getElementById("pesquisarTbPreco"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            apresentaMensagem('apresentadorMensagem', null);
                            componentesNovaTurma(xhr, function () {
                                limparTabelaPreco();
                                dijit.byId("cd_curso").set('disabled', false);
                                dijit.byId("cd_duracao").set('disabled', false);
                                dijit.byId("cd_regime").set('disabled', false);
                                dijit.byId("dta_tabela_preco").set('disabled', false);
                                dijit.byId("nm_parcelas").set('disabled', false);
                                dijit.byId("vl_aula").set('disabled', false);
                                dijit.byId("vl_parcela").set('disabled', false);
                                dijit.byId("vl_matricula").set('disabled', false);
                                dijit.byId('tagHistorico').set('open', false);
                                dijit.byId("cad").show();
                                dijit.byId("cd_curso").set("value", 0);
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoTabelaPreco");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            var cbCurso = dijit.byId("cbCurso").get("value") == null || dijit.byId("cbCurso").get("value") == "" ? 0 : dijit.byId("cbCurso").get("value");
                            var cbRegime = dijit.byId("cbRegime").get("value") == null || dijit.byId("cbRegime").get("value") == "" ? 0 : dijit.byId("cbRegime").get("value");
                            var cbDuracao = dijit.byId("cbDuracao").get("value") == null || dijit.byId("cbDuracao").get("value") == "" ? 0 : dijit.byId("cbDuracao").get("value");
                            var dta_tabela = hasValue(dojo.byId("cbData").value) ? dojo.date.locale.parse(dojo.byId("cbData").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var codProduto = dijit.byId("codProduto").get("value") == null || dijit.byId("codProduto").get("value") == "" ? 0 : dijit.byId("codProduto").get("value");
                            xhr.get({
                                url: Endereco() + "/api/financeiro/geturlrelatorioTabelaPreco?" + getStrGridParameters('gridTabelaPreco') + "cdCurso=" + cbCurso + "&cdDuracao=" + cbDuracao + "&cdRegime=" + cbRegime + "&dtaCad=" + dojo.byId("cbData").value + "&codProduto=" + codProduto,//dta_tabela,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioTabelaPreco");

                dijit.byId("vl_parcela").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("vl_parcela").value) && hasValue(dijit.byId("nm_parcelas").value)) {
                                var val = dojo.byId("vl_parcela").value.replace(",", ".");
                                dojo.byId("vl_total").value = maskFixed(parseFloat(val) * parseInt(dojo.byId("nm_parcelas").value), 2);
                            }
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                })
                dijit.byId("nm_parcelas").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dojo.byId("vl_parcela").value) && hasValue(dijit.byId("nm_parcelas").value)) {
                                var val = dojo.byId("vl_parcela").value.replace(",", ".");
                                dojo.byId("vl_total").value = maskFixed(parseFloat(val) * parseInt(dojo.byId("nm_parcelas").value), 2);
                            }

                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                })


                dijit.byId("cd_curso").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("cd_curso").value) && hasValue(dijit.byId("cd_duracao").value))
                                retornaHistorico(dijit.byId("cd_curso").value, dijit.byId("cd_duracao").value, dijit.byId("cd_regime").value);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                })

                dijit.byId("cd_duracao").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("cd_curso").value) && hasValue(dijit.byId("cd_duracao").value))
                                retornaHistorico(dijit.byId("cd_curso").value, dijit.byId("cd_duracao").value, dijit.byId("cd_regime").value);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                })

                dijit.byId("cd_regime").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("cd_curso").value) && hasValue(dijit.byId("cd_duracao").value))
                                retornaHistorico(dijit.byId("cd_curso").value, dijit.byId("cd_duracao").value, dijit.byId("cd_regime").value);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                })
			dijit.byId("dta_tabela_preco").on("change", function (e) {
                var dataMin = new Date(1899, 12, 01);
                if (dojo.date.compare(dataMin, e) > 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDtaTabPrecoMin);
                    apresentaMensagem('apresentadorMensagemTabelaPreco', mensagensWeb);
                    return false;
                }
                else {
                    var dataMax = new Date(2079, 05, 06);
                    if (dojo.date.compare(e, dataMax) > 0) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDtaTabPrecoMax);
                        apresentaMensagem('apresentadorMensagemTabelaPreco', mensagensWeb);
                        return false;
                    }
                }
            })
                xhr.get({
                    url: Endereco() + "/api/Curso/getCursoTabelaPreco",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataCurso) {
                    try{
                        loadCursoTabela(jQuery.parseJSON(dataCurso).retorno, 'cbCurso');
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                   function (error) {
                       apresentaMensagem('apresentadorMensagem', error);
                   });
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getDuracaoTabelaPreco",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataDuracao) {
                    try{
                        loadDuracaoTabela(jQuery.parseJSON(dataDuracao).retorno, 'cbDuracao');
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                   function (error) {
                       apresentaMensagem('apresentadorMensagem', error);
                   });
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getRegimeTabelaPreco",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataRegime) {
                    try{
                        loadRegimeTabela(jQuery.parseJSON(dataRegime).retorno, 'cbRegime');
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                   function (error) {
                       apresentaMensagem('apresentadorMensagem', error);
                   });



                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getProdutoTabela",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataProdAtivo) {
                    try{
                        loadProduto(dataProdAtivo, 'codProduto');
                        dijit.byId("codProduto").set("value", 0);
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemTabelaPreco', error);
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323067', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['cbCurso', 'cbDuracao', 'cbData', 'cbRegime', 'codProduto'], 'pesquisarTbPreco', ready);

            } catch (e) {
                postGerarLog(e);
            }
        });
    })

};

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formTabelaPreco');
        	clearForm("formTabelaPreco");
            apresentaMensagem('apresentadorMensagem', '');
            componentesNovaTurma(dojo.xhr, function () {
                try{
                    keepValues(null, dijit.byId('gridTabelaPreco'), true);
                    dijit.byId("cad").show();
                    IncluirAlterar(0, 'divAlterarTabelaPreco', 'divIncluirTabelaPreco', 'divExcluirTabelaPreco', 'apresentadorMensagemTabelaPreco', 'divCancelarTabelaPreco', 'divLimparTabelaPreco');
           			dijit.byId("cd_curso").set('disabled', true);
		            dijit.byId("cd_duracao").set('disabled', true);
        		    dijit.byId("cd_regime").set('disabled', true);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function limparTabelaPreco() {
    require(["dojo/data/ObjectStore",
              "dojo/store/Memory"
    ], function (ObjectStore, Memory) {
        try{
            getLimpar('#formTabelaPreco');
            clearForm('cad');
            IncluirAlterar(1, 'divAlterarTabelaPreco', 'divIncluirTabelaPreco', 'divExcluirTabelaPreco', 'apresentadorMensagemTabelaPreco', 'divCancelarTabelaPreco', 'divLimparTabelaPreco');
            document.getElementById("cd_tabela_preco").value = 0;
            dijit.byId("cd_curso").reset();
            dijit.byId("cd_regime").reset();
            dijit.byId("cd_duracao").reset();
            document.getElementById("dta_tabela_preco").value = "";


            //dijit.byId("cd_tabela_preco").value = null;
            dijit.byId("cd_curso").value = null;
            dijit.byId("cd_regime").value = null;
            dijit.byId("cd_duracao").value = null;
            dijit.byId("dta_tabela_preco").value = null;
            //Limpa as informações da grade do Historico :
            var gridHistorico = dijit.byId('gridHistorico');

            if (hasValue(gridHistorico)) {
                var data = new Array();
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });
                gridHistorico.setStore(dataStore);
                gridHistorico.update();
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function EnviarDadosc() {
    try{
        if (document.getElementById("divAlterarTabelaPreco").style.display == "")
            AlterarTabelaPreco();
        else
            IncluirTabelaPreco();
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirTabelaPreco() {
    try{
        if (!dijit.byId("formTabelaPreco").validate()) 
            return false;
        var dta_tabela = hasValue(dojo.byId("dta_tabela_preco").value) ? dojo.date.locale.parse(dojo.byId("dta_tabela_preco").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        apresentaMensagem('apresentadorMensagemTabelaPreco', null);
        var cdRegime = dijit.byId("cd_regime").value == 0 ? null : dijit.byId("cd_regime").value;
        apresentaMensagem('apresentadorMensagem', null);
        showCarregando();
        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post(Endereco() + "/financeiro/postTabelaPreco", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_curso: dijit.byId("cd_curso").value,
                    cd_regime: cdRegime,
                    cd_duracao: dijit.byId("cd_duracao").value,
                    dta_tabela_preco: dta_tabela,
                    nm_parcelas: dijit.byId("nm_parcelas").value,
                    vl_parcela: dijit.byId("vl_parcela").value,
                    vl_matricula: dijit.byId("vl_matricula").value,
                    vl_aula: dijit.byId("vl_aula").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridTabelaPreco';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_tabela_preco", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionado', 'cd_tabela_preco', 'selecionaTodosTabelaPreco', ['pesquisarTbPreco', 'relatorioTabelaPreco'], 'todosItensTabelaPreco');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tabela_preco");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTabelaPreco', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTabelaPreco', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function loadCurso(data, linkCurso) {
    require(["dojo/store/Memory",  "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkCurso == 'cdCurso'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_curso, name: value.no_curso });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("cd_curso").store = stateStore;
		        dijit.byId("cd_curso").value = 0;
		        dijit.byId("cd_curso").set("required", true);
		    } catch (e) {
		        postGerarLog(e);
		    }
		})    
}
function loadCursoTabela(data, linkCurso) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var items = [];
		        if ((linkCurso == 'cbCurso'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_curso, name: value.no_curso });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkCurso).store = stateStore;
		        dijit.byId("cbCurso").set("value", 0);
		        dijit.byId("cbCurso").required = false;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadDuracao(data, linkDuracao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var items = [];
		        if ((linkDuracao == 'cdDuracao'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_duracao, name: value.dc_duracao });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("cd_duracao").store = stateStore;
		        dijit.byId("cd_duracao").set("value", 0);
		        dijit.byId("cd_duracao").set("required", true);
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadDuracaoTabela(data, linkDuracao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkDuracao == 'cbDuracao'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_duracao, name: value.dc_duracao });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("cbDuracao").store = stateStore;
		        dijit.byId("cbDuracao").set("value", 0);
		        dijit.byId("cbDuracao").required = false;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadRegime(data, linkRegime) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkRegime == 'cdRegime'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_regime, name: value.no_regime });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("cd_regime").store = stateStore;
		        dijit.byId("cd_regime").value = 0;
		        dijit.byId("cd_regime").required = false;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})   
}
function loadRegimeTabela(data, linkRegime) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkRegime == 'cbRegime'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_regime, name: value.no_regime });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId("cbRegime").store = stateStore;
		        dijit.byId("cbRegime").set("value", 0);
		        dijit.byId("cbRegime").required = false;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function PesquisarTabelaPreco(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            var cbCurso = dijit.byId("cbCurso").get("value") == null || dijit.byId("cbCurso").get("value") == "" ? 0 : dijit.byId("cbCurso").get("value");
            var codProduto = dijit.byId("codProduto").get("value") == null || dijit.byId("codProduto").get("value") == "" ? 0 : dijit.byId("codProduto").get("value");
            var cbRegime = dijit.byId("cbRegime").get("value") == null || dijit.byId("cbRegime").get("value") == "" ? 0 : dijit.byId("cbRegime").get("value");
            var cbDuracao = dijit.byId("cbDuracao").get("value") == null || dijit.byId("cbDuracao").get("value") == "" ? 0 : dijit.byId("cbDuracao").get("value");
            var dta_tabela = hasValue(dojo.byId("cbData").value) ? dojo.date.locale.parse(dojo.byId("cbData").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    handleAs: "json",
                    target: Endereco() + "/api/financeiro/GetTabelaPrecoSearch?cdCurso=" + cbCurso + "&cdDuracao=" + cbDuracao + "&cdRegime=" + cbRegime + "&dtaCad=" + dojo.byId("cbData").value + "&codProduto=" + codProduto,//dta_tabela,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTabelaPreco = dijit.byId("gridTabelaPreco");
            if (limparItens)
                gridTabelaPreco.itensSelecionados = [];
            gridTabelaPreco.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarTabelaPreco() {
    try{
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNaoAlteraTabelaPreço);
        apresentaMensagem("apresentadorMensagemTabelaPreco", mensagensWeb);
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarTabelaPreco(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tabela_preco').value != 0)
                    itensSelecionados = [{
                        cd_tabela_preco: dojo.byId("cd_tabela_preco").value,
                        cd_curso: dijit.byId("cd_curso").value,
                        cd_regime: dijit.byId("cd_regime").value,
                        cd_duracao: dijit.byId("cd_duracao").value,
                        dta_tabela_preco: dojo.byId("dta_tabela_preco").value,
                        nm_parcelas: dijit.byId("nm_parcelas").value,
                        vl_parcela: dijit.byId("vl_parcela").value,
                        vl_matricula: dijit.byId("vl_matricula").value
                    }];
            xhr.post({
                url: Endereco() + "/api/financeiro/postDeleteTabelaPreco",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        PesquisarTabelaPreco(true);

                        var todos = dojo.byId("todosItens_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        dijit.byId("pesquisarTbPreco").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridTabelaPreco').itensSelecionados, "cd_tabela_preco", itensSelecionados[r].cd_tabela_preco);


                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarTbPreco").set('disabled', false);
                        dijit.byId("relatorioTabelaPreco").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTabelaPreco', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioTabelaPreco").style.display))
                    apresentaMensagem('apresentadorMensagemTabelaPreco', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function componentesNovaTurma(xhr, pFuncao) {
    xhr.get({
        url: Endereco() + "/api/turma/componentesNovaTabelaPreco?cdDuracao=&cdProduto=&cdRegime=&cdCurso=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            if (hasValue(data.retorno.cursos))
                loadCurso(data.retorno.cursos, 'cd_curso');
            if (hasValue(data.retorno.duracoes))
                loadDuracao(data.retorno.duracoes, 'cd_duracao');
            if (hasValue(data.retorno.regimes))
                loadRegime(data.retorno.regimes, 'cd_regime');
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