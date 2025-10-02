var CADMAT = 1;
var CAD_ESCOLA_USUARIO = 2;
var ENVIARTRANSFERENCIACAD = 31;

function mostrarMsg(apresentadorMensagem, msgDescObrigtoria) {
    var mensagensWeb = new Array();
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgDescObrigtoria);
    apresentaMensagem(apresentadorMensagem, mensagensWeb);
}

function montargridPesquisaPessoa(funcao) {
    require([
        "dojo/ready",
	    "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dojox/json/ref"
    ], function (ready, EnhancedGrid, Pagination, ObjectStore, Cache, Memory, on, Button, ref) {
        ready(function () {
            try {
                var myStore = null;
                var gridPesquisaPessoa = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: myStore }) }),
                    structure: [
                         { name: "<input id='selecionaTodosPessoaFK' style='display:none'/>", field: "selecionadoPessoaFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPessasFK },
                        { name: "Nome", field: "no_pessoa", width: "30%" },
                        { name: "CPF\\CNPJ", field: "nm_cpf_cgc_dependente", width: "30%" },
                        { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "100px" },
                        { name: "Data Cadastro", field: "dta_cadastro", width: "70px" },
                        { name: "Natureza", field: "natureza_pessoa", width: "55px", styles: "text-align: center;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEncFiltro,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["12", "24", "48", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "12",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridPesquisaPessoa");
                gridPesquisaPessoa.canSort = function (col) { return Math.abs(col) != 1 };
                //gridPesquisaPessoa.on("RowDblClick", function () {
                //    retornarPessoa();
                //}, true);
                gridPesquisaPessoa.startup();
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            var tipoOrigem = dojo.byId("idOrigemPessoaFK").value;
                            if (hasValue(tipoOrigem))
                                switch (parseInt(tipoOrigem)) {
                                    case CADMAT:
                                        retornarPessoaMat();
                                        break;
                                    case CAD_ESCOLA_USUARIO:
                                        retornarPessoaEscola();
                                        break;
                                    case ENVIARTRANSFERENCIACAD:
                                        retornarPessoaEscolaCad();
                                        break;
                                    case USARCPF:
                                        retornarPessoaCPF();
                                        break;
                                    case CPFRELAC:
                                        retornarPessoaCPFRELAC();
                                        break;
                                    
                                }
                            else
                                retornarPessoa();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecPessoa");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("proPessoa")))
                                dijit.byId("proPessoa").hide();
                            if (hasValue(dijit.byId("fkPessoaPesq")))
                                dijit.byId("fkPessoaPesq").hide();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fecharFK");
                loadTipoPessoaFK();
                if (dijit.byId("sexoPessoaFK") != null && dijit.byId("sexoPessoaFK") != undefined) {
                    loadPesqSexo(Memory, dijit.byId("sexoPessoaFK"));
                }
                
                decreaseBtn(document.getElementById("pesqPessoa"), '32px');
                dijit.byId("tipoPessoaFK").on("change", function (e) {
                    try {
                        dijit.byId("sexoPessoaFK").set("value", 0);
                        if (e == 1)
                            toggleDisabled(dijit.byId("sexoPessoaFK"), false);
                        else
                            if (e == 2)
                                toggleDisabled(dijit.byId("sexoPessoaFK"), true);
                            else
                                toggleDisabled(dijit.byId("sexoPessoaFK"), false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['_nomePessoaFK', '_apelido', 'tipoPessoaFK', 'CnpjCpf', 'sexoPessoaFK'], 'pesqPessoa', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(' URL: = ' + window.location.href + ' ' + e);
            }
        });
    });
}

function pesquisaPessoaFK(usarPesquisaPadrao, tipo_pesquisa) {
    try {
        tipo_pesquisa = tipo_pesquisa = null ? 0 : tipo_pesquisa;
        if (usarPesquisaPadrao)
            require([
                  "dojo/ready"
            ], function (ready) {
                ready(function () {
                    try {
                        find(dojo.byId("_nomePessoaFK").value, dojo.byId("_apelido").value, dijit.byId("tipoPessoaFK").value, dojo.byId("CnpjCpf").value, tipo_pesquisa);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function find(nome, apelido, tipoPessoa, cnpjCpf, tipo_pesquisa) {
    try{
        if (nome != "" || apelido != "" || tipoPessoa != "" || cnpjCpf != "") {
            require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"],
            function (JsonRest, ObjectStore, Cache, Memory) {
                try{
                    var myStore = Cache(
                         JsonRest({
                             target: Endereco() + "/api/aluno/getPessoaSearchEscolaWithCPFCNPJ?nome=" + nome + "&apelido=" + apelido + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                 "&tipoPessoa=" + parseInt(tipoPessoa) + "&cnpjCpf=" + cnpjCpf + "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=0" + '&tipo_pesquisa=' + tipo_pesquisa,
                             handleAs: "json",
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoa");
                    grid.noDataMessage = msgNotRegEnc;
                    grid.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        }
        else {
            mensagemPesquisa();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTipoPessoaFK() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try{
             var statusStore = new Memory({
                 data: [
                 { name: "Todos", id: "0" },
                 { name: "Física", id: "1" },
                 { name: "Jurídica", id: "2" }
                 ]
             });

             var tipoPessoaFK = new filteringSelect({
                 id: "tipoPessoaFK",
                 name: "tipoPessoaFK",
                 store: statusStore,
                 value: "0",
                 searchAttr: "name",
                 style: "width:100%;"
             }, "tipoPessoaFK");

             tipoPessoaFK.on("change", function () {
                 try{
                     if (this.get("value") == 0) {
                         dojo.byId("CnpjCpf").value = "";
                         $('#CnpjCpf').unmask();
                        dojo.byId("lblCPF").innerHTML = "CPF \ CNPJ:";
                     }
                     dojo.byId("CnpjCpf").readOnly = true;
                     if (this.get("value") == 1) {
                         dojo.byId("CnpjCpf").value = "";
                         dojo.byId("CnpjCpf").readOnly = false;
                        dojo.byId("lblCPF").innerHTML = "CPF:";
                         $("#CnpjCpf").mask("999.999.999-99");
                     }
                     if (this.get("value") == 2) {
                         dojo.byId("CnpjCpf").value = "";
                         dojo.byId("CnpjCpf").readOnly = false;
                        dojo.byId("lblCPF").innerHTML = "CNPJ:";
                         $("#CnpjCpf").mask("99.999.999/9999-99");
                     }
                 }
                 catch (e) {
                     postGerarLog(e);
                 }
             });
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}

function limparPesquisaPessoaFK() {
    try{
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        dijit.byId('tipoPessoaFK').set('value', 0);
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
        dijit.byId('sexoPessoaFK').set('value', 0);
        dijit.byId('inicioPessoaFK').set('checked', false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPessasFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaPessoa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPessoaFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoPessoaFK', -1, 'selecionaTodosPessoaFK', 'selecionaTodosPessoaFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoPessoaFK', " + rowIndex + ", '" + id + "', 'selecionaTodosPessoaFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
