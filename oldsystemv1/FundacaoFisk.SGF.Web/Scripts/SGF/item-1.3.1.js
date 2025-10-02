
var HAS_ESTAGIO = 3;
var BIBLIOTECA = 3, MATERIAL = 1, ESTOQUE = 2, IMOBILIZADO = 4, SERVICO = 5, CUSTODESPESA = 6;
var NIVEL1 = 1, NIVEL2 = 2;

var PRIVADA = 1, PUBLICA = 2;
var ITEM = 6;
var origem = new Array(
    { name: "Produto Nacional", id: "0" },
    { name: "Importação - Compra", id: "1" },
    { name: "Importação Direta", id: "2" }
);

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainerItemServico");
        var pane = dijit.byId("tabPrincipal");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxPlano(value, rowIndex, obj) {
    try {
        var gridName = 'gridPlano';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPlano');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_subgrupo", grid._by_idx[rowIndex].item.cd_item_subgrupo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item_subgrupo', 'selecionadoPlano', -1, 'selecionaTodosPlano', 'selecionaTodosPlano', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item_subgrupo', 'selecionadoPlano', " + rowIndex + ", '" + id + "', 'selecionaTodosPlano', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarStatusTodos(nomElement) {
    require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (ready, Memory, filteringSelect) {
         try {
             var statusStore = new Memory({
                 data: [
                     { name: "Todos", id: "0" },
                     { name: "Sim", id: "1" },
                     { name: "Não", id: "2" }
                 ]
             });
             ready(function () {
                 var status = new filteringSelect({
                     id: nomElement,
                     name: "status",
                     value: "0",
                     store: statusStore,
                     searchAttr: "name",
                     style: "width: 75px;"
                 }, nomElement);
             });
         }
         catch (e) {
             postGerarLog(e);
         }
     });
};

//#endregion

//#region  métodos auxiliares

function loadCategoriaGrupoItem() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
		function (Memory, filteringSelect) {
		    try {
		        var items = [];
		        items.push({ id: 0, name: "Todos" });
		        items.push(
                    { name: "Privada", id: "1" },
		            { name: "Pública", id: "2" });
		        var statusStoreTipoConta = new Memory({
		            data: items
		        });

		        dijit.byId("categoria").store = statusStoreTipoConta;
		        dijit.byId("categoria").set("value", 0);
		    }
		    catch (e) {
		        postGerarLog(e);
		    }
		});
}

function limparFormItem() {
    try {
        dojo.byId('cd_item').value = 0;
        dijit.byId("nm_codigo_cest").reset();
        dojo.byId("categoriaGrupoCad").value = 0;
        getLimpar('#formItemServico');
        clearForm('formItemServico');
        getLimpar('#formDadosFiscais');
        clearForm('formDadosFiscais');
        getLimpar('#formAliquotas');
        clearForm('formAliquotas');
        getLimpar('#formValoresEscola');
        clearForm('formValoresEscola');
        getLimpar('#formItemBiblioteca');
        clearForm('formItemBiblioteca');
        limparGrid();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparSubGrupoTipo() {
    try {
        dijit.byId("tipoMov").reset();
        dijit.byId("cbSubGrupo2").reset();
        dijit.byId("ckManterCad").set("checked", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try {
        var cdItem = dojo.byId('cd_item').value;
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab))
            tab = dojo.query(e.target)[0];// Clicou na borda da aba

        if (jQuery.parseJSON(MasterGeral()) == false)
            dijit.byId("tabContainerItemServico_tablist_escolaContentPane").domNode.style.visibility = 'hidden';

        if (tab.getAttribute('widgetId') == "tabContainerItemServico_tablist_escolaContentPane") {
            popularGridEscola();
            dojo.byId('abriuEscola').value = true;
        }
        if (tab.getAttribute('widgetId') == "tabContainerItemServico_tablist_planoContentPane") {
            popularGridPlano();
            dojo.byId('abriuPlano').value = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function popularGridEscola() {
    try {
        var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;
        var gridEscolas = dijit.byId('gridEscolas');
        var popular = !eval(dojo.byId('abriuEscola').value);
        if (gridEscolas != null && gridEscolas.store != null && gridEscolas.store.objectStore.data.length == 0 && popular) {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getEscolatWithItem?cd_item=" + item,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagemItemServico', data);
                    var empresas = data.retorno;
                    gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                    gridEscolas.update();
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemItemServico', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function popularGridPlano() {
    try {
        var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;
        if (hasValue(item)) {
            var gridPlano = dijit.byId('gridPlano');
            var popular = !eval(dojo.byId('abriuPlano').value);
            if (gridPlano != null && gridPlano.store != null && gridPlano.store.objectStore.data.length == 0 && popular) {
                showCarregando();
                dojo.xhr.get({
                    url: Endereco() + "/api/financeiro/getSubGrupoTpHasItem?cd_item=" + item,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    data = jQuery.parseJSON(data);
                    apresentaMensagem('apresentadorMensagemItemServico', data);
                    var planos = data.retorno;
                    gridPlano.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: planos }) }));
                    gridPlano.update();
                    showCarregando();
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemItemServico', error);
                });
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, value, grid, ehLink) {
    try {
        getLimpar('#formItemServico');
        clearForm('formItemServico');
        limparGrid();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        dojo.byId('abriuEscola').value = false;
        dojo.byId('abriuPlano').value = false;

        dijit.byId("custo").set("disabled", true);
        dijit.byId("quantidade").set("disabled", true);

        var recover = ehLink == null;
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        var cdGrupo = hasValue(value.cd_grupo_estoque) && value.cd_grupo_estoque > 0 ? value.cd_grupo_estoque : 0;
        var cdTipoItem = hasValue(value.cd_tipo_item) && value.cd_tipo_item > 0 ? value.cd_tipo_item : 0;
        if (recover) recoverDataItem(cdGrupo, cdTipoItem, "grupoItem", "tipoItens");

        dojo.byId('abriuEscola').value = false;
        dojo.byId('abriuPlano').value = false;
        dijit.byId('idMaterial').set('checked', value.id_material_didatico);
        dijit.byId('idVoucher').set('checked', value.id_voucher_carga);
        dijit.byId('idqtVoucher').set('value', value.qt_horas_carga);

        dojo.byId("cd_item").value = value.cd_item;
        dojo.byId("nome").value = value.no_item;
        dojo.byId("sigla").value = hasValue(value.dc_sgl_item) ? value.dc_sgl_item : "";
        dojo.byId("dc_classificacao_fiscal").value = hasValue(value.dc_classificacao_fiscal) ? value.dc_classificacao_fiscal : "";
        dojo.byId("dc_classificacao").value = hasValue(value.dc_classificacao) ? value.dc_classificacao : "";
        dojo.byId("codigoIntegracao").value =value.cd_integracao;
        dojo.byId("icms").value = hasValue(value.icms) ? value.icms : 0;
        dojo.byId("iss").value = hasValue(value.iss) ? value.iss : 0;
        dojo.byId("codigoBarras").value = value.dc_codigo_barra;
        if (hasValue(dojo.byId('tipoItens').value, true)) {
            hasValue(value.cd_tipo_item) ? dijit.byId("tipoItens").set("value", value.cd_tipo_item) : 0;
        }
        if (hasValue(dojo.byId('grupoItem').value, true)) {
            hasValue(value.cd_grupo_estoque) ? dijit.byId("grupoItem").set("value", value.cd_grupo_estoque) : 0;
        }
        if (hasValue(dojo.byId('origem').value, true)) {
            hasValue(value.cd_origem_fiscal) ? dijit.byId("origem").set("value", value.cd_origem_fiscal) : 0;
        }
        if (dojo.byId("quantidade") != null)
            dojo.byId("quantidade").value = hasValue(value.qt_estoque) ? value.qt_estoque : 0;

        if (dojo.byId("venda") != null)
            dojo.byId("venda").value = hasValue(value.venda) ? value.venda : 0;

        if (dojo.byId("custo") != null)
            dojo.byId("custo").value = hasValue(value.custo) ? value.custo : 0;

        if (hasValue(value.cd_cest))
            dijit.byId("nm_codigo_cest").set("value", value.cd_cest);

        dojo.byId("titulo").value = hasValue(value.dc_titulo) ? value.dc_titulo : "";
        dojo.byId("autor").value = hasValue(value.no_autor) ? value.no_autor : "";
        dojo.byId("assunto").value = hasValue(value.dc_assunto) ? value.dc_assunto : "";
        dojo.byId("local").value = hasValue(value.dc_local) ? value.dc_local : "";
        dojo.byId("categoriaGrupoCad").value = value.id_categoria_grupo;

        if (value['id_item_ativo'] != null && value['id_item_ativo'] != undefined) {
	        dijit.byId("ativo").set("checked", value.id_item_ativo);
        }

        //*** if (jQuery.parseJSON(Master()) == true) criarGridEscola();
        esconderValoresItem(value.id_categoria_grupo, value.cd_tipo_item);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesSubGrupoTp(value, grid, ehLink) {
    try {
        limparSubGrupoTipo();
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
        document.getElementById("cd_subgrupo_tp").value = value.cd_item_subgrupo;
        dijit.byId("tipoMov").set("value", value.id_tipo_movimento);
        document.getElementById("tipoMov").value = value.no_tipo;
        document.getElementById("cd_subgrupo2").value = value.cd_subgrupo_conta;
        document.getElementById("cbSubGrupo2").value = value.no_subgrupo;
        dijit.byId("ckManterCad").set("checked", value.id_manter);
        document.getElementById('divExcluirPlano').style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Item servico
function formatCheckBoxItemServico(value, rowIndex, obj) {
    try {
        var gridName = 'gridItemServico';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosItemServico');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item", grid._by_idx[rowIndex].item.cd_item);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoItemServico', -1, 'selecionaTodosItemServico', 'selecionaTodosItemServico', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item', 'selecionadoItemServico', " + rowIndex + ", '" + id + "', 'selecionaTodosItemServico', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Escola
function formatCheckBoxEscola(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscola');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEscola', -1, 'selecionaTodosEscola', 'selecionaTodosEscola', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEscola', " + rowIndex + ", '" + id + "', 'selecionaTodosEscola', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//--**********************************--\\

function mostraTabs(Permissoes) {
    require([
            "dijit/registry",
            "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            try {
                if (!possuiPermissao('item', Permissoes)) {
                    registry.byId('tabItemServico').set('disabled', !registry.byId('tabItemServico').get('disabled'));
                    document.getElementById('tabItemServico').style.visibility = "hidden";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

//Métodos para carregar o Grupo de estoque
function recoverDataItem(cdGrupo, cdTipoItem, fieldGrupo, fieldTipo) {
    // Popula os produtos:
    cdGrupo = cdGrupo == null ? 0 : cdGrupo;
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/returnDataItem?cdGrupo=" + cdGrupo + "&cdTipoItem=" + cdTipoItem + "&tipoMovimento=",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataItem) {
            try {
                dataItem = jQuery.parseJSON(dataItem);
                if (!hasValue(dataItem.erro)) {
                    loadGrupoEstoque(dataItem.retorno.grupos, fieldGrupo);
                    loadTipoItem(dataItem.retorno.tipos, fieldTipo);
                    if (hasValue(cdGrupo) && cdGrupo > 0)
                        dijit.byId(fieldGrupo).set("value", cdGrupo);
                    if (hasValue(cdTipoItem) && cdTipoItem > 0)
                        dijit.byId(fieldTipo).set("value", cdTipoItem);
                }
                else
                    apresentaMensagem('apresentadorMensagemCurso', dataItem.erro);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}

//Métodos para carregar o Grupo de estoque
function recoverDataPesquisa(fieldGrupo, fieldTipo) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/returnDataItemPesq",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataItem) {
            try {
                dataItem = jQuery.parseJSON(dataItem);
                if (!hasValue(dataItem.erro)) {
                    loadGrupoEstoque(dataItem.retorno.grupos, fieldGrupo);
                    loadTipoItem(dataItem.retorno.tipos, fieldTipo);
                }
                else
                    apresentaMensagem('apresentadorMensagem', dataItem.erro);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function loadGrupoEstoque(items, linkGrupo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbGrupo = dijit.byId(linkGrupo);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbGrupo.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//fim Método Grupo
function loadTipoItem(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbTipoItem = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_item, name: value.dc_tipo_item });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipoItem.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function esconderValoresItem(categoriaGrupo, tipoItem) {
    try {
        var isMaster = dojo.byId('isMasterGeral').value;
        //dijit.byId("idMaterial").setAttribute('disabled', !isMaster);

        if (!$.parseJSON(isMaster))
            if (categoriaGrupo == PRIVADA) {
                dijit.byId("tipoItens").setAttribute('disabled', true);
                dijit.byId("grupoItem").setAttribute('disabled', true);
                dijit.byId("nome").setAttribute('disabled', true);
                dijit.byId("sigla").setAttribute('disabled', true);
                dijit.byId("codigoBarras").setAttribute('disabled', true);
                dijit.byId("ativo").setAttribute('disabled', true);
                dijit.byId("idMaterial").setAttribute('disabled', true);
                dijit.byId("codigoIntegracao").setAttribute('disabled', true);
                dijit.byId("origem").setAttribute('disabled', true);
                dijit.byId("dc_classificacao_fiscal").setAttribute('disabled', true);
                dijit.byId("dc_classificacao").setAttribute('disabled', true);
                dijit.byId("icms").setAttribute('disabled', true);
                dijit.byId("iss").setAttribute('disabled', true);
                if (tipoItem == BIBLIOTECA) {
                    dijit.byId("titulo").setAttribute('disabled', true);
                    dijit.byId("autor").setAttribute('disabled', true);
                    dijit.byId("assunto").setAttribute('disabled', true);
                    dijit.byId("local").setAttribute('disabled', true);
                }
                dijit.byId("tabContainerItemServico_tablist_planoContentPane").domNode.style.visibility = 'hidden';
            }
            else {
                dijit.byId("tipoItens").setAttribute('disabled', false);
                dijit.byId("grupoItem").setAttribute('disabled', false);
                dijit.byId("nome").setAttribute('disabled', false);
                dijit.byId("sigla").setAttribute('disabled', false);
                dijit.byId("codigoBarras").setAttribute('disabled', false);
                dijit.byId("ativo").setAttribute('disabled', false);
                dijit.byId("idMaterial").setAttribute('disabled', true);
                dijit.byId("codigoIntegracao").setAttribute('disabled', false);
                dijit.byId("origem").setAttribute('disabled', false);
                dijit.byId("dc_classificacao_fiscal").setAttribute('disabled', false);
                dijit.byId("dc_classificacao").setAttribute('disabled', false);
                dijit.byId("icms").setAttribute('disabled', false);
                dijit.byId("iss").setAttribute('disabled', false);
                dijit.byId("titulo").setAttribute('disabled', false);
                dijit.byId("autor").setAttribute('disabled', false);
                dijit.byId("assunto").setAttribute('disabled', false);
                dijit.byId("local").setAttribute('disabled', false);
                dijit.byId("tabContainerItemServico_tablist_planoContentPane").domNode.style.visibility = '';
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region Montar Casdastro
function montarCadastroItem() {
    //Criação da Grade

    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/FilteringSelect",
        "dojox/grid/enhanced/plugins/Filter",
        "dijit/Dialog"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, FilteringSelect, Filter) {
        ready(function () {
            try {
                //Tirando tag de alíquotas
                $("#PanelAliquotas").hide();
                montarStatus("statusItemServico");
                montarStatusTodos("incideBaixa");
                montarStatusTodos("pparc");
                loadCategoriaGrupoItem();
                criarOuCarregarCompFiltering('origem', origem, "width: 150px;", 0, ready, Memory, FilteringSelect, 'id', 'name', null);
                dijit.byId("origem").set("required", false);
                $("#PanelBiblioteca").hide();

                if (jQuery.parseJSON(MasterGeral()) == false)
                    dijit.byId("tabContainerItemServico_tablist_escolaContentPane").domNode.style.visibility = 'hidden';

                VerificarMasterGeral();
                recoverDataPesquisa("grupoPes", "tipo");
                var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/financeiro/getItemSearch?desc=&inicio=false&status=1&tipoItemInt=null&grupoItem=null&categoria=0&escola=true",
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_item"
                        }
                    ), Memory({ idProperty: "cd_item" }));
                var gridItemServico = new EnhancedGrid({
                    store: ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodosItemServico' style='display:none'/>", field: "selecionadoItemServico", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxItemServico },
                     //   { name: "Código", field: "cd_item", width: "5%", styles: "text-align: right; min-width:75px; max-width:75px;" },
                        { name: "Descrição", field: "no_item", width: "30%" },
                        { name: "Grupo", field: "no_grupo_estoque", width: "15%", styles: "text-align: center;" },
                        { name: "Tipo", field: "dc_tipo_item", width: "15%", styles: "text-align: center;" },
                        { name: "Categoria", field: "categoria_grupo", width: "10%", styles: "text-align: center;" },
                        { name: "Qtde.", field: "qt_estoque", width: "5%", styles: "text-align: right;" },
                        { name: "Ativo", field: "item_ativo", width: "10%", styles: "text-align: center;" }
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
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridItemServico"); // make sure you have a target HTML element with this id
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridItemServico.pagination.plugin._paginator.plugin.connect(gridItemServico.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridItemServico, 'cd_item', 'selecionaTodosItemServico');
                });
                var idGrupoItem = 0;
                gridItemServico.startup();
                gridItemServico.canSort = function (col) { return Math.abs(col) != 1; };
                gridItemServico.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                                  item = this.getItem(idx),
                                  store = this.store;
                        showCarregando();
                        limparFormItem();
                        dojo.byId('abriuEscola').value = false;
                        item.cd_grupo_estoque = item.cd_grupo_estoque == null ? 0 : item.cd_grupo_estoque;
                        xhr.get({
                            url: Endereco() + "/api/financeiro/returnDataItem?cdGrupo=" + item.cd_grupo_estoque + "&cdTipoItem=" + item.cd_tipo_item + "&tipoMovimento=",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataItem) {
                            try {
                                if (jQuery.parseJSON(MasterGeral()) == false)
                                    dijit.byId("tabContainerItemServico_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                                dataItem = jQuery.parseJSON(dataItem);
                                loadGrupoEstoque(dataItem.retorno.grupos, "grupoItem");
                                loadTipoItem(dataItem.retorno.tipos, "tipoItens");
                                apresentaMensagem('apresentadorMensagem', '');
                                var tabContainerItemServico = dijit.byId("tabContainerItemServico");
                                keepValues(null, item, gridItemServico, false);
                                setarTabCad();
                                dijit.byId("cadItemServico").show();
                                dijit.byId('tabContainerItemServico').resize();
                                IncluirAlterar(0, 'divAlterarItemServico', 'divIncluirItemServico', 'divExcluirItemServico', 'apresentadorMensagemItemServico', 'divCancelarItemServico', 'divLimparItemServico');
                                limparGrid();
                                hideCarregando();
                            }
                            catch (er) {
                                hideCarregando();
                                postGerarLog(er);
                            }
                        },
                            function (error) {
                                hideCarregando()
                            apresentaMensagem('apresentadorMensagemCurso', error);
                        });
                    }
                    catch (e) {
                        hideCarregando()
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridItemServico, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosItemServico').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoItemServico', -1, 'selecionaTodosItemServico', 'selecionaTodosItemServico', 'gridItemServico')", gridItemServico.rowsPerPage * 3);
                    });
                });

                //Verifica se o usuario é master/ desabilita a aba de escola
                if (jQuery.parseJSON(MasterGeral()) == false) {
                    dijit.byId("tabContainerItemServico_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                }

                //***************** Adiciona link de ações:****************************\\
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditarItemServico(gridItemServico.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridItemServico.itensSelecionados, 'DeletarItemServico(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

                var parametros = getParamterosURL();

                if (hasValue(parametros['tipoRetorno'])) {
                    var acaoCurso = new MenuItem({
                        label: "Curso",
                        onClick: function () { redirecionaMaterialDidatico(parametros['tipoRetorno']); }
                    });
                    menu.addChild(acaoCurso);

                    // Mosta a tela de cadastro de item pois se trata de um link:
                    //dijit.byId("cadItemServico").show();
                }

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasItemServico",
                    dropDown: menu,
                    id: "acoesRelacionadasItemServico"
                });
                dom.byId("linkAcoesItemServico").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridItemServico, 'todosItemServico', ['pesquisarItemServico', 'relatorioItemServico']); PesquisarItemServico(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridItemServico', 'selecionadoItemServico', 'cd_item', 'selecionaTodosItemServico', ['pesquisarItemServico', 'relatorioItemServico'], 'todosItensItemServico'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensItemServico",
                    dropDown: menu,
                    id: "todosItensItemServico"
                });
                dom.byId("linkSelecionadosItemServico").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                query("#pesquisaItemServico").on("keyup", function (e) {
                    if (e.keyCode == 13) {
                        apresentaMensagem('apresentadorMensagem', null);
                        PesquisarItemServico(true);
                    }
                });
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        IncluirItemServico();
                    }
                }, "incluirItemServico");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        AlterarItemServico();
                    }
                }, "alterarItemServico");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarItemServico();
                        });
                    }
                }, "deleteItemServico");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        limparFormItem()
                    }
                }, "limparItemServico");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        showCarregando();
                        keepValues(null, null, gridItemServico, null);
                        setarTabCad();
                    }
                }, "cancelarItemServico");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadItemServico").hide();
                    }
                }, "fecharItemServico");

                // Crud Plano de Contas    
                new Button({
                    label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        IncluirSubGrupo();
                    }
                }, "incluirPlano");

                new Button({
                    label: "Alterar", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        editarSubgrupo();
                    }
                }, "alterarPlano");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        limparSubGrupoTipo();
                    }
                }, "limparPlano");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent'
                }, "cancelarPlano");
                new Button({
                    label: "Fechar",
                    iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogPlano").hide();
                    }
                }, "fecharPlano");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        PesquisarItemServico(true);
                    }
                }, "pesquisarItemServico");
                decreaseBtn(document.getElementById("pesquisarItemServico"), '32px');
                // Adiciona link de ações Plano de Contas:
                var menuPlano = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarSubgrupo(gridPlano.itensSelecionados); }
                });
                menuPlano.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_item_subgrupo', gridPlano);
                    }
                });
                menuPlano.addChild(acaoRemover);

                //Botao Incluir Planos de Contas
                var buttonPlano = new Button({
                    label: "Incluir",
                    name: "itensPlano",
                    iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    id: "itensPlano",
                    onClick: function () {
                        IncluirAlterar(1, 'divAlterarPlano', 'divIncluirPlano', 'divExcluirPlano', 'apresentadorMensagemPlano', 'divCancelarPlano', 'divLimparPlano');
                        limparSubGrupoTipo();
                        dijit.byId("dialogPlano").show();
                    }
                });
                dom.byId("btnaddPlano").appendChild(buttonPlano.domNode);
                var buttonEscola = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPlano",
                    dropDown: menuPlano,
                    id: "acoesRelacionadasPlano"
                });
                dom.byId("linkPlano").appendChild(buttonEscola.domNode);

                //#region grade da escola
                if (jQuery.parseJSON(MasterGeral()) == true) {
                    var gridEscolas = new EnhancedGrid({
                        store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                        structure: [
                            { name: "<input id='selecionaTodosEscola' style='display:none'/>", field: "selecionadoEscola", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxEscola, filterable: false},
                            { name: "Escola", field: "dc_reduzido_pessoa", width: "98%" }
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
                                plugins: { nestedSorting: true }
                            },
                            filter: {
	                            closeFilterbarButton: true,
	                            ruleCount: 1,
                                itemsName: "Escola"
                            }
                        }
                    }, "gridEscolas");
                    gridEscolas.canSort = function (col) { return Math.abs(col) != 1; };
                    gridEscolas.startup();
                }
                var gridPlano = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosPlano' style='display:none'/>", field: "selecionadoPlano", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPlano },
                        { name: "Tipo", field: "no_tipo", width: "35%", styles: "min-width:80px;" },
                        { name: "SubGrupo", field: "no_subgrupo", width: "60%", styles: "min-width:80px;" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridPlano");
                gridPlano.canSort = function () { return true };
                gridPlano.startup();
                gridPlano.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        keepValuesSubGrupoTp(item, gridPlano, false);
                        IncluirAlterar(0, 'divAlterarPlano', 'divIncluirPlano', 'divExcluirPlano', 'apresentadorMensagemPlano', 'divCancelarPlano', 'divLimparPlano');
                        dijit.byId("dialogPlano").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

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
                                    abrirPessoaFK(false);
                                });
                            else
                                abrirPessoaFK(false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirEscolaFK");

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_pessoa', gridEscolas);
                    }
                });
                menu.addChild(menuTodosItens);

                var acaoFiltro = new MenuItem({
	                label: "Mostrar Filtro",
	                onClick: function () {
		                dijit.byId("gridEscolas").showFilterBar(true);
	                }
                });
                menu.addChild(acaoFiltro);

                if (!hasValue(dijit.byId('todosItensEscola'))) {
                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "todosItensEscola",
                        dropDown: menu,
                        id: "todosItensEscola"
                    });
                    dom.byId("linkSelecionadosEscola").appendChild(button.domNode);
                }//if

                //#endregion
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {

                            limparFormItem();
                            showCarregando();
                            setTimeout(esconderValoresItem(0, 0), 1000);
                            recoverDataItem(0, 0, "grupoItem", "tipoItens");
                            var tabContainerItemServico = dijit.byId("tabContainerItemServico");
                            tabContainerItemServico.selectChild(tabContainerItemServico.getChildren()[0]);
                            apresentaMensagem('apresentadorMensagem', null);
                            //*** if (jQuery.parseJSON(Master()) == true) criarGridEscola();
                            IncluirAlterar(1, 'divAlterarItemServico', 'divIncluirItemServico', 'divExcluirItemServico', 'apresentadorMensagemItemServico', 'divCancelarItemServico', 'divLimparItemServico');
                            dojo.byId("dc_classificacao_fiscal").value = '';

                            dojo.byId('abriuEscola').value = false;
                            dojo.byId('abriuPlano').value = false;
                            if (jQuery.parseJSON(MasterGeral()) == false) {
                                dijit.byId("custo").set("disabled", false);
                                dijit.byId("quantidade").set("disabled", false);
                            }
                            else
                            {
                                dijit.byId("custo").set("disabled", true);
                                dijit.byId("quantidade").set("disabled", true);
                            }
                            setarTabCad();
                            dijit.byId("cadItemServico").show();
                            dijit.byId('tabContainerItemServico').resize();
                            $("#tagFiscal").show();
                            // $("#PanelAliquotas").show();
                            $("#valoresEscola").show();

                            $("#PanelBiblioteca").hide();
                            limparGrid();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoItemServico");
                //----Monta o botão de Relatório----

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                            var tipoItem = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : 0;
                            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : 0;
                            xhr.get({
                                url: Endereco() + "/api/financeiro/GetUrlRelatorioItemServico?" + getStrGridParameters('gridItemServico') + "desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" + retornaStatus("statusItemServico") + "&tipoItemInt=" + tipoItem + "&grupoItem=" + grupoItem + "&categoria=" + retornaStatus("categoria") + "&escola=true",
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        })
                    }
                }, "relatorioItemServico");

                //Regra para a biblioteca
                dijit.byId("tipoItens").on("change", function (e) {
                    try {
                        switch (e) {
                            case BIBLIOTECA:
                                $("#PanelBiblioteca").show();
                                dijit.byId("titulo").set('value', dijit.byId('nome').get('value'));
                                dijit.byId("iss").set('disabled', true);
                                dijit.byId("valoresEscola").set("open", false);
                                $("#tagFiscal").show();
                                dojo.byId("trCodigos").style.display = "";
                                dojo.byId("lbSigla").style.display = "";
                                dojo.byId("tdSigla").style.display = "";
                                dojo.byId("lbCdBarra").style.display = "";
                                dojo.byId("tdCdBarra").style.display = "";
                                dijit.byId("iss").set('disabled', false);
                                dojo.byId("lbMaterial").style.display = "none";
                                dojo.byId("tdMaterial").style.display = "none";
                                dojo.byId("lbVoucher").style.display = "none";
                                dojo.byId("tdVoucher").style.display = "none";
                                dojo.byId("lbqtVoucher").style.display = "none";
                                dojo.byId("tdqtVoucher").style.display = "none";
                                break;
                            case IMOBILIZADO:
                            case ESTOQUE:
                                $("#PanelBiblioteca").hide();
                                dijit.byId("titulo").set('value');
                                dijit.byId("valoresEscola").set("open", true);
                                dijit.byId("iss").set('disabled', true);
                                $("#tagFiscal").show();
                                dojo.byId("trCodigos").style.display = "";
                                dojo.byId("lbMaterial").style.display = "none";
                                dojo.byId("tdMaterial").style.display = "none";
                                dojo.byId("lbVoucher").style.display = "none";
                                dojo.byId("tdVoucher").style.display = "none";
                                dojo.byId("lbqtVoucher").style.display = "none";
                                dojo.byId("tdqtVoucher").style.display = "none";
                                dojo.byId("lbSigla").style.display = "";
                                dojo.byId("tdSigla").style.display = "";
                                dojo.byId("lbCdBarra").style.display = "";
                                dojo.byId("tdCdBarra").style.display = "";
                                break;
                             case CUSTODESPESA:
                                $("#PanelBiblioteca").hide();
                                dijit.byId("iss").set('disabled', false);
                                dojo.byId("trCodigos").style.display = "none";
                                $("#tagFiscal").hide();
                                dijit.byId("titulo").set('value');
                                dojo.byId("lbSigla").style.display = "none";
                                dojo.byId("tdSigla").style.display = "none";
                                dojo.byId("lbCdBarra").style.display = "none";
                                dojo.byId("tdCdBarra").style.display = "none";
                                dojo.byId("lbMaterial").style.display = "none";
                                dojo.byId("tdMaterial").style.display = "none";
                                dojo.byId("lbVoucher").style.display = "none";
                                dojo.byId("tdVoucher").style.display = "none";
                                dojo.byId("lbqtVoucher").style.display = "none";
                                dojo.byId("tdqtVoucher").style.display = "none";
                                dijit.byId("valoresEscola").set("open", false);
                                break;
                            case SERVICO:
                                $("#PanelBiblioteca").hide();
                                dijit.byId("icms").set('disabled', true);
                                dojo.byId("trCodigos").style.display = "none";
                                $("#tagFiscal").hide();
                                dijit.byId("titulo").set('value');
                                dojo.byId("lbSigla").style.display = "none";
                                dojo.byId("tdSigla").style.display = "none";
                                dojo.byId("lbCdBarra").style.display = "none";
                                dojo.byId("tdCdBarra").style.display = "none";
                                dojo.byId("lbVoucher").style.display = "";
                                dojo.byId("tdVoucher").style.display = "";
                                dojo.byId("lbqtVoucher").style.display = "";
                                dojo.byId("tdqtVoucher").style.display = "";
                                dijit.byId("valoresEscola").set("open", false);
                                break;
                            case MATERIAL:
                                $("#PanelBiblioteca").hide();
                                $("#tagFiscal").show();
                                dijit.byId("titulo").set('value');
                                dojo.byId("lbSigla").style.display = "";
                                dojo.byId("tdSigla").style.display = "";
                                dojo.byId("lbCdBarra").style.display = "";
                                dojo.byId("tdCdBarra").style.display = "";
                                dojo.byId("lbVoucher").style.display = "none";
                                dojo.byId("tdVoucher").style.display = "none";
                                dojo.byId("lbqtVoucher").style.display = "none";
                                dojo.byId("tdqtVoucher").style.display = "none";
                                dojo.byId("trCodigos").style.display = "";
                                dojo.byId("lbMaterial").style.display = "";
                                dojo.byId("tdMaterial").style.display = "";
                                dijit.byId("idMaterial").set('disabled', e != MATERIAL || !dojo.byId('isMasterGeral').value);
                                dijit.byId("iss").set('disabled', true);
                                dijit.byId("valoresEscola").set("open", true);
                                break;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("nome").on("change", function (e) {
                    try {
                        var tipoItem = dijit.byId('tipoItens').value;
                        if (tipoItem == BIBLIOTECA)
                            dijit.byId("titulo").set('value', dijit.byId('nome').get('value'));
                        else
                            dijit.byId("titulo").set('value');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("grupoItem").on("change", function (e) {
                    esconderValoresItem(dojo.byId("categoriaGrupoCad").value, dijit.byId("tipoItens").value);
                });

                var buttonFkArray = ['pesquisarItem', 'pesquisarItem'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                montarStatus("statusItem");
                //configuraMateriaisDidaticos(gridItemServico);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc423101931', '765px', '771px');
                        });
                }

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            dojo.byId("nivel").value = NIVEL2;
                            if (!hasValue(dijit.byId("gridSubgrupoContaFK"))){
								showCarregando();
                                montarSubgrupoContas(
                                    function () {
                                        dijit.byId("pesquisarSubgrupoContaFK").on("click", function (e) {
											showCarregando();
                                            apresentaMensagem("apresentadorMensagemProPessoa", null);
                                            pesquisarSubgrupoContasFK(true, NIVEL2);
                                        });
                                    },
                                    NIVEL2);
							}
                            else{
								showCarregando();
                                pesquisarSubgrupoContasFK(true, NIVEL2);
							}

                            funcaoFKSubgrupo(xhr, ObjectStore, Memory);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btSubGrupo2");

                var statusStore = new Memory({
                        data: [
                        { name: "Entrada", id: 1 },
                        { name: "Saída", id: 2 },
                        { name: "Despesa", id: 3 },
                        { name: "Entrada de Serviço", id: 7 },
                        { name: "Saída de Serviço", id: 4 },
                        { name: "Devolução de entrada", id: 5 },
                        { name: "Devolução de saída", id: 6 }
                        ]
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323066', '765px', '771px');
                        });
                }
                var tipoOrigens = new FilteringSelect({
                    id: "tipoMov",
                    name: "tipoMov",
                    store: statusStore,
                    searchAttr: "name",
                    style: "width:100%;"
                }, "tipoMovs");
                var buttonArray = ['btSubGrupo2'];
                diminuirBotoes(buttonArray);

                if (hasValue(document.getElementById("limparSubGrupo2"))) {
                    document.getElementById("limparSubGrupo2").parentNode.style.minWidth = '40px';
                    document.getElementById("limparSubGrupo2").parentNode.style.width = '40px';
                }
                adicionarAtalhoPesquisa(['pesquisaItemServico', 'statusItemServico', 'grupoPes', 'tipo', 'categoria'], 'pesquisarItemServico', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}
//#endregion

//#region criarGridEscola -   configuraMateriaisDidaticos - atualizaGridItem - redirecionaMaterialDidatico -  limparItem - pesquisarMatDid -VerificarMasterGeral

function configuraMateriaisDidaticos(gridMatDid) {
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try {
                var parametros = getParamterosURL();
                var link = {
                    TipoRetorno: parametros['tipoRetorno'],
                    Selecionados: null,
                    DadosRetorno: null
                };
                var parametroLink = '';

                if (hasValue(parametros['tipoRetorno']))
                    parametroLink = '?tipoRetorno=' + parametros['tipoRetorno'];

                xhr.post(Endereco() + "/financeiro/linkItens" + parametroLink, {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        linkMateriaisDidaticos: link
                    })
                }).then(function (data) {
                    data = data.retorno;
                    if (hasValue(data.Selecionados))
                        gridMatDid.itensSelecionadosLink = data.Selecionados;
                    else
                        gridMatDid.itensSelecionadosLink = [];

                    // Atualiza com timeout para funcionar no IE:
                    setTimeout('atualizaGridItem()', 10);
                }, function (error) {
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function atualizaGridItem() {
    try {
        dijit.byId('gridItemServico').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function redirecionaMaterialDidatico(tipoRetorno) {
    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        try {
            var selecionados = [];
            var selecinadosRetorno = [];//usado para manipular o objeto a ser retornado so com as propriedades necessárias para o link.
            var gridItemServico = dijit.byId('gridItemServico');
            var itensSelecionados = dijit.byId('gridItemServico').itensSelecionados;

            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Selecione algum item do tipo Material Didático para relacioná-lo ao curso.');
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }

            if (hasValue(gridItemServico.itensSelecionadosLink) && gridItemServico.itensSelecionadosLink.length > 0) {
                selecionados = gridItemServico.itensSelecionadosLink;
                for (var i = 0; i < selecionados.length; i++) {
                    var itemReturn = {
                        cd_item: selecionados[i].cd_item,
                        cd_tipo_item: selecionados[i].cd_tipo_item,
                        item_ativo: selecionados[i].id_item_ativo,
                        no_item: selecionados[i].no_item
                    };
                    selecinadosRetorno.push(itemReturn);
                }
            }

            for (var i = 0; i < itensSelecionados.length; i++) {
                var novoItem = {
                    cd_item: itensSelecionados[i].cd_item,
                    no_item: itensSelecionados[i].no_item,
                    item_ativo: itensSelecionados[i].item_ativo,
                    cd_tipo_item: itensSelecionados[i].cd_tipo_item
                };
                //selecionados.push(novoItem);
                insertObjSort(selecinadosRetorno, 'cd_item', novoItem, false);
            }

            xhr.post(Endereco() + "/financeiro/linkItensRetorno", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    TipoRetorno: tipoRetorno,
                    Selecionados: selecinadosRetorno,
                    DadosRetorno: null
                })
            }).then(function (data) {
                try {
                    if (hasValue(data.retorno))
                        window.location = data.retorno;
                    else
                        apresentaMensagem('apresentadorMensagem', data);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparItem() {
    try {
        apresentaMensagem('apresentadorMensagemItem', null);
        getLimpar('#formItem');
        clearForm('formItem');
        IncluirAlterar(1, 'divAlterarItem', 'divIncluirItem', 'divExcluirItem', 'apresentadorMensagemItem', 'divCancelarItem', 'divLimparItem');
        document.getElementById("cd_item").value = '';
        dojo.byId("categoriaGrupoCad").value = '';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarMatDid(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getItemSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaMatDid").value) + "&inicio=false&status=1&tipoItemInt=1" + "&categoria=" + retornaStatus("categoria") + "&escola=false",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridMaterialDidatico");

            if (limparItens)
                grid.itensSelecionados = [];
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

// Recupera o master geral do sistema
function VerificarMasterGeral() {
    require(["dojo/request/xhr"], function (xhr) {
        xhr(Endereco() + "/api/escola/VerificarMasterGeral/", {
            handleAs: "json",
            headers: { Accept: "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                dojo.byId('isMasterGeral').value = data.retorno;
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        })
    });
}
//#endregion

// #region Métodos de persistencia para Item Servico
function montarEscolasGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        var listaEscolas = [];
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null && gridEscolas.store.objectStore != null && gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = [];
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {
                    listaEscolas.push({
                        cd_pessoa: escolas[i].cd_pessoa,
                        no_pessoa: escolas[i].no_pessoa
                    });
                }
            }
        }
        return listaEscolas;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarItemSubgrupoGrid() {
    try {
        var gridPlano = dijit.byId('gridPlano');
        var listaPlanos = [];
        if (hasValue(gridPlano)) {
            if (gridPlano.store != null && gridPlano.store.objectStore != null && gridPlano.store.objectStore.data.length > 0)
                listaPlanos = [];
            if (hasValue(gridPlano) && gridPlano.store.objectStore.data.length > 0) {
                var planos = gridPlano.store.objectStore.data;
                for (var i = 0; i < planos.length; i++) {
                    listaPlanos.push({
                        id_tipo_movimento: planos[i].id_tipo_movimento,
                        cd_item: dojo.byId("cd_item").value,
                        cd_subgrupo_conta: planos[i].cd_subgrupo_conta,
                        cd_item_subgrupo: planos[i].cd_item_subgrupo
                    });
                }
            }
        }
        return listaPlanos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Item Servico
function PesquisarItemServico(limparItens) {
    var tipoItem = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : null;
    var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getItemSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" + retornaStatus("statusItemServico") + "&tipoItemInt=" + tipoItem + "&grupoItem=" + grupoItem + "&categoria=" + retornaStatus("categoria") + "&escola=true",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_item"
                }
                    ), Memory({ idProperty: "cd_item" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridItemServico = dijit.byId("gridItemServico");
            if (limparItens) {
                gridItemServico.itensSelecionados = [];
            }
            gridItemServico.noDataMessage = msgNotRegEnc;
            gridItemServico.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function ObjItem() {
    try {
        var quantidade = hasValue(dojo.number.parse(dijit.byId("quantidade").get('value'))) ? dojo.number.parse(dijit.byId("quantidade").get('value')) : 0;

        var valorItem = 0;
        if (dojo.byId("venda") != null)
            valorItem = hasValue(dojo.number.parse(dojo.byId("venda").value)) ? dojo.number.parse(dojo.byId("venda").value) : 0;

        var valorCusto = 0;
        if (dojo.byId("custo") != null)
            valorCusto = hasValue(dojo.number.parse(dojo.byId("custo").value)) ? dojo.number.parse(dojo.byId("custo").value) : 0;

        var titulo = dijit.byId("titulo").get('value');
        var autor = hasValue(dojo.byId("autor").value) ? dojo.byId("autor").value : "";
        var assunto = hasValue(dojo.byId("assunto").value) ? dojo.byId("assunto").value : "";
        var local = hasValue(dojo.byId("local").value) ? dojo.byId("local").value : "";

        this.cd_item = dojo.byId("cd_item").value;
        this.no_item = dojo.byId("nome").value;
        this.dc_sgl_item = hasValue(dijit.byId("sigla").get("value")) ? dijit.byId("sigla").get("value") : null;
        this.dc_classificacao_fiscal = hasValue(dijit.byId("dc_classificacao_fiscal").get("value")) ? dijit.byId("dc_classificacao_fiscal").get("value") : null;
        this.dc_classificacao = hasValue(dijit.byId("dc_classificacao").get("value")) ? dijit.byId("dc_classificacao").get("value") : null;
        this.cd_integracao = hasValue(dijit.byId("codigoIntegracao").get("value") ) ? dijit.byId("codigoIntegracao").get("value") : "";
        this.pc_aliquota_icms = dijit.byId("icms").get("value") > 0 ? dijit.byId("icms").get("value") : 0;
        this.pc_aliquota_iss = dijit.byId("iss").get("value") > 0 ? dijit.byId("iss").get("value") : 0;
        this.dc_codigo_barra = hasValue(dijit.byId("codigoBarras").get("value")) ? dijit.byId("codigoBarras").get("value") : null;
        this.id_item_ativo = dijit.byId("ativo").checked;
        this.id_material_didatico = dijit.byId("idMaterial").checked;
        this.id_voucher_carga = dijit.byId("idVoucher").checked;
        this.qt_horas_carga = dijit.byId("idqtVoucher").get("value");
        this.cd_tipo_item = dijit.byId("tipoItens").get("value");
        this.cd_grupo_estoque = dijit.byId("grupoItem").get("value");
        this.dc_tipo_item = dojo.byId("tipoItens").value;
        this.no_grupo_estoque = dojo.byId("grupoItem").value;
        this.cd_origem_fiscal = dijit.byId("origem").get("value");
        this.qt_estoque = quantidade;
        this.vl_item = valorItem;
        this.vl_custo = valorCusto;
        this.dc_titulo = titulo;
        this.no_autor = autor;
        this.dc_assunto = assunto;
        this.dc_local = local;
        this.cd_cest = hasValue(dijit.byId("nm_codigo_cest").get("value")) ? dijit.byId("nm_codigo_cest").get("value") : null;
        this.hasClickEscola = dojo.byId('abriuEscola').value;


        this.escolas = montarEscolasGrid();
        this.itemSubgrupo = montarItemSubgrupoGrid();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//IncluirItemServico
function IncluirItemServico() {
    var masterGeral = jQuery.parseJSON(MasterGeral()) == true ? true : false;

    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemItemServico', null);
    var item = new ObjItem();

    require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {
        if (!validarCamposItemServico(windows))
            return false;

        showCarregando();

        xhr.post(Endereco() + "/api/escola/postInsertItemServico", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(item)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridItemServico';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadItemServico").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_item", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoItemServico', 'cd_item', 'selecionaTodosItemServico', ['pesquisarItemServico', 'relatorioItemServico'], 'todosItensItemServico');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_item");
                    showCarregando();

                } else {
                    apresentaMensagem('apresentadorMensagemItemServico', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            $('#aguardar').css('display', "none");
            showCarregando();
            apresentaMensagem('apresentadorMensagemItemServico', error.response.data);
        });
    });
}

// Deletar  item servico
function DeletarItemServico(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_item').value != 0)
                itensSelecionados = [{
                    cd_item: dom.byId("cd_item").value,
                    no_item: dojo.byId("nome").value,
                    cd_grupo_estoque: dijit.byId("grupoItem").value
                }];
        xhr.post({
            url: Endereco() + "/api/Financeiro/postdeleteitemservico",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensItemServico");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadItemServico").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridItemServico').itensSelecionados, "cd_item", itensSelecionados[r].cd_item);
                    PesquisarItemServico(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisaItemServico").set('disabled', false);
                    dijit.byId("relatorioItemServico").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } else {
                    if (!hasValue(dojo.byId("cadItemServico").style.display))
                        apresentaMensagem('apresentadorMensagemItemServico', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadItemServico").style.display))
                apresentaMensagem('apresentadorMensagemItemServico', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    })
}

function AlterarItemServico() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemItemServico', null);

        var item = new ObjItem();

        if (jQuery.parseJSON(MasterGeral()) == true) {
            var gridEscola = dijit.byId('gridEscolas');
            var hasClickEscola = eval(dojo.byId('abriuEscola').value);

            if ((gridEscola.store == null || gridEscola.store.objectStore == null || gridEscola.store.objectStore.data.length <= 0) && (hasClickEscola)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgEscolaDeverSerInserida);
                apresentaMensagem("apresentadorMensagemItemServico", mensagensWeb);
                return false;
            }
        }
        // $('#aguardar').append('<img style="position: relative;left: 158px; top: 79px" src="/images/carregando.gif"/>').css('display', "");

        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, windows) {

            if (!validarCamposItemServico(windows))
                return false;

            showCarregando();

            xhr.post(Endereco() + "/api/escola/postEditItemServico", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(item)
            }).then(function (data) {
                try {
                    $('#aguardar').css('display', "none");
					data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridItemServico';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadItemServico").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_item", dom.byId("cd_item").value);
                        insertObjSort(grid.itensSelecionados, "cd_item", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoItemServico', 'cd_item', 'selecionaTodosItemServico', ['pesquisarItemServico', 'relatorioItemServico'], 'todosItensItemServico');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_item");
                        showCarregando();
                    } else {
                        apresentaMensagem('apresentadorMensagemItemServico', data);
                        showCarregando();
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemItemServico', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCamposItemServico(windowUtils) {
    try {
        var validado = true;
        var tabs = dijit.byId("tabContainerItemServico");
        var pane = dijit.byId("tabPrincipal");

        if (!dijit.byId("formItemServico").validate()) {
            validado = false;
            tabs.selectChild(pane);
        }

        if (dijit.byId("tipoItens").value == BIBLIOTECA)
            if (!dijit.byId('formItemBiblioteca').validate()) {
                tabs.selectChild(pane);
                dijit.byId("PanelBiblioteca").set("open", true);
                validado = false;
            }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region eventos de link
function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            limparItem();
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, null, dijit.byId('gridItemServico'), true);
            IncluirAlterar(0, 'divAlterarCurso', 'divIncluirCurso', 'divExcluirCurso', 'apresentadorMensagemCurso', 'divCancelarCurso', 'divLimparCurso');
            dijit.byId('tabContainer').resize();
            dijit.byId("cad").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarItemServico(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            dojo.byId('abriuEscola').value = false;
            itensSelecionados[0].cd_grupo_estoque = itensSelecionados[0].cd_grupo_estoque == null ? 0 : itensSelecionados[0].cd_grupo_estoque;
            limparFormItem();
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/returnDataItem?cdGrupo=" + itensSelecionados[0].cd_grupo_estoque + "&cdTipoItem=" + itensSelecionados[0].cd_tipo_item + "&tipoMovimento=",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataItem) {
                if (jQuery.parseJSON(MasterGeral()) == false)
                    dijit.byId("tabContainerItemServico_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                dataItem = jQuery.parseJSON(dataItem);
                loadGrupoEstoque(dataItem.retorno.grupos, "grupoItem");
                loadTipoItem(dataItem.retorno.tipos, "tipoItens");
                apresentaMensagem('apresentadorMensagem', '');
                var tabContainerItemServico = dijit.byId("tabContainerItemServico");
                keepValues(null, null, dijit.byId('gridItemServico'), true);
                setarTabCad();
                dijit.byId("cadItemServico").show();
                dijit.byId('tabContainerItemServico').resize();
                IncluirAlterar(0, 'divAlterarItemServico', 'divIncluirItemServico', 'divExcluirItemServico', 'apresentadorMensagemItemServico', 'divCancelarItemServico', 'divLimparItemServico');
                limparGrid();
                hideCarregando();
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  eventos para a aba de escola
function pesquisarEscolasFK() {
    var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;

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
                            target: Endereco() + "/api/escola/getEscolaNotWithItem?nome=" + dojo.byId("_nomePessoaFK").value +
                                        "&fantasia=" + dojo.byId("_apelido").value +
                                        "&cnpj=" + dojo.byId("CnpjCpf").value +
                                        "&cd_item=" + item +
                                            "&inicio=" + document.getElementById("inicioPessoaFK").checked,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoa");
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false)
                }
                catch (e) {
                    postGerarLog(e);
                }
            })
        });
}

function abrirPessoaFK(isPesquisa) {
    dojo.ready(function () {
        try {
            dijit.byId("fkPessoaPesq").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaPesq").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
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

function limparGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        if (hasValue(gridEscolas)) {
            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridEscolas.update();
        }
        var gridPlano = dijit.byId('gridPlano');
        if (hasValue(gridPlano)) {
            gridPlano.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridPlano.update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadItemServico").open) {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_pessoa');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_pessoa", {
                        cd_pessoa: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion eventos para a aba de escola
function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try {
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKSubgrupo(xhr, ObjectStore, Memory) {
    try {
        dijit.byId("cadSubGrupo").show();
        apresentaMensagem('apresentadorMensagemSubgrupoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirSubGrupo() {
    try {
        if (!dijit.byId("dialogPlano").validate())
            return false;
        document.getElementById('divExcluirPlano').style.visibility = "hidden";
        var subGrupoTp = parseInt(dojo.byId("cd_subgrupo_tp").value);
        subGrupoTp = ++subGrupoTp;
        dojo.byId("cd_subgrupo_tp").value = subGrupoTp;
        var gridPlano = dijit.byId("gridPlano");

        var storeGridPlanos = (hasValue(gridPlano) && hasValue(gridPlano.store.objectStore.data)) ? gridPlano.store.objectStore.data : [];
        insertObjSort(gridPlano.store.objectStore.data, "id_subGrupoTp", {
            cd_subgrupo_tp: dojo.byId('cd_subgrupo2').value,
            cd_item_subgrupo: subGrupoTp,
            cd_subgrupo_conta: dojo.byId('cd_subgrupo2').value,
            id_subGrupoTp: subGrupoTp,
            id_tipo_movimento: dijit.byId('tipoMov').value,
            no_tipo: document.getElementById('tipoMov').value,
            no_subgrupo: document.getElementById('cbSubGrupo2').value
        });
        gridPlano.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridPlano.store.objectStore.data }) }));
        if (!dijit.byId("ckManterCad").checked)
            dijit.byId("dialogPlano").hide();
        else
            limparSubGrupoTipo();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarSubgrupo(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagemPlano', '');
            keepValuesSubGrupoTp(null, dijit.byId('gridPlano'), true);
            IncluirAlterar(0, 'divAlterarPlano', 'divIncluirPlano', 'divExcluirPlano', 'apresentadorMensagemPlano', 'divCancelarPlano', 'divLimparPlano');
            dijit.byId("dialogPlano").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarSubgrupo() {
    try {
        if (!dijit.byId("dialogPlano").validate())
            return false;
        var grid = dijit.byId("gridPlano");
        for (var i = 0; i < grid._by_idx.length; i++)
            if (grid._by_idx[i].item.cd_item_subgrupo == dojo.byId("cd_subgrupo_tp").value) {

                grid._by_idx[i].item.cd_subgrupo_conta = dojo.byId("cd_subgrupo2").value;
                grid._by_idx[i].item.id_tipo_movimento = dijit.byId('tipoMov').value;
                grid._by_idx[i].item.no_tipo = document.getElementById('tipoMov').value;
                grid._by_idx[i].item.no_subgrupo = document.getElementById('cbSubGrupo2').value;

                break;
            }
        grid.update();
        dijit.byId("dialogPlano").hide();
        grid.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}
