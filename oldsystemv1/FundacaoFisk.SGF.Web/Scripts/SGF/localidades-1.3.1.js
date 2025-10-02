//Os tipos de formulário:
var FORM_PAIS = 1;
var FORM_ESTADO = 2;
var FORM_CIDADE = 3;
var FORM_BAIRRO = 4;
var FORM_DISTRITO = 5;
var FORM_TIPO_ENDERECO = 6;
var FORM_CLASSE_TELEFONE = 7;
var FORM_TIPO_LOGRADOURO = 8;
var FORM_TIPO_TELEFONE = 9;
var FORM_OPERADORA = 10;
var FORM_ATIVIDADE = 11;
var FORM_LOGRADOURO = 12;
var PESQUISACIDADELOGRADOURO = 1, CADPESQUISACIDADELOGRADOURO = 2,PESQUISARCIDADEDISTRITO = 3, PESQUISACIDADECADDISTRITO = 4, PESQUISACIDADEBAIRRO = 5, PESQUISACIDADECADBAIRRO;
var PESQUISABAIRROLOGRADOURO = 1, CADPESQUISABAIRROLOGRADOURO = 2;
var PESQUISARCIDADELOGRADOURO = 2;
var DADOSNF = 4;


function loadPais() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
        xhr.get({
            url: Endereco() +"/api/localidade/getAllPais",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataPais) {
            try{
                var itemsCb = [];
                var cbPais = dijit.byId("cd_pais");

                Array.forEach(jQuery.parseJSON(dataPais).retorno, function (value, i) {
                    itemsCb.push({ id: value.cd_localidade, name: value.dc_pais });
                });
                var stateStore = new Memory({
                    data: itemsCb
                });
                cbPais.store = stateStore;
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemEstado', error);
        });
    });
}
function loadEstado() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getAllEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try{
                    var itemsCb = [];
                    var cbEstado = dijit.byId("cd_estado");

                    Array.forEach(dataEstado.retorno, function (value, i) {
                        itemsCb.push({ id: value.cd_localidade, name: value.sg_estado });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbEstado.store = stateStore;
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
}
function loadPesquisaPais() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function ( xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getPaisEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataPais) {
                try{
                    var itemsCb = [];
                    itemsCb.push({ id: 0, name: "Todos" });
                    var cbPais = dijit.byId("pesquisapais");

                    Array.forEach(jQuery.parseJSON(dataPais).retorno, function (value, i) {
                        itemsCb.push({ id: value.cd_localidade, name: value.dc_pais });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbPais.store = stateStore;
                    cbPais.set("value", 0);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemEstado', error);
            });
        });
}

function loadPesquisaEstado() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/GetEstadoEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try{
                    var itemsCb = [];
                    itemsCb.push({ id: 0, name: "Todos" });
                    var cbEstado = dijit.byId("pesquisaEst");

                    Array.forEach(dataEstado.retorno, function (value, i) {
                        itemsCb.push({ id: value.cd_localidade, name: value.sg_estado });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbEstado.store = stateStore;
                    cbEstado.set("value", 0);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
}

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, value, grid, ehLink) {//(tipoForm, value) {
    try{
        loadPais();
        loadEstado();
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

        switch (tipoForm) {
            case FORM_PAIS: {
                getLimpar('#formPais');
                dojo.byId("cd_localidade").value = value.cd_localidade;
                dojo.byId("cd_tipo_localidade").value = "1";
                dojo.byId("dc_num_pais").value = value.dc_num_pais;
                dojo.byId("dc_nacionalidade_masc").value = value.dc_nacionalidade_masc;
                dojo.byId("dc_pais").value = value.dc_pais;
                dojo.byId("dc_nacionalidade_fem").value = value.dc_nacionalidade_fem;
                dojo.byId("sg_pais").value = value.sg_pais;
                return false;
            }
            case FORM_ESTADO: {

                getLimpar('#formEstado');
                dojo.byId("cd_localidade_estado").value = value.cd_localidade;
                dojo.byId("cd_tipo_localidade").value = "2";
                dojo.byId("no_localidade").value = value.no_localidade;
                dojo.byId("sg_estado").value = value.sg_estado;

                dijit.byId('cd_pais')._onChangeActive = false;
                dijit.byId('cd_pais').set('value', value.cd_loc_relacionada);
                dijit.byId('cd_pais')._onChangeActive = true;

                return false;
            }
            case FORM_CIDADE: {
                getLimpar('#formCidade');
                dojo.byId("cd_localidade_cidade").value = value.cd_localidade;
                dojo.byId("cd_tipo_localidade").value = "3";
                dojo.byId("no_localidade_cidade").value = value.no_localidade;
                dojo.byId("nm_municipio").value = value.nm_municipio;
                dijit.byId('cd_estado')._onChangeActive = false;
                dijit.byId('cd_estado').set('value', value.cd_loc_relacionada);
                dijit.byId('cd_estado')._onChangeActive = true;
                return false;
            }
            case FORM_BAIRRO: {
                getLimpar('#formBairro');
                dojo.byId("cd_localidade_bairro").value = value.cd_localidade;
                dojo.byId("cd_tipo_localidade").value = "4";
                dojo.byId("no_localidade_bairro").value = value.no_localidade;
                dojo.byId("cd_cidade_cad_bairro").value = value.cd_loc_relacionada;
                dojo.byId("cadNomCidadeBairro").value = value.no_localidade_cidade;
                dijit.byId("cadNomCidadeBairro").set("value", value.no_localidade_cidade);
                return false;
            }
            case FORM_DISTRITO: {
                getLimpar('#formDistrito');
                dojo.byId("cd_localidade_distrito").value = value.cd_localidade;
                dojo.byId("cd_tipo_localidade").value = "5";
                dojo.byId("no_localidade_distrito").value = value.no_localidade;
                dojo.byId("cd_cidade_cad_distrito").value = value.cd_loc_relacionada;
                dojo.byId("cadNomCidadeDistrito").value = value.no_localidade_cidade;
                dijit.byId("cadNomCidadeDistrito").set("value", value.no_localidade_cidade);
                return false;
            }
            case FORM_TIPO_ENDERECO: {
                getLimpar('#formTipoEndereco');
                dojo.byId("cd_tipo_endereco").value = value.cd_tipo_endereco;
                dojo.byId("no_tipo_endereco").value = value.no_tipo_endereco;
                return false;
            }
            case FORM_CLASSE_TELEFONE: {
                getLimpar('#formClasseTelefone');
                dojo.byId("cd_classe_telefone").value = value.cd_classe_telefone;
                dojo.byId("dc_classe_telefone").value = value.dc_classe_telefone;
                return false;
            }
            case FORM_TIPO_LOGRADOURO: {
                getLimpar('#formTipoLogradouro');
                dojo.byId("cd_tipo_logradouro").value = value.cd_tipo_logradouro;
                dojo.byId("no_tipo_logradouro").value = value.no_tipo_logradouro;
                dojo.byId("sg_tipo_logradouro").value = value.sg_tipo_logradouro;
                return false;
            }
            case FORM_TIPO_TELEFONE: {
                getLimpar('#formTipoTelefone');
                dojo.byId("cd_tipo_telefone").value = value.cd_tipo_telefone;
                dojo.byId("no_tipo_telefone").value = value.no_tipo_telefone;
                return false;
            }
            case FORM_OPERADORA: {
                getLimpar('#formOperadora');
                dojo.byId("cd_operadora").value = value.cd_operadora;
                dojo.byId("no_operadora").value = value.no_operadora;
                dojo.byId("operadora_ativa").value = value.id_operadora_ativa == true ? dijit.byId("operadora_ativa").set("value", true) : dijit.byId("operadora_ativa").set("value", false);
                return false;
            }
            case FORM_ATIVIDADE: {
                getLimpar('#formAtividade');
                dojo.byId("cd_atividade").value = value.cd_atividade;
                dojo.byId("no_atividade").value = value.no_atividade;
                dojo.byId("atividade_ativa").value = value.id_atividade_ativa == true ? dijit.byId("atividade_ativa").set("value", true) : dijit.byId("atividade_ativa").set("value", false);
                dojo.byId("cd_cnae_atividade").value = value.cd_cnae_atividade;
                dojo.byId("id_natureza_atividade").value = value.id_natureza_atividade == 1 ? 'Física' : 'Jurídica';
                return false;
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function loadNatureza() {
    require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
        function (ready, Memory, filteringSelect) {
            try{
                var stateStore = new Memory({
                    data: [
                    { name: "Todos", id: "0"},
                    { name: "Física", id: "1" },
                    { name: "Jurídica", id: "2" }
                    ]
                });

                //dijit.byId("naturezaAtividade").store = stateStore;
                //dijit.byId("naturezaAtividade").value = 0;
                ready(function () {
                    var natureza = new filteringSelect({
                        id: "natureza",
                        name: "natureza",
                        value: "0",
                        store: stateStore,
                        searchAttr: "name",
                        style: "width: 75px;"
                    }, "naturezaAtividade");
                })

                var stateStoreCad = new Memory({
                    data: [
                    { name: "Física", id: "1" },
                    { name: "Jurídica", id: "2" }
                    ]
                });
                dijit.byId("id_natureza_atividade").store = stateStoreCad;
            } catch (e) {
                postGerarLog(e);
            }
        });
}

////$("#labelTitulos").val("Secretaria");
function mostraTabs(Permissoes) {
    require([
         "dijit/registry",
         "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            if (!possuiPermissao('pais', Permissoes)) {
                registry.byId('tabPais').set('disabled', !registry.byId('tabPais').get('disabled'));
                document.getElementById('tabPais').style.visibility = "hidden";
            }
            if (!possuiPermissao('estad', Permissoes)) {
                registry.byId('tabEstado').set('disabled', !registry.byId('tabEstado').get('disabled'));
                document.getElementById('tabEstado').style.visibility = "hidden";
            }
            if (!possuiPermissao('cidd', Permissoes)) {
                registry.byId('tabCidade').set('disabled', !registry.byId('tabCidade').get('disabled'));
                document.getElementById('tabCidade').style.visibility = "hidden";
            }
            if (!possuiPermissao('bair', Permissoes)) {
                registry.byId('tabBairro').set('disabled', !registry.byId('tabBairro').get('disabled'));
                document.getElementById('tabBairro').style.visibility = "hidden";
            }
            if (!possuiPermissao('dist', Permissoes)) {
                registry.byId('tabDistrito').set('disabled', !registry.byId('tabDistrito').get('disabled'));
                document.getElementById('tabDistrito').style.visibility = "hidden";
            }
            if (!possuiPermissao('tpend', Permissoes)) {
                registry.byId('tabTipoEndereco').set('disabled', !registry.byId('tabTipoEndereco').get('disabled'));
                document.getElementById('tabTipoEndereco').style.visibility = "hidden";
            }
            if (!possuiPermissao('ctele', Permissoes)) {
                registry.byId('tabClasseTelefone').set('disabled', !registry.byId('tabClasseTelefone').get('disabled'));
                document.getElementById('tabClasseTelefone').style.visibility = "hidden";
            }
            if (!possuiPermissao('tlog', Permissoes)) {
                registry.byId('tabTipoLogradouro').set('disabled', !registry.byId('tabTipoLogradouro').get('disabled'));
                document.getElementById('tabTipoLogradouro').style.visibility = "hidden";
            }
            if (!possuiPermissao('ttele', Permissoes)) {
                registry.byId('tabTipoTelefone').set('disabled', !registry.byId('tabTipoTelefone').get('disabled'));
                document.getElementById('tabTipoTelefone').style.visibility = "hidden";
            }
            if (!possuiPermissao('lgra', Permissoes)) {
                registry.byId('tabLogradouro').set('disabled', !registry.byId('tabLogradouro').get('disabled'));
                document.getElementById('tabLogradouro').style.visibility = "hidden";
            }
            if (!possuiPermissao('oper', Permissoes)) {
                registry.byId('tabOperadora').set('disabled', !registry.byId('tabOperadora').get('disabled'));
                document.getElementById('tabOperadora').style.visibility = "hidden";
            }
        })
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridPais';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxEstado(value, rowIndex, obj) {
    try{
        var gridName = 'gridEstado';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEstado');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoEstado', -1, 'selecionaTodosEstado', 'selecionaTodosEstado', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'selecionadoEstado', " + rowIndex + ", '" + id + "', 'selecionaTodosEstado', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxCidade(value, rowIndex, obj) {
    try{
        var gridName = 'gridCidade';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCidade');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoCidade', -1, 'selecionaTodosCidade', 'selecionaTodosCidade', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'selecionadoCidade', " + rowIndex + ", '" + id + "', 'selecionaTodosCidade', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxBairro(value, rowIndex, obj) {
    try{
        var gridName = 'gridBairro';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBairro');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoBairro', -1, 'selecionaTodosBairro', 'selecionaTodosBairro', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'selecionadoBairro', " + rowIndex + ", '" + id + "', 'selecionaTodosBairro', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxDistrito(value, rowIndex, obj) {
    try{
        var gridName = 'gridDistrito';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosDistrito');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoDistrito', -1, 'selecionaTodosDistrito', 'selecionaTodosDistrito', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'selecionadoDistrito', " + rowIndex + ", '" + id + "', 'selecionaTodosDistrito', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxTipoEndereco(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoEndereco';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTipoEndereco');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_endereco", grid._by_idx[rowIndex].item.cd_tipo_endereco);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_endereco', 'selecionadoTipoEndereco', -1, 'selecionaTodosTipoEndereco', 'selecionaTodosTipoEndereco', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_endereco', 'selecionadoTipoEndereco', " + rowIndex + ", '" + id + "', 'selecionaTodosTipoEndereco', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxTipoLogradouro(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoLogradouro';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTipoLogradouro');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_logradouro", grid._by_idx[rowIndex].item.cd_tipo_logradouro);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_logradouro', 'selecionadoTipoLogradouro', -1, 'selecionaTodosTipoLogradouro', 'selecionaTodosTipoLogradouro', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_logradouro', 'selecionadoTipoLogradouro', " + rowIndex + ", '" + id + "', 'selecionaTodosTipoLogradouro', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxTipoTelefone(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoTelefone';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTipoTelefone');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_telefone", grid._by_idx[rowIndex].item.cd_tipo_telefone);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_telefone', 'selecionadoTipoTelefone', -1, 'selecionaTodosTipoTelefone', 'selecionaTodosTipoTelefone', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_telefone', 'selecionadoTipoTelefone', " + rowIndex + ", '" + id + "', 'selecionaTodosTipoTelefone', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxClasseTelefone(value, rowIndex, obj) {
    try{
        var gridName = 'gridClasseTelefone';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosClasseTelefone');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_classe_telefone", grid._by_idx[rowIndex].item.cd_classe_telefone);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_classe_telefone', 'selecionadoClasseTelefone', -1, 'selecionaTodosClasseTelefone', 'selecionaTodosClasseTelefone', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_classe_telefone', 'selecionadoClasseTelefone', " + rowIndex + ", '" + id + "', 'selecionaTodosClasseTelefone', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxOperadora(value, rowIndex, obj) {
    try{
        var gridName = 'gridOperadora';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosOperadora');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_operadora", grid._by_idx[rowIndex].item.cd_operadora);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_operadora', 'selecionadoOperadora', -1, 'selecionaTodosOperadora', 'selecionaTodosOperadora', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_operadora', 'selecionadoOperadora', " + rowIndex + ", '" + id + "', 'selecionaTodosOperadora', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxAtividade(value, rowIndex, obj) {
    try{
        var gridName = 'gridAtividade';
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
    } catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxLogradouro(value, rowIndex, obj) {
    try{
        var gridName = 'gridLogradouro';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosLogradouro');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_localidade", grid._by_idx[rowIndex].item.cd_localidade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_localidade', 'logradouroSelecionado', -1, 'selecionaTodosLogradouro', 'selecionaTodosLogradouro', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_localidade', 'logradouroSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosLogradouro', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarEstado()
{
    require([
           "dojo/dom",
           "dojox/grid/EnhancedGrid",
           "dojox/grid/enhanced/plugins/Pagination",
           "dojo/store/JsonRest",
           "dojo/data/ObjectStore",
           "dojo/store/Cache",
           "dojo/store/Memory",
           "dojo/query",
           "dijit/form/Button",
           "dijit/form/DropDownButton",
           "dijit/DropDownMenu",
           "dijit/MenuItem",
           "dojo/ready"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try{

            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getestadosearch?descricao=&inicio=false&cdPais=0",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_localidade"
                }), Memory({ idProperty: "cd_localidade" })
           );
            loadPais();
            loadPesquisaPais();
            var gridEstado = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosEstado' style='display:none'/>", field: "selecionadoEstado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEstado },
                  //  { name: "Código", field: "cd_localidade", width: "5%" },
                	{ name: "Estado", field: "no_localidade", width: "70%" },
                    { name: "Sigla", field: "sg_estado", width: "10%" },
                    { name: "País", field: "no_pais", width: "15%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridEstado"); // make sure you have a target HTML element with this id
            gridEstado.startup();
            gridEstado.pagination.plugin._paginator.plugin.connect(gridEstado.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridEstado, 'cd_localidade_estado', 'selecionaTodos'); });
            gridEstado.canSort = function (col) { return Math.abs(col) != 1; };
            gridEstado.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formEstado');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_ESTADO, null, gridEstado, false);
                    dijit.byId("formularioEstado").show();
                    IncluirAlterar(0, 'divAlterarEstado', 'divIncluirEstado', 'divExcluirEstado', 'apresentadorMensagemEstado', 'divCancelarEstado', 'divLimparEstado');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarEstado', 'divIncluirEstado', 'divExcluirEstado', 'apresentadorMensagemEstado', 'divCancelarEstado', 'divLimparEstado');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosEstado(); } }, "incluirEstado");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosEstado(); } }, "alterarEstado");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarEstado() }); } }, "deleteEstado");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparEstado();
                }
            }, "limparEstado");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_ESTADO, null, gridEstado, null);
                }
            }, "cancelarEstado");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioEstado").hide();
                }
            }, "fecharEstado");
            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridEstado, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosEstado').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoEstado', -1, 'selecionaTodosEstado', 'selecionaTodosEstado', 'gridEstado')", gridEstado.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarEstado(gridEstado.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridEstado.itensSelecionados, 'DeletarEstado(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasEstado",
                dropDown: menu,
                id: "acoesRelacionadasEstado"
            });
            dom.byId("linkAcoesEstado").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridEstado, 'todosItensEstado', ['pesquisarEstado', 'relatorioEstado']); PesquisarEstado(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridEstado', 'selecionadoEstado', 'cd_localidade', 'selecionaTodosEstado', ['pesquisarEstado', 'relatorioEstado'], 'todosItensEstado'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensEstado",
                dropDown: menu,
                id: "todosItensEstado"
            });
            dom.byId("linkSelecionadosEstado").appendChild(button.domNode);
            ///////////////////////////////////////
            adicionarAtalhoPesquisa(['pesquisaEstado', 'pesquisapais'], 'pesquisarEstado', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarCidade() {
    require([
       "dojo/dom",
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
       "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            loadEstado();
            loadPesquisaEstado();
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getCidadesearch?descricao=&inicio=false&nmMunicipio=0&cdEstado=0",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_localidade"
                }), Memory({ idProperty: "cd_localidade" })
           );

            var gridCidade = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosCidade' style='display:none'/>", field: "selecionadoCidade", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCidade },
                  //  { name: "Código", field: "cd_localidade", width: "5%" },
                	{ name: "Cidade", field: "no_localidade", width: "75%" },
                    { name: "Nr. Município", field: "nm_municipio", width: "10%" },
                    { name: "Estado", field: "sg_estado", width: "10%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridCidade"); // make sure you have a target HTML element with this id
            gridCidade.startup();
            gridCidade.pagination.plugin._paginator.plugin.connect(gridCidade.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridPais, 'cd_localidade_cidade', 'selecionaTodos'); });
            gridCidade.canSort = function (col) { return Math.abs(col) != 1; };
            gridCidade.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formCidade');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_CIDADE, null, gridCidade, false);
                    dijit.byId("formularioCidade").show();
                    IncluirAlterar(0, 'divAlterarCidade', 'divIncluirCidade', 'divExcluirCidade', 'apresentadorMensagemCidade', 'divCancelarCidade', 'divLimparCidade');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarCidade', 'divIncluirCidade', 'divExcluirCidade', 'apresentadorMensagemCidade', 'divCancelarCidade', 'divLimparCidade');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosCidade(); } }, "incluirCidade");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosCidade(); } }, "alterarCidade");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            DeletarCidade();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                }
            }, "deleteCidade");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparCidade();
                }
            }, "limparCidade");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_CIDADE, null, gridCidade, null);
                }
            }, "cancelarCidade");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioCidade").hide();
                }
            }, "fecharCidade");
            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridCidade, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosCidade').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoCidade', -1, 'selecionaTodosCidade', 'selecionaTodosCidade', 'gridCidade')", gridCidade.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarCidade(gridCidade.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridCidade.itensSelecionados, 'DeletarCidade(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasCidade",
                dropDown: menu,
                id: "acoesRelacionadasCidade"
            });
            dom.byId("linkAcoesCidade").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridCidade, 'todosItensCidade', ['pesquisarCidade', 'relatorioCidade']); PesquisarCidade(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridCidade', 'selecionadoCidade', 'cd_localidade_cidade', 'selecionaTodosCidade', ['pesquisarCidade', 'relatorioCidade'], 'todosItensCidade'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensCidade",
                dropDown: menu,
                id: "todosItensCidade"
            });
            dom.byId("linkSelecionadosCidade").appendChild(button.domNode);
            ///////////////////////////////////////
            adicionarAtalhoPesquisa(['pesquisaCidade', 'pesmunicipio', 'pesquisaEst'], 'pesquisarCidade', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarBairro() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getBairrosearch?descricao=&inicio=false&cd_cidade=0",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_localidade"
                }), Memory({ idProperty: "cd_localidade" })
           );

            var gridBairro = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosBairro' style='display:none'/>", field: "selecionadoBairro", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBairro },
                    //{ name: "Código", field: "cd_localidade", width: "5%" },
                	{ name: "Bairro", field: "no_localidade", width: "45%" },
                	{ name: "Cidade", field: "no_localidade_cidade", width: "50%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridBairro"); // make sure you have a target HTML element with this id
            gridBairro.startup();
            gridBairro.pagination.plugin._paginator.plugin.connect(gridBairro.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridBairro, 'cd_localidade_bairro', 'selecionaTodos'); });
            gridBairro.canSort = function (col) { return Math.abs(col) != 1; };
            gridBairro.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formBairro');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_BAIRRO, null, gridBairro, false);
                    dijit.byId("formularioBairro").show();
                    IncluirAlterar(0, 'divAlterarBairro', 'divIncluirBairro', 'divExcluirBairro', 'apresentadorMensagemBairro', 'divCancelarBairro', 'divLimparBairro');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarBairro', 'divIncluirBairro', 'divExcluirBairro', 'apresentadorMensagemBairro', 'divCancelarBairro', 'divLimparBairro');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosBairro(); } }, "incluirBairro");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosBairro(); } }, "alterarBairro");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            DeletarBairro();
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    });
                }
            }, "deleteBairro");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparBairro();
                }
            }, "limparBairro");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_BAIRRO, null, gridBairro, null);
                }
            }, "cancelarBairro");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioBairro").hide();
                }
            }, "fecharBairro");

            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridBairro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosBairro').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade_bairro', 'selecionadoBairro', -1, 'selecionaTodosBairro', 'selecionaTodosBairro', 'gridBairro')", gridBairro.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarBairro(gridBairro.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridBairro.itensSelecionados, 'DeletarBairro(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasBairro",
                dropDown: menu,
                id: "acoesRelacionadasBairro"
            });
            dom.byId("linkAcoesBairro").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridBairro, 'todosItensBairro', ['pesquisarBairro', 'relatorioBairro']); PesquisarBairro(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridBairro', 'selecionadoBairro', 'cd_localidade_bairro', 'selecionaTodosBairro', ['pesquisarBairro', 'relatorioBairro'], 'todosItensBairro'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensBairro",
                dropDown: menu,
                id: "todosItensBairro"
            });
            dom.byId("linkSelecionadosBairro").appendChild(button.domNode);

            dijit.byId("pesCidadeBairro").on("click", function (e) {
                if (!hasValue(dijit.byId("gridPesquisaCidade")))
                    montargridPesquisaCidade(function () {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                        dijit.byId("pesquisar").on("click", function (e) {
                            pesquisaCidadeFK();
                        });
                        dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADEBAIRRO;
                    });
                else {
                    abrirPesquisaCidadeFKLocalidades();
                    dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADEBAIRRO;
                }
            });
            dijit.byId("cadCidadeBairro").on("click", function (e) {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaCidade")))
                        montargridPesquisaCidade(function () {
                            abrirPesquisaCidadeFKLocalidades();
                            dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                            dijit.byId("pesquisar").on("click", function (e) {
                                pesquisaCidadeFK();
                            });
                            dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADECADBAIRRO;
                        });
                    else {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADECADBAIRRO;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("limparCidadeBairro").on("click", function (e) {
                dojo.byId('cd_cidade_pesq_bairro').value = 0;
                dojo.byId("pesNomCidadeBairro").value = "";
                dijit.byId('limparCidadeBairro').set("disabled", true);
            });
            adicionarAtalhoPesquisa(['pesquisaBairro'], 'pesquisarBairro', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarDistrito() {
    require([
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dojo/ready",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/on"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, ready, Button, DropDownButton, DropDownMenu, MenuItem, on) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getDistritosearch?descricao=&inicio=false&cd_cidade=0",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_localidade"
                }), Memory({ idProperty: "cd_localidade" })
           );

            var gridDistrito = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosDistrito' style='display:none'/>", field: "selecionadoDistrito", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxDistrito },
                 //   { name: "Código", field: "cd_localidade", width: "5%" },
                	{ name: "Distrito", field: "no_localidade", width: "55%" },
                	{ name: "Cidade", field: "no_localidade_cidade", width: "40%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridDistrito"); // make sure you have a target HTML element with this id
            gridDistrito.startup();
            gridDistrito.pagination.plugin._paginator.plugin.connect(gridDistrito.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridDistrito, 'cd_localidade_distrito', 'selecionaTodos'); });
            gridDistrito.canSort = function (col) { return Math.abs(col) != 1; };
            gridDistrito.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formDistrito');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_DISTRITO, null, gridDistrito, false);
                    dijit.byId("formularioDistrito").show();
                    IncluirAlterar(0, 'divAlterarDistrito', 'divIncluirDistrito', 'divExcluirDistrito', 'apresentadorMensagemDistrito', 'divCancelarDistrito', 'divLimparDistrito');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarDistrito', 'divIncluirDistrito', 'divExcluirDistrito', 'apresentadorMensagemDistrito', 'divCancelarDistrito', 'divLimparDistrito');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosDistrito(); } }, "incluirDistrito");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosDistrito(); } }, "alterarDistrito");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            DeletarDistrito();
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    });
                }
            }, "deleteDistrito");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparDistrito();
                }
            }, "limparDistrito");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_DISTRITO, null, gridDistrito, null);
                }
            }, "cancelarDistrito");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioDistrito").hide();
                }
            }, "fecharDistrito");
            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridEstado, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosDistrito').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoDistrito', -1, 'selecionaTodosDistrito', 'selecionaTodosDistrito', 'gridDistrito')", gridDistrito.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarDistrito(gridDistrito.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridDistrito.itensSelecionados, 'DeletarDistrito(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasDistrito",
                dropDown: menu,
                id: "acoesRelacionadasDistrito"
            });
            dom.byId("linkAcoesDistrito").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridDistrito, 'todosItensDistrito', ['pesquisarDistrito', 'relatorioDistrito']); PesquisarDistrito(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridDistrito', 'selecionadoDistrito', 'cd_localidade', 'selecionaTodosDistrito', ['pesquisarDistrito', 'relatorioDistrito'], 'todosItensDistrito'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensDistrito",
                dropDown: menu,
                id: "todosItensDistrito"
            });
            dom.byId("linkSelecionadosDistrito").appendChild(button.domNode);

            dijit.byId("pesCidadeDistrito").on("click", function (e) {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaCidade")))
                        montargridPesquisaCidade(function () {
                            abrirPesquisaCidadeFKLocalidades();
                            dijit.byId("pesquisar").on("click", function (e) {
                                pesquisaCidadeFK();
                            });
                            dojo.byId("tipoPesquisaCidade").value = PESQUISARCIDADEDISTRITO;
                        });
                    else {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.byId("tipoPesquisaCidade").value = PESQUISARCIDADEDISTRITO;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("cadCidadeDistrito").on("click", function (e) {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaCidade")))
                        montargridPesquisaCidade(function () {
                            abrirPesquisaCidadeFKLocalidades();
                            dijit.byId("pesquisar").on("click", function (e) {
                                pesquisaCidadeFK();
                            });
                            dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADECADDISTRITO;
                        });
                    else {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADECADDISTRITO;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("limparCidadeDistritoPes").on("click", function (e) {
                try{
                    dojo.byId('cd_cidade_pesq_distrito').value = 0;
                    dojo.byId("pesNomCidadeDistrito").value = "";
                    dijit.byId('limparCidadeDistritoPes').set("disabled", true);
                } catch (e) {
                    postGerarLog(e);
                }
            });
            adicionarAtalhoPesquisa(['pesquisaDistrito'], 'pesquisarDistrito', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarTipoEndereco() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoEnderecosearch?descricao=&inicio=false",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_endereco"
                }), Memory({ idProperty: "cd_tipo_endereco" })
           );

            var gridTipoEndereco = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTipoEndereco' style='display:none'/>", field: "selecionadoTipoEndereco", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoEndereco },
                //    { name: "Código", field: "cd_tipo_endereco", width: "5%" },
               		  { name: "Tipo de Endereço", field: "no_tipo_endereco", width: "95%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridTipoEndereco"); // make sure you have a target HTML element with this id
            gridTipoEndereco.startup();
            gridTipoEndereco.pagination.plugin._paginator.plugin.connect(gridTipoEndereco.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridTipoEndereco, 'cd_tipo_endereco', 'selecionaTodos'); });
            gridTipoEndereco.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoEndereco.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formTipoEndereco');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_TIPO_ENDERECO, null, gridTipoEndereco, false);
                    dijit.byId("formularioTipoEndereco").show();
                    IncluirAlterar(0, 'divAlterarTipoEndereco', 'divIncluirTipoEndereco', 'divExcluirTipoEndereco', 'apresentadorMensagemTipoEndereco', 'divCancelarTipoEndereco', 'divLimparTipoEndereco');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarTipoEndereco', 'divIncluirTipoEndereco', 'divExcluirTipoEndereco', 'apresentadorMensagemTipoEndereco', 'divCancelarTipoEndereco', 'divLimparTipoEndereco');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoEndereco(); } }, "incluirTipoEndereco");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoEndereco(); } }, "alterarTipoEndereco");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            DeletarTipoEndereco();
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    });
                }
            }, "deleteTipoEndereco");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparTipoEndereco();
                }
            }, "limparTipoEndereco");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_TIPO_ENDERECO, null, gridTipoEndereco, null);
                }
            }, "cancelarTipoEndereco");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioTipoEndereco").hide();
                }
            }, "fecharTipoEndereco");

            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoEndereco, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTipoEndereco').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoTipoEndereco', -1, 'selecionaTodosTipoEndereco', 'selecionaTodosTipoEndereco', 'gridTipoEndereco')", gridTipoEndereco.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarTipoEndereco(gridTipoEndereco.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTipoEndereco.itensSelecionados, 'DeletarTipoEndereco(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoEndereco",
                dropDown: menu,
                id: "acoesRelacionadasTipoEndereco"
            });
            dom.byId("linkAcoesTipoEndereco").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoEndereco, 'todosItensTipoEndereco', ['pesquisarTipoEndereco', 'relatorioTipoEndereco']); PesquisarTipoEndereco(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoEndereco', 'selecionadoTipoEndereco', 'cd_localidade', 'selecionaTodosTipoEndereco', ['pesquisarTipoEndereco', 'relatorioTipoEndereco'], 'todosItensTipoEndereco'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoEndereco",
                dropDown: menu,
                id: "todosItensTipoEndereco"
            });
            dom.byId("linkSelecionadosTipoEndereco").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaTipoEndereco'], 'pesquisarTipoEndereco', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarClasseTelefone() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getClasseTelefonesearch?descricao=&inicio=false",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_classe_telefone"
                }), Memory({ idProperty: "cd_classe_telefone" })
           );

            var gridClasseTelefone = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosClasseTelefone' style='display:none'/>", field: "selecionadoClasseTelefone", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxClasseTelefone },
                  //  { name: "Código", field: "cd_classe_telefone", width: "5%" },
               		  { name: "Classe do Telefone", field: "dc_classe_telefone", width: "95%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridClasseTelefone"); // make sure you have a target HTML element with this id
            gridClasseTelefone.startup();
            gridClasseTelefone.pagination.plugin._paginator.plugin.connect(gridClasseTelefone.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridClasseTelefone, 'cd_tipo_telefone', 'selecionaTodos'); });
            gridClasseTelefone.canSort = function (col) { return Math.abs(col) != 1; };
            gridClasseTelefone.on("RowDblClick", function (evt) {
                try{
                    getLimpar('#formClasseTelefone');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_CLASSE_TELEFONE, null, gridClasseTelefone, false);
                    dijit.byId("formularioClasseTelefone").show();
                    IncluirAlterar(0, 'divAlterarClasseTelefone', 'divIncluirClasseTelefone', 'divExcluirClasseTelefone', 'apresentadorMensagemClasseTelefone', 'divCancelarClasseTelefone', 'divLimparClasseTelefone');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarClasseTelefone', 'divIncluirClasseTelefone', 'divExcluirClasseTelefone', 'apresentadorMensagemClasseTelefone', 'divCancelarClasseTelefone', 'divLimparClasseTelefone');
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosClasseTelefone(); } }, "incluirClasseTelefone");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosClasseTelefone(); } }, "alterarClasseTelefone");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarClasseTelefone() }); } }, "deleteClasseTelefone");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparClasseTelefone();
                }
            }, "limparClasseTelefone");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_CLASSE_TELEFONE, null, gridClasseTelefone, null);
                }
            }, "cancelarClasseTelefone");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioClasseTelefone").hide();
                }
            }, "fecharClasseTelefone");


            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridClasseTelefone, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosClasseTelefone').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoClasseTelefone', -1, 'selecionaTodosClasseTelefone', 'selecionaTodosClasseTelefone', 'gridClasseTelefone')", gridClasseTelefone.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarClasseTelefone(gridClasseTelefone.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemover(gridClasseTelefone.itensSelecionados, 'DeletarClasseTelefone(itensSelecionados)'); }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasClasseTelefone",
                dropDown: menu,
                id: "acoesRelacionadasClasseTelefone"
            });
            dom.byId("linkAcoesClasseTelefone").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridClasseTelefone, 'todosItensClasseTelefone', ['pesquisarClasseTelefone', 'relatorioClasseTelefone']); PesquisarClasseTelefone(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridClasseTelefone', 'selecionadoClasseTelefone', 'cd_localidade', 'selecionaTodosClasseTelefone', ['pesquisarClasseTelefone', 'relatorioClasseTelefone'], 'todosItensClasseTelefone'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensClasseTelefone",
                dropDown: menu,
                id: "todosItensClasseTelefone"
            });
            dom.byId("linkSelecionadosClasseTelefone").appendChild(button.domNode);
            ///////////////////////////////////////
            adicionarAtalhoPesquisa(['pesquisaClasseTelefone'], 'pesquisarClasseTelefone', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarTipoLogradouro() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoLogradourosearch?descricao=&inicio=false",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_logradouro"
                }), Memory({ idProperty: "cd_tipo_logradouro" })
           );

            var gridTipoLogradouro = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTipoLogradouro' style='display:none'/>", field: "selecionadoTipoLogradouro", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoLogradouro },
                //    { name: "Código", field: "cd_tipo_logradouro", width: "5%" },
                	  { name: "Tipo de Logradouro", field: "no_tipo_logradouro", width: "65%" },
                    { name: "Sigla", field: "sg_tipo_logradouro", width: "30%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridTipoLogradouro"); // make sure you have a target HTML element with this id
            gridTipoLogradouro.startup();
            gridTipoLogradouro.pagination.plugin._paginator.plugin.connect(gridTipoLogradouro.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridTipoLogradouro, 'cd_tipo_logradouro', 'selecionaTodos'); });
            gridTipoLogradouro.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoLogradouro.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    getLimpar('#formTipoLogradouro');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_TIPO_LOGRADOURO, null, gridTipoLogradouro, false);
                    dijit.byId("formularioTipoLogradouro").show();
                    IncluirAlterar(0, 'divAlterarTipoLogradouro', 'divIncluirTipoLogradouro', 'divExcluirTipoLogradouro', 'apresentadorMensagemTipoLogradouro', 'divCancelarTipoLogradouro', 'divLimparTipoLogradouro');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarTipoLogradouro', 'divIncluirTipoLogradouro', 'divExcluirTipoLogradouro', 'apresentadorMensagemTipoLogradouro', 'divCancelarTipoLogradouro', 'divLimparTipoLogradouro');
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoLogradouro(); } }, "incluirTipoLogradouro");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoLogradouro(); } }, "alterarTipoLogradouro");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            DeletarTipoLogradouro();
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    });
                }
            }, "deleteTipoLogradouro");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparTipoLogradouro();
                }
            }, "limparTipoLogradouro");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_TIPO_LOGRADOURO, null, gridTipoLogradouro, null);
                }
            }, "cancelarTipoLogradouro");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioTipoLogradouro").hide();
                }
            }, "fecharTipoLogradouro");

            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoLogradouro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTipoLogradouro').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoTipoLogradouro', -1, 'selecionaTodosTipoLogradouro', 'selecionaTodosTipoLogradouro', 'gridTipoLogradouro')", gridTipoLogradouro.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarTipoLogradouro(gridTipoLogradouro.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTipoLogradouro.itensSelecionados, 'DeletarTipoLogradouro(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoLogradouro",
                dropDown: menu,
                id: "acoesRelacionadasTipoLogradouro"
            });
            dom.byId("linkAcoesTipoLogradouro").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoLogradouro, 'todosItensTipoLogradouro', ['pesquisarTipoLogradouro', 'relatorioTipoLogradouro']); PesquisarTipoLogradouro(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoLogradouro', 'selecionadoTipoLogradouro', 'cd_localidade', 'selecionaTodosTipoLogradouro', ['pesquisarTipoLogradouro', 'relatorioTipoLogradouro'], 'todosItensTipoLogradouro'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoLogradouro",
                dropDown: menu,
                id: "todosItensTipoLogradouro"
            });
            dom.byId("linkSelecionadosTipoLogradouro").appendChild(button.domNode);
            ///////////////////////////////////////
            adicionarAtalhoPesquisa(['pesquisaTipoLogradouro'], 'pesquisarTipoLogradouro', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarTipoTelefone() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoTelefonesearch?descricao=&inicio=false",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_telefone"
                }), Memory({ idProperty: "cd_tipo_telefone" })
           );

            var gridTipoTelefone = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTipoTelefone' style='display:none'/>", field: "selecionadoTipoTelefone", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoTelefone },
                   // { name: "Código", field: "cd_tipo_telefone", width: "5%" },
                	  { name: "Tipo de Contato", field: "no_tipo_telefone", width: "95%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridTipoTelefone"); // make sure you have a target HTML element with this id
            gridTipoTelefone.startup();
            gridTipoTelefone.pagination.plugin._paginator.plugin.connect(gridTipoTelefone.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridTipoTelefone, 'cd_tipo_telefone', 'selecionaTodos'); });
            gridTipoTelefone.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoTelefone.on("RowDblClick", function (evt) {
                try{
                    getLimpar('#formTipoTelefone');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_TIPO_TELEFONE, null, gridTipoTelefone, false);
                    dijit.byId("formularioTipoTelefone").show();
                    IncluirAlterar(0, 'divAlterarTipoTelefone', 'divIncluirTipoTelefone', 'divExcluirTipoTelefone', 'apresentadorMensagemTipoTelefone', 'divCancelarTipoTelefone', 'divLimparTipoTelefone');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarTipoTelefone', 'divIncluirTipoTelefone', 'divExcluirTipoTelefone', 'apresentadorMensagemTipoTelefone', 'divCancelarTipoTelefone', 'divLimparTipoTelefone');
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoTelefone(); } }, "incluirTipoTelefone");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosTipoTelefone(); } }, "alterarTipoTelefone");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarTipoTelefone() }); } }, "deleteTipoTelefone");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparTipoTelefone();
                }
            }, "limparTipoTelefone");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_TIPO_TELEFONE, null, gridTipoTelefone, null);
                }
            }, "cancelarTipoTelefone");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioTipoTelefone").hide();
                }
            }, "fecharTipoTelefone");


            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoTelefone, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTipoTelefone').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoTipoTelefone', -1, 'selecionaTodosTipoTelefone', 'selecionaTodosTipoTelefone', 'gridTipoTelefone')", gridTipoTelefone.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarTipoTelefone(gridTipoTelefone.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemover(gridTipoTelefone.itensSelecionados, 'DeletarTipoTelefone(itensSelecionados)'); }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoTelefone",
                dropDown: menu,
                id: "acoesRelacionadasTipoTelefone"
            });
            dom.byId("linkAcoesTipoTelefone").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoTelefone, 'todosItensTipoTelefone', ['pesquisarTipoTelefone', 'relatorioTipoTelefone']); PesquisarTipoTelefone(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoTelefone', 'selecionadoTipoTelefone', 'cd_localidade', 'selecionaTodosTipoTelefone', ['pesquisarTipoTelefone', 'relatorioTipoTelefone'], 'todosItensTipoTelefone'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoTelefone",
                dropDown: menu,
                id: "todosItensTipoTelefone"
            });
            dom.byId("linkSelecionadosTipoTelefone").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaTipoTelefone'], 'pesquisarTipoTelefone', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirTabEstado() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirEstado').className))
            montarEstado();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323028', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabCidade() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirCidade').className))
            montarCidade();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323029', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabBairro() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirBairro').className))
            montarBairro();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323030', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabDistrito() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirDistrito').className))
            montarDistrito();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323031', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabLogradouro() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        $("#nrCEP").mask("99999-999");
        if (!hasValue(document.getElementById('incluirLogradouro').className))
            montarLogradouro();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323032', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabTipoEndereco() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTipoEndereco').className))
            montarTipoEndereco();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323033', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabTipoLogradouro() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTipoLogradouro').className))
            montarTipoLogradouro();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323034', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabClasseTelefone() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirClasseTelefone').className))
            montarClasseTelefone();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323035', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabTipoTelefone() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTipoTelefone').className))
            montarTipoTelefone();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323036', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function abrirTabOperadora() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirOperadora').className))
            montarOperadora();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323037', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function montarOperadora() {
    require([
         "dojo/dom",
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
         "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");

            montarStatus("statusOperadora");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getOperadorasearch?descricao=&inicio=false&status=1",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_operadora"
                }), Memory({ idProperty: "cd_operadora" })
           );

            var gridOperadora = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosOperadora' style='display:none'/>", field: "selecionadoOperadora", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxOperadora },
                  //  { name: "Código", field: "cd_operadora", width: "5%" },
                	{ name: "Operadora", field: "no_operadora", width: "65%" },
                    { name: "Ativa", field: "operadora_ativa", width: "30%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridOperadora"); // make sure you have a target HTML element with this id
            gridOperadora.startup();
            gridOperadora.pagination.plugin._paginator.plugin.connect(gridOperadora.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridOperadora, 'cd_operadora', 'selecionaTodos'); });
            gridOperadora.canSort = function (col) { return Math.abs(col) != 1; };
            gridOperadora.on("RowDblClick", function (evt) {
                try{
                    getLimpar('#formOperadora');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_OPERADORA, null, gridOperadora, false);
                    dijit.byId("formularioOperadora").show();
                    IncluirAlterar(0, 'divAlterarOperadora', 'divIncluirOperadora', 'divExcluirOperadora', 'apresentadorMensagemOperadora', 'divCancelarOperadora', 'divLimparOperadora');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarOperadora', 'divIncluirOperadora', 'divExcluirOperadora', 'apresentadorMensagemOperadora', 'divCancelarOperadora', 'divLimparOperadora');
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosOperadora(); } }, "incluirOperadora");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosOperadora(); } }, "alterarOperadora");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarOperadora() }); } }, "deleteOperadora");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparOperadora();
                }
            }, "limparOperadora");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_OPERADORA, null, gridOperadora, null);
                }
            }, "cancelarOperadora");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioOperadora").hide();
                }
            }, "fecharOperadora");


            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridOperadora, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosOperadora').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionadoOperadora', -1, 'selecionaTodosOperadora', 'selecionaTodosOperadora', 'gridOperadora')", gridOperadora.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarOperadora(gridOperadora.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemover(gridOperadora.itensSelecionados, 'DeletarOperadora(itensSelecionados)'); }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasOperadora",
                dropDown: menu,
                id: "acoesRelacionadasOperadora"
            });
            dom.byId("linkAcoesOperadora").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridOperadora, 'todosItensOperadora', ['pesquisarOperadora', 'relatorioOperadora']); PesquisarOperadora(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridOperadora', 'selecionadoOperadora', 'cd_localidade', 'selecionaTodosOperadora', ['pesquisarOperadora', 'relatorioOperadora'], 'todosItensOperadora'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensOperadora",
                dropDown: menu,
                id: "todosItensOperadora"
            });
            dom.byId("linkSelecionadosOperadora").appendChild(button.domNode);
            adicionarAtalhoPesquisa(['pesquisaOperadora', 'statusOperadora'], 'pesquisarOperadora', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function montarLogradouro() {
    require([
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/on",
        "dojox/json/ref"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem,on,ref) {
        try{
            var myStoreLogradouro = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/GetLogradouroSearch?descricao=&inicio=false&cd_estado=0&cd_cidade=0&cd_bairro=0&cep=",
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            var gridLogradouro = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStoreLogradouro }),
                structure: [
                    { name: "<input id='selecionaTodosLogradouro' style='display:none'/>", field: "logradouroSelecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxLogradouro },
                      { name: "CEP", field: "dc_num_cep", width: "12%" },
                      { name: "Logradouro", field: "no_localidade", width: "32%" },
                  	  { name: "Bairro", field: "no_localidade_bairro", width: "25%" },
                  	  { name: "Cidade", field: "no_localidade_cidade", width: "26%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridLogradouro"); // make sure you have a target HTML element with this id
            gridLogradouro.startup();
            gridLogradouro.pagination.plugin._paginator.plugin.connect(gridLogradouro.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridLogradouro, 'cd_localidade', 'logradouroSelecionado'); });
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridLogradouro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_localidade', 'logradouroSelecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridLogradouro')", gridLogradouro.rowsPerPage * 3);
                });
            });
            gridLogradouro.canSort = function (col) { return Math.abs(col) != 1; };
            gridLogradouro.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    limparCadLogradouro
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValuesLogradouro(null, gridLogradouro, false);
                    dijit.byId("formularioLogradouro").show();
                    IncluirAlterar(0, 'divAlterarLogradouro', 'divIncluirLogradouro', 'divExcluirLogradouro', 'apresentadorMensagemLogradouro', 'divCancelarLogradouro', 'divLimparLogradouro');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarLogradouro', 'divIncluirLogradouro', 'divExcluirLogradouro', 'apresentadorMensagemLogradouro', 'divCancelarLogradouro', 'divLimparLogradouro');

            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { salvarLogradouro(); } }, "incluirLogradouro");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { alterarLogradouro(); } }, "alterarLogradouro");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            deletarLogradouros(gridLogradouro.itensSelecionados);
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    });
                }
            }, "deleteLogradouro");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    apresentaMensagem('apresentadorMensagemLogradouro', null);
                    limparCadLogradouro();
                }
            }, "limparLogradouro");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                    apresentaMensagem('apresentadorMensagemLogradouro', null);
                keepValuesLogradouro(null, gridLogradouro, null);
                }
            }, "cancelarLogradouro");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioLogradouro").hide();
                }
            }, "fecharLogradouro");

            //////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridLogradouro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosLogradouro').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_loc_logradouro', 'logradouroSelecionado', -1, 'selecionaTodosLogradouro', 'selecionaTodosLogradouro', 'gridLogradouro')",
                                    gridLogradouro.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarLogradouro(gridLogradouro.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemoverLogradouros(gridLogradouro.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    } 
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasLogradouro",
                dropDown: menu,
                id: "acoesRelacionadasLogradouro"
            });
            dom.byId("linkAcoesLogradouro").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridLogradouro, 'todosItensLogradouro', ['pesquisarLogradouro', 'relatorioLogradouro']); pesquisarLogradouro(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridLogradouro', 'logradouroSelecionado', 'cd_localidade', 'selecionaTodosLogradouro', ['pesquisarLogradouro', 'relatorioLogradouro'], 'todosItensLogradouro'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensLogradouroo",
                dropDown: menu,
                id: "todosItensLogradouro"
            });
            dom.byId("linkSelecionadosLogradouro").appendChild(button.domNode);
            ///////////////////////////////////////
            dijit.byId("pesCidadeLogradouro").on("click", function (e) {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaCidade")))
                        montargridPesquisaCidade(function () {
                            abrirPesquisaCidadeFKLocalidades();
                            dijit.byId("pesquisar").on("click", function (e) {
                                pesquisaCidadeFK();
                            });
                            dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADELOGRADOURO;
                        });
                    else {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.byId("tipoPesquisaCidade").value = PESQUISACIDADELOGRADOURO;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("cadCidadeLogradouro").on("click", function (e) {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaCidade")))
                        montargridPesquisaCidade(function () {
                            abrirPesquisaCidadeFKLocalidades();
                            dijit.byId("pesquisar").on("click", function (e) {
                                pesquisaCidadeFK();
                            });
                            dojo.byId("tipoPesquisaCidade").value = CADPESQUISACIDADELOGRADOURO;
                        });
                    else {
                        abrirPesquisaCidadeFKLocalidades();
                        dojo.byId("tipoPesquisaCidade").value = CADPESQUISACIDADELOGRADOURO;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("limparCidadeLogradouroPes").on("click", function (e) {
                try{
                    dojo.byId('cd_cidade_pesq_logradouro').value = 0;
                    dojo.byId("pesNomCidadeLogradouro").value = "";
                    dijit.byId('limparCidadeLogradouroPes').set("disabled", true);
                    dijit.byId('bairroLogradouro').reset();
                    dijit.byId('bairroLogradouro').set("disabled", true);
                } catch (e) {
                    postGerarLog(e);
                }
            });
            $("#cep").mask("99999-999");
            adicionarAtalhoPesquisa(['descricaoLogradouro', "nrCEP"], 'pesquisarLogradouro', ready);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
///////////Atividade
function abrirTabAtividade() {
    apresentaMensagem('apresentadorMensagem', null);
    require([
         "dojo/dom",
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
         "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            query("body").addClass("claro");

            montarStatus("statusAtividade");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getAtividadesearch?descricao=&inicio=false&status=1&natureza=0&cnae=",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_atividade"
                }), Memory({ idProperty: "cd_atividade" })
           );

            var gridAtividade = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosAtividade' style='display:none'/>", field: "selecionadoAtividade", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividade },
                    { name: "Código", field: "cd_atividade", width: "10%" },
                    { name: "Atividade", field: "no_atividade", width: "50%" },
                	{ name: "Natureza", field: "natureza_atividade", width: "10%" },
                    { name: "CNAE", field: "cd_cnae_atividade", width: "15%" },
                    { name: "Ativa", field: "atividade_ativa", width: "10%" }
                ],
                canSort: true,
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
                        position: "button"
                    }
                }
            }, "gridAtividade"); // make sure you have a target HTML element with this id
            gridAtividade.startup();
            gridAtividade.pagination.plugin._paginator.plugin.connect(gridAtividade.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridAtividade, 'cd_atividade', 'selecionaTodosAtividade'); });
            gridAtividade.canSort = function (col) { return Math.abs(col) != 1; };
            gridAtividade.on("RowDblClick", function (evt) {
                try{
                    getLimpar('#formAtividade');
                    apresentaMensagem('apresentadorMensagem', '');
                    var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                    keepValues(FORM_ATIVIDADE, null, gridAtividade, false);
                    dijit.byId("formularioAtividade").show();
                    IncluirAlterar(0, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
            query("#pesquisaAtividade").on("keyup", function (e) { if (e.keyCode == 13) PesquisarAtividade(true); });
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosAtividade(); } }, "incluirAtividade");
            new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosAtividade(); } }, "alterarAtividade");
            new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarAtividade() }); } }, "deleteAtividade");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                    limparAtividade();
                }
            }, "limparAtividade");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                keepValues(FORM_ATIVIDADE, null, gridAtividade, null);
                }
            }, "cancelarAtividade");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                    dijit.byId("formularioAtividade").hide();
                }
            }, "fecharAtividade");

            loadNatureza();
            ////////////////////////////
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridOperadora, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosAtividade').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_atividade', 'selecionadoAtividade', -1, 'selecionaTodosAtividade', 'selecionaTodosAtividade', 'gridAtividade')", gridAtividade.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarAtividade(gridAtividade.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemover(gridAtividade.itensSelecionados, 'DeletarAtividade(itensSelecionados)'); }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasAtividade",
                dropDown: menu,
                id: "acoesRelacionadasAtividade"
            });
            dom.byId("linkAcoesAtividade").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridAtividade, 'todosItensAtividade', ['pesquisarAtividade', 'relatorioAtividade']); PesquisarAtividade(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridAtividade', 'selecionadoAtividade', 'cd_atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensAtividade",
                dropDown: menu,
                id: "todosItensAtividade"
            });
            dom.byId("linkSelecionadosAtividade").appendChild(button.domNode);
            btnPesquisar(document.getElementById("pesquisarAtividade"));
            ///////////////////////////////////////
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabPais')
            abrirTabPais();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabEstado')
            abrirTabEstado();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabCidade')
            abrirTabCidade();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabBairro')
            abrirTabBairro();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabDistrito')
            abrirTabDistrito();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoEndereco')
            abrirTabTipoEndereco();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabClasseTelefone')
            abrirTabClasseTelefone();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoLogradouro')
            abrirTabTipoLogradouro();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoTelefone')
            abrirTabTipoTelefone();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabOperadora')
            abrirTabOperadora();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAtividade')
            abrirTabAtividade();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabLogradouro')
            abrirTabLogradouro();
    } catch (e) {
        postGerarLog(e);
    }
}
function montaAuxiliarPessoa() {
    require([
        "dojo/dom",
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
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                decreaseBtn(document.getElementById("pesCidadeLogradouro"), '18px');
                decreaseBtn(document.getElementById("cadCidadeLogradouro"), '18px');
                decreaseBtn(document.getElementById("buscaCep"), '18px');
                decreaseBtn(document.getElementById("limparCidadeLogradouroPes"), '40px');
                decreaseBtn(document.getElementById("pesCidadeDistrito"), '18px');
                decreaseBtn(document.getElementById("limparCidadeDistritoPes"), '40px');
                decreaseBtn(document.getElementById("cadCidadeDistrito"), '18px');
                decreaseBtn(document.getElementById("limparCadCidadeDistrito"), '40px');
                decreaseBtn(document.getElementById("pesCidadeBairro"), '18px');
                decreaseBtn(document.getElementById("limparCidadeBairro"), '40px');
                decreaseBtn(document.getElementById("cadCidadeBairro"), '18px');
                query("body").addClass("claro");
                dijit.byId("tabContainer").resize();
                if (hasValue(dijit.byId("menuManual"))) {
                    var menuManual = dijit.byId("menuManual");
                    if (hasValue(menuManual.handler))
                        menuManual.handler.remove();
                    menuManual.handler = menuManual.on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323027', '765px', '771px');
                        });
                }
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/localidade/getpaissearch?descricao=&inicio=false",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_localidade"
                    }), Memory({ idProperty: "cd_localidade" })
               );

                var gridPais = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                       // { name: "Código", field: "cd_localidade", width: "5%" },
                        { name: "Número", field: "dc_num_pais", width: "10%" },
                    	{ name: "País", field: "dc_pais", width: "35%" },
                        { name: "Sigla", field: "sg_pais", width: "10%" },
                        { name: "Nacionalidade (Masculino)", field: "dc_nacionalidade_masc", width: "20%" },
                        { name: "Nacionalidade (Feminino)", field: "dc_nacionalidade_fem", width: "20%" }
                    ],
                    canSort: true,
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
                            position: "button"
                        }
                    }
                }, "gridPais"); // make sure you have a target HTML element with this id
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:

                gridPais.startup();
                gridPais.pagination.plugin._paginator.plugin.connect(gridPais.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridPais, 'cd_localidade_pais', 'selecionaTodos'); });
                gridPais.canSort = function (col) { return Math.abs(col) != 1; };
                gridPais.on("RowDblClick", function (evt) {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        getLimpar('#formPais');
                        apresentaMensagem('apresentadorMensagem', '');
                        var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                        keepValues(FORM_PAIS, null, gridPais, false);
                        dijit.byId("formularioPais").show();
                        IncluirAlterar(0, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                IncluirAlterar(1, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosPais(); } }, "incluirPais");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosPais(); } }, "alterarPais");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            try {
                                if (!eval(MasterGeral())) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                    return;
                                }
                                DeletarPais();
                            } catch (e) {
                                postGerarLog(e);
                            } 
                        });
                    }
                }, "deletePais");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparPais();
                    }}, "limparPais");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                    keepValues(FORM_PAIS, null, gridPais, null);
                    }
                }, "cancelarPais");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        dijit.byId("formularioPais").hide();
                    }
                }, "fecharPais");

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPais, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_localidade', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridPais')", gridPais.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoEditar(gridPais.itensSelecionados);
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoRemover(gridPais.itensSelecionados, 'DeletarPais(itensSelecionados)');
                        } catch (e) {
                            postGerarLog(e);
                        } 
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridPais, 'todosItens', ['pesquisarPais', 'relatorioPais']);
                        PesquisarPais(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridPais', 'selecionado', 'cd_localidade', 'selecionaTodos', ['pesquisarPais', 'relatorioPais'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                montaBotoesAuxiliarPessoa();

                dijit.byId("tabContainer_tablist_menuBtn").on("click", function () {
                    //dijit.byId("tabContainer_menu").on("_create", function () {
                    if (hasValue(dijit.byId("tabContainer_menu")) && dijit.byId("tabContainer_menu")._created) {
                        //alert("doido");
                        dijit.byId("tabPais_stcMi").on("click", function () {
                            abrirTabPais();
                        });
                        dijit.byId("tabEstado_stcMi").on("click", function () {
                            abrirTabEstado();
                        });
                        dijit.byId("tabCidade_stcMi").on("click", function () {
                            abrirTabCidade();
                        });
                        dijit.byId("tabBairro_stcMi").on("click", function () {
                            abrirTabBairro();
                        });
                        dijit.byId("tabDistrito_stcMi").on("click", function () {
                            abrirTabDistrito();
                        });
                        dijit.byId("tabTipoEndereco_stcMi").on("click", function () {
                            abrirTabTipoEndereco();
                        });
                        dijit.byId("tabClasseTelefone_stcMi").on("click", function () {
                            abrirTabClasseTelefone();
                        });
                        dijit.byId("tabTipoLogradouro_stcMi").on("click", function () {
                            abrirTabTipoLogradouro();
                        });
                        dijit.byId("tabTipoTelefone_stcMi").on("click", function () {
                            abrirTabTipoTelefone();
                        });
                        dijit.byId("tabOperadora_stcMi").on("click", function () {
                            abrirTabOperadora();
                        });
                        dijit.byId("tabAtividade_stcMi").on("click", function () {
                            abrirTabAtividade();
                        });
                    }
                });
                adicionarAtalhoPesquisa(['pesquisaPais'], 'pesquisarPais', ready);
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}
function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formPais');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_PAIS, null, dijit.byId('gridPais'), true);
            dijit.byId("formularioPais").show();
            IncluirAlterar(0, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarEstado(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formEstado');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ESTADO, null, dijit.byId('gridEstado'), true);
            dijit.byId("formularioEstado").show();
            IncluirAlterar(0, 'divAlterarEstado', 'divIncluirEstado', 'divExcluirEstado', 'apresentadorMensagemEstado', 'divCancelarEstado', 'divLimparEstado');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarCidade(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formCidade');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_CIDADE, null, dijit.byId('gridCidade'), true);
            dijit.byId("formularioCidade").show();
            IncluirAlterar(0, 'divAlterarCidade', 'divIncluirCidade', 'divExcluirCidade', 'apresentadorMensagemCidade', 'divCancelarCidade', 'divLimparCidade');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarBairro(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formBairro');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_BAIRRO, null, dijit.byId('gridBairro'), true);
            dijit.byId("formularioBairro").show();
            IncluirAlterar(0, 'divAlterarBairro', 'divIncluirBairro', 'divExcluirBairro', 'apresentadorMensagemBairro', 'divCancelarBairro', 'divLimparBairro');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarDistrito(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formDistrito');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_DISTRITO, null, dijit.byId('gridDistrito'), true);
            dijit.byId("formularioDistrito").show();
            IncluirAlterar(0, 'divAlterarDistrito', 'divIncluirDistrito', 'divExcluirDistrito', 'apresentadorMensagemDistrito', 'divCancelarDistrito', 'divLimparDistrito');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarTipoEndereco(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formTipoEndereco');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_TIPO_ENDERECO, null, dijit.byId('gridTipoEndereco'), true);
            dijit.byId("formularioTipoEndereco").show();
            IncluirAlterar(0, 'divAlterarTipoEndereco', 'divIncluirTipoEndereco', 'divExcluirTipoEndereco', 'apresentadorMensagemTipoEndereco', 'divCancelarTipoEndereco', 'divLimparTipoEndereco');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarClasseTelefone(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formClasseTelefone');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_CLASSE_TELEFONE, null, dijit.byId('gridClasseTelefone'), true);
            dijit.byId("formularioClasseTelefone").show();
            IncluirAlterar(0, 'divAlterarClasseTelefone', 'divIncluirClasseTelefone', 'divExcluirClasseTelefone', 'apresentadorMensagemClasseTelefone', 'divCancelarClasseTelefone', 'divLimparClasseTelefone');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarTipoTelefone(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formTipoTelefone');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_TIPO_TELEFONE, null, dijit.byId('gridTipoTelefone'), true);
            dijit.byId("formularioTipoTelefone").show();
            IncluirAlterar(0, 'divAlterarTipoTelefone', 'divIncluirTipoTelefone', 'divExcluirTipoTelefone', 'apresentadorMensagemTipoTelefone', 'divCancelarTipoTelefone', 'divLimparTipoTelefone');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarTipoLogradouro(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formTipoLogradouro');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_TIPO_LOGRADOURO, null, dijit.byId('gridTipoLogradouro'), true);
            dijit.byId("formularioTipoLogradouro").show();
            IncluirAlterar(0, 'divAlterarTipoLogradouro', 'divIncluirTipoLogradouro', 'divExcluirTipoLogradouro', 'apresentadorMensagemTipoLogradouro', 'divCancelarTipoLogradouro', 'divLimparTipoLogradouro');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarOperadora(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formOperadora');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_OPERADORA, null, dijit.byId('gridOperadora'), true);
            dijit.byId("formularioOperadora").show();
            IncluirAlterar(0, 'divAlterarOperadora', 'divIncluirOperadora', 'divExcluirOperadora', 'apresentadorMensagemOperadora', 'divCancelarOperadora', 'divLimparOperadora');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function eventoEditarAtividade(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formAtividade');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_ATIVIDADE, null, dijit.byId('gridAtividade'), true);
            dijit.byId("formularioAtividade").show();
            IncluirAlterar(0, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function montaBotoesAuxiliarPessoa() {
    require([
          "dojo/_base/xhr",
          "dijit/form/Button"
    ], function (xhr, Button) {
        try{
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("formularioPais").show();
                        getLimpar('#formPais');
                        clearForm("formPais");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoPais");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarPais(true); }
            }, "pesquisarPais");
            decreaseBtn(document.getElementById("pesquisarPais"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaPais").value) ? Endereco() + "/api/localidade/geturlrelatorioPais?" + getStrGridParameters('gridPais') + "descricao=&inicio=" + document.getElementById("inicioDescPais").checked : Endereco() + "/api/localidade/geturlrelatorioPais?" + getStrGridParameters('gridPais') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaPais").value) + "&inicio=" + document.getElementById("inicioDescPais").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '925px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioPais");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        loadPais();
                        dijit.byId("formularioEstado").show();
                        getLimpar('#formEstado');
                        clearForm("formEstado");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarEstado', 'divIncluirEstado', 'divExcluirEstado', 'apresentadorMensagemEstado', 'divCancelarEstado', 'divLimparEstado');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoEstado");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarEstado(true); }
            }, "pesquisarEstado");
            decreaseBtn(document.getElementById("pesquisarEstado"), '32px');


            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaEstado").value) ? Endereco() + "/api/localidade/geturlrelatorioEstado?" + getStrGridParameters('gridEstado') + "descricao=&inicio=" + document.getElementById("inicioDescEstado").checked + "&cdPais=" + dijit.byId("pesquisapais").value : Endereco() + "/api/localidade/geturlrelatorioEstado?" + getStrGridParameters('gridEstado') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaEstado").value) + "&inicio=" + document.getElementById("inicioDescEstado").checked + "&cdPais=" + dijit.byId("pesquisapais").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioEstado");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        loadEstado();
                        dijit.byId("formularioCidade").show();
                        getLimpar('#formCidade');
                        clearForm("formCidade");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarCidade', 'divIncluirCidade', 'divExcluirCidade', 'apresentadorMensagemCidade', 'divCancelarCidade', 'divLimparCidade');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoCidade");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarCidade(true); }
            }, "pesquisarCidade");
            decreaseBtn(document.getElementById("pesquisarCidade"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    try {
                        var municipio = hasValue(dojo.byId("pesmunicipio").value) ? parseInt((dojo.byId("pesmunicipio").value).replace(".", "")) : 0;
                        var pesEst = hasValue(dijit.byId("pesquisaEst").value) ? dijit.byId("pesquisaEst").value : 0;
                        xhr.get({
                            url: Endereco() + "/api/localidade/geturlrelatorioCidade?" + getStrGridParameters('gridCidade') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaCidade").value) + "&inicio=" + document.getElementById("inicioDescCidade").checked + "&nmMunicipio=" + municipio + "&cdEstado=" + pesEst,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "relatorioCidade");
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("formularioBairro").show();
                        getLimpar('#formBairro');
                        clearForm("formBairro");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarBairro', 'divIncluirBairro', 'divExcluirBairro', 'apresentadorMensagemBairro', 'divCancelarBairro', 'divLimparBairro');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoBairro");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarBairro(true); }
            }, "pesquisarBairro");
            decreaseBtn(document.getElementById("pesquisarBairro"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/localidade/geturlrelatorioBairro?" + getStrGridParameters('gridBairro') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaBairro").value) + "&inicio=" + document.getElementById("inicioDescBairro").checked + "&cd_cidade=" + dojo.byId("cd_cidade_pesq_bairro").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioBairro");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("formularioDistrito").show();
                        getLimpar('#formDistrito');
                        clearForm("formDistrito");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarDistrito', 'divIncluirDistrito', 'divExcluirDistrito', 'apresentadorMensagemDistrito', 'divCancelarDistrito', 'divLimparDistrito');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoDistrito");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarDistrito(true); }
            }, "pesquisarDistrito");
            decreaseBtn(document.getElementById("pesquisarDistrito"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/localidade/geturlrelatorioDistrito?" + getStrGridParameters('gridDistrito') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaDistrito").value) + "&inicio=" + document.getElementById("inicioDescDistrito").checked + "&cd_cidade=" + dojo.byId("cd_cidade_pesq_distrito").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioDistrito");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("formularioTipoEndereco").show();
                        getLimpar('#formTipoEndereco');
                        clearForm("formTipoEndereco");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoEndereco', 'divIncluirTipoEndereco', 'divExcluirTipoEndereco', 'apresentadorMensagemTipoEndereco', 'divCancelarTipoEndereco', 'divLimparTipoEndereco');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoEndereco");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarTipoEndereco(true); }
            }, "pesquisarTipoEndereco");
            decreaseBtn(document.getElementById("pesquisarTipoEndereco"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTipoEndereco").value) ? Endereco() + "/api/localidade/geturlrelatorioTipoEndereco?" + getStrGridParameters('gridTipoEndereco') + "descricao=&inicio=" + document.getElementById("inicioDescTipoEndereco").checked : Endereco() + "/api/localidade/geturlrelatorioTipoEndereco?" + getStrGridParameters('gridTipoEndereco') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoEndereco").value) + "&inicio=" + document.getElementById("inicioDescTipoEndereco").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioTipoEndereco");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dijit.byId("formularioClasseTelefone").show();
                        getLimpar('#formClasseTelefone');
                        clearForm("formClasseTelefone");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarClasseTelefone', 'divIncluirClasseTelefone', 'divExcluirClasseTelefone', 'apresentadorMensagemClasseTelefone', 'divCancelarClasseTelefone', 'divLimparClasseTelefone');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoClasseTelefone");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarClasseTelefone(true); }
            }, "pesquisarClasseTelefone");
            decreaseBtn(document.getElementById("pesquisarClasseTelefone"), '32px');
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaClasseTelefone").value) ? Endereco() + "/api/localidade/geturlrelatorioClasseTelefone?" + getStrGridParameters('gridClasseTelefone') + "descricao=&inicio=" + document.getElementById("inicioDescClasseTelefone").checked : Endereco() + "/api/localidade/geturlrelatorioClasseTelefone?" + getStrGridParameters('gridClasseTelefone') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaClasseTelefone").value) + "&inicio=" + document.getElementById("inicioDescClasseTelefone").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioClasseTelefone");


            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("formularioTipoLogradouro").show();
                        getLimpar('#formTipoLogradouro');
                        clearForm("formTipoLogradouro");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoLogradouro', 'divIncluirTipoLogradouro', 'divExcluirTipoLogradouro', 'apresentadorMensagemTipoLogradouro', 'divCancelarTipoLogradouro', 'divLimparTipoLogradouro');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoLogradouro");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarTipoLogradouro(true); }
            }, "pesquisarTipoLogradouro");
            decreaseBtn(document.getElementById("pesquisarTipoLogradouro"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTipoLogradouro").value) ? Endereco() + "/api/localidade/geturlrelatorioTipoLogradouro?" + getStrGridParameters('gridTipoLogradouro') + "descricao=&inicio=" + document.getElementById("inicioDescTipoLogradouro").checked : Endereco() + "/api/localidade/geturlrelatorioTipoLogradouro?" + getStrGridParameters('gridTipoLogradouro') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoLogradouro").value) + "&inicio=" + document.getElementById("inicioDescTipoLogradouro").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioTipoLogradouro");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dijit.byId("formularioTipoTelefone").show();
                        getLimpar('#formTipoTelefone');
                        clearForm("formTipoTelefone");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoTelefone', 'divIncluirTipoTelefone', 'divExcluirTipoTelefone', 'apresentadorMensagemTipoTelefone', 'divCancelarTipoTelefone', 'divLimparTipoTelefone');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoTelefone");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarTipoTelefone(true); }
            }, "pesquisarTipoTelefone");
            decreaseBtn(document.getElementById("pesquisarTipoTelefone"), '32px');
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTipoTelefone").value) ? Endereco() + "/api/localidade/geturlrelatorioTipoTelefone?" + getStrGridParameters('gridTipoTelefone') + "descricao=&inicio=" + document.getElementById("inicioDescTipoTelefone").checked : Endereco() + "/api/localidade/geturlrelatorioTipoTelefone?" + getStrGridParameters('gridTipoTelefone') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoTelefone").value) + "&inicio=" + document.getElementById("inicioDescTipoTelefone").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioTipoTelefone");

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dijit.byId("formularioOperadora").show();
                        getLimpar('#formOperadora');
                        clearForm("formOperadora");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarOperadora', 'divIncluirOperadora', 'divExcluirOperadora', 'apresentadorMensagemOperadora', 'divCancelarOperadora', 'divLimparOperadora');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoOperadora");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarOperadora(true); }
            }, "pesquisarOperadora");
            decreaseBtn(document.getElementById("pesquisarOperadora"), '32px');
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaOperadora").value) ? Endereco() + "/api/localidade/geturlrelatorioOperadora?" + getStrGridParameters('gridOperadora') + "descricao=&inicio=" + document.getElementById("inicioDescOperadora").checked + "&status=" + retornaStatus("statusOperadora") : Endereco() + "/api/localidade/GeturlrelatorioOperadora?" + getStrGridParameters('gridOperadora') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaOperadora").value) + "&inicio=" + document.getElementById("inicioDescOperadora").checked + "&status=" + retornaStatus("statusOperadora"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioOperadora");

            //

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dijit.byId("formularioAtividade").show();
                        getLimpar('#formAtividade');
                        clearForm("formAtividade");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoAtividade");

            new Button({
                label: "",
                iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarAtividade(true); }
            }, "pesquisarAtividade");
            btnPesquisar(document.getElementById("pesquisarAtividade"));
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/localidade/geturlrelatorioAtividade?" + getStrGridParameters('gridAtividade') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaAtividade").value) + "&inicio=" + document.getElementById("inicioDescAtividade").checked + "&status=" + retornaStatus("statusAtividade") + "&natureza=" + retornaStatus("natureza") + "&cnae=" + encodeURIComponent(document.getElementById("cnae").value),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioAtividade");
            //Criação dos botões principais da tela de lograouro.
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        limparCadLogradouro();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarLogradouro', 'divIncluirLogradouro', 'divExcluirLogradouro', 'apresentadorMensagemLogradouro', 'divCancelarLogradouro', 'divLimparLogradouro');
                        dijit.byId("formularioLogradouro").show();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoLogradouro");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    pesquisarLogradouro(true);
                }
            }, "pesquisarLogradouro");
            decreaseBtn(document.getElementById("pesquisarLogradouro"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    var cd_bairro = hasValue(dijit.byId("bairroLogradouro").value) ? dijit.byId("bairroLogradouro").value : 0;
                    xhr.get({
                        url: Endereco() + "/api/localidade/GeturlrelatorioLogradouro?" + getStrGridParameters('gridLogradouro') + "descricao=" +
                                            dojo.byId("descricaoLogradouro").value + "&inicio=" + document.getElementById("inicioDescLogradouro").checked + "&cd_estado=0&cd_cidade=" +
                                            dojo.byId("cd_cidade_pesq_logradouro").value + "&cd_bairro=" + cd_bairro + "&cep=" + dijit.byId("nrCEP").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                })
                }
            }, "relatorioLogradouro");
        } catch (e) {
            postGerarLog(e);
        }

    });
}

function abrirTabPais() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirPais').className))
            montaAuxiliarPessoa();
        if (hasValue(dijit.byId("menuManual"))) {
            var menuManual = dijit.byId("menuManual");
            if (hasValue(menuManual.handler))
                menuManual.handler.remove();
            menuManual.handler = menuManual.on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323027', '765px', '771px');
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function limparPais() {
    try{
        getLimpar('#formPais');
        clearForm('formPais');
        IncluirAlterar(1, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');
        document.getElementById("cd_localidade").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosPais() {
    try{
        if (document.getElementById("divAlterarPais").style.display == "")
            AlterarPais();
        else
            IncluirPais();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirPais() {
    try{
        if (!dijit.byId("formPais").validate())
            return false;
        apresentaMensagem('apresentadorMensagemPais', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postPais", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade").value,
                    cd_tipo_localidade: "1",
                    dc_num_pais: dom.byId("dc_num_pais").value,
                    dc_nacionalidade_masc: dom.byId("dc_nacionalidade_masc").value,
                    dc_pais: dom.byId("dc_pais").value,
                    dc_nacionalidade_fem: dom.byId("dc_nacionalidade_fem").value,
                    sg_pais: dom.byId("sg_pais").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridPais';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', itemAlterado);
                        dijit.byId("formularioPais").hide();
                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionado', 'cd_localidade', 'selecionaTodos', ['pesquisarPais', 'relatorioPais'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemPais', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPais', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarPais(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getpaissearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaPais").value) + "&inicio=" + document.getElementById("inicioDescPais").checked,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );
            if (hasValue(dijit.byId("menuManual"))) {
                dijit.byId("menuManual").on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323027', '765px', '771px');
                    });
            }

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridPais = dijit.byId("gridPais");
            if (limparItens)
                gridPais.itensSelecionados = [];
            gridPais.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarPais() {
    try{
        var gridName = 'gridPais';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formPais").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarpais",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade").value,
                    cd_tipo_localidade: "1",
                    dc_num_pais: dom.byId("dc_num_pais").value,
                    dc_nacionalidade_masc: dom.byId("dc_nacionalidade_masc").value,
                    dc_pais: dom.byId("dc_pais").value,
                    dc_nacionalidade_fem: dom.byId("dc_nacionalidade_fem").value,
                    sg_pais: dom.byId("sg_pais").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItens_label");
                    loadPesquisaPais();
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioPais").hide();
                    removeObjSort(grid.itensSelecionados, "cd_localidade", dom.byId("cd_localidade").value);
                    insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionado', 'cd_localidade', 'selecionaTodos', ['pesquisarPais', 'relatorioPais'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_localidade");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPais', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarPais(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_localidade').value != 0)
                    itensSelecionados = [{
                        cd_localidade: dom.byId("cd_localidade").value,
                        cd_tipo_localidade: "1",
                        dc_num_pais: dom.byId("dc_num_pais").value,
                        dc_nacionalidade_masc: dom.byId("dc_nacionalidade_masc").value,
                        dc_pais: dom.byId("dc_pais").value,
                        dc_nacionalidade_fem: dom.byId("dc_nacionalidade_fem").value,
                        sg_pais: dom.byId("sg_pais").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeletepais",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItens_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioPais").hide();
                        dijit.byId("pesquisaPais").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridPais').itensSelecionados, "cd_localidade", itensSelecionados[r].cd_localidade);

                        PesquisarPais(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarPais").set('disabled', false);
                        dijit.byId("relatorioPais").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemPais', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioPais").style.display))
                    apresentaMensagem('apresentadorMensagemPais', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function limparEstado() {
    try{
        getLimpar('#formEstado');
        clearForm('formEstado');
        IncluirAlterar(1, 'divAlterarEstado', 'divIncluirEstado', 'divExcluirEstado', 'apresentadorMensagemEstado', 'divCancelarEstado', 'divLimparEstado');
        document.getElementById("cd_localidade_estado").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosEstado() {
    try{
        if (document.getElementById("divAlterarEstado").style.display == "")
            AlterarEstado();
        else
            IncluirEstado();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirEstado() {
    try{
        if (!dijit.byId("formEstado").validate())
            return false;
        apresentaMensagem('apresentadorMensagemEstado', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postEstado", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_estado").value,
                    cd_tipo_localidade: "2",
                    no_localidade: dom.byId("no_localidade").value,
                    sg_estado: dom.byId("sg_estado").value,
                    cd_loc_relacionada: dijit.byId("cd_pais").value,
                    no_pais: dojo.byId("cd_pais").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridEstado';
                        var grid = dijit.byId(gridName);
                        loadPesquisaPais();
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioEstado").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_localidade_estado", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoEstado', 'cd_localidade_estado', 'selecionaTodosEstado', ['pesquisarEstado', 'relatorioEstado'], 'todosItensEstado');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade_estado");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemEstado', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemEstado', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarEstado(limparItens) {
    var cdPais = hasValue(dijit.byId("pesquisapais").value)? dijit.byId("pesquisapais").value : 0;
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getEstadosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaEstado").value) + "&inicio=" + document.getElementById("inicioDescEstado").checked + "&cdPais=" + cdPais,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridEstado = dijit.byId("gridEstado");
            if (limparItens)
                gridEstado.itensSelecionados = [];
            gridEstado.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
        });
}
function AlterarEstado() {
    try{
        var gridName = 'gridEstado';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formEstado").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarEstado",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_estado").value,
                    cd_tipo_localidade: "1",
                    no_localidade: dom.byId("no_localidade").value,
                    sg_estado: dom.byId("sg_estado").value,
                    cd_loc_relacionada: dijit.byId("cd_pais").value,
                    no_pais: dojo.byId("cd_pais").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItens_label");
                    loadPesquisaEstado();
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioEstado").hide();
                    removeObjSort(grid.itensSelecionados, "cd_localidade", dom.byId("cd_localidade_estado").value);
                    insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoEstado', 'cd_localidade', 'selecionaTodosEstado', ['pesquisarEstado', 'relatorioEstado'], 'todosItensEstado');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_localidade");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemEstado', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarEstado(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_localidade_estado').value != 0)
                    itensSelecionados = [{
                        cd_localidade: dom.byId("cd_localidade_estado").value,
                        cd_tipo_localidade: "1",
                        no_localidade: dom.byId("no_localidade").value,
                        sg_estado: dom.byId("sg_estado").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeleteestado",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensEstado_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioEstado").hide();
                        dijit.byId("pesquisaEstado").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridEstado').itensSelecionados, "cd_localidade_estado", itensSelecionados[r].cd_localidade);

                        PesquisarEstado(true);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarEstado").set('disabled', false);
                        dijit.byId("relatorioEstado").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemEstado', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioEstado").style.display))
                    apresentaMensagem('apresentadorMensagemEstado', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function limparCidade() {
    try{
        getLimpar('#formCidade');
        clearForm('formCidade');
        IncluirAlterar(1, 'divAlterarCidade', 'divIncluirCidade', 'divExcluirCidade', 'apresentadorMensagemCidade', 'divCancelarCidade', 'divLimparCidade');
        document.getElementById("cd_localidade_cidade").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosCidade() {
    try{
        if (document.getElementById("divAlterarCidade").style.display == "")
            AlterarCidade();
        else
            IncluirCidade();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirCidade() {
    try{
        if (!dijit.byId("formCidade").validate())
            return false;
        apresentaMensagem('apresentadorMensagemCidade', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postCidade", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_cidade").value,
                    cd_tipo_localidade: "3",
                    no_localidade: dom.byId("no_localidade_cidade").value,
                    cd_loc_relacionada: dijit.byId("cd_estado").value,
                    nm_municipio: parseInt((dojo.byId("nm_municipio").value).replace(".", "")),
                    sg_estado: dojo.byId('cd_estado').value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridCidade';
                        var grid = dijit.byId(gridName);
                        loadPesquisaEstado();
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioCidade").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoCidade', 'cd_localidade', 'selecionaTodos', ['pesquisarCidade', 'relatorioCidade'], 'todosItensCidade', 2);
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemCidade', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarCidade(limparItens) {
    require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var municipio = hasValue(dojo.byId("pesmunicipio").value) ? parseInt((dojo.byId("pesmunicipio").value).replace(".", "")) : 0;
            var pesEst = hasValue(dijit.byId("pesquisaEst").value) ? dijit.byId("pesquisaEst").value : 0;
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getCidadesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaCidade").value) + "&inicio=" + document.getElementById("inicioDescCidade").checked + "&nmMunicipio=" + municipio + "&cdEstado=" + pesEst,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridCidade = dijit.byId("gridCidade");
            if (limparItens)
                gridCidade.itensSelecionados = [];
            gridCidade.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarCidade() {
    try{
        var gridName = 'gridCidade';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formCidade").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postAlterarCidade",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_cidade").value,
                    cd_tipo_localidade: "3",
                    no_localidade: dom.byId("no_localidade_cidade").value,
                    cd_loc_relacionada: dijit.byId("cd_estado").value,
                    nm_municipio: parseInt((dojo.byId("nm_municipio").value).replace(".", "")),
                    sg_estado: dojo.byId('cd_estado').value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensCidade_label");

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("formularioCidade").hide();
                    removeObjSort(grid.itensSelecionados, "cd_localidade", dom.byId("cd_localidade_cidade").value);
                    insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCidade', 'cd_localidade', 'selecionaTodos', ['pesquisarCidade', 'relatorioCidade'], 'todosItensCidade');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_localidade");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarCidade(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_localidade_cidade').value != 0)
                    itensSelecionados = [{
                        cd_localidade: dom.byId("cd_localidade_cidade").value,
                        cd_tipo_localidade: "3",
                        no_localidade: dom.byId("no_localidade_cidade").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeletecidade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensCidade_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioCidade").hide();
                        dijit.byId("pesquisaCidade").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridCidade').itensSelecionados, "cd_localidade", itensSelecionados[r].cd_localidade);

                        PesquisarCidade(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarCidade").set('disabled', false);
                        dijit.byId("relatorioCidade").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemCidade', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioCidade").style.display))
                    apresentaMensagem('apresentadorMensagemCidade', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);

            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}

function limparBairro() {
    try{
        getLimpar('#formBairro');
        clearForm('formBairro');
        IncluirAlterar(1, 'divAlterarBairro', 'divIncluirBairro', 'divExcluirBairro', 'apresentadorMensagemBairro', 'divCancelarBairro', 'divLimparBairro');
        document.getElementById("cd_localidade_bairro").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosBairro() {
    try{
        if (document.getElementById("divAlterarBairro").style.display == "")
            AlterarBairro();
        else
            IncluirBairro();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirBairro() {
    try{
        if (!dijit.byId("formBairro").validate())
            return false;
        apresentaMensagem('apresentadorMensagemBairro', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/PostInsertBairro", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_bairro").value,
                    no_localidade: dom.byId("no_localidade_bairro").value,
                    cd_loc_relacionada: dom.byId("cd_cidade_cad_bairro").value,
                    no_localidade_cidade: dom.byId("cadNomCidadeBairro").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridBairro';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioBairro").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoBairro', 'cd_localidade', 'selecionaTodosBairro', ['pesquisarBairro', 'relatorioBairro'], 'todosItensBairro');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemBairro', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemBairro', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarBairro(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getBairrosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaBairro").value) + "&inicio=" +
                                        document.getElementById("inicioDescBairro").checked + "&cd_cidade=" + dojo.byId("cd_cidade_pesq_bairro").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridBairro = dijit.byId("gridBairro");

            if (limparItens)
                gridBairro.itensSelecionados = [];

            gridBairro.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarBairro() {
    try{
        var gridName = 'gridBairro';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formBairro").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/PostUpdateBairro",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_bairro").value,
                    no_localidade: dom.byId("no_localidade_bairro").value,
                    cd_loc_relacionada: dom.byId("cd_cidade_cad_bairro").value,
                    no_localidade_cidade: dom.byId("cadNomCidadeBairro").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioBairro").hide();
                        removeObjSort(grid.itensSelecionados, "cd_localidade", dom.byId("cd_localidade_bairro").value);
                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoBairro', 'cd_localidade', 'selecionaTodosBairro', ['pesquisarBairro', 'relatorioBairro'], 'todosItensBairro');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemBairro', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemBairro', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarBairro(itensSelecionados) {
    try{
        if (!dijit.byId("formBairro").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Bairro selecionado.");
            apresentaMensagem('apresentadorMensagemBairro', mensagensWeb);
        }

        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_localidade_bairro').value != 0)
                    itensSelecionados = [{
                        cd_localidade: dom.byId("cd_localidade_bairro").value,
                        cd_tipo_localidade: "4",
                        no_localidade: dom.byId("no_localidade_bairro").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeletebairro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensBairro_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioBairro").hide();
                        dijit.byId("pesquisaBairro").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridBairro').itensSelecionados, "cd_localidade", itensSelecionados[r].cd_localidade);

                        PesquisarBairro(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarBairro").set('disabled', false);
                        dijit.byId("relatorioBairro").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemBairro', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioBairro").style.display))
                apresentaMensagem('apresentadorMensagemBairro', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

//Metodos distritos
function limparDistrito() {
    try{
        getLimpar('#formDistrito');
        clearForm('formDistrito');
        IncluirAlterar(1, 'divAlterarDistrito', 'divIncluirDistrito', 'divExcluirDistrito', 'apresentadorMensagemDistrito', 'divCancelarDistrito', 'divLimparDistrito');
        document.getElementById("cd_localidade_distrito").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosDistrito() {
    try{
        if (document.getElementById("divAlterarDistrito").style.display == "")
            AlterarDistrito();
        else
            IncluirDistrito();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirDistrito() {
    try{
        if (!dijit.byId("formDistrito").validate())
            return false;
        apresentaMensagem('apresentadorMensagemDistrito', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/PostInsertDistrito", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_distrito").value,
                    no_localidade: dom.byId("no_localidade_distrito").value,
                    cd_loc_relacionada: dom.byId("cd_cidade_cad_distrito").value,
                    no_localidade_cidade: dom.byId("cadNomCidadeDistrito").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridDistrito';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioDistrito").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoDistrito', 'cd_localidade', 'selecionaTodosDistrito', ['pesquisarDistrito', 'relatorioDistrito'], 'todosItensDistrito');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemDistrito', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemDistrito', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarDistrito(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getDistritosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaDistrito").value) + "&inicio=" + document.getElementById("inicioDescDistrito").checked + "&cd_cidade=" + dojo.byId("cd_cidade_pesq_distrito").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({ })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridDistrito = dijit.byId("gridDistrito");

            if (limparItens)
                gridDistrito.itensSelecionados = [];

            gridDistrito.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarDistrito() {
    try{
        var gridName = 'gridDistrito';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formDistrito").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/PostUpdateDistrito",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_localidade: dom.byId("cd_localidade_distrito").value,
                    no_localidade: dom.byId("no_localidade_distrito").value,
                    cd_loc_relacionada: dom.byId("cd_cidade_cad_distrito").value,
                    no_localidade_cidade: dom.byId("cadNomCidadeDistrito").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioDistrito").hide();
                        removeObjSort(grid.itensSelecionados, "cd_localidade", dom.byId("cd_localidade_distrito").value);
                        insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoDistrito', 'cd_localidade', 'selecionaTodosDistrito', ['pesquisarDistrito', 'relatorioDistrito'], 'todosItensDistrito');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_localidade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemDistrito', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemDistrito', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarDistrito(itensSelecionados) {
    try{
        if (!dijit.byId("formDistrito").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Distrito selecionado.");
            apresentaMensagem('apresentadorMensagemDistrito', mensagensWeb);
        }

        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_localidade_distrito').value != 0)
                    itensSelecionados = [{
                        cd_localidade: dom.byId("cd_localidade_distrito").value,
                        cd_tipo_localidade: "5",
                        no_localidade: dom.byId("no_localidade_distrito").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeletedistrito",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensDistrito_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioDistrito").hide();
                        dijit.byId("pesquisaDistrito").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridDistrito').itensSelecionados, "cd_localidade", itensSelecionados[r].cd_localidade);

                        PesquisarDistrito(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarDistrito").set('disabled', false);
                        dijit.byId("relatorioDistrito").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemDistrito', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioDistrito").style.display))
                apresentaMensagem('apresentadorMensagemDistrito', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function limparTipoEndereco() {
    try{
        getLimpar('#formTipoEndereco');
        clearForm('formTipoEndereco');
        IncluirAlterar(1, 'divAlterarTipoEndereco', 'divIncluirTipoEndereco', 'divExcluirTipoEndereco', 'apresentadorMensagemTipoEndereco', 'divCancelarTipoEndereco', 'divLimparTipoEndereco');
        document.getElementById("cd_tipo_endereco").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosTipoEndereco() {
    try{
        if (document.getElementById("divAlterarTipoEndereco").style.display == "")
            AlterarTipoEndereco();
        else
            IncluirTipoEndereco();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirTipoEndereco() {
    try{
        if (!dijit.byId("formTipoEndereco").validate())
            return false;
        apresentaMensagem('apresentadorMensagemTipoEndereco', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/posttipoendereco", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_tipo_endereco: dom.byId("cd_tipo_endereco").value,
                    no_tipo_endereco: dom.byId("no_tipo_endereco").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridTipoEndereco';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoEndereco").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_tipo_endereco", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoEndereco', 'cd_tipo_endereco', 'selecionaTodosTipoEndereco', ['pesquisarTipoEndereco', 'relatorioTipoEndereco'], 'todosItensTipoEndereco');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_endereco");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoEndereco', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemTipoEndereco', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarTipoEndereco(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoEnderecosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoEndereco").value) + "&inicio=" + document.getElementById("inicioDescTipoEndereco").checked,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_endereco"
                }), Memory({ idProperty: "cd_tipo_endereco" })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTipoEndereco = dijit.byId("gridTipoEndereco");

            if (limparItens)
                gridTipoEndereco.itensSelecionados = [];

            gridTipoEndereco.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarTipoEndereco() {
    try{
        var gridName = 'gridTipoEndereco';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formTipoEndereco").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterartipoendereco",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_tipo_endereco: dom.byId("cd_tipo_endereco").value,
                    no_tipo_endereco: dom.byId("no_tipo_endereco").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoEndereco").hide();
                        removeObjSort(grid.itensSelecionados, "cd_tipo_endereco", dom.byId("cd_tipo_endereco").value);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_endereco", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoEndereco', 'cd_tipo_endereco', 'selecionaTodosTipoEndereco', ['pesquisarTipoEndereco', 'relatorioTipoEndereco'], 'todosItensTipoEndereco');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_endereco");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoEndereco', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoEndereco', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarTipoEndereco(itensSelecionados) {
    try{
        if (!dijit.byId("formTipoEndereco").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe TipoEndereco selecionado.");
            apresentaMensagem('apresentadorMensagemTipoEndereco', mensagensWeb);
        }

        require(["dojo/dom",  "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_endereco').value != 0)
                    itensSelecionados = [{
                        cd_tipo_endereco: dom.byId("cd_tipo_endereco").value,
                        no_tipo_endereco: dom.byId("no_tipo_endereco").value
                    }];

            xhr.post({
                url: Endereco() + "/api/localidade/postdeletetipoendereco",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensTipoEndereco_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoEndereco").hide();
                        dijit.byId("pesquisaTipoEndereco").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridTipoEndereco').itensSelecionados, "cd_tipo_endereco", itensSelecionados[r].cd_tipo_endereco);

                        PesquisarTipoEndereco(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarTipoEndereco").set('disabled', false);
                        dijit.byId("relatorioTipoEndereco").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoEndereco', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formularioTipoEndereco").style.display))
                    apresentaMensagem('apresentadorMensagemTipoEndereco', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);

            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function limparClasseTelefone() {
    try{
        getLimpar('#formClasseTelefone');
        clearForm('formClasseTelefone');
        IncluirAlterar(1, 'divAlterarClasseTelefone', 'divIncluirClasseTelefone', 'divExcluirClasseTelefone', 'apresentadorMensagemClasseTelefone', 'divCancelarClasseTelefone', 'divLimparClasseTelefone');
        document.getElementById("cd_classe_telefone").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosClasseTelefone() {
    try{
        if (document.getElementById("divAlterarClasseTelefone").style.display == "")
            AlterarClasseTelefone();
        else
            IncluirClasseTelefone();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirClasseTelefone() {
    try{
        if (!dijit.byId("formClasseTelefone").validate())
            return false;
        apresentaMensagem('apresentadorMensagemClasseTelefone', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postClasseTelefone", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_classe_telefone: dom.byId("cd_classe_telefone").value,
                    dc_classe_telefone: dom.byId("dc_classe_telefone").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridClasseTelefone';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioClasseTelefone").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_classe_telefone", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoClasseTelefone', 'cd_classe_telefone', 'selecionaTodosClasseTelefone', ['pesquisarClasseTelefone', 'relatorioClasseTelefone'], 'todosItensClasseTelefone');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_classe_telefone");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemClasseTelefone', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemClasseTelefone', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarClasseTelefone(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getClasseTelefonesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaClasseTelefone").value) + "&inicio=" + document.getElementById("inicioDescClasseTelefone").checked,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridClasseTelefone = dijit.byId("gridClasseTelefone");
            if (limparItens)
                gridClasseTelefone.itensSelecionados = [];
            gridClasseTelefone.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarClasseTelefone() {
    try{
        var gridName = 'gridClasseTelefone';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formClasseTelefone").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarClasseTelefone",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_classe_telefone: dom.byId("cd_classe_telefone").value,
                    dc_classe_telefone: dom.byId("dc_classe_telefone").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioClasseTelefone").hide();
                        removeObjSort(grid.itensSelecionados, "cd_classe_telefone", dom.byId("cd_classe_telefone").value);
                        insertObjSort(grid.itensSelecionados, "cd_classe_telefone", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoClasseTelefone', 'cd_classe_telefone', 'selecionaTodosClasseTelefone', ['pesquisarClasseTelefone', 'relatorioClasseTelefone'], 'todosItensClasseTelefone');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_classe_telefone");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemClasseTelefone', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemClasseTelefone', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarClasseTelefone(itensSelecionados) {
    try{
        if (!dijit.byId("formClasseTelefone").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe ClasseTelefone selecionado.");
            apresentaMensagem('apresentadorMensagemClasseTelefone', mensagensWeb);
        }

        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_classe_telefone').value != 0)
                    itensSelecionados = [{
                        cd_classe_telefone: dom.byId("cd_classe_telefone").value,
                        dc_classe_telefone: dom.byId("dc_classe_telefone").value
                    }];
            xhr.post({

                url: Endereco() + "/api/localidade/postdeleteClasseTelefone",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {

                        var todos = dojo.byId("todosItensClasseTelefone_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioClasseTelefone").hide();
                        dijit.byId("pesquisaClasseTelefone").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridClasseTelefone').itensSelecionados, "cd_classe_telefone", itensSelecionados[r].cd_classe_telefone);

                        PesquisarClasseTelefone(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarClasseTelefone").set('disabled', false);
                        dijit.byId("relatorioClasseTelefone").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemClasseTelefone', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioClasseTelefone").style.display))
                apresentaMensagem('apresentadorMensagemClasseTelefone', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function limparTipoLogradouro() {
    try{
        getLimpar('#formTipoLogradouro');
        clearForm('formTipoLogradouro');
        IncluirAlterar(1, 'divAlterarTipoLogradouro', 'divIncluirTipoLogradouro', 'divExcluirTipoLogradouro', 'apresentadorMensagemTipoLogradouro', 'divCancelarTipoLogradouro', 'divLimparTipoLogradouro');
        document.getElementById("cd_tipo_logradouro").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosTipoLogradouro() {
    try{
        if (document.getElementById("divAlterarTipoLogradouro").style.display == "")
            AlterarTipoLogradouro();
        else
            IncluirTipoLogradouro();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirTipoLogradouro() {
    try{
        if (!dijit.byId("formTipoLogradouro").validate())
            return false;
        apresentaMensagem('apresentadorMensagemTipoLogradouro', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postTipoLogradouro", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_tipo_logradouro: dom.byId("cd_tipo_logradouro").value,
                    no_tipo_logradouro: dom.byId("no_tipo_logradouro").value,
                    sg_tipo_logradouro: dom.byId("sg_tipo_logradouro").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridTipoLogradouro';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoLogradouro").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_tipo_logradouro", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoLogradouro', 'cd_tipo_logradouro', 'selecionaTodosTipoLogradouro', ['pesquisarTipoLogradouro', 'relatorioTipoLogradouro'], 'todosItensTipoLogradouro');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_logradouro");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoLogradouro', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoLogradouro', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarTipoLogradouro(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoLogradourosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoLogradouro").value) + "&inicio=" + document.getElementById("inicioDescTipoLogradouro").checked,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({ })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTipoLogradouro = dijit.byId("gridTipoLogradouro");
            if (limparItens)
                gridTipoLogradouro.itensSelecionados = [];

            gridTipoLogradouro.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarTipoLogradouro() {
    try{
        var gridName = 'gridTipoLogradouro';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formTipoLogradouro").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarTipoLogradouro",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_tipo_logradouro: dom.byId("cd_tipo_logradouro").value,
                    no_tipo_logradouro: dom.byId("no_tipo_logradouro").value,
                    sg_tipo_logradouro: dom.byId("sg_tipo_logradouro").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoLogradouro").hide();
                        removeObjSort(grid.itensSelecionados, "cd_tipo_logradouro", dom.byId("cd_tipo_logradouro").value);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_logradouro", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoLogradouro', 'cd_tipo_logradouro', 'selecionaTodosTipoLogradouro', ['pesquisarTipoLogradouro', 'relatorioTipoLogradouro'], 'todosItensTipoLogradouro');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_logradouro");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoLogradouro', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoLogradouro', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarTipoLogradouro(itensSelecionados) {
    try{
        if (!dijit.byId("formTipoLogradouro").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe TipoLogradouro selecionado.");
            apresentaMensagem('apresentadorMensagemTipoLogradouro', mensagensWeb);
        }

        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_logradouro').value != 0)
                    itensSelecionados = [{
                        cd_tipo_logradouro: dom.byId("cd_tipo_logradouro").value,
                        no_tipo_logradouro: dom.byId("no_tipo_logradouro").value,
                        sg_tipo_logradouro: dom.byId("sg_tipo_logradouro").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeleteTipoLogradouro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensTipoLogradouro_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoLogradouro").hide();
                        dijit.byId("pesquisaTipoLogradouro").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridTipoLogradouro').itensSelecionados, "cd_tipo_logradouro", itensSelecionados[r].cd_tipo_logradouro);

                        PesquisarTipoLogradouro(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarTipoLogradouro").set('disabled', false);
                        dijit.byId("relatorioTipoLogradouro").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoLogradouro', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioTipoLogradouro").style.display))
                apresentaMensagem('apresentadorMensagemTipoLogradouro', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}
function limparTipoTelefone() {
    try{
        getLimpar('#formTipoTelefone');
        clearForm('formTipoTelefone');
        IncluirAlterar(1, 'divAlterarTipoTelefone', 'divIncluirTipoTelefone', 'divExcluirTipoTelefone', 'apresentadorMensagemTipoTelefone', 'divCancelarTipoTelefone', 'divLimparTipoTelefone');
        document.getElementById("cd_tipo_telefone").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosTipoTelefone() {
    try{
        if (document.getElementById("divAlterarTipoTelefone").style.display == "")
            AlterarTipoTelefone();
        else
            IncluirTipoTelefone();
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirTipoTelefone() {
    try{
        if (!dijit.byId("formTipoTelefone").validate())
            return false;
        apresentaMensagem('apresentadorMensagemTipoTelefone', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom",  "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/posttipoTelefone", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_tipo_telefone: dom.byId("cd_tipo_telefone").value,
                    no_tipo_telefone: dom.byId("no_tipo_telefone").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridTipoTelefone';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoTelefone").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_tipo_telefone", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoTelefone', 'cd_tipo_telefone', 'selecionaTodosTipoTelefone', ['pesquisarTipoTelefone', 'relatorioTipoTelefone'], 'todosItensTipoTelefone');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_telefone");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoTelefone', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemTipoTelefone', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarTipoTelefone(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getTipoTelefonesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoTelefone").value) + "&inicio=" + document.getElementById("inicioDescTipoTelefone").checked,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({  })
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTipoTelefone = dijit.byId("gridTipoTelefone");
            if (limparItens)
                gridTipoTelefone.itensSelecionados = [];
            gridTipoTelefone.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarTipoTelefone() {
    try{
        var gridName = 'gridTipoTelefone';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formTipoTelefone").validate())
            return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterartipoTelefone",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_tipo_telefone: dom.byId("cd_tipo_telefone").value,
                    no_tipo_telefone: dom.byId("no_tipo_telefone").value
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoTelefone").hide();
                        removeObjSort(grid.itensSelecionados, "cd_tipo_telefone", dom.byId("cd_tipo_telefone").value);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_telefone", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoTelefone', 'cd_tipo_telefone', 'selecionaTodosTipoTelefone', ['pesquisarTipoTelefone', 'relatorioTipoTelefone'], 'todosItensTipoTelefone');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_telefone");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoTelefone', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoTelefone', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarTipoTelefone(itensSelecionados) {
    try{
        if (!dijit.byId("formTipoTelefone").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe TipoTelefone selecionado.");
            apresentaMensagem('apresentadorMensagemTipoTelefone', mensagensWeb);
        }

        require(["dojo/dom",  "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_telefone').value != 0)
                    itensSelecionados = [{
                        cd_tipo_telefone: dom.byId("cd_tipo_telefone").value,
                        no_tipo_telefone: dom.byId("no_tipo_telefone").value
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeletetipoTelefone",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensTipoTelefone_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioTipoTelefone").hide();
                        dijit.byId("pesquisaTipoTelefone").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridTipoTelefone').itensSelecionados, "cd_tipo_telefone", itensSelecionados[r].cd_tipo_telefone);

                        PesquisarTipoTelefone(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarTipoTelefone").set('disabled', false);
                        dijit.byId("relatorioTipoTelefone").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoTelefone', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioTipoTelefone").style.display))
                apresentaMensagem('apresentadorMensagemTipoTelefone', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function limparOperadora() {
    try{
        getLimpar('#formOperadora');
        clearForm('formOperadora');
        IncluirAlterar(1, 'divAlterarOperadora', 'divIncluirOperadora', 'divExcluirOperadora', 'apresentadorMensagemOperadora', 'divCancelarOperadora', 'divLimparOperadora');
        document.getElementById("cd_operadora").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosOperadora() {
    try{
        if (document.getElementById("divAlterarOperadora").style.display == "")
            AlterarOperadora();
        else
            IncluirOperadora();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirOperadora() {
    try{
        if (!dijit.byId("formOperadora").validate())
            return false;
        apresentaMensagem('apresentadorMensagemOperadora', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postOperadora", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_operadora: dom.byId("cd_operadora").value,
                    no_operadora: dom.byId("no_operadora").value,
                    id_operadora_ativa: domAttr.get("operadora_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridOperadora';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioOperadora").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_operadora", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoOperadora', 'cd_operadora', 'selecionaTodosOperadora', ['pesquisarOperadora', 'relatorioOperadora'], 'todosItensOperadora');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_operadora");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemOperadora', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemOperadora', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarOperadora(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try {
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getOperadorasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaOperadora").value) + "&inicio=" + document.getElementById("inicioDescOperadora").checked + "&status=" + retornaStatus("statusOperadora"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({})
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridOperadora = dijit.byId("gridOperadora");
            if (limparItens)
                gridOperadora.itensSelecionados = [];
            gridOperadora.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarOperadora() {
    try{
        var gridName = 'gridOperadora';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formOperadora").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarOperadora",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_operadora: dom.byId("cd_operadora").value,
                    no_operadora: dom.byId("no_operadora").value,
                    id_operadora_ativa: domAttr.get("operadora_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioOperadora").hide();
                        removeObjSort(grid.itensSelecionados, "cd_operadora", dom.byId("cd_operadora").value);
                        insertObjSort(grid.itensSelecionados, "cd_operadora", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoOperadora', 'cd_operadora', 'selecionaTodosOperadora', ['pesquisarOperadora', 'relatorioOperadora'], 'todosItensOperadora');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_operadora");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemOperadora', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemOperadora', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarOperadora(itensSelecionados) {
    try{
        if (!dijit.byId("formOperadora").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Operadora selecionado.");
            apresentaMensagem('apresentadorMensagemOperadora', mensagensWeb);
        }

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if(!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_operadora').value != 0)
                    itensSelecionados = [{
                        cd_operadora: dom.byId("cd_operadora").value,
                        no_operadora: dom.byId("no_operadora").value,
                        id_operadora_ativa: domAttr.get("operadora_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeleteOperadora",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensOperadora_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioOperadora").hide();
                        dijit.byId("pesquisaOperadora").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridOperadora').itensSelecionados, "cd_operadora", itensSelecionados[r].cd_operadora);

                        PesquisarOperadora(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarOperadora").set('disabled', false);
                        dijit.byId("relatorioOperadora").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        if (!hasValue(dojo.byId("formularioOperadora").style.display))
                            apresentaMensagem('apresentadorMensagemOperadora', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            apresentaMensagem('apresentadorMensagemOperadora', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function limparAtividade() {
    try{
        getLimpar('#formAtividade');
        clearForm('formAtividade');
        IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
        document.getElementById("cd_atividade").value = '';
    } catch (e) {
        postGerarLog(e);
    }
}
function EnviarDadosAtividade() {
    try{
        if (document.getElementById("divAlterarAtividade").style.display == "")
            AlterarAtividade();
        else
            IncluirAtividade();
    } catch (e) {
        postGerarLog(e);
    }
}
function IncluirAtividade() {
    try{
        if (!dijit.byId("formAtividade").validate())
            return false;
        apresentaMensagem('apresentadorMensagemAtividade', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postAtividade", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_atividade: dom.byId("cd_atividade").value,
                    no_atividade: dom.byId("no_atividade").value,
                    cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                    id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                    id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridAtividade';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_atividade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoAtividade', 'cd_atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_atividade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAtividade', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAtividade', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function PesquisarAtividade(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getAtividadesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaAtividade").value) + "&inicio=" + document.getElementById("inicioDescAtividade").checked + "&status=" + retornaStatus("statusAtividade") + "&natureza=" + retornaStatus("natureza") + "&cnae=" + encodeURIComponent(document.getElementById("cnae").value),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({})
           );

            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAtividade = dijit.byId("gridAtividade");
            if (limparItens)
                gridAtividade.itensSelecionados = [];
            gridAtividade.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarAtividade() {
    try{
        var gridName = 'gridAtividade';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formAtividade").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarAtividade",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_atividade: dom.byId("cd_atividade").value,
                    no_atividade: dom.byId("no_atividade").value,
                    cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                    id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                    id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();
                        removeObjSort(grid.itensSelecionados, "cd_atividade", dom.byId("cd_atividade").value);
                        insertObjSort(grid.itensSelecionados, "cd_atividade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoAtividade', 'cd_Atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_atividade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAtividade', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAtividade', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function DeletarAtividade(itensSelecionados) {
    try{
        if (!dijit.byId("formAtividade").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Atividade selecionado.");
            apresentaMensagem('apresentadorMensagemAtividade', mensagensWeb);
        }

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_atividade').value != 0)
                    itensSelecionados = [{
                        cd_atividade: dom.byId("cd_atividade").value,
                        no_atividade: dom.byId("no_atividade").value,
                        cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                        id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                        id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeleteAtividade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensAtividade_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();
                        dijit.byId("pesquisaAtividade").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridAtividade').itensSelecionados, "cd_Atividade", itensSelecionados[r].cd_Atividade);

                        PesquisarAtividade(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarAtividade").set('disabled', false);
                        dijit.byId("relatorioAtividade").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        if (!hasValue(dojo.byId("formularioAtividade").style.display))
                            apresentaMensagem('apresentadorMensagemAtividade', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            apresentaMensagem('apresentadorMensagemAtividade', error);
        });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D Logradouro
function pesquisarLogradouro(limparItens) {
    require([
               "dojo/store/JsonRest",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/query"
    ], function (JsonRest, ObjectStore, Cache, Memory, query) {
        try{
            var cd_bairro = hasValue(dijit.byId("bairroLogradouro").value) ? dijit.byId("bairroLogradouro").value : 0;
            var myStoreLogradouro = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/GetLogradouroSearch?descricao=" + dojo.byId("descricaoLogradouro").value + "&inicio=" + document.getElementById("inicioDescLogradouro").checked +
                       "&cd_estado=0&cd_cidade=" + dojo.byId("cd_cidade_pesq_logradouro").value + "&cd_bairro=" + cd_bairro + "&cep=" + dijit.byId("nrCEP").value,
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({})
           );
            dataStore = new ObjectStore({ objectStore: myStoreLogradouro });
            var gridLogradouro = dijit.byId("gridLogradouro");
            if (limparItens)
                gridLogradouro.itensSelecionados = [];
            gridLogradouro.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirPesquisaCidadeFKLocalidades() {
    try{
        limparFiltrosCidaddeFK();
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = PESQUISARCIDADELOGRADOURO;
        dijit.byId("paisFk").set("value", 1);
        pesquisaCidadeFK(true);
        dijit.byId("dialogConsultaCidade").show();
    } catch (e) {
        postGerarLog(e);
    }
}

function carregarBairroPorCidade(cd_cidade, tipoPesquisa,cd_bairro) {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/localidade/getBairroPorCidade?cd_cidade=" + cd_cidade + "&cd_bairro=" + cd_bairro,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            var cdBairro = null;
            if (cd_bairro != null && cd_bairro > 0)
                cdBairro = cd_bairro;
            if (tipoPesquisa == PESQUISABAIRROLOGRADOURO) {
                dijit.byId("bairroLogradouro").set("disabled", false);
                criarOuCarregarCompFiltering("bairroLogradouro", data.retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
            }
            if (tipoPesquisa == CADPESQUISABAIRROLOGRADOURO) {
                dijit.byId("cadbairroLogradouro").set("disabled", false);
                criarOuCarregarCompFiltering("cadbairroLogradouro", data.retorno, "", cdBairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemLogradouro', error);
    });
}

function keepValuesLogradouro(value, grid, ehLink) {
    try{
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

        if (hasValue(value)) {
            getLimpar('#formLogradouro');
            carregarBairroPorCidade(value.cd_localidade_cidade, CADPESQUISABAIRROLOGRADOURO, value.cd_loc_relacionada);
            dojo.byId("cd_localidade_logradouro").value = value.cd_localidade;
            dojo.byId("cd_localidade_cidade_logradouro").value = value.cd_localidade_cidade;
            dojo.byId("cadNomCidadeLogradouro").value = value.no_localidade_cidade;
            dojo.byId("cep").value = value.dc_num_cep;
            dojo.byId("descLogradouro").value = value.no_localidade;
            dijit.byId("bairroLogradouro").set("disabled", false);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparCadLogradouro() {
    try{
        dojo.byId("cd_localidade_logradouro").value = 0;
        dojo.byId("cd_localidade_cidade_logradouro").value = 0;
        getLimpar('#formLogradouro');
        clearForm("formLogradouro");
        dijit.byId("cadbairroLogradouro").set("disabled", true);
    } catch (e) {
        postGerarLog(e);
    }
}

function salvarLogradouro() {
    try{
        var compBairro = dijit.byId("cadbairroLogradouro");
        if (!dijit.byId("formularioLogradouro").validate()) {
            if (!hasValue(compBairro.value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrigatorioBairro);
                apresentaMensagem("apresentadorMensagemLogradouro", mensagensWeb);
            }
            return false;
        } else
            if (!hasValue(compBairro.value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrigatorioBairro);
                apresentaMensagem("apresentadorMensagemLogradouro", mensagensWeb);
                return false;
            }
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/postInsertLogradouro",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                cd_localidade: dojo.byId("cd_localidade_logradouro").value,
                no_localidade: dojo.byId("descLogradouro").value,
                cd_loc_relacionada: dijit.byId("cadbairroLogradouro").value,
                dc_num_cep: dojo.byId("cep").value,
                cd_localidade_cidade: dojo.byId("cd_localidade_cidade_logradouro").value,
                no_localidade_bairro: dijit.byId("cadbairroLogradouro").displayedValue,
                no_localidade_cidade: dojo.byId("cadNomCidadeLogradouro").value,
            })
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridLogradouro';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionado', 'cd_localidade', 'selecionaTodos', ['pesquisarLogradouro', 'relatorioLogradouro'], 'todosItensLogradouro');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_localidade");
                    showCarregando();
                    dijit.byId("formularioLogradouro").hide();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemLogradouro', data);
                }
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemLogradouro', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarLogradouro() {
    try{
        var compBairro = dijit.byId("cadbairroLogradouro");
        if (!dijit.byId("formularioLogradouro").validate()) {
            if (!hasValue(compBairro.value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrigatorioBairro);
                apresentaMensagem("apresentadorMensagemLogradouro", mensagensWeb);
            }
            return false;
        } else
            if (!hasValue(compBairro.value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrigatorioBairro);
                apresentaMensagem("apresentadorMensagemLogradouro", mensagensWeb);
                return false;
            }
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/postUpdateLogradouro",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                cd_localidade: dojo.byId("cd_localidade_logradouro").value,
                no_localidade: dojo.byId("descLogradouro").value,
                cd_loc_relacionada: dijit.byId("cadbairroLogradouro").value,
                dc_num_cep: dojo.byId("cep").value,
                cd_localidade_cidade: dojo.byId("cd_localidade_cidade_logradouro").value,
                no_localidade_bairro: dijit.byId("cadbairroLogradouro").displayedValue,
                no_localidade_cidade: dojo.byId("cadNomCidadeLogradouro").value,
            })
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridLogradouro';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_localidade", dojo.byId("cd_localidade_logradouro").value);
                    insertObjSort(grid.itensSelecionados, "cd_localidade", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionado', 'cd_localidade', 'selecionaTodos', ['pesquisarLogradouro', 'relatorioLogradouro'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_localidade");
                    showCarregando();
                    dijit.byId("formularioLogradouro").hide();
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemLogradouro', data);
                }
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemLogradouro', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
};

function deletarLogradouros(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_localidade_logradouro').value != 0)
                itensSelecionados = [{
                    cd_localidade: dojo.byId("cd_localidade_logradouro").value
                }];
        dojo.xhr.post({
            url: Endereco() + "/api/localidade/postDeleteLogradouros",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItensLogradouro_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridLogradouro').itensSelecionados, "cd_localidade", itensSelecionados[r].cd_localidade);
                pesquisarLogradouro(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarLogradouro").set('disabled', false);
                dijit.byId("relatorioLogradouro").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("formularioLogradouro").hide();
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("formularioLogradouro").style.display))
                apresentaMensagem('apresentadorMensagemLogradouro', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverLogradouros(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarLogradouros(itensSelecionados); });
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarLogradouro(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridLogradouro = dijit.byId('gridLogradouro');
            apresentaMensagem('apresentadorMensagem', '');
            keepValuesLogradouro(null, gridLogradouro, false);
            IncluirAlterar(0, 'divAlterarLogradouro', 'divIncluirLogradouro', 'divExcluirLogradouro', 'apresentadorMensagemLogradouro', 'divCancelarLogradouro', 'divLimparLogradouro');
            dijit.byId("formularioLogradouro").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//Metodo gobal de retorno da cidade.

function retornarCidadeLocalidades() {
    try{
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
            var value = gridPesquisaCidade.itensSelecionados;
            if (value.length > 0) {
                switch (eval(dojo.byId("tipoPesquisaCidade").value)) {
                    case PESQUISACIDADELOGRADOURO:
                        dijit.byId("limparCidadeLogradouroPes").set("disabled", false);
                        carregarBairroPorCidade(value[0].cd_cidade, PESQUISABAIRROLOGRADOURO, 0);
                        $("#cd_cidade_pesq_logradouro").val(value[0].cd_cidade);
                        $("#pesNomCidadeLogradouro").val(value[0].no_cidade);
                        break;
                    case CADPESQUISACIDADELOGRADOURO:
                        dijit.byId("cadbairroLogradouro").set("disabled", false);
                        carregarBairroPorCidade(value[0].cd_cidade, CADPESQUISACIDADELOGRADOURO, 0);
                        $("#cd_localidade_cidade_logradouro").val(value[0].cd_cidade);
                        $("#cadNomCidadeLogradouro").val(value[0].no_cidade);
                        break;
                    case PESQUISARCIDADEDISTRITO:
                        dijit.byId("limparCidadeDistritoPes").set("disabled", false);
                        $("#cd_cidade_pesq_distrito").val(value[0].cd_cidade);
                        $("#pesNomCidadeDistrito").val(value[0].no_cidade);
                        break;
                    case PESQUISACIDADECADDISTRITO:
                        $("#cd_cidade_cad_distrito").val(value[0].cd_cidade);
                        $("#cadNomCidadeDistrito").val(value[0].no_cidade);
                        dijit.byId("cadNomCidadeDistrito").set("value", value[0].no_cidade);
                        break;
                    case PESQUISACIDADEBAIRRO:
                        dijit.byId("limparCidadeBairro").set("disabled", false);
                        $("#cd_cidade_pesq_bairro").val(value[0].cd_cidade);
                        $("#pesNomCidadeBairro").val(value[0].no_cidade);
                        break;
                    case PESQUISACIDADECADBAIRRO:
                        $("#cd_cidade_cad_bairro").val(value[0].cd_cidade);
                        $("#cadNomCidadeBairro").val(value[0].no_cidade);
                        dijit.byId("cadNomCidadeBairro").set("value", value[0].no_cidade);
                        break;
                }
            }
        }
        if (!valido)
            return false;
        dijit.byId("dialogConsultaCidade").hide();
    } catch (e) {
        postGerarLog(e);
    }
}