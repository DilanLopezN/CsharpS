function mostrarMsg(apresentadorMensagem, msgDescObrigtoria) {
    try{
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgDescObrigtoria);
        apresentaMensagem(apresentadorMensagem, mensagensWeb);
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPessasRelFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaPessoaRel';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPessasRelFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoPessasRelFK', -1, 'selecionaTodosPessasRelFK', 'selecionaTodosPessasRelFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoPessasRelFK', " + rowIndex + ", '" + id + "', 'selecionaTodosPessasRelFK', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montargridPesquisaPessoaRel(usarPesquisaPadrao) {
    require([
        "dojo/ready",
	    "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/DateTextBox"
    ], function (ready, EnhancedGrid, Pagination, ObjectStore, Memory, on, Button) {
        ready(function () {
            var myStore = null;
            var gridPesquisaPessoaRel = new EnhancedGrid({
                store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data : myStore}) }),
                structure: [
                    { name: "<input id='selecionaTodosPessasRelFK' style='display:none'/>", field: "selecionadoPessasRelFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPessasRelFK },
			        { name: "Nome", field: "no_pessoa", width: "30%" },
                    { name: "CPF\\CNPJ", field: "nm_cpf_cgc_dependente", width: "28%" },
                    { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "18%" },
                    { name: "Data Cadastro", field: "dta_cadastro", width: "14%" },
                    { name: "Natureza", field: "natureza_pessoa", width: "10%", styles: "text-align: center;" }
                ],
                canSort: false,
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
                        maxPageStep: 4,
                        /*position of the pagination bar*/
                        position: "button"
                    }
                }
            }, "gridPesquisaPessoaRel");
            gridPesquisaPessoaRel.on("RowDblClick", function () {
                retornarPessoaRel();
            }, true);
            gridPesquisaPessoaRel.startup();
            new Button({
                label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                }
            }, "pesqPessoaRel");
            decreaseBtn(document.getElementById("pesqPessoaRel"), '32px');
            new Button({ label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () { retornarPessoaRel(); } }, "selecPessoaRel");
            new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("proPessoaRel").hide(); } }, "fecharRelFK");
            loadTipoPessoaRelFK();
            loadPapeisFK();
            loadPesqSexo(Memory, dijit.byId("sexoPessoaRelFK"));
            adicionarAtalhoPesquisa(['_nomePessoaRelFK', '_apelidoRel', 'tipoPessoaRelFK', 'papelPessoaRelFK', 'sexoPessoaRelFK', 'CnpjCpfPessoaRel'], 'pesqPessoaRel', ready);
            dijit.byId("tipoPessoaRelFK").on("change", function (e) {
                try{
                    dijit.byId("sexoPessoaRelFK").set("value", 0);
                    if (e == 1) {
                        toggleDisabled(dijit.byId("sexoPessoaRelFK"), false);
                        papeis(true);
                    }
                    else
                        if (e == 2) {
                            toggleDisabled(dijit.byId("sexoPessoaRelFK"), true);
                            papeis(false);
                        }
                        else {
                            toggleDisabled(dijit.byId("sexoPessoaRelFK"), false);
                            papeis(null);
                        }
                } catch (e) {
                    postGerarLog(e);
                }
            })
        });
    });
}

function loadTipoPessoaRelFK() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var statusStore = new Memory({
                 data: [
                 { name: "Todos", id: "0" },
                 { name: "Física", id: "1" },
                 { name: "Jurídica", id: "2" }
                 ]
             });

             var tipoPessoaFK = new filteringSelect({
                 id: "tipoPessoaRelFK",
                 name: "tipoPessoaRelFK",
                 store: statusStore,
                 value: "0",
                 searchAttr: "name",
                 style: "width:47%;"
             }, "tipoPessoaRelFK");

             tipoPessoaFK.on("change", function () {
                 if (this.get("value") == 0) {
                     dojo.byId("CnpjCpfPessoaRel").value = "";
                     $('#CnpjCpfPessoaRel').unmask();
                 }
                 dojo.byId("CnpjCpfPessoaRel").readOnly = true;
                 if (this.get("value") == 1) {
                     dojo.byId("CnpjCpfPessoaRel").value = "";
                     dojo.byId("CnpjCpfPessoaRel").readOnly = false;
                     $("#CnpjCpfPessoaRel").mask("999.999.999-99");
                 }
                 if (this.get("value") == 2) {
                     dojo.byId("CnpjCpfPessoaRel").value = "";
                     dojo.byId("CnpjCpfPessoaRel").readOnly = false;
                     $("#CnpjCpfPessoaRel").mask("99.999.999/9999-99")
                 }
             });
         } catch (e) {
                 postGerarLog(e);
             }
     })
}

function limparPesquisaPessoaRelFK() {
    try{
        dojo.byId("_nomePessoaRelFK").value = "";
        dojo.byId("_apelidoRel").value = "";
        dojo.byId("CnpjCpfPessoaRel").value = "";
        dijit.byId("papelPessoaRelFK").set("value", 0);
        dijit.byId("sexoPessoaRelFK").set("value", 0);
        dijit.byId("tipoPessoaRelFK").set("value", 0);
        dijit.byId("inicioPessoaRelFK").set("checked", false);
        if (hasValue(dijit.byId("gridPesquisaPessoaRel")) && hasValue(dijit.byId("gridPesquisaPessoaRel").itensSelecionados))
            dijit.byId("gridPesquisaPessoaRel").itensSelecionados = [];
    } catch (e) {
        postGerarLog(e);
    }
}

function loadPapeisFK() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try{
             var statusStore = new Memory({
                 data: [
                 { id: 0, name: "Todos" },
                          { id: 1, name: "Pais" },
                          { id: 3, name: "Responsável" },
                          { id: 7, name: "Empresa" }
                 ]
             });

             var papelRelFK = new filteringSelect({
                 id: "papelPessoaRelFK",
                 name: "papelPessoaRelFK",
                 store: statusStore,
                 value: "0",
                 searchAttr: "name",
                 style: "width:100%;"
             }, "papelPessoaRelFK");
         } catch (e) {
             postGerarLog(e);
         }
    });
}

function papeis(fisica) {
    try {
        if (fisica == true) {
            var statusStore = [
                { id: 0, name: "Todos" },
                { id: 1, name: "Pais" },
                { id: 3, name: "Responsável" }
            ];
            dijit.byId("papelPessoaRelFK").store.data = statusStore;
            dijit.byId("papelPessoaRelFK").set("value", 0);
        }
        else
            if (fisica == false) {
                var statusStore = [
                    { id: 0, name: "Todos" },
                    { id: 7, name: "Empresa" }
                ];
                dijit.byId("papelPessoaRelFK").store.data = statusStore;
                dijit.byId("papelPessoaRelFK").set("value", 0);
            }

            else {
                var statusStore = [
                    { id: 0, name: "Todos" },
                    { id: 1, name: "Pais" },
                    { id: 3, name: "Responsável" },
                    { id: 7, name: "Empresa" }
                ];
                dijit.byId("papelPessoaRelFK").store.data = statusStore;
                dijit.byId("papelPessoaRelFK").set("value", 0);
            }
    } catch (e) {
        postGerarLog(e);
    }
}