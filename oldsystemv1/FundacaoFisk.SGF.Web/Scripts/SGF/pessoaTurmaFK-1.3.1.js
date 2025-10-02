var CADMAT = 1;
var CAD_ESCOLA_USUARIO = 2;

function mostrarMsg(apresentadorMensagem, msgDescObrigtoria) {
    var mensagensWeb = new Array();
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgDescObrigtoria);
    apresentaMensagem(apresentadorMensagem, mensagensWeb);
}

function montargridPesquisaPessoaTurma(funcao) {
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
                var gridPesquisaPessoaTurma = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: myStore }) }),
                    structure: [
                         { name: "<input id='selecionaTodosPessoaFK' style='display:none'/>", field: "selecionadoPessoaFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPessasTurmaFK },
                        { name: "Nome", field: "no_pessoa", width: "30%" },
                        { name: "CPF\\CNPJ", field: "nm_cpf_cgc_dependente", width: "30%" },
                        { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "100px" },
                        { name: "Data Cadastro", field: "dta_cadastro", width: "70px" },
                        { name: "Natureza", field: "natureza_pessoa", width: "55px", styles: "text-align: center;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                }, "gridPesquisaPessoaTurma");
                gridPesquisaPessoaTurma.canSort = function (col) { return Math.abs(col) != 1 };
                //gridPesquisaPessoaTurma.on("RowDblClick", function () {
                //    retornarPessoa();
                //}, true);
                gridPesquisaPessoaTurma.startup();
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            var tipoOrigem = (dojo.byId("idOrigemPessoaFK") != null && dojo.byId("idOrigemPessoaFK") != undefined) ? dojo.byId("idOrigemPessoaFK").value : null;
                            if (hasValue(tipoOrigem))
                                switch (parseInt(tipoOrigem)) {
                                    case CADMAT:
                                        retornarPessoaMat();
                                        break;
                                    case CAD_ESCOLA_USUARIO:
                                        retornarPessoaEscola();
                                        break;
                                }
                            else
                                retornarPessoaTurma();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecPessoaTurma");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("fkPessoaTurmaPesq")))
                                dijit.byId("fkPessoaTurmaPesq").hide();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fecharPessoaTurmaFK");

                loadTipoPessoaTurmaFK();

                if (hasValue(dijit.byId("sexoPessoaTurmaFK"))) {
                    loadPesqSexo(Memory, dijit.byId("sexoPessoaTurmaFK"));
                }
                decreaseBtn(document.getElementById("pesqPessoaTurma"), '32px');
                dijit.byId("tipoPessoaTurmaFK").on("change", function (e) {
                    try {
                        dijit.byId("sexoPessoaTurmaFK").set("value", 0);
                        if (e == 1)
                            toggleDisabled(dijit.byId("sexoPessoaTurmaFK"), false);
                        else
                            if (e == 2)
                                toggleDisabled(dijit.byId("sexoPessoaTurmaFK"), true);
                            else
                                toggleDisabled(dijit.byId("sexoPessoaTurmaFK"), false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['_nomePessoaTurmaFK', '_apelidoTurma', 'tipoPessoaTurmaFK', 'CnpjCpfTurma', 'sexoPessoaTurmaFK'], 'pesqPessoaTurma', ready);
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
                        find(dojo.byId("_nomePessoaTurmaFK").value, dojo.byId("_apelidoTurma").value, dijit.byId("tipoPessoaTurmaFK").value, dojo.byId("CnpjCpfTurma").value, tipo_pesquisa);
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
    try {
        if (nome != "" || apelido != "" || tipoPessoa != "" || cnpjCpf != "") {
            require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"],
            function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var myStore = Cache(
                         JsonRest({
                             target: Endereco() + "/api/aluno/getPessoaSearchEscolaWithCPFCNPJ?nome=" + nome + "&apelido=" + apelido + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                 "&tipoPessoa=" + parseInt(tipoPessoa) + "&cnpjCpf=" + cnpjCpf + "&sexo=" + dijit.byId("sexoPessoaTurmaFK").value + "&papel=0"  + '&tipo_pesquisa=' + tipo_pesquisa,
                             handleAs: "json",
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoaTurma");
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

function loadTipoPessoaTurmaFK() {
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
             var tipoPessoaTurmaFK = null;
             if (dijit.byId("tipoPessoaTurmaFK") == undefined) {
                 tipoPessoaTurmaFK = new filteringSelect({
                     id: "tipoPessoaTurmaFK",
                     name: "tipoPessoaTurmaFK",
                     store: statusStore,
                     value: "0",
                     searchAttr: "name",
                     style: "width:100%;"
                 },
                    "tipoPessoaTurmaFK");
             } else {
                 tipoPessoaTurmaFK = dijit.byId("tipoPessoaTurmaFK");
             }

             tipoPessoaTurmaFK.on("change", function () {
                 try {
                     if (this.get("value") == 0) {
                         dojo.byId("CnpjCpfTurma").value = "";
                         $('#CnpjCpfTurma').unmask();
                     }
                     dojo.byId("CnpjCpfTurma").readOnly = true;
                     if (this.get("value") == 1) {
                         dojo.byId("CnpjCpfTurma").value = "";
                         dojo.byId("CnpjCpfTurma").readOnly = false;
                         $("#CnpjCpfTurma").mask("999.999.999-99");
                     }
                     if (this.get("value") == 2) {
                         dojo.byId("CnpjCpfTurma").value = "";
                         dojo.byId("CnpjCpfTurma").readOnly = false;
                         $("#CnpjCpfTurma").mask("99.999.999/9999-99");
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
    try {
        dojo.byId("_nomePessoaTurmaFK").value = "";
        dojo.byId("_apelidoTurma").value = "";
        dojo.byId("CnpjCpfTurma").value = "";
        dijit.byId('tipoPessoaTurmaFK').set('value', 0);
        if (hasValue(dijit.byId("gridPesquisaPessoaTurma"))) {
            dijit.byId("gridPesquisaPessoaTurma").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoaTurma").itensSelecionados))
                dijit.byId("gridPesquisaPessoaTurma").itensSelecionados = [];
        }
        dijit.byId('sexoPessoaTurmaFK').set('value', 0);
        dijit.byId('inicioPessoaFK').set('checked', false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPessasTurmaFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaPessoaTurma';
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
