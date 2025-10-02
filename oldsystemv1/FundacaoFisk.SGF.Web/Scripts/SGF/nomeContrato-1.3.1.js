var CADASTRO = 1, EDICAO = 2;
var masterGeral = false;

function montarNomeContrato() {
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
        "dojo/dom",
        "dijit/Tooltip",
         "dojox/json/ref"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, Tooltip,ref) {
        ready(function () {
            try {
                xhr.get({
                    url: Endereco() + "/api/escola/VerificarMasterGeral",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    masterGeral = data.retorno;
                    var myStore =
                       Cache(
                               JsonRest({
                                   target: Endereco() + "/api/secretaria/getSearchNomeContrato?desc=&layout&inicio=false&status=" + parseInt(1),
                                   handleAs: "json",
                                   preventCache: true,
                                   headers: { "Accept": "application/json", "Authorization": Token() }
                               }), Memory({}));

                    var gridNomeContrato = new EnhancedGrid({
                        store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                        structure:
                          [
                            { name: "<input id='selecionaTodosNoCont' style='display:none'/>", field: "selecionadoNoCont", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxNoCont },
                            { name: "Descrição", field: "no_contrato", width: "20%", styles: "min-width:80px;" },
                            { name: "Arquivo", field: "no_relatorio", width: "20%", styles: "min-width:80px;" },
                            { name: "Prev.Dias", field: "id_previsao_dias", width: "6%", styles: "text-align: center; min-width:15px;", formatter: formatCheckPrevDias },
                            { name: "Valor Hora", field: "id_valor_hora_aula", width: "8%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckValorHora },
                            { name: "Aditamento", field: "id_motivo_aditamento", width: "8%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckAditamento },
                            { name: "Tipo Pgto.", field: "id_tipo_pgto", width: "6%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckTipoPag },
                            { name: "Valor Material", field: "id_valor_material", width: "8%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckValorMaterial },
                            { name: "Reajuste Anual", field: "id_reajuste_anual", width: "6%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckValorReajuste },
                            { name: "Ativo", field: "id_nome_ativo", width: "6%", styles: "text-align: center; min-width:15px; max-width: 20px;", formatter: formatCheckAtivo },
                            { name: "Tipo", field: "tipoNomeContrato", width: "10%", styles: "text-align: center; min-width:15px; max-width: 20px;" }
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
                                /*page step to be displayed*/
                                maxPageStep: 5,
                                /*position of the pagination bar*/
                                position: "button"
                            }
                        }
                    }, "gridNomeContrato");
                    gridNomeContrato.canSort = function (col) { return Math.abs(col) != 1 };
                    gridNomeContrato.pagination.plugin._paginator.plugin.connect(gridNomeContrato.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                        verificaMostrarTodos(evt, gridNomeContrato, 'cd_nome_contrato', 'selecionaTodosNoCont');
                    });

                    require(["dojo/aspect"], function (aspect) {
                        aspect.after(gridNomeContrato, "_onFetchComplete", function () {
                            // Configura o check de todos:
                            if (dojo.byId('selecionaTodosNoCont').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_nome_contrato', 'selecionadoNoCont', -1, 'selecionaTodosNoCont', 'selecionaTodosNoCont', 'gridTurma')", gridNomeContrato.rowsPerPage * 3);
                        });
                    });
                    gridNomeContrato.on("RowDblClick", function (evt) {
                        try {
                            var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                            gridNomeContrato.itemSelecionado = item;
                            keepValues(item, gridNomeContrato, false);
                            IncluirAlterar(0, 'divAlterarNoCont', 'divIncluirNoCont', 'divExcluirNoCont', 'apresentadorMensagemNoCont', 'divCancelarNoCont', 'divClearNoCont');
                            dijit.byId("cadNomeCont").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }, true);
                    gridNomeContrato.startup();


                    //Crud Pessoa
                    new Button({
                        label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                            crudNomeContratoEArquivo(xhr, CADASTRO);
                        }
                    }, "incluirNoCont");
                    new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(null, gridNomeContrato, null); } }, "cancelarNoCont");
                    new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadNomeCont").hide(); } }, "fecharNoCont");
                    new Button({
                        label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                            crudNomeContratoEArquivo(xhr, EDICAO);
                        }
                    }, "alterarNoCont");
                    new Button({
                        label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                            caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarNomeContratos(gridNomeContrato.itensSelecionados, xhr); });
                        }
                    }, "deleteNoCont");
                    new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparNoCont(); } }, "limparNoCont");
                    //Fim

                    new Button({
                        label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                            apresentaMensagem('apresentadorMensagem', null);
                            pesquisarNoCont(true);
                        }
                    }, "pesquisarNoCont");
                    decreaseBtn(document.getElementById("pesquisarNoCont"), '32px');

                    new Button({
                        label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                limparNoCont();
                                IncluirAlterar(1, 'divAlterarNoCont', 'divIncluirNoCont', 'divExcluirNoCont', 'apresentadorMensagemNoCont', 'divCancelarNoCont', 'divClearNoCont');
                                dijit.byId("cadNomeCont").show();
                            }
                    }, "novaNoCont");
                    new Button({
                        label: "",
                        iconClass: 'dijitEditorIcon dijitEditorIconDownload',
                        onClick: function () { }
                    }, "btnDown");
                    // Adiciona link de ações:
                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () { eventoEditar(gridNomeContrato.itensSelecionados, xhr); }
                    });
                    menu.addChild(acaoEditar);

                    var acaoRemover = new MenuItem({
                        label: "Excluir",
                        onClick: function () { eventoRemoverNomesTurmas(gridNomeContrato.itensSelecionados, xhr); }
                    });
                    menu.addChild(acaoRemover);

                    if (!masterGeral) {
                        var acaoRemover = new MenuItem({
                            label: "Especializar",
                            onClick: function () { eventoEspecializarNomeContrato(gridNomeContrato.itensSelecionados); }
                        });
                        menu.addChild(acaoRemover);
                    }

                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadas",
                        dropDown: menu,
                        id: "acoesRelacionadas"
                    });
                    dom.byId("linkAcoesNoCont").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });

                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function () {
                            buscarTodosItens(gridNomeContrato, 'todosItens', ['pesquisarNoCont']);
                            pesquisarNoCont(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function () {
                            dijit.byId("statusNoCont").set("value", 0);
                            buscarItensSelecionados('gridNomeContrato', 'selecionadoNoCont', 'cd_nome_contrato', 'selecionaTodosNoCont', ['pesquisarNoCont'], 'todosItens');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItens",
                        dropDown: menu,
                        id: "todosItens"
                    });
                    dom.byId("linkSelecionadosNoCont").appendChild(button.domNode);

                    var buttonFkArray = ['uploader', 'btnDown'];

                    for (var p = 0; p < buttonFkArray.length; p++) {
                        var buttonFk = document.getElementById(buttonFkArray[p]);

                        if (hasValue(buttonFk)) {
                            buttonFk.parentNode.style.minWidth = '18px';
                            buttonFk.parentNode.style.width = '18px';
                        }
                    }
                    montarStatus("statusNoCont");
                    new Tooltip({
                        connectId: ["uploader"],
                        label: "Upload",
                        position: ['above']
                    });
                    new Tooltip({
                        connectId: ["btnDown"],
                        label: "Download",
                        position: ['above']
                    });
                    dijit.byId('uploader').on("change", function (evt) {
                        try {
                            var mensagensWeb = new Array();
                            var files = dijit.byId("uploader")._files;
                            if (hasValue(files) && files.length > 0) {
                                if (dijit.byId("uploader")._files[0].name.length > 128) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanho);
                                    apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
                                } else {
                                    if (hasValue(files[0]) && files[0].size > 500000) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivotExcedeuTamanho);
                                        apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
                                        return false;
                                    }
                                    var nomeArquivo = files[0].name;
                                    var extArquivo = nomeArquivo.substr(nomeArquivo.length - 4, nomeArquivo.length)
                                    if (hasValue(extArquivo) && extArquivo != "dotx") {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoLayoutInvalida);
                                        apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
                                        return false;
                                    }
                                    apresentaMensagem('apresentadorMensagemNoCont', null);
                                    dojo.byId("edlayout").value = files[0].name;
                                }
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId('btnDown').on("click", function (evt) {
                        try {
                            var noRelatorio = dojo.byId("edlayout").value;
                            var cdNomeContrato = dojo.byId("cd_nome_contrato").value;
                            if (hasValue(noRelatorio)) {
                                var url = Endereco() + "/secretaria/GetArquivoNomeDocumento?noRelatorio=" + noRelatorio + "&cdNomeContrato=" + dojo.byId("cd_nome_contrato").value;
                                window.open(url);
                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgAvisoNoContSemDocumento);
                                apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    if (hasValue(dijit.byId("menuManual"))) {
                        dijit.byId("menuManual").on("click",
                            function(e) {
                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323038', '765px', '771px');
                            });
                    }
                    adicionarAtalhoPesquisa(['descNoCont', 'layout', 'statusNoCont'], 'pesquisarNoCont', ready);
                    showCarregando();
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxNoCont(value, rowIndex, obj) {
    try {
        var gridName = 'gridNomeContrato';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosNoCont');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_nome_contrato", grid._by_idx[rowIndex].item.cd_nome_contrato);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:9px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_nome_contrato', 'selecionadoNoCont', -1, 'selecionaTodosNoCont', 'selecionaTodosNoCont', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_nome_contrato', 'selecionadoNoCont', " + rowIndex + ", '" + id + "', 'selecionaTodosNoCont', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarNoCont(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore =
                  Cache(
                          JsonRest({
                              target: Endereco() + "/api/secretaria/getSearchNomeContrato?desc=" + dojo.byId("descNoCont").value + "&layout=" + dojo.byId("layout").value + "&inicio=" + document.getElementById("inicioNoCont").checked + "&status=" + dijit.byId("statusNoCont").value,
                              handleAs: "json",
                              preventCache: true,
                              headers: { "Accept": "application/json", "Authorization": Token() }
                          }), Memory({}));

            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridNomeContrato");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoRemoverNomesTurmas(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarNomeContratos(itensSelecionados, xhr); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridNomeContrato = dijit.byId('gridNomeContrato');
            apresentaMensagem('apresentadorMensagem', null);
            gridNomeContrato.itemSelecionado = gridNomeContrato.itensSelecionados[0];
            keepValues(null, gridNomeContrato, true);
            IncluirAlterar(0, 'divAlterarNoCont', 'divIncluirNoCont', 'divExcluirNoCont', 'apresentadorMensagemNoCont', 'divCancelarNoCont', 'divClearNoCont');
            dijit.byId("cadNomeCont").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEspecializarNomeContrato(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var mensagensWeb = new Array();
            if (masterGeral)
                return false;
            IncluirAlterar(1, 'divAlterarNoCont', 'divIncluirNoCont', 'divExcluirNoCont', 'apresentadorMensagemNoCont', 'divCancelarNoCont', 'divClearNoCont');
            if (!hasValue(itensSelecionados[0].cd_pessoa_escola)) {
                limparNoCont();
                var cd_nome_contrato_pai = itensSelecionados[0].cd_nome_contrato;
                dojo.byId("cd_nome_contrato_pai").value = cd_nome_contrato_pai;
                apresentaMensagem('apresentadorMensagem', null);
                dijit.byId("cadNomeCont").show();
            } else {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Somente é possível especilizar layout de contrato com tipo 'Padrão'.");
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mountNomeContrato(relatorioTemporario) {
    try {
        var retorno = {
            cd_nome_contrato: dojo.byId("cd_nome_contrato").value,
            cd_nome_contrato_pai: hasValue(dojo.byId("cd_nome_contrato_pai").value) && dojo.byId("cd_nome_contrato_pai").value > 0 ? dojo.byId("cd_nome_contrato_pai").value : null,
            no_contrato: dojo.byId("noCont").value,
            no_relatorio: dojo.byId("edlayout").value,
            id_nome_ativo: dijit.byId("ckAtivo").get("checked"),
            id_previsao_dias: dijit.byId("ckPrevDias").get("checked"),
            id_valor_hora_aula: dijit.byId("ckValAula").get("checked"),
            id_motivo_aditamento: dijit.byId("ckAdit").get("checked"),
            id_tipo_pgto: dijit.byId("ckTipoPag").get("checked"),
            id_valor_material: dijit.byId("ckVlMaterial").get("checked"),
            id_reajuste_anual: dijit.byId("ckReajusteAnual").get("checked") ,
            relatorioTemporario: relatorioTemporario
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D
function limparNoCont() {
    try {
        dojo.byId("cd_nome_contrato").value = 0;
        dojo.byId("cd_nome_contrato_pai").value = 0;
        dojo.byId("des_url_arquivo").value = "";
        dijit.byId("uploader").reset();
        //dijit.byId().reset();
        clearForm("formNoCont");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink) {
    try {
        limparNoCont();
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

        if (value != null && value.cd_nome_contrato > 0) {
            dojo.byId("cd_nome_contrato").value = value.cd_nome_contrato;
            dojo.byId("noCont").value = value.no_contrato;
            dojo.byId("edlayout").value = value.no_relatorio;
            dijit.byId("ckAtivo").set("value", value.id_nome_ativo);
            dijit.byId("ckPrevDias").set("value", value.id_previsao_dias);
            dijit.byId("ckValAula").set("value", value.id_valor_hora_aula);
            dijit.byId("ckAdit").set("value", value.id_motivo_aditamento);
            dijit.byId("ckTipoPag").set("value", value.id_tipo_pgto);
            dijit.byId("ckVlMaterial").set("value", value.id_valor_material);
            dijit.byId("ckReajusteAnual").set("value", value.id_reajuste_anual);
            
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarNoCont() {
    try {
        var retorno = true;
        if (!dijit.byId("formNoCont").validate()) {
            retorno = false;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarNomeContrato(xhr, relatorioTemporario) {
    try {
        var noContrato = null;
        noContrato = mountNomeContrato(relatorioTemporario);
        showCarregando();

        xhr.post({
            url: Endereco() + "/api/secretaria/postInsertNomeContrato",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(noContrato)
        }).then(function (data) {
            try {
                if (!hasValue(jQuery.parseJSON(data).erro)) {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridNomeContrato';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_nome_contrato", itemAlterado);
                    dijit.byId("statusNoCont").set("value", 0);
                    buscarItensSelecionados(gridName, 'selecionaTodosNoCont', 'cd_nome_contrato', 'selecionaTodosNoCont', ['pesquisarNoCont'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_nome_contrato");
                    showCarregando();
                    dijit.byId("cadNomeCont").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemNoCont', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemNoCont', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarNomeContrato(xhr, relatorioTemporario) {
    try {
        var noContrato = null;
        noContrato = mountNomeContrato(relatorioTemporario);
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/secretaria/postUpdateNomeContrato",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(noContrato)
        }).then(function (data) {
            try {
                if (!hasValue(jQuery.parseJSON(data).erro)) {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridNomeContrato';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    removeObjSort(grid.itensSelecionados, "cd_nome_contrato", dojo.byId("cd_nome_contrato").value);
                    insertObjSort(grid.itensSelecionados, "cd_nome_contrato", itemAlterado);
                    dijit.byId("statusNoCont").set("value", 0);
                    buscarItensSelecionados(gridName, 'selecionaTodosNoCont', 'cd_nome_contrato', 'selecionaTodosNoCont', ['pesquisarNoCont'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_nome_contrato");
                    showCarregando();
                    dijit.byId("cadNomeCont").hide();
                }
                else {
                    apresentaMensagem('apresentadorMensagemNoCont', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemNoCont', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function crudNomeContratoEArquivo(xhr, tipoOperacao) {
    try {
        if (!validarNoCont()) {
            return false;
        }
        var files = dijit.byId("uploader")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("file" + i, files[i]);
                }
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/secretaria/UploadDocumento",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (results) {
                        try {
                            if (hasValue(results) && hasValue(results.indexOf) && results.indexOf('<') >= 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada3);
                                apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
                                return false;
                            }
                            if (hasValue(results) && !hasValue(results.erro)  ) {
                                if (tipoOperacao == CADASTRO)
                                    salvarNomeContrato(xhr, results);
                                else
                                    alterarNomeContrato(xhr, results);
                            } else
                                apresentaMensagem('apresentadorMensagemNoCont', results);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemNoCont', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
            }
        } else {
            if (tipoOperacao == CADASTRO)
                salvarNomeContrato(xhr, "");
            else
                alterarNomeContrato(xhr, "");
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarNomeContratos(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_nome_contrato').value != 0)
                itensSelecionados = [{
                    cd_nome_contrato: dojo.byId("cd_nome_contrato").value
                }];
        xhr.post({
            url: Endereco() + "/api/secretaria/postDeleteNomesContratos",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadNomeCont").hide();
                dijit.byId("descNoCont").set("value", '');
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridNomeContrato').itensSelecionados, "cd_nome_contrato", itensSelecionados[r].cd_nome_contrato);
                pesquisarNoCont(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarNoCont").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadNomeCont").style.display))
                apresentaMensagem('apresentadorMensagemNoCont', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatCheckPrevDias(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '_3';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckValorHora(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-4';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckAditamento(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-5';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckTipoPag(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-6';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckValorMaterial(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-7';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckValorReajuste(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-9';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatCheckAtivo(value, rowIndex, obj) {
    try {
        var gridNomeContrato = dijit.byId("gridNomeContrato");
        var icon;
        var id = obj.field + '_Selected_' + gridNomeContrato._by_idx[rowIndex].item.cd_nome_contrato + '-8';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaNomContrato(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxConsultaNomContrato(value, rowIndex, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                disabled: true,
                name: "checkBox",
                checked: value,
            }, id);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}