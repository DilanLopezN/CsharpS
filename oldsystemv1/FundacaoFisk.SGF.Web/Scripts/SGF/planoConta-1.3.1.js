//#region criação dos metodos para montar o checkBox da grade - enumeradores - clearMensagem
var MEU_PLANO = 1;
var PLANO_CONTA_DISPONIVEL = 0;
var PAI = 1;
var FILHO = 2;
var NETO = 3;

function clearMensagem() {
    try{
        apresentaMensagem("apresentadorMensagem", null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region formatCheckBoxPlanoConta
function formatCheckBoxPlanoConta(value, rowIndex, obj, k) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var icon;
        var id = k.field + '_Selected_' + gridPlanoContas._by_idx[rowIndex].item.id[0];
        var cd_conta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
        var cd_grupo_conta = gridPlanoContas._by_idx[rowIndex].item.cd_grupo_conta[0];
        var cd_subgrupo_pai = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_pai[0];
        var cd_subgrupo_conta = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_conta[0];
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";

        setTimeout("configuraCheckBoxPlano(" + value + ", '" + rowIndex + "', '" + k.field + "', '" + id + "', " + gridPlanoContas._by_idx[rowIndex].item.marcado + ", " + cd_conta + ", " + cd_grupo_conta + ", " + cd_subgrupo_conta + ", " + cd_subgrupo_pai + ")", 1);
    return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxPlano(value, rowIndex, field, id, marcado, cd_conta, cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                disabled: eval(MasterGeral()) == true ? false : true,
                checked: marcado,
                onChange: function (b) { checkBoxChangePlano(rowIndex, field, this, cd_conta, cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai) }
            }, id);
        }
        else {
            value = value;
            var dijitObj = dijit.byId(id);
            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
            dijitObj.onChange = function (b) { checkBoxChangePlano(rowIndex, field, this, cd_conta, cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai) };
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangePlano(rowIndex, campo, obj, cd_conta, cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var idPai = 0;
        var idCampo = 0;
        for (var i = 0; i < gridPlanoContas.store._arrayOfTopLevelItems.length; i++) {
            cd_grupo_conta = cd_grupo_conta == null ? gridPlanoContas.store._arrayOfTopLevelItems[i].cd_grupo_conta[0] : cd_grupo_conta;
            if (cd_grupo_conta == gridPlanoContas.store._arrayOfTopLevelItems[i].cd_grupo_conta[0] && cd_subgrupo_conta == 0) {
                gridPlanoContas.store._arrayOfTopLevelItems[i].marcado[0] = obj.checked;
                var campo = 'marcado_Selected_' + gridPlanoContas.store._arrayOfTopLevelItems[i].id[0];
                atualizaCheckBox(campo, obj.checked);
                if (gridPlanoContas.store._arrayOfTopLevelItems[i].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children.length > 0) {
                    for (var j = 0; j < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; j++) {
                        cd_subgrupo_conta = cd_subgrupo_conta == null ? gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].cd_subgrupo_conta[0] : cd_subgrupo_conta;
                        gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].marcado[0] = obj.checked;
                        var campo = 'marcado_Selected_' + gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].id[0];
                        atualizaCheckBox(campo, obj.checked);
                        if (gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length > 0) {
                            for (var l = 0; l < gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length; l++) {
                                cd_subgrupo_conta = cd_subgrupo_conta == null ? gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[l].cd_subgrupo_conta[0] : cd_subgrupo_conta;
                                cd_subgrupo_pai = cd_subgrupo_pai == null ? gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[l].cd_subgrupo_pai[0] : cd_subgrupo_pai;
                                gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[l].marcado[0] = obj.checked;
                                var campo = 'marcado_Selected_' + gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[l].id[0];
                                atualizaCheckBox(campo, obj.checked);
                            }
                        }
                    }
                }
            }//if cd_subgrupo_conta
            if (cd_subgrupo_conta > 0) {
                if (gridPlanoContas.store._arrayOfTopLevelItems[i].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children.length > 0) {
                    for (var l = 0; l < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; l++) {
                        if (cd_grupo_conta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].cd_grupo_conta[0] && cd_subgrupo_conta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].cd_subgrupo_conta[0]) {
                            idCampo = gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].id[0];
                            gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].marcado[0] = obj.checked;
                            marcarCheckBox(obj, idCampo);
                            var desmarcar = verificaSeExiteGrupoMarcado(cd_grupo_conta, cd_subgrupo_conta, 0);

                            if (obj.checked == true) {
                                gridPlanoContas.store._arrayOfTopLevelItems[i].marcado[0] = obj.checked;
                                marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].id[0]);
                            }
                            if (desmarcar == false && obj.checked == false) {
                                gridPlanoContas.store._arrayOfTopLevelItems[i].marcado[0] = obj.checked;
                                marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].id[0]);
                            }

                            if (gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children.length > 0)
                                for (var n = 0; n < gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children.length; n++) {
                                    gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[n].marcado[0] = obj.checked;
                                    var campo = 'marcado_Selected_' + gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[n].id[0];
                                    atualizaCheckBox(campo, obj.checked);
                                }
                        }
                        if (gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children.length > 0) {
                            for (var m = 0; m < gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children.length; m++) {
                                if (cd_subgrupo_pai == gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].cd_subgrupo_conta[0] && cd_conta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[m].cd_subgrupo_conta[0]) {
                                    marcarCheckBox(gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[m].marcado[0], obj, gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[m].id[0]);
                                    gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].children[m].marcado[0] = obj.checked;
                                    var desmarcar = verificaSeExiteGrupoMarcado(cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai);
                                    if (obj.checked == true) {
                                        gridPlanoContas.store._arrayOfTopLevelItems[i].marcado[0] = obj.checked;
                                        gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].marcado[0] = obj.checked;
                                        marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].id[0]);
                                        marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].id[0]);
                                    }
                                    if (desmarcar == false && obj.checked == false) {
                                        gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].marcado[0] = obj.checked;
                                        marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].children[l].id[0]);
                                        var desmarcar = verificaSeExiteGrupoMarcado(cd_grupo_conta, cd_subgrupo_conta, 0);
                                        if (desmarcar == false && obj.checked == false) {
                                            gridPlanoContas.store._arrayOfTopLevelItems[i].marcado[0] = obj.checked;
                                            marcarCheckBox(obj, gridPlanoContas.store._arrayOfTopLevelItems[i].id[0]);
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function marcarCheckBox(obj, id) {
    try{
        var campo = 'marcado_Selected_' + id;
        atualizaCheckBox(campo, obj.checked);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaCheckBox(dijitObj, value) {
    try{
        dijitObj = dijit.byId(dijitObj);
        if (hasValue(dijitObj) && !dijitObj.disabled) {
            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaSeExiteGrupoMarcado(cd_grupo_conta, cd_subgrupo_conta, cd_subgrupo_pai) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var desmarcar = false;
        //regra para desmarcar os filhos
        for (var p = 0; p < gridPlanoContas.store._arrayOfTopLevelItems.length; p++) {
            if (gridPlanoContas.store._arrayOfTopLevelItems[p].children != null && cd_grupo_conta == gridPlanoContas.store._arrayOfTopLevelItems[p].cd_grupo_conta[0])
                for (var q = 0; q < gridPlanoContas.store._arrayOfTopLevelItems[p].children.length; q++) {
                    if (cd_grupo_conta == gridPlanoContas.store._arrayOfTopLevelItems[p].cd_grupo_conta[0]) {
                        if (gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].marcado[0] == true)
                            desmarcar = true;
                    }
                    if (cd_subgrupo_pai > 0)
                        if (gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].children != null && cd_subgrupo_pai == gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].cd_subgrupo_conta[0])
                            for (var r = 0; r < gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].children.length; r++) {
                                if (gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].children[r].marcado[0] == true && cd_subgrupo_pai == gridPlanoContas.store._arrayOfTopLevelItems[p].children[q].children[r].cd_subgrupo_pai[0]) {
                                    desmarcar = true;
                                    break;
                                } else desmarcar = false;
                            }
                }
        }
        return desmarcar;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region formatDropDownTipoConta
function formatDropDownTipoConta(value, rowIndex, obj, k) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var icon;
        var isPai = gridPlanoContas._by_idx[rowIndex].item.pai[0];
        var idPai = 0;
        var tipoPlano = dojo.byId('tipoPlano').value;
        var niveis = dojo.byId('niveis').value;
        var filhos = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_pai;
        value = value == '' ? 0 : value;
        if (niveis == 2) {
            // constroe so para os filhos
            if (isPai == FILHO && tipoPlano == MEU_PLANO && filhos != null && filhos > 0) {
                idPai = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_pai;
                var idField = k.field + '_drpw_' + gridPlanoContas._by_idx[rowIndex].item._0;
                if (hasValue(dijit.byId(idField)))
                    dijit.byId(idField).destroy();
                if (rowIndex != -1) icon = "<input style='height:19px' id='" + idField + "' /> ";
                setTimeout("configuraDropDownTipo(" + value + ", '" + idField + "'," + gridPlanoContas._by_idx[rowIndex].item.cd_conta + "," + idPai + ")", 1);
                return icon;
            }
        }
        else
            if (isPai == FILHO && tipoPlano == MEU_PLANO) {
                idPai = gridPlanoContas._by_idx[rowIndex].item.cd_grupo_conta[0];
                var idField = k.field + '_drpw_' + gridPlanoContas._by_idx[rowIndex].item._0;
                if (hasValue(dijit.byId(idField)))
                    dijit.byId(idField).destroy();
                if (rowIndex != -1) icon = "<input style='height:19px' id='" + idField + "' /> ";
                setTimeout("configuraDropDownTipo('" + value + "', '" + idField + "'," + gridPlanoContas._by_idx[rowIndex].item.cd_conta + "," + idPai + ")", 1);
                return icon;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDropDownTipo(value, idfield, idConta, idPai) {
    try{
        var tipo = [];
        if (!hasValue(dijit.byId(idfield))) {
            if (value == null || value == 0) value = 0;
            else value;
            require(["dojo/store/Memory", "dijit/form/FilteringSelect"], function (Memory, FilteringSelect) {
                var stateStore = new Memory({
                    data: [
                            { name: "Escolha Tipo", id: 0 },
                            { name: "Fixa", id: 1 },
                            { name: "Variável", id: 2 }
                    ]
                });
                var cbxTipoConta = new FilteringSelect({
                    id: idfield,
                    name: idfield,
                    store: stateStore,
                    value: value,
                    disabled: eval(MasterGeral()) == true? false : true,
                    searchAttr: "name",
                    style: "width:100%; height:19px;",
                    onBlur: function (b) { atualizarTipo(idConta, idPai, this); }
                }, idfield);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarTipo(idConta, idPai, obj) {
    try{
        var nivel = dojo.byId('niveis').value;
        var gridTipoPlano = dijit.byId("gridPlanoContas").store._arrayOfTopLevelItems;

        if (nivel == 2)// siginifica que o plano tem dois níveis
            for (var i in gridTipoPlano) 
                for (var j in gridTipoPlano[i].children) 
                    if (gridTipoPlano[i].children[j].cd_subgrupo_conta == idPai && gridTipoPlano[i].children[j].children != null) 
                        for (var k in gridTipoPlano[i].children[j].children) 
                            if (gridTipoPlano[i].children[j].children[k].cd_subgrupo_conta == idConta) {
                                gridTipoPlano[i].children[j].children[k].id_tipo_conta[0] = obj.value;
                                break;
                            }

        if (nivel == 1)
            for (var i in gridTipoPlano)
                for (var j in gridTipoPlano[i].children)
                    if (gridTipoPlano[i].children[j].cd_grupo_conta == idPai && gridTipoPlano[i].children[j].cd_subgrupo_conta == idConta) {
                        gridTipoPlano[i].children[j].id_tipo_conta[0] = obj.value;
                        break;
                    }

        if (obj.value == null || obj.value == '') {
            dijit.byId(obj.id).set("value", 0);
            dojo.byId(obj.id).value = 'Escolha Tipo';
        }
        updateDadosForPlanoEmpresa();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function updateDadosForPlanoEmpresa() {
    try{
        var dadosGrid = dijit.byId("gridPlanoContas").store._arrayOfTopLevelItems;
        var newPlanoContas = [];
        for (var i = 0, l = 0; i < dadosGrid.length; i++) {
            if (dadosGrid[i].cd_conta[0] != null) {
                newPlanoContas.push({
                    cd_conta: dadosGrid[i].cd_conta,
                    cd_grupo_conta: dadosGrid[i].cd_grupo_conta,
                    cd_subgrupo_conta: dadosGrid[i].cd_subgrupo_conta,
                    cd_subgrupo_pai: dadosGrid[i].cd_subgrupo_pai,
                    cd_plano_conta: dadosGrid[i].cd_plano_conta,
                    dc_conta: dadosGrid[i].dc_conta,
                    id: dadosGrid[i].id,
                    marcado: false,
                    pai: dadosGrid[i].pai,
                    id_conta_segura: false,
                    id_ativo: false
                });
                l += 1;

                //Montando os filhos
                if (dadosGrid[i].children != null && dadosGrid[i].children.length > 0) {
                    var novoChildrenPrimeiroNivel = new Array();

                    for (var j = 0, m = 0; j < dadosGrid[i].children.length; j++) {
                        novoChildrenPrimeiroNivel.push({
                            cd_conta: dadosGrid[i].children[j].cd_conta,
                            cd_grupo_conta: dadosGrid[i].children[j].cd_grupo_conta,
                            cd_subgrupo_conta: dadosGrid[i].children[j].cd_subgrupo_conta,
                            cd_subgrupo_pai: dadosGrid[i].children[j].cd_subgrupo_pai,
                            cd_plano_conta: dadosGrid[i].children[j].cd_plano_conta,
                            dc_conta: dadosGrid[i].children[j].dc_conta,
                            id: dadosGrid[i].children[j].id,
                            marcado: false,
                            pai: dadosGrid[i].children[j].pai,
                            id_tipo_conta: dadosGrid[i].children[j].id_tipo_conta,
                            id_conta_segura: dadosGrid[i].children[j].id_conta_segura,
                            id_ativo: dadosGrid[i].children[j].id_ativo
                        });
                        m += 1;
                        //Monta os netos
                        if (dadosGrid[i].children[j].children != null && dadosGrid[i].children[j].children.length > 0) {
                            var novoChildrenSegundoNivel = new Array();
                            for (var k = 0; k < dadosGrid[i].children[j].children.length; k++) {
                                novoChildrenSegundoNivel.push({
                                    cd_conta: dadosGrid[i].children[j].children[k].cd_conta,
                                    cd_grupo_conta: dadosGrid[i].children[j].children[k].cd_grupo_conta,
                                    cd_subgrupo_conta: dadosGrid[i].children[j].children[k].cd_subgrupo_conta,
                                    cd_subgrupo_pai: dadosGrid[i].children[j].children[k].cd_subgrupo_pai,
                                    cd_plano_conta: dadosGrid[i].children[j].children[k].cd_plano_conta,
                                    dc_conta: dadosGrid[i].children[j].children[k].dc_conta,
                                    id: dadosGrid[i].children[j].children[k].id,
                                    marcado: false,
                                    pai: dadosGrid[i].children[j].children[k].pai,
                                    id_tipo_conta: dadosGrid[i].children[j].children[k].id_tipo_conta,
                                    id_conta_segura: dadosGrid[i].children[j].children[k].id_conta_segura,
                                    id_ativo: dadosGrid[i].children[j].children[k].id_ativo
                                });
                            }
                            novoChildrenPrimeiroNivel[m - 1].children = novoChildrenSegundoNivel;
                        }
                    }
                    newPlanoContas[l - 1].children = novoChildrenPrimeiroNivel;
                }// if children
            }
        }//for i
        dojo.byId('planoContasEmpresa').value = newPlanoContas;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region formatCheckBoxInativo
function formatCheckBoxInativo(value, rowIndex, obj, k) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var icon;
        var id;
        var tipoPlano = dojo.byId('tipoPlano').value;
        var niveis = dojo.byId('niveis').value;
        var isPai = gridPlanoContas._by_idx[rowIndex].item.pai;
        var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
        var filhos = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_pai;

        if (niveis == 2)
            if (isPai == FILHO && tipoPlano == MEU_PLANO && filhos != null && filhos > 0) {
                id = k.field + '_Selected_' + gridPlanoContas._by_idx[rowIndex].item._0;
                var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
                if (hasValue(dijit.byId(id)))
                    dijit.byId(id).destroy();
                if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
                setTimeout("configuraCheckBoxInativo(" + value + ", " + rowIndex + ", '" + id + "'," + idConta + ")", 1);
                return icon;
            }

        if (niveis == 1)
            if (isPai == FILHO && tipoPlano == MEU_PLANO) {
                id = k.field + '_Selected_' + gridPlanoContas._by_idx[rowIndex].item._0;
                var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
                if (hasValue(dijit.byId(id)))
                    dijit.byId(id).destroy();
                if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
                setTimeout("configuraCheckBoxInativo(" + value + ", " + rowIndex + ", '" + id + "'," + idConta + ")", 1);
                return icon;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxInativo(value, rowIndex, id, idConta) {
    if (!hasValue(dijit.byId(id))) {
        require(["dijit/form/CheckBox"], function (CheckBox) {
            try{
                var checkBox = new CheckBox({
                    name: "checkBox" + id,
                    value: value,
                    disabled: eval(MasterGeral()) == true ? false : true,
                    checked: value == null ? false : value,
                    onChange: function (b) { atualizarCheckBoxInativo(idConta, this) }
                }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function atualizarCheckBoxInativo(idConta, obj) {
    try{
        var nivel = dojo.byId('niveis').value;
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        if (nivel == 2)
            for (var i = 0; i < gridPlanoContas.store._arrayOfTopLevelItems.length; i++)
                for (var j = 0; j < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; j++)
                    if (gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length > 0)
                        for (var k = 0; k < gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length; k++)
                            if (idConta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[k].cd_subgrupo_conta[0]) {
                                gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[k].id_ativo[0] = obj.checked;
                                break;
                            }
        if (nivel == 1)
            for (var i = 0; i < gridPlanoContas.store._arrayOfTopLevelItems.length; i++)
                for (var j = 0; j < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; j++)
                    if (idConta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].cd_subgrupo_conta[0]) {
                        gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].id_ativo[0] = obj.checked;
                        break;
                    }

        updateDadosForPlanoEmpresa();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endrefion

function formatCheckBoxContaSegura(value, rowIndex, obj, k) {
    try{
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        var icon;
        var id;
        var tipoPlano = dojo.byId('tipoPlano').value;
        var niveis = dojo.byId('niveis').value;
        var isPai = gridPlanoContas._by_idx[rowIndex].item.pai;
        var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
        var filhos = gridPlanoContas._by_idx[rowIndex].item.cd_subgrupo_pai;

        if (niveis == 2)
            if (isPai == FILHO && tipoPlano == MEU_PLANO && filhos != null && filhos > 0) {
                id = k.field + '_Selected_' + gridPlanoContas._by_idx[rowIndex].item._0;
                var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
                if (hasValue(dijit.byId(id)))
                    dijit.byId(id).destroy();
                if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
                setTimeout("configuraCheckBoxContaSegura(" + value + ", " + rowIndex + ", '" + id + "'," + idConta + ")", 1);
                return icon;
            }

        if (niveis == 1)
            if (isPai == FILHO && tipoPlano == MEU_PLANO) {
                id = k.field + '_Selected_' + gridPlanoContas._by_idx[rowIndex].item._0;
                var idConta = gridPlanoContas._by_idx[rowIndex].item.cd_conta[0];
                if (hasValue(dijit.byId(id)))
                    dijit.byId(id).destroy();
                if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
                setTimeout("configuraCheckBoxContaSegura(" + value + ", " + rowIndex + ", '" + id + "'," + idConta + ")", 1);
                return icon;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxContaSegura(value, rowIndex, id, idConta) {
    if (!hasValue(dijit.byId(id))) {
        require(["dijit/form/CheckBox"], function (CheckBox) {
            try{
                var checkBox = new CheckBox({
                    name: "checkBox" + id,
                    value: value,
                    disabled: eval(MasterGeral()) == true ? false : true,
                    checked: value == null ? false : value,
                    onChange: function (b) { atualizarCheckBoxContaSegura(idConta, this) }
                }, id);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function atualizarCheckBoxContaSegura(idConta, obj) {
    try{
        var nivel = dojo.byId('niveis').value;
        var gridPlanoContas = dijit.byId("gridPlanoContas");
        if (nivel == 2)
            for (var i = 0; i < gridPlanoContas.store._arrayOfTopLevelItems.length; i++)
                for (var j = 0; j < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; j++)
                    if (gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children != null && gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length > 0)
                        for (var k = 0; k < gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children.length; k++)
                            if (idConta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[k].cd_subgrupo_conta[0]) {
                                gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].children[k].id_conta_segura[0] = obj.checked;
                                break;
                            }
        if (nivel == 1)
            for (var i = 0; i < gridPlanoContas.store._arrayOfTopLevelItems.length; i++)
                for (var j = 0; j < gridPlanoContas.store._arrayOfTopLevelItems[i].children.length; j++)
                    if (idConta == gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].cd_subgrupo_conta[0]) {
                        gridPlanoContas.store._arrayOfTopLevelItems[i].children[j].id_conta_segura[0] = obj.checked;
                        break;
                    }

        updateDadosForPlanoEmpresa();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#endregion

//#region montarCadastroPlanoConta - manipulaDadosGridPlanoContas - atualizaIdGridPlanoConta - clearChildrenLenthZero - loadGridPlanoConta - percorreGridForOrdering - montarGridPlanoContas
function montarCadastroPlanoConta() {
    require([
      "dojo/_base/xhr",
      "dojo/dom",
      "dijit/registry",
      "dojox/grid/EnhancedGrid",
      "dojox/grid/enhanced/plugins/Pagination",
      "dojo/data/ObjectStore",
      "dojo/store/Cache",
      "dojo/store/Memory",
      "dojo/query",
      "dojo/dom-attr",
      "dijit/form/Button",
      "dojo/ready",
      "dijit/form/DropDownButton",
      "dijit/DropDownMenu",
      "dijit/MenuItem",
      "dojo/domReady!",
      "dojo/parser",
      "dijit/form/FilteringSelect",
      "dijit/Dialog"
    ], function (xhr, dom, _registry, EnhancedGrid, Pagination, ObjectStore, Cache, Memory, query, domAttr, Button,
             ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect) {
        ready(function () {
            try{
                var desabilitar = eval(MasterGeral()) == true? false : true;
                //Valores que estão sendo montados no drop down de nível.
                var storeNivel = 0;
                var hasGrupoConta = false;
                var hasDisponivel = false;
                dojo.byId('pesquisarBD').value = false;
                dojo.byId('hasDisponivel').value = "";
                dojo.byId('wasPersisted').value = false;
                var storeSubGrupo = [
                      { name: "1 Nível", id: 1 },
                      { name: "2 Níveis", id: 2 }
                ];
                xhr.get({
                    preventCache: true,
                    handleAs: "json",
                    url: Endereco() + "/api/escola/getParametrosForPlanoConta",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataRetorno) {
                    try{
                        storeNivel = dataRetorno.retorno.nivel_plano_conta;
                        hasDisponivel = dataRetorno.retorno.hasGrupoSubGrupoDisponivel;
                        dojo.byId('hasDisponivel').value = hasDisponivel;
                        dojo.byId('niveis').value = storeNivel;
                        if (hasDisponivel == true) {
                            hasGrupoConta = PLANO_CONTA_DISPONIVEL;
                            menuIncluirExcluir.addChild(acaoIncluir);
                            menuIncluirExcluir.removeChild(acaoExcluir);
                        }
                        else {
                            hasGrupoConta = MEU_PLANO;
                            menuIncluirExcluir.addChild(acaoExcluir);
                            menuIncluirExcluir.removeChild(acaoIncluir);
                        }
                        criarOuCarregarCompFiltering("cbNiveis", storeSubGrupo, "", storeNivel, ready, Memory, FilteringSelect, 'id', 'name', null);
                        loadGridPlanoConta(hasGrupoConta, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel, hasDisponivel);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                  function (error) {
                      apresentaMensagem('apresentadorMensagem', error.response.data);
                  });
                //Criando botões
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchPlanoConta(MEU_PLANO, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect);
                    }
                }, "pesquisarPlanoContas");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    disabled: desabilitar,
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSalvarFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        salvarPlanoConta();
                    }
                }, "salvarPlanoContas");

                //Botões de ações relacionadas
                var menu = new DropDownMenu({ style: "height: 25px" });
                var menuIncluirExcluir = new DropDownMenu({ style: "height: 25px" });

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    disabled: desabilitar,
                    onClick: function () {
                       nivel = dojo.byId('niveis').value;
                       hasDisponivel =  dojo.byId('hasDisponivel').value;
                       cancelarPlanoConta(hasGrupoConta, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, nivel, hasDisponivel, menuIncluirExcluir);
                    }
                }, "cancelarPlanoContas");
                acaoExcluir = new MenuItem({
                    label: "Excluir",
                    disabled: desabilitar,
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            var planosDisponiveis = new Array();

                            clearMensagem();
                            if (consistirGridPlanoContas(dijit.byId('gridPlanoContas').store._arrayOfTopLevelItems, "excluir") == false) return false;

                            armazenaDadosGridPlanoContas(false);
                            destroyCreateGridPlanoContas();

                            planosDisponiveis = hasValue(dojo.byId('planoContasDisponivel').value) ? clone(dojo.byId('planoContasDisponivel').value) : [];
                            montarGridPlanoContas(planosDisponiveis, PLANO_CONTA_DISPONIVEL, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                            menuIncluirExcluir.addChild(acaoIncluir);
                            menuIncluirExcluir.removeChild(acaoExcluir);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                acaoIncluir = new MenuItem({
                    label: "Incluir",
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            var planosEmpresa = new Array();

                            clearMensagem();
                            if (consistirGridPlanoContas(dijit.byId('gridPlanoContas').store._arrayOfTopLevelItems, "incluir") == false) return false;

                            armazenaDadosGridPlanoContas(true);
                            destroyCreateGridPlanoContas();

                            planosEmpresa = hasValue(dojo.byId('planoContasEmpresa').value) ? clone(dojo.byId('planoContasEmpresa').value) : [];
                            montarGridPlanoContas(planosEmpresa, MEU_PLANO, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                            menuIncluirExcluir.addChild(acaoExcluir);
                            menuIncluirExcluir.removeChild(acaoIncluir);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menuIncluirExcluir.addChild(acaoExcluir);
                var acaoMeusPlanos = new MenuItem({
                    label: "Meu Plano de Contas",
                    onClick: function () {
                        try{
                            clearMensagem();
                            dojo.byId('tipoPlano').value = MEU_PLANO;
                            menuIncluirExcluir.addChild(acaoExcluir);
                            menuIncluirExcluir.removeChild(acaoIncluir);
                            destroyCreateGridPlanoContas();
                            var novoPlanoEmpresa = new Array();
                            var meusPlanosCtc = dojo.byId('planoContasEmpresa').value;
                            novoPlanoEmpresa = clone(manipulaDadosGridPlanoContas(meusPlanosCtc, false));
                            montarGridPlanoContas(novoPlanoEmpresa, MEU_PLANO, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoMeusPlanos);

                var acaoPlanosDisp = new MenuItem({
                    label: "Plano de Contas Disponível",
                    onClick: function () {
                        try{
                            clearMensagem();
                            dojo.byId('tipoPlano').value = PLANO_CONTA_DISPONIVEL;
                            menuIncluirExcluir.addChild(acaoIncluir);
                            menuIncluirExcluir.removeChild(acaoExcluir);
                            destroyCreateGridPlanoContas();
                            var novoPlanoDisponivel = new Array();
                            var planosDisponiveis = dojo.byId('planoContasDisponivel').value;
                          //  var marcar = dojo.byId('hasDisponivel').value;
                           // marcar = marcar == "false" ? false : true;
                            novoPlanoDisponivel = clone(manipulaDadosGridPlanoContas(planosDisponiveis, true));
                            montarGridPlanoContas(novoPlanoDisponivel, PLANO_CONTA_DISPONIVEL, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoPlanosDisp);

                button = new DropDownButton({
                    label: "Visões",
                    name: "acoesRelacionadasVisoes",
                    dropDown: menu,
                    id: "acoesRelacionadasVisoes"
                });
                dom.byId("linkSelecionadosPlanoContas").appendChild(button.domNode);
                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("dialogOrcamento").show();
                    }
                }, "incluirOrcamento");

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasExclusao",
                    dropDown: menuIncluirExcluir,
                    id: "acoesRelacionadasExclusao"
                });
                dom.byId("linkAcoesPlanoContas").appendChild(button.domNode);

                btnPesquisar(document.getElementById("pesquisarPlanoContas"));
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323039', '765px', '771px');
                        });
                }

                showCarregando();
            }
            catch (er) {
                postGerarLog(er);
            }
        })
    });
}

function manipulaDadosGridPlanoContas(arrayReturn, marcar) {
    try {
        var dadosGrid = arrayReturn;
        var retorno = new Array();
        //Montando os pais
    
        if (dadosGrid != null)
            for (var i = 0, l = 0; i < dadosGrid.length; i++) {
                if (dadosGrid[i].cd_conta != null)
                    retorno.push({
                        cd_conta: dadosGrid[i].cd_conta,
                        cd_grupo_conta: dadosGrid[i].cd_grupo_conta,
                        cd_subgrupo_conta: dadosGrid[i].cd_subgrupo_conta,
                        cd_subgrupo_pai: dadosGrid[i].cd_subgrupo_pai,
                        cd_plano_conta: dadosGrid[i].cd_plano_conta,
                        dc_conta: dadosGrid[i].dc_conta,
                        id: dadosGrid[i].id,
                        marcado: marcar,
                        pai: dadosGrid[i].pai,
                        id_conta_segura: false,
                        id_ativo: false
                    });
                l += 1;                //Montando os filhos
                if (dadosGrid[i].children != null && dadosGrid[i].children.length > 0) {
                    var novoChildrenPrimeiroNivel = new Array();

                    for (var j = 0, m = 0; j < dadosGrid[i].children.length; j++) {
                        novoChildrenPrimeiroNivel.push({
                            cd_conta: dadosGrid[i].children[j].cd_conta,
                            cd_grupo_conta: dadosGrid[i].children[j].cd_grupo_conta,
                            cd_subgrupo_conta: dadosGrid[i].children[j].cd_subgrupo_conta,
                            cd_subgrupo_pai: dadosGrid[i].children[j].cd_subgrupo_pai,
                            cd_plano_conta: dadosGrid[i].children[j].cd_plano_conta,
                            dc_conta: dadosGrid[i].children[j].dc_conta,
                            id: dadosGrid[i].children[j].id,
                            marcado: marcar,
                            pai: dadosGrid[i].children[j].pai,
                            id_tipo_conta: dadosGrid[i].children[j].id_tipo_conta,
                            id_conta_segura: dadosGrid[i].children[j].id_conta_segura,
                            id_ativo: true
                        });
                        m += 1;
                        //Monta os netos
                        if (dadosGrid[i].children[j].children != null && dadosGrid[i].children[j].children.length > 0) {
                            var novoChildrenSegundoNivel = new Array();
                            for (var k = 0; k < dadosGrid[i].children[j].children.length; k++) {
                                novoChildrenSegundoNivel.push({
                                    cd_conta: dadosGrid[i].children[j].children[k].cd_conta,
                                    cd_grupo_conta: dadosGrid[i].children[j].children[k].cd_grupo_conta,
                                    cd_subgrupo_conta: dadosGrid[i].children[j].children[k].cd_subgrupo_conta,
                                    cd_subgrupo_pai: dadosGrid[i].children[j].children[k].cd_subgrupo_pai,
                                    cd_plano_conta: dadosGrid[i].children[j].children[k].cd_plano_conta,
                                    dc_conta: dadosGrid[i].children[j].children[k].dc_conta,
                                    id: dadosGrid[i].children[j].children[k].id,
                                    marcado: marcar,
                                    pai: dadosGrid[i].children[j].children[k].pai,
                                    id_tipo_conta: dadosGrid[i].children[j].children[k].id_tipo_conta,
                                    id_conta_segura: dadosGrid[i].children[j].children[k].id_conta_segura,
                                    id_ativo: true
                                });
                            }
                            novoChildrenPrimeiroNivel[m - 1].children = novoChildrenSegundoNivel;
                        }
                    }
                    retorno[l - 1].children = novoChildrenPrimeiroNivel;
                }// if children
            }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaIdGridPlanoConta(dataRetorno) {
    try{
        var contador = 0;
        if(hasValue(dataRetorno)){
            for (var d = 0; d < dataRetorno.length; d++) {
                if(hasValue(dataRetorno[d].cd_conta[0]))
                    dataRetorno[d].cd_conta = dataRetorno[d].cd_conta[0];
                contador++;
                dataRetorno[d].id = contador;

                var segundo_nivel = dataRetorno[d].children;

                if (segundo_nivel != null) {
                    for (var e = 0; e < segundo_nivel.length; e++) {
                        if (hasValue(segundo_nivel[e].cd_conta[0]))
                            segundo_nivel[e].cd_conta = segundo_nivel[e].cd_conta[0];

                        contador++;
                        segundo_nivel[e].id = contador;

                        var terceiro_nivel = segundo_nivel[e].children;
                        if (terceiro_nivel != null) {
                            for (var f = 0; f < terceiro_nivel.length; f++) {
                                if (hasValue(terceiro_nivel[f].cd_conta[0]))
                                    terceiro_nivel[f].cd_conta = terceiro_nivel[f].cd_conta[0];

                                contador++;
                                terceiro_nivel[f].id = contador;
                            }
                            //quickSortObj(terceiro_nivel, 'dc_conta');
                        }
                    }
                    //quickSortObj(segundo_nivel, 'dc_conta');
                }
            }
            //quickSortObj(dataRetorno, 'dc_conta');
        }
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearChildrenLenthZero(dataRetorno) {
    try{
        for (var i = 0; i < dataRetorno.length; i++)
            if (dataRetorno[i].children.length > 0)
                for (var j = 0; j < dataRetorno[i].children.length; j++) {
                    if (dataRetorno[i].children[j].children != null && dataRetorno[i].children[j].children.length > 0) {
                        for (var m = 0; m < dataRetorno[i].children[j].children.length; m++)
                            delete dataRetorno[i].children[j].children[m].children;
                    } else delete dataRetorno[i].children[j].children;
                }
        return dataRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGridPlanoConta(tipoPlano, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel, hasDisponivel) {
    try {
        var planoConta = new PlanoContas(storeNivel, null, false);
        xhr.get({
            preventCache: true,
            handleAs: "json",
            url: Endereco() + "/api/financeiro/getGrupoContaArvore?cd_grupo_conta=" + planoConta.cd_grupo_conta + "&no_subgrupo_conta=" + planoConta.no_subGrupo + "&inicio=" + planoConta.inicio + "&nivel=" + planoConta.nivel_plano_conta + "&tipoPlanoConta=" + tipoPlano + "&marcar=" + hasDisponivel,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataRetorno) {
            try{
                var dataRetornoVisao = (tipoPlano == PLANO_CONTA_DISPONIVEL) ? dataRetorno.retorno[0] : dataRetorno.retorno[1];
                dojo.byId('planoContasDisponivel').value = [];
                dojo.byId('planoContasEmpresa').value = [];

                dojo.byId('planoContasDisponivel').value = dataRetorno.retorno[0];
                dojo.byId('planoContasEmpresa').value = dataRetorno.retorno[1];

                montarGridPlanoContas(dataRetornoVisao, tipoPlano, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function percorreGridForOrdering(dataRetorno) {
    try{
        for (var i = 0; i < dataRetorno.length; i++) {
            ordenaArvorePlanoContas(dataRetorno);
            if (dataRetorno[i].children.length > 0)
                for (var j = 0; j < dataRetorno[i].children.length; j++) {
                    if (dataRetorno[i].cd_grupo_conta[0] == dataRetorno[i].children[j].cd_grupo_conta[0])
                        ordenaArvorePlanoContas(dataRetorno[i].children);
                    if (dataRetorno[i].children[j].children != null && dataRetorno[i].children[j].children.length > 0)
                        for (var m = 0; m < dataRetorno[i].children[j].children.length; m++) 
                            if (dataRetorno[i].children[j].cd_subgrupo_conta[0] == dataRetorno[i].children[j].children[m].cd_subgrupo_pai[0])
                                ordenaArvorePlanoContas(dataRetorno[i].children[j].children);
                }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ordenaArvorePlanoContas(listaPlanoConta) {
    try{
        listaPlanoConta.sort(function byOrdem(a, b) { return a.cd_conta < b.cd_conta; });
        return listaPlanoConta;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGridPlanoContas(dataRetorno, tipoPlano, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel) {
    try {
        var titleName = tipoPlano == MEU_PLANO ? 'Meu Plano de Contas' : 'Plano de Contas Disponível';
        dojo.byId('tipoPlano').value = tipoPlano;
        dojo.byId('niveis').value = storeNivel;
        dataRetorno = atualizaIdGridPlanoConta(dataRetorno)
        clearChildrenLenthZero(dataRetorno);
        //percorreGridForOrdering(dataRetorno);

        var data = {
            identifier: 'id',
            label: 'dc_conta',
            items: dataRetorno
        };

        var store = new dojo.data.ItemFileWriteStore({ data: data });

        var model = new dijit.tree.ForestStoreModel({
            store: store, childrenAttrs: ['children']
        });

        var layout = [
          { name: titleName, field: 'dc_conta', width: '65%' },
          { name: 'Tipo Conta', field: 'id_tipo_conta', width: '10%', styles: "text-align: center;", formatter: formatDropDownTipoConta },
          { name: 'Conta Seg.', field: 'id_conta_segura', width: '10%', styles: "text-align: center;", formatter: formatCheckBoxContaSegura },
          { name: 'Ativo', field: 'id_ativo', width: '10%', styles: "text-align: center;", formatter: formatCheckBoxInativo },
          { name: ' ', field: 'marcado', width: '5%', styles: "text-align: center;", formatter: formatCheckBoxPlanoConta },// Disponível
          { name: '', field: 'id', width: '0%', styles: "display: none;" },
          { name: '', field: 'pai', width: '0%', styles: "display: none;" }
        ];
        if(hasValue(dijit.byId("gridPlanoContas"), true))
            dijit.byId("gridPlanoContas").destroy();
        var gridPlanoContas = new dojox.grid.LazyTreeGrid({
            id: 'gridPlanoContas',
            treeModel: model,
            structure: layout
        }, document.createElement('div'));

        dojo.byId("gridPlanoContas").appendChild(gridPlanoContas.domNode);
        gridPlanoContas.canSort = function (col) { return false; };
        gridPlanoContas.startup();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region searchPlanoConta - PlanoContas - montarListPlanos
function searchPlanoConta(visao, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect) {
    try{
        destroyCreateGridPlanoContas();
        loadGridPlanoConta(MEU_PLANO, xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function PlanoContas(storeNivel, tipo, persistir) {
    try{
        var listDisponiveis = new Array()
        this.nivel_plano_conta = storeNivel;
        this.disponivel = hasValue(dojo.byId('hasDisponivel').value) ? dojo.byId('hasDisponivel').value : false;
        if (!persistir) {
            this.cd_grupo_conta = 0;
            if (hasValue(dijit.byId('cbGrupoContas')))
                this.cd_grupo_conta = dijit.byId('cbGrupoContas').get('value') == "" ? 0 : dijit.byId('cbGrupoContas').get('value');
            if (hasValue(dijit.byId('cbNiveis')))
                this.nivel = dijit.byId('cbNiveis') == "" ? 1 : dijit.byId('cbNiveis').get('value');
            this.inicio = document.getElementById("inicioSubGrupo").checked;
            this.no_subGrupo = dojo.byId('lbSubGrupo').value;
        }

        this.tipoPlano = tipo;
        if (persistir) {
            var listPlanoContasEmpresa = dojo.byId('planoContasEmpresa').value;
            var listPlanoDisponiveis = dojo.byId('planoContasDisponivel').value;
            this.planosContasEmpresa = montarListPlanos(listPlanoContasEmpresa, storeNivel);
            this.planosContasDisponiveis = montarListPlanos(listPlanoDisponiveis, storeNivel);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//TODO
function montarListPlanos(listPlanos, storeNivel) {
    try {
        var listaRetorno = new Array();
        if (listPlanos != null && listPlanos.length > 0) {
            for (var i = 0; i < listPlanos.length; i++) {
                for (var j = 0; j < listPlanos[i].children.length; j++) {
                    if (listPlanos[i].children[j].children != null && storeNivel == 2) {
                        for (var k = 0; k < listPlanos[i].children[j].children.length; k++) {
                            listaRetorno.push({
                                cd_plano_conta: hasValue(listPlanos[i].children[j].children[k].cd_plano_conta[0]) ? listPlanos[i].children[j].children[k].cd_plano_conta[0] : listPlanos[i].children[j].children[k].cd_plano_conta,
                                cd_subgrupo_conta: hasValue(listPlanos[i].children[j].children[k].cd_subgrupo_conta[0]) ? listPlanos[i].children[j].children[k].cd_subgrupo_conta[0] : listPlanos[i].children[j].children[k].cd_subgrupo_conta,
                                id_conta_segura: hasValue(listPlanos[i].children[j].children[k].id_conta_segura[0]) ? listPlanos[i].children[j].children[k].id_conta_segura[0] : listPlanos[i].children[j].children[k].id_conta_segura,
                                cd_pessoa_empresa: 0,
                                id_ativo: hasValue(listPlanos[i].children[j].children[k].id_ativo[0]) ? listPlanos[i].children[j].children[k].id_ativo[0] : listPlanos[i].children[j].children[k].id_ativo,
                                id_tipo_conta: hasValue(listPlanos[i].children[j].children[k].id_tipo_conta[0]) ? listPlanos[i].children[j].children[k].id_tipo_conta[0] : listPlanos[i].children[j].children[k].id_tipo_conta
                            });
                        }
                    } else {
                        listaRetorno.push({
                            cd_plano_conta: listPlanos[i].children[j].cd_plano_conta[0],
                            cd_subgrupo_conta: listPlanos[i].children[j].cd_subgrupo_conta[0],
                            id_conta_segura: listPlanos[i].children[j].id_conta_segura[0],
                            cd_pessoa_empresa: 0,
                            id_ativo: hasValue(listPlanos[i].children[j].id_ativo[0]) ? listPlanos[i].children[j].id_ativo[0] : listPlanos[i].children[j].id_ativo,
                            id_tipo_conta: listPlanos[i].children[j].id_tipo_conta[0]
                        });
                    }
                }
            }

            return listaRetorno;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridPlanoContas() {
    try{
        if (hasValue(dijit.byId("gridPlanoContas"))) {
            dijit.byId("gridPlanoContas").destroy();
            $('<div>').attr('id', 'gridPlanoContas').appendTo('#paiGridPlanoConta');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region planoContasDisponivel - planoContasEmpresa -verificaIfExists  - armazenaDadosGridPlanoContas - consistirGridPlanoContas -hasChildrenChecked
function mergeplanoContasDisponivel(planosDisponiveis, isIncluir) {
    try{
        var novosPlanosDisponiveis = dojo.byId('planoContasDisponivel').value;//Novo plano que vai ser inserido.
        if (novosPlanosDisponiveis != null && novosPlanosDisponiveis.length > 0 && !isIncluir) 
            mergeDadosExistentes(planosDisponiveis, novosPlanosDisponiveis, "planoContasDisponivel")
        else 
            dojo.byId('planoContasDisponivel').value = planosDisponiveis;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mergePlanoContasEmpresa(planosEmpresa, isIncluir) {
    try{
        var novoPlanoContas = dojo.byId('planoContasEmpresa').value;//Novo plano que vai ser inserido.
        novoPlanoContas = manipulaDadosGridPlanoContas(novoPlanoContas, false);
        if (novoPlanoContas != null && novoPlanoContas.length > 0 && isIncluir) 
            mergeDadosExistentes(planosEmpresa, novoPlanoContas, "planoContasEmpresa")
        else 
            dojo.byId('planoContasEmpresa').value = planosEmpresa;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mergeDadosExistentes(dadosExistentes, novosDados, visao) {
    try{
        var dadosGrid = dijit.byId('gridPlanoContas').store._arrayOfTopLevelItems;
        var novaVisao = new Array();
        var planoPai = new Array();
        var planoFilho = new Array();
        var planoNeto = new Array();
        if (novosDados != null && novosDados.length > 0)
            novaVisao = cloneArray(novosDados);

        if (dadosExistentes.length > 0) {
            for (var i = 0; i < dadosExistentes.length; i++) {
                for (var j = 0, a = 0; j < novosDados.length; j++) {
                    var exists = verificaIfExists(dadosExistentes[i].cd_conta[0], novaVisao, PAI, 0, 0);
                    if (!exists) {
                        planoPai[a] = dadosExistentes[i];
                        $.merge(novaVisao, planoPai);
                        a++;
                    } else
                        for (var l = 0; l < dadosExistentes[i].children.length; l++) {
                            for (var m = 0, b = 0; m < novosDados[j].children.length; m++) {
                                if (dadosExistentes[i].children[l].cd_grupo_conta[0] == novosDados[j].children[m].cd_grupo_conta) {
                                    var exists = verificaIfExists(dadosExistentes[i].children[l].cd_conta[0], novaVisao, FILHO, dadosExistentes[i].cd_grupo_conta[0], 0);
                                    if (!exists) {
                                        planoFilho[b] = dadosExistentes[i].children[l];
                                        $.merge(novaVisao[j].children, planoFilho);
                                        b++;
                                    } else
                                        if (dadosExistentes[i].children[l].children != null && dadosExistentes[i].children[l].children.length > 0)
                                            for (var n = 0; n < dadosExistentes[i].children[l].children.length; n++) {
                                                if (novosDados[j].children[m].children != null && novosDados[j].children[m].children.length > 0)
                                                    for (var o = 0, c = 0; o < novosDados[j].children[m].children.length; o++) {
                                                        if (dadosExistentes[i].children[l].cd_subgrupo_conta[0] == novosDados[j].children[m].children[o].cd_subgrupo_pai) {
                                                            var exists = verificaIfExists(dadosExistentes[i].children[l].children[n].cd_conta[0], novaVisao, NETO, 0, dadosExistentes[i].children[l].cd_subgrupo_conta[0]);
                                                            if (!exists) {
                                                                planoNeto[c] = dadosExistentes[i].children[l].children[n];
                                                                $.merge(novaVisao[j].children[m].children, planoNeto);
                                                                c++;
                                                            }
                                                        }
                                                    }
                                            }
                                }
                            }
                        }
                }
            }
        }
        dojo.byId(visao).value = novaVisao;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaIfExists(idPlanoEmpresa, novosDados, tipoObjeto, idGrupo, idSubgrupPai) {
    try{
        var novoPlano = new Array();
        var exists = false;
        novoPlano = cloneArray(novosDados);
        if (tipoObjeto == PAI)
            for (var i in novoPlano) 
                if (idPlanoEmpresa == novoPlano[i].cd_conta) {
                    exists = true;
                    break;
                }

        if (tipoObjeto == FILHO)
            for (var j in novoPlano) 
                for (var l in novoPlano[j].children) 
                    if (idPlanoEmpresa == novoPlano[j].children[l].cd_subgrupo_conta && idGrupo == novoPlano[j].children[l].cd_grupo_conta) {
                        exists = true;
                        break;
                    }

        if (tipoObjeto == NETO)
            for (var m in novoPlano) 
                for (var n in novoPlano[m].children) 
                    if (novoPlano[m].children[n].children != null && novoPlano[m].children.length > 0)
                        for (var o in novoPlano[m].children[n].children) 
                            if (idPlanoEmpresa == novoPlano[m].children[n].children[o].cd_subgrupo_conta && idSubgrupPai == novoPlano[m].children[n].cd_subgrupo_conta) {
                                exists = true;
                                break;
                            }

        return exists;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function armazenaDadosGridPlanoContas(isIncluir) {
    try{
        var dadosGrid = dijit.byId('gridPlanoContas').store._arrayOfTopLevelItems;
        var marcar = isIncluir == true ? false : true;
        var marcado;
        dojo.byId('pesquisarBD').value = true;
            var retorno = new Array();
            //Montando os pais
            for (var i = 0, l = 0; i < dadosGrid.length; i++) {

                if (hasValue(dadosGrid[i].marcado[0]))
                    marcado = dadosGrid[i].marcado[0];
                else
                    marcado = dadosGrid[i].marcado;

                if ((marcado == true) && dadosGrid[i].cd_conta[0] != null) {
                    retorno.push({
                        cd_conta: dadosGrid[i].cd_conta,
                        cd_grupo_conta: dadosGrid[i].cd_grupo_conta,
                        cd_subgrupo_conta: dadosGrid[i].cd_subgrupo_conta,
                        cd_subgrupo_pai: dadosGrid[i].cd_subgrupo_pai,
                        cd_plano_conta: dadosGrid[i].cd_plano_conta,
                        dc_conta: dadosGrid[i].dc_conta,
                        id: dadosGrid[i].id,
                        marcado: marcar,
                        pai: dadosGrid[i].pai,
                        id_conta_segura: false,
                        id_ativo: false
                    });
                    l += 1;                //Montando os filhos
                    if (dadosGrid[i].children != null && dadosGrid[i].children.length > 0) {
                        var novoChildrenPrimeiroNivel = new Array();

                        for (var j = 0, m = 0; j < dadosGrid[i].children.length; j++) {

                            if (hasValue(dadosGrid[i].children[j].marcado[0]))
                                marcado = dadosGrid[i].children[j].marcado[0];
                            else
                                marcado = dadosGrid[i].children[j].marcado;

                            if (marcado == true) {
                                novoChildrenPrimeiroNivel.push({
                                    cd_conta: dadosGrid[i].children[j].cd_conta,
                                    cd_grupo_conta: dadosGrid[i].children[j].cd_grupo_conta,
                                    cd_subgrupo_conta: dadosGrid[i].children[j].cd_subgrupo_conta,
                                    cd_subgrupo_pai: dadosGrid[i].children[j].cd_subgrupo_pai,
                                    cd_plano_conta: dadosGrid[i].children[j].cd_plano_conta,
                                    dc_conta: dadosGrid[i].children[j].dc_conta,
                                    id: dadosGrid[i].children[j].id,
                                    marcado: marcar,
                                    pai: dadosGrid[i].children[j].pai,
                                    id_tipo_conta: dadosGrid[i].children[j].id_tipo_conta,
                                    id_conta_segura: dadosGrid[i].children[j].id_conta_segura,
                                    id_ativo: true
                                });
                                m += 1;
                                //Monta os netos
                                if (dadosGrid[i].children[j].children != null && dadosGrid[i].children[j].children.length > 0) {
                                    var novoChildrenSegundoNivel = new Array();
                                    for (var k = 0; k < dadosGrid[i].children[j].children.length; k++) {

                                        if (hasValue(dadosGrid[i].children[j].children[k].marcado[0]))
                                            marcado = dadosGrid[i].children[j].children[k].marcado[0];
                                        else
                                            marcado = dadosGrid[i].children[j].children[k].marcado;

                                        if (marcado == true) {
                                            novoChildrenSegundoNivel.push({
                                                cd_conta: dadosGrid[i].children[j].children[k].cd_conta,
                                                cd_grupo_conta: dadosGrid[i].children[j].children[k].cd_grupo_conta,
                                                cd_subgrupo_conta: dadosGrid[i].children[j].children[k].cd_subgrupo_conta,
                                                cd_subgrupo_pai: dadosGrid[i].children[j].children[k].cd_subgrupo_pai,
                                                cd_plano_conta: dadosGrid[i].children[j].children[k].cd_plano_conta,
                                                dc_conta: dadosGrid[i].children[j].children[k].dc_conta,
                                                id: dadosGrid[i].children[j].children[k].id,
                                                marcado: marcar,
                                                pai: dadosGrid[i].children[j].children[k].pai,
                                                id_tipo_conta: dadosGrid[i].children[j].children[k].id_tipo_conta,
                                                id_conta_segura: dadosGrid[i].children[j].children[k].id_conta_segura,
                                                id_ativo: true
                                            });
                                        }
                                    }
                                    novoChildrenPrimeiroNivel[m - 1].children = novoChildrenSegundoNivel;
                                }
                            }
                        }
                        retorno[l - 1].children = novoChildrenPrimeiroNivel;
                    }// if children
                }
            }//for i

            var planosDisponiveis = new Array();
            for (var a = 0, e = 0; a < dadosGrid.length; a++) {

                var cdConta = dadosGrid[a].cd_conta[0];
                var hasChecked = hasChildrenChecked(cdConta, 0);

                if (hasValue(dadosGrid[a].marcado[0] == false))
                    marcado = dadosGrid[a].marcado[0];
                else
                    marcado = dadosGrid[a].marcado;

                if ((marcado == false) || hasChecked == false) {
                    planosDisponiveis.push({
                        cd_conta: dadosGrid[a].cd_conta,
                        cd_grupo_conta: dadosGrid[a].cd_grupo_conta,
                        cd_subgrupo_conta: dadosGrid[a].cd_subgrupo_conta,
                        cd_subgrupo_pai: dadosGrid[a].cd_subgrupo_pai,
                        cd_plano_conta: dadosGrid[a].cd_plano_conta,
                        dc_conta: dadosGrid[a].dc_conta,
                        id: dadosGrid[a].id,
                        marcado: marcar,
                        pai: dadosGrid[a].pai,
                        id_conta_segura: false,
                        id_ativo: false
                    });
                    e += 1;

                    if (dadosGrid[a].children != null && dadosGrid[a].children.length > 0) {
                        var planoPrimeiroNivel = new Array();
                        for (var b = 0, f = 0; b < dadosGrid[a].children.length; b++) {

                            hasChecked = hasChildrenChecked(cdConta, dadosGrid[a].children[b].cd_conta[0]);

                            if (hasValue(dadosGrid[a].children[b].marcado[0] == false))
                                marcado = dadosGrid[a].children[b].marcado[0];
                            else
                                marcado = dadosGrid[a].children[b].marcado;

                            if ((marcado == false) || hasChecked == false) {
                                planoPrimeiroNivel.push({
                                    cd_conta: dadosGrid[a].children[b].cd_conta,
                                    cd_grupo_conta: dadosGrid[a].children[b].cd_grupo_conta,
                                    cd_subgrupo_conta: dadosGrid[a].children[b].cd_subgrupo_conta,
                                    cd_subgrupo_pai: dadosGrid[a].children[b].cd_subgrupo_pai,
                                    cd_plano_conta: dadosGrid[a].children[b].cd_plano_conta,
                                    dc_conta: dadosGrid[a].children[b].dc_conta,
                                    id: dadosGrid[a].children[b].id,
                                    marcado: marcar,
                                    pai: dadosGrid[a].children[b].pai,
                                    id_tipo_conta: dadosGrid[a].children[b].id_tipo_conta,
                                    id_conta_segura: dadosGrid[a].children[b].id_conta_segura,
                                    id_ativo: dadosGrid[a].children[b].id_ativo
                                });
                                f += 1;
                                //Monta os netos
                                if (dadosGrid[a].children[b].children != null && dadosGrid[a].children[b].children.length > 0) {
                                    var planoSegundoNivel = new Array();
                                    for (var c = 0; c < dadosGrid[a].children[b].children.length; c++) {

                                        if (hasValue(dadosGrid[a].children[b].children[c].marcado[0] == false))
                                            marcado = dadosGrid[a].children[b].children[c].marcado[0];
                                        else
                                            marcado = dadosGrid[a].children[b].children[c].marcado;

                                        if (marcado == false) {
                                            planoSegundoNivel.push({
                                                cd_conta: dadosGrid[a].children[b].children[c].cd_conta,
                                                cd_grupo_conta: dadosGrid[a].children[b].children[c].cd_grupo_conta,
                                                cd_subgrupo_conta: dadosGrid[a].children[b].children[c].cd_subgrupo_conta,
                                                cd_subgrupo_pai: dadosGrid[a].children[b].children[c].cd_subgrupo_pai,
                                                cd_plano_conta: dadosGrid[a].children[b].children[c].cd_plano_conta,
                                                dc_conta: dadosGrid[a].children[b].children[c].dc_conta,
                                                id: dadosGrid[a].children[b].children[c].id,
                                                marcado: marcar,
                                                pai: dadosGrid[a].children[b].children[c].pai,
                                                id_tipo_conta: dadosGrid[a].children[b].children[c].id_tipo_conta,
                                                id_conta_segura: dadosGrid[a].children[b].children[c].id_conta_segura,
                                                id_ativo: dadosGrid[a].children[b].children[c].id_ativo
                                            });
                                        }
                                    }
                                    planoPrimeiroNivel[f - 1].children = planoSegundoNivel;
                                }
                            }
                        }
                        planosDisponiveis[e - 1].children = planoPrimeiroNivel;
                    }// if children
                }
            }
              if (isIncluir) {
                mergePlanoContasEmpresa(retorno, isIncluir);
                mergeplanoContasDisponivel(planosDisponiveis, isIncluir);
            }
            else {
                mergePlanoContasEmpresa(planosDisponiveis, isIncluir);
                mergeplanoContasDisponivel(retorno, isIncluir);
              }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consistirGridPlanoContas(store, incluirExcluir) {
    try{
        var retorno = true;
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNaoExistsRegGrid);

        if (store.length == 0) {
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            retorno = false;
        }
        else {
            for (var i = 0; i < store.length; i++) {
                if (store[i].marcado[0] == false) {
                    for (var j = 0; j < store[i].children.length; j++) {
                        if (store[i].children[j].marcado[0] == false) retorno = false;
                        if (store[i].children[j].marcado[0] == false && store[i].children[j].children != null) {
                            for (var l = 0; l < store[i].children[j].children.length; l++) {
                                if (store[i].children[j].children[l].marcado[0] == false) {
                                    retorno = false;
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNaoExistsRegCheked + incluirExcluir + " no plano de contas.");
                                }
                                else {
                                    retorno = true;
                                    break;
                                }
                            }
                        } else if (store[i].children[j].marcado[0] == true) {
                            retorno = true;
                            break;
                        }
                    }
                } else {
                    retorno = true;
                    break;
                }
            }
        }
        if (retorno == false) apresentaMensagem("apresentadorMensagem", mensagensWeb);
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//verifica se  existe filhos marcados, e retorna um booleano
function hasChildrenChecked(cod_pai, cod_filho) {
    try{
        var hasFilhoChecked = true;
        var gridPlanoContas = dijit.byId('gridPlanoContas').store._arrayOfTopLevelItems;
        for (var i = 0; i < gridPlanoContas.length; i++) {
            if (gridPlanoContas[i].cd_conta == cod_pai) {
                if (cod_filho == 0) {
                    for (var j = 0; j < gridPlanoContas[i].children.length; j++) {
                        if (gridPlanoContas[i].children[j].marcado[0] == false) { // verifica se tem alguma conta filha desmarcada.
                            hasFilhoChecked = false;
                            break;
                        } else {//caso não haja nenhuma conta filha desmarcada verifica os filhos desta
                            if (gridPlanoContas[i].children[j].children != null) {
                                for (var k = 0; k < gridPlanoContas[i].children[j].children.length; k++) {
                                    if (gridPlanoContas[i].children[j].children[k].marcado[0] == false) {
                                        hasFilhoChecked = false;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else {
                    for (var j = 0; j < gridPlanoContas[i].children.length; j++) {
                        if (gridPlanoContas[i].children[j].cd_conta == cod_filho) {
                            if (gridPlanoContas[i].children[j].marcado[0] == false) { // verifica se tem alguma conta filha desmarcada.
                                hasFilhoChecked = false;
                                break;
                            } else { //caso não haja nenhuma conta filha desmarcada verifica os filhos desta
                                if (gridPlanoContas[i].children[j].children != null) {
                                    for (var k = 0; k < gridPlanoContas[i].children[j].children.length; k++) {
                                        if (gridPlanoContas[i].children[j].children[k].marcado[0] == false) {
                                            hasFilhoChecked = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return hasFilhoChecked;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region   Persistência - salvarPlanoConta - cancelarPlanoConta
function salvarPlanoConta() {
    try {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', null);
        var tipoGrupoConta = false;
        var nivel = hasValue(dojo.byId('niveis').value) ? dojo.byId('niveis').value : 2;
        var Plano = new PlanoContas(nivel, MEU_PLANO, true);
        if (hasValue(Plano.planosContasDisponiveis) || hasValue(Plano.planosContasEmpresa))
            require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, domAttr, xhr, ref, window) {
                xhr.post(Endereco() + "/api/financeiro/PostPlanoConta", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson(Plano)
                }).then(function (data) {
                    try{
                        if (!hasValue(data.erro)) {
                            dojo.byId('wasPersisted').value = true;
                            var itemAlterado = data.retorno;
                            apresentaMensagem('apresentadorMensagem', data);
                        } else
                            apresentaMensagem('apresentadorMensagem', data);
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                    showCarregando();
                },
                function (error) {
                    showCarregando();
                    dojo.byId('wasPersisted').value = false;
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function cancelarPlanoConta(hasGrupoConta, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel, hasDisponivel, menuIncluirExcluir) {
    try {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, domAttr, xhr, ref, window) {
            xhr.post(Endereco() + "/api/financeiro/postCancelarPlanoContas?nivel=" + storeNivel, {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(null)
            }).then(function (dataRetorno) {
                try {
                    if (!hasValue(dataRetorno.erro)) {
                        var planoConstas = dataRetorno.retorno;

                        dojo.byId('planoContasDisponivel').value = [];
                        dojo.byId('planoContasEmpresa').value = [];
                        dojo.byId('planoContasDisponivel').value = clone(planoConstas[0]);
                        dojo.byId('planoContasEmpresa').value = clone(planoConstas[1]);

                        destroyCreateGridPlanoContas();

                        if (planoConstas[1] != null && planoConstas[1].length > 0) {
                            montarGridPlanoContas(planoConstas[1], MEU_PLANO, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                            menuIncluirExcluir.addChild(acaoExcluir);
                            menuIncluirExcluir.removeChild(acaoIncluir);

                        }
                        else {
                            montarGridPlanoContas(planoConstas[0], PLANO_CONTA_DISPONIVEL, xhr, dom, _registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, ready, FilteringSelect, storeNivel);
                            menuIncluirExcluir.addChild(acaoIncluir);
                            menuIncluirExcluir.removeChild(acaoExcluir);
                        }
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
                showCarregando();
            },
           function (error) {
               showCarregando();
               apresentaMensagem('apresentadorMensagem', error.response.data);
           });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion