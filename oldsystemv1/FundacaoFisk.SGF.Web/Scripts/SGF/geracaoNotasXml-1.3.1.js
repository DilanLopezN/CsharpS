
function formatCheckBoxck(value, rowIndex, obj) {
    var gridNota = dijit.byId('gridNotasXml');
    var icon;
    var id = obj.field + '_Selected_' + gridNota._by_idx[rowIndex].item.id_ativo + rowIndex;

    if (hasValue(dijit.byId(id)))
        dijit.byId(id).destroy();
    if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
    setTimeout("setCheckBoxId(" + value + ", " + rowIndex + ", " + id + ")", 1);
    return icon;
}

function formatDataEmissao(value, rowIndex, obj)
{
    if (hasValue(value))
    {
        var valor_data = dojo.date.locale.format(new Date(value),
                       { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "long", locale: "pt-br" });

        return valor_data
    }
}

function formatDataNota(value, rowIndex, obj)
{
    if (hasValue(value)) {
        var valor_data = dojo.date.locale.format(new Date(value),
                       { selector: "date", datePattern: "dd/MM/yyyy :HH:mm:ss", formatLength: "long", locale: "pt-br" });

        return valor_data
    }
}

function setCheckBoxId(value, rowIndex, id) {
    require(["dijit/form/CheckBox", "dojo/domReady!"], function (CheckBox) {
        var checkBox = new CheckBox({
            name: "checkBox" + id,
            value: value,
            checked: value
        }, id);
    });
}

require([
        "dijit/layout/TabContainer",
        "dijit/layout/ContentPane",
        "dojo/NodeList-traverse",
        "dijit/form/TextBox",
        "dijit/Dialog",
        "dijit/form/Select",
        "dojo/_base/array",
        "dojo/_base/event",
        "dojox/validate/web",
        "dojox/validate/check",
        "dijit/form/NumberTextBox",
        "dijit/form/ValidationTextBox",
        "dijit/form/FilteringSelect",
        "dijit/form/Form",
        "dojox/form/Manager",
        "dojox/form/manager/_Mixin",
        "dojox/form/manager/_NodeMixin",
        "dojox/form/manager/_EnableMixin",
        "dojox/form/manager/_ValueMixin",
        "dojox/form/manager/_DisplayMixin",
        "dijit/form/RadioButton",
        "dojo/parser"
]);

function selecionaTab(e) {
    var tab = dojo.query(e.target).parents('.dijitTab')[0];

    if (!hasValue(tab)) // Clicou na borda da aba:
        tab = dojo.query(e.target)[0];
    else
        if (tab.innerText != 'Novos') {
            AbrirFiltros(true);
            $("#filtroNotasXML").show();
            dijit.byId('btPesquisa').set('disabled', false);
            dijit.byId('ckProcessada').set('checked', true);
            dijit.byId('ckResolvido').set('checked', true);
            sugereDataCorrente();
            apresentaMensagem('apresentadorMensagemCurso', null);
        }
        else {
            $("#filtroNotasXML").hide();
        }
}

function AbrirFiltros(value) {
    dijit.byId('panePesqGeral').set('open', value);
    dijit.byId('panePesqGeral').set('disabled', !value);
}

function formatCheckBoxNotasXml(value, rowIndex, obj) {
    var gridName = 'gridNotasXml';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosNotasXml');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_importacao_XML", grid._by_idx[rowIndex].item.cd_importacao_XML);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_importacao_XML', 'selecionadoNotasXml', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_importacao_XML', 'selecionadoNotasXml', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

    return icon;
}

function montarCadastroGeraNota() {
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
        "dijit/form/CheckBox",
        "dijit/Dialog",
        "dojo/parser",
         "dijit/form/FilteringSelect",
         "dojo/_base/array",
         "dojo/ready"
        //"dojo/domReady!"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox, CheckBox, FilteringSelect, array, ready) {
            try {
                $("#filtroNotasXML").hide();
                var myStore =
                Cache(
                   JsonRest({
                       target: Endereco() + "/api/secretaria/abrirGerarXML/",
                       handleAs: "json",
                       cache: false,
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }), Memory({}));
                         var gridNovos = new EnhancedGrid({
                             store: ObjectStore({ objectStore: myStore }),
                             structure:
                             [
                                 { name: "Arquivo", field: "no_arquivo_XML", width: "80%", styles: "min-width:80px;" },
                                 { name: "Caminho", field: "dc_path_arquivo", width: "80%", styles: "min-width:80px;" }
                             ],
                             selectionMode: "single",
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
                                     plugins: { nestedSorting: false }
                                 }
                             }
                         }, "gridNovos");
                         gridNovos.pagination.plugin._paginator.plugin.connect(gridNovos.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                             verificaMostrarTodos(evt, gridNovos, 'no_arquivo_XML');
                         });
                         gridNovos.canSort = function (col) { return Math.abs(col) != 1 };
                         gridNovos.startup();   

                         var data = new Array();
                         var gridNotasXml = new EnhancedGrid({
                             store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "dt_importacao_xml" }) }),
                             structure:
                             [
                                     { name: "<input id='selecionaTodosNotasXml' style='display:none'/>", field: "selecionadoNotasXml", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxNotasXml },
                                     { name: "Data", field: "dt_importacao_xml", width: "140px", styles: "min-width:140px;", formatter: formatDataNota },
                                     { name: "Arquivo", field: "no_arquivo_XML", width: "35%", styles: "min-width:80px;" },
                                     { name: "Nota", field: "nm_nota_fiscal", width: "55px", styles: "min-width:55px;" },
                                     { name: "Emissão", field: "dt_emissao_nf", width: "70px", styles: "min-width:70px;", formatter: formatDataEmissao },
                                     { name: "Escola", field: "no_escola", width: "30%", styles: "min-width:70px;" },
                                     { name: "Obs", field: "dc_mensagem_XML", width: "15%", styles: "min-width:80px;" }
                             ],
                             selectionMode: "single",
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
                         }, "gridNotasXml");
                         gridNotasXml.pagination.plugin._paginator.plugin.connect(gridNotasXml.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                             verificaMostrarTodos(evt, gridNotasXml, 'cd_importacao_XML', 'selecionaTodos');
                         });
                         gridNotasXml.canSort = function (col) { return Math.abs(col) != 1 };

                         gridNotasXml.startup();

                      
                        // Adiciona link de ações:
                        var menu = new DropDownMenu({ style: "height: 25px", id: "ActionMenu" });

                        var acaoExcluir = new MenuItem({
                            label: "Resolver/Não Resolver",
                            onClick: function () {
                                resolverNaoResolver(gridNotasXml.itensSelecionados, xhr, Memory, ObjectStore);
                            }
                        });
                        menu.addChild(acaoExcluir);

                        button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasNotasXml",
                            dropDown: menu,
                            id: "acoesRelacionadasNotasXml"
                        });
                        dom.byId("linkAcoes").appendChild(button.domNode);

                        new Button({
                            label: "",
                            id: "btPesquisa",
                            disabled: true,
                            iconClass: 'dijitEditorIconSearchSGF',
                            onClick: function () {
                                pesquisaNotasGeradas();
                            }
                        }, "pesquisarNotasXML");
                        decreaseBtn(document.getElementById("btPesquisa"), '32px');

                        new Button({
                            label: "Ver Detalhes",
                            iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                            onClick: function () {
                                    eventoVisualizarNotaGerada(gridNotasXml.itensSelecionados);
                            }
                        }, "relatorioNotasXml");
                        

                        new Button({
                            id: "btnGerar",
                            label: "Gerar",
                            iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                            onClick: function () {
                                gerarNotasXmlProc();
                            }
                        }, "btGerarNota");

                        new Button({
                            id: "btnFechar",
                            label: "Fechar",
                            iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                            onClick: function () {
                                var grid = dijit.byId("gridNotasXml");
                                grid.itensSelecionados = [];
                                dijit.byId('cad').hide();
                            }
                        }, "fecharDet");

                        var checkBox = new CheckBox({
                            name: "ckResolvido",
                            onClick: function () {
                            }
                        }, 'ckResolvido');

                        var checkBox = new CheckBox({
                            name: "ckProcessada",
                            onClick: function () {
                                if (dijit.byId('ckProcessada').checked)
                                    dijit.byId('ckResolvido').set('checked', true);
                            }
                        }, 'ckProcessada');

                        var checkBox = new CheckBox({
                            name: "ckInconsist",
                            onClick: function () {
                                if (dijit.byId('ckInconsist').checked)
                                    dijit.byId('ckResolvido').set('checked', false);
                            }
                        }, 'ckInconsist');

                        var checkBox = new CheckBox({
                            name: "ckItens",
                            onClick: function () {
                                if (dijit.byId('ckItens').checked)
                                    dijit.byId('ckResolvido').set('checked', false);
                            }
                        }, 'ckItens');
                        sugereDataCorrente();

                        // Adiciona link de Todos os Itens:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () {
                                var limparItens = true;
                                buscarTodosItens(gridNotasXml, 'linkSelecionados', ['btPesquisa']);
                                pesquisaNotasGeradas(limparItens);
                            }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () {
                                buscarItensSelecionados('gridNotasXml', 'selecionadoNotasXml', 'cd_importacao_XML', 'selecionaTodos', ['btPesquisa'], 'linkSelecionados');
                            }
                        });
                        menu.addChild(menuItensSelecionados);

                        button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItensNotasXml",
                            dropDown: menu,
                            id: "linkSelecionados"
                        });
                        dom.byId("linkSelecionados").appendChild(button.domNode);
                    }
            catch (e) {
                postGerarLog(e);
            }
            //});
    })
}

function habilitaGerar() {
    dijit.byId('btnGerar').set('disabled', dijit.byId('gridNovos')._by_idx.length == 0);
}

function pesquisaNotasGeradas(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridNotasXml = dijit.byId("gridNotasXml");
            showCarregando();

            var idImportacao = [];

            // trata os campos para a pesquisa
            if (dojo.byId("ckProcessada").checked == true) {
                idImportacao.push(1);
            }
            //else {
            //    idImportacao.push(1, 2, 3);
            //}

            if (dojo.byId("ckResolvido").checked == 1) {
                var idResolvido = true;
            } else {
                var idResolvido = false;
            }

            if (dojo.byId("ckInconsist").checked == true) {
                idImportacao.push(2);
            }

            if (dojo.byId("ckItens").checked == true) {
                idImportacao.push(3);
            }
            
            if (hasValue(dijit.byId("emitente"))) {
                var emitentente = dijit.byId("emitente").value;
            } else {
                var emitentente = "";
            }

            if (hasValue(dijit.byId("escola"))) {
                var escola = dijit.byId("escola").value;
            } else {
                var escola = "";
            }

            if (dojo.byId("ckInicio").checked) {
                var inicio = 1;
            } else {
                var inicio = 0;
            }

            if (hasValue(dijit.byId("numeroNota"))) {
                var numeroNota = dijit.byId("numeroNota").value;
            } else {
                var numeroNota = "";
            }

            if (hasValue(dijit.byId("edItem"))) {
                var edItem = dijit.byId("edItem").value;
            } else {
                var edItem = "";
            }

            // Trata as datas
            if (hasValue(dijit.byId("dtaInicial"))) {
                var data_ini = dojo.date.locale.parse(dojo.byId("dtaInicial").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                data_ini = dojo.date.locale.format(data_ini,
                    { selector: "date", datePattern: "yyyy-MM-dd", formatLength: "long", locale: "pt-br" });
            } else {
                var data_ini = null;
            }

            if (hasValue(dijit.byId("dtEmissNota"))) {
                var data_nota = dojo.date.locale.parse(dojo.byId("dtEmissNota").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                data_nota = dojo.date.locale.format(data_nota,
                    { selector: "date", datePattern: "yyyy-MM-dd", formatLength: "long", locale: "pt-br" });
            } else {
                var data_nota = null;
            }

            if (hasValue(dijit.byId("dtaFinal"))) {
                var data_fim = dojo.date.locale.parse(dojo.byId("dtaFinal").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                data_fim = dojo.date.locale.format(data_fim,
                    { selector: "date", datePattern: "yyyy-MM-dd", formatLength: "long", locale: "pt-br" });
            } else {
                var data_fim = null;
            }
            
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/secretaria/getListaXmlGerados?cd_importacao_XML=" + idImportacao.toString() + "&id_resolvido=" + idResolvido +  "&no_pessoa=" + emitentente + "&no_escola=" + escola + "&inicio=" + inicio + "&nm_nota_fiscal=" + numeroNota + "&dt_emissao_nf=" + data_nota + "&itemExistente=" + edItem + "&data_ini=" + data_ini + "&dataFim=" + data_fim,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });

            var grid = dijit.byId("gridNotasXml");

            if (limparItens) {
                grid.itensSelecionados = [];
            }

            grid.setStore(dataStore);
            showCarregando();

        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    }
    );
}

// chama a procedure sp_import_xml ao clicar no botão "Gerar"
function gerarNotasXmlProc() {
    try {
        if (dijit.byId('gridNovos')._by_idx.length == 0) {
            var mensagensWeb = new Array();
            var mensagemErro = "Não existem notas a serem geradas";
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemErro);
            apresentaMensagem('apresentadorMensagemCurso', mensagensWeb);
            return false
        };
        apresentaMensagem('apresentadorMensagemCurso', null);
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/secretaria/postGerarXmlProc",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        }).then(function (data) {
            var dataG = jQuery.parseJSON(data);
            dijit.byId("gridNovos").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataG }) }));
            hideCarregando();
        }, function (error) {
            hideCarregando();
            //dijit.byId("gridNovos").refresh();
            apresentaMensagem('apresentadorMensagemCurso', error.response.data);
        }
        );
      }
      catch (e) {
        hideCarregando();
         postGerarLog(e);
      }
}

function sugereDataCorrente() {
    try {
        dojo.xhr.post({
            url: Endereco() + "/util/PostDataHoraCorrente",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson()
        }).then(function (data) {
            if (data.indexOf("<!DOCTYPE html>") < 0) {
                var dataCorrente = jQuery.parseJSON(data).retorno;
                var dataSugerida = dataCorrente.dataPortugues.split(" ");
                if (dataSugerida.length > 0) {
                    var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    dijit.byId('dtaInicial').set("value", date);
                    //pesquisaNotasGeradas();
                }
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemPessoa', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparNotasXML() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        try {
            var gridNotasXml = dijit.byId('gridNotasXml');
            gridNotasXml.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            apresentaMensagem('apresentadorMensagemCurso', null);
            document.getElementById("cd_importacao_XML").value = '';
            gridNotasXml.update();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparFiltrosPesquisa() {
    try {
        dijit.byId('ckProcessada').set('checked', true);
        dijit.byId('ckResolvido').set('checked', false);
        dijit.byId('ckInconsist').set('checked', false);
        dijit.byId('ckItens').set('checked', false);
        dijit.byId('ckInicio').set('checked', false);
        dojo.byId('emitente').value = '';
        dojo.byId('escola').value = '';
        dojo.byId('numeroNota').value = '';
        dojo.byId('edItem').value = '';
        dojo.byId('dtEmissNota').value = '';
        dojo.byId('dtaFinal').value = '';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoVisualizarNotaGerada(itensSelecionados) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
                caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Visualizar.', null);
            else if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para Visualizar.', null);
            else {
                dijit.byId("dt_importacao_xml").set("value", itensSelecionados[0].dt_importacao_xml);
                dijit.byId("dc_path_arquivo").set("value", itensSelecionados[0].dc_path_arquivo);
                dijit.byId("nm_nota_fiscal").set("value", itensSelecionados[0].nm_nota_fiscal);
                dijit.byId("dt_emissao_nf").set("value", itensSelecionados[0].dt_emissao_nf);
                dijit.byId("no_escola").set("value", itensSelecionados[0].no_escola);
                dijit.byId("no_pessoa").set("value", itensSelecionados[0].no_pessoa);
                dijit.byId("no_item_inexistente").set("value", itensSelecionados[0].no_item_inexistente);
                dijit.byId("cd_mensagem_XML").set("value", itensSelecionados[0].dc_mensagem_XML);

                if (itensSelecionados[0].id_tipo_importacao == 1) {
                    var tipo = "Processada";
                }
                if (itensSelecionados[0].id_tipo_importacao == 2) {
                    var tipo = "Inconsistente";
                }
                if (itensSelecionados[0].id_tipo_importacao == 3) {
                    var tipo = "Itens";
                }

                dijit.byId("id_tipo_importacao").set("value", tipo);

                if (itensSelecionados[0].id_resolvido == 1) {
                    var resolvido = "Sim";
                } else {
                    var resolvido = "Não";
                }

                dijit.byId("id_resolvido").set("value", resolvido);
                dijit.byId("no_usuario").set("value", itensSelecionados[0].no_usuario);

                dijit.byId("cad").show();
            }
        } 
        catch (e) {
            postGerarLog(e);
        }
}

function resolverSelecionados(itensSelecionados)
{
    var notasSele = [];
    for (var i = 0; i < itensSelecionados.length; i++)
    {
        var cdImportXML = itensSelecionados[i].cd_importacao_XML;
        notasSele.push(cdImportXML);
    }
    return notasSele;
}


function resolverNaoResolver(itensSelecionados, xhr, Memory, ObjectStore)
{
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro ajustar a situação .', null);
        } else {
            var resultCodigos = resolverSelecionados(itensSelecionados);
            dojo.xhr.post({
                url: Endereco() + "/api/secretaria/atualizaResolverNaoResolver", 
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                chache: false,
                postData: dojox.json.ref.toJson({
                    cds_xml: resultCodigos
                })
            }).then(function (data) {
                data = jQuery.parseJSON(data);
                buscarItensSelecionados('gridNotasXml', 'selecionadoNotasXml', 'cd_importacao_XML', 'selecionaTodos', ['btPesquisa'], 'linkSelecionados');
            })
        }
    }   
    catch (e) {
        postGerarLog(e);
    }
}
