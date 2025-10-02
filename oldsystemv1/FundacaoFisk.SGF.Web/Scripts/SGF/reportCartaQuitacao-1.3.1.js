//#region Monta relatório
function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunoCartaQuitacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoAlunoCartaQuitacao', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarMetodosViewCarta() {
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
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, DropDownButton, DropDownMenu, MenuItem, ItemFileReadStore) {
        ready(function () {
            try {


                var urlSearch = "/api/Aluno/findAlunoCartaQuitacao?ano=" + dijit.byId('txAno').value +
                    "&cdPessoa=0";

                var myStore =
                    Cache(
                        JsonRest({
                            target: (Endereco().replace("Relatorio/", "") + urlSearch),
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));

                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridAlunoCartaQuitacao = new EnhancedGrid({
                    store: dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoAlunoCartaQuitacao", width: "5px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        //{ name: "Código", field: "cd_pessoa", width: "60px", styles: "text-align: right; min-width:60px; max-width:60px;" },
                        { name: "Nome", field: "no_responsavel", width: "55%", minwidth: "10%" },
                        { name: "Email", field: "dc_fone_mail", width: "25%", minwidth: "10%"  },
                        { name: "Cpf/Cnpj", field: "nm_cpfcnpj", width: "10%" },
                    ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
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
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridAlunoCartaQuitacao");
                gridAlunoCartaQuitacao.pagination.plugin._paginator.plugin.connect(gridAlunoCartaQuitacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridAlunoCartaQuitacao, 'cd_pessoa', 'selecionaTodos');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAlunoCartaQuitacao, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoAlunoCartaQuitacao', -1, 'selecionaTodos', 'selecionaTodos', 'gridAlunoCartaQuitacao')", gridAlunoCartaQuitacao.rowsPerPage * 3);
                    });
                });
                gridAlunoCartaQuitacao.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 7; };

                gridAlunoCartaQuitacao.startup();

                

               /* showCarregando();
                dojo.xhr.get({
                    url: (Endereco().replace("Relatorio/", "") + urlSearch),
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        
                        //data = jQuery.parseJSON(data);
                        if (hasValue(data))
                            gridAlunoCartaQuitacao.itensSelecionados = data;
                        gridAlunoCartaQuitacao.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
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
                });*/

                new Button({
	                label: "", iconClass: 'dijitEditorIconSearchSGF',
	                onClick: function () {
		                apresentaMensagem('apresentadorMensagem', null);
                        pesquisarAlunoCartaQuitacao();
	                }
                }, "pesquisarAlunoCartaQuitacao");



                //montarStatus("statusAluno");
                new Button({
                    label: 'Emitir Carta(s)', iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                new Button({
                    label: 'Emitir Carta(s)', iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio1");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFKTitulo();
                                    });
                                });
                            else
                                abrirPessoaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");
                document.getElementById("pesAluno").parentNode.style.minWidth = '18px';
                document.getElementById("pesAluno").parentNode.style.width = '18px';

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPes").value = 0;
                        dojo.byId("noAluno").value = "";
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                    }
                }, "limparAluno");
                if (hasValue(document.getElementById("limparAluno"))) {
                    document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAluno").parentNode.style.width = '40px';
                }

                dijit.byId("txAno").on("change", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            
                            dojo.xhr.get({
                                url: Endereco() + "/api/aluno/GetUrlRelatorioAlunosCartaQuitacao?" + getStrGridParameters('gridAlunoCartaQuitacao') + "ano=" +
                                    dijit.byId('txAno').value +
                                    "&cdPessoa=" +
                                    dojo.byId("cdAlunoPes").value ,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                            },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioCarta");

                //var menu = new DropDownMenu({ style: "height: 25px" });
                //var acaoEditar = new MenuItem({
                //    label: "Selecionar Todo Itens",
                //    onClick: function () { eventoSelecionarDeselecionarItens(true); }
                //});
                //menu.addChild(acaoEditar);

                //var acaoEditar = new MenuItem({
                //    label: "Deselecionar Todo Itens",
                //    onClick: function () { eventoSelecionarDeselecionarItens(false); }
                //});
                //menu.addChild(acaoEditar);

                

                //var button = new DropDownButton({
                //    label: "Ações Relacionadas",
                //    name: "acoesRelacionadas",
                //    dropDown: menu,
                //    id: "acoesRelacionadas"
                //});
                //dojo.byId("linkAcoes").appendChild(button.domNode);

                adicionarAtalhoPesquisa(['txAno',], 'pesquisarAlunoCartaQuitacao', ready);

            } catch (e) {
                postGerarLog(e);
            }
        });

    });
}


function eventoSelecionarDeselecionarItens(seleciona) {
    try {
        var grid = dijit.byId("gridAlunoCartaQuitacao");
        if (seleciona == true) {
            dijit.byId("selecionaTodos").set("checked", true);
        } else {
            
            dijit.byId("selecionaTodos").set("checked", false);
            
        }
    } catch (e) {
        postGerarLog(e);
    } 
}

function pesquisarAlunoCartaQuitacao() {
    try {

        require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            var urlSearch = "/api/Aluno/findAlunoCartaQuitacao?ano=" +
                dijit.byId('txAno').value +
                "&cdPessoa=" +
                dojo.byId("cdAlunoPes").value;
            var gridAlunoCartaQuitacao = dijit.byId("gridAlunoCartaQuitacao");


            var myStore =
                Cache(
                    JsonRest({
                        target: (Endereco().replace("Relatorio/", "") + urlSearch),
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
        
            gridAlunoCartaQuitacao.setStore(dataStore);



        });
        /*
        showCarregando();
        dojo.xhr.get({
            url: (Endereco().replace("Relatorio/", "") + urlSearch),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                debugger
                //data = jQuery.parseJSON(data);
                if (hasValue(data))
                    gridAlunoCartaQuitacao.itensSelecionados = data;
                gridAlunoCartaQuitacao.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
                gridAlunoCartaQuitacao.update();
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
            });*/
    }
    catch (e) {
        postGerarLog(e);
    }
}

function emitirRelatorio() {
    try {
        var grid = dijit.byId("gridAlunoCartaQuitacao");
        var cdsPessoas = dijit.byId("gridAlunoCartaQuitacao");

        if ((!hasValue(grid.itensSelecionados)) || (hasValue(grid.itensSelecionados && grid.itensSelecionados.length == 0))) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegSendEmailCartaQuitacao, null);
        } else if (!grid.itensSelecionados.every(function (current) { return hasValue(current.dc_fone_mail) })) {
            caixaDialogo(DIALOGO_CONFIRMAR, msgAlunosWithoutEmailRegister, function () {
                cdsPessoas = grid.itensSelecionados.map(function (item) { return item.cd_pessoa; }).join("|");

                apresentaMensagem('apresentadorMensagem', null);
                var url = Endereco() + "/relatorio/GerarCartas?ano=" + dijit.byId('txAno').value +
                    "&cdPessoa=" + (cdsPessoas + "");
                dojo.xhr.get({
                    url,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        window.open(data);
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
            });
        }
        else {
            cdsPessoas = grid.itensSelecionados.map(function (item) { return item.cd_pessoa; }).join("|");

            apresentaMensagem('apresentadorMensagem', null);
            var url = Endereco() + "/relatorio/GerarCartas?ano=" + dijit.byId('txAno').value +
                "&cdPessoa=" + (cdsPessoas + "");
            dojo.xhr.get({
                url,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    window.open(data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        }

    } catch (e) {
        postGerarLog(e);
    }

}

function abrirPessoaFK() {
    try {
        limparPesquisaPessoaFK();
        dijit.byId("tipoPessoaFK").set("value", 1);
        pesquisaPessoaFKTitulo();
        dijit.byId("proPessoa").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKTitulo() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function () {
                try {
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                "&sexo=" + dijit.byId("sexoPessoaFK").value + "&responsavel=true",
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
            });
        });
}

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
            dojo.byId("cdAlunoPes").value = gridPessoaSelec.itensSelecionados[0].cd_pessoa;
            dojo.byId("noAluno").value = gridPessoaSelec.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }

        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}
