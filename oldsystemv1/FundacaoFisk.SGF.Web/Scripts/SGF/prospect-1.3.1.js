//#region - declaração de variáveis e constantes
var STATUS_TODOS = 0;
var STATUS_ATIVO = 1;
var STATUS_INATIVO = 2;
//********************\\
var TELEFONE = 1, CELULAR = 3, EMAIL = 4;
var PROSPECT = 11;
var FAIXA_ETARIA_INFANTIL = 4;
var FAIXA_ETARIA_INFANTIL_ADULTO = 3;
var FAIXA_ETARIA_ADULTO = 1;
var EnumAcaoFollowUp = {
    AULADEMONSTRATIVA: 10
};
//#endregion

//#region - formatCheckBox, mostraTabs, formatCheckBoxMnao, formatCheckBoxFollowUp
function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridProspect';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_prospect", grid._by_idx[rowIndex].item.cd_prospect);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        setTimeout("configuraCheckBox(" + value + ", 'cd_prospect', 'selecionadoProspect', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxEmailEnviado(value, rowIndex, obj) {
    try {
        var gridFollowUp = dijit.byId("gridFollowUp");
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1 && (hasValue(value) || value == false))
            icon = "<input class='formatCheckBox'  id='" + id + "' /> ";

        setTimeout("configuraCheckBoxEmailEnviado(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxEmailEnviado(value, rowIndex, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value
            }, id);
            checkBox.set('disabled', true);
        }
        else {
            var dijitObj = dijit.byId(id);
            dijitObj.set('value', value);
            dijitObj.set('disabled', true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadAllEstados() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getAllEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                    try {
                        var itemsCb = [];
                        var cbEstado = dijit.byId("estado");

                        Array.forEach(dataEstado.retorno, function (value, i) {
                            itemsCb.push({ id: value.cd_localidade, name: value.no_localidade, sigla: value.sg_estado });
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


function loadAllTiposLogradouro() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getAllTipoLogradouro",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataTipoLogradouro) {
                    try {
                        var itemsCb = [];
                        var cbTipoLogradouro = dijit.byId("logradouro");

                        Array.forEach(dataTipoLogradouro.retorno, function (value, i) {
                            itemsCb.push({ id: value.cd_tipo_logradouro, name: value.no_tipo_logradouro, sigla: value.sg_tipo_logradouro });
                        });
                        var stateStore = new Memory({
                            data: itemsCb
                        });
                        cbTipoLogradouro.store = stateStore;
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemCidade', error);
                });
        });
}

function formatCheckBoxParticipacaoAtividadeProspect(value, rowIndex, obj) {
    try {
        var gridName = 'gridAtividadesProspects';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input disabled='disabled' class='formatCheckBox' id='" + id + "'/> ";

        setTimeout("configuraCheckBoxParticipou(" + value + ", '" + id + "', '" + gridName + "', 'disabled')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function configuraCheckBoxParticipou(value, id, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            try {
                var dojoId = dojo.byId(id);
                var grid = dijit.byId(gridName);


                if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                    dijit.byId(id).destroy();
                if (value == undefined)
                    value = false;
                if (disabled == null || disabled == undefined) disabled = false;
                if (hasValue(dojoId) && dojoId.type == 'text')
                    var checkBox = new dijit.form.CheckBox({
                        name: "checkBox",
                        checked: value,
                        disabled: disabled
                    }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function loadAllTiposEndereco() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/getAllTipoEndereco",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataTipoEndereco) {
                    try {
                        var itemsCb = [];
                        var cbEndereco = dijit.byId("tipoEndereco");

                        Array.forEach(dataTipoEndereco.retorno, function (value, i) {
                            itemsCb.push({ id: value.cd_tipo_endereco, name: value.no_tipo_endereco });
                        });
                        var stateStore = new Memory({
                            data: itemsCb
                        });
                        cbEndereco.store = stateStore;
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemCidade', error);
                });
        });
}

function loadComponentesTela(dataTipoEndereco, dataLogradouro, dataEstado,  Memory, carregando) {
    try {
        loadTipoEndereco(dataTipoEndereco, 'tipoEndereco', Memory);
        //loadTipoEndereco(dataTipoEndereco, 'tipoEnderecoRelac', Memory);
        loadLogradouro(dataLogradouro, 'logradouro', Memory);
        //loadLogradouro(dataLogradouro, 'logradouroRelac', Memory);
        loadEstado(dataEstado, 'estado', Memory);
        //loadEstado(dataEstado, 'estadoRelac', Memory);
    }
    catch (e) {
        postGerarLog(e);
    }
}


//carrega os dados para uma nova pessoa
// Monta o tipo de endereço
function loadTipoEndereco(dataTipoEndereco, idComponente, Memory) {
    try {
        (hasValue(dataTipoEndereco.retorno)) ? dataTipoEndereco = dataTipoEndereco.retorno : dataTipoEndereco;

        var dados = [];
        $.each(dataTipoEndereco, function (index, val) {
            dados.push({
                "name": val.no_tipo_endereco,
                "id": val.cd_tipo_endereco
            });
        });

        var stateStore = new Memory({
            data: eval(dados)
        });
        dijit.byId(idComponente).store = stateStore;
        //dijit.byId(idComponente).set("value", 2);
    }
    catch (e) {
        postGerarLog(e);
    }
}



// Monta o Logradouro
function loadLogradouro(data, idComponente, Memory) {
    try {
        (hasValue(data.retorno)) ? data = data.retorno : data;

        var dados = [];
        $.each(data, function (index, val) {
            dados.push({
                "name": val.no_tipo_logradouro,
                "id": val.cd_tipo_logradouro
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

//Monta máscaras para telefone, cpf e cnpj
function inicial() {
    require([
           "dojo/ready",
           "dojo/on",
           "dojo/store/Memory",
           "dijit/form/FilteringSelect"
    ], function (ready, on, Memory, FilteringSelect) {
        ready(function () {
            try {
                $("#cep").mask("99999-999");
                $("#cepOutrosEnd").mask("99999-999");
                $("#cepRelac").mask("99999-999");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function mostraTabs(Permissoes) {
    require([
         "dijit/registry",
         "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            try{
                if (!possuiPermissao('prospect', Permissoes)) {
                    registry.byId('tabProspect').set('disabled', !registry.byId('tabProspect').get('disabled'));
                    document.getElementById('tabProspect').style.visibility = "hidden";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function formatCheckBoxMnao(value, rowIndex, obj) {
    try{
        var gridName = 'gridMotivoNao'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMnao');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_nao_matricula", grid._by_idx[rowIndex].item.cd_motivo_nao_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:9px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_nao_matricula', 'selecionadoMnao', -1, 'selecionaTodosMnao', 'selecionaTodosMnao', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_nao_matricula', 'selecionadoMnao', " + rowIndex + ", '" + id + "', 'selecionaTodosMnao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxFollowUp(value, rowIndex, obj) {
    try{
        var gridName = 'gridFollowUp'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFollowUp');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nroOrdem", grid._by_idx[rowIndex].item.cd_follow_up);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:9px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nroOrdem', 'selecionadoFollowUp', -1, 'selecionaTodosFollowUp', 'selecionaTodosFollowUp', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nroOrdem', 'selecionadoFollowUp', " + rowIndex + ", '" + id + "', 'selecionaTodosFollowUp', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion
//#region - Métodos auxiliares - LimparCadPospect(), sexoProspect,  periodoProspect,  maskDate, isEmail, returnDataProspect, loadFormEditacao, populaProdutoProspect, testarData

function testarData() {
    try{
        var retornar = true;
        var result = dojo.date.compare(dataHoje, dijit.byId('dtaCadastro').get('value'), "date");

        if (result < 0 ){
            caixaDialogo(DIALOGO_ERRO, 'Data de cadastro maior que a data corrente.', null);
            retornar = false;
        }
        if (retornar) {
            var result = dojo.date.compare(dataHoje, dijit.byId('dtaNasci').get('value'), "date");

            if (result < 0) {
                caixaDialogo(DIALOGO_ERRO, 'Data de nascimento maior que a data corrente.', null);
                retornar = false;
            }
        }

        return retornar;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadPospect() {
    require(["dojo/data/ObjectStore", "dojo/store/Memory", "dojo/ready"], function (ObjectStore, Memory, ready) {
        ready(function () {
            try{
                dojo.byId('cd_pessoa_fisica').value = "";
                dojo.byId("cd_prospect").value = "";
                dojo.byId("nomeProspect").value = "";
                dojo.byId('telefone').value = "";
                dijit.byId("emailProspect")._onChangeActive = false;
                dijit.byId("emailProspect").reset();
                dijit.byId("emailProspect")._onChangeActive = true;
                dojo.byId('celular').value = "";
                dijit.byId("telefone").set("value", "");
                dijit.byId("celular").set("value", "");
               
                dijit.byId("dt_matricula_prosp").reset();
                dijit.byId("valorMatProsp").set("value", "");
                dijit.byId("ckBaixa").set("checked", false);
                dijit.byId("ativo").set("checked", true);
                dojo.byId('planoContasProsp').value = '';
                dojo.byId('cd_plano_contas_prosp').value = 0;
                dijit.byId("limparPlanoContasProsp").set('disabled', true);

                dijit.byId("cbLocal").reset();
                dijit.byId("cbLiquidacao").reset();
                dijit.byId("valorAbatMat").reset();
                
                dojo.byId('no_escola').value = '';

                dijit.byId('id_faixa_etaria_1').set('checked', false);
                dijit.byId('id_faixa_etaria_2').set('checked', false);
                dijit.byId('id_faixa_etaria_3').set('checked', false);

                if (hasValue(dojo.byId('cbMarketing').value, true)) 
                    dijit.byId("cbMarketing").set("value", "");
                
                if (hasValue(dojo.byId('cbSexo').value, true)) 
                    dijit.byId("cbSexo").reset();
                
                if (hasValue(dojo.byId('cbPeriodo').value, true)) 
                    dijit.byId("cbPeriodo").reset();
                
                if (hasValue(dojo.byId('operadora').value, true)) 
                    dijit.byId("operadora").reset();
                
                if (hasValue(dojo.byId('cbDiaSemana').value, true)) 
                    dijit.byId("cbDiaSemana").reset();
                
                if(hasValue(dijit.byId('cbProduto')) && hasValue(dijit.byId('cbProduto').store))
                    dijit.byId('cbProduto').setStore(dijit.byId('cbProduto').store, [0]);

                if (hasValue(dijit.byId('cbPeriodo')) && hasValue(dijit.byId('cbPeriodo').store))
                    dijit.byId('cbPeriodo').setStore(dijit.byId('cbPeriodo').store, [0]);

                if (hasValue(dijit.byId('cbDiaSemana')) && hasValue(dijit.byId('cbDiaSemana').store))
                    dijit.byId('cbDiaSemana').setStore(dijit.byId('cbDiaSemana').store, [0]);

                var gridFollowUp = dijit.byId("gridFollowUp");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                gridFollowUp.setStore(dataStore);
                var gridMotivoNao = dijit.byId("gridMotivoNao");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                gridMotivoNao.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
    apresentaMensagem("apresentadorMensagemProspect", "");
}

function loadSexo(Memory) {
    try{
        var stateStore = new Memory({
            data: [
                    { name: "Feminino", id: 1 },
					{ name: "Masculino", id: 2 },
					{ name: "Não Binário", id: 3 },
					{ name: "Prefiro não responder ou Neutro", id: 4 },
            ]
        });
        dijit.byId('cbSexo').store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPeriodo(Memory, registry, ItemFileReadStore) {
    try{
        var w = registry.byId("cbPeriodo");
        var stateStore = [
                   { name: "Manhã", id: "1" },
                   { name: "Tarde", id: "2" },
                   { name: "Noite", id: "3" }
        ];
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: stateStore
            }
        });
        w.setStore(store, []);
    }
    catch (e) {
        postGerarLog(e);
    }
}



function loadTesteClassificacao(nomElement, Memory, registry, ItemFileReadStore) {
	require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
		function (Memory, filteringSelect) {
			try {
				var statusStore = null;

				
			
				statusStore = new Memory({
					data: [
						{ name: "Todos", id: "0" },
						{ name: "Com Teste de Classificação", id: "1" },
						{ name: "Com Pré Matrícula Onlíne", id: "2" },
						{ name: "Com (Teste de Classificação e Pré Matrícula Onlíne)", id: "3" },
						{ name: "Sem (Teste de Classificação e Pré Matrícula Onlíne)", id: "4" }
					]
				});
				var status = new filteringSelect({
					id: nomElement,
					name: "testeClassificacaoPreMatricula",
					value: "0",
					store: statusStore,
					searchAttr: "name",
					style: "width: 100%;"
				}, nomElement);
			}
			catch (e) {
				postGerarLog(e);
			}
		});
}


function loadDiaSemana(Memory, registry, ItemFileReadStore) {
    try {
        var cbDia = registry.byId("cbDiaSemana");
        var stateStore = [
                   { name: "Domingo", id: "1" },
                   { name: "Segunda", id: "2" },
                   { name: "Terça",   id: "3" },
                   { name: "Quarta",  id: "4" },
                   { name: "Quinta",  id: "5" },
                   { name: "Sexta",   id: "6" },
                   { name: "Sábado",  id: "7" }
                   
        ];
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: stateStore
            }
        });
        cbDia.setStore(store, []);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Testa o Email
function isEmail(email) {
    try{
        var exclude = /[^@\-\.\w]|^[_@\.\-]|[\._\-]{2}|[@\.]{2}|(@)[^@]*\1/;
        var check = /@[\w\-]+\./;
        var checkend = /\.[a-zA-Z]{2,5}$/;
        var obj = eval("document.forms[0][2]");
        if (hasValue(trim(email))) {
            if (((email.search(exclude) != -1) || (email.search(check)) == -1) || (email.search(checkend) == -1)) {
                caixaDialogo(MENSAGEM_AVISO, 'E-mail no formato inválido.', "");
                return false;
                obj.focus();
            }
            else 
                return true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function returnDataProspect(cdProspect, showCarreg) {
    if (showCarreg)
        showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/returnDataProspect?cdOperadora=null&cdProspect=" + cdProspect,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataProspect) {
        try {
            dataProspect = jQuery.parseJSON(dataProspect);
            if (!hasValue(dataProspect.erro)) {
                dojo.byId('dc_email_escola').value = dataProspect.retorno.dc_email_escola;
                dojo.byId('atendente').value = dataProspect.retorno.no_pessoa;
                dojo.byId('cdAtendente').value = dataProspect.retorno.cd_usuario;
                if (hasValue(dijit.byId("cbMarketing").value == "")) {
                    var midias = dataProspect.retorno.midias;

                    if (indexValue(midias, "no_midia", "Teste de Classificação") != null &&
                        indexValue(midias, "no_midia", "Pré Matrícula online") !== null) {
                        if (indexValue(midias, "no_midia", "Teste de Classificação") + 1 !=
                            dataProspect.retorno.cd_midia &&
                            indexValue(midias, "no_midia", "Pré Matrícula online") + 1 != dataProspect.retorno.cd_midia) {
                            midias.splice(indexValue(midias, "no_midia", "Teste de Classificação"), 1);
                            midias.splice(indexValue(midias, "no_midia", "Pré Matrícula online"), 1);
                            dijit.byId("cbMarketing").set('disabled', false);
                        }
                    } 
                    
                    loadSelect(midias, 'cbMarketing', 'cd_midia', 'no_midia');
                    if (hasValue(dijit.byId("cbMarketing")))
                        dijit.byId("cbMarketing").set("value", id);
                }
                if (hasValue(dijit.byId("operadora").value == "")) {
                    loadSelect(dataProspect.retorno.operadoras, 'operadora', 'cd_operadora', 'no_operadora');
                    if (hasValue(dijit.byId("operadora")))
                        dijit.byId("operadora").set("value", id);
                }
                if (cdProspect > 0)
                    montarGridProspectSites(cdProspect, dataProspect.retorno); //loadFormEditacao(dataProspect.retorno)
                else {
                    dijit.byId('dtaCadastro').reset();
                    dijit.byId('dtaCadastro').set("value", dataProspect.retorno.dt_cadastramento);
                    dijit.byId('dtaCadastro').set('disabled', false);
                    dijit.byId('dt_matricula_prosp').reset();
                    dijit.byId('dt_matricula_prosp').set("value", dataProspect.retorno.dt_cadastramento);
                    dijit.byId('dt_matricula_prosp').set('disabled', false);
                    dijit.byId('dtaNasci').reset();
                    dijit.byId('dtaNasci').set("value", dataProspect.retorno.dt_nascimento_prospect);
                }

                var dataEndereco;
                if (hasValue(dataProspect.retorno.endereco)) {
                    dataEndereco = dataProspect.retorno.endereco;
                }
                if (hasValue(dataProspect.retorno.endereco)) {
                    setarRetornoEnderecoPrincipal(dataProspect.retorno.endereco, true, dataEndereco.num_cep, 'apresentadorMensagemPessoa');
                    var compNumero = dijit.byId("numero");
                    if (hasValue(dataEndereco.dc_num_endereco))
                        compNumero.set("value", dataEndereco.dc_num_endereco);

                    var compComplemento = dijit.byId("complemento");
                    if (hasValue(dataEndereco.dc_compl_endereco))
                        compComplemento.set("value", dataEndereco.dc_compl_endereco);

                    var compEndereco = dijit.byId("tipoEndereco");
                    if (hasValue(dataEndereco.cd_tipo_endereco))
                        compEndereco.set("value", dataEndereco.cd_tipo_endereco);
                }
            } else
                apresentaMensagem('apresentadorMensagem', dataProspect);
            if (showCarreg)
                showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        if (showCarreg)
            showCarregando();
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function indexValue(arraytosearch, key, valuetosearch) {

    for (var i = 0; i < arraytosearch.length; i++) {
    
        if (arraytosearch[i][key] == valuetosearch) {
            return i;
        }
    }
    return null;
}

function loadFormEditacao(value, testesProspect) {
    try{
        limparCadPospect();
        //dados do usuário
        //loadSelect(value.operadoras, 'operadora', 'cd_operadora', 'no_operadora');
        returnLocalMovimento(value.cd_local_movto, value.cd_tipo_liquidacao, false);
        dojo.byId('atendente').value = value.no_usuario;
        dojo.byId('cdAtendente').value = value.cd_usuario;
        dojo.byId("cd_prospect").value = value.cd_prospect;
        dojo.byId("nomeProspect").value = value.no_pessoa;
        dojo.byId('cd_pessoa_fisica').value = value.cd_pessoa_fisica;
        dijit.byId('dtaCadastro').value = value.dta_cadastramento_prospect != null ? dojo.byId("dtaCadastro").value = value.dta_cadastramento_prospect : "";
        dijit.byId('dtaNasci').value = value.dta_nascimento_prospect != null ? dojo.byId("dtaNasci").value = value.dta_nascimento_prospect : "";
        dojo.byId("emailProspect").value = value.email;
        dojo.byId('celular').value = value.celular;
        dojo.byId("ativo").value = value.id_prospect_ativo == true ? dijit.byId("ativo").set("value", true) : dijit.byId("ativo").set("value", false);
        dojo.byId('telefone').value = value.telefone;

        dojo.byId("dt_matricula_prosp").value = value.dta_matricula_prospect;
        dijit.byId("valorMatProsp").set("value", value.vl_matricula);
        dijit.byId("ckBaixa").set("checked", value.gerar_baixa);
        dojo.byId("cd_plano_contas_prosp").value = value.cd_plano_conta_tit;
        dojo.byId("planoContasProsp").value = value.desc_plano_conta;
        dijit.byId("planoContasProsp").set("value", value.desc_plano_conta);
        dijit.byId("cbLocal").set("value", value.cd_local_movto);
        dijit.byId("cbLiquidacao").set("value", value.cd_tipo_liquidacao);

        dojo.byId('no_escola').value = value.no_escola;
        if (value.id_faixa_etaria == FAIXA_ETARIA_INFANTIL)
            dijit.byId('id_faixa_etaria_1').set('checked', true);
        else if (value.id_faixa_etaria == FAIXA_ETARIA_INFANTIL_ADULTO)
            dijit.byId('id_faixa_etaria_2').set('checked', true);
        else if (value.id_faixa_etaria == FAIXA_ETARIA_ADULTO)
            dijit.byId('id_faixa_etaria_3').set('checked', true);
        dijit.byId("cbMotivoInatividade").set("value", value.cd_motivo_inativo);
        dojo.byId("valorAbatMat").value = value.vlAbatimento;

        if (hasValue(value.cd_plano_conta_tit))
            dijit.byId("limparPlanoContasProsp").set('disabled', false);

        if (hasValue(dojo.byId('operadora').value, true))
            hasValue(value.cd_operadora) ? dijit.byId("operadora").set("value", value.cd_operadora) : 0;

        if (hasValue(dojo.byId('cbSexo').value, true))
            hasValue(value.nm_sexo) ? dijit.byId("cbSexo").set("value", value.nm_sexo) : 0;

        habilitaOperadora(dojo.byId("celular"));

        if (hasValue(dojo.byId('cbMarketing').value, true)) {
            
            hasValue(value.cd_midia) ? dijit.byId("cbMarketing").set("value", value.cd_midia) : 0;

            if ((indexValue(value.midias, "no_midia", "Teste de Classificação") + 1) == value.cd_midia ||
                (indexValue(value.midias, "no_midia", "Pré Matrícula online") + 1) == value.cd_midia)
            {

                dojo.byId('testeClassificacao').style.display = 'block';
                dijit.byId('testeClassificacao').set('open', true);
                dojo.byId('matriculaOnline').style.display = 'none';
                dijit.byId('matriculaOnline').set('open', false);

                dijit.byId("cbMarketing").set('disabled', true);


                var gridTesteClassificacao = dijit.byId("gridTeste");
                gridTesteClassificacao.itensSelecionados = [];
                gridTesteClassificacao.setStore(testesProspect);

                //dojo.byId("edIdTeste").value = value.id_teste_online;
                //dojo.byId("edAcertos").value = value.pc_acerto_teste;
                //dojo.byId("edFase").value = value.dc_acerto_teste;

                //dijit.byId("edIdTeste").set('disabled', true);
                //dijit.byId("edAcertos").set('disabled', true);
                //dijit.byId("edFase").set('disabled', true);

                
            //} else if ((indexValue(value.midias, "no_midia", "Pré Matrícula online") + 1) == value.cd_midia) {

            //    dojo.byId('matriculaOnline').style.display = 'block';
            //    dijit.byId('matriculaOnline').set('open', true);
            //    dojo.byId('testeClassificacao').style.display = 'none';
            //    dijit.byId('testeClassificacao').set('open', false);

            //    dijit.byId("cbMarketing").set('disabled', true);

            //    var gridPreMatricula = dijit.byId("gridPreMatricula");
            //    gridPreMatricula.itensSelecionados = [];
            //    gridPreMatricula.setStore(testesProspect);

            //    //dojo.byId("edIDMatricula").value = value.id_teste_online;
            //    //dijit.byId("edIDMatricula").set('disabled', true);

            } else {

                dojo.byId('testeClassificacao').style.display = 'none';
                dijit.byId('testeClassificacao').set('open', false);
                dojo.byId('matriculaOnline').style.display = 'none';
                dijit.byId('matriculaOnline').set('open', false);
            }
            
        }
            

        var cbDiaSemana = new Array();
        for (var i = 0; i < value.dias.length; i++)
            cbDiaSemana[i] = value.dias[i].id_dia_semana;

        dijit.byId('cbDiaSemana').setStore(dijit.byId('cbDiaSemana').store, cbDiaSemana);
            
        var setCbProduto = new Array();
        for (var i = 0; i < value.produtos.length; i++)
            setCbProduto[i] = value.produtos[i].cd_produto;

        if(hasValue(dijit.byId('cbProduto')) && hasValue(dijit.byId('cbProduto').store))
            dijit.byId('cbProduto').setStore(dijit.byId('cbProduto').store, setCbProduto);

        var setCbPeriodo = new Array();
        for (var i = 0; i < value.periodos.length; i++)
            setCbPeriodo[i] = value.periodos[i].id_periodo;

        dijit.byId('cbPeriodo').setStore(dijit.byId('cbPeriodo').store, setCbPeriodo);

        loadGridFollowUp(value.cd_prospect);
        loadGridMotivoNao(value.cd_prospect);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montarGridProspectSites(cdProspect, retorno) {

    try {

        if (retorno.id_tipo_online == null) {

            loadFormEditacao(retorno, null);

        } else {
            apresentaMensagem('apresentadorMensagemProspect', null);
            require([
                "dojo/_base/xhr",
                "dojo/data/ObjectStore",
                "dojo/store/Memory"
            ], function (xhr, ObjectStore, Memory) {
                xhr.get({
                    url: Endereco() + "/api/secretaria/getProspectSite?cdProspect=" + cdProspect + "&tipo=" + retorno.id_tipo_online,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() },
                }).then(function (values) {
                        try {
                            var dataStore;

                            if (hasValue(values.retorno) && values.retorno.length > 0) {
                                dataStore = new ObjectStore({
                                    objectStore: new Memory({ data: values.retorno, idProperty: "cd_prospect_site" })
                                });

                            } else {
                                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                            }
                            loadFormEditacao(retorno, dataStore);

                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagemProspect', error);

                    });
            });
        }


    }
    catch (e) {
        postGerarLog(e);
    }

}

function keepValues(value, grid, ehLink) {
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
        dijit.byId('dtaCadastro').set('disabled', true);
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];
        returnDataProspect(grid.itensSelecionados[0].cd_prospect, true);

        pesquisarProspectsAtividadesProspect(grid.itensSelecionados[0].cd_prospect);
        //returnLocalMovimento(grid.itensSelecionados[0].cd_local_movto, grid.itensSelecionados[0].cd_tipo_liquidacao);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaProdutoProspect() {
    require([
       "dojo/_base/xhr",
        "dojo/ready",
        "dijit/registry",
        "dojo/data/ItemFileReadStore"
    ], function (xhr, ready, registry, ItemFileReadStore) {
        ready(function () {
            try{
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/getProduto?hasDependente=0&cd_produto=null",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    var produtos =   jQuery.parseJSON(data).retorno;
                    var w = registry.byId("cbProduto");
                    var produtoCb = [];
                    if (produtos != null || produtos.length > 0)
                        $.each(produtos, function (index, val) {
                            produtoCb.push({
                                "cd_produto": val.cd_produto+"",
                                "no_produto": val.no_produto
                            });
                        });
                    var store = new ItemFileReadStore({
                        data: {
                            identifier: "cd_produto",
                            label: "no_produto",
                            items: produtoCb
                        }
                    });
                    w.setStore(store, []);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}



function habilitaOperadora(obj) {
    try{
        if (hasValue(obj.value) && obj.value != '(__) ____-_____' && obj.value != '(__) _____-____' && (obj.value.length > 2 && obj.value.substring(obj.value.length - 2, obj.value.length) != '__'))
            dijit.byId('operadora').set('disabled', false);
        else {
            dijit.byId('operadora').set('disabled', true);
            dijit.byId('operadora').set('value', '');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarProspectCadastro(windowUtils) {
    try{
        var telefone = dijit.byId("telefone");
        var email = dijit.byId("email");
        var maskTelefone = '(__) ____-____';
        var maskCelular = '(__) ____-_____';

        //var rg = dijit.byId("nroRg");
        var validado = true;
        if (!dijit.byId("formProspect").validate())
            validado = false;

        if (dojo.byId("telefone").value == maskTelefone)
            dijit.byId("telefone").set("value", "");

        if (dojo.byId("celular").value == maskCelular)
            dijit.byId("celular").set("value", "");

        if (!hasValue(dojo.byId("celular").value) && !hasValue(dojo.byId("telefone").value)) {
            dijit.byId("telefone").set("required", true);
            dijit.byId("celular").set("required", true);
            var celular = dijit.byId("celular");
            if (!celular.validate()) {
                mostrarMensagemCampoValidado(windowUtils, celular);
                validado = false;
            }
            if (!telefone.validate()) {
                mostrarMensagemCampoValidado(windowUtils, telefone);
                validado = false;
            }
        } else {
            dijit.byId("telefone").set("required", false);
            dijit.byId("celular").set("required", false);
            validado = true;
        }

        if (!testarData())
            validado = false;

        if (!dijit.byId("formProspect").validate())
            validado = false;
        if(!dijit.byId("formPreMat").validate())
            validado = false;

        if (hasValue(dojo.byId('valorMatProsp').value))
            dijit.byId("planoContasProsp").set("required", true);

        if (!dijit.byId("planoContasProsp").validate())
            validado = false;
        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region - loadGridFollowUp, loadGridMotivoNao, destroyCreateGridFollowUp, destroyCreateGridMotivoNao
function loadGridFollowUp(cdProspect) {
    apresentaMensagem('apresentadorMensagemProspect', null);
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/secretaria/GetFollowUpProspect?cdProspect=" + cdProspect,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() },
            idProperty: "nroOrdem"
        }).then(function (dataFollowUp) {
            try{
                if (dataFollowUp.MensagensWeb.length > 0)
                    apresentaMensagem('apresentadorMensagemFollowUp', dataFollowUp);
                else
                    apresentaMensagem('apresentadorMensagemFollowUp', null);
                var gridFollowUp = dijit.byId("gridFollowUp");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataFollowUp.retorno, idProperty: "nroOrdem" }) });

                gridFollowUp.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemFollowUp', error);
            var gridFollowUp = dijit.byId("gridFollowUp");
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            gridFollowUp.setStore(dataStore);
        });
    });
}

function loadGridMotivoNao(cdProspect) {
    apresentaMensagem('apresentadorMensagemProspect', null);
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/secretaria/getProspectMotivoNaoMatricula?cdProspect=" + cdProspect,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() },
            idProperty: "cd_prospect_motivo_nao_matricula"
        }).then(function (dataMotivoNao) {
            try{
                if (dataMotivoNao.MensagensWeb.length > 0)
                    apresentaMensagem('apresentadorMensagemMotivoNao', dataMotivoNao);
                else
                    apresentaMensagem('apresentadorMensagemMotivoNao', null);
                var gridMotivoNao = dijit.byId("gridMotivoNao");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataMotivoNao.retorno, idProperty: "cd_prospect_motivo_nao_matricula" }) });

                gridMotivoNao.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemMotivoNao', error);
            var gridMotivoNao = dijit.byId("gridMotivoNao");
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            gridMotivoNao.setStore(dataStore);
        });
    });
}


function formatCheckBoxAtividadesProspects(value, rowIndex, obj) {
    try {
        var gridName = 'gridAtividadesProspects';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAtividadesProspects');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_prospect", grid._by_idx[rowIndex].item.cd_prospect);
            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_prospect', 'selecionadoAtividadesProspects', -1, 'selecionaTodosAtividadesProspects', 'selecionaTodosAtividadesProspects', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_prospect', 'selecionadoAtividadesProspects', " + rowIndex + ", '" + id + "', 'selecionaTodosAtividadesProspects', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region - criação da Grid prospect
function montarGridProspect() {
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, DropDownButton, DropDownMenu, MenuItem, ItemFileReadStore) {
        var cdProspect = 0;
        ready(function () {
            try {
                loadAllTiposEndereco();
                loadAllTiposLogradouro();
                loadAllEstados();
                montarStatus("status");
                populaProdutoProspect();
                returnDataProspect(cdProspect,false);
                loadSexo(Memory);
                loadPeriodo(Memory, registry, ItemFileReadStore);
                loadTesteClassificacao("cbTesteClassificacaoMatriculaOnline", Memory, registry, ItemFileReadStore);
                loadDiaSemana(Memory, registry, ItemFileReadStore);
                
                dojo.byId('dataHoje').value = dataHoje;

                setMenssageMultiSelect(PERIODO, 'cbPeriodo');
                setMenssageMultiSelect(DIA, 'cbDiaSemana');
                setMenssageMultiSelect(PRODUTO, 'cbProduto');

                dijit.byId("cbDiaSemana").on("change", function (e) {
                    setMenssageMultiSelect(DIA, 'cbDiaSemana');
                });
                dijit.byId("cbPeriodo").on("change", function (e) {
                    setMenssageMultiSelect(PERIODO, 'cbPeriodo');
                });
                dijit.byId("cbProduto").on("change", function (e) {
                    setMenssageMultiSelect(PRODUTO, 'cbProduto');
                });
                dijit.byId('ckBaixa').on('change', function (e) {
                    try{
                        if (e) {
                            dijit.byId('dt_matricula_prosp').set("required", true);
                            dijit.byId('valorMatProsp').set("required", true);
                            dijit.byId('cbLocal').set("required", true);
                            verificaObrigPlanoConta();
                            dijit.byId('cbLiquidacao').set("required", true);

                            dijit.byId('cbLocal').set('disabled', false);
                            dijit.byId('cbLiquidacao').set('disabled', false);
                        }
                        else {
                            if (!hasValue(dijit.byId('valorMatProsp').value)) {
                                dijit.byId('dt_matricula_prosp').set('required', false);
                                dijit.byId('planoContasProsp').set("required", false);
                            }
                            dijit.byId('valorMatProsp').set("required", false);
                            dijit.byId('cbLocal').set("required", false);
                            dijit.byId('cbLiquidacao').set("required", false);

                            dijit.byId('cbLocal').set('disabled', true);
                            dijit.byId('cbLocal').reset();
                            dijit.byId('cbLiquidacao').set('disabled', true);
                            dijit.byId('cbLiquidacao').reset();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId('cbLiquidacao').set("required", false);
                dijit.byId('dt_matricula_prosp').set("required", false);
                dijit.byId('valorMatProsp').set("required", false);
                dijit.byId('cbLocal').set("required", false);
                dijit.byId('planoContasProsp').set("required", false);

                //marca true e desabilita se veio do popup de prospects não lidos
                var urlParams = new URLSearchParams(window.location.search);
                var urlSearch = null;
                if(urlParams.has('redirect')) {
                    urlSearch = "/secretaria/getProspectSearch?nome=&inicio=false&email=&dataIni=&dataFim=&status=1&aluno=false&testeClassificacaoMatriculaOnline=3";
                    dijit.byId("ckTesteClassificacao").set("checked", true);
                    dijit.byId("ckTesteClassificacao").set('disabled', true);

                    dijit.byId("ckMatriculaOnline").set("checked", true);
                    dijit.byId("ckMatriculaOnline").set('disabled', true);
                }else {
                    urlSearch = "/secretaria/getProspectSearch?nome=&inicio=false&email=&dataIni=&dataFim=&status=1&aluno=false&testeClassificacaoMatriculaOnline=0";
                }


                //Grid de aluno atividade
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                var gridAtividadesProspects = new EnhancedGrid({
                    store: dataStore,
                    structure:
                    [
                        { name: "<input id='selecionaTodosAtividadesProspects' style='display:none'/>", field: "selecionadoAtividadesProspects", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividadesProspects },
                        //{ name: "Código", field: "cd_aluno", width: "15%", minwidth: "10%" },
                        { name: "Nome", field: "no_pessoa", width: "35%", minwidth: "10%" },
                        { name: "Atividade", field: "dc_tipo_atividade_extra", width: "25%", minwidth: "10%" },
                        //{ name: "Escola", field: "dc_reduzido_pessoa_escola", width: "35%", minwidth: "10%" },
                        { name: "Participou", field: "ind_participacao", width: "20%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParticipacaoAtividadeProspect },
                        { name: "Obs.", field: "txt_obs_atividade_prospect", width: "50%", minwidth: "10%" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["9", "18", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "9",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 3,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridAtividadesProspects");

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAtividadesProspects, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosAtividadesProspects').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_atividade_prospect',  'selecionadoAtividadesProspects', -1, 'selecionaTodosAtividadesProspects', 'selecionaTodosAtividadesProspects', 'gridAtividadesProspects')", gridAtividadesProspects.rowsPerPage * 3);
                    });
                });

                gridAtividadesProspects.canSort = function (col) { return Math.abs(col) != 1; };
                gridAtividadesProspects.startup();

                var myStore =
                Cache(
                        JsonRest({
                            target: Endereco() + urlSearch,
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));


                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridProspect = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoProspect", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                       // { name: "Código", field: "cd_pessoa", width: "60px", styles: "text-align: right; min-width:60px; max-width:60px;" },
                        { name: "Nome", field: "no_pessoa", width: "35%", styles: "min-width:10%; max-width: 50%;" },
                        { name: "E-mail", field: "email", width: "20%" },
                        { name: "Telefone", field: "telefone", width: "10%", styles: "min-width:80px; max-width:80px;" },
                        { name: "Celular", field: "celular", width: "10%", styles: "min-width:80px; max-width:80px;" },
                        { name: "Data Cadastro", field: "dta_cadastramento_prospect", width: "10%", styles: "min-width:80px; max-width:80px;" },
                        { name: "Ativo", field: "prospect_ativo", width: "10%", styles: "text-align: center;" }
                    ],
                    noDataMessage: msgNotRegEncFiltro,
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
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridProspect");
                gridProspect.pagination.plugin._paginator.plugin.connect(gridProspect.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridProspect, 'cd_prospect', 'selecionaTodos');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridProspect, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_prospect', 'selecionadoProspect', -1, 'selecionaTodos', 'selecionaTodos', 'gridProspect')", gridProspect.rowsPerPage * 3);
                    });
                });
                gridProspect.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 7; };
                gridProspect.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        var cd_prospect = item.cd_prospect;
                        var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                        limparCadPospect();
                        limparEnderecoPrincipal();
                        //returnLocalMovimento(dataProspect.retorno.cd_local_movto, dataProspect.retorno.cd_tipo_liquidacao);
                        returnLocalMovimento(0,null,false);
                        dijit.byId("gridFollowUp").setStore(dataStore);
                        dijit.byId("gridMotivoNao").setStore(dataStore);
                        xhr.get({
                            url: Endereco() + "/api/secretaria/returnDataProspect?cdOperadora=null&cdProspect=" + cd_prospect,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataProspect) {
                            try {

                                dataProspect = jQuery.parseJSON(dataProspect);
                                if (!hasValue(dataProspect.erro)) {

                                    dojo.byId('atendente').value = dataProspect.retorno.no_pessoa;
                                    dojo.byId('cdAtendente').value = dataProspect.retorno.cd_usuario;

                                    dojo.byId("dt_matricula_prosp").value = dataProspect.retorno.dta_matricula_prospect;
                                    dijit.byId("valorMatProsp").set("value", dataProspect.retorno.vl_matricula);
                                    dijit.byId("ckBaixa").set("checked", dataProspect.retorno.gerar_baixa);
                                    dojo.byId("cd_plano_contas_prosp").value = dataProspect.retorno.cd_plano_conta_tit;
                                    dojo.byId("planoContasProsp").value = dataProspect.retorno.desc_plano_conta;
                                    dijit.byId("planoContasProsp").set("value", dataProspect.retorno.desc_plano_conta);
                                    dijit.byId("cbLocal").set("value", dataProspect.retorno.cd_local_movto);
                                    dijit.byId("cbLiquidacao").set("value", dataProspect.retorno.cd_tipo_liquidacao);
                                    if (hasValue(dataProspect.retorno.cd_plano_conta_tit))
                                        dijit.byId("limparPlanoContasProsp").set('disabled', false);


                                    if (hasValue(dijit.byId("cbMarketing").value == "")) {
                                        var midias = dataProspect.retorno.midias;

                                        if (indexValue(midias, "no_midia", "Teste de Classificação") != null && indexValue(midias, "no_midia", "Pré Matrícula online") !== null
                                            && (indexValue(midias, "no_midia", "Teste de Classificação") + 1) != dataProspect.retorno.cd_midia && (indexValue(midias, "no_midia", "Pré Matrícula online") + 1) != dataProspect.retorno.cd_midia) {
                                            midias.splice(indexValue(midias, "no_midia", "Teste de Classificação"), 1);
                                            midias.splice(indexValue(midias, "no_midia", "Pré Matrícula online"), 1);
                                            if (dijit.byId('cbMarketing').disabled == true) {
                                                dijit.byId("cbMarketing").set('disabled', false);
                                            }
                                        }
                                        
                                        loadSelect(midias, 'cbMarketing', 'cd_midia', 'no_midia');
                                        if (hasValue(dijit.byId("cbMarketing")))
                                            dijit.byId("cbMarketing").set("value", id);
                                    }


                                    if (hasValue(dijit.byId("operadora").value == "")) {
                                        loadSelect(dataProspect.retorno.operadoras, 'operadora', 'cd_operadora', 'no_operadora');
                                        if (hasValue(dijit.byId("operadora")))
                                            dijit.byId("operadora").set("value", id);
                                    }

                                    loadSexo(Memory);

                                    var dataEndereco;
                                    if (hasValue(dataProspect.retorno.endereco)) {
                                         dataEndereco = dataProspect.retorno.endereco;
                                    }
                                    //loadDiaSemana(Memory);
                                    if (hasValue(dataProspect.retorno.endereco)) {
                                        dojo.byId("cdEndereco").value = dataProspect.retorno.endereco.cd_endereco;
                                        setarRetornoEnderecoPrincipal(dataProspect.retorno.endereco, true, dataEndereco.num_cep, 'apresentadorMensagemPessoa');
                                        var compNumero = dijit.byId("numero");
                                        if (hasValue(dataEndereco.dc_num_endereco))
                                            compNumero.set("value", dataEndereco.dc_num_endereco);

                                        var compComplemento = dijit.byId("complemento");
                                        if (hasValue(dataEndereco.dc_compl_endereco))
                                            compComplemento.set("value", dataEndereco.dc_compl_endereco);

                                        var compEndereco = dijit.byId("tipoEndereco");
                                        if (hasValue(dataEndereco.cd_tipo_endereco))
                                            compEndereco.set("value", dataEndereco.cd_tipo_endereco);
                                    }

                                    //dataProspect.retorno.endereco
                                    if (cd_prospect > 0) montarGridProspectSites(cd_prospect, dataProspect.retorno);//loadFormEditacao(dataProspect.retorno);

                                    //keepValues(item, gridProspect, false);
                                    IncluirAlterar(0, 'divAlterarProspect', 'divIncluirProspect', 'divExcluirProspect', 'apresentadorMensagemProspect', 'divCancelarProspect', 'divLimparProspect');

                                    dijit.byId('dtaCadastro').set('disabled', true);
                                    dijit.byId("cadProspect").show();
                                    dijit.byId("gridFollowUp").update();
                                    dijit.byId("gridMotivoNao").update();
                                    pesquisarProspectsAtividadesProspect(item.cd_prospect);
                                } else
                                    apresentaMensagem('apresentadorMensagem', dataProspect);
                            }
                            catch (er) {
                                postGerarLog(er);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridProspect.startup();
                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisaProspect(true);
                    }
                }, "pesquisarProspect");
                decreaseBtn(document.getElementById("pesquisarProspect"), '32px');
                
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try{
                            var dataIni = minDate;
                            var dataFim = maxDate;
                            if (hasValue(dojo.byId("dtaIni").value))
                                dataIni = dojo.byId("dtaIni").value;

                            if (hasValue(dojo.byId("dtaFim").value))
                                dataFim = dojo.byId("dtaFim").value;
                            var statusProspect = dijit.byId('status').get('value');

                            require(["dojo/_base/xhr"], function (xhr) {
                                xhr.get({
                                    url: Endereco() + "/secretaria/GetUrlRelatorioProspect?" + getStrGridParameters('gridProspect') + "nome=" + dojo.byId("nome").value + "&email=" + dojo.byId("email").value + "&inicio=" + document.getElementById("inicioProspect").checked +
                                        "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&status=" + statusProspect + "&aluno=" + dijit.byId("ckAlunoProsp").checked +
                                        "&testeClassificacaoMatriculaOnline=" + dijit.byId('cbTesteClassificacaoMatriculaOnline').get('value'),
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    if(hasValue(data.retorno))
                                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data.retorno, '950px', '750px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioProspect");

                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        try {
                            limparEnderecoPrincipal();
                            configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", null, null, null);
                            IncluirAlterar(1, 'divAlterarProspect', 'divIncluirProspect', 'divExcluirProspect', 'apresentadorMensagemProspect', 'divCancelarProspect', 'divLimparProspect');
                            dijit.byId("tagFollowUp").set("open", false);
                            dijit.byId("tagMotivosNao").set("open", false);
                            limparCadPospect();
                            returnLocalMovimento();
                            returnDataProspect(0,true);

                            habilitaOperadora(dojo.byId("celular"));
                            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                            dijit.byId("gridFollowUp").setStore(dataStore);
                            dijit.byId("gridMotivoNao").setStore(dataStore);

                            loadSexo(Memory);
                            //loadDiaSemana(Memory);

                            dijit.byId("cadProspect").show();
                            dijit.byId("gridFollowUp").update();
                            dijit.byId("gridMotivoNao").update();

                            dojo.byId('testeClassificacao').style.display = 'none';
                            dijit.byId('testeClassificacao').set('open', false);
                            dojo.byId('matriculaOnline').style.display = 'none';
                            dijit.byId('matriculaOnline').set('open', false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoProspect");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        incluirProspect();
                    }
                }, "incluirProspect");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        var cdProspect = hasValue(dojo.byId('cd_prospect').value) ? dojo.byId('cd_prospect').value : 0;
                        returnDataProspect(cdProspect,true);
                       // keepValues(null, gridProspect, false);
                    }
                }, "cancelarProspect");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        clearForm('formProspect');
                        dijit.byId("cadProspect").hide();
                    }
                }, "fecharProspect");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        editarProspect();
                    }
                }, "alterarProspect");
                new Button({
                    label: "Excluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            deletarProspect();
                        });
                    }
                }, "deletetarProspect");
                new Button({
                    label: "Limpar",
                    iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparCadPospect();
                    }
                }, "limparProspect");

                dijit.byId("cep").on("change", function (e) {
                    try {
                        if (hasValue(e) && e != "_____-___")
                            pesquisarCepPessoa(e, ENDERECOPRINCIPAL, dojo.byId("descApresMsg").value);
                        else
                            configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridLogradouroFK")))
                                montargridPesquisaLogradouroFK(function () { abrirPesquisaLogradouroFK(); });
                            else
                                abrirPesquisaLogradouroFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesLogradouro");
                var pesLogradouro = document.getElementById('pesLogradouro');
                if (hasValue(pesLogradouro)) {
                    pesLogradouro.parentNode.style.minWidth = '18px';
                    pesLogradouro.parentNode.style.width = '18px';
                }

                registry.byId("limparEnderecoPrincipal").on("click", function (e) {
                    try {
                        limparEnderecoPrincipal();
                        configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", null, null, null);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridProspect.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridProspect.itensSelecionados, 'deletarProspect(itensSelecionados)'); }
                });

                menu.addChild(acaoRemover);

                var acaoAluno = new MenuItem({
                    label: "Aluno",
                    onClick: function () {
                        redirecionarAluno();
                    }
                });
                menu.addChild(acaoAluno);

                var acaoRecibo = new MenuItem({
                    label: "Recibo",
                    onClick: function () {
                        try{
                            var gridProspect = dijit.byId('gridProspect');
                            if (!hasValue(gridProspect.itensSelecionados) || gridProspect.itensSelecionados.length <= 0)
                                caixaDialogo(DIALOGO_AVISO, 'Selecione algum prospect para emitir o recibo.', null);
                            else if (gridProspect.itensSelecionados.length > 1)
                                caixaDialogo(DIALOGO_ERRO, 'Selecione somente um prospect para emitir o recibo.', null);
                            else {
                                apresentaMensagem('apresentadorMensagem', null);
                                var cdProspect = gridProspect.itensSelecionados[0].cd_prospect;
                                xhr.get({
                                    url: Endereco() + "/api/financeiro/getUrlRelatorioReciboProspect?cd_prospect=" + cdProspect,
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/relatorioReciboProspect?' + data, '765px', '771px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoRecibo);

                //var acaoProspectGerado = new MenuItem({
                //    label: "Enviar Promoção Prospect",
                //    onClick: function () {
                //        try {
                //            var gridProspect = dijit.byId('gridProspect');
                //            if (!hasValue(gridProspect.itensSelecionados) || gridProspect.itensSelecionados.length <= 0)
                //                caixaDialogo(DIALOGO_AVISO, 'Selecione algum prospect para emitir o recibo.', null);
                //            else if (gridProspect.itensSelecionados.length > 1)
                //                caixaDialogo(DIALOGO_ERRO, 'Selecione somente um prospect para emitir o recibo.', null);
                //            else {
                //                var dataHoje = new Date(2023, 0, 1);
                //                var dataProspect = new Date(gridProspect.itensSelecionados[0].dt_cadastramento);
                //                if (dojo.date.compare(dataProspect, dataHoje) < 0)
                //                    caixaDialogo(DIALOGO_ERRO, 'Selecione somente um prospect com data de cadastro a partir de Janeiro de 2023.', null);
                //                else {
                //                    apresentaMensagem('apresentadorMensagem', null);
                //                    var cdProspect = gridProspect.itensSelecionados[0].cd_prospect;
                //                    xhr.get({
                //                        url: Endereco() + "/api/Secretaria/getSendPromocaoProspect?cd_prospect=" + cdProspect,
                //                        preventCache: true,
                //                        handleAs: "json",
                //                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                //                    }).then(function (data) {
                //                        var mensagensWeb = new Array();
                //                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Promoção enviada com sucesso.");
                //                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                //                    },
                //                        function (error) {
                //                            apresentaMensagem('apresentadorMensagem', error);
                //                        });
                //                }
                //            }
                                
                            
                //        }
                //        catch (e) {
                //            postGerarLog(e);
                //        }
                //    }
                //});
                //menu.addChild(acaoProspectGerado);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dojo.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridProspect, 'todosItens', ['pesquisarProspect', 'relatorioProspect']);
                        pesquisaProspect(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridProspect', 'selecionadoProspect', 'cd_prospect', 'selecionaTodos', ['pesquisarProspect', 'relatorioProspect'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                //#region gridMotivoNao
                var dataMotivoNao = [];
                var gridMotivoNao = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: dataMotivoNao }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosMnao' style='display:none'/>", field: "selecionadoMnao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMnao },
                        { name: "Descrição", field: "dc_motivo_nao_matricula", width: "70%" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridMotivoNao");
                gridMotivoNao.startup();

                var menuMnao = new DropDownMenu({ style: "height: 25px" });

                var acaoEditarMnao = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_motivo_nao_matricula', dijit.byId("gridMotivoNao")); }
                });
                menuMnao.addChild(acaoEditarMnao);

                var buttonMnao = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasMnao",
                    dropDown: menuMnao,
                    id: "acoesRelacionadasMnao"
                });
                dojo.byId("linkAcoesMotivoNao").appendChild(buttonMnao.domNode);


                //#endregion

                //#region gridFollowUp
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                var gridFollowUp = new EnhancedGrid({
                    store: dataStore,
                    structure:
                      [
                        { name: "<input id='selecionaTodosFollowUp' style='display:none'/>", field: "selecionadoFollowUp", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFollowUp },
                        { name: "Data/Hora", field: "dta_follow_up", width: "25%" },
                        { name: "Atendente", field: "no_usuario", width: "15%" },
                        { name: "Assunto", field: "_dc_assunto", width: "55%"},
                        { name: "E-mail", field: "id_email_enviado", width: "10%", styles: "text-align: center;", formatter: formatCheckBoxEmailEnviado }
                      ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["9", "18", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "9",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 3,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridFollowUp");

                gridFollowUp.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        dijit.byId("gridFollowUp").itemSelecionado = item;

                        if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                            montaFollowUpFK(function () {
                                dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                                    configuraLayoutFollowUp(e);
                                });
                                populaFollowUpAluno(function () { preparaKeepValues(); });

                                montaFollowUpFKPersonalizadoAluno();
                            });
                        else
                            populaFollowUpAluno(function () { preparaKeepValues(); });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridFollowUp, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosFollowUp').type == 'text')
                            setTimeout("configuraCheckBox(false, 'nroOrdem',  'selecionadoProg', -1, 'selecionaTodosFollowUp', 'selecionaTodosFollowUp', 'gridFollowUp')", gridFollowUp.rowsPerPage * 3);
                    });
                });

                gridFollowUp.canSort = function (col) { return Math.abs(col) != 1; };
                gridFollowUp.startup();
                //dijit.byId("gridFollowUp").resize();

                criarOuCarregarCompFiltering("cbMotivoInatividade", [{ name: "Efetuou matrícula em escola/concorrente", id: "1" },
                                                                     { name: "Não deseja receber mala direta/e-mail MKT", id: "2" },
                                                                     { name: "Não tem mais interesse no curso", id: "3" }
                                                                    ], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');

                var menuFollowUp = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarFollowUp(); }
                });
                menuFollowUp.addChild(acaoEditar);

                var acaoEditarFollowUp = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(Memory, ObjectStore, 'nroOrdem', dijit.byId("gridFollowUp"));
                    }
                });
                menuFollowUp.addChild(acaoEditarFollowUp);

                var acaoEnviarEmailFollowUp = new MenuItem({
                    label: "Enviar E-mail",
                    onClick: function () {                        
                        setIdEmailEnviadoView();
                        if (!hasValue(dojo.byId("cd_prospect").value) && dojo.byId("cd_prospect").value == 0) {
                            incluirProspectQuandoEnviarEmail();
                        } else {
                            editarProspectQuandoEnviarEmail();
                        }
                    }
                });
                menuFollowUp.addChild(acaoEnviarEmailFollowUp);

                var buttonMnao = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasFollowUp",
                    dropDown: menuFollowUp,
                    id: "acoesRelacionadasFollowUp"
                });
                dojo.byId("linkAcoesFollowUp").appendChild(buttonMnao.domNode);

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        incluirFollowUp(Memory, ObjectStore);
                    }
                }, "incluirFollowUp");

                //#endregion

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemProspect',
                                    PROSPECT);
                            else
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, PROSPECT);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btplanoContas");

                var buttonFkArray = ['btplanoContas'];
                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId('planoContasProsp').value = '';
                        dojo.byId('cd_plano_contas_prosp').value = 0;
                        dijit.byId("limparPlanoContasProsp").set('disabled', true);
                    }
                }, "limparPlanoContasProsp");
                decreaseBtn(document.getElementById("limparPlanoContasProsp"), '40px');

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        limparPesquisaMtvNMatFK();
                        dijit.byId("proMotivoNMatricula").show();
                    }
                }, "incluirMotivoNMatricula");

                dijit.byId("proMotivoNMatricula").on("Show", function (e) {
                    dijit.byId("gridMotivoNMatricluaFK").update();
                });

                //returnDataProspect(0,false);
                dojo.byId("descApresMsg").value == 'apresentadorMensagemProspect';
                if (hasValue(dijit.byId("emailProspect")))
                    dijit.byId("emailProspect").on("blur", function (evt) {
                        if (trim(dojo.byId("emailProspect").value) != "")
                            ExistsEmailBase();
                    });
                registry.byId("celular").on("change", function (e) {
                    if (e) habilitaOperadora(dojo.byId('celular'));
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323041', '765px', '771px');
                        });
                }

                dijit.byId("dt_matricula_prosp").on("change", function (e) {

                });
                dijit.byId("valorMatProsp").on("change", function (e) {
                    try{
                        if (hasValue(e)) {
                            dijit.byId("dt_matricula_prosp").set("required", true);
                            verificaObrigPlanoConta();
                        }
                        else {
                            dijit.byId("dt_matricula_prosp").set("required", false);
                            dijit.byId("planoContasProsp").set("required", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('ativo').on("change", function (value) {
                    dijit.byId('cbMotivoInatividade').set("disabled", value);
                    if (value)
                        dijit.byId('cbMotivoInatividade').reset();
                });

                
                var gridPreMatricula = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Produto", field: "no_produto", width: "20%" },
                        { name: "Tipo", field: "dc_produto_online", width: "15%" },
                        { name: "ID", field: "id_teste_online", width: "80%" }
                    ],
                    canSort: false,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridPreMatricula");
                gridPreMatricula.canSort = function (col) { return false };
                gridPreMatricula.startup();


                
                var gridTeste = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Produto", field: "no_produto", width: "20%" },
                        { name: "Tipo", field: "dc_produto_online", width: "15%" },
                        { name: "ID", field: "id_teste_online", width: "10%" },
                        { name: "Acertos(%)", field: "pc_acerto_teste", width: "20%", styles: "text-align:center;" },
                        { name: "Fase", field: "dc_acerto_teste", width: "40%" }
                    ],
                    canSort: false,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridTeste");
                gridTeste.canSort = function (col) { return false };
                gridTeste.startup();

                adicionarAtalhoPesquisa(['nome', 'email', 'status', 'dtaIni', 'dtaFim'], 'pesquisarProspect', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function configuraLayoutFollowUp(desabilitado) {
    if (desabilitado) {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', true);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', true);
        dijit.byId('mensagemFollowUppartial').set('disabled', true);
        dijit.byId('ckLidoFollowUpFK').set('checked', true);
    }
    else {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', false);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', false);
        dijit.byId('mensagemFollowUppartial').set('disabled', false);
    }
}

function eventoEditarFollowUp() {
    try {
        var gridFollowUp = dijit.byId("gridFollowUp");
        var itensSelecionados = gridFollowUp.itensSelecionados;
        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            gridFollowUp.itemSelecionado = gridFollowUp.itensSelecionados[0];

            if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                montaFollowUpFK(function () {
                    dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                        configuraLayoutFollowUp(e);
                    });
                    populaFollowUpAluno(function () { preparaKeepValues(); });
                    montaFollowUpFKPersonalizadoAluno();
                });
            else
                populaFollowUpAluno(function () { preparaKeepValues(); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaFollowUpFKPersonalizadoAluno() {
    dijit.byId('cancelarFollowUpPartial').on('click', function () {
        populaFollowUpAluno(function () { preparaKeepValues(); });
    });
    dijit.byId('btnLimparFollowUpFK').on('click', function () {
        dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomeProspect').value;
        dijit.byId('btnAlunoProspectFK').set('disabled', true);
    });
    dijit.byId('btnFecharFollowUpPartial').on('click', function () {
        dijit.byId('cadFollowUp').hide();
    });
}

function preparaKeepValues() {
    IncluirAlterar(0, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial', 'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
    limparCadFollowUpPartial();
    findComponentesNovoFollowUpPartial(function () {
        dojo.ready(function () {
            dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomeProspect').value;
            dijit.byId('btnAlunoProspectFK').set('disabled', true);
            dijit.byId('ckResolvidoFollowUpFK').set('disabled', false);
            showP('lblLido', true);
            dijit.byId('ckLidoFollowUpFK').set('style', 'display:block');
            dijit.byId("cadFollowUp").show();
            keepValuesFollowUp();
        });
    });
}

function keepValuesFollowUp() {
    
    var compckLidoFollowUpFK = dijit.byId('ckLidoFollowUpFK');
    var item = dijit.byId("gridFollowUp").itemSelecionado
    debugger
    if (hasValue(item.dta_follow_up))
        dojo.byId('dtaCadastroFollowUpFK').value = item.dta_follow_up;
    
    dijit.byId('mensagemFollowUppartial').set('value', item.dc_assunto);
    msgAulaDemonstrativaAux = item.dc_assunto;
    
    dijit.byId('cadAcaoFollowUpFK').set('value', item.cd_acao_follow_up);

    if (hasValue(item.cd_usuario_destino)) {
	    dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = item.cd_usuario_destino;
        dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").set("value", item.no_usuario_destino);

        dijit.byId("limparCadProspectAlunoUsuarioDestinoFollowUpFK").set("disabled", false);
    }

    dojo.byId('dtaProxContatoFollowUpFK').value = item.dta_proximo_contato;
    dojo.byId('nroOrdem').value = item.ordem;
    dojo.byId('cd_follow_up_partial').value = item.cd_follow_up;
    if (hasValue(item.id_tipo_atendimento) && item.id_tipo_atendimento > 0)
        dijit.byId("cadTipoAtendimento").set("value", item.id_tipo_atendimento);
    compckLidoFollowUpFK._onChangeActive = false;
    compckLidoFollowUpFK.set("value", item.id_follow_lido);
    compckLidoFollowUpFK._onChangeActive = true;
    dijit.byId('ckResolvidoFollowUpFK').set('checked', item.id_follow_resolvido);

    if (hasValue(item.Turma)) {
        dojo.byId("cd_turma_pesquisa").value = item.Turma.cd_turma;
        dojo.byId("cbTurma").value = item.Turma.no_turma;
        dijit.byId('limparProTurmaFK').set("disabled", false);
    }
    configuraLayoutFollowUp(item.id_follow_resolvido);
}
//#endregion

//#region - redirecionarAluno
function redirecionarAluno() {
    try{
        var gridProspect = dijit.byId('gridProspect');

    if (!hasValue(gridProspect.itensSelecionados) || gridProspect.itensSelecionados.length <= 0 || !gridProspect.itensSelecionados[0].id_prospect_ativo)
        caixaDialogo(DIALOGO_ERRO, msgProspectAtivo, null);
    else
        if (!hasValue(gridProspect.itensSelecionados) || gridProspect.itensSelecionados.length > 1 || !gridProspect.itensSelecionados[0].id_prospect_ativo)
            caixaDialogo(DIALOGO_ERRO, msgProspectAtivo, null);
        else
            window.location = Endereco() + '/Secretaria/Aluno?cd_prospect=' + gridProspect.itensSelecionados[0].cd_prospect;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function abrirFKProspect() {
    try {
        limparPesquisaProspectFK();
        pesquisarProspectFK();
        dijit.byId("cadProspect").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirFKProspectFollowUpPartialPersonalizadoAluno() {
    try {
        montarFKProspect(function () {
            dijit.byId('tipoPesquisaFKProspect').set('value', PESQUISARALUNO);
            dijit.byId('tipoPesquisaFKProspect').set('disabled', true);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaFollowUpAluno(p_funcao) {
    try {
        //Personaliza a tela da FK de follow up para a tela de aluno:
        showP('trTipoFollowUpFK', false);
        dojo.byId('trInternoUserFollowUpFK').style.display = 'none';
        dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
        dojo.byId('trProspectAlunoFollowUpFK').style.display = '';
        dojo.byId('trEmailTelefoneProspectAluno').style.display = 'none';
        showP('trProximoContatoFollowUpFK', true);
        dojo.byId('trMasterFollowUp').style.display = 'none';
        dojo.byId('trResolvidoLidoFollowUp').style.display = '';
        dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", false);
        dijit.byId("descAlunoProspectFollowUpFK").set("required", true);
        dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", false);
        dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);

        dijit.byId('btnIncluirFollowUpPartial').set('label', "Incluir");
        dijit.byId('alterarFollowUpPartial').set('label', "Alterar");

        dijit.byId('panePesqGeralFollowUpFK').set('title', "Prospect");
        alterarVisibilidadeEscolas(false);

        var btnAlunoProspectFK = dijit.byId("btnAlunoProspectFK");
        if (hasValue(btnAlunoProspectFK.handler))
            btnAlunoProspectFK.handler.remove();
        btnAlunoProspectFK.handler = btnAlunoProspectFK.on("click", function (e) {
            abrirFKProspectFollowUpPartialPersonalizadoAluno();
        });

        var btnIncluirFollowUpPartial = dijit.byId("btnIncluirFollowUpPartial");
        if (hasValue(btnIncluirFollowUpPartial.handler))
            btnIncluirFollowUpPartial.handler.remove();
        btnIncluirFollowUpPartial.handler = btnIncluirFollowUpPartial.on("click", function () {
            incluirLinhaFollowUp();
        });

        var alterarFollowUpPartial = dijit.byId("alterarFollowUpPartial");
        if (hasValue(alterarFollowUpPartial.handler))
            alterarFollowUpPartial.handler.remove();
        alterarFollowUpPartial.handler = alterarFollowUpPartial.on("click", function () {
            alterarLinhaFollowUp();
        });

        var deleteFollowUpPartial = dijit.byId("deleteFollowUpPartial");
        if (hasValue(deleteFollowUpPartial.handler))
            deleteFollowUpPartial.handler.remove();
        deleteFollowUpPartial.handler = deleteFollowUpPartial.on("click", function () {
            removerLinhaFollowUp();
        });

        if (hasValue(p_funcao))
            p_funcao.call();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function removerLinhaFollowUp() {
    try {
        var gridFollowUp = dijit.byId("gridFollowUp");
        var storeFollowUp = gridFollowUp.store.objectStore.data;
        quickSortObj(storeFollowUp, 'nroOrdem');
        var indice = binaryObjSearch(storeFollowUp, "nroOrdem", gridFollowUp.itemSelecionado.nroOrdem);

        //Verifica se o usuário tem permissão para remover:
        if (!hasValue(gridFollowUp.itemSelecionado.no_usuario) || !hasValue(document.getElementById('nomeUsuario').innerText)
                || gridFollowUp.itemSelecionado.no_usuario.trim() != document.getElementById('nomeUsuario').innerText.trim()) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPermissaoAlterarFollowUp);

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPermissaoAlterarFollowUp);

            if (dijit.byId('cadFollowUp').open)
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            else
                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);

            return false;
        }
        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            //Remove o item da grade:
            storeFollowUp.splice(indice, 1);
            gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));
            dijit.byId("gridFollowUp").itemSelecionado = null;
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
        });
        //dijit.byId("gridFollowUp").update();
        dijit.byId('cadFollowUp').hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarLinhaFollowUp() {
    try {
        //Verifica se o campo de mensagem está preenchido:
        if (!hasValue(dijit.byId('mensagemFollowUppartial').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMensagemFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        if (dijit.byId('cadAcaoFollowUpFK').value == EnumAcaoFollowUp.AULADEMONSTRATIVA) {
            var mensagensWeb = new Array();

            if (dojo.byId("cd_turma_pesquisa").value == '' || dojo.byId("cd_turma_pesquisa").value == 0) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaFollowUpObrig);
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                return false;
            }

            if (!hasValue(dojo.byId('dtaProxContatoFollowUpFK').value)) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Obrigatório informar data do próximo contato.");
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
                return false;
            }
        }

        //Verifica se o usuário pode alterar:
        if (!hasValue(dijit.byId("gridFollowUp").itemSelecionado.no_usuario) || !hasValue(document.getElementById('nomeUsuario').innerText)
                || dijit.byId("gridFollowUp").itemSelecionado.no_usuario.trim() != document.getElementById('nomeUsuario').innerText.trim()) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPermissaoAlterarFollowUp);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            dojo.xhr.post({
                url: Endereco() + "/util/PostDataHoraCorrente",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    if (IsJsonString(data))
                        var dataCorrente = jQuery.parseJSON(data).retorno;
                    else
                        var dataCorrente = data.retorno;

                    var gridFollowUp = dijit.byId("gridFollowUp");
                    var dataFollowup = new Date(dataCorrente.dataIngles);
                    var storeFollowUp;

                    storeFollowUp = gridFollowUp.store.objectStore.data;
                    quickSortObj(storeFollowUp, 'nroOrdem');
                    var indice = binaryObjSearch(storeFollowUp, "nroOrdem", gridFollowUp.itemSelecionado.nroOrdem);
                    
					if (hasValue(storeFollowUp[indice])) {
						storeFollowUp[indice].cd_follow_up_partial = dojo.byId('cd_follow_up_partial').value;
						storeFollowUp[indice].no_usuario = document.getElementById('nomeUsuario').innerText;
						storeFollowUp[indice].dta_follow_up = dataCorrente.dataPortugues;
						storeFollowUp[indice].dt_follow_up = dataFollowup;
						storeFollowUp[indice].dc_assunto = dijit.byId("mensagemFollowUppartial").value;
						storeFollowUp[indice]._dc_assunto = hasValue(dijit.byId("mensagemFollowUppartial").value) && dijit.byId("mensagemFollowUppartial").value.indexOf('<') < 0
								&& dijit.byId("mensagemFollowUppartial").value.indexOf('>') < 0 ? dijit.byId("mensagemFollowUppartial").value : "...";
						storeFollowUp[indice].cd_acao_follow_up = dijit.byId('cadAcaoFollowUpFK').value;
						storeFollowUp[indice].dt_proximo_contato = dojo.byId('dtaProxContatoFollowUpFK').value;
						storeFollowUp[indice].dta_proximo_contato = dojo.byId('dtaProxContatoFollowUpFK').value;
						storeFollowUp[indice].nroOrdem = storeFollowUp[storeFollowUp.length - 1].nroOrdem + 1;
						storeFollowUp[indice].id_follow_lido = dijit.byId('ckLidoFollowUpFK').checked;
						storeFollowUp[indice].id_follow_resolvido = dijit.byId('ckResolvidoFollowUpFK').checked;
						storeFollowUp[indice].id_tipo_atendimento = hasValue(dijit.byId("cadTipoAtendimento").value) ? dijit.byId("cadTipoAtendimento").value : null;
						//storeFollowUp[indice].cd_turma = dojo.byId("cd_turma_pesquisa").value;
						storeFollowUp[indice].cd_turma = hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0 ? dojo.byId("cd_turma_pesquisa").value : null,
					    storeFollowUp[indice].Turma = {
					        cd_turma: hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0 ? dojo.byId("cd_turma_pesquisa").value : null,
					        no_turma: dojo.byId("cbTurma").value
					    };
                        storeFollowUp[indice].cd_usuario_destino = hasValue(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) > 0 ? parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) : null;
                        storeFollowUp[indice].no_usuario_destino = hasValue(dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value) ? dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value : null;
						storeFollowUp[indice].id_alterado = true;

						storeFollowUp.sort(function byOrdem(a, b) { return a.nroOrdem < b.nroOrdem; });
						gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));
                    }
                    //dijit.byId("gridFollowUp").update();
                    dijit.byId('cadFollowUp').hide();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirLinhaFollowUp() {
    try {
        //Verifica se o campo de mensagem está preenchido:
        if (!hasValue(dijit.byId('mensagemFollowUppartial').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMensagemFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        if (dijit.byId('cadAcaoFollowUpFK').value == EnumAcaoFollowUp.AULADEMONSTRATIVA &&
                (dojo.byId("cd_turma_pesquisa").value == '' || dojo.byId("cd_turma_pesquisa").value == 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        if (dijit.byId('cadAcaoFollowUpFK').value == EnumAcaoFollowUp.AULADEMONSTRATIVA &&
            !hasValue(dojo.byId('dtaProxContatoFollowUpFK').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Para o tipo 'Aula Demonstrativa' a data do próximo contato é obrigatória.");
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            dojo.xhr.post({
                url: Endereco() + "/util/PostDataHoraCorrente",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson()
            }).then(function (data) {
                try {
					if (hasValue(data) && data.toString().indexOf('<html>') >= 0) {
                        caixaDialogo(DIALOGO_ERRO, msgSessaoExpiradaMVC, 0, 0, 0);
                        return false;
                    }
                    var dataCorrente = jQuery.parseJSON(data).retorno;
                    var gridFollowUp = dijit.byId("gridFollowUp");
                    var ordem;
                    var dataFollowup = new Date(dataCorrente.dataIngles);
                    var storeFollowUp;

                    if (hasValue(gridFollowUp.store.objectStore.data) && gridFollowUp.store.objectStore.data.length > 0) {
                        storeFollowUp = gridFollowUp.store.objectStore.data;
                        ordem = gridFollowUp.store.objectStore.data[0].nroOrdem + 1;
                    }
                    else {
                        storeFollowUp = [];
                        ordem = 1;
                    }

                    var idFollowUp = geradorIdFollowUp(gridFollowUp);

                    var myNewItem = {
                        id: idFollowUp,
                        cd_follow_up_partial: dojo.byId('cd_follow_up_partial').value,
                        no_usuario: document.getElementById('nomeUsuario').innerText,
                        dta_follow_up: dataCorrente.dataPortugues,
                        dt_follow_up: dataFollowup,
                        dc_assunto: dijit.byId("mensagemFollowUppartial").value,
                        _dc_assunto: hasValue(dijit.byId("mensagemFollowUppartial").value) && dijit.byId("mensagemFollowUppartial").value.indexOf('<') < 0
                            && dijit.byId("mensagemFollowUppartial").value.indexOf('>') < 0 ? dijit.byId("mensagemFollowUppartial").value : "...",
                        cd_acao_follow_up: dijit.byId('cadAcaoFollowUpFK').value,
                        dt_proximo_contato: dojo.byId('dtaProxContatoFollowUpFK').value,
                        dta_proximo_contato: dojo.byId('dtaProxContatoFollowUpFK').value,
                        id_follow_lido: dijit.byId('ckLidoFollowUpFK').checked,
                        id_follow_resolvido: dijit.byId('ckResolvidoFollowUpFK').checked,
                        id_tipo_atendimento: hasValue(dijit.byId("cadTipoAtendimento").value) ? dijit.byId("cadTipoAtendimento").value : null,
                        nroOrdem: ordem,
                        id_email_enviado: false,
                        id_email_enviado_view: false,
                        cd_turma: hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0 ? dojo.byId("cd_turma_pesquisa").value : null,
                        Turma: {
                            cd_turma: hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0 ? dojo.byId("cd_turma_pesquisa").value : null,
                            no_turma: dojo.byId("cbTurma").value
                        },
                        cd_usuario_destino: hasValue(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) > 0 ? parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) : null,
	                    no_usuario_destino: hasValue(dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value) ? dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value : null
                    };
                    storeFollowUp.push(myNewItem);
                    storeFollowUp.sort(function byOrdem(a, b) { return a.nroOrdem < b.nroOrdem; });
                    gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));

                    dijit.byId('cadFollowUp').hide();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirFollowUp(Memory, ObjectStore) {
    try {
        showCarregando();
        if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial'))) {
            dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                configuraLayoutFollowUp(e);
            });
            montaFollowUpFK(function () {
                populaFollowUpAluno(novoFollowUpPartial(function () {
                    dijit.byId('ckResolvidoFollowUpFK').set('disabled', true);
                    configuraLayoutFollowUp(false);
                    showP('lblLido', false);
                    dijit.byId('ckLidoFollowUpFK').set('style', 'display:none');
                    dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomeProspect').value;
                    dijit.byId('btnAlunoProspectFK').set('disabled', true);
                    showCarregando();
                }));
                montaFollowUpFKPersonalizadoAluno();
            });
        }
        else {
            populaFollowUpAluno(novoFollowUpPartial(function () {
                dijit.byId('ckResolvidoFollowUpFK').set('disabled', true);
                showP('lblLido', false);

                dijit.byId('ckLidoFollowUpFK').set('style', 'display:none');
                dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomeProspect').value;
                dijit.byId('btnAlunoProspectFK').set('disabled', true);
                showCarregando();
            }));
            configuraLayoutFollowUp(false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarMtvNMatFK() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try{
                var gridMotivoNao = dijit.byId("gridMotivoNao");
                var gridMotivoNMatricluaFK = dijit.byId("gridMotivoNMatricluaFK");
                if (hasValue(jQuery.parseJSON(dojo.byId("ehSelectGradeMNMatriculaFK").value)) && jQuery.parseJSON(dojo.byId("ehSelectGradeMNMatriculaFK").value)) {
                    var dataMNMatricula = dijit.byId("gridMotivoNMatricluaFK").selection.getSelected()[0];
                    insertObjSort(gridMotivoNao.store.objectStore.data, "cd_motivo_nao_matricula", {cd_motivo_nao_matricula: dataMNMatricula.cd_motivo_nao_matricula, dc_motivo_nao_matricula: dataMNMatricula.dc_motivo_nao_matricula });
                    gridMotivoNao.setStore(new ObjectStore({ objectStore: new Memory({ data: gridMotivoNao.store.objectStore.data }) }));
                } else {
                    if (!hasValue(gridMotivoNMatricluaFK.itensSelecionados) || gridMotivoNMatricluaFK.itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else {
                        var storeGridCurso = (hasValue(gridMotivoNao) && hasValue(gridMotivoNao.store.objectStore.data)) ? gridMotivoNao.store.objectStore.data : [];
                        if (storeGridCurso != null && storeGridCurso.length > 0) {
                            $.each(gridMotivoNMatricluaFK.itensSelecionados, function (idx, value) {
                                //gridMotivoNao.store.newItem(value);
                                insertObjSort(gridMotivoNao.store.objectStore.data, "cd_motivo_nao_matricula", { cd_motivo_nao_matricula: value.cd_motivo_nao_matricula, dc_motivo_nao_matricula: value.dc_motivo_nao_matricula });
                            });
                            gridMotivoNao.setStore(new ObjectStore({ objectStore: new Memory({ data: gridMotivoNao.store.objectStore.data }) }));
                            //gridCursoAval.update();
                        } else {
                            var dados = [];
                            $.each(gridMotivoNMatricluaFK.itensSelecionados, function (index, val) {
                                insertObjSort(dados, "cd_motivo_nao_matricula", {cd_motivo_nao_matricula: val.cd_motivo_nao_matricula, dc_motivo_nao_matricula: val.dc_motivo_nao_matricula });
                            });
                            //slice(0)
                            gridMotivoNao.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                        }
                    }
                }
                dojo.byId("ehSelectGradeMNMatriculaFK").value = false;
                dijit.byId("proMotivoNMatricula").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}
//#endregion

//#region - crud

function ExistsEmailBase() {
    var mensagem = ' já está com este e-mail, deseja cadastrá-la como Prospect? Os dados desta pessoa serão preenchidos automaticamente.';
    try {
        var cdProspect = hasValue(dojo.byId('cd_prospect').value) ? dojo.byId('cd_prospect').value : 0;
        var mensagensWeb = new Array();
        if ($("#emailProspect").val()) {
            if (hasValue(dojo.byId('dc_email_escola').value) && dojo.byId('dc_email_escola').value != $("#emailProspect").val())
                dojo.xhr.get({
                    url: Endereco() + "/secretaria/getExistsProspectEmail?email=" + $("#emailProspect").val() + "&cdProspect=" + cdProspect,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {

                        if (hasValue(data.retorno) && data.retorno.cd_prospect != null && data.retorno.cd_prospect > 0) {
                            if (hasValue(data.retorno.cd_pessoa_escola)) {
                                var dados = data.retorno;
                                if (hasValue(data.retorno.pessoaFisica))
                                    dados = data.retorno.pessoaFisica;
                                caixaDialogo(DIALOGO_CONFIRMAR, data.MensagensWeb[0].mensagem, function executaRetorno() {
                                    setValuePessoaFisica(dados);
                                });
                                return false;
                            } else {
                                apresentaMensagem("apresentadorMensagemProspect", null);
                                //mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, "Prospect  " + data.retorno.no_pessoa + msgInfoProspectCadastrado);
                                caixaDialogo(DIALOGO_CONFIRMAR, "Prospect  " + data.retorno.no_pessoa + msgInfoProspectCadastrado, function executaRetorno() {
                                    limparCadPospect();
                                    returnDataProspect(data.retorno.cd_prospect, true);
                                });
                                return false;
                            }
                        }

                         if (hasValue(data.retorno) && data.retorno.cd_pessoa != null && data.retorno.cd_pessoa > 0) {
                            
                             var email = dojo.byId("emailProspect").value;

                             apresentaMensagem("apresentadorMensagemProspect", mensagensWeb);
                             //mensagensWeb[0] = new MensagensWeb(DIALOGO_AVISO, msgEmailExist + " " + data.retorno.no_pessoa);
                             caixaDialogo(DIALOGO_CONFIRMAR, data.retorno.no_pessoa + mensagem, function executaRetorno() {
                                 limparCadPospect();
                                 setValuePessoaFisica(data.retorno);
                             }, function () {
                                 dojo.byId("emailProspect").value = email;
                             });
                        } else {
                            apresentaMensagem(dojo.byId("descApresMsg").value, null);
                            if (hasValue(data.retorno) && data.retorno.cd_aluno != null && data.retorno.cd_aluno > 0) {
                                limparCadPospect();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Aluno " + data.retorno.no_pessoa + " " + msgEmailExist);
                            } if (hasValue(data.retorno) && data.retorno.cd_prospect != null && data.retorno.cd_prospect > 0) {
                                limparCadPospect();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Prospect " + data.retorno.no_pessoa + " " + msgEmailExist);
                            }
                            apresentaMensagem("apresentadorMensagemProspect", mensagensWeb);
                        }
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemProspect', error);
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValuePessoaFisica(dataPessoa) {
    try{
        getLimpar('#formProspect');
        clearForm('formProspect');
        //dados do usuário
        returnDataProspect(0,true);
        dojo.byId("nomeProspect").value = dataPessoa.no_pessoa;
        dojo.byId("emailProspect").value = dataPessoa.email;
        if (hasValue(dojo.byId('cbSexo').value, true)) {
            hasValue(dataPessoa.nm_sexo) ? dijit.byId("cbSexo").set("value", dataPessoa.nm_sexo) : 0;
        }
        if (hasValue(dataPessoa.TelefonePessoa)) {
            for (var i = 0; i < dataPessoa.TelefonePessoa.length; i++) {
                var tipo = dataPessoa.TelefonePessoa[i].cd_tipo_telefone;
                if (tipo == TELEFONE) dojo.byId('telefone').value = dataPessoa.TelefonePessoa[i].dc_fone_mail;
                if (tipo == CELULAR) {
                    dojo.byId('celular').value = dataPessoa.TelefonePessoa[i].dc_fone_mail;
                    if (hasValue(dojo.byId('operadora').value, true)) {
                        hasValue(dataPessoa.TelefonePessoa[i].cd_operadora) ? dijit.byId("operadora").set("value", dataPessoa.TelefonePessoa[i].cd_operadora) : 0;
                    }
                }
                if (tipo == EMAIL) dojo.byId("emailProspect").value = dataPessoa.TelefonePessoa[i].dc_fone_mail;
            }
        }
        habilitaOperadora(dojo.byId("celular"));
        dojo.addOnLoad(function () {
            dijit.byId('dtaCadastro').attr("value", new Date(ano, mes, dia));
        });
        dojo.byId('cd_pessoa_fisica').value = dataPessoa.cd_pessoa;
        dojo.byId('dtaNasci').value = dataPessoa.dt_nascimento_prospect;

        var dataEnderecoPessoa;
        if (hasValue(dataPessoa.enderecoUI)) {
            dataEnderecoPessoa = dataPessoa.enderecoUI;
        }
        if (hasValue(dataPessoa.enderecoUI)) {
            setarRetornoEnderecoPrincipal(dataPessoa.enderecoUI, true, dataEnderecoPessoa.num_cep, 'apresentadorMensagemPessoa');
            var compNumero = dijit.byId("numero");
            if (hasValue(dataEnderecoPessoa.dc_num_endereco))
                compNumero.set("value", dataEnderecoPessoa.dc_num_endereco);

            var compComplemento = dijit.byId("complemento");
            if (hasValue(dataEnderecoPessoa.dc_compl_endereco))
                compComplemento.set("value", dataEnderecoPessoa.dc_compl_endereco);

            var compEndereco = dijit.byId("tipoEndereco");
            if (hasValue(dataEnderecoPessoa.cd_tipo_endereco))
                compEndereco.set("value", dataEnderecoPessoa.cd_tipo_endereco);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ProspectSearchUI(isPesquisa) {
    try{
        if (isPesquisa) {
            this.emailGrid = hasValue(dojo.byId('email').value) ? dojo.byId('email').value : "";
            this.nomeGrid = hasValue(dojo.byId('nome').value) ? dojo.byId('nome').value : "";
            this.inicioGrid = document.getElementById("inicioProspect").checked;
        }
        var produtos = dijit.byId('cbProduto').get('value');
        var listaFollowUp = [];
        var grid = dijit.byId("gridFollowUp");
        for (var i = 0; i < grid.store.objectStore.data.length; i++) {
            if (hasValue(grid) && hasValue(grid.store.objectStore)) {
                var follow = {
                    cd_usuario: grid.store.objectStore.data[i].cd_usuario,
                    cd_follow_up: grid.store.objectStore.data[i].cd_follow_up,
                    cd_prospect: hasValue(dojo.byId('cd_prospect').value) ? dojo.byId('cd_prospect').value : 0,
                    cd_aluno: null,
                    dt_proximo_contato: grid.store.objectStore.data[i].dt_proximo_contato,
                    cd_acao_follow_up: grid.store.objectStore.data[i].cd_acao_follow_up,
                    dt_follow_up: grid.store.objectStore.data[i].dt_follow_up,
                    dc_assunto: grid.store.objectStore.data[i].dc_assunto,
                    cd_aluno_follow_up: grid.store.objectStore.data[i].cd_aluno_follow_up,
                    id_follow_lido: grid.store.objectStore.data[i].id_follow_lido,
                    id_follow_resolvido: grid.store.objectStore.data[i].id_follow_resolvido,
                    id_alterado: grid.store.objectStore.data[i].id_alterado,
                    id_tipo_atendimento: grid.store.objectStore.data[i].id_tipo_atendimento,
                    id_email_enviado_view: grid.store.objectStore.data[i].id_email_enviado_view,
                    cd_usuario_destino: grid.store.objectStore.data[i].cd_usuario_destino,
                    no_usuario_destino: grid.store.objectStore.data[i].no_usuario_destino,
                    cd_turma: hasValue(dojo.byId("cd_turma_pesquisa").value) && dojo.byId("cd_turma_pesquisa").value > 0 ? dojo.byId("cd_turma_pesquisa").value : null
                };
                listaFollowUp.push(follow);
            }
        }
        var listaMotivoNao = [];
        var gridMotivoNao = dijit.byId("gridMotivoNao");
        if (hasValue(gridMotivoNao.store.objectStore.data) && gridMotivoNao.store.objectStore.data.length > 0) {
            gridMotivoNao.store.save();
            var grid = gridMotivoNao.store.objectStore.data;
            for (var i = 0; i < grid.length; i++) {
                listaMotivoNao.push({ cd_motivo_nao_matricula: grid[i].cd_motivo_nao_matricula, cd_prospect: hasValue(dojo.byId('cd_prospect').value) ? dojo.byId('cd_prospect').value : 0, cd_prospect_motivo_nao_matricula: grid[i].cd_prospect_motivo_nao_matricula })
            }
        }
        this.cd_prospect = hasValue(dojo.byId('cd_prospect').value) ? dojo.byId('cd_prospect').value : 0;
        this.no_pessoa = hasValue(dojo.byId('nomeProspect').value) ? $.trim(dojo.byId('nomeProspect').value.replace(/ {2,}/g, ' ')) : "";
        this.cd_pessoa_fisica = hasValue(dojo.byId('cd_pessoa_fisica').value) ? dojo.byId('cd_pessoa_fisica').value : 0;
        this.email = hasValue(dojo.byId('emailProspect').value) ? dojo.byId('emailProspect').value : "";
        this.telefone = hasValue(dojo.byId("telefone").value) ? dojo.byId("telefone").value : "";
        this.celular = hasValue(dojo.byId('celular').value) ? dojo.byId('celular').value : "";
        this.dt_cadastramento = hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        this.dt_nascimento_prospect = hasValue(dojo.byId("dtaNasci").value) ? dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        this.nm_sexo = hasValue(dijit.byId('cbSexo').value) ? dijit.byId('cbSexo').value : null;
        this.cd_midia = hasValue(dijit.byId("cbMarketing").value) ? dijit.byId("cbMarketing").value : null;
        this.no_midia = hasValue(dojo.byId("cbMarketing").value) ? dojo.byId("cbMarketing").value : null
        this.cd_usuario = hasValue(dojo.byId("cdAtendente").value) ? dojo.byId("cdAtendente").value : 0;
        this.no_usuario = hasValue(dojo.byId("atendente").value) ? dojo.byId("atendente").value : 0;
        this.produtos = getProdutos(produtos);
        this.periodos = getPeriodos(dijit.byId('cbPeriodo').value);
        this.dias = getDias(dijit.byId('cbDiaSemana').value);
        this.listaFollowUp = listaFollowUp;
        this.cd_operadora = hasValue(dijit.byId('operadora').value) ? dijit.byId('operadora').value : null;
        this.id_dia_semana = 1;
        
        this.id_prospect_ativo = dijit.byId('ativo').checked;
        this.listaMotivos = listaMotivoNao;//[{ cd_motivo_nao_matricula: 6, dc_motivo_nao_matricula: 'ddd', id_motivo_nao_matricula_ativo: true }];
        this.no_escola = dojo.byId('no_escola').value;
        this.id_faixa_etaria = dijit.byId('id_faixa_etaria_1').checked ? FAIXA_ETARIA_INFANTIL : (dijit.byId('id_faixa_etaria_2').checked ? FAIXA_ETARIA_INFANTIL_ADULTO : (dijit.byId('id_faixa_etaria_3').checked ? FAIXA_ETARIA_ADULTO : null));
        this.cd_motivo_inativo = hasValue(dijit.byId("cbMotivoInatividade").value) ? dijit.byId("cbMotivoInatividade").value : null;

        //Dado Endereço:
        this.endereco = {
                cd_endereco: dojo.byId("cdEndereco").value,
                cd_loc_cidade: hasValue(dijit.byId("cidade").get("value")) ? dijit.byId("cidade").get("value") : 0,
                cd_loc_estado: hasValue(dijit.byId("estado").get("value")) ? dijit.byId("estado").get("value") : 0,
                cd_loc_pais: 1,
                cd_tipo_endereco: hasValue(dijit.byId("tipoEndereco").get("value")) ? dijit.byId("tipoEndereco").get("value") : 0,
                cd_operadora: hasValue(dijit.byId("operadora").get("value")) ? dijit.byId("operadora").get("value") : 0,
                cd_tipo_logradouro: hasValue(dijit.byId("logradouro").get("value")) ? dijit.byId("logradouro").get('value') : null,
                cd_loc_bairro: hasValue(dijit.byId('bairro').get("value")) ? dijit.byId('bairro').get("value") : null,
                cd_loc_logradouro: hasValue(dojo.byId('codRua').value) ? dojo.byId('codRua').value : null,
                dc_compl_endereco: hasValue(dijit.byId("complemento").get("value")) ? dijit.byId("complemento").get("value") : '',
                dc_num_cep: hasValue(dojo.byId("cep").value) ? dojo.byId("cep").value : '',
                dc_num_endereco: hasValue(dojo.byId("numero").value) ? dojo.byId("numero").value : ''
        }

        //Dados da pré-matricula:
        this.dt_matricula = hasValue(dojo.byId("dt_matricula_prosp").value) ? dojo.date.locale.parse(dojo.byId("dt_matricula_prosp").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        this.vl_matricula = dijit.byId('valorMatProsp').value;
        this.gerar_baixa = dijit.byId('ckBaixa').checked;
        this.cd_local_movto = dijit.byId('cbLocal').value
        this.cd_plano_conta_tit = dojo.byId('cd_plano_contas_prosp').value;
        this.cd_tipo_liquidacao = dijit.byId('cbLiquidacao').value;
        this.parametro = new Parametro();

    }
    catch (e) {
        postGerarLog(e);
    }
}

function Parametro() {
    try{
        this.cd_local_movto = dojo.byId('cd_local_movto_parm').value;
        this.id_alterar_venc_final_semana = dojo.byId('id_alterar_venc_final_semana').value;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//trasforma em um objeto para ser reconhedido na classe UI
function getProdutos(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_produto: data[i] });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}
function getPeriodos(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ id_periodo: data[i] });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getDias(data) {
    try {
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ id_dia_semana: data[i] });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaProspect(limparItens) {
    try{
        var variaveisPesquisa = new ProspectSearchUI(true);
        var dataIni = '';
        var dataFim = '';
        var checkInicio = false;

        if (hasValue(dojo.byId("dtaIni").value))
            dataIni = dojo.byId("dtaIni").value;

        if (hasValue(dojo.byId("dtaFim").value))
            dataFim = dojo.byId("dtaFim").value;

        if (hasValue(variaveisPesquisa.inicioGrid))
            checkInicio = variaveisPesquisa.inicioGrid;

        var statusProspect = dijit.byId('status').get('value');

        require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/secretaria/getProspectSearch?nome=" + variaveisPesquisa.nomeGrid + "&inicio=" + checkInicio + "&email=" + variaveisPesquisa.emailGrid + "&dataIni=" + dataIni + "&dataFim=" + dataFim + "&status=" + statusProspect + "&aluno=" + dijit.byId("ckAlunoProsp").checked + "&testeClassificacaoMatriculaOnline=" + dijit.byId('cbTesteClassificacaoMatriculaOnline').get('value'),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_prospect"
                    }), Memory({ idProperty: "cd_prospect" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridProspect = dijit.byId("gridProspect");
                if (limparItens) {
                    gridProspect.itensSelecionados = [];
                }
                gridProspect.noDataMessage = msgNotRegEnc;
                gridProspect.setStore(dataStore);
            }
            catch (er) {
                postGerarLog(er);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirProspectQuandoEnviarEmail() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemProspect', null);

    var ProspectInsert = new ProspectSearchUI(false);
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, window) {
        if (!validarProspectCadastro(window))
            return false;
        showCarregando();
        xhr.post(Endereco() + "/secretaria/postInsertProspectQuandoEnviarEmail", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(ProspectInsert)
        }).then(function (data) {
            try {
                showCarregando();
                
                if (!hasValue(data.erro)) {
                    dijit.byId("gridFollowUp").itensSelecionados = [];
                    setIdEmailEnviadoView();
                    dojo.byId("cd_prospect").value = data.retorno.cd_prospect;
                    returnDataProspect(data.retorno.cd_prospect, true);

                } else
                    apresentaMensagem('apresentadorMensagemProspect', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProspect', error.response.data);
        });
    });
}

function incluirProspect() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemProspect', null);

    

    var ProspectInsert = new ProspectSearchUI(false);
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, window) {
        if (!validarProspectCadastro(window))
            return false;
        showCarregando();
        xhr.post(Endereco() + "/secretaria/postInsertProspect", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(ProspectInsert)
        }).then(function (data) {
            try {
                showCarregando();
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridProspect';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadProspect").hide();
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    insertObjSort(grid.itensSelecionados, "cd_prospect", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoProspect', 'cd_prospect', 'selecionaTodosProspect', ['pesquisarProspect', 'relatorioProspect'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_prospect");
                } else
                    apresentaMensagem('apresentadorMensagemProspect', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProspect', error.response.data);
        });
    });
}

function editarProspectQuandoEnviarEmail() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemProspect', null);

    var ProspectEdit = new ProspectSearchUI(false);
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, window) {
        if (!validarProspectCadastro(window)) {
            return false;
        }
        showCarregando();
        xhr.post(Endereco() + "/secretaria/postAlterarProspectQuandoEnviarEmail", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(ProspectEdit)
        }).then(function (data) {
            try {
                showCarregando();
                if (!hasValue(data.erro)) {
                    
                    dijit.byId("gridFollowUp").itensSelecionados = [];
                    setIdEmailEnviadoView();
                    dojo.byId("cd_prospect").value = data.retorno.cd_prospect;
                    returnDataProspect(data.retorno.cd_prospect, true);
          
                } else
                    apresentaMensagem('apresentadorMensagemProspect', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProspect', error.response.data);
        });
    });
}

function editarProspect() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemProspect', null);

    var ProspectEdit = new ProspectSearchUI(false);
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, window) {
        if (!validarProspectCadastro(window)) {
            return false;
        }
        showCarregando();
        xhr.post(Endereco() + "/secretaria/postAlterarProspect", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(ProspectEdit)
        }).then(function (data) {
            try {
                showCarregando(); 
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridProspect';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadProspect").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    removeObjSort(grid.itensSelecionados, "cd_prospect", dom.byId("cd_prospect").value);
                    insertObjSort(grid.itensSelecionados, "cd_prospect", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoProspect', 'cd_prospect', 'selecionaTodosProspect', ['pesquisarProspect', 'relatorioProspect'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_prospect");
                } else
                    apresentaMensagem('apresentadorMensagemProspect', data);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProspect', error.response.data);
        });
    });
}

function deletarProspect(itensSelecionados) {
    showCarregando();
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_prospect').value != 0)
                itensSelecionados = [{
                    cd_prospect: dom.byId("cd_prospect").value,
                    cd_pessoa_fisica: dom.byId("cd_pessoa_fisica").value
                }];
        xhr.post({
            url: Endereco() + "/api/secretaria/PostDeleteProspect",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens");
                apresentaMensagem('apresentadorMensagem', data);
                data = jQuery.parseJSON(data).retorno;
                dijit.byId("cadProspect").hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridProspect').itensSelecionados, "cd_prospect", itensSelecionados[r].cd_prospect);
                pesquisaProspect(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarProspect").set('disabled', false);
                dijit.byId("relatorioProspect").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadProspect").style.display)) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemProspect', error);
            }
            else {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            }
        });
    });
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparEnderecoPrincipal();
            getLimpar('#formProspect');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(null, dijit.byId('gridProspect'), true);
            dijit.byId("cadProspect").show();
            IncluirAlterar(0, 'divAlterarProspect', 'divIncluirProspect', 'divExcluirProspect', 'apresentadorMensagemProspect', 'divCancelarProspect', 'divLimparProspect');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function returnLocalMovimento(cdLocal, cdTpLiq, isCadastro) {
    cdLocal = cdLocal > 0 ? cdLocal : 0;
    isCadastro = isCadastro == null ? true : false;
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getLocalMovtoETpLiquidacao?cdLocalMovto=" + cdLocal + "&cdTpLiq=" + cdTpLiq + "&isCadastro=" + isCadastro,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                data = $.parseJSON(data);
                loadLocalMovimento(data.locaMovto);
                loadTipoLiquidacao(data.tipoLiquidacao);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}

function loadLocalMovimento(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbMotivo = dijit.byId('cbLocal');
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_local_movto, name: value.nomeLocal });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbMotivo.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTipoLiquidacao(registros) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbTipoLiquidacao = dijit.byId('cbLiquidacao');
            Array.forEach(registros, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_liquidacao, name: value.dc_tipo_liquidacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipoLiquidacao.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try{
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaObrigPlanoConta() {
    showCarregando();
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/escola/getParametrosMovimento",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                if (data.retorno.id_requer_plano_contas_mov)
                    dijit.byId("planoContasProsp").set("required", true);
                else
                    dijit.byId("planoContasProsp").set("required", false);
                dojo.byId('cd_local_movto_parm').value = data.retorno.cd_local_movto;
                dojo.byId('id_alterar_venc_final_semana').value = data.retorno.id_alterar_venc_final_semana;
                if (data.retorno.cd_plano_conta_tax > 0 && dojo.byId("cd_plano_contas_prosp").value <= 0) {
                    dijit.byId("planoContasProsp").set("value", data.retorno.desc_plano_conta_tax);
                    dojo.byId("cd_plano_contas_prosp").value = data.retorno.cd_plano_conta_tax;
                    dijit.byId("limparPlanoContasProsp").set('disabled', false);
                }
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}


function abrirPesquisaLogradouroFK(data) {
    try {
        limparFiltrosLogradouroFKEnredecoPessoa();
        dojo.byId('setValuePesquisaLogradouroFK').value = PESCADENDERECOPESSOA;
        var compEstado = dijit.byId("estado");
        var compCidade = dijit.byId("cidade");
        var compBairro = dijit.byId("bairro");
        var compNumero = dijit.byId("numeroEndereco");
        var compEstadoLogFK = dijit.byId("estadFKLogradouro");
        var cd_estado = 0;
        var cd_cidade = 0;
        var cd_bairro = 0;
        var nome_cidade = "";
        if (data != null) {
            cd_estado = data.cd_loc_estado;
            cd_cidade = data.cd_loc_cidade;
            nome_cidade = data.noLocCidade;
            //cd_bairro = data.cd_loc_bairro;
        } else {
            cd_estado = compEstado.value;
            cd_cidade = compCidade.value;
            //cd_bairro = compBairro.value;
            nome_cidade = compCidade.displayedValue;
        }
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = 1;

        if (cd_estado > 0) {
            compEstadoLogFK._onChangeActive = false;
            compEstadoLogFK.set("value", cd_estado);
            compEstadoLogFK._onChangeActive = true;
        }
        compNumero.reset();
        //compBairro.reset();
        if (cd_cidade > 0) {
            dojo.byId("cd_cidade_pesq_logradouroFK").value = cd_cidade;
            dijit.byId("pesNomCidadeLogradouroFK").set("value", nome_cidade);
            dojo.byId("pesNomCidadeLogradouroFK").value = nome_cidade;
            dijit.byId("limparCidadeLogradouroPesFK").set("disabled", false);
            carregarBairroPorCidadeFK(cd_cidade, cd_bairro, function () {
                pesquisarLogradourofk(true);
                dijit.byId("proLogradouroFK").show();
                dijit.byId('gridLogradouroFK').update();
            });
            return false;
        }
        pesquisarLogradourofk(true);
        dijit.byId("proLogradouroFK").show();
        dijit.byId('gridLogradouroFK').update();
    }
    catch (e) {
        postGerarLog(e);
    }
}



function retornarLogradouroFK() {
    try {
        var valido = true;
        var gridLogradouroFK = dijit.byId("gridLogradouroFK");
        if (!hasValue(gridLogradouroFK.itensSelecionados) || gridLogradouroFK.itensSelecionados.length <= 0 || gridLogradouroFK.itensSelecionados.length > 1) {
            if (gridLogradouroFK.itensSelecionados != null && gridLogradouroFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridLogradouroFK.itensSelecionados != null && gridLogradouroFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var itemSelecionado = gridLogradouroFK.itensSelecionados[0];
            

                if (!hasValue(itemSelecionado.cd_localidade)) {
                    var data = {
                        tipoLogradouro: itemSelecionado.no_tipo_logradouro,
                        noLocRua: itemSelecionado.no_localidade,
                        noLocCidade: itemSelecionado.no_localidade_cidade,
                        noLocBairro: itemSelecionado.no_localidade_bairro,
                        noLocEstado: itemSelecionado.no_localidade_estado,
                        descTipoLogradouro: itemSelecionado.no_tipo_logradouro,
                        num_cep: itemSelecionado.dc_num_cep
                    };
                    pesquisarEnderecoOrCadastrar(data, ENDERECOPRINCIPAL, 'apresentadorMensagemPessoa', false);
                }
                else {
                    if (itemSelecionado.cd_localidade == 0 && itemSelecionado.nm_cep == "")
                        throw new Exception("Registro inconsistente. Endereço : " + JSON.stringify(itemSelecionado));
                    carregarEndereoByLogradouroOrCep(itemSelecionado.cd_localidade, "");
                }
            
        }
        if (!valido)
            return false;
        dijit.byId("proLogradouroFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarEndereoByLogradouroOrCep(cd_logradouro, nm_cep) {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/localidade/getEnderecoByCdLogradouro?cd_logradouro=" + cd_logradouro + "&nm_cep=" + nm_cep,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data).retorno;
                    configurarLayoutEndereco(LAYOUTPESQUISALOGRADOURO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
                    setValuesEndereco(data, "estado", "cidade", "bairro", "rua", "codRua", "cep", "logradouro");
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemPessoa", error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFiltrosLogradouroFKEnredecoPessoa() {
    limpaCidadeLogradouroFK();
    var compEstadoLogFK = dijit.byId("estadFKLogradouro");
    compEstadoLogFK._onChangeActive = false;
    compEstadoLogFK.reset();
    compEstadoLogFK._onChangeActive = true;
    dijit.byId("outrosLogradouros").reset();
    dojo.byId("descricaoLogradouroFK").value = "";
    showP('imgIntDescricaoPesqLogradouro', false);
    //showP('trNumeroEndereco', false);
    var gridLogradouroFK = dijit.byId("gridLogradouroFK");
    if (hasValue(gridLogradouroFK) && hasValue(gridLogradouroFK.itensSelecionados))
        gridLogradouroFK.itensSelecionados = [];
    gridLogradouroFK.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory(null) }));
    gridLogradouroFK.update();

}


function setarRetornoEnderecoPrincipal(data, pesquisarWebServiceCep, nm_cep, idMsg) {
    try {
        var compCep = dijit.byId("cep");
        if (data != null) {
            configurarLayoutEndereco(LAYOUTPESQUISACEP, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
            setValuesEndereco(data, "estado", "cidade", "bairro", "rua", "codRua", "cep", "logradouro");
        } else {
            caixaDialogo(DIALOGO_AVISO, msgInfoCEPNaoEncontrado);
            configurarLayoutEndereco(LAYOUTPADRAO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
        }
        compCep._onChangeActive = false;
        compCep.set("value", compCep.value);
        compCep._onChangeActive = true;
    }
    catch (e) {
        postGerarLog(e);
    }

    
}


function limparEnderecoPrincipal() {
    try {
        dojo.byId("cdEndereco").value = 0;
        //dojo.byId("codBairro").value = 0;
        dojo.byId("codRua").value = 0;
        dijit.byId("operadora").reset();
        dijit.byId("logradouro").reset();
        dijit.byId("tipoEndereco").reset();
        dijit.byId("estado").reset();
        dijit.byId("estado").oldValue = 0;
        dijit.byId("cidade").reset();
        dijit.byId("bairro").reset();
        dijit.byId("bairro").oldValue
        dijit.byId("rua").reset();
        dojo.byId("numero").value = "";
        dojo.byId("complemento").value = "";
        dojo.byId("cep").value = "";
        dijit.byId("cep").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function setValueEnderecoPrincipal(data) {
    try {
        configurarLayoutEndereco(LAYOUTPESQUISALOGRADOURO, "estado", "cidade", "bairro", "rua", "cep", "cadCidade", "cadBairro", "cadRua");
        var compEstado = dijit.byId("estado");
        var compCidade = dijit.byId("cidade");
        var compBairro = dijit.byId("bairro");
        var compCep = dijit.byId("cep");
        dojo.byId("cdEndereco").value = data.cd_endereco;
        //Estado
        if (hasValue(data.cd_loc_estado)) {
            compEstado._onChangeActive = false;
            compEstado.set("value", data.cd_loc_estado);
            compEstado.oldValue = data.cd_loc_estado;
            compEstado._onChangeActive = true
        }
        //Cidade
        if (hasValue(data.cd_loc_cidade)) {
            compCidade._onChangeActive = false;
            criarOuCarregarCompFiltering("cidade", [{ id: data.cd_loc_cidade, name: data.noLocCidade }], "", data.cd_loc_cidade, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            compCidade._onChangeActive = true;
        }
        //bairro
        if (hasValue(data.cd_loc_bairro)) {
            compBairro._onChangeActive = false;
            if (hasValue(data.bairros)) {
                criarOuCarregarCompFiltering("bairro", data.bairros, "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localidade', 'no_localidade');

            } else
                criarOuCarregarCompFiltering("bairro", [{ id: data.cd_loc_bairro, name: data.noLocBairro }], "", data.cd_loc_bairro, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            dijit.byId("bairro").oldValue = data.cd_loc_bairro;
            compBairro._onChangeActive = true
        }
        //Cep
        if (hasValue(data.num_cep)) {
            compCep._onChangeActive = false;
            compCep.set("value", data.num_cep);
            compCep._onChangeActive = true;
            compEstado.set("readOnly", true);
        }
        if (hasValue(data.cd_loc_logradouro)) {
            dojo.byId("codRua").value = data.cd_loc_logradouro;
            dijit.byId("rua").set("value", data.noLocRua);
        }
        if (hasValue(data.cd_tipo_logradouro))
            dijit.byId("logradouro").set("value", data.cd_tipo_logradouro);
        if (hasValue(data.cd_tipo_endereco))
            dijit.byId("tipoEndereco").set("value", data.cd_tipo_endereco);
        data.dc_compl_endereco != null ? dojo.byId("complemento").value = data.dc_compl_endereco : "";
        data.dc_num_endereco != null ? dojo.byId("numero").value = data.dc_num_endereco : "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setIdEmailEnviadoView() {
    var grid = dijit.byId("gridFollowUp");
    
    for (var i = 0; i < grid.store.objectStore.data.length; i++) {
        if (hasValue(grid) && hasValue(grid.store.objectStore)) {

            var followUp = jQuery.grep(grid.itensSelecionados, function (value) {

                if (hasValue(value.id) && hasValue(grid.store.objectStore.data[i].id))
                    return value.id == grid.store.objectStore.data[i].id;
                else 
                    return value.cd_follow_up == grid.store.objectStore.data[i].cd_follow_up;
            });

            if (hasValue(followUp))
                grid.store.objectStore.data[i].id_email_enviado_view = true;
            else
                grid.store.objectStore.data[i].id_email_enviado_view = false;
        }
    }
}

function geradorIdFollowUp(grid) {
    try {
        var id = grid.store.objectStore.data.length;
        var itensArray = grid.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
        if (id == 0)
            id = 1;
        else if (id > 0)
            id = itensArray[id - 1].id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}



function pesquisarProspectsAtividadesProspect(cdProspect) {
    
    //var cdProspect = hasValue(dojo.byId("cd_prospect").value) ? dojo.byId("cd_prospect").value : 0;
    var cdProspect = hasValue(cdProspect) ? cdProspect : 0;
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/GetAtividadeProspectByCdProspect?cdProspect=" + cdProspect,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_atividade_prospect"
        }).then(function (data) {
            try {

                var grid = dijit.byId("gridAtividadesProspects");

                var dataRetorno = jQuery.parseJSON(data).retorno;

                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataRetorno, idProperty: "cd_atividade_prospect" }) });
                grid.setStore(dataStore);

                dijit.byId('tagAtividadeProspect').set('open', true);
                dijit.byId("gridAtividadesProspects").update();
                dijit.byId('tagAtividadeProspect').set('open', false);
                
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagemAtividadeExtra', error.response.data);
            });
    });
}
//#endregion