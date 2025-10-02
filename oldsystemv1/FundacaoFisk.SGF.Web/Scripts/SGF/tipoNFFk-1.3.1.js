var DEVOLUCAO = 5, SERVICO_TIPO = 9;
function formatCheckBoxTipoNFFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoNFFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTipoNFFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_nota_fiscal", grid._by_idx[rowIndex].item.cd_tipo_nota_fiscal);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_nota_fiscal', 'selecionadoTipoNFFK', -1, 'selecionaTodosTipoNFFK', 'selecionaTodosTipoNFFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_nota_fiscal', 'selecionadoTipoNFFK', " + rowIndex + ", '" + id + "', 'selecionaTodosTipoNFFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarTipoNFFK(funcao) {
    require([
        "dojo/ready",
        "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
	    "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button"
    ], function (ready, EnhancedGrid, Pagination, ObjectStore, Memory, Button) {
        ready(function () {
            try {
                montarStatus("statusTpNFFK");
                
                var myStore = null;
                var TipoNFFK = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: myStore }) }),
                    structure: [
                        { name: "<input id='selecionaTodosTipoNFFK' style='display:none'/>", field: "selecionadoTipoNFFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoNFFK },
			            { name: "Descrição", field: "dc_tipo_nota_fiscal", width: "20%", styles: "min-width:80px;" },
						{ name: "CFOP", field: "dc_CFOP", width: "7%", styles: "text-align:center; min-width:40px;" },
						{ name: "Natureza", field: "movimento", width: "10%", styles: "text-align:center; min-width:40px;" },
						{ name: "Sit. Trib.", field: "nmSitTrib", width: "7%", styles: "text-align:center; min-width:40px;" },
						{ name: "Movimenta Estoque", field: "mtvoEstoque", width: "10%", styles: "text-align:center; min-width:40px;" },
						{ name: "Movimenta Financeiro", field: "mtvoFinanc", width: "10%", styles: "text-align:center; min-width:40px;" },
						{ name: "Devolução", field: "devolucao", width: "10%", styles: "text-align:center; min-width:40px;" },
                        { name: "Ativo", field: "ativo", width: "7%", styles: "text-align:center; min-width:40px;" }
                    ],
                    canSort: false,
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "24", "48", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "15",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTipoNFFK");

                TipoNFFK.startup();

                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () {

                    }
                }, "selecionaTipoNFFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("fkTipoNF").hide();
                    }
                }, "fecharTipoNFFK");

                decreaseBtn(document.getElementById("pesquisarTipoNFFK"), '32px');
                adicionarAtalhoPesquisa(['descTpNFFK', 'statusTpNFFK'], 'pesquisarTipoNFFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function pesquisaTipoNFFK(usarPesquisaPadrao, id_servico) {
    try{
        if (usarPesquisaPadrao)
            require([
                  "dojo/ready"
            ], function (ready) {
                ready(function () {
                    findTpNF(dijit.byId('descTpNFFK').get('value'), dijit.byId('inicioTpNFFK').checked, id_servico);
                });
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findTpNF(descriaco, marcado, id_servico) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var dev = null;
            if (hasValue(TIPOMOVIMENTO) && TIPOMOVIMENTO > 0)
                if (TIPOMOVIMENTO == DEVOLUCAO)
                    dev = true;
                else
                    dev = false;
            var movimento = hasValue(dijit.byId("cbMovtoFK").value) ? dijit.byId("cbMovtoFK").value : 0;
            var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getTipoNotaFiscalSearch?desc=" + descriaco + "&natOp=&inicio=" + marcado + "&status=" + retornaStatus("statusTpNFFK") + "&movimento=" + movimento + "&devolucao=" + dev + "&escola=true&id_regime_trib=0&id_servico=" + ((id_servico == false && TIPOMOVIMENTO === SAIDA) ? false : (id_servico == false && TIPOMOVIMENTO !== SAIDA)? null : id_servico),
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_tipo_nota_fiscal"
               }
            ), Memory({ idProperty: "cd_tipo_nota_fiscal" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridTipoNFFK");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaTipoNFFK() {
    try{
        dijit.byId('descTpNFFK').set('value', '');
        dijit.byId('inicioTpNFFK').set('checked', false);
        dijit.byId("statusTpNFFK").set('value', 1);
        if (hasValue(dijit.byId("gridTipoNFFK"))) {
            dijit.byId("gridTipoNFFK").currentPage(1);
            if (hasValue(dijit.byId("gridTipoNFFK").itensSelecionados))
                dijit.byId("gridTipoNFFK").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTpMovtoFK(padrao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var stMovto = dijit.byId("cbMovtoFK");

	        var statusStore = new Memory({
	            data: [
                { name: "Entrada", id: 1 },
                { name: "Saída", id: 2 }
	            ]
	        });
	        
	        if (hasValue(padrao) && padrao > 0) {
	            if (padrao == DEVOLUCAO) {
	                var statusStoreDevolucao = new Memory({
	                    data: [
                        { name: "Entrada", id: 1 },
                        { name: "Saída", id: 2 }
	                    ]
	                });
	                stMovto.store = statusStoreDevolucao;
	            }
	            else {
	                stMovto.store = statusStore;
	                stMovto.set("value", padrao);
	            }
	                //if (padrao != SERVICO_TIPO)
	                //    stMovto.set("disabled", false);
	                //else
	                //    stMovto.set("disabled", true);
	        }
	        else
	            stMovto.store = statusStore;
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}