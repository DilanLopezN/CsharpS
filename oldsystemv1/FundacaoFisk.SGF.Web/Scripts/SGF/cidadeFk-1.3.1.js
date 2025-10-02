var PESSOARELACIONADA = 1, LOCALIDADES = 2, LOGRADOUROFK = 3, DADOSNFPES = 4, DADOSNFCAD = 5, TIPO_PESQUISA_FOLLOW_ESCOLA_FK = 6;

function montargridPesquisaCidade(funcao) {
    require([
        "dojo/ready",
        "dojo/_base/array",
	    "dojo/_base/xhr",
	    "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
	    "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/Dialog"
    ], function (ready, array, xhr, EnhancedGrid, Pagination, ObjectStore, Memory, on, Button) {
        ready(function () {
            try{
                var myStore = null;
                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridPesquisaCidade = new EnhancedGrid({
                    structure: [
                        { name: "<input id='selecionaTodasCidadesFK' style='display:none'/>", field: "cidadeFKSelecionada", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCidadeFK },
			            { name: "Cidade", field: "no_cidade", width: "20%" },
                        { name: "Estado", field: "estado", width: "10%" },
                        { name: "Cód. IBGE ", field: "nm_cidade", width: "20%", styles: "text-align: center;" },
                        { name: "País", field: "pais", width: "20%" }
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
                }, "gridPesquisaCidade");
                gridPesquisaCidade.canSort = function () { return true };
                gridPesquisaCidade.startup();
                gridPesquisaCidade.canSort = function (col) { return Math.abs(col) != 1; };
                gridPesquisaCidade.itensSelecionados = new Array();
                //gridPesquisaCidade.on("RowDblClick", function () {
                //    retornarCidade();
                //}, true);
                //xhr.get({
                //    url: Endereco() + "/api/localidade/getAllEstado/", //estado
                //    preventCache: true,
                //    handleAs: "json",
                //    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                //}).then(function (dataEstado) {
                //    loadEstadoFK(dataEstado);
                //});
                xhr.get({
                    url: Endereco() + "/api/localidade/GetAllPais/", //pais
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataPais) {
                    loadPaisCidadeFK(dataPais);
                    if(hasValue(funcao))
                        funcao.call();
                });
                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        //pesquisaCidadeFK();
                    }
                }, "pesquisar");
                decreaseBtn(document.getElementById("pesquisar"), '32px');
            
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try{
                            var origemCidade = dojo.byId("id_origem_cidade");
                            if (hasValue(origemCidade.value) && origemCidade.value > 0) {
                                switch (parseInt(origemCidade.value)) {
                                    case PESSOARELACIONADA:
                                        retornarCidadePessoaRelacionada();
                                        break;
                                    case LOCALIDADES:
                                        retornarCidadeLocalidades();
                                        break;
                                    case LOGRADOUROFK:
                                        retornarCidadeLogradouroFK();
                                        break;
                                    case DADOSNFCAD:
                                        retornarCidadeDadosNF();
                                        break;
                                    case DADOSNFPES:
                                        retornarCidadeDadosNF();
                                        break;
                                    case TIPO_PESQUISA_FOLLOW_ESCOLA_FK:
                                        retornarCidadeEscolaFK();
                                }

                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não foi definido nenhum método de retorno");
                                apresentaMensagem("apresentadorMensagemCidadeFK", mensagensWeb);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionar");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogConsultaCidade").hide(); } }, "fechar");
                dijit.byId("paisFk").on("change", function (e) {
                    if (hasValue(e) && e > 0) {
                        dijit.byId("estadoFk").reset();
                        dijit.byId("estadoFk").set("disabled", false);
                        carregarEstadoPorPais(e,null);
                    }
                    else {
                        dijit.byId("estadoFk").reset();
                        dijit.byId("estadoFk").set("disabled", true);
                    }
                });
                adicionarAtalhoPesquisa(['nomeCidade', 'numeroMunicipio', 'paisFk', 'estadoFk'], 'pesquisar', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function pesquisaCidadeFK() {
    try{
        var pais = hasValue(dijit.byId('paisFk').value) ? dijit.byId('paisFk').value : 0;
        var estado = hasValue(dijit.byId('estadoFk').value) ? dijit.byId('estadoFk').value : 0;
        var numeroMunicipio = hasValue(dojo.byId('numeroMunicipio').value) ? dojo.byId('numeroMunicipio').value : 0;
        var cidade = hasValue(dojo.byId('nomeCidade').value) ? dojo.byId('nomeCidade').value : "";
        require([
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/GetCidadePaisEstado?pais=" + pais + "&estado=" + estado + "&numeroMunicipio=" + numeroMunicipio + "&cidade=" + encodeURIComponent(cidade),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_cidade"
                }), Memory({ idProperty: "cd_cidade" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
                gridPesquisaCidade.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Monta os Filtering
// Monta a Estado
function loadEstadoFK(data) {
    require(["dojo/_base/array",
             "dojo/store/Memory"],
    function (array, Memory) {
        try{
            (hasValue(data.retorno)) ? data = data.retorno : data;

            var dados = [];
            var arrayEstado = new Array();
            $.each(data, function (index, value) {
                dados.push({
                    'id': value.cd_localidade,
                    'name': value.no_localidade,
                    'sgl': value.sg_estado
                });
            });

            var stateStore = new Memory({
                data: eval(dados)
            });
            dijit.byId("estadoFk").store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}

// Monta a Estado
function loadPaisCidadeFK(data) {
    require(["dojo/store/Memory"],
    function ( Memory) {
        try{
            var dadoRetorno = [];

            if (hasValue($.parseJSON(data).retorno)){
                dadosRetorno = $.parseJSON(data).retorno;
            }
            var dados = [];
            $.each(dadosRetorno, function (index, value) {
                dados.push({
                    'id': value.cd_localidade,
                    'name': value.dc_pais
                });
            });

            var stateStore = new Memory({
                data: eval(dados)
            });
            dijit.byId("paisFk").store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}

function carregarEstadoPorPais(cd_pais,funcao) {
    try{
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/GetEstadoByPais?cd_pais=" + cd_pais,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            loadEstadoFK(data.retorno);
            if (hasValue(funcao))
                funcao.call();
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCidadeFK', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFiltrosCidaddeFK() {
    try{
        var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
        dojo.byId("nomeCidade").value = "";
        dijit.byId("estadoFk").reset();
        dijit.byId("estadoFk").set("disabled", true);
        dijit.byId("paisFk").reset();

        if (hasValue(gridPesquisaCidade) && hasValue(gridPesquisaCidade.itensSelecionados))
            gridPesquisaCidade.itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxCidadeFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaCidade';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasCidadesFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_cidade", grid._by_idx[rowIndex].item.cd_cidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_cidade', 'cidadeFKSelecionada', -1, 'selecionaTodasCidadesFK', 'selecionaTodasCidadesFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_cidade', 'cidadeFKSelecionada', " + rowIndex + ", '" + id + "', 'selecionaTodasCidadesFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}