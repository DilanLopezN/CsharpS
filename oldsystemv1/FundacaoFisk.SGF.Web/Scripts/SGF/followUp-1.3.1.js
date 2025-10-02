var NOMEUSUARIO = "", CODUSUARIOLOGADO = 0, TIPOFALSO = 2;

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridCadFollowUp';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_follow_up", grid._by_idx[rowIndex].item.cd_follow_up);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  style='height: 16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_follow_up', 'followUpSelecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_follow_up', 'followUpSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroFollowUp() {
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
    "dijit/form/FilteringSelect",
    "dojo/_base/array",
    "dojox/json/ref"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button, ready, on, ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, array,ref) {
        ready(function () {
            try {
                getUsuarioLogado();
                loadFiltroTipoFollowUpFK();
                loadFiltroLidoFollowUpFK();
                loadFiltroResolvidoFollowUpFK();
                findComponentesFiltroFollowUp();
                if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                    montaFollowUpFK(function () {
                        setarEventosBotoesPrincipaisCadFollowUpFK(on);
                        dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                            if (e)
                                if (hasValue(dijit.byId("cadTipoFollowUpFK").value) && dijit.byId("cadTipoFollowUpFK").value > 0 && (
                                    parseInt(dijit.byId("cadTipoFollowUpFK").value) == PROSPECTALUNO) || parseInt(dijit.byId("cadTipoFollowUpFK").value) == ADMINISTRACAOGERAL)
                                    dijit.byId("ckLidoFollowUpFK").set("checked", true);
                            montarLayoutPorTipoFollowUp()
                        });
                    });
                var myStore =
               Cache(
                       JsonRest({
                           target: Endereco() + "/api/secretaria/getFollowUpSearch?id_tipo_follow=0&cd_usuario_org=0&cd_usuario_destino=0&cd_prospect=0&cd_aluno=0" +
                                                "&cd_acao=0&resolvido=2&lido=2&data=true&proximo_contato=false&dtInicial=&dtFinal=&id_usuario_adm=false",
                           handleAs: "json",
                           preventCache: true,
                           headers: { "Accept": "application/json", "Authorization": Token() }
                       }), Memory({}));
                var gridCadFollowUp = new EnhancedGrid({
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "followUpSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        { name: "Tipo", field: "desc_tipo", width: "14%", styles: "min-width:80px;" },
						{ name: "Usuário de Origem", field: "no_usuario_origem", width: "22%", styles: "min-width:80px;" },
						{ name: "Usuário de Destino", field: "no_usuario_destino", width: "22%", styles: "min-width:80px;" },
                        { name: "Prospect/Aluno", field: "no_prospect_aluno", width: "22%", styles: "min-width:80px;" },
						{ name: "Data", field: "dta_data", width: "13%", styles: "min-width:80px;" },
						{ name: "Data Próximo", field: "dta_proximo_contato", width: "10%", styles: "min-width:80px;" },
						{ name: "Lido", field: "lido", width: "7%", styles: "text-align:center;min-width:80px;" },
						{ name: "Resolvido", field: "resolvido", width: "7%", styles: "text-align:center;min-width:80px;" }
                    ],
                    canSort: true,
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
                            position: "button"
                        }
                    }
                }, "gridCadFollowUp");
                gridCadFollowUp.startup();
                gridCadFollowUp.layout.setColumnVisibility(4, true);
                gridCadFollowUp.pagination.plugin._paginator.plugin.connect(gridCadFollowUp.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridCadFollowUp, 'cd_follow_up', 'selecionaTodos');
                });

                gridCadFollowUp.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        var gridCadFollowUp = dijit.byId('gridCadFollowUp');

                        gridCadFollowUp.itemSelecionado = item;
                        destroyCreateGridEscolasFollowUpPartial();
                        keepValuesFollowUpPartial(gridCadFollowUp,null,false);
                        IncluirAlterar(0, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial',
                                          'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
                        dijit.byId("cadFollowUp").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridCadFollowUp.itensSelecionados = new Array();
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridCadFollowUp, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_follow_up', 'followUpSelecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridCadFollowUp')", gridCadFollowUp.rowsPerPage * 3);
                    });
                });
                gridCadFollowUp.canSort = function (col) { return Math.abs(col) != 1};

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    abrirUsuarioFKFollowUp(PESQUSERORIGFOLLOWUP);
                                });
                            else
                                abrirUsuarioFKFollowUp(PESQUSERORIGFOLLOWUP);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKUsuarioOrigemPesq");
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dijit.byId("usuarioOrgPesq").reset();
                            dojo.byId("cdUsuarioOrgPesq").value = 0;
                            dijit.byId("limparUsuarioOrigemPesq").set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparUsuarioOrigemPesq");
                decreaseBtn(document.getElementById("limparUsuarioOrigemPesq"), '40px');
                decreaseBtn(document.getElementById("FKUsuarioOrigemPesq"), '18px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    //dojo.query("#nomPessoaFK").on("keyup", function (e) {
                                    //    if (e.keyCode == 13) pesquisarUsuarioFK();
                                    //});
                                    abrirUsuarioFKFollowUp(PESQUSERDESTINOFOLLOWUP);
                                });
                            else
                                abrirUsuarioFKFollowUp(PESQUSERDESTINOFOLLOWUP);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKUsuarioDestinoPesq");
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dijit.byId("usuarioDestinoPesq").reset();
                            dojo.byId("cdUsuarioDestinoPesq").value = 0;
                            dijit.byId("limparUsuDestinoPesq").set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparUsuDestinoPesq");
                decreaseBtn(document.getElementById("limparUsuDestinoPesq"), '40px');
                decreaseBtn(document.getElementById("FKUsuarioDestinoPesq"), '18px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("fecharProspectFK"))) {
                                montarGridPesquisaProspect(true, function () {
                                    abrirFKProspectFollowUp();
                                    //dojo.query("#nomeProspectFK").on("keyup", function (e) {
                                    //    if (e.keyCode == 13) pesquisarProspectFK();
                                    //});
                                    dijit.byId("fecharProspectFK").on("click", function (e) {
                                        dijit.byId("cadProspectFollowUpFK").hide();
                                    });
                                },true);
                            }
                            else
                                abrirFKProspectFollowUp();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKProspectAlunoPesq");
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dijit.byId("prospectAlunoPesq").reset();
                            dojo.byId("cdProspectPesq").value = 0;
                            dojo.byId("cdAlunoPesq").value = 0;
                            dijit.byId("limparProspectAlunoPesq").set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparProspectAlunoPesq");
                decreaseBtn(document.getElementById("limparProspectAlunoPesq"), '40px');
                decreaseBtn(document.getElementById("FKProspectAlunoPesq"), '18px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    abrirUsuarioFKFollowUp(PESQUSERORIGMASTERFOLLOWUP);
                                });
                            else
                                abrirUsuarioFKFollowUp(PESQUSERORIGMASTERFOLLOWUP);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKUsuarioOrgAdmPesq");
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dijit.byId("usuarioOrgAdmPesq").reset();
                            dojo.byId("cdUsuarioOrgAdmPesq").value = 0;
                            dijit.byId("limparFKUsuarioOrgAdmPesq").set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparFKUsuarioOrgAdmPesq");
                decreaseBtn(document.getElementById("FKUsuarioOrgAdmPesq"), '18px');
                decreaseBtn(document.getElementById("limparFKUsuarioOrgAdmPesq"), '40px');
                //Botões de ações FollowUp
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarFollowUp(true); } }, "pesquisarFollowUp");
                decreaseBtn(document.getElementById("pesquisarFollowUp"), '32px');
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        try {
                            var cdUsuarioOrgPesq = hasValue(dojo.byId("cdUsuarioOrgPesq").value) ? dojo.byId("cdUsuarioOrgPesq").value : 0;
                            var cdUsuarioDestinoPesq = hasValue(dojo.byId("cdUsuarioDestinoPesq").value) ? dojo.byId("cdUsuarioDestinoPesq").value : 0;
                            var cdUsuarioOrgAdmPesq = hasValue(dojo.byId("cdUsuarioOrgAdmPesq").value) ? dojo.byId("cdUsuarioOrgAdmPesq").value : 0;
                            var cdProspectPesq = hasValue(dojo.byId("cdProspectPesq").value) ? dojo.byId("cdProspectPesq").value : 0;
                            var cdAlunoPesq = hasValue(dojo.byId("cdAlunoPesq").value) ? dojo.byId("cdAlunoPesq").value : 0;
                            var tipoFollowUpPesq = hasValue(dijit.byId("tipoFollowUpPesq").value) ? dijit.byId("tipoFollowUpPesq").value : 0;
                            var cdAcaoPesq = hasValue(dijit.byId("acaoPesq").value) ? dijit.byId("acaoPesq").value : 0;
                            var idAdmin = false;
                            var resolvidoPesq = hasValue(dijit.byId("resolvidoPesq").value) ? dijit.byId("resolvidoPesq").value : 0;
                            var lidoPesq = hasValue(dijit.byId("lidoPesq").value) ? dijit.byId("lidoPesq").value : 0;
                            xhr.get({
                                url: Endereco() + "/api/secretaria/geturlrelatorioFollowUp?" + getStrGridParameters('gridCadFollowUp') + "id_tipo_follow=" + tipoFollowUpPesq + "&cd_usuario_org=" +
                                    cdUsuarioOrgPesq + "&cd_usuario_destino=" + cdUsuarioDestinoPesq + "&cd_prospect=" + cdProspectPesq + "&cd_aluno=" + cdAlunoPesq + "&cd_acao=" + cdAcaoPesq +
                                                  "&resolvido=" + resolvidoPesq + "&lido=" + lidoPesq +
                                                  "&data=" + document.getElementById("ckDataPesq").checked + "&proximo_contato=" + document.getElementById("ckProximoPesq").checked +
                                                  "&dtInicial=" + dojo.byId("dtaInicialPesq").value + "&dtFinal=" + dojo.byId("dtaFinalPesq").value + "&id_usuario_adm=" + idAdmin,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioFollowUp");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                novoFollowUpPartial(function () {
                                    try {
                                        //limparCadFollowUpPartial(false);
                                        dijit.byId("cadTipoFollowUpFK").reset();
                                        loadCadTipoFollowUpFK(INTERNO, false);
                                        dijit.byId("cadTipoFollowUpFK").set("disabled", false);
                                        dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                                        dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("value", eval(LoginUsuario()));
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                });
                            }
                }, "novoFollowUp");

                // Adiciona link de ações:
                var menuFollowUpFK = new DropDownMenu({ style: "height: 25px" });
                var acaoEditarFollowUpFK = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarFollowUpPartial(gridCadFollowUp.itensSelecionados); }
                });
                menuFollowUpFK.addChild(acaoEditarFollowUpFK);

                var acaoRemoverFollowUpFK = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverFollowUps(gridCadFollowUp.itensSelecionados); }
                });
                menuFollowUpFK.addChild(acaoRemoverFollowUpFK);

                var acaoResponderFollowUp = new MenuItem({
                    label: "Responder",
                    onClick: function () { responderFollowUp(dijit.byId('gridCadFollowUp').itensSelecionados); }
                });
                menuFollowUpFK.addChild(acaoResponderFollowUp);

                var buttonFollowUpFK = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menuFollowUpFK,
                    id: "acoesRelacionadasFollowUpFK"
                });
                dom.byId("linkAcoesFollowUpFK").appendChild(buttonFollowUpFK.domNode);
                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridCadFollowUp, 'todosItens', ['pesquisarFollowUp', 'relatorioFollowUp']);
                        pesquisarFollowUp(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridCadFollowUp', 'followUpSelecionado', 'cd_follow_up', 'selecionaTodos', ['pesquisarFollowUp', 'relatorioFollowUp'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionadosFollowUp").appendChild(button.domNode);

                dijit.byId("tipoFollowUpPesq").on("change", function (cdFiltroTipo) {
                    try {
                        dijit.byId('tagPesqGeral').set('open', true);
                        if (hasValue(cdFiltroTipo))
                            switch (parseInt(cdFiltroTipo)) {
                                case TODOS:
                                    dojo.byId('tgPesqInterno').style.display = 'block';
                                    dojo.byId('tgPesqAluno').style.display = 'block';
                                    dojo.byId('tgPesqMaster').style.display = 'block';
                                    dijit.byId('tgPesqInterno').set('open', true);
                                    dijit.byId('tgPesqAluno').set('open', false);
                                    dijit.byId('tgPesqMaster').set('open', false);
                                    break
                                case INTERNO:
                                    dojo.byId('tgPesqInterno').style.display = 'block';
                                    dijit.byId('tgPesqInterno').set('open', true);
                                    dojo.byId('tgPesqAluno').style.display = 'none';
                                    dojo.byId('tgPesqMaster').style.display = 'none';
                                    break
                                case PROSPECTALUNO:
                                    dojo.byId('tgPesqInterno').style.display = 'none';
                                    dojo.byId('tgPesqAluno').style.display = 'block';
                                    dijit.byId('tgPesqAluno').set('open', true);
                                    dojo.byId('tgPesqMaster').style.display = 'none';
                                    break
                                case ADMINISTRACAOGERAL:
                                    dojo.byId('tgPesqInterno').style.display = 'none';
                                    dojo.byId('tgPesqAluno').style.display = 'none';
                                    dojo.byId('tgPesqMaster').style.display = 'block';
                                    dijit.byId('tgPesqMaster').set('open', true);
                                    break
                            }
                        else
                            dijit.byId("tipoFollowUpPesq").set("value", Todos);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("resolvidoPesq").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < 0)
                            dijit.byId("resolvidoPesq").set("value", TIPOFALSO);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("lidoPesq").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < 0)
                            dijit.byId("lidoPesq").set("value", TIPOFALSO);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                resolvidoPesq 
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function responderFollowUp(itensSelecionados) {
    try {
        apresentaMensagem('apresentadorMensagem', '');
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgErroSelecionarRegistroResponder, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgErroSelecionarApenasRegistroResponder, null);
        else {
            var gridCadFollowUp = dijit.byId('gridCadFollowUp');

            gridCadFollowUp.itemSelecionado = itensSelecionados[0];
            destroyCreateGridEscolasFollowUpPartial();
            keepValuesFollowUpPartial(gridCadFollowUp, function () {
                try {
                    if (verificaFollowInternoParaResponder()) {
                        IncluirAlterar(1, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial',
                          'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
                        dijit.byId("mensagemFollowUppartial").set('disabled', false);
                        dijit.byId("cadFollowUp").show();

                        //Prepara a tela para responder:
                        dijit.byId("FKUsuarioDestinoCad").set('disabled', true);
                        dijit.byId("cadTipoFollowUpFK").set('disabled', true);
                        dijit.byId("ckResolvidoFollowUpFK").set('disabled', true);
                        showP('trInternoAdmFollowUpFK', false);
                        dijit.byId('cadFollowUp').set('title', 'Cadastro de Follow-Up (Resposta)');

                        alterarVisibilidadeEscolas(false);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function trocaUsuarioDestinoOrigem() {
    try {
        var cadNomeUsuarioOrigFollowUpFK = dojo.byId('cadNomeUsuarioOrigFollowUpFK').value;
        dojo.byId('cadNomeUsuarioOrigFollowUpFK').value = dojo.byId('cadNomeUsuarioDestinoFollowUpFK').value;
        dojo.byId('cadNomeUsuarioDestinoFollowUpFK').value = cadNomeUsuarioOrigFollowUpFK;

        var cdUsuarioDestinoFollowUpFK = dojo.byId('cdUsuarioDestinoFollowUpFK').value;
        dojo.byId('cdUsuarioDestinoFollowUpFK').value = dojo.byId('cdUsuarioOrigFollowUpFK').value;
        dojo.byId('cdUsuarioOrigFollowUpFK').value = cdUsuarioDestinoFollowUpFK;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaFollowInternoParaResponder() {
    if (dijit.byId('cadTipoFollowUpFK').value != INTERNO) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFollowInternoRespondido);
        apresentaMensagem('apresentadorMensagem', mensagensWeb);
        return false;
    }

    //Verifica o usuário de origem, uma vez que o de destino foi trocado com o de origem:
    if (!hasValue(dojo.byId("cdUsuarioOrigFollowUpFK").value)) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFollowUsuarioDestinoNaoInformado);
        apresentaMensagem('apresentadorMensagem', mensagensWeb);
        return false;
    }
    return true;
}

function getUsuarioLogado() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/escola/getnomeusuario",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var retorno = jQuery.parseJSON(data).retorno;
                NOMEUSUARIO = retorno;
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findComponentesFiltroFollowUp() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/componentesPesquisaFollowUp",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = hasValue(data) && hasValue(data.retorno) ? data.retorno : null;
                if(data != null)
                    CODUSUARIOLOGADO = data.cd_usuario;
                if (data != null && data.acaoeFollowUp != null)
                    criarOuCarregarCompFiltering("acaoPesq", data.acaoeFollowUp, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_acao_follow_up', 'dc_acao_follow_up', FEMININO);
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
        
function setarEventosBotoesPrincipaisCadFollowUpFK(on) {
    try {
        dijit.byId("btnIncluirFollowUpPartial").on("click", function () {
            try {
                apresentaMensagem("apresentadorMensagemCadFollowUpPartial", null);
                salvarFollowUpPartial();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("alterarFollowUpPartial").on("click", function () {
            try {
                apresentaMensagem("apresentadorMensagemCadFollowUpPartial", null);
                alterarFollowUpPartial();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("btnLimparFollowUpFK").on("click", function () {
            try {
                dijit.byId("cadTipoFollowUpFK").reset();
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("value", eval(LoginUsuario()));
                //dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("click", true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("cancelarFollowUpPartial").on("click", function () {
            try {
                keepValuesFollowUpPartial(dijit.byId("gridCadFollowUp"), function () {
                    //dijit.byId("cadTipoFollowUpFK").reset();
                    dijit.byId("cadTipoFollowUpFK").set("disabled", true);
                },false);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("deleteFollowUpPartial").on("click", function () {
            try {
                apresentaMensagem('apresentadorMensagemDiarioPartial', null);
                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarFollowUpsPartial(dijit.byId("gridCadFollowUp").itensSelecionados) });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        dijit.byId("btnFecharFollowUpPartial").on("click", function () {
            dijit.byId("cadFollowUp").hide();
        });
        dijit.byId("cadTipoFollowUpFK").on("change", function (cdTipo) {
            dijit.byId('panePesqGeralFollowUpFK').set('open', true);
            if (hasValue(cdTipo))
                montarLayoutPorTipoFollowUp();
            else
                dijit.byId("cadTipoFollowUpFK").set("value", INTERNO);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFiltroTipoFollowUpFK() {
    try {
        var tipoFollowUpFK = dijit.byId("tipoFollowUpPesq");
        var storeTipo = new dojo.store.Memory({
            data: [
              { name: "Todos", id: "0" },
              { name: "Interno", id: "1" },
              { name: "Prospect/Aluno", id: "2" },
              { name: "Administração Geral", id: "3" }
            ]
        });
        tipoFollowUpFK.store = storeTipo;
        tipoFollowUpFK.set("value", TODOS);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFiltroResolvidoFollowUpFK() {
    try {
        var resolvidoPesq = dijit.byId("resolvidoPesq");
        var storeTipo = new dojo.store.Memory({
            data: [
              { name: "Todos", id: "0" },
              { name: "Sim", id: "1" },
              { name: "Não", id: "2" }
            ]
        });
        resolvidoPesq.store = storeTipo;
        resolvidoPesq.set("value", TIPOFALSO);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFiltroLidoFollowUpFK() {
    try {
        var lidoPesq = dijit.byId("lidoPesq");
        var storeTipo = new dojo.store.Memory({
            data: [
              { name: "Todos", id: "0" },
              { name: "Sim", id: "1" },
              { name: "Não", id: "2" }
            ]
        });
        lidoPesq.store = storeTipo;
        lidoPesq.set("value", TIPOFALSO);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarLayoutPorTipoFollowUp() {
    try {
        var edicao = false;
        if (hasValue(dojo.byId("cd_follow_up_partial").value) && dojo.byId("cd_follow_up_partial").value > 0)
            edicao = true;
        if (edicao)
            dijit.byId("cadTipoFollowUpFK").set("disabled", true);
        else
            dijit.byId("cadTipoFollowUpFK").set("disabled", false);
        switch (parseInt(dijit.byId("cadTipoFollowUpFK").value)) {
            case INTERNO:
                dojo.byId('trInternoUserFollowUpFK').style.display = '';
                dojo.byId('trProspectAlunoFollowUpFK').style.display = 'none';
                dojo.byId('trProspectAlunoUsuarioDestFollowUpFK').style.display = 'none';
                dojo.byId('trEmailTelefoneProspectAluno').style.display = 'none';
                dojo.byId('trProximoContatoFollowUpFK').style.display = 'none';
                dojo.byId('trMasterFollowUp').style.display = 'none';
                dojo.byId('trResolvidoLidoFollowUp').style.display = '';
                dijit.byId("ckResolvidoFollowUpFK").set("disabled", true);
                dijit.byId("ckLidoFollowUpFK").set("disabled", true);
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", true);
                dijit.byId("descAlunoProspectFollowUpFK").set("required", false);
                dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", false);
                dojo.byId("tdQtdUserLido").style.display = "none";
                if ((!eval(Master()) || (eval(Master()) && eval(MasterGeral())))) {
                    dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
                    dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", true);
                    alterarVisibilidadeEscolas(false);
                } else {
                    alterarVisibilidadeEscolas(true);
                    dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);
                    dojo.byId('trInternoAdmFollowUpFK').style.display = '';
                }
                if (edicao) {
                    if (dijit.byId("ckLidoFollowUpFK").get("checked")) {
                        dijit.byId('mensagemFollowUppartial').set('disabled', true);
                        dijit.byId("FKUsuarioDestinoCad").set("disabled", true);
                        dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                        dijit.byId("incluirEscolaFKFollowUpFK").set("disabled", true);
                        if (!hasValue(dojo.byId("cdUsuarioDestinoFollowUpFK").value) || dojo.byId("cdUsuarioDestinoFollowUpFK").value == 0) {
                            dojo.byId("tdQtdUserLido").style.display = "";
                            dijit.byId("ckResolvidoFollowUpFK").set("disabled", false);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", false);
                            alterarVisibilidadeEscolas(false);
                        } else {
                            alterarVisibilidadeEscolas(true);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", true);
                            dijit.byId("ckResolvidoFollowUpFK").set("disabled", false);
                        }
                        if (parseInt(dojo.byId("cdUsuarioOrigFollowUpFK").value) != CODUSUARIOLOGADO) {
                            dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                            dijit.byId("ckLidoFollowUpFK").set("disabled", false);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", true);
                            if (!hasValue(dojo.byId("cdUsuarioDestinoFollowUpFK").value) || dojo.byId("cdUsuarioDestinoFollowUpFK").value == 0)
                                dijit.byId("ckResolvidoFollowUpFK").set("disabled", true);
                            dojo.byId("tdQtdUserLido").style.display = "none";
                            alterarVisibilidadeEscolas(false);
                        }
                    } else {
                        dijit.byId('mensagemFollowUppartial').set('disabled', false);
                        if (hasValue(dojo.byId("cdUsuarioDestinoFollowUpFK").value) && dojo.byId("cdUsuarioDestinoFollowUpFK").value > 0) {
                            dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", false);
                            alterarVisibilidadeEscolas(false);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", true);
                            if (dojo.byId("cdUsuarioDestinoFollowUpFK").value == CODUSUARIOLOGADO)
                                dijit.byId("ckResolvidoFollowUpFK").set("disabled", false);
                        } else {
                            dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                            alterarVisibilidadeEscolas(true);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", false);
                            dijit.byId("ckResolvidoFollowUpFK").set("disabled", false);
                        }
                        dijit.byId("FKUsuarioDestinoCad").set("disabled", false);
                        dijit.byId("incluirEscolaFKFollowUpFK").set("disabled", false);
                        if (parseInt(dojo.byId("cdUsuarioOrigFollowUpFK").value) != CODUSUARIOLOGADO) {
                            dijit.byId("FKUsuarioDestinoCad").set("disabled", true);
                            dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                            dijit.byId("ckLidoFollowUpFK").set("disabled", false);
                            dijit.byId("ckAdmFollowUpFK").set("disabled", true);
                            dojo.byId("tdQtdUserLido").style.display = "none";
                            if (!hasValue(dojo.byId("cdUsuarioDestinoFollowUpFK").value) || dojo.byId("cdUsuarioDestinoFollowUpFK").value == 0)
                                dijit.byId("ckResolvidoFollowUpFK").set("disabled", true);
                            alterarVisibilidadeEscolas(false);
                            dijit.byId('mensagemFollowUppartial').set('disabled', true);
                        } else
                            dojo.byId("tdQtdUserLido").style.display = "";
                    }
                } else {
                    dijit.byId("limparCadUsuarioDestinoFollowUpFK").set("disabled", true);
                    dijit.byId("FKUsuarioDestinoCad").set("disabled", false);
                    dijit.byId("ckAdmFollowUpFK").set("disabled", false);
                    dijit.byId('mensagemFollowUppartial').set('disabled', false);
                }
                break;
            case PROSPECTALUNO:
                dijit.byId('ckLidoFollowUpFK').set('disabled', true)
                dojo.byId('trInternoUserFollowUpFK').style.display = 'none';
                dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
                dojo.byId('trProspectAlunoFollowUpFK').style.display = '';
                dojo.byId('trProspectAlunoUsuarioDestFollowUpFK').style.display = '';
                dojo.byId('trEmailTelefoneProspectAluno').style.display = '';
                dojo.byId('trProximoContatoFollowUpFK').style.display = '';
                dojo.byId('trMasterFollowUp').style.display = 'none';
                dojo.byId('trResolvidoLidoFollowUp').style.display = '';
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", false);
                dijit.byId("descAlunoProspectFollowUpFK").set("required", true);
                dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", false);
                alterarVisibilidadeEscolas(false);
                dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);
                if (edicao) {
                    if (dijit.byId("ckResolvidoFollowUpFK").checked) {
                        dijit.byId('cadAcaoFollowUpFK').set('disabled', true);
                        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', true);
                        dijit.byId('mensagemFollowUppartial').set('disabled', true);
                        dijit.byId('btnAlunoProspectFK').set('disabled', true);
                        dijit.byId('mensagemFollowUppartial').set('disabled', true);

                    } else {
                        dijit.byId('cadAcaoFollowUpFK').set('disabled', false);
                        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', false);
                        dijit.byId('mensagemFollowUppartial').set('disabled', false);
                        dijit.byId('btnAlunoProspectFK').set('disabled', false);
                        dijit.byId('mensagemFollowUppartial').set('disabled', false);
                        dijit.byId("ckResolvidoFollowUpFK").set("disabled", false);
                    }
                }
                break
            case ADMINISTRACAOGERAL:
                dojo.byId('trInternoUserFollowUpFK').style.display = 'none';
                dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
                dojo.byId('trProspectAlunoFollowUpFK').style.display = 'none';
                dojo.byId('trProspectAlunoUsuarioDestFollowUpFK').style.display = 'none';
                dojo.byId('trEmailTelefoneProspectAluno').style.display = 'none';
                dojo.byId('trProximoContatoFollowUpFK').style.display = 'none';
                dojo.byId('trMasterFollowUp').style.display = '';
                dojo.byId('trResolvidoLidoFollowUp').style.display = '';
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", false);
                dijit.byId("descAlunoProspectFollowUpFK").set("required", false);
                dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", true);
                if (eval(MasterGeral()))
                    alterarVisibilidadeEscolas(true);
                dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);
                if (edicao) {
                    if (parseInt(dojo.byId("cdUsuarioOrigFollowUpFK").value) != CODUSUARIOLOGADO) {
                        dijit.byId('ckAdm').set('disabled', true);
                        dijit.byId('ckResolvidoFollowUpFK').set('disabled', true);
                        if (dijit.byId('ckResolvidoFollowUpFK').checked)
                            dijit.byId('ckLidoFollowUpFK').set('disabled', true);
                        else
                            dijit.byId('ckLidoFollowUpFK').set('disabled', false);
                        dijit.byId('mensagemFollowUppartial').set('disabled', true);
                    } else {
                        dijit.byId('ckResolvidoFollowUpFK').set('disabled', false);
                        dojo.byId("tdQtdUserLido").style.display = "";
                        if (dijit.byId("ckLidoFollowUpFK").checked) {
                            dijit.byId('ckAdm').set('disabled', true);
                            dijit.byId('mensagemFollowUppartial').set('disabled', true);
                        } else {
                            dijit.byId('ckAdm').set('disabled', false);
                            dijit.byId('mensagemFollowUppartial').set('disabled', false);
                        }
                    }
                }
                break
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverFollowUps(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarFollowUpsPartial(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Funções de criação das FK's.

function abrirFKProspectFollowUp() {
    try {
        limparPesquisaProspectFK();
        dojo.byId("cadProspectFollowUpFK_title").innerHTML = "Pesquisa de Prospect/Aluno";
        dojo.byId("idOrigemProspectFK").value = ORIGPESQFOLLOWUP;
        dojo.byId("trTipoPesquisaFKProspect").style.display = "";
        dojo.byId("panelProspectFK").style.height = '390px';
        dojo.byId("paiPanelProspectFK").style.height = '395px';
        pesquisarProspectFKOrigemFollowUp(true);
        dijit.byId("cadProspectFollowUpFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarProspectFKOrigemFollowUp() { //selecionaProspectFK
    try {
        var valido = true;
        var gridPesquisaProspect = dijit.byId("gridPesquisaProspect");

        if (!hasValue(gridPesquisaProspect.itensSelecionados) || gridPesquisaProspect.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (gridPesquisaProspect.itensSelecionados.length > 1) {
                caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                valido = false;
            }
            else {
                if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect) || hasValue(gridPesquisaProspect.itensSelecionados[0].cd_aluno)) {
                    dijit.byId('prospectAlunoPesq').set("value", gridPesquisaProspect.itensSelecionados[0].no_pessoa);
                    if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect) && parseInt(gridPesquisaProspect.itensSelecionados[0].cd_prospect) > 0)
                        dojo.byId('cdProspectPesq').value = gridPesquisaProspect.itensSelecionados[0].cd_prospect;
                    if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_aluno) && parseInt(gridPesquisaProspect.itensSelecionados[0].cd_aluno) > 0)
                        dojo.byId('cdAlunoPesq').value = gridPesquisaProspect.itensSelecionados[0].cd_aluno;
                    dijit.byId("limparProspectAlunoPesq").set("disabled", false);
                } 
            } 
        if (!valido)
            return false;
        dijit.byId("cadProspectFollowUpFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarUsuarioFKPesqFollowUp() {
    try {
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        var gridUsuarios = dijit.byId("gridUsuarioGrupo");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0 || gridUsuarioSelec.itensSelecionados.length > 1) {
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridUsuarioSelec.itensSelecionados != null && gridUsuarioSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }else {
            var tipoOrigem = dojo.byId("idOrigemUsuarioFK").value;
            if (hasValue(tipoOrigem))
                switch (parseInt(tipoOrigem)) {
                    case PESQUSERORIGFOLLOWUP:
                        dijit.byId("usuarioOrgPesq").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                        dojo.byId("cdUsuarioOrgPesq").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario ;
                        dijit.byId("limparUsuarioOrigemPesq").set("disabled", false);
                        if (NOMEUSUARIO != undefined && NOMEUSUARIO != gridUsuarioSelec.itensSelecionados[0].no_login) {
                            dijit.byId("usuarioDestinoPesq").set("value", NOMEUSUARIO);
                            dojo.byId("cdUsuarioDestinoPesq").value = CODUSUARIOLOGADO;
                            dijit.byId("limparUsuDestinoPesq").set("disabled", false);
                        }
                        break;
                    case PESQUSERDESTINOFOLLOWUP:
                        dijit.byId("usuarioDestinoPesq").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                        dojo.byId("cdUsuarioDestinoPesq").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                        dijit.byId("limparUsuDestinoPesq").set("disabled", false);
                        if (NOMEUSUARIO != undefined && NOMEUSUARIO != gridUsuarioSelec.itensSelecionados[0].no_login) {
                            dojo.byId("cdUsuarioOrgPesq").value = CODUSUARIOLOGADO;
                            dijit.byId("usuarioOrgPesq").set("value", NOMEUSUARIO);
                            dijit.byId("limparUsuarioOrigemPesq").set("disabled", false);
                        }
                        break;
                    case PESQUSERORIGMASTERFOLLOWUP:
                        dijit.byId("usuarioOrgAdmPesq").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
                        dojo.byId("cdUsuarioOrgAdmPesq").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
                        dijit.byId("limparFKUsuarioOrgAdmPesq").set("disabled", false);
                        break;
                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                        return false;
                        break;
                }
        }
        if (!valido)
            return false;
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

//C.R.U.D
function pesquisarFollowUp(limparItens) {
    try {
        var grid = dijit.byId("gridCadFollowUp");
        var cdUsuarioOrgPesq = hasValue(dojo.byId("cdUsuarioOrgPesq").value) ? dojo.byId("cdUsuarioOrgPesq").value : 0;
        var cdUsuarioDestinoPesq = hasValue(dojo.byId("cdUsuarioDestinoPesq").value) ? dojo.byId("cdUsuarioDestinoPesq").value : 0;
        var cdUsuarioOrgAdmPesq = hasValue(dojo.byId("cdUsuarioOrgAdmPesq").value) ? dojo.byId("cdUsuarioOrgAdmPesq").value : 0;
        var cdProspectPesq = hasValue(dojo.byId("cdProspectPesq").value) ? dojo.byId("cdProspectPesq").value : 0;
        var cdAlunoPesq = hasValue(dojo.byId("cdAlunoPesq").value) ? dojo.byId("cdAlunoPesq").value : 0;
        var tipoFollowUpPesq = hasValue(dijit.byId("tipoFollowUpPesq").value) ? dijit.byId("tipoFollowUpPesq").value : 0;
        var cdAcaoPesq = hasValue(dijit.byId("acaoPesq").value) ? dijit.byId("acaoPesq").value : 0;
        var idAdmin = false;
        var resolvidoPesq = hasValue(dijit.byId("resolvidoPesq").value) ? dijit.byId("resolvidoPesq").value : 0;
        var lidoPesq = hasValue(dijit.byId("lidoPesq").value) ? dijit.byId("lidoPesq").value : 0;
        var myStore =
            dojo.store.Cache(
                    dojo.store.JsonRest({
                        target: Endereco() + "/api/secretaria/getFollowUpSearch?id_tipo_follow=" + tipoFollowUpPesq + "&cd_usuario_org=" + cdUsuarioOrgPesq +
                            "&cd_usuario_destino=" + cdUsuarioDestinoPesq + "&cd_prospect=" + cdProspectPesq + "&cd_aluno=" + cdAlunoPesq + "&cd_acao=" + cdAcaoPesq +
                            "&resolvido=" + resolvidoPesq + "&lido=" + lidoPesq +
                            "&data=" + document.getElementById("ckDataPesq").checked + "&proximo_contato=" + document.getElementById("ckProximoPesq").checked +
                            "&dtInicial=" + dojo.byId("dtaInicialPesq").value + "&dtFinal=" + dojo.byId("dtaFinalPesq").value + "&id_usuario_adm=" + idAdmin,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        if (limparItens)
            grid.itensSelecionados = [];
        grid.setStore(dataStore);
        if (tipoFollowUpPesq == PROSPECTALUNO) {
            //grid.layout.setColumnVisibility(4, true)
            grid.layout.setColumnVisibility(3, false)
        }
        else {
            //grid.layout.setColumnVisibility(4, false)
            grid.layout.setColumnVisibility(3, true)
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarFollowUpPartial() {
    try {
        var followUp = null;
        if (!validarFollowUpCadastro()) {
            return false;
        }
        showCarregando();
        followUp = mountDataFollowUpPartialForPost();
        dojo.xhr.post({
            url: Endereco() + "/api/secretaria/postInsertFollowUp",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(followUp)
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridCadFollowUp';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_follow_up", itemAlterado);
                    buscarItensSelecionados(grid, 'followUpSelecionado', 'cd_follow_up', 'selecionaTodos', ['pesquisarFollowUp', 'relatorioFollowUp'], 'todosItens');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_follow_up");
                    showCarregando();
                    dijit.byId("cadFollowUp").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemCadFollowUpPartial', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditFollowUp(cdFollowUp, id_tipo_follow, pFuncao, eh_resposta) {
    showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getComponentesByFollowUpEdit?cd_follow_up=" + cdFollowUp + "&id_tipo_follow=" + id_tipo_follow,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            //apresentaMensagem("apresentadorMensagemCadFollowUpPartial", null);
            //onActiveChangeCamposGerarTitulos(false);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.acaoeFollowUp))
                    criarOuCarregarCompFiltering("cadAcaoFollowUpFK", data.retorno.acaoeFollowUp, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_acao_follow_up', 'dc_acao_follow_up');

                //Caso seja resposta, coloca o followup na resposta:
                if (eh_resposta && hasValue(data.retorno)) {
                    if (!hasValue(data.retorno.followUpsTodasRepostas))
                        data.retorno.followUpsTodasRepostas = new Array();
                    var followResposta = clone(data.retorno);
                    
                    data.retorno.followUpsTodasRepostas.push(followResposta);

                    //Configura 
                    dojo.byId('cd_follow_up_pai').value = data.retorno.cd_follow_up;
                    dojo.byId('cd_follow_up_origem').value = hasValue(data.retorno.cd_follow_up_origem) ? data.retorno.cd_follow_up_origem : data.retorno.cd_follow_up;

                    data.retorno._dc_assunto = '';
                    data.retorno.dc_assunto = '';
                    data.retorno.dt_proximo_contato = '';
                    data.retorno.dta_proximo_contato = '';
                    data.retorno.dt_follow_up = '';
                    data.retorno.dta_follow_up = '';
                    data.retorno.id_follow_lido = false;
                    data.retorno.id_follow_resolvido = false;
                    data.retorno.cd_follow_up_pai = data.retorno.cd_follow_up;
                    data.retorno.cd_follow_up = null;

                    //Trocam os usuários de origem e destino:
                    var cd_usuario_origem = data.retorno.cd_usuario;
                    data.retorno.cd_usuario = data.retorno.cd_usuario_destino;
                    data.retorno.cd_usuario_destino = cd_usuario_origem;

                    var no_usuario_origem = data.retorno.no_usuario_origem;
                    data.retorno.no_usuario_origem = data.retorno.no_usuario_destino;
                    data.retorno.no_usuario_destino = no_usuario_origem;
                }
                loadDataFollowUpPartial(data.retorno, eh_resposta);
                if (hasValue(pFuncao))
                    pFuncao.call();
            }
            showCarregando();
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemCadFollowUpPartial", error);
        showCarregando();
    });
}

function loadDataFollowUpPartial(data, eh_resposta) {
    try {
        var compCkLidoFollowUpFK = dijit.byId("ckLidoFollowUpFK");
        var compCkResolvidoFollowUpFK = dijit.byId("ckResolvidoFollowUpFK");
        dijit.byId("cadTipoFollowUpFK")._onChangeActive = false;
        dijit.byId("cadTipoFollowUpFK").set("value", data.id_tipo_follow);
        dijit.byId("cadTipoFollowUpFK")._onChangeActive = true;
        dijit.byId("panePesqGeralFollowUpFK").set("open", true);
        dojo.byId("cd_follow_up_partial").value = data.cd_follow_up;
        compCkResolvidoFollowUpFK._onChangeActive = false;
        compCkResolvidoFollowUpFK.set("checked", data.id_follow_resolvido);
        compCkResolvidoFollowUpFK._onChangeActive = true;
        dijit.byId("dtaCadastroFollowUpFK").set("value", data.dta_follow_up);
        dijit.byId("mensagemFollowUppartial").set("value", hasValue(data.dc_assunto) ? data.dc_assunto : "");
        switch (data.id_tipo_follow) {
            case INTERNO:
                compCkLidoFollowUpFK._onChangeActive = false;
                compCkLidoFollowUpFK.set("value", data.id_follow_lido);
                compCkLidoFollowUpFK._onChangeActive = true;
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("value", data.no_usuario_origem);
                dojo.byId("cdUsuarioOrigFollowUpFK").value = data.cd_usuario;
                if (hasValue(data.cd_usuario_destino)) {
                    dojo.byId("cdUsuarioDestinoFollowUpFK").value = data.cd_usuario_destino;
                    dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("value", data.no_usuario_destino);
                }
                else
                    dojo.byId("lblQtdUserLido").innerHTML = data.qtd_lido + " Usuários";
                dijit.byId("ckAdmFollowUpFK").set("checked", data.id_usuario_administrador);
                montarHistoricoMensagensAnteriores(data.followUpsTodasRepostas);
                if (data.cd_usuario != CODUSUARIOLOGADO && !eh_resposta) {
                    compCkLidoFollowUpFK._onChangeActive = false;
                    compCkLidoFollowUpFK.set("value", true);
                    compCkLidoFollowUpFK._onChangeActive = true;
                    marcarFollowUpComoLido(true);
                }
                break;
            case PROSPECTALUNO:
                dijit.byId("btnAlunoProspectFK").set("disabled", false);
                dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("value", data.no_usuario_origem);
                dojo.byId("cdUsuarioOrigFollowUpFK").value = data.cd_usuario;

                if (hasValue(data.cd_usuario_destino)) {
                    dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = data.cd_usuario_destino;
                    dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").set("value", data.no_usuario_destino);
                }


                if (hasValue(data.cd_aluno) || hasValue(data.cd_prospect)) {
                    if (hasValue(data.cd_aluno))
                        dojo.byId("cdAlunoFollowUpPartial").value = data.cd_aluno;
                    else
                        dojo.byId("cdProspectFollowUpPartial").value = data.cd_prospect;
                    dijit.byId("descAlunoProspectFollowUpFK").set("value", data.no_aluno_prospect);
                }
                if (hasValue(data.cd_acao_follow_up))
                    dijit.byId("cadAcaoFollowUpFK").set("value", data.cd_acao_follow_up);
                if (hasValue(data.dt_proximo_contato))
                    dijit.byId("dtaProxContatoFollowUpFK").set("value", data.dt_proximo_contato);
                if (hasValue(data.id_tipo_atendimento) && data.id_tipo_atendimento > 0)
                    dijit.byId("cadTipoAtendimento").set("value", data.id_tipo_atendimento);
                if (hasValue(data.dc_mail_pessoa))
                    dijit.byId("cadEmailProspectAluno").set("value", data.dc_mail_pessoa);
                if (hasValue(data.dc_telefone_pessoa))
                    dijit.byId("cadTelefoneProspectAluno").set("value", data.dc_telefone_pessoa);
                if (hasValue(data.cd_turma))
                    dojo.byId("cd_turma_pesquisa").value = data.cd_turma;
                if (hasValue(data.no_turma))
                    dojo.byId("cbTurma").value = data.no_turma;
                if (hasValue(data.dc_assunto))
                    dijit.byId("mensagemFollowUppartial").value = data.dc_assunto;

            case ADMINISTRACAOGERAL:
                dojo.byId("cdUsuarioOrigFollowUpFK").value = data.cd_usuario;
                dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("value", data.no_usuario_origem);
                dijit.byId("ckAdm").set("checked", data.id_usuario_administrador);
                dojo.byId("lblQtdUserLido").innerHTML = data.qtd_lido + " Usuários";
                if (data.cd_usuario != CODUSUARIOLOGADO) {
                    compCkLidoFollowUpFK._onChangeActive = false;
                    compCkLidoFollowUpFK.set("value", true);
                    compCkLidoFollowUpFK._onChangeActive = true;
                    marcarFollowUpComoLido(true);
                } else {
                    compCkLidoFollowUpFK._onChangeActive = false;
                    compCkLidoFollowUpFK.set("value", data.id_follow_lido);
                    compCkLidoFollowUpFK._onChangeActive = true;
                }
                break;
        }
        loadCadTipoFollowUpFK(null, true);
        montarLayoutPorTipoFollowUp();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesFollowUpPartial(grid, pFuncao, eh_resposta) {
    try {
        var value = grid.itemSelecionado;

        limparCadFollowUpPartial();
        if (value.cd_follow_up > 0)
            showEditFollowUp(value.cd_follow_up, value.id_tipo_follow, pFuncao, eh_resposta);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mountDataFollowUpPartialForPost() {
    try {
        var escolas = null;
        var gridEscolasFollowUpFK = dijit.byId("gridEscolasFollowUpFK");

        if (hasValue(gridEscolasFollowUpFK) && gridEscolasFollowUpFK.store.objectStore.data != null)
            escolas = gridEscolasFollowUpFK.store.objectStore.data;

        var retorno = {
            cd_follow_up: dojo.byId("cd_follow_up_partial").value,
            id_follow_resolvido: dijit.byId("ckResolvidoFollowUpFK").get("checked"),
            id_follow_lido: dijit.byId("ckLidoFollowUpFK").get("checked"),
            dt_follow_up: dojo.date.locale.parse(dojo.byId("dtaCadastroFollowUpFK").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            id_tipo_follow: dijit.byId("cadTipoFollowUpFK").get("value"),
            dc_assunto: dijit.byId("mensagemFollowUppartial").get("value"),
            cd_follow_up_pai: dojo.byId('cd_follow_up_pai').value,
            cd_follow_up_origem: dojo.byId('cd_follow_up_origem').value,
            id_tipo_atendimento: hasValue(dijit.byId("cadTipoAtendimento").value) ? dijit.byId("cadTipoAtendimento").value : null
        }

        switch (parseInt(dijit.byId("cadTipoFollowUpFK").value)) {
            case INTERNO:
                retorno.cd_usuario = dojo.byId("cdUsuarioOrigFollowUpFK").value;
                retorno.cd_usuario_destino = hasValue(dojo.byId("cdUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdUsuarioDestinoFollowUpFK").value) > 0 ? dojo.byId("cdUsuarioDestinoFollowUpFK").value : null;
                retorno.id_usuario_administrador = dijit.byId("ckAdmFollowUpFK").get("checked");
                if (escolas != null)
                    retorno.escolas = escolas;
                break;
            case PROSPECTALUNO:
                retorno.cd_usuario = dojo.byId("cdUsuarioOrigFollowUpFK").value;
                retorno.cd_usuario_destino = hasValue(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) > 0 ? dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value : null;
                if (hasValue(dojo.byId("cdAlunoFollowUpPartial").value) || hasValue(dojo.byId("cdProspectFollowUpPartial").value))
                    if (hasValue(dojo.byId("cdProspectFollowUpPartial").value) && parseInt(dojo.byId("cdProspectFollowUpPartial").value) > 0)
                        retorno.cd_prospect = dojo.byId("cdProspectFollowUpPartial").value;
                    else
                        retorno.cd_aluno = dojo.byId("cdAlunoFollowUpPartial").value;
                if (hasValue(dijit.byId("cadAcaoFollowUpFK").value))
                    retorno.cd_acao_follow_up = dijit.byId("cadAcaoFollowUpFK").value;
                if (hasValue(dijit.byId("dtaProxContatoFollowUpFK").value))
                    retorno.dt_proximo_contato = dojo.date.locale.parse(dojo.byId("dtaProxContatoFollowUpFK").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                if (hasValue(dijit.byId("cadAcaoFollowUpFK").value) &&
                    dijit.byId("cadAcaoFollowUpFK").value === EnumAcaoFollowUp.AULADEMONSTRATIVA) {
                    retorno.cd_turma = dojo.byId("cd_turma_pesquisa").value;
                }
            case ADMINISTRACAOGERAL:
                retorno.cd_usuario = dojo.byId("cdUsuarioOrigAdmFollowUpFK").value;
                retorno.id_usuario_administrador = dijit.byId("ckAdm").get("checked");
                break;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarFollowUpPartial() {
    try {
        var followUp = null;
        if (!validarFollowUpCadastro()) 
            return false;
        showCarregando();
        followUp = mountDataFollowUpPartialForPost();
        dojo.xhr.post({
            url: Endereco() + "/api/secretaria/postUpdateFollowUp",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(followUp)
        }).then(function (data) {
            try {
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridCadFollowUp';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "gridCadFollowUp", dojo.byId("cd_follow_up_partial").value);
                    insertObjSort(grid.itensSelecionados, "cd_follow_up_partial", itemAlterado);
                    buscarItensSelecionados(gridName, 'followUpSelecionado', 'cd_follow_up', 'selecionaTodos', ['pesquisarFollowUp', 'relatorioFollowUp'], 'todosItens');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_follow_up");
                    dijit.byId("cadFollowUp").hide();
                }
                else
                    apresentaMensagem('apresentadorMensagemCadFollowUpPartial', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarFollowUpsPartial(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId("cd_follow_up_partial").value != 0)
                itensSelecionados = [{
                    cd_follow_up: dojo.byId("cd_follow_up_partial").value
                }];
        dojo.xhr.post({
            url: Endereco() + "/api/secretaria/postDeleteFollowUps",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridCadFollowUp').itensSelecionados, "cd_follow_up", itensSelecionados[r].cd_follow_up);
                pesquisarFollowUp(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarFollowUp").set('disabled', false);
                dijit.byId("relatorioFollowUp").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadFollowUp").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadFollowUp").style.display))
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarFollowUpPartial(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridCadFollowUp = dijit.byId('gridCadFollowUp');

            gridCadFollowUp.itemSelecionado = itensSelecionados[0];
            apresentaMensagem('apresentadorMensagem', '');
            keepValuesFollowUpPartial(gridCadFollowUp, null, false);
            IncluirAlterar(0, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial',
                'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
            dijit.byId("cadFollowUp").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}