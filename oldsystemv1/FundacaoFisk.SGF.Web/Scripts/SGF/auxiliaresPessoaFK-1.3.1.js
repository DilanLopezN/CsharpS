//#region - formatCheckBoxAtividade - loadNaturezaPessoaCadastroFK(Memory)
var FISICA_FK = 1, JURIDICA_FK = 2;

function formatCheckBoxAtividade(value, rowIndex, obj) {
    try{
        var gridName = 'gridAuxiliaresPessoaFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAtividade');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_atividade", grid._by_idx[rowIndex].item.cd_atividade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_atividade', 'selecionadoAtividade', -1, 'selecionaTodosAtividade', 'selecionaTodosAtividade', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_atividade', 'selecionadoAtividade', " + rowIndex + ", '" + id + "', 'selecionaTodosAtividade', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadNaturezaPessoaCadastroFK(Memory) {
    try{
        var stateStore = new Memory({
            data: [
            { name: "Física", id: "1" },
            { name: "Jurídica", id: "2" },
            { name: "Cargo", id: "3" }
            ]
        });
        dijit.byId("naturezaAtividadeFK").store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region montargridPesquisaAuxiliaresPessoa(funcao)  -  limparGridAuxiliaresPessoa() - pesquisarAuxiliaresPessoaFK() - clearFormAtividadeFk(natureza) 
function montargridPesquisaAuxiliaresPessoa(funcao) {
    require([
    "dojo/ready",
    "dojo/_base/array",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button"
    ], function (ready, array, EnhancedGrid, Pagination,  Memory, on, Button) {
        ready(function () {
            try{
                var myStore = null;
                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridAuxiliaresPessoaFK = new EnhancedGrid({
                    structure: [
                        { name: "<input id='selecionaTodosAtividade' style='display:none'/>", field: "selecionadoAtividade", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividade },
                        //{ name: "Código", field: "cd_atividade", width: "10%" },
                        { name: "Atividade/Profissão", field: "no_atividade", width: "55%" },
                        { name: "Natureza", field: "natureza_atividade", width: "15%" },
                        { name: "CNAE", field: "cd_cnae_atividade", width: "10%" },
                        { name: "Ativa", field: "atividade_ativa", width: "10%" }
                    ],
                    canSort: false,
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["14", "28", "56", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "14",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridAuxiliaresPessoaFK");
                gridAuxiliaresPessoaFK.canSort = function () { return true };
                gridAuxiliaresPessoaFK.startup();
                gridAuxiliaresPessoaFK.canSort = function (col) { return Math.abs(col) != 1; };
                gridAuxiliaresPessoaFK.itensSelecionados = new Array();

                dojo.query("#desAtividadeFK").on("keyup", function (e) {
                    if (e.keyCode == 13) {
                        pesquisarAuxiliaresPessoaFK()
                    }
                });

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        pesquisarAuxiliaresPessoaFK()
                    }
                }, "pesquisarAtividadeFK");

                decreaseBtn(document.getElementById("pesquisarAtividadeFK"), '32px');

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("proAuxiliaresPessoaFK").hide();
                    }
                }, "fecharAtividadeFK");
           
                montarStatus("statusAtividadeFK");
                loadNaturezaPessoaCadastroFK(Memory);            
            
                dijit.byId('naturezaAtividadeFK').on("change", function (e) {
                    if (e == FISICA_FK) {
                         dijit.byId('cnaeFK').set('disabled', true);
                        limparGridAuxiliaresPessoa();
                    }
                    else {
                        dijit.byId('cnaeFK').set('disabled', false);
                        limparGridAuxiliaresPessoa();
                    }
                });
                adicionarAtalhoPesquisa(['desAtividadeFK', 'statusAtividadeFK', 'naturezaAtividadeFK', 'cnaeFK'], 'pesquisarAtividadeFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function pesquisarAuxiliaresPessoaFK() {
    var descriacao = dijit.byId('desAtividadeFK').get('value');
    var inicio = dijit.byId('ckInicioAtividadeFK').checked;
    var status = dijit.byId('statusAtividadeFK').get('value');
    var naturaza = dijit.byId('naturezaAtividadeFK').get('value');
    var cnae = dijit.byId('cnaeFK').get('value');

    if (true)
        require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
        ], function (JsonRest,ObjectStore, Cache, Memory) {
            try{
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getAtividadesearch?descricao=" + descriacao + "&inicio=" + inicio + "&status="+ status +  "&natureza=" +  naturaza+ "&cnae=" + cnae,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_aluno"
                }), Memory({ idProperty: "cd_aluno" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridAuxiliaresPessoaFK = dijit.byId("gridAuxiliaresPessoaFK");
                gridAuxiliaresPessoaFK.itensSelecionados = [];
                gridAuxiliaresPessoaFK.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function clearFormAtividadeFk(natureza) {
    try{
        limparGridAuxiliaresPessoa();
        dijit.byId('desAtividadeFK').set('value', "");
        dijit.byId('ckInicioAtividadeFK').checked = false;
        dijit.byId('statusAtividadeFK').set('value', 0);
        dijit.byId('naturezaAtividadeFK')._onChangeActive = false;
        dijit.byId('naturezaAtividadeFK').set('value', natureza);
        dijit.byId('naturezaAtividadeFK').set('disabled', true);
        dijit.byId('naturezaAtividadeFK')._onChangeActive = true;
        dijit.byId('cnaeFK').set('value', "");
        pesquisarAuxiliaresPessoaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGridAuxiliaresPessoa() {
    try{
        dijit.byId("gridAuxiliaresPessoaFK").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        dijit.byId("gridAuxiliaresPessoaFK").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion