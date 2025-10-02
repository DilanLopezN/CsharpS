
function montarSubgrupoContas(funcao, nivelpesquisa) {
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        "dojo/on",
        "dijit/form/Button",
        "dijit/Dialog",
        "dijit/form/DateTextBox",
        "dojox/grid/LazyTreeGrid",
        "dijit/tree/ForestStoreModel",
        "dojo/data/ItemFileWriteStore",
	    "dojo/domReady!"
    ], function (ready, xhr, on, Button) {
        ready(function () {
            try {
                //Cria a grid de Plano de Contas:
                //var visionData = mudaEstruturaSubGrupo(treeData);
                var data_plano = {
                    identifier: 'id',
                    label: 'name',
                    items: []
                };
                var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
                var model = new dijit.tree.ForestStoreModel({
                    store: store, childrenAttrs: ['children']
                });

                var titleName = 'Subgrupo de Contas';
                var layout = [
                    { name: titleName, field: 'name', width: '94%' },
                    { name: ' ', field: 'selecionado', width: '6%', styles: "text-align: center;", formatter: formatCheckBoxSubgrupoContaFK },
                    { name: '', field: 'id', width: '0%', styles: "display: none;" },
                    { name: '', field: 'cd', width: '0%', styles: "display: none;" }
                ];
                var gridName = "gridSubgrupoContaFK";

                var gridSubgrupoContaFK = new dojox.grid.LazyTreeGrid({
                    id: gridName,
                    treeModel: model,
                    structure: layout
                }, document.createElement('div'));

                dojo.byId(gridName).appendChild(gridSubgrupoContaFK.domNode);
                gridSubgrupoContaFK.startup();

                gridSubgrupoContaFK.itensSelecionados = new Array();

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarSubgrupoContaFK");
                decreaseBtn(document.getElementById("pesquisarSubgrupoContaFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () {
                        try {
                            var gridSubgrupoContaFK = dijit.byId("gridSubgrupoContaFK");

                            if (!hasValue(gridSubgrupoContaFK.itensSelecionados) || gridSubgrupoContaFK.itensSelecionados.length <= 0) {
                                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                                valido = false;
                            } else if (gridSubgrupoContaFK.itensSelecionados.length > 1) {
                                caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                                valido = false;
                            }
                            else {
                                var tipo = parseInt(dojo.byId('nivel').value);
                                switch (tipo) {
                                    case NIVEL1: {
                                        dojo.byId('cbSubGrupo').value = gridSubgrupoContaFK.itensSelecionados[0].name;
                                        dojo.byId('cd_subgrupo1').value = gridSubgrupoContaFK.itensSelecionados[0].cd;
                                        dijit.byId("limparSubGrupo1").set("disabled", false);
                                        gerar_titulo = true;

                                        break;
                                    }
                                    case NIVEL2: {
                                        dojo.byId('cbSubGrupo2').value = gridSubgrupoContaFK.itensSelecionados[0].name;
                                        dojo.byId('cd_subgrupo2').value = gridSubgrupoContaFK.itensSelecionados[0].cd;
                                        if (hasValue(dijit.byId("limparSubGrupo2")))
                                            dijit.byId("limparSubGrupo2").set("disabled", false);
                                        gerar_titulo = true;

                                        break;
                                    }

                                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                        return false;
                                        break;
                                }

                                dijit.byId("cadSubGrupo").hide();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaSubgrupoContaFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("cadSubGrupo").hide(); }
                }, "fecharSubgrupoContFK");

                xhr.get({
                    url: Endereco() + "/api/escola/getParametrosNiveisPlanoConta",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        var nivelParametro = 1;
                        var nivel_parametro = data.retorno;
                        if (hasValue(nivelpesquisa))
                            nivelParametro = nivelpesquisa;
                        else
                            nivelParametro = hasValue(nivel_parametro) && nivel_parametro > 0 ? nivel_parametro : 2;
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getSubgrupoContaSearchFK?descricao=&inicio=false&cdGrupo=0&tipoNivel=" + nivelParametro,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataSubgruposConta) {
                            try {
                                var visionData = mudaEstruturaSubGrupo(dataSubgruposConta);
                                var data_plano = {
                                    identifier: 'id',
                                    label: 'name',
                                    items: visionData
                                };
                                var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
                                gridSubgrupoContaFK.setStore(store);
                                gridSubgrupoContaFK.itensSelecionados = new Array();
                                if (!hasValue(dataSubgruposConta) || (hasValue(dataSubgruposConta) && dataSubgruposConta.length <= 0)) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotRegEnc);
                                    apresentaMensagem("apresentadorMensagemPlanoContasFK", mensagensWeb);
                                    dojo.byId('apresentadorMensagemPlanoContasFK').style.display = "";
                                }
                                showCarregando();
                            }
                            catch (e) {
                                showCarregando();
                                postGerarLog(e);
                            }
                        },
                        function (error) { //Cria a grid de Plano de Contas:
                            showCarregando();
                            apresentaMensagem('apresentadorMensagemSubgrupoContasFK', error);
                        });
                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                },
               function (error) {
                   showCarregando();
                   apresentaMensagem('apresentadorMensagemSubgrupoContasFK', error);
               });
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}

// Subgrupo de Contas
function pesquisarSubgrupoContasFK(limparItens, nivelPesquisa) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getSubgrupoContaSearchFK?descricao=" + encodeURIComponent(document.getElementById("pesquisaSubgrupoContaFK").value) + "&inicio=" +
                           document.getElementById("inicioDescSubgrupoContaFK").checked + "&cdGrupo=0&tipoNivel=" + nivelPesquisa,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (dataSubgruposConta) {
        try {
            var gridSubgrupoContaFK = dijit.byId("gridSubgrupoContaFK");
            var visionData = mudaEstruturaSubGrupo(dataSubgruposConta);
            var data_plano = {
                identifier: 'id',
                label: 'name',
                items: visionData
            };
            var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
            gridSubgrupoContaFK.setStore(store);
            gridSubgrupoContaFK.itensSelecionados = new Array();
            if (!hasValue(dataSubgruposConta) || (hasValue(dataSubgruposConta) && dataSubgruposConta.length <= 0)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotRegEnc);
                apresentaMensagem("apresentadorMensagemSubgrupoContasFK", mensagensWeb);
                dojo.byId('apresentadorMensagemSubgrupoContasFK').style.display = "";
            }
            showCarregando();
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) { //Cria a grid de Plano de Contas:
        showCarregando();
        apresentaMensagem('apresentadorMensagemSubgrupoContasFK', error);
    });
}

function limparPesquisaPoliticaComercialFK() {
    try {
        dijit.byId('pesquisaSubgrupoContaFK').set('value', '');
        dijit.byId('inicioDescSubgrupoContaFK').set('checked', false);
        if (hasValue(dijit.byId("gridSubgrupoContaFK").itensSelecionados))
            dijit.byId("gridSubgrupoContaFK").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mudaEstruturaSubGrupo(data) {
    try {
        var retorno = new Array();
        var treeId = 1;

        if (hasValue(data)) {

            if (data.length == null || data.length == 0) data = data.retorno;

            for (var i = 0; i < data.length; i++) {
                var grupo = { name: data[i].no_grupo_conta, cd: data[i].cd_grupo_conta, id: treeId };
                treeId += 1;

                var sub_grupos = data[i].SubGrupos;
                if (hasValue(sub_grupos) && sub_grupos.length > 0) {
                    grupo.children = new Array();

                    for (var j = 0; j < sub_grupos.length; j++) {
                        var sub_grupo1 = { name: sub_grupos[j].no_subgrupo_conta, cd: sub_grupos[j].cd_subgrupo_conta, id: treeId };
                        treeId += 1;

                        //Se houver segundo nível, monta ele:
                        var sub_grupos2 = sub_grupos[j].SubgruposFilhos;

                        if (hasValue(sub_grupos2) && sub_grupos2.length > 0) {
                            sub_grupo1.children = new Array();

                            for (var m = 0; m < sub_grupos2.length; m++) {
                                var sub_grupo2 = { name: sub_grupos2[m].no_subgrupo_conta, cd: sub_grupos2[m].cd_subgrupo_conta, id: treeId };
                                treeId += 1

                                sub_grupo1.children.push(sub_grupo2);
                            }
                        }

                        grupo.children.push(sub_grupo1);
                    }
                }
                retorno.push(grupo);
            }
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxSubgrupoContaFK(value, rowIndex, obj, k) {
    try {
        var gridSubgrupoContaFK = dijit.byId("gridSubgrupoContaFK");
        var icon;
        var item = gridSubgrupoContaFK._by_idx[rowIndex].item;
        var id = k.field + '_Selected_' + item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && !hasValue(item.children))
            icon = "<input id='" + id + "' class='formatCheckBox' /> ";
        else
            icon = "<div class='formatCheckBoxPlano'>...</div>";

        if (hasValue(gridSubgrupoContaFK.itensSelecionados))
            for (var i = 0; i < gridSubgrupoContaFK.itensSelecionados.length; i++)
                if (hasValue(gridSubgrupoContaFK._by_idx[rowIndex].item.cd_subgrupo_conta)
                    && gridSubgrupoContaFK.itensSelecionados[i].cd_subgrupo_conta[0] == gridSubgrupoContaFK._by_idx[rowIndex].item.cd_subgrupo_conta[0])
                    value = true;

        setTimeout("configuraCheckBoxSubgrupoContas(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "')", 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxSubgrupoContas(value, rowIndex, field, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value,
                onChange: function (b) { checkBoxChangeSubgrupoContas(rowIndex, this); }
            }, id);
        }
        else {
            var dijitObj = dijit.byId(id);

            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeSubgrupoContas(rowIndex, obj) {
    try {
        var gridSubgrupoContaFK = dijit.byId("gridSubgrupoContaFK");
        var item = gridSubgrupoContaFK.getItem(rowIndex);

        if (obj.checked)
            insertObjSort(gridSubgrupoContaFK.itensSelecionados, 'id', item, false);
        else
            removeObjSort(gridSubgrupoContaFK.itensSelecionados, "id", item.id);
    }
    catch (e) {
        postGerarLog(e);
    }
}