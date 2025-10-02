function mascaraHoraEvento(ready) {
    ready(function () {
        maskHour('#hh_inicial_evento');
        maskHour('#hh_final_evento');
        maskHour('#hh_inicio_evento');
        maskHour('#hh_fim_evento');
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridCalEvento';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCalEventos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_calendario_evento", grid._by_idx[rowIndex].item.cd_calendario_evento);

            value = value || indice != null; // Item est� selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_calendario_evento', 'selecionadoCalEvento', -1, 'selecionaTodosCalEventos', 'selecionaTodosCalEventos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_calendario_evento', 'selecionadoCalEvento', " + rowIndex + ", '" + id + "', 'selecionaTodosCalEventos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarMetodosCalendarioEvento() {

    //Criação da Grade de sala
    require([
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/DataGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojox/data/JsonRestStore",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dojo/dom-attr",
        "dijit/form/Button",
        "dijit/form/TextBox",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/DateTextBox",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox) {
        ready(function () {
            showCarregando();

            mascaraHoraEvento(ready);

            //*** Cria a grade de Eventos **\\        
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioEventosPorFiltros?dc_titulo_evento=&inicio=false&status=true&dt_inicial_evento=&dt_final_evento=&hh_inicial_evento=&hh_final_evento=",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEvento) {
                var gridCalEvento = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: dataEvento }) }),
                    structure: [
                        { name: "<input id='selecionaTodosCalEventos' style='display:none'/>", field: "selecionadoCalEvento", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        { name: "Título", field: "dc_titulo_evento", width: "40%", styles: "min-width:80px;" },
                        { name: "Data Inicial", field: "dt_inicial_evento", width: "15%", styles: "min-width:80px;text-align:center;", formatter: dataAtualFormatada },
                        { name: "Hora Inicial", field: "hh_inicial_evento", width: "15%", styles: "min-width:80px;text-align:center;", formatter: horaFormatada },
                        { name: "Data Final", field: "dt_final_evento", width: "15%", styles: "min-width:80px;text-align:center;", formatter: dataAtualFormatada },
                        { name: "Hora Final", field: "hh_final_evento", width: "15%", styles: "min-width:80px;text-align:center;", formatter: horaFormatada },
                        { name: "Ativo", field: "id_ativo", width: "10%", styles: "min-width:80px;text-align:center;", formatter: statusAtivoFormatado }
                    ],
                    noDataMessage: msgNotRegEnc,
                    canSort: true,
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
                }, "gridCalEvento");
                gridCalEvento.startup();
                gridCalEvento.canSort = function (col) { return Math.abs(col) != 1; };
                gridCalEvento.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex;
                        var calendarioEventoSelecionado = this.getItem(idx);
                        LimitarHrInicialFinal();
                        obterCalendarioEventoPorID(calendarioEventoSelecionado.cd_calendario_evento);
                        dijit.byId("dialogCalendarioEvento").show();
                        IncluirAlterar(0, 'divAlterarCalEvento', 'divIncluirCalEvento', 'divExcluirCalEvento', 'apresentadorMensagemCalEvento', 'divCancelarCalEvento', 'divLimparCalEvento');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                showCarregando();
            });

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridCalEvento, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosCalEventos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_calendario_evento', 'selecionadoCalEvento', -1, 'selecionaTodosCalEventos', 'selecionaTodosCalEventos', 'gridCalEvento')", gridCalEvento.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    eventoEditarCalEvento(dijit.byId('gridCalEvento').itensSelecionados);
                }
            });
            menu.addChild(acaoEditar);

            var acaoExcluir = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    eventoRemover(dijit.byId('gridCalEvento').itensSelecionados, 'deletarCalEvento(itensSelecionados)');
                }
            });
            menu.addChild(acaoExcluir);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasCalEvento",
                dropDown: menu,
                id: "acoesRelacionadasCalEvento"
            });
            dom.byId("linkAcoesCalEvento").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridCalEvento, 'todosItensCalEvento', ['pesquisarEvento', 'relatorioCalEvento']); pesquisarCalendarioEvento(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridCalEvento', 'selecionadoCalEvento', 'cd_calendario_evento', 'selecionaTodosCalEventos', ['pesquisarEvento', 'relatorioCalEvento'], 'todosItensCalEvento'); }
            });
            menu.addChild(menuItensSelecionados);


            button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensCalEvento",
                dropDown: menu,
                id: "todosItensCalEvento"
            });
            dom.byId("linkSelecionadosCalEvento").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar",
                iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                onClick: function () {
                    salvarCalendarioEvento();
                }
            }, "incluirCalEvento");

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    editarCalendarioEvento();
                }
            }, "alterarCalEvento");

            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarCalEvento() });
                }
            }, "deleteCalEvento");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    limparCamposCalEvento();
                }
            }, "limparCalEvento");

            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagemCalEvento', null);
                    obterCalendarioEventoPorID(dom.byId("cd_calendario_evento").value);
                }
            }, "cancelarCalEvento");

            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("dialogCalendarioEvento").hide();
                    limparCamposCalEvento();
                }
            }, "fecharCalEvento");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    if (!dijit.byId('dt_inicial_evento').validate() || !dijit.byId('dt_final_evento').validate() ||
                        !dijit.byId('hh_inicial_evento').validate() || !dijit.byId('hh_final_evento').validate())
                        return false;
                    apresentaMensagem('apresentadorMensagem', null);
                    pesquisarCalendarioEvento(true);
                }
            }, "pesquisarEvento");

            decreaseBtn(document.getElementById("pesquisarEvento"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    limparCamposCalEvento();
                    IncluirAlterar(1, 'divAlterarCalEvento', 'divIncluirCalEvento', 'divExcluirCalEvento', 'apresentadorMensagemCalEvento', 'divCancelarCalEvento', 'divLimparCalEvento');
                    LimitarHrInicialFinal();
                    dijit.byId("dialogCalendarioEvento").show();

                    //dijit.byId('tabContainer').resize();
                    //Para alinhar o painel do dojo, verificar se isso ocorre no desenvolvimento:
                    //dojo.byId('tabContainer_tablist').children[3].children[0].style.width = '100%';
                }
            }, "novoCalEvento");

            new Button({
                label: "Visualizar",
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    gerarRelatorioCalendarioEvento();
                }
            }, "relatorioCalEvento");

            montarStatus("status_evento");
            LimitarHrInicialFinalFiltro();
        })
    });
};


function LimitarHrInicialFinal() {
    require(["dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr", "dojox/json/ref"],
        function (ready, Calendar, Observable, Memory, declare, on, xhr, ref) {
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
                                minI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(3, 5));
                                minI = minI == 0 ? "00" : minI;
                            }
                            if (hasValue(getHorarioFuncEscola.retorno.hr_final)) {
                                horaF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(0, 2));
                                minF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(3, 5));
                                minF = minF == 0 ? "00" : minF;
                            }
                        }

                        dijit.byId("hh_inicio_evento").constraints.min.setMinutes(minI);
                        dijit.byId("hh_inicio_evento").constraints.min.setHours(horaI);
                        dijit.byId("hh_inicio_evento").constraints.max.setHours(horaF);
                        dijit.byId("hh_fim_evento").constraints.max.setMinutes(minF);
                        dijit.byId("hh_fim_evento").constraints.min.setHours(horaI);
                        dijit.byId("hh_fim_evento").constraints.max.setHours(horaF);

                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemTurma', error);
                    });
            });
        });
}


function LimitarHrInicialFinalFiltro() {
    require(["dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr", "dojox/json/ref"],
        function (ready, Calendar, Observable, Memory, declare, on, xhr, ref) {
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
                                minI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(3, 5));
                                minI = minI == 0 ? "00" : minI;
                            }
                            if (hasValue(getHorarioFuncEscola.retorno.hr_final)) {
                                horaF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(0, 2));
                                minF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(3, 5));
                                minF = minF == 0 ? "00" : minF;
                            }
                        }

                        dijit.byId("hh_inicial_evento").constraints.min.setMinutes(minI);
                        dijit.byId("hh_inicial_evento").constraints.min.setHours(horaI);
                        dijit.byId("hh_inicial_evento").constraints.max.setHours(horaF);
                        dijit.byId("hh_final_evento").constraints.max.setMinutes(minF);
                        dijit.byId("hh_final_evento").constraints.min.setHours(horaI);
                        dijit.byId("hh_final_evento").constraints.max.setHours(horaF);

                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemTurma', error);
                    });
            });
        });
}

function pesquisarCalendarioEvento(limparItens) {
    require([
      "dojo/_base/xhr",
      "dojo/data/ObjectStore",
      "dojo/store/Cache",
      "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        try {
            showCarregando();

            var gridCalEvento = dijit.byId("gridCalEvento");

            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioEventosPorFiltros?dc_titulo_evento=" + dijit.byId("dc_titulo_evento").value + "&inicio=" + dijit.byId("inicio_dc_titulo_evento").checked +
                    "&status=" + status() + "&dt_inicial_evento=" + dojo.byId("dt_inicial_evento").value + "&dt_final_evento=" + dojo.byId("dt_final_evento").value +
                    "&hh_inicial_evento=" + dojo.byId("hh_inicial_evento").value + "&hh_final_evento=" + dojo.byId("hh_final_evento").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEvento) {

                if (limparItens) {
                    gridCalEvento.itensSelecionados = [];
                }
                showCarregando();
                gridCalEvento.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataEvento }) }));
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function salvarCalendarioEvento() {
    try {

        validarTextarea('txt_dc_desc_evento');
        if (!dijit.byId("formCalendarioEvento").validate() || !dijit.byId("txt_dc_desc_evento").validator())
            return false;
        showCarregando();

        var calendarioEvento = {
            dc_titulo_evento: dijit.byId("txt_dc_titulo").value,
            dc_desc_evento: dijit.byId("txt_dc_desc_evento").value,
            id_ativo: dijit.byId("chk_id_ativo").checked,
            dt_inicial_evento: dojo.date.locale.parse(dojo.byId("dt_inicio_evento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_final_evento: dojo.date.locale.parse(dojo.byId("dt_fim_evento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            hh_inicial_evento: dojo.byId("hh_inicio_evento").value,
            hh_final_evento: dojo.byId("hh_fim_evento").value
        };

        require([
          "dojo/_base/xhr"
        ], function (xhr) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/adicionarCalendarioEvento",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify(calendarioEvento)
            }).then(function (data) {
                var eventos = JSON.parse(data);

                if (!hasValue(eventos.erro)) {
                    eventos = eventos.retorno;

                    var gridCalEventos = dijit.byId("gridCalEvento");
                    apresentaMensagem('apresentadorMensagemCalEvento', data);
                    dijit.byId("dialogCalendarioEvento").hide();
                    if (!hasValue(gridCalEventos.itensSelecionados)) {
                        gridCalEventos.itensSelecionados = [];
                    }
                    insertObjSort(gridCalEventos.itensSelecionados, "cd_calendario_evento", eventos);
                    buscarItensSelecionados('gridCalEvento', 'selecionadoCalEvento', 'cd_calendario_evento', 'selecionaTodosCalEventos', ['pesquisarEvento', 'relatorioCalEvento'], 'todosItensCalEvento');
                    setGridPagination(gridCalEventos, eventos, "cd_calendario_evento");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCalEvento', data);
                }
            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioNovo);
                    apresentaMensagem("apresentadorMensagemCalEvento", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagemCalEvento", error);
            });
        });
    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function deletarCalEvento(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            var cd_calendario_evento = 0;
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
                if (dojo.byId('cd_calendario_evento').value > 0)
                    itensSelecionados= [{ cd_calendario_evento: dom.byId("cd_calendario_evento").value }];
            }
           
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/deletarCalendarioEvento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                var evento = JSON.parse(data);
                evento = JSON.parse(evento);

                if (!hasValue(evento.erro)) {
                    var todos = dojo.byId("todosItensCalEvento");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('dialogCalendarioEvento').hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(itensSelecionados, "cd_calendario_evento", itensSelecionados[r].cd_calendario_evento);
                    pesquisarCalendarioEvento(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarEvento").set('disabled', false);
                    dijit.byId("relatorioCalEvento").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    showCarregando();
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', data);
                }
            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioExcluir);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagem", error);
            });
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function eventoEditarCalEvento(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparCamposCalEvento();
            apresentaMensagem('apresentadorMensagem', '');
            var evento = itensSelecionados[0];
            LimitarHrInicialFinal();
            obterCalendarioEventoPorID(evento.cd_calendario_evento);
            dijit.byId("dialogCalendarioEvento").show();
            IncluirAlterar(0, 'divAlterarCalEvento', 'divIncluirCalEvento', 'divExcluirCalEvento', 'apresentadorMensagemCalEvento', 'divCancelarCalEvento', 'divLimparCalEvento');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarCalendarioEvento() {

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {

            validarTextarea('txt_dc_desc_evento');
            if (!dijit.byId("formCalendarioEvento").validate() || !dijit.byId("txt_dc_desc_evento").validator())
                return false;

            showCarregando();

            var calendarioEvento = {
                cd_calendario_evento: dojo.byId("cd_calendario_evento").value,
                dc_titulo_evento: dijit.byId("txt_dc_titulo").value,
                dc_desc_evento: dijit.byId("txt_dc_desc_evento").value,
                id_ativo: dijit.byId("chk_id_ativo").checked,
                dt_inicial_evento: dojo.date.locale.parse(dojo.byId("dt_inicio_evento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                dt_final_evento: dojo.date.locale.parse(dojo.byId("dt_fim_evento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                hh_inicial_evento: dojo.byId("hh_inicio_evento").value,
                hh_final_evento: dojo.byId("hh_fim_evento").value
            };
            xhr.post({
                url: Endereco() + "/api/coordenacao/editarCalendarioEvento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(calendarioEvento)
            }).then(function (data) {

                var evento = JSON.parse(data);
                evento = JSON.parse(evento);
                if (!hasValue(evento.erro)) {
                    evento = evento.retorno;

                    var gridCalEventos = dijit.byId("gridCalEvento");
                    apresentaMensagem('apresentadorMensagemCalEvento', data);
                    dijit.byId("dialogCalendarioEvento").hide();
                    if (!hasValue(gridCalEventos.itensSelecionados)) {
                        gridCalEventos.itensSelecionados = [];
                    }
                    removeObjSort(gridCalEventos.itensSelecionados, "cd_calendario_evento", dom.byId("cd_calendario_evento").value);
                    insertObjSort(gridCalEventos.itensSelecionados, "cd_calendario_evento", evento);
                    buscarItensSelecionados('gridCalEvento', 'selecionadoCalEvento', 'cd_calendario_evento', 'selecionaTodosCalEventos', ['pesquisarEvento', 'relatorioCalEvento'], 'todosItensCalEvento');
                    setGridPagination(gridCalEventos, evento, "cd_calendario_evento");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCalEvento', data);
                }

            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioEditar);
                    apresentaMensagem("apresentadorMensagemCalEvento", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagemCalEvento", error);
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function obterCalendarioEventoPorID(cd_calendario_evento) {
    require(["dojo/_base/xhr"], function (xhr) {
        try {
            showCarregando();
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioEventoPorID?cd_calendario_evento=" + cd_calendario_evento,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                showCarregando();
                var evento = jQuery.parseJSON(data);
                if (!hasValue(evento.erro)) {
                    preencheInputCalendarioEvento(evento.retorno);
                }
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function gerarRelatorioCalendarioEvento() {
    try {
        apresentaMensagem('apresentadorMensagem', null);

        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/GetUrlRelatorioCalendarioEvento?" + getStrGridParameters('gridCalEvento') + "dc_titulo_evento=" + dijit.byId("dc_titulo_evento").value + "&inicio=" + dijit.byId("inicio_dc_titulo_evento").checked +
                    "&status=" + status() + "&dt_inicial_evento=" + dojo.byId("dt_inicial_evento").value + "&dt_final_evento=" + dojo.byId("dt_final_evento").value +
                    "&hh_inicial_evento=" + dojo.byId("hh_inicial_evento").value + "&hh_final_evento=" + dojo.byId("hh_final_evento").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        })
    }
    catch (e) {
        postGerarLog(e);
    }
}

function preencheInputCalendarioEvento(evento) {
    if (hasValue(evento)) {
        dojo.byId("cd_calendario_evento").value = evento.cd_calendario_evento;
        dojo.byId("txt_dc_titulo").value = evento.dc_titulo_evento;
        dijit.byId("txt_dc_desc_evento").set("value", evento.dc_desc_evento);
        dijit.byId("chk_id_ativo").set("value", evento.id_ativo);
        dojo.byId("dt_inicio_evento").value = dataAtualFormatada(evento.dt_inicial_evento);
        dojo.byId("dt_fim_evento").value = dataAtualFormatada(evento.dt_final_evento);
        dojo.byId("hh_inicio_evento").value = horaFormatada(evento.hh_inicial_evento);
        dojo.byId("hh_fim_evento").value = horaFormatada(evento.hh_final_evento);
    }
    // Remove state error do campo Textarea.
    dijit.byId("txt_dc_desc_evento").set("state", "Incomplete");
}

function limparCamposCalEvento() {
    dojo.byId("cd_calendario_evento").value = 0;
    dijit.byId("txt_dc_titulo").reset();
    dijit.byId("txt_dc_desc_evento").reset();
    dijit.byId("chk_id_ativo").set("value", true);
    dijit.byId("dt_inicio_evento").reset();
    dijit.byId("dt_fim_evento").reset();
    dijit.byId("hh_inicio_evento").reset();
    dijit.byId("hh_fim_evento").reset();
    apresentaMensagem('apresentadorMensagemCalEvento', null);
}

function status() {
    if (dijit.byId("status_evento").value == 1)
        return true;
    if (dijit.byId("status_evento").value == 2)
        return false;
    return null;
}

function validarTextarea(field) {
    var fnameTextBox = dijit.byId(field);
    fnameTextBox.validator = function () {
        if (!hasValue(dojo.byId(field).value.trim())) {
            fnameTextBox.set("state", "Error");
            return false;
        }
        return true;
    }
}