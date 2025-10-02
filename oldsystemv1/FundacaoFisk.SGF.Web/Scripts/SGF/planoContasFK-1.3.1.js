var PRINCIPAL_MATRICULA = 1, TAXA_MATRICULA = 2, RELATORIO_POSICAO_FINANCEIRA = 3, MOVIMENTO = 4,
    CONTA_CORRENTE = 5, ITEM = 6, ESCOLA_MATRICULA = 7, ESCOLA_TAXA = 8, ESCOLA_CCORRENTE = 9, BAIXA_FINANCEIRA = 10, PROSPECT = 11,
    ESCOLA_JUROS = 12, ESCOLA_MULTA = 13, ESCOLA_DESCONTO = 14, MOVIMENTO_DEVOLUCAO = 15, ESCOLA_TAXA_BANCARIA = 16, ESCOLA_MATERIAL = 17, ESCOLA_SERVICO = 18;

function montarFKPlanoContas(pFuncaoRetorno, apresentador_menagens, tipoRetorno, tipoMovimento) {
    require([
    "dojo/_base/xhr",
    "dojo/query",
    "dijit/form/Button",
    "dojo/ready",
    "dijit/Dialog",
    "dojox/grid/LazyTreeGrid",
    "dijit/tree/ForestStoreModel",
    "dojo/data/ItemFileWriteStore"
    ], function (xhr, query, Button, ready) {
        ready(function () {
            try {
                showCarregando();
                if (tipoRetorno > 0)
                    dojo.byId('tipoRetorno').value = tipoRetorno;

                var parametroPesquisa = "";

                switch (tipoRetorno) {
                    case MOVIMENTO: {
                        if (tipoMovimento == PESQUISA)
                            parametroPesquisa = "getPlanoContasWithMovimento?tipoMovimento=" + tipoMovimento;
                        else
                            parametroPesquisa = "getPlanoContasTreeSearch";
                        break;
                    }

                    case CONTA_CORRENTE: {
                        parametroPesquisa = "getPlanoContasTreeSearch";
                        break;
                    }

                    default: {
                        parametroPesquisa = 'getPlanoContasTreeSearch';
                        break;
                    };
                }

                loadTreeGrid(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento, false);

                if (!hasValue(dijit.byId('selecionaPlanoContasFK')))
                    new Button({
                        label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                            var gridPesquisaPlanoContas = dijit.byId("gridPesquisaPlanoContas");

                            if (!hasValue(gridPesquisaPlanoContas.itensSelecionados) || gridPesquisaPlanoContas.itensSelecionados.length <= 0) {
                                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                                valido = false;
                            } else if (gridPesquisaPlanoContas.itensSelecionados.length > 1) {
                                caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                                valido = false;
                            }
                            else {
                                var tipo = parseInt(dojo.byId('tipoRetorno').value);
                                switch (tipo) {
                                    case PRINCIPAL_MATRICULA: {
                                        dojo.byId('planoContasMat').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasMat").set("disabled", false);
                                        gerar_titulo = true;
                                        break;
                                    }
                                    case TAXA_MATRICULA: {
                                        dojo.byId('planoContasTaxa').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas_tx').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContaTx").set('disabled', false);
                                        gerar_titulo = true;
                                        break;
                                    }
                                    case RELATORIO_POSICAO_FINANCEIRA: {
                                        dojo.byId('noPlanoRelPos').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cdPlanoPesRel').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoRelPosPes").set('disabled', false);
                                        break;
                                    }
                                    case MOVIMENTO: {
                                        if (TIPO_PESQUISA == PESQUISA) {
                                            dojo.byId('noPlanoContaPesq').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                            dojo.byId('cdPlanoContaPesq').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                            if (hasValue(dojo.byId('cdPlanoContaPesq').value))
                                                dijit.byId("limparPlanoContaPesq").set('disabled', false);
                                        } else {
                                            dojo.byId('descPlanoConta').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                            dojo.byId('cd_plano_contas').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        }
                                        break;
                                    }
                                    case CONTA_CORRENTE: {
                                        if (TIPO_PESQUISA == 1) {
                                            dojo.byId('desPlanoContasPes').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                            dojo.byId('cdFkPlanoContaPes').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                            if (hasValue(dojo.byId("desPlanoContasPes").value))
                                                dijit.byId("limparFkPlanoContasPes").set('disabled', false);
                                        } else {
                                            dojo.byId('dcPlanoCad').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                            dojo.byId('cdFkPlanoContaCad').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                            dijit.byId("limparPlanoContas").set('disabled', false);
                                        }
                                        break;
                                    }

                                    case ITEM: {
                                        dojo.byId('planoContasItem').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas_item').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContaItem").set('disabled', false);
                                        break;
                                    }

                                    case BAIXA_FINANCEIRA: {
                                        dojo.byId('cdPlanoContasTitulo').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas_titulo').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        break;
                                    }

                                    case ESCOLA_MATRICULA: {
                                        dojo.byId('planoContasMat').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_matricula').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoMat").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_SERVICO: {
                                        dojo.byId('planoContasServico').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_servico').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoServico").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_TAXA: {
                                        dojo.byId('planoContasTaxa').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_taxa').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoTaxa").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_CCORRENTE: {
                                        dojo.byId('planoContasTransferencia').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas_trasferencia').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContaTransf").set('disabled', false);
                                        break;
                                    }

                                    case PROSPECT: {

                                        dojo.byId('planoContasProsp').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_contas_prosp').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasProsp").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_JUROS: {
                                        dojo.byId('planoContasJuros').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_conta_juros').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContaJuros").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_MULTA: {
                                        dojo.byId('planoContasMulta').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_conta_multa').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasMulta").set('disabled', false);
                                        break;
                                    }
                                    case ESCOLA_DESCONTO: {
                                        dojo.byId('planoContasDesconto').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_conta_desc').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasDesconto").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_TAXA_BANCARIA: {
                                        dojo.byId('planoContasTaxasBancarias').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_taxa_bco').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasTaxasBancarias").set('disabled', false);
                                        break;
                                    }

                                    case ESCOLA_MATERIAL: {
                                        dojo.byId('planoContasMaterial').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cd_plano_conta_material').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        dijit.byId("limparPlanoContasMaterial").set('disabled', false);
                                        break;
                                    }

                                    case MOVIMENTO_DEVOLUCAO: {
                                        dojo.byId('noPlanoContaPesqFK').value = gridPesquisaPlanoContas.itensSelecionados[0].name;
                                        dojo.byId('cdPlanoContaPesqFK').value = gridPesquisaPlanoContas.itensSelecionados[0].cd_plano_conta;
                                        if (hasValue(dojo.byId('cdPlanoContaPesqFK').value))
                                            dijit.byId("limparPlanoContaPesqFK").set('disabled', false);

                                        break;
                                    }
                                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                        return false;
                                        break;
                                }

                                dijit.byId("cadPlanoContas").hide();
                            }
                        }
                    }, "selecionaPlanoContasFK");

                if (!hasValue(dijit.byId('fecharPlanoContasFK')))
                    new Button({
                        label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function () {
                            dijit.byId("cadPlanoContas").hide();
                        }
                    }, "fecharPlanoContasFK");

                if (!hasValue(dijit.byId('pesquisarPlanoFK'))) {
                    new Button({
                        label: "", iconClass: 'dijitEditorIconSearchSGF',
                        onClick: function () {
                            if (window.TIPO_PESQUISA !== undefined)
                                parametroPesquisa = (TIPO_PESQUISA == CADASTRO || TIPO_PESQUISA == PESQUISA && PESQUISA_CONTA_CORRENTE == 1 || (TIPO_PESQUISA_VINCULA_MATERIAL != null && TIPO_PESQUISA_VINCULA_MATERIAL != undefined && TIPO_PESQUISA_VINCULA_MATERIAL > 0))  ? "getPlanoContasTreeSearch" : "getPlanoContasWithMovimento?tipoMovimento=" + tipoMovimento;;

                            pesquisarDescricao(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento);
                        }
                    }, "pesquisarPlanoFK");

                    decreaseBtn(document.getElementById("pesquisarPlanoFK"), '32px');
                }
                dojo.query("#descricaoPlanoContas").on("keyup", function (e) {
                    if (e.keyCode == 13)
                        dijit.byId('pesquisarPlanoFK').onClick();
                });
                dijit.byId('formPlanoContasFK').onSubmit = function () { return false; };
                showCarregando();
            }
            catch (e) {
                postGerarLog(' Tipo retorno = ' + dojo.byId('tipoRetorno').value + ' ' + e);
            }
        });
    });
}

function pesquisarDescricao(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento) {
    try{
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
        reloadTreeGrid(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPlanoContasFK(value, rowIndex, obj, k) {
    try{
        var gridPesquisaPlanoContas = dijit.byId("gridPesquisaPlanoContas");
        var icon;
        var item = gridPesquisaPlanoContas._by_idx[rowIndex].item;
        var id = k.field + '_Selected_' + item._0;

        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && !hasValue(item.children))
            icon = "<input id='" + id + "' class='formatCheckBox' /> ";
        else
            icon = "<div class='formatCheckBoxPlano'>...</div>";

        if(hasValue(gridPesquisaPlanoContas.itensSelecionados))
            for (var i = 0; i < gridPesquisaPlanoContas.itensSelecionados.length; i++)
                if (hasValue(gridPesquisaPlanoContas._by_idx[rowIndex].item.cd_plano_conta)
                    && gridPesquisaPlanoContas.itensSelecionados[i].cd_plano_conta[0] == gridPesquisaPlanoContas._by_idx[rowIndex].item.cd_plano_conta[0])
                    value = true;

        setTimeout("configuraCheckBoxPlanoContas(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "')", 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxPlanoContas(value, rowIndex, field, id) {
    try{
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value,
                onChange: function (b) { checkBoxChangePlanoContas(rowIndex, this); }
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

function checkBoxChangePlanoContas(rowIndex, obj) {
    try{
        var gridPesquisaPlanoContas = dijit.byId("gridPesquisaPlanoContas");
        var item = gridPesquisaPlanoContas.getItem(rowIndex);

        if (obj.checked)
            insertObjSort(gridPesquisaPlanoContas.itensSelecionados, 'id', item, false);
        else
            removeObjSort(gridPesquisaPlanoContas.itensSelecionados, "id", item.id);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mudaEstrutura(data) {
    try{
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
                        if (hasValue(sub_grupos[j].SubgrupoPlanoConta[0]) && hasValue(sub_grupos[j].SubgrupoPlanoConta[0].cd_plano_conta))
                            sub_grupo1.cd_plano_conta = sub_grupos[j].SubgrupoPlanoConta[0].cd_plano_conta;
                        treeId += 1;

                        //Se houver segundo nível, monta ele:
                        var sub_grupos2 = sub_grupos[j].SubgruposFilhos;

                        if (hasValue(sub_grupos2) && sub_grupos2.length > 0) {
                            sub_grupo1.children = new Array();

                            for (var m = 0; m < sub_grupos2.length; m++) {
                                var sub_grupo2 = { name: sub_grupos2[m].no_subgrupo_conta, cd: sub_grupos2[m].cd_subgrupo_conta, id: treeId, cd_plano_conta: sub_grupos2[m].SubgrupoPlanoConta[0].cd_plano_conta };
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

function loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno) {
    dojo.ready(function () {
        dojo.byId('tipoRetorno').value = tipoRetorno;
        xhr.get({
            url: Endereco() + "/api/financeiro/getPlanoContasTreeSearch?descricao=&inicio=false",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (treeData) {
            try{
                var gridPesquisaPlanoContas = dijit.byId("gridPesquisaPlanoContas");
                if (!hasValue(treeData) || treeData.length <= 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroConfigPlanoContas);
                    apresentaMensagem(apresentador_menagens, mensagensWeb);
                    return false;
                }

                //Cria a grid de Plano de Contas:
                var visionData = mudaEstrutura(treeData);
                var data_plano = {
                    identifier: 'id',
                    label: 'name',
                    items: visionData
                };
                var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
                gridPesquisaPlanoContas.setStore(store);
                gridPesquisaPlanoContas.itensSelecionados = new Array();
            }
            catch (e) {
                postGerarLog(' Tipo retorno = ' + dojo.byId('tipoRetorno').value + ' ' + e);
            }
        });
    });
}

function reloadTreeGrid(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento) {
    try{
        destroyCreateGridPlano();
        loadTreeGrid(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento, true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridPlano() {
    try{
        if (hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
            dijit.byId("gridPesquisaPlanoContas").destroy();
            $('<div>').attr('id', 'gridPesquisaPlanoContas').appendTo('#gridPesquisaPlanoContasPai');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTreeGrid(xhr, pFuncaoRetorno, apresentador_menagens, parametroPesquisa, tipoMovimento, reload) {
    if (parametroPesquisa.indexOf('?') == -1)
        parametroPesquisa = parametroPesquisa + "?descricao=" + dojo.byId('descricaoPlanoContas').value + "&inicio=" + dijit.byId('inicioDescricaoPlanoContas').checked;
    else
        parametroPesquisa = parametroPesquisa + "&descricao=" + dojo.byId('descricaoPlanoContas').value + "&inicio=" + dijit.byId('inicioDescricaoPlanoContas').checked;

    xhr.get({
        url: Endereco() + "/api/financeiro/" + parametroPesquisa,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (treeData) {
        try{
            if (!hasValue(tipoMovimento)) {
                if ((!hasValue(treeData) || treeData.length <= 0)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroConfigPlanoContas);
                    apresentaMensagem(apresentador_menagens, mensagensWeb);
                    return false;
                }
            }

            //Cria a grid de Plano de Contas:
            var visionData = mudaEstrutura(treeData);
            var data_plano = {
                identifier: 'id',
                label: 'name',
                items: visionData
            };
            var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            var titleName = 'Plano de Contas';
            var layout = [
                { name: titleName, field: 'name', width: '94%' },
                { name: ' ', field: 'selecionado', width: '6%', styles: "text-align: center;", formatter: formatCheckBoxPlanoContasFK },
                { name: '', field: 'id', width: '0%', styles: "display: none;" },
                { name: '', field: 'cd', width: '0%', styles: "display: none;" }
            ];
            var gridName = "gridPesquisaPlanoContas";

            destroyCreateGridPlano();
            var gridPesquisaPlanoContas = new dojox.grid.LazyTreeGrid({
                id: gridName,
                treeModel: model,
                structure: layout
            }, document.createElement('div'));

            dojo.byId(gridName).appendChild(gridPesquisaPlanoContas.domNode);
            gridPesquisaPlanoContas.startup();

            gridPesquisaPlanoContas.itensSelecionados = new Array();

            if (hasValue(pFuncaoRetorno, false))
                pFuncaoRetorno.call();

            if (reload)
                expandAllDojoLazyTreeGrid(dijit.byId('gridPesquisaPlanoContas'), 2);

            if (!hasValue(treeData) || (hasValue(treeData) && treeData.retorno.length <= 0)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotRegEnc);
                apresentaMensagem("apresentadorMensagemPlanoContasFK", mensagensWeb);
                dojo.byId('apresentadorMensagemPlanoContasFK').style.display = "";
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem(apresentador_menagens, error);
    });
}