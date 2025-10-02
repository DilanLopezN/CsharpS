var TIPO_FINANCEIRO_CARTAO = 5;

function formatCheckBoxBaixaCartao(value, rowIndex, obj) {
    var gridName = 'gridBaixaCartao';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosTitulo');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

        value = value || indice != null; // Item est� selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionadoTitulo', -1, 'selecionaTodosTitulo', 'selecionaTodosTitulo', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_titulo', 'selecionadoTitulo', " + rowIndex + ", '" + id + "', 'selecionaTodosTitulo', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxGeradas(value, rowIndex, obj) {
    var gridName = 'gridGeradas';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosGeradas');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_baixa_automatica", grid._by_idx[rowIndex].item.cd_baixa_automatica);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_baixa_automatica', 'selecionadoGeradas', -1, 'selecionaTodosGeradas', 'selecionaTodosGeradas', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_baixa_automatica', 'selecionadoGeradas', " + rowIndex + ", '" + id + "', 'selecionaTodosGeradas', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxBaixas(value, rowIndex, obj) {
    var gridName = 'gridBaixas';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosBaixa');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_baixa", grid._by_idx[rowIndex].item.cd_baixa);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_baixa', 'selecionadoBaixa', -1, 'selecionaTodosBaixa', 'selecionaTodosBaixa', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_baixa', 'selecionadoBaixa', " + rowIndex + ", '" + id + "', 'selecionaTodosBaixa', '" + gridName + "')", 2);

    return icon;
}


function formatDataTitulo(dataParametro) {
    if (hasValue(dataParametro)) {
        //    this.data = dojo.date.locale.parse(new Date(dataParametro),
        //        { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.data = dojo.date.locale.format(new Date(dataParametro),
            { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
        return this.data;
    } else {
        return "";
    }
}

function trocaLocalFormatter(id) {
    var data = [
        { name: "Não", id: false },
        { name: "Sim", id: true }

    ];

    var item = data.filter(function (item) {
        return item.id === id;
    });

    return hasValue(item) ? item[0].name : "";
}

function formatDataHora(dataParametro) {
    if (hasValue(dataParametro)) {
        //    this.data = dojo.date.locale.parse(new Date(dataParametro),
        //        { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.data = dojo.date.locale.format(new Date(dataParametro),
            { selector: "date", datePattern: "dd/MM/yyyy HH:mm:ss ", formatLength: "short", locale: "pt-br" });
        return this.data;
    }
}

function valorTituloFormatter(id) {
    return maskFixed(id, 2).toString();
}

function porcentagemFormatter(id) {
    return id + "%";
}

function montarMetodosBaixaAutomaticaCartao() {

    //Cria��o da Grade de sala
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
        "dojo/on",
        "dojo/query",
        "dojo/dom-attr",
        "dijit/form/Button",
        "dijit/form/TextBox",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dijit/form/FilteringSelect",
        "dojo/dom",
        "dijit/form/DateTextBox",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, on, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom, DateTextBox) {
        ready(function () {
            //*** Cria a grade de baixa cartao **\\
            dijit.byId('tabContainer').resize();
            var data = [];

            //GRID BAIXACHEQUE
            //var gridBaixaCartao = new EnhancedGrid({
            //    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) }),
            //    structure:
            //    [
            //        {
            //            name: "<input id='selecionaTodosTitulo' style='display:none'/>",
            //            field: "selecionadoTitulo",
            //            width: "5%",
            //            styles: "text-align:center; min-width:15px; max-width:20px;",
            //            formatter: formatCheckBoxBaixaCartao
            //        },
            //        { name: "Cartao", field: "no_local_movimento", width: "25%", styles: "min-width:80px;" },
            //        { name: "Pessoa", field: "no_pessoa", width: "40%", styles: "min-width:70px;" },
            //        { name: "Titulo", field: "nm_titulo", width: "70px", styles: "min-width:70px;text-align:center;" },
            //        { name: "Parcela", field: "nm_parcela_titulo", width: "70px", styles: "min-width:70px;text-align:center;" },
            //        { name: "Emissão", field: "dt_emissao_titulo", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
            //        { name: "Vencimento", field: "dt_vcto_titulo", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
            //        { name: "Valor", field: "vl_titulo", width: "70px", styles: "min-width:70px;text-align:right;", formatter: valorTituloFormatter },
            //        { name: "Valor Taxa", field: "vl_taxa", width: "15%", styles: "min-width:70px;text-align:right;", formatter: valorTituloFormatter },
            //        { name: "D+", field: "nm_dias_cartao", width: "70px", styles: "min-width:70px;text-align:center;" },
            //        { name: "Taxa(%)", field: "pc_taxa_cartao", width: "70px", styles: "min-width:70px;text-align:center;", formatter: porcentagemFormatter }
            //    ],
            //    //canSort: function(colIndex, field){
            //    //    return colIndex !=0 && field != 'selecionadoCurso';
            //    //},
            //    selectionMode: "single",
            //    noDataMessage: "Nenhum registro encontrado.",
            //    plugins: {
            //        pagination: {
            //            pageSizes: ["17", "28", "56", "100", "All"],
            //            description: true,
            //            sizeSwitch: true,
            //            pageStepper: true,
            //            defaultPageSize: "17",
            //            gotoButton: true,
            //            maxPageStep: 5,
            //            position: "bottom"
            //        }
            //    }
            //},"gridBaixaCartao");

            //gridBaixaCartao.pagination.plugin._paginator.plugin.connect(gridBaixaCartao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
            //    verificaMostrarTodos(evt, gridBaixaCartao, 'cd_titulo', 'selecionaTodosTitulo');
            //});
            //gridBaixaCartao.canSort = function (col) { return Math.abs(col) != 1; };
            //gridBaixaCartao.startup();

            sugereDataCorrente(function () { montarGridGeradas(true) });

            //montarGridGeradas();

            //GRID BAIXA
            var dataBaixa = [];
            var gridBaixas = new EnhancedGrid({
                store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataBaixa }) }),
                structure:
                [
                    { name: "<input id='selecionaTodosBaixa' style='display:none'/>", field: "selecionadoBaixa", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBaixas },
                    { name: "Local Movimento", field: "no_local", width: "25%", styles: "min-width:80px;" },
                    { name: "Emissor", field: "no_emissor", width: "25%", styles: "min-width:80px;" },
                    { name: "Pessoa", field: "no_pessoa", width: "30%", styles: "min-width:70px;" },
                    { name: "Titulo", field: "nm_titulo", width: "70px", styles: "min-width:70px;text-align:center;" },
                    { name: "Parcela", field: "nm_parcela", width: "70px", styles: "min-width:70px;text-align:center;" },
                    { name: "Vencimento", field: "dt_venc_ini", width: "70px", styles: "min-width:70px;" },
                    { name: "Dt.Baixa", field: "dt_baixa", width: "70px", styles: "min-width:70px;" },
                    { name: "Valor Baixa", field: "vl_cnab", width: "70px", styles: "min-width:70px;text-align:right;" },
                    { name: "Valor Taxa", field: "vl_taxa", width: "70px", styles: "min-width:80px;text-align:right;" }
                ],
                //canSort: function(colIndex, field){
                //    return colIndex !=0 && field != 'selecionadoCurso';
                //},
                selectionMode: "single",
                noDataMessage: msgNotRegEnc,
                plugins: {
                    pagination: {
                        pageSizes: ["17", "28", "56", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "17",
                        gotoButton: true,
                        maxPageStep: 5,
                        position: "bottom"
                    }
                }
            }, "gridBaixas");
            gridBaixas.canSort = function (col) { return Math.abs(col) != 1 };

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridBaixas, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosBaixa').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_baixa_automatica', 'selecionadoBaixa', -1, 'selecionaTodosBaixa', 'selecionaTodosBaixa', 'gridBaixas')", gridBaixas.rowsPerPage * 3);
                });
            });


            // Adiciona link de Todos os Itens:
            //menu = new DropDownMenu({ style: "height: 25px" });
            //var menuTodosItens = new MenuItem({
            //    label: "Todos Itens",
            //    onClick: function () { buscarTodosItens(gridBaixaCartao, 'todosItensBaixaCartao', ['pesquisarTitulos']); getTitulosForBaixaAutomatica(); }
            //});
            //menu.addChild(menuTodosItens);

            //var menuItensSelecionados = new MenuItem({
            //    label: "Itens Selecionados",
            //    onClick: function () {
            //        buscarItensSelecionados('gridBaixaCartao', 'selecionadoTitulo', 'cd_titulo', 'selecionaTodosTitulo', ['pesquisarTitulos'], 'todosItensBaixaCartao');
            //        //buscarItensSelecionados();
            //    }
            //});
            //menu.addChild(menuItensSelecionados);


            //button = new DropDownButton({
            //    label: "Todos Itens",
            //    name: "todosItensBaixaCartao",
            //    dropDown: menu,
            //    id: "todosItensBaixaCartao"
            //});
            //dom.byId("linkSelecionadosBaixaCartao").appendChild(button.domNode);

            // Adiciona link de ações para Cnab:
            //var menu = new DropDownMenu({ style: "height: 25px", id: "ActionMenu" });
            var menuT = new DropDownMenu({ style: "height: 25px", id: "ActionMenuT" });
            var menuB = new DropDownMenu({ style: "height: 25px", id: "ActionMenuB" });

            //var acaoExcluir = new MenuItem({
            //    label: "Excluir",
            //    onClick: function () {
            //        deletarTituloGrid(Memory, ObjectStore);
            //    }
            //});
            //menu.addChild(acaoExcluir);

            //var acaoEditar = new MenuItem({
            //    label: "Gerar Baixa",
            //    onClick: function () {
            //        gerarBaixaAutomatica(Memory, ObjectStore, dijit.byId("gridBaixaCartao"));
            //    }
            //});
            //menu.addChild(acaoEditar);

            //button = new DropDownButton({
            //    label: "Ações Relacionadas",
            //    name: "acoesRelacionadasBaixaCartao",
            //    dropDown: menu,
            //    id: "acoesRelacionadasBaixaCartao"
            //});
            //dom.byId("linkAcoesBaixaCartao").appendChild(button.domNode);

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    //if (dijit.byId('tabContainer').selectedChildWidget.id == "tgNovos") {
                    //    getTitulosForBaixaAutomatica();
                    //}
//                    if (dijit.byId('tabContainer').selectedChildWidget.id == "tgGeradas") {
                        //montarCadastroMotivoMatricula();
                        //listarBaixaAutomaticasEfetuadas();
                        montarGridGeradas();
//                    }
                    //if (dijit.byId('tabContainer').selectedChildWidget.id == "tgBaixas") {
                    //    dijit.byId("pesquisarTitulos").set('disabled', true);
                    //    console.log("passou");
                    //}

                }
            },
                "pesquisarTitulos");
            decreaseBtn(document.getElementById("pesquisarTitulos"), '32px');

            //new Button({
            //    id: "btnBaixaCartao",
            //    label: "Gerar",
            //    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
            //    onClick: function () {
            //        gerarBaixaAutomatica(Memory, ObjectStore, dijit.byId("gridBaixaCartao"));
            //    }
            //},
            //    "novoBaixaCartao");


            //LINK AÇOES GERADAS
            var acaoDetalhe = new MenuItem({
                label: "Listar Baixas",
                onClick: function () {
                    var grid = dijit.byId("gridGeradas");
                    if (grid.itensSelecionados != null && grid.itensSelecionados.length == 0 || grid.itensSelecionados == null) {
                        caixaDialogo(DIALOGO_AVISO, "Não foi selecionado registro para ver Detalhes.", null);
                    }
                    else if (grid.itensSelecionados != null && grid.itensSelecionados.length > 1) {
                        caixaDialogo(DIALOGO_AVISO, "Somente um registro pode ser selecionado para ver Detalhes.", null);
                    }
                    else {
                        dijit.byId('tgBaixas').set('disabled', false);
                        dijit.byId('gridBaixas').startup();
                        dijit.byId('gridBaixas').resize();
                        dijit.byId('tgBaixas').set('title', 'Baixas do código: ' + grid.itensSelecionados[0].cd_baixa_automatica);
                        //dijit.byId('tgBaixas').set('title', 'Baixas do código: ' + dijit.byId('gridGeradas')._by_idx[dijit.byId('gridGeradas').selection.selectedIndex].item.cd_baixa_automatica);
                        //dijit.byId('tabContainer_tablist').containerNode.childNodes[2].click;
                        var tabs = dijit.byId("tabContainer");
                        var pane = dijit.byId("tgBaixas");
                        tabs.selectChild(pane);
                        //dijit.byId("pesquisarTitulos").set('disabled', true);
                        getBaixasEfetuadasForBaixaAutomaticaCartao(grid.itensSelecionados[0]);
                    }
                }
            });

            menuT.addChild(acaoDetalhe);
            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasGeradas",
                dropDown: menuT,
                id: "acoesRelacionadasGeradas"
            });
            dom.byId("linkAcoesGeradas").appendChild(button.domNode);


            //LINK AÇÕES BAIXAS
            var acaoEditarB = new MenuItem({
                label: "Visualizar Baixa",
                onClick: function () {
                    eventoEditarBaixaTitulo(dijit.byId("gridBaixas").itensSelecionados, xhr, ready, Memory, FilteringSelect, on);
                    dijit.byId("gridBaixas").itensSelecionados = null;
                }
            });

            menuB.addChild(acaoEditarB);
            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasBaixas",
                dropDown: menuB,
                id: "acoesRelacionadasBaixas"
            });
            dom.byId("linkAcoesBaixas").appendChild(button.domNode);

            loadUsuario();
            getLocalMovtoBaixaAut();
            dijit.byId('tgBaixas').set('disabled', true);

            dijit.byId("lista_localmovt_cartao").on("change", function (cd_local) {
                dijit.byId("lista_localmovt_local_banco").reset();
                getLocaisMovtoBaixaAutById(cd_local);
            });

        });


    });


    //function deletarTituloGrid(Memory, ObjectStore) {
    //    try {
    //        var grid = dijit.byId("gridBaixaCartao");
    //        grid.store.save();
    //        var dados = grid.store.objectStore.data;

    //        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
    //            //Percorre a lista da grade para deleção (O(n)):
    //            for (var i = dados.length - 1; i >= 0; i--) {

    //                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
    //                if (binaryObjSearch(grid.itensSelecionados, 'cd_titulo', eval('dados[i].' + 'cd_titulo')) != null)
    //                    dados.splice(i, 1); // Remove o item do array
    //            }

    //            grid.itensSelecionados = new Array();
    //            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
    //            grid.setStore(dataStore);
    //            grid.update();


    //        }
    //        else
    //            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    //    }
    //    catch (e) {
    //        postGerarLog(e);
    //    }
    //}

    function loadUsuario() {
        try {
            dojo.xhr.get({
                url: Endereco() + "/auth/GetNomeCodigoUsuario",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    var retorno = jQuery.parseJSON(data).retorno;
                    nomeUsuarioLogado = retorno.no_login;
                    cdUsuarioLogado = retorno.cd_usuario;
                    dijit.byId("no_usuario").set("value", nomeUsuarioLogado);
                    dojo.byId("cdFkUsuario").value = cdUsuarioLogado;
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
    };


    function sugereDataCorrente(pFuncao) {
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
                        dijit.byId('dtaCadastro').set("value", date);
                        dijit.byId('dtaFinal').set("value", date);
                        dijit.byId('dtaInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));

                        var hora = dataSugerida[1].split(":");
                        //hora[0] = (parseInt(hora[0]) + 1).toString();
                        if (hasValue(hora) && hora[0].length < 2)
                            hora[0] = "0" + hora[0];
                        var horasTimeSpin = "T" + hora[0] + ":" + hora[1] + ":" + hora[2];
                        dijit.byId('horaCadastro').set("value", horasTimeSpin);
                    }
                    if (hasValue(pFuncao))
                        pFuncao.call();
                }
                else
                if (hasValue(pFuncao))
                    pFuncao.call();
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemPessoa', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    }

    function getLocalMovtoBaixaAut() {
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getLocalMovtoBaixaAut",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                response = jQuery.parseJSON(data);
                loadSelect(response.retorno, "lista_localmovt_cartao", 'cd_local_movto', 'no_local_movto');
            } catch (e) {
                postGerarLog(e);
            }

        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }

    function getLocaisMovtoBaixaAutById(cd_local_banco) {

        if (hasValue(cd_local_banco)) {
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/getLocaisMovtoBaixaAutById?cd_local_banco=" + cd_local_banco,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    response = jQuery.parseJSON(data);
                    if (hasValue(response.retorno)) {
                        dijit.byId('lista_localmovt_local_banco').setAttribute('disabled', false);
                        //dijit.byId('lista_localmovt_local_banco').set("required", true);
                        loadSelect(response.retorno, "lista_localmovt_local_banco", 'cd_local_movto', 'no_local_movto');
                    } else {
                        dijit.byId('lista_localmovt_local_banco').setAttribute('disabled', true);
                        //dijit.byId('lista_localmovt_local_banco').set("required", false);
                    }
                } catch (e) {
                    postGerarLog(e);
                }

            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        } else {
            dijit.byId("lista_localmovt_local_banco").reset();
            dijit.byId('lista_localmovt_local_banco').setAttribute('disabled', true);
            //dijit.byId('lista_localmovt_local_banco').set("required", false);
        }
    }

    //function gerarBaixaAutomatica(Memory, ObjectStore, grid) {

    //    try {
    //        grid.store.save();
    //        var dados = grid.store.objectStore.data;


    //        if (grid.itensSelecionados != null && grid.itensSelecionados.length == 0 || grid.itensSelecionados == null) {
    //            caixaDialogo(DIALOGO_AVISO, "Nenhum Título foi selecionado.", null);
    //        } else {
    //            var titulos = montarListIdTitulos(grid.itensSelecionados);
    //            if ((dijit.byId("formFiltros").validate() == false ||
    //                dijit.byId("dtaFinal").validate() == false ||
    //                dijit.byId("lista_localmovt_cartao").validate() == false ||
    //                dijit.byId('lista_localmovt_local_banco').validate() == false)) {
    //                return false;
    //            } else {
    //                var gridBaixaCartao = dijit.byId("gridBaixaCartao");
    //                showCarregando();
    //                dojo.xhr.post({
    //                    url: Endereco() + "/api/financeiro/gerarBaixaAutomatica",
    //                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
    //                    handleAs: "json",
    //                    postData: dojox.json.ref.toJson({
    //                        cd_local_movto: dijit.byId("lista_localmovt_cartao").get("value"),
    //                        cd_cartao_credito: dijit.byId("lista_localmovt_local_banco").get("value"), 
    //                        dt_inicial: dijit.byId("dtaInicial").get("value"),
    //                        dt_final: dijit.byId("dtaFinal").get("value"),
    //                        dh_baixa_automatica: montarDataHoraBaixaAutomatica(),
    //                        id_tipo: 1,
    //                        cds_titulos: titulos,
    //                        id_trocar_local: false
    //                    })
    //                }).then(function (data) {
    //                    try {
    //                        //data = jQuery.parseJSON(data).retorno;
    //                        //listarBaixaAutomaticasEfetuadas();
    //                        montarGridGeradas();
    //                        getTitulosForBaixaAutomatica();
    //                        //montarCadastroMotivoMatricula();
    //                        //gridBaixaCartao.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
    //                    } catch (e) {
    //                        postGerarLog(e);
    //                    }
    //                    showCarregando();
    //                },
    //                    function (error) {
    //                        showCarregando();
    //                        apresentaMensagem("apresentadorMensagem", error);
    //                    });

    //            }

    //            var tabs = dijit.byId("tabContainer");
    //            var pane = dijit.byId("tgGeradas");
    //            tabs.selectChild(pane);
    //        }

    //    }
    //    catch (e) {
    //        postGerarLog(e);
    //    }
    //}

    function montarGridGeradas(consultaInicial) {
        try {
            dijit.byId('lista_localmovt_local_banco').set("required", false);

            if ((dijit.byId("formFiltros").validate() == false ||
                dijit.byId("dtaFinal").validate() == false)) {
                //||
                //dijit.byId("lista_localmovt_cartao").validate() == false  ||
                //dijit.byId('lista_localmovt_local_banco').validate() == false)) {

                require([
                    "dojo/dom",
                    "dijit/registry",
                    "dojox/grid/EnhancedGrid",
                    "dojox/grid/enhanced/plugins/Pagination",
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory",
                    "dijit/form/Button",
                    "dijit/form/DropDownButton",
                    "dijit/DropDownMenu",
                    "dijit/MenuItem",
                    "dojo/ready"
                ], function (dom, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {

                    if (hasValue(dijit.byId("gridGeradas"))) {
                        dijit.byId("gridGeradas").destroyRecursive(true);
                    }
                    //if (typeof registry.byId("gridGeradas") != "undefined") {
                    //    registry.byId("gridGeradas").destroyRecursive();
                    //}

                    var gridGeradas = new EnhancedGrid({
                        store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: [] }) }),
                        structure: [
                            { name: "<input id='selecionaTodosGeradas' style='display:none'/>", field: "selecionadoGeradas", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxGeradas },
                            { name: "Código", field: "cd_baixa_automatica", width: "50px", styles: "min-width:50px;text-align:center;" },
                            { name: "Local", field: "no_local_movto", width: "25%", styles: "min-width:70px;" },
                            { name: "Usuario", field: "no_usuario", width: "15%", styles: "min-width:70px;" },
                            { name: "Data", field: "dh_baixa_automatica", width: "10%", styles: "min-width:70px;", formatter: formatDataHora },
                            { name: "Data Inicial", field: "dt_inicial", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
                            { name: "Data Final", field: "dt_final", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo }
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
                                maxPageStep: 4,
                                /*position of the pagination bar*/
                                position: "bottom"
                            }
                        }
                    }, "gridGeradas");
                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridGeradas.pagination.plugin._paginator.plugin.connect(gridGeradas.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                        verificaMostrarTodos(evt, gridGeradas, 'cd_baixa_automatica', 'selecionaTodosGeradas');
                    });
                    gridGeradas.canSort = function (col) { return Math.abs(col) != 1; };
                    gridGeradas.startup();

                    require(["dojo/aspect"], function (aspect) {
                        aspect.after(gridGeradas, "_onFetchComplete", function () {
                            // Configura o check de todos:
                            if (dojo.byId('selecionaTodosGeradas').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_baixa_automatica', 'selecionadoGeradas', -1, 'selecionaTodosGeradas', 'selecionaTodosGeradas', 'gridGeradas')", gridGeradas.rowsPerPage * 3);
                        });
                    });
                });

                return false;
            } else {

                require([
                    "dojo/dom",
                    "dijit/registry",
                    "dojox/grid/EnhancedGrid",
                    "dojox/grid/enhanced/plugins/Pagination",
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory",
                    "dijit/form/Button",
                    "dijit/form/DropDownButton",
                    "dijit/DropDownMenu",
                    "dijit/MenuItem",
                    "dojo/ready"
                ], function (dom, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {

                    if (hasValue(dijit.byId("gridGeradas"))) {
                        dijit.byId("gridGeradas").destroyRecursive(true);
                    }
                    //if (typeof registry.byId("gridGeradas") != "undefined") {
                    //    registry.byId("gridGeradas").destroyRecursive();
                        //}
                        var local = hasValue(dojo.byId("lista_localmovt_cartao").value) ? dijit.byId("lista_localmovt_cartao").get("value") : 0;
                    var storeGeradas = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/financeiro/listarBaixaAutomaticasEfetuadasCartao?cd_local_movto=" + local +
                                        "&cd_cartao_credito=0" + //dijit.byId("lista_localmovt_local_banco").get("value") +
                                        "&dt_inicial=" + montarDataBaixaAutomatica(dijit.byId("dtaInicial").get("value")) +
                                        "&dt_final=" + montarDataBaixaAutomatica(dijit.byId("dtaFinal").get("value")) +
                                        "&dh_baixa_automatica=" + montarDataHoraBaixaAutomatica() +
                                        "&id_tipo=" + 1 + "&id_trocar_local=" + false,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                    idProperty: "cd_baixa_automatica"
                                }
                                ), Memory({ idProperty: "cd_baixa_automatica" }));


                    var gridGeradas = new EnhancedGrid({
                        store: ObjectStore({ objectStore: consultaInicial == true ? new dojo.store.Memory({ data: null }) : storeGeradas }),
                        structure: [
                            { name: "<input id='selecionaTodosGeradas' style='display:none'/>", field: "selecionadoGeradas", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxGeradas },
                            { name: "Código", field: "cd_baixa_automatica", width: "50px", styles: "min-width:50px;text-align:center;" },
                            { name: "Local", field: "no_local_movto", width: "25%", styles: "min-width:70px;" },
                            { name: "Cartão", field: "dc_cartao_movto", width: "25%", styles: "min-width:80px;" },
                            { name: "Usuario", field: "no_usuario", width: "15%", styles: "min-width:70px;" },
                            { name: "Data", field: "dh_baixa_automatica", width: "15%", styles: "min-width:70px;", formatter: formatDataHora },
                            { name: "Data Inicial", field: "dt_inicial", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
                            { name: "Data Final", field: "dt_final", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo }
                        ],
                        noDataMessage: msgNotRegEncFiltro,
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                /*page step to be displayed*/
                                maxPageStep: 4,
                                /*position of the pagination bar*/
                                position: "bottom"
                            }
                        }
                    }, "gridGeradas");
                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridGeradas.pagination.plugin._paginator.plugin.connect(gridGeradas.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                        verificaMostrarTodos(evt, gridGeradas, 'cd_baixa_automatica', 'selecionaTodosGeradas');
                    });
                    gridGeradas.canSort = function (col) { return Math.abs(col) != 1; };
                    gridGeradas.startup();
                    gridGeradas.on("RowDblClick", function (evt) {
                        try {
                            var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                            if (hasValue(item)) {
                                dijit.byId('tgBaixas').set('disabled', false);
                                dijit.byId('gridBaixas').startup();
                                dijit.byId('gridBaixas').resize();
                                dijit.byId('tgBaixas').set('title', 'Baixas do código: ' + item.cd_baixa_automatica);
                                var tabs = dijit.byId("tabContainer");
                                var pane = dijit.byId("tgBaixas");
                                tabs.selectChild(pane);
                                getBaixasEfetuadasForBaixaAutomaticaCartao(item);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }, true);

                    require(["dojo/aspect"], function (aspect) {
                        aspect.after(gridGeradas, "_onFetchComplete", function () {
                            // Configura o check de todos:
                            if (dojo.byId('selecionaTodosGeradas').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_baixa_automatica', 'selecionadoGeradas', -1, 'selecionaTodosGeradas', 'selecionaTodosGeradas', 'gridGeradas')", gridGeradas.rowsPerPage * 3);
                        });
                    });
                });
            }

        } catch (e) {

        }

    }

    function getBaixasEfetuadasForBaixaAutomaticaCartao(itensSelecionados) {
        try {

            require([
                    "dojo/dom",
                    "dijit/registry",
                    "dojo/on",
                    "dojox/grid/EnhancedGrid",
                    "dojox/grid/enhanced/plugins/Pagination",
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory",
                    "dijit/form/Button",
                    "dijit/form/DropDownButton",
                    "dijit/DropDownMenu",
                    "dijit/MenuItem",
                    "dojo/_base/xhr",
                    "dijit/form/FilteringSelect",
                    "dojo/ready"
            ],
                function (dom,
                    registry,
                    on,
                    EnhancedGrid,
                    Pagination,
                    JsonRest,
                    ObjectStore,
                    Cache,
                    Memory,
                    Button,
                    DropDownButton,
                    DropDownMenu,
                    MenuItem,
                    xhr,
                    FilteringSelect,
                    ready) {

                    if (hasValue(dijit.byId("gridBaixas"))) {
                        dijit.byId("gridBaixas").destroyRecursive(true);
                    }
                    //if (typeof registry.byId("gridGeradas") != "undefined") {
                    //    registry.byId("gridGeradas").destroyRecursive();
                    //}
                    var local = hasValue(dojo.byId("lista_localmovt_cartao").value) ? dijit.byId("lista_localmovt_cartao").get("value") : 0;
                    var storeBaixas = Cache(
                        JsonRest({
                            target: Endereco() +
                                "/api/financeiro/getBaixasEfetuadasForBaixaAutomaticaCartao?cd_baixa_automatica=" + itensSelecionados.cd_baixa_automatica +
                                "&cd_local_movto=" + local +
                                "&cd_cartao_credito=0" + //dijit.byId("lista_localmovt_local_banco").get("value") +
                                "&dt_inicial=" + montarDataBaixaAutomatica(dijit.byId("dtaInicial").get("value")) +
                                "&dt_final=" + montarDataBaixaAutomatica(dijit.byId("dtaFinal").get("value")) +
                                "&dh_baixa_automatica=" + montarDataHoraBaixaAutomatica() +
                                "&id_tipo=" + 1 +
                                "&id_trocar_local=" + false,
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            idProperty: "cd_baixa_automatica"
                        }
                        ),
                        Memory({ idProperty: "cd_baixa_automatica" }));


                    var gridBaixas = new EnhancedGrid({
                        store: ObjectStore({ objectStore: storeBaixas }),
                        structure: [
                            { name: "<input id='selecionaTodosBaixa' style='display:none'/>", field: "selecionadoBaixa", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBaixas },
                            { name: "Local Movimento", field: "no_local_movto", width: "25%", styles: "min-width:80px;" },
                            { name: "Cartão", field: "dc_cartao_movto", width: "25%", styles: "min-width:80px;" },
                            { name: "Pessoa", field: "no_pessoa", width: "30%", styles: "min-width:70px;" },
                            { name: "Titulo", field: "nm_titulo", width: "70px", styles: "min-width:70px;text-align:center;" },
                            { name: "Parcela", field: "nm_parcela_titulo", width: "70px", styles: "min-width:70px;text-align:center;" },
                            { name: "Vencimento", field: "dt_vcto_titulo", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
                            { name: "Dt.Baixa", field: "dt_baixa_titulo", width: "70px", styles: "min-width:70px;", formatter: formatDataTitulo },
                            { name: "Valor Baixa", field: "vl_liquidacao_baixa", width: "70px", styles: "min-width:70px;text-align:right;", formatter: valorTituloFormatter },
                            { name: "Valor Taxa", field: "vl_taxa_cartao", width: "70px", styles: "min-width:80px;text-align:right;", formatter: valorTituloFormatter }
                        ],
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
                                maxPageStep: 4,
                                /*position of the pagination bar*/
                                position: "bottom"
                            }
                        }
                    },
                        "gridBaixas");
                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridBaixas.pagination.plugin._paginator.plugin.connect(
                        gridBaixas.pagination.plugin._paginator,
                        'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridBaixas, 'cd_baixa_automatica', 'selecionaTodosBaixa');

                        });

                    gridBaixas.canSort = function (col) { return Math.abs(col) != 1; };
                    gridBaixas.startup();
                    gridBaixas.on("RowDblClick", function (evt) {
                        try {
                            var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                            if (hasValue(item)) {
                                if (!hasValue(gridBaixas.itensSelecionados))
                                    caixaDialogo(DIALOGO_AVISO, "Não foi selecionado registro para listar Baixas.", null);
                                else {
                                    eventoEditarBaixaTitulo(gridBaixas.itensSelecionados, xhr, ready, Memory, FilteringSelect, on);
                                    dijit.byId("gridBaixas").itensSelecionados = null;
                                }
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }, true);

                    require(["dojo/aspect"],
                        function (aspect) {
                            aspect.after(gridBaixas,
                                "_onFetchComplete",
                                function () {
                                    // Configura o check de todos:
                                    if (dojo.byId('selecionaTodosBaixa').type == 'text')
                                        setTimeout(
                                            "configuraCheckBox(false, 'cd_baixa_automatica', 'selecionadoBaixa', -1, 'selecionaTodosBaixa', 'selecionaTodosBaixa', 'gridBaixas')",
                                            gridBaixas.rowsPerPage * 3);
                                });
                        });
                });


            return false;


        } catch (e) {

        }

    }

//    function getTitulosForBaixaAutomatica() {
//        try {

//            if ((dijit.byId("formFiltros").validate() == false ||
//                dijit.byId("dtaFinal").validate() == false ||
//                dijit.byId("lista_localmovt_cartao").validate() == false || 
//                dijit.byId('lista_localmovt_local_banco').validate() == false)) {
//                return false;
//            } else {

//                var gridBaixaCartao = dijit.byId("gridBaixaCartao");
//                showCarregando();
//                dojo.xhr.post({
//                    url: Endereco() + "/api/financeiro/getTitulosForBaixaAutomatica",
//                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
//                    handleAs: "json",
//                    postData: dojox.json.ref.toJson({
//                        cd_local_movto_cartao: dijit.byId("lista_localmovt_cartao").get("value"),
//                        cd_local_movto_banco: dijit.byId("lista_localmovt_local_banco").get("value"),
//                        dtInicial: dijit.byId("dtaInicial").get("value"),
//                        dtFinal: dijit.byId("dtaFinal").get("value")
//                    })
//                }).then(function (data) {
//                    try {
//                        //data = jQuery.parseJSON(data).retorno;
//                        gridBaixaCartao.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
//                        //dijit.byId("pesquisarTitulos").set('disabled', false);
//                    } catch (e) {
//                        postGerarLog(e);
//                    }
//                    showCarregando();
//                },
//                function (error) {
//                    apresentaMensagem("apresentadorMensagem", error);
//                    showCarregando();
//                });

//            }
//        } catch (e) {
//            postGerarLog(e);
//        }
//    };
};

function atualizaDetalheBaixaCartao(itensSelecionados) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getBaixasEfetuadasForBaixaAutomaticaCartao?cd_baixa_automatica=" + itensSelecionados.cd_baixa_automatica +
                        "&cd_local_movto=" + dijit.byId("lista_localmovt_cartao").get("value") +
                        "&dt_inicial=" + montarDataBaixaAutomatica(dijit.byId("dtaInicial").get("value")) +
                        "&dt_final=" + montarDataBaixaAutomatica(dijit.byId("dtaFinal").get("value")) +
                        "&dh_baixa_automatica=" + montarDataHoraBaixaAutomatica() + "&id_tipo=" + 1 + "&id_trocar_local=" + false,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_baixa_automatica"
                }
                ), Memory({ idProperty: "cd_baixa_automatica" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridBaixas = dijit.byId("gridBaixas");
            gridBaixas.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}


function montarDataHoraBaixaAutomatica() {
    try {
        var date = dojo.date.locale.format(new Date(),
            { selector: "date", datePattern: "MM/dd/yyyy HH:mm:ss", formatLength: "short", locale: "pt-br" });

        return date;
    } catch (e) {

    }
}

function montarDataBaixaAutomatica(data) {
    try {
        var date = dojo.date.locale.format(data,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });

        //sugereDataCorrente();

        //var date = dijit.byId('horaCadastro').value;

        //date.setDate(dijit.byId('dtaCadastro').value.getDate());
        //date.setMonth(dijit.byId('dtaCadastro').value.getMonth());
        //date.setFullYear(dijit.byId('dtaCadastro').value.getFullYear());
        return date;
    } catch (e) {

    }
}

function montarListIdTitulos(itens) {
    var result = [];
    if (hasValue(itens)) {
        $.each(itens,
            function (idx, value) {
                result.push(value.cd_titulo);
            });
    } else {
        caixaDialogo(DIALOGO_AVISO, "Nenhum Título foi selecionado.", null);
        return;
    }
    return result;
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        else
        //   if (dijit.byId('tabContainer').selectedChildWidget.id == "tgNovos") {
        //            //dijit.byId('gridGeradas').startup();
        //            dijit.byId('tgBaixas').set('disabled', true);
        //            dijit.byId('tgBaixas').set('title', 'Baixas');
        //            dijit.byId("pesquisarTitulos").set('disabled', false);
        //        }
        //else
           if (dijit.byId('tabContainer').selectedChildWidget.id == "tgGeradas") {
                //dijit.byId('gridGeradas').startup();
                dijit.byId('tgBaixas').set('disabled', true);
                dijit.byId('tgBaixas').set('title', 'Baixas');
                dijit.byId("pesquisarTitulos").set('disabled', false);
            }
            else {
               if (dijit.byId('tabContainer').selectedChildWidget.id == "tgBaixas") {
                    dijit.byId('tgBaixas').set('disabled', true);
                    dijit.byId('tgBaixas').set('title', 'Baixas');
                    dijit.byId("pesquisarTitulos").set('disabled', true);
                }
            }

            
        //if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tgGeradas') {
        //    montarGridGeradas();
        //}

    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarEventosBotoesPrincipaisCadTransacao(xhr, on) {
    try {
        dijit.byId("incluirBaixa").set("disabled", true)
        dijit.byId("alterarBaixa").set("disabled", true)
        dijit.byId("deleteBaixa").set("disabled", true)

        dijit.byId("incluirBaixa").on("click", function () {
            incluirBaixa(xhr, dojox.json.ref);
        });

        dijit.byId("alterarBaixa").on("click", function () {
            alterarBaixaTelaCartao(xhr, dojox.json.ref, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, dojo.on);
        });

        dijit.byId("deleteBaixa").on("click", function () {
            caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarTransFinan(xhr, dojox.json.ref, dojo.store.Memory, dojo.data.ObjectStore) });
        });

        dijit.byId("fecharBaixa").on("click",
            function () {
                dijit.byId("cadBaixaFinanceira").hide();
            });

        dijit.byId("cancelarBaixa").on("click", function () {
            limparCamposBaixaCad();
            eventoEditarBaixaTitulo(dijit.byId("gridBaixas").itensSelecionados, xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, on);
        });

        dijit.byId("limparBaixa").on("click", function () {
            var gridTitulo = dijit.byId('gridTitulo');
            var itensSelecionados = gridTitulo.itensSelecionados;
            showCarregando();
            limparCamposBaixaCad();
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            simularBaixaTitulos(itensSelecionados, xhr, dojox.json.ref, Permissoes);
        });
    } catch (e) {
        postGerarLog(e);
    }
}



function alterarBaixaTelaCartao(xhr, ref, ready, Memory, FilteringSelect, on) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCadBaixa', null);

        //if (!dijit.byId("formCadBaixa").validate()) {
        //    dijit.byId("tgCartao").set("open", true);
        //    return false;
        //}




        showCarregando();
        var transacao = montaListaBaixa();
        xhr.post({
            url: Endereco() + "/api/escola/postUpdateTransacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(transacao)
        }).then(function (data) {
            try {
                // apresentaMensagem('apresentadorMensagem', data);

                data = jQuery.parseJSON(data).retorno;
                console.log(data);
                var grid = dijit.byId("gridGeradas");
                if (grid.itensSelecionados != null && grid.itensSelecionados.length == 0 || grid.itensSelecionados == null) {
                    caixaDialogo(DIALOGO_AVISO, "Não foi selecionado registro para listar Baixas.", null);
                }
                else if (grid.itensSelecionados != null && grid.itensSelecionados.length > 1) {
                    caixaDialogo(DIALOGO_AVISO, "Somente um registro pode ser selecionado para listar Baixas.", null);
                }
                else {
                    atualizaDetalheBaixaCartao(grid.itensSelecionados[0]);
                    dijit.byId("cadBaixaFinanceira").hide();
                }
                
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCadBaixa', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}



function deletarTransFinan(xhr, ref) {
    try {
        var grid = dijit.byId("gridBaixas");
        if (grid.itensSelecionados != null && grid.itensSelecionados.length == 0 || grid.itensSelecionados == null) {
            caixaDialogo(DIALOGO_AVISO, "Não foi selecionado registro para ver Detalhes.", null);
        }
        else if (grid.itensSelecionados != null && grid.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, "Somente um registro pode ser selecionado para ver Detalhes.", null);
        }
        else {
            itensSelecionados = {
                cd_tran_finan: grid.itensSelecionados[0].cd_tran_finan
            };

            var gridGeradas = dijit.byId("gridGeradas");
            atualizaDetalheBaixaCartao(gridGeradas.itensSelecionados[0]);
            dijit.byId("cadBaixaFinanceira").hide();
        }

        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postDeleteTransFinanceiraBaixa",
            headers: { "Accept": "applicatsion/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {

                var grid = dijit.byId("gridGeradas");

                dijit.byId("cadBaixaFinanceira").hide();
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                showCarregando();
                if (!hasValue(dojo.byId("cd_tran_finan").style.display))
                    apresentaMensagem('apresentadorMensagemCadBaixa', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

