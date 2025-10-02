var TIPOPESQUISACIDADELOGRADOUROFK = 3;
var TODOS = 0, PAIS_BRASIL = 1;
var LONG_ATTR = 0, SHORT_ATTR = 1;
var PESCADENDERECOPESSOA = 1, PESCADENDERECORESPONSAVELMATRICULA = 2;

function montargridPesquisaLogradouroFK(funcao) {
    require([
        "dojo/ready",
	    "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
        "dojo/on",
        "dijit/form/Button"
    ], function (ready, EnhancedGrid, Pagination, on, Button) {
        ready(function () {
            try {
                dijit.byId("pesCidadeLogradouroFK").set("disabled", true);
                decreaseBtn(document.getElementById("pesCidadeLogradouroFK"), '18px');
                decreaseBtn(document.getElementById("limparCidadeLogradouroPesFK"), '40px');
                componentesPesquisaFK(funcao);
                var gridLogradouroFK = new EnhancedGrid({
                    store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodosLogradouroFK' style='display:none'/>", field: "logradouroSelecionadoFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxLogradouroFK },
                        { name: "CEP", field: "dc_num_cep", width: "11%" },
                        { name: "Tipo", field: "no_tipo_logradouro", width: "10%" },
                        { name: "Logradouro", field: "no_localidade", width: "26%" },
                        { name: "Bairro", field: "no_localidade_bairro", width: "23%" },
                        { name: "Cidade", field: "no_localidade_cidade", width: "15%" },
                        { name: "Estado", field: "no_localidade_estado", width: "10%" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                }, "gridLogradouroFK"); // make sure you have a target HTML element with this id
                gridLogradouroFK.startup();
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarLogradourofk(true);
                    }
                }, "pesquisarLogradouroFK");
                decreaseBtn(document.getElementById("pesquisarLogradouroFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        var gridLogradouroFK = dijit.byId("gridLogradouroFK");
                        if (!hasValue(gridLogradouroFK.itensSelecionados) || gridLogradouroFK.itensSelecionados.length <= 0) {
                            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                            return false;
                        }
                        else if (gridLogradouroFK.itensSelecionados.length > 1) {
                            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                            return false;
                        }
                        else {
                            var tipo = parseInt(dojo.byId('setValuePesquisaLogradouroFK').value);
                            switch (tipo) {
                                case PESCADENDERECOPESSOA: {
                                    if (hasValue(retornarLogradouroFK))
                                        retornarLogradouroFK();
                                    break;
                                }
                                case PESCADENDERECORESPONSAVELMATRICULA: {
                                    if (hasValue(retornarLogradouroFKMat))
                                        retornarLogradouroFKMat();
                                    break;
                                }
                                    break;
                                default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                    return false;
                                    break;
                            }
                        }

                    }
                }, "selecionalogradouroFK");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("proLogradouroFK")))
                                dijit.byId("proLogradouroFK").hide();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fecharLogradouroFK");

                dijit.byId("pesCidadeLogradouroFK").on("click", function (e) {
                    try {
                        if (!hasValue(dijit.byId("gridPesquisaCidade")))
                            montargridPesquisaCidade(function () {
                                abrirPesquisaCidadeFKLogradouroFK();
                                dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                                dijit.byId("pesquisar").on("click", function (e) {
                                    pesquisaCidadeFK();
                                });
                            });
                        else
                            abrirPesquisaCidadeFKLogradouroFK();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("outrosLogradouros").on("click", function (e) {
                    try {
                        if (dijit.byId("outrosLogradouros").checked) {
                            showP('imgIntDescricaoPesqLogradouro', true);
                            //showP('trNumeroEndereco', true);
                        }
                        else {
                            showP('imgIntDescricaoPesqLogradouro', false);
                            //showP('trNumeroEndereco', false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("limparCidadeLogradouroPesFK").on("click", function (e) {
                    try {
                        limpaCidadeLogradouroFK();
                        dijit.byId('pesCidadeLogradouroFK').set("disabled", false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("estadFKLogradouro").on("change", function (cdEstado) {
                    try {
                        if (!hasValue(cdEstado) || cdEstado < TODOS)
                            dijit.byId("pesCidadeLogradouroFK").set("disabled", true);
                        else
                            dijit.byId("pesCidadeLogradouroFK").set("disabled", false);

                        //Limpa a cidade e bairro:
                        limpaCidadeLogradouroFK();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['descricaoLogradouroFK', 'pesNomCidadeLogradouroFK', 'bairroLogradouroFK'], 'pesquisarLogradouroFK', ready);

                //Removendo a validação do campo de bairro, para deixá-lo não requerido e com qualquer informação não preenchida:
                dijit.byId("bairroLogradouroFK").validate = function () { return true; };
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function limpaCidadeLogradouroFK() {
    dojo.byId('cd_cidade_pesq_logradouroFK').value = 0;
    dojo.byId("pesNomCidadeLogradouroFK").value = "";
    dijit.byId('limparCidadeLogradouroPesFK').set("disabled", true);
    dijit.byId('bairroLogradouroFK').reset();
    dijit.byId("bairroLogradouroFK").set("value", 0);
    dijit.byId('bairroLogradouroFK').set("disabled", true);
}

function formatCheckBoxLogradouroFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridLogradouroFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosLogradouroFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'logradouroSelecionadoFK', -1, 'selecionaTodosLogradouroFK', 'selecionaTodosLogradouroFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'logradouroSelecionadoFK', " + rowIndex + ", '" + id + "', 'selecionaTodosLogradouroFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaPessoaFK() {
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

function abrirPesquisaCidadeFKLogradouroFK() {
    try {
        limparFiltrosCidaddeFK();
        var compPaisCidadeFK = dijit.byId("paisFk");
        var cdEstado = dijit.byId("estadFKLogradouro").value;
        compPaisCidadeFK._onChangeActive = false;
        compPaisCidadeFK.set("value", PAIS_BRASIL);
        compPaisCidadeFK.set("disabled", true);
        compPaisCidadeFK._onChangeActive = false;
        if (hasValue(cdEstado)) {
            carregarEstadoPorPais(PAIS_BRASIL, function () {
                dijit.byId("estadoFk").set("value", cdEstado);
                dijit.byId("estadoFk").set("disabled", true);
                pesquisaCidadeFK(true);
                dijit.byId("dialogConsultaCidade").show();
            });
        }
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = TIPOPESQUISACIDADELOGRADOUROFK;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarCidadeLogradouroFK() {
    try {
        var valido = true;
        var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
        if (!hasValue(gridPesquisaCidade.itensSelecionados) || gridPesquisaCidade.itensSelecionados.length <= 0 || gridPesquisaCidade.itensSelecionados.length > 1) {
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            carregarBairroPorCidadeFK(gridPesquisaCidade.itensSelecionados[0].cd_cidade, 0);
            dojo.byId("cd_cidade_pesq_logradouroFK").value = gridPesquisaCidade.itensSelecionados[0].cd_cidade;
            dijit.byId("pesNomCidadeLogradouroFK").set("value", gridPesquisaCidade.itensSelecionados[0].no_cidade);
            dijit.byId("limparCidadeLogradouroPesFK").set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("dialogConsultaCidade").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarBairroPorCidadeFK(cd_cidade, cd_bairro, funcao) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/localidade/getBairroPorCidade?cd_cidade=" + cd_cidade + "&cd_bairro=0",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            var codigoBairroDefault = null;
            if (cd_bairro > 0)
                codigoBairroDefault = cd_bairro;
            dijit.byId("bairroLogradouroFK").set("disabled", false);
            if (hasValue(data.retorno))
                criarOuCarregarCompFiltering("bairroLogradouroFK", data.retorno, "", codigoBairroDefault, dojo.ready, dojo.store.Memory, dijit.form.Select, 'cd_localidade', 'no_localidade');
            else {
                var statusStore = new dojo.store.Memory({
                    data: []
                });
                dijit.byId("bairroLogradouroFK").store = statusStore;
                dijit.byId("bairroLogradouroFK").reset();
                dijit.byId("bairroLogradouroFK").set("value", 0);
            }
            if (hasValue(funcao))
                funcao.call();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemLogradouro', error);
    });
}

function pesquisarLogradourofk(limparItens) {
    var gridLogradouroFK = dijit.byId("gridLogradouroFK");
    apresentaMensagem('apresentadorMensagemLogradouroFK', null);
    if (dijit.byId('formPesquisaLogradouro').validate()) {
        if (document.getElementById("outrosLogradouros").checked) {
            var numero = hasValue(dojo.byId('numeroEndereco').value) ? parseInt(dojo.byId('numeroEndereco').value) : null;
            var dc_sgl_estado = hasValue(dijit.byId('estadFKLogradouro').value) ? dijit.byId('estadFKLogradouro').item.sgl : "";
            dojo.xhr.get({
                preventCache: true,
                url: Endereco() + "/api/localidade/getLogradouroCorreio?descricao=" + encodeURIComponent(dojo.byId("descricaoLogradouroFK").value) + "&estado=" +
                                 encodeURIComponent(dc_sgl_estado) + "&cidade=" + encodeURIComponent(dojo.byId('pesNomCidadeLogradouroFK').value) +
                                 "&bairro=" + encodeURIComponent(dojo.byId("bairroLogradouroFK").value) + "&cep=&numero=" + numero,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    var dataStoreLogFK = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) });
                    if (limparItens)
                        gridLogradouroFK.itensSelecionados = [];
                    if (!hasValue(data) || data.length <= 0) {
                        mensagensWeb = [];
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoLogradouroNaoEncontrado);
                        apresentaMensagem("apresentadorMensagemLogradouroFK", mensagensWeb);
                    }
                    gridLogradouroFK.setStore(dataStoreLogFK);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemLogradouroFK', error);
                });
        }
        else {
            require([
                       "dojo/store/JsonRest",
                       "dojo/data/ObjectStore",
                       "dojo/store/Cache",
                       "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var cd_bairro = hasValue(dijit.byId("bairroLogradouroFK").item) &&
                                    hasValue(dijit.byId("bairroLogradouroFK").item.id) && hasValue(dijit.byId("bairroLogradouroFK").item.name) ?
                                    dijit.byId("bairroLogradouroFK").item.id : 0;
                    var cod_cidade = dojo.byId("cd_cidade_pesq_logradouroFK").value == null || dojo.byId("cd_cidade_pesq_logradouroFK").value == "" ? 0 : dojo.byId("cd_cidade_pesq_logradouroFK").value;
                    var myStoreLogradouroFK = Cache(
                        JsonRest({
                            target: Endereco() + "/api/localidade/getLogradouroSearch?descricao=" + dojo.byId("descricaoLogradouroFK").value + "&inicio=false&cd_estado=" + dijit.byId("estadFKLogradouro").value
                                +"&cd_cidade="+ cod_cidade + "&cd_bairro=" + cd_bairro + "&cep=",
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }), Memory({})
                   );
                    var dataStoreLogFK = new ObjectStore({ objectStore: myStoreLogradouroFK });
                    if (limparItens)
                        gridLogradouroFK.itensSelecionados = [];
                    gridLogradouroFK.setStore(dataStoreLogFK);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        }
    }
}

function limparFiltrosLogradouroFK() {
    try {
        var gridLogradouroFK = dijit.byId("gridLogradouroFK");
        showP('imgIntDescricaoPesqLogradouro', false);
        //showP('trNumeroEndereco', false);
        dojo.byId("descricaoLogradouroFK").value = "";
        dojo.byId("cd_cidade_pesq_logradouroFK").value = 0;
        dojo.byId("pesNomCidadeLogradouroFK").value = "";
        dijit.byId("bairroLogradouroFK").reset();
        dijit.byId("bairroLogradouroFK").set("value", 0);
        dijit.byId("outrosLogradouros").reset();
        dijit.byId("estadFKLogradouro").reset();
        if (hasValue(gridLogradouroFK) && hasValue(gridLogradouroFK.itensSelecionados))
            gridLogradouroFK.itensSelecionados = [];
        gridLogradouroFK.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory(null) }));
        gridLogradouroFK.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function componentesPesquisaFK(funcao) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/localidade/GetEstadoByPais?cd_pais=" + PAIS_BRASIL,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (hasValue(data.retorno))
                loadEstado(data.retorno, "estadFKLogradouro", dojo.store.Memory);
            if (hasValue(funcao))
                funcao.call();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
   function (error) {
       apresentaMensagem('apresentadorMensagemLogradouro', error);
   });
}

// Monta a Estado
function loadEstado(data, idComponente, Memory) {
    try {
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
        dijit.byId(idComponente).store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}