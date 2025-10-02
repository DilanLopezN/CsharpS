var ENTRADA = 1, SAIDA = 2, DESPESAS = 3, SERVICO = 4, DEVOLUCAO = 5;
var BIBLIOTECA = 48, ORIGMATRICULA = 22, BAIXAFINANCEIRA = 47;
var TIPOMOVIMENTO = null, ORIGEMCHAMADONF = 0, TIPOFINANCEIROTITULO = 3, TIPOFINANCEIROCHEQUE = 4;
var PESQUISA_MOVIMENTO = 1, PESQUISA = 1, CADASTRO = 2;
var TIPOITEMSERVICO = 5; APLICACAODIRETA = 6;
var NOVO = 1, EDITAR = 2, EXCLUIR = 3;
var MOVIMENTO = 4, MOVIMENTOORIGEM = 69;
var RECEBER = 1, PAGAR = 2, TITULO_ABERTO = 1, TITULO_FECHADO = 2;
var gerar_titulo = false, numeracao_NF_automatica = false, emitir_nf_servico = false, emitir_nf_mercantil = false, empresa_propria = false;
var obrigar_plano_conta = false, plano_conta_automatico = false;
var TIPO_PESQUISA = 0;
var AlteraDesc = true;
var TIPO_SAIDA = 2, TIPO_ENTRADA = 1;
var NF_ABERTO = 1, NF_FECHADO = 2, NF_CANCELADO = 3, SITUACAOTRIBUTARIAPIS = 65, SITUACAOTRIBUTARIACOFINS = 107, SITUACAOTRIBUTARIAPIS_OUTRASOP = 90, SITUACAOTRIBUTARIACOFINS_OUTRASOP = 132;
var ISENTO = 1, REDUZIDO = 3;
var MOVIMENTO_DEVOLUCAO = 15;
var origem = 0;
var MOVIMENTO = 2;
var STATUS_NF_FECHADO = 2, STATUS_NF_CANCELADO = 3;
var alterou_tp_nf = false;
var regime_tributario = 0;
var REGIME_NORMAL = 3;
var pcAliqAproxSaida = 0, pcAliqAproxServico = 0;
var CHEQUE = 4;
var CARTAO = 5;
var CHEQUEPREDATADO = 4, CHEQUEVISTA = 10, EVENTO_GRID_CHEQUE = 0, ISVIEW_CHEQUE_TRANSACAO = null;
var CARTAOCREDITO = 2, CARTAODEBITO = 3;
var TROCA_FINANCEIRA = 110;

var MATERIAL_DIDATICO = 1;
var SETAR_TIPO = true;
var QTD_MATERIAL = 0;
var CONSULTA_INICIAL = true;
var CONSULTA_PESSOA = false;
var CONSULTA_ITEM = false;
var CONSULTA_ALUNO = false;
//Configura qual tela será configurada.

function configurarMovimento() {
    try {
        var parametros = getParamterosURL();
        dojo.byId("tdLblPesqStatusNF").style.display = "none";
        dojo.byId("tdPesqStatusNF").style.display = "none";
        dijit.byId('ckVendaFuturaCad').set('disabled', true);
        if (hasValue(parametros['tipo'])) {
            switch (eval(parametros['tipo'])) {
                case ENTRADA:
                    document.getElementById("labelTitulos").innerHTML = "Entrada";
                    document.getElementById("titulo").innerHTML = "Entrada";
                    document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Movimento de Entrada";
                    dijit.byId("fkPessoaPesq").set("title", "Fornecedor");
                    document.getElementById("lblPessoaCad").innerHTML = "Fornecedor:";
                    document.getElementById("tipoPessoaPesqlbl").innerHTML = "Fornecedor:";
                    document.getElementById("lblNumeroPesquisa").innerHTML = "Número Entrada:";
                    document.getElementById("lblNroMvto").innerHTML = "Número Entrada:";
                    document.getElementById("lblPolComer").innerHTML = "Forma de Pagamento:";
                    dijit.byId("fkPoliticaComercial").set("title", "Forma de Pagamento");
                    //dijit.byId("nfEsc").set("checked", false);
                    dijit.byId("nfEsc").set("disabled", false);
                    dojo.byId("trALuno").style.display = "none";
                    dojo.byId("trCurso").style.display = "none";
                    dijit.byId("tpContrato").set("required", false);
                    dijit.byId("noCursoFKMovimento").set("required", false);
                    TIPOMOVIMENTO = ENTRADA;
                    break;
                case SAIDA:
                    document.getElementById("lblNumeroPesquisa").innerHTML = "Número Saída:";
                    document.getElementById("lblNroMvto").innerHTML = "Número Saída:";
                    document.getElementById("labelTitulos").innerHTML = "Saída";
                    document.getElementById("titulo").innerHTML = "Saída";
                    document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Movimento de Saída";
                    dijit.byId("fkPessoaPesq").set("title", "Cliente");
                    document.getElementById("lblPessoaCad").innerHTML = "Cliente:";
                    document.getElementById("tipoPessoaPesqlbl").innerHTML = "Cliente:";
                    document.getElementById("lblPolComer").innerHTML = "Política de Saída:";
                    dijit.byId("fkPoliticaComercial").set("title", "Política de Saída");
                    dojo.byId("trALuno").style.display = ""; //Foi liberado pois tem venda de outros materiais para o Aluno que precisa aparecer no histórico
                    dojo.byId("trCurso").style.display = "none";
                    dijit.byId("tpContrato").set("required", false);
                    dijit.byId("noCursoFKMovimento").set("required", false);
                    TIPOMOVIMENTO = SAIDA;
                    ORIGEMCHAMADONF = verificarOrigemChamadoNF(parametros);
                    if (hasValue(parametros['id_material_didatico']) && eval(parametros['id_material_didatico']) == 1) {
                        dojo.byId("trALuno").style.display = ""; 
                        dojo.byId("trCurso").style.display = "";
                        document.getElementById("labelTitulos").innerHTML = "Cadastro de Movimento de Venda de Material Didático";
                        document.getElementById("titulo").innerHTML = "Cadastro de Movimento de Venda de Material Didático";
                        document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Movimento de Venda de Material Didático";
                        dijit.byId("tpContrato").set("required", true);
                        dijit.byId("noCursoFKMovimento").set("required", true);
                        dojo.byId("vinculadoCursoTitulo").style.display = "";
                        dojo.byId("vinculadoCursoCampo").style.display = "";
                        dojo.byId("notDisplayViculadoCurso").style.display = "none";

                    }

                    break;
                case SERVICO:
                    document.getElementById("lblNumeroPesquisa").innerHTML = "Número Serviço:";
                    document.getElementById("lblNroMvto").innerHTML = "Número Serviço:";
                    document.getElementById("labelTitulos").innerHTML = "Serviço";
                    document.getElementById("titulo").innerHTML = "Serviço";
                    document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Serviço";
                    dijit.byId("fkPessoaPesq").set("title", "Cliente");
                    document.getElementById("lblPessoaCad").innerHTML = "Cliente:";
                    document.getElementById("tipoPessoaPesqlbl").innerHTML = "Cliente:";
                    document.getElementById("lblPolComer").innerHTML = "Política de Serviço:";
                    dijit.byId("fkPoliticaComercial").set("title", "Política de Serviço");
                    dojo.byId("trALuno").style.display = "";
                    dojo.byId("trCurso").style.display = "none";
                    dijit.byId("tpContrato").set("required", false);
                    dijit.byId("noCursoFKMovimento").set("required", false);
                    
                    TIPOMOVIMENTO = SERVICO;
                    ORIGEMCHAMADONF = verificarOrigemChamadoNF(parametros);
                    break;
                case DESPESAS:
                    document.getElementById("lblNumeroPesquisa").innerHTML = "Número Despesa:";
                    document.getElementById("lblNroMvto").innerHTML = "Número Despesa:";
                    dijit.byId("fkPessoaPesq").set("title", "Fornecedor");
                    document.getElementById("lblPessoaCad").innerHTML = "Fornecedor:";
                    document.getElementById("tipoPessoaPesqlbl").innerHTML = "Fornecedor:";
                    document.getElementById("labelTitulos").innerHTML = "Despesa";
                    document.getElementById("titulo").innerHTML = "Despesa";
                    document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Despesa";
                    document.getElementById("lblPolComer").innerHTML = "Forma de Pagamento:";
                    dijit.byId("fkPoliticaComercial").set("title", "Forma de Pagamento");
                    dojo.byId("trALuno").style.display = "none";
                    dojo.byId("trCurso").style.display = "none";
                    //Desabilitando o campo
                    dojo.byId("tdLblNF").style.display = "none";
                    dojo.byId("tdCkNF").style.display = "none";
                    dojo.byId("tdPesqCkNF").style.display = "none";
                    dojo.byId("tdLblPesqCkNF").style.display = "none";
                    dijit.byId("tpContrato").set("required", false);
                    dijit.byId("noCursoFKMovimento").set("required", false);

                    TIPOMOVIMENTO = DESPESAS;
                    break;
                case DEVOLUCAO:
                    document.getElementById("labelTitulos").innerHTML = "Devolução";
                    document.getElementById("titulo").innerHTML = "Devolução";
                    document.getElementById("cadMovimento_title").innerHTML = "Cadastro de Movimento de Devolução";
                    dijit.byId("fkPessoaPesq").set("title", "Fornecedor");
                    document.getElementById("lblPessoaCad").innerHTML = "Fornecedor:";
                    document.getElementById("tipoPessoaPesqlbl").innerHTML = "Fornecedor:";
                    document.getElementById("lblNumeroPesquisa").innerHTML = "Número Devolução:";
                    document.getElementById("lblNroMvto").innerHTML = "Número Devolução:";
                    document.getElementById("lblPolComer").innerHTML = "Forma de Pagamento:";
                    dijit.byId("fkPoliticaComercial").set("title", "Forma de Pagamento");
                    //dijit.byId("ckNotaFiscal").set("checked", true);
                    //dijit.byId("ckNotaFiscal").set("disabled", true);
                    //Liberar para Devolver Movimento
                    //dijit.byId("nfEsc").set("checked", true);
                    //dijit.byId("nfEsc").set("disabled", true);
                    dojo.byId("trALuno").style.display = "none";
                    dojo.byId("trCurso").style.display = "none";
                    dijit.byId("tpContrato").set("required", false);
                    dijit.byId("noCursoFKMovimento").set("required", false);
                    TIPOMOVIMENTO = DEVOLUCAO;
                    break;
            }
            configuraLayoutMovimento();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configLayoutOpçoesMovimento(tipoMvto, loadPage, emitir_nf_servico, emitir_nf_mercantil) {
    switch (tipoMvto) {
        case ENTRADA:
            document.getElementById("lblNroMvto").innerHTML = "Número Entrada:";
            if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                dijit.byId("ckNotaFiscalPesq").set("checked", true);
            }
            if (loadPage) {
                dijit.byId("ckNotaFiscal").set("checked", emitir_nf_mercantil);
                dijit.byId("ckNotaFiscalPesq").set("checked", emitir_nf_mercantil);
            }
            break;
        case SAIDA:
            document.getElementById("lblNroMvto").innerHTML = "Número Saída:";
            // Para chegar somente a NF de saída.
            if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                dijit.byId("ckNotaFiscalPesq").set("checked", true);
            }
            if (loadPage) {
                dijit.byId("ckNotaFiscal").set("checked", emitir_nf_mercantil);
                dijit.byId("ckNotaFiscalPesq").set("checked", emitir_nf_mercantil);
            }
            break;
        case SERVICO:
            document.getElementById("lblNroMvto").innerHTML = "Número Serviço:";

            //dijit.byId("ckNotaFiscal").set("disabled", true);
            if (hasValue(emitir_nf_servico) && emitir_nf_servico) {
                dijit.byId("ckNotaFiscalPesq").set("checked", true);
            }
            if (loadPage) {
                dijit.byId("ckNotaFiscal").set("checked", emitir_nf_servico);
                dijit.byId("ckNotaFiscalPesq").set("checked", emitir_nf_servico);
            }
            break;
        case DESPESAS:
            document.getElementById("lblNroMvto").innerHTML = "Número Despesa:";
            break;
        case DEVOLUCAO:
            document.getElementById("lblNroMvto").innerHTML = "Número Devolução:";
            if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                dijit.byId("ckNotaFiscalPesq").set("checked", true);
            }
            if (loadPage) {
                dijit.byId("ckNotaFiscal").set("checked", emitir_nf_mercantil);
                dijit.byId("ckNotaFiscalPesq").set("checked", emitir_nf_mercantil);
            }

            //dijit.byId("ckNotaFiscal").set("checked", true);
            //dijit.byId("ckNotaFiscalPesq").set("checked", true);
            //dijit.byId("ckNotaFiscal").set("disabled", true);
            //dijit.byId("ckNotaFiscalPesq").set("disabled", true);
            //dijit.byId("nfEsc").set("checked", true);
            //dijit.byId("nfEsc").set("disabled", true);
            break;

    }
    if (dijit.byId(ckNotaFiscalPesq).checked && tipoMvto != DESPESAS)
        dijit.byId("gridMovimento").layout.setColumnVisibility(10, true);
    else
        dijit.byId("gridMovimento").layout.setColumnVisibility(10, false);
    if (!empresa_propria) {
        dijit.byId('tagNfeS').toggleable = false;
        dojo.byId("tagNfeS").style.display = "none";
    }
}

function verificarOrigemChamadoNF(parametros) {
    var tipoChamadoNF = 0;
    if (hasValue(parametros) && hasValue(parametros['origemNF'])) {
        switch (eval(parametros['origemNF'])) {
            case BIBLIOTECA:
                tipoChamadoNF = BIBLIOTECA;
                break;
            case ORIGMATRICULA:
                tipoChamadoNF = ORIGMATRICULA;
                break;
            case BAIXAFINANCEIRA:
                tipoChamadoNF = BAIXAFINANCEIRA;
                break;
        }
    }
    return tipoChamadoNF
}

function configuraLayoutMovimento() {
    require([
       "dojo/ready",
       "dojo/on",
    ], function (ready, on) {
        ready(function () {
            try {
                var compCheckEmissao = dijit.byId("ckEmissao");
                var compCheckMovimento = dijit.byId("ckMovimento");
                var tipo = TIPOMOVIMENTO;
                if (tipo == DEVOLUCAO)
                    tipo = dojo.byId('id_natureza_movto').value;
                if (tipo != ENTRADA) {
                    compCheckMovimento.set("checked", false);
                    compCheckEmissao.set("checked", true);
                    dojo.style("tdLblEmissao", "display", "none");
                    dojo.style("tdEmissao", "display", "none");
                    dojo.style("tdlblMovto", "display", "none");
                    dojo.style("tdMovto", "display", "none");
                    //dojo.byId("paneDataInicial").innerText = "Período de Emissão";
                    dijit.byId("paneDataInicial").set("Title", "Período de Emissão");
                } else {
                    compCheckMovimento.set("checked", false);
                    compCheckEmissao.set("checked", true);
                    compCheckEmissao.on("change", function (e) {
                        try {
                            if (!e)
                                compCheckMovimento.set("checked", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    compCheckMovimento.on("change", function (e) {
                        try {
                            if (!e)
                                compCheckEmissao.set("checked", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function destroyCreateGridTitulos() {
    try {
        if (hasValue(dijit.byId("gridTitulo"))) {
            dijit.byId("gridTitulo").destroyRecursive();
            $('<div>').attr('id', 'gridTitulo').attr('style', 'height:310px;').appendTo('#PaiGridTitulo');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextQtdKit(value, rowIndex, obj) {
    try {
        var gridKit = dijit.byId("gridKit");
        var icon;
        var desc = obj.field + '_input_' + gridKit._by_idx[rowIndex].item.cd_item_kit;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input type='number' max='1000000' id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxValorGridKit('" + value + "', '" + desc + "','" + gridKit._by_idx[rowIndex].item.qt_item_kit + "','" + gridKit._by_idx[rowIndex].item.cd_item_kit + "'," + rowIndex + ")", 3);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxValorGridKit(value, desc, itemValue, cd_item_kit, rowIndex) {
    try {
        var desabilitar = false;
        if (value == undefined || isNaN(parseInt(value))) value = '1';

        if (!hasValue(dijit.byId(desc))) {
            var newTextBox = new dijit.form.NumberSpinner({
                name: "textBox" + desc,
                disabled: desabilitar,
                //value: unmaskFixed(value, 2),
                old_value: value,
                maxlength: 6,
                style: "width: 100%;",
                type: "number",
                onBlur: function (b) {
                    //$('#' + desc).focus();
                },
                onChange: function (b) {
                    //if (b != itemValue)
                    atualizarValoresGridKit(desc, this, rowIndex, b, cd_item_kit);
                },
                smallDelta: 1,
                constraints: { min: 0, places: 0 }
            }, desc);
            newTextBox._onChangeActive = false;
            newTextBox.set('value', value);
            newTextBox.value = value;
            newTextBox._onChangeActive = true;
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraInt(document.getElementById(desc));
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarValoresGridKit(desc, obj, rowIndex, qt_item_kit, cd_item_kit) {
    try {
        var gridItemKit = dijit.byId("gridKit");
        var item = getItemStoreGridKit(gridItemKit, rowIndex);
        var objDijit = dijit.byId(obj.id);

        if (objDijit.value == 0)
            objDijit.value = 1;

        var ultimo_valor_kit = gridItemKit.store.objectStore.data[rowIndex].qt_item_kit;
        insertQtdGridKit(item, objDijit.old_value, objDijit.value, rowIndex, gridItemKit);

        calcularQuantidadeItemKit(cd_item_kit, objDijit.value, objDijit.old_value, ultimo_valor_kit);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getItemStoreGridKit(grid, id) {
    try {
        if (hasValue(grid.store.objectStore.data[id])) {
            return grid.store.objectStore.data[id];
        }
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function insertQtdGridKit(item, valorAntigo, valorAtual, index, grid) {
    try {
        if (isNaN(valorAtual) || !hasValue(valorAtual, true)) {
            grid.store.objectStore.data[index].qt_item_kit = valorAntigo;
            return;
        }
        else {
            grid.store.objectStore.data[index].qt_item_kit = valorAtual;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroMovto(permissoes) {
    //Criação da Grade de sala
    require([
    "dojo/_base/xhr",
    "dojo/dom",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/query",
    "dojo/dom-attr",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dojo/ready",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/_base/array",
    "dijit/MenuSeparator",
    "dojox/json/ref",
    "dojo/dom-style"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button, ready, on, ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, array, MenuSeparator, ref, domStyle) {
        ready(function () {
            try {
                if (TIPOMOVIMENTO != ENTRADA) {
                    dojo.byId('lb_importa_xml').style.display = "none";
                    dojo.byId('td_ck_import_xml').style.display = "none";
                } 

                inserirIdTabsCadastro();
                statusNF();
                loadSituacaoPesquisa();
                xhr.get({
                    url: Endereco() + "/api/escola/getParametrosMovimento",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        var parametrosTela = getParamterosURL();

                        obrigar_plano_conta = data.retorno.id_requer_plano_contas_mov;
                        numeracao_NF_automatica = data.retorno.id_numero_nf_automatico;
                        emitir_nf_servico = data.retorno.id_emitir_nf_servico;
                        emitir_nf_mercantil = data.retorno.id_emitir_nf_mercantil;
                        pcAliqAproxSaida = data.retorno.pc_aliquota_ap_saida;
                        pcAliqAproxServico = data.retorno.pc_aliquota_ap_servico;
                        if (obrigar_plano_conta)
                            dijit.byId("descPlanoConta").set("required", true);
                        if (hasValue(data) && hasValue(data.retorno) && hasValue(data.retorno.Escola))
                            empresa_propria = data.retorno.Escola.id_empresa_propria;
                        dojo.byId("tagCheque").style.display = "none";

                        var notaFiscalConfig = false;
                        switch (TIPOMOVIMENTO) {
                            case ENTRADA:
                                if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil)
                                    notaFiscalConfig = true;
                                break;
                            case SAIDA:
                                if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil)
                                    notaFiscalConfig = true;
                                break;
                            case SERVICO:
                                if (hasValue(emitir_nf_servico) && emitir_nf_servico)
                                    notaFiscalConfig = true;
                                break;
                            case DEVOLUCAO:
                                notaFiscalConfig = true;
                        }
                        //(int cd_tipo_movimento, int cd_pessoa, int cd_item, int cd_plano_conta, int numero, string serie,bool emissao, bool movimento, string dtInicial, string dtFinal)

                        //var myStore =
                        //               Cache(
                        //                       JsonRest({
                        //                           target: Endereco() + "/api/fiscal/getMovimentoSearch?id_tipo_movimento=" + parseInt(TIPOMOVIMENTO) + "&cd_pessoa=" + parseInt(0) +
                        //                           "&cd_item=" + parseInt(0) + "&cd_plano_conta=" + parseInt(0) + "&numero=" + parseInt(0) +
                        //                           "&serie=&emissao=false&movimento=false&dtInicial=&dtFinal&nota_fiscal=" + notaFiscalConfig + "&statusNF=0" +
                        //                               "&isImportXML=" + (TIPOMOVIMENTO != ENTRADA ? 0 : (dijit.byId("ckImportNota").checked) ? 1 : 0) +
                        //                               "&id_material_didatico=" + (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false) +
                        //                               "&id_venda_futura=" + dijit.byId('ckVendaFutura').checked,
                        //                           handleAs: "json",
                        //                           preventCache: true,
                        //                           headers: { "Accept": "application/json", "Authorization": Token() }
                        //                       }), Memory({}));

                        var gridMovimento = new EnhancedGrid({
                            store: dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoMovto", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                                { name: "Número", field: "dc_numero_serie", width: "8%", styles: "min-width:80px;" },
                                { name: "Movimento", field: "dc_natureza_tipo_nf", width: "10%", styles: "min-width:80px;" },
                                { name: "Pessoa", field: "no_pessoa", width: "20%", styles: "min-width:80px;" },
                                { name: "Emissão", field: "dta_emissao_movimento", width: "7%", styles: "min-width:80px;" },
                                { name: "Movimento", field: "dta_mov_movimento", width: "7%", styles: "min-width:80px;" },
                                { name: "Vencimentos", field: "dc_datas_titulos", width: "11%", styles: "min-width:80px;" },
                                { name: "Total Geral", field: "vl_qtd_total_geral", width: "6%", styles: "text-align:right;" },
                                { name: "Tipo Financeiro", field: "dc_tipo_financeiro", width: "10%", styles: "min-width:80px;" },
                                { name: "Política Comercial", field: "dc_politica_comercial", width: "10%", styles: "min-width:80px;" },
                                { name: "Status NF", field: "status_nf_pesq", width: "7%", styles: "text-align:center; min-width:80px;" },
                                { name: "Futura", field: "dc_futura", width: "4%", styles: "text-align:center; min-width:80px;" }
                            ],
                            canSort: true,
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
                                    position: "button"
                                }
                            }
                        }, "gridMovimento");
                        gridMovimento.startup();
                        gridMovimento.pagination.plugin._paginator.plugin.connect(gridMovimento.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridMovimento, 'cd_movimento', 'selecionaTodos');
                        });
                        configLayoutOpçoesMovimento(TIPOMOVIMENTO, true, emitir_nf_servico, emitir_nf_mercantil);
                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridMovimento, "_onFetchComplete", function () {
                                // Configura o check de todos:
                                if (dojo.byId('selecionaTodos').type == 'text')
                                    setTimeout("configuraCheckBox(false, 'cd_movimento', 'selecionadoMovto', -1, 'selecionaTodos', 'selecionaTodos', 'gridMovimento')", gridMovimento.rowsPerPage * 3);
                            });
                        });
                        gridMovimento.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 6 && Math.abs(col) != 7 };
                        gridMovimento.on("RowDblClick", function (evt) {
                            try {
                                var idx = evt.rowIndex,
                                        item = this.getItem(idx),
                                        store = this.store;
                                editarMovimento(item, (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false));

                                dojo.byId('divKit').style.display = "none";
                                dojo.byId('divItem').style.display = "block";
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);
                        if (TIPOMOVIMENTO == SAIDA &&
                            ((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1)) {
                            dijit.byId("gridMovimento").layout.setColumnVisibility(11, true);
                            dojo.byId("trFutura").style.display = "";

                        }
                        else {
                            dijit.byId("gridMovimento").layout.setColumnVisibility(11, false);
                            dojo.byId("trFutura").style.display = "none";
                        }

                        //Habilita ou desabilita a coluna "Movimento" da grade de movimentos.
                        if (tipo != SERVICO && tipo != DEVOLUCAO)
                            gridMovimento.layout.setColumnVisibility(2, false);
                        if (tipo == DEVOLUCAO)
                            tipo = dojo.byId('id_natureza_movto').value;
                        if (tipo != ENTRADA)
                            gridMovimento.layout.setColumnVisibility(5, false);
                        var tipo = TIPOMOVIMENTO;
                        switch (TIPOMOVIMENTO) {
                            case ENTRADA:
                                gridMovimento.getCell(1).name = "Número Entrada";
                                gridMovimento.getCell(3).name = "Fornecedor";
                                gridMovimento.getCell(9).name = "Forma de Pagamento";
                                break;
                            case SAIDA:
                                gridMovimento.getCell(1).name = "Número Saída";
                                gridMovimento.getCell(3).name = "Cliente";
                                gridMovimento.getCell(9).name = "Política de Saída";
                                break;
                            case SERVICO:
                                gridMovimento.getCell(1).name = "Número Serviço";
                                gridMovimento.getCell(3).name = "Cliente";
                                gridMovimento.getCell(9).name = "Política de Serviço";
                                break;
                            case DESPESAS:
                                gridMovimento.getCell(1).name = "Número Despesa";
                                gridMovimento.getCell(3).name = "Fornecedor";
                                gridMovimento.getCell(9).name = "Forma de Pagamento";
                                break;
                            case DEVOLUCAO:
                                gridMovimento.getCell(1).name = "Número Devolução";
                                gridMovimento.getCell(3).name = "Fornecedor";
                                gridMovimento.getCell(9).name = "Forma de Pagamento";
                                break;
                        }
                        // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
                        if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                            var ieversion = new Number(RegExp.$1)
                            if (ieversion == 7)
                                // Para IE7
                                dojo.byId('cadMovimento').childNodes[1].style.height = '100%';
                        }

                        var gridItem = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure:
                            [
                                { name: "<input id='selecionaTodosItem' style='display:none'/>", field: "selecionadoItem", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxItem },
                                { name: "Itens", field: "dc_item_movimento", width: "20%" },
                                { name: "Qtde.", field: "qt_item_movimento", width: "7%", styles: "text-align:center; min-width:15px; max-width:20px;" },
                                { name: "R$ Unitário", field: "vlUnitarioItem", width: "10%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                                { name: "Desc. (%)", field: "pcDescontoItem", width: "9%", styles: "text-align:right; min-width:15px; max-width:20px;" },
							    { name: "Desc.", field: "vlDescontoItem", width: "6%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                                { name: "Valor Total", field: "vlTotalItem", width: "10%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                                { name: "Valor Líquido", field: "vlLiquidoItem", width: "12%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                                { name: "Plano de Contas", field: "dc_plano_conta", width: "15%" },
                                { name: "Grupo de Estoque", field: "cd_grupo_estoque", width: "15%", styles: "display: none;" }
                            ],
                            canSort: true,
                            noDataMessage: "Nenhum registro encontrado.",
                            contentEditable: function (col) { return false; },
                        }, "gridItem");
                        gridItem.canSort = function (col) { return Math.abs(col) != 1; };
                        gridItem.startup();

                        require(["dojo/aspect"],
                            function (aspect) {
                                aspect.after(gridItem,
                                    "_onFetchComplete",
                                    function () {
                                        // Configura o check de todos:
                                        if (dojo.byId('selecionaTodosItem').type == 'text')
                                            setTimeout(
                                                "configuraCheckBox(false, 'cd_item', 'selecionadoItem', -1, 'selecionaTodosItem', 'selecionaTodosItem', 'gridItem')",
                                                gridItem.rowsPerPage * 3);
                                    });
                            });

                        gridItem.on("RowDblClick", function (evt) {
                            try {
                                var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                                apresentaMensagem("apresentadorMensagemMovto", null);
                                if (TIPOMOVIMENTO == ENTRADA && validaEntradaMasterMaterialDidatico(item).valid == false) {
                                    var retornoValidaCompra = validaEntradaMasterMaterialDidatico(item);
                                    if (retornoValidaCompra.valid == false) {
                                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                                    }
                                    return ;
                                }
                                if (TIPOMOVIMENTO == DEVOLUCAO && dojo.byId('id_natureza_movto').value == ENTRADA && validaEntradaMasterMaterialDidatico(item).valid == false) {
                                    var retornoValidaCompra = validaEntradaMasterMaterialDidatico(item);
                                    if (retornoValidaCompra.valid == false) {
                                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                                    }
                                    return;
                                }

                                IncluirAlterar(0, 'divAlterarItem', 'divIncluirItem', 'divIncluirItem', 'apresentadorMensagemItem', 'divCancelarItem', 'divClearItem');
                                (function (callback) {
                                    //retornarItemKitFK(item, null);
                                    if (hasValue(item) && hasValue(item.cd_grupo_estoque)) {
                                        situacaoTributariaItem(item.cd_grupo_estoque);
                                    }
                                    keepValuesItem(item, gridItem, false);
                                    if (hasValue(item) && hasValue(item.cd_grupo_estoque)) {
                                        situacaoTributariaItem(item.cd_grupo_estoque);
                                    }
                                    dijit.byId("dialogItem").show();
                                    callback.call();
                                })(function () {
                                    calculaValorItem(item);
                                });


                                //keepVelusOutrosContatos();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);

                        gridItem.editItem = null;

                        //Baixa Financeira 
                        var gridBaixa = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure:
                            [
                                { name: "<input id='selecionaTodosBaixa' style='display:none'/>", field: "selecionadoBaixa", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBaixa },
                                { name: "Data", field: "dta_baixa", width: "10%", styles: "min-width:60px;" },
                                { name: "Valor", field: "VLLiquidacaoBaixa", width: "10%", styles: "text-align:right; min-width:60px;" },
                                { name: "Multa", field: "VLMultaBaixa", width: "10%", styles: "text-align:right; min-width:60px;" },
                                { name: "Juros", field: "VLJurosBaixa", width: "10%", styles: "text-align:right; min-width:60px;" },
                                { name: "Desconto", field: "VLDescontoBaixa", width: "10%", styles: "text-align:right; min-width:60px;" },
                                { name: "Tipo Baixa", field: "dc_tipo_liqui", width: "20%", styles: "text-align:center;min-width:50px;" },
                                { name: "Banco", field: "no_banco_baixa", width: "30%", styles: "min-width:70px;" }
                            ],
                            canSort: true,
                            noDataMessage: "Nenhum registro encontrado."
                        }, "gridBaixa");
                        dojo.byId('gridBaixa').style.width = '1000px';
                        gridBaixa.startup();

                        //MODAL TITULO
                        new dijit.form.Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { } }, "incluirTitulo");
                        new dijit.form.Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                var gridTitulo = dijit.byId('gridTitulo');
                                keepValuesTitulo(null, gridTitulo, null);
                            }
                        }, "cancelarTitulo");
                        new dijit.form.Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogTitulo").hide(); } }, "fecharTitulo");
                        new dijit.form.Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { alterarTitulo(); } }, "alterarTitulo");
                        //new dijit.form.Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { } }, "deleteTitulo");
                        new dijit.form.Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { } }, "limparTitulo");
                        new dijit.form.Button({
                            label: "",
                            iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            disabled: true,
                            onClick: function () { }
                        }, "cadPessoaResponsavelTit");

                        //Crud Movimento
                        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { salvarMovimento(); } }, "incluirMovto");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                try {
                                    //dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
                                    destroyCreateGridTitulos();
                                    keepValuesMovimento(null, gridMovimento, false);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cancelarMovto");
                        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadMovimento").hide(); } }, "fecharMovto");
                        new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { alterarMovimento(); } }, "alterarMovto");
                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarMovimentos(gridMovimento.itensSelecionados); });
                            }
                        }, "deleteMovto");
                        new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparMovimento(); } }, "limparMovto");
                        //Fim

                        //Crud Item
                        new Button({
                            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                incluirItemGrade();
                            }
                        }, "incluirItem");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                limparItem();
                                keepValuesItem(null, gridItem, null);
                            }
                        }, "cancelarItem");
                        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogItem").hide(); } }, "fecharItem");
                        new Button({
                            label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                altararItem(null);
                            }
                        }, "alterarItem");
                        new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { }); } }, "deleteItem");
                        new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparItem(); } }, "clearItem");

                        new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarMovimento(true); } }, "pesquisarMovto");
                        decreaseBtn(document.getElementById("pesquisarMovto"), '32px');
                        new Button({
                            label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                                try {
                                    var cdPessoaPesq = hasValue(dojo.byId("cdPessoaPesq").value) ? dojo.byId("cdPessoaPesq").value : 0;
                                    var cdItem = hasValue(dojo.byId("cdItem").value) ? dojo.byId("cdItem").value : 0;
                                    var cdPlanoContaPesq = hasValue(dojo.byId("cdPlanoContaPesq").value) ? dojo.byId("cdPlanoContaPesq").value : 0;
                                    var numeroPesq = hasValue(dojo.byId("numeroPesq").value) ? dojo.byId("numeroPesq").value : 0;
                                    var serie = hasValue(dojo.byId("numeroSeriePesq").value) ? dojo.byId("numeroSeriePesq").value : "";
                                    var venda_futura = (dijit.byId('ckVendaFuturaCad') != null && dijit.byId('ckVendaFuturaCad') != undefined) ? dijit.byId('ckVendaFuturaCad').checked : false;
                                    xhr.get({
                                        url: Endereco() + "/api/fiscal/GeturlrelatorioMovimento?" + getStrGridParameters('gridMovimento') + "&id_tipo_movimento=" + parseInt(TIPOMOVIMENTO) + "&cd_pessoa=" + parseInt(cdPessoaPesq) +
                                        "&cd_item=" + parseInt(cdItem) + "&cd_plano_conta=" + parseInt(cdPlanoContaPesq) + "&numero=" + parseInt(numeroPesq) + "&serie=" + serie +
                                        "&emissao=" + document.getElementById("ckEmissao").checked + "&movimento=" + document.getElementById("ckMovimento").checked +
                                            "&dtInicial=" + dojo.byId("dtaInicial").value + "&dtFinal=" + dojo.byId("dtaFinal").value + "&nota_fiscal=" + document.getElementById("ckNotaFiscalPesq").checked + "&statusNF=" + dijit.byId("statusNFPesq").value + "&id_venda_futura=" + venda_futura,
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                    }).then(function (data) {
                                        try {
                                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    },
                                    function (error) {
                                        apresentaMensagem('apresentadorMensagem', error);
                                    });
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "relatorioMovto");
                        new Button({
                            label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                                    function () {
                                        try {


                                            if (TIPOMOVIMENTO == SAIDA) {

                                                xhr.get({
                                                    url: Endereco() + "/api/fiscal/VerificaNFESemDataAutorizacao",
                                                    preventCache: true,
                                                    handleAs: "json",
                                                    headers: {
                                                        "Accept": "application/json",
                                                        "Content-Type": "application/json",
                                                        "Authorization": Token()
                                                    }
                                                }).then(function(data) {
                                                        try {
                                                            if (hasValue(data) && data.length > 0) {


                                                                caixaDialogo(DIALOGO_ERRO,
                                                                    'Existem notas sem retorno:' + data.toString(),
                                                                    null);

                                                            }

                                                        } catch (e) {
                                                            postGerarLog(e);
                                                        }
                                                    },
                                                    function(error) {
                                                        apresentaMensagem('apresentadorMensagem', error);
                                                    });
                                            }

                                            showCarregando();
                                            limparMovimento();
                                            dojo.addOnLoad(function() {
                                                try {
                                                    var compEmis = dijit.byId('dtaEmis');
                                                    var compMovto = dijit.byId('dtaMovto');
                                                    compEmis._onChangeActive = false;
                                                    compMovto._onChangeActive = false;
                                                    compEmis.attr("value", new Date(ano, mes, dia));
                                                    compMovto.attr("value", new Date(ano, mes, dia));
                                                    compEmis._onChangeActive = true;
                                                    compMovto._onChangeActive = true;
                                                    if (!dijit.byId("pcAcrec").disabled) {
                                                        dijit.byId("vlDesconto").set("disabled", false);
                                                        dijit.byId("pcDesconto").set("disabled", false);
                                                    }
                                                } catch (e) {
                                                    postGerarLog(e);
                                                }
                                            });

                                            destroyCreateGridTitulos();
                                            IncluirAlterar(1,
                                                'divAlterarMovto',
                                                'divIncluirMovto',
                                                'divExcluirMovto',
                                                'apresentadorMensagemMovto',
                                                'divCancelarMovto',
                                                'divClearMovto');
                                            findIsLoadComponetesNovoMovto(xhr, ready, Memory, FilteringSelect);
                                            dijit.byId("cadMovimento").show();

                                            if (TIPOMOVIMENTO == SAIDA &&
                                            (((parametrosTela['id_material_didatico'] != null &&
                                                    parametrosTela['id_material_didatico'] != undefined) &&
                                                eval(parametrosTela['id_material_didatico']) == 1))) {
                                                dijit.byId("tpContrato").set("disabled", false);
                                                dijit.byId("pesCursoFKMovimento").set("disabled", false);
                                                dijit.byId("limparCursoFKMovimento").set("disabled", false);
                                                dijit.byId("pesAlunoFKMovimento").set("disabled", false);
                                                criarOuCarregarCompFiltering("tpContrato", [], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_contrato', 'no_contrato');
                                                
                                            }
                                       
                                            
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                        }, "novoMovto");

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPolicaComercialFK"))) {
                                        montarPoliticaComercial(function () {
                                            abrirPoliticaComercialFK();
                                            dojo.query("#desPoliticaComercialFK").on("keyup", function (e) {
                                                try {
                                                    if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                                }
                                                catch (e) {
                                                    postGerarLog(e);
                                                }
                                            });
                                            dijit.byId("pesquisarPolicaComercialFK").on("click", function (e) {
                                                try {
                                                    apresentaMensagem("apresentadorMensagemProPessoa", null);
                                                    pesquisaPolicaComercialFK(true);
                                                }
                                                catch (e) {
                                                    postGerarLog(e);
                                                }
                                            });
                                            dijit.byId("fecharPolicaComercialFK").on("click", function (e) {
                                                dijit.byId("fkPoliticaComercial").hide();
                                            });
                                        });

                                        criarEventoSelecionaPolitica();
                                    }
                                    else
                                        abrirPoliticaComercialFK();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "proPoliticaComercial");

                        new Button({ label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () { } }, "cadBanco");
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    dojo.byId("tipoPesquisaFKItem").value = CADASTRO;
                                    SETAR_TIPO = true;
                                    if (!hasValue(dijit.byId("gridPesquisaItem")))
                                        montargridPesquisaItem(function () {
                                            try {
                                                //limparPesquisaCursoFK(false);
                                                dijit.byId("tipo").reset();
                                                dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                                chamarPesquisaItemFK(CADASTRO, xhr, Memory, FilteringSelect, array, ready);
                                                //abrirItemFKCadastro(xhr, ready, Memory, FilteringSelect,true);
                                                dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(CADASTRO, xhr, Memory, FilteringSelect, array, ready); });
                                                dijit.byId("pesquisarItemFK").on("click", function (e) {
                                                    apresentaMensagem("apresentadorMensagemItemFK", null);
                                                    var tipoPesquisaFKItem = dojo.byId("tipoPesquisaFKItem");
                                                    if (hasValue(tipoPesquisaFKItem.value))
                                                        chamarPesquisaItemFK(tipoPesquisaFKItem.value, xhr, Memory, FilteringSelect, array, ready);
                                                });
                                            }
                                            catch (e) {
                                                postGerarLog(e);
                                            }
                                        }, false, true);
                                    else {
                                        //limparPesquisaCursoFK(false);
                                        dijit.byId("tipo").reset();
                                        chamarPesquisaItemFK(CADASTRO, xhr, Memory, FilteringSelect, array, ready);
                                    }
                                    //if (!hasValue(dijit.byId("pesquisarItemFK")))
                                    //    montargridPesquisaItem(function () {
                                    //        abrirItemFK();
                                    //        dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) pesquisarItemFK(MATERIALDIDATICO); });
                                    //        dijit.byId("pesquisarItemFK").on("click", function (e) {
                                    //            apresentaMensagem("apresentadorMensagemItemFK", null);
                                    //            pesquisarItemFK(MATERIALDIDATICO);
                                    //        });
                                    //        dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("proItem").hide(); });
                                    //    }, true, true);
                                    //else
                                    //    abrirItemFK();

                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cadItem");

                        //#region pesquisa de pessoa e plano de contas no cadastro
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    origem = MOVIMENTO;
                                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                        montargridPesquisaPessoa(function () {
                                            dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                                if (e.keyCode == 13) pesquisaPessoaCadFK();
                                            });
                                            dijit.byId("pesqPessoa").on("click", function (e) {
                                                consultarPessoaMovimento();
                                            });
                                            dijit.byId("fecharFK").on("click", function (e) {
                                                dijit.byId("fkPessoaPesq").hide();
                                            });
                                            abrirPessoaFK(false);
                                        });
                                    else {
                                        CONSULTA_PESSOA = false;
                                        abrirPessoaFK(false);
                                    }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cadPessoa");

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    TIPO_PESQUISA = CADASTRO;
                                    if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                        montarFKPlanoContas(
                                            function () { funcaoFKPlanoContasCadastro(xhr, ObjectStore, Memory, false); },
                                            'apresentadorMensagem',
                                            MOVIMENTO, TIPO_PESQUISA);
                                    }
                                    else
                                        funcaoFKPlanoContasCadastro(xhr, ObjectStore, Memory, true, MOVIMENTO);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cadPlanoConta");

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridCFOPFK")))
                                        montarCFOPFK(function () {
                                            //selecionaCFOPFK
                                            dijit.byId("pesquisarCFOPFK").on("click", function (e) {
                                                try {
                                                    var id_natureza = 0;
                                                    var parametros = getParamterosURL();
                                                    if (hasValue(parametros['tipo'])) {
                                                        var tipo = eval(parametros['tipo']);
                                                        if (tipo == DEVOLUCAO)
                                                            tipo = dojo.byId('id_natureza_movto').value;
                                                        switch (tipo) {
                                                            case ENTRADA:
                                                                id_natureza = ENTRADACFOP;
                                                                break;
                                                            case SAIDA:
                                                                id_natureza = SAIDACFOP;
                                                                break;
                                                            case SERVICO:
                                                                id_natureza = SERVICOCFOP;
                                                                break;
                                                        }
                                                    }
                                                    searchCFOP();
                                                }
                                                catch (e) {
                                                    postGerarLog(e);
                                                }
                                            });
                                            dijit.byId("selecionaCFOPFK").on("click", function (e) {
                                                retornarCFOPFK();
                                            });
                                            abrirCFOPFK(false);
                                        });
                                    else
                                        abrirCFOPFK(false);

                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cadCFOPItem");

                        //#endregion
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                        montargridPesquisaPessoa(function () {
                                            try {
                                                abrirPessoaFK(true);
                                                dojo.query("#_nomePessoaFK").on("keyup", function (e) { if (e.keyCode == 13) pesquisaPessoaFKMovimento(true); });
                                                dijit.byId("pesqPessoa").on("click", function (e) {
                                                    consultarPessoaMovimento();
                                                });
                                                dijit.byId("fecharFK").on("click", function (e) {
                                                    dijit.byId("fkPessoaPesq").hide();
                                                });
                                            }
                                            catch (e) {
                                                postGerarLog(e);
                                            }
                                        });
                                    else {
                                        CONSULTA_PESSOA = false;
                                        abrirPessoaFK(true);
                                    }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesPessoaPesq");

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true,
                            onClick: function () {
                                try {
                                    dojo.byId('noPessoaPesq').value = '';
                                    dojo.byId('cdPessoaPesq').value = 0;
                                    dijit.byId("limparPessoaRelPosPes").set('disabled', true);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparPessoaRelPosPes");
                        decreaseBtn(document.getElementById("limparPessoaRelPosPes"), '40px');

                        new dijit.form.Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                try {
                                    dojo.byId("noPessoaMovto").value = "";
                                    dojo.byId("cdPessoaMvtoCad").value = 0;
                                    dijit.byId("limparPessoaFKMovimento").set('disabled', true);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparPessoaFKMovimento");
                        decreaseBtn(document.getElementById("limparPessoaFKMovimento"), '40px');
                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                dijit.byId("limparPlanoConta").set('disabled', true);
                            }
                        }, "limparPlanoConta");
                        decreaseBtn(document.getElementById("limparPlanoConta"), '40px');

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    dojo.byId("tipoPesquisaFKItem").value = PESQUISA;
                                    if (!hasValue(dijit.byId("gridPesquisaItem")))
                                        montargridPesquisaItem(function () {
                                            try {
                                                dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                                limparPesquisaCursoFK(false);
                                                dijit.byId("tipo").reset();
                                                chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready);
                                                //abrirItemFK(xhr, Memory, FilteringSelect, array)
                                                dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready); });
                                                dijit.byId("pesquisarItemFK").on("click", function (e) {
                                                    try {
                                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                                        var tipoPesquisaFKItem = dojo.byId("tipoPesquisaFKItem");
                                                        if (hasValue(tipoPesquisaFKItem.value))
                                                            chamarPesquisaItemFK(tipoPesquisaFKItem.value, xhr, Memory, FilteringSelect, array, ready);
                                                    }
                                                    catch (e) {
                                                        postGerarLog(e);
                                                    }
                                                });
                                            }
                                            catch (e) {
                                                postGerarLog(e);
                                            }
                                        }, false, true);
                                    else {
                                        limparPesquisaCursoFK(false);
                                        dijit.byId("tipo").reset();
                                        chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready);
                                    }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesItemPesq");

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                dojo.byId("cdItem").value = '';
                                dojo.byId('noItemPesq').value = '';
                                dijit.byId("limparItem").set('disabled', true);
                            }
                        }, "limparItem");
                        decreaseBtn(document.getElementById("limparItem"), '40px');

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                dijit.byId("limparPlanoRelPosPes").set('disabled', true);
                            }
                        }, "limparPlanoRelPosPes");

                        decreaseBtn(document.getElementById("limparPlanoRelPosPes"), '40px');
                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                dijit.byId("limpaPolCom").set('disabled', true);
                            }
                        }, "limpaPolCom");
                        decreaseBtn(document.getElementById("limpaPolCom"), '40px');

                        //Botao Incluir Item
                        var button = new Button({
                            label: "Incluir", name: "itensT", id: "itensT", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                                try {
                                    limparItem();
                                    IncluirAlterar(1, 'divAlterarItem', 'divIncluirItem', 'divExcluirItem', 'apresentadorMensagemItem', 'divCancelarItem', 'divClearItem');

                                    dijit.byId('cadItem').set('disabled', false);
                                    if (dijit.byId("ckNotaFiscal").checked) {
                                        switch (TIPOMOVIMENTO) {
                                            case ENTRADA:
                                            case SAIDA:
                                            case DEVOLUCAO:
                                                if (!dijit.byId("formCadMovimento").validate()) {
                                                    setarTabCadMovimento();
                                                    return false;
                                                }

                                                if (((parametrosTela['id_material_didatico'] != null &&
                                                        parametrosTela['id_material_didatico'] != undefined) &&
                                                    eval(parametrosTela['id_material_didatico']) == 1)) {

                                                    if (!hasValue(dojo.byId("noAlunoFKMovimento").value)) {
                                                        caixaDialogo(DIALOGO_AVISO, msgAlunoNotFoundMovimentoSaidaMaterial, null);
                                                        setarTabCadMovimento();
                                                        return false;
                                                    }else if (!hasValue(dojo.byId("noCursoFKMovimento").value)) {
                                                        caixaDialogo(DIALOGO_AVISO, msgCursoNotFoundMovimentoSaidaMaterial, null);
                                                        setarTabCadMovimento();
                                                        return false;
                                                    } else if (!hasValue(dijit.byId("tpContrato").value)) {
                                                        caixaDialogo(DIALOGO_AVISO, msgContratoNotFoundMovimentoSaidaMaterial, null);
                                                        setarTabCadMovimento();
                                                        return false;
                                                    }
                                                }
                                                dadosFiscaisNFProduto(function () {
                                                    try {
                                                        var gridItem = dijit.byId("gridItem");
                                                        if (hasValue(gridItem.editItem) && !alterou_tp_nf) {
                                                            loadSituacao(gridItem.editItem.situacoesTributariaICMS, 0);
                                                            //criarOuCarregarCompFiltering("sitTribItem", gridItem.editItem.situacoesTributariaICMS, "", null, dojo.ready, dojo.store.Memory,
                                                            //                              dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                                            if (hasValue(gridItem.editItem.situacoesTributariaPIS))
                                                                if (regime_tributario == REGIME_NORMAL)
                                                                    criarOuCarregarCompFiltering("cbStTribPis", gridItem.editItem.situacoesTributariaPIS, "", SITUACAOTRIBUTARIAPIS, dojo.ready, dojo.store.Memory,
                                                                                                  dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                                                else
                                                                    criarOuCarregarCompFiltering("cbStTribPis", gridItem.editItem.situacoesTributariaPIS, "", SITUACAOTRIBUTARIAPIS_OUTRASOP, dojo.ready, dojo.store.Memory,
                                                                                              dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                                            if (hasValue(gridItem.editItem.situacoesTributariaCOFINS))
                                                                if (regime_tributario == REGIME_NORMAL)
                                                                    criarOuCarregarCompFiltering("cbStTribCof", gridItem.editItem.situacoesTributariaCOFINS, "", SITUACAOTRIBUTARIACOFINS, dojo.ready, dojo.store.Memory,
                                                                                                 dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                                                else
                                                                    criarOuCarregarCompFiltering("cbStTribCof", gridItem.editItem.situacoesTributariaCOFINS, "", SITUACAOTRIBUTARIACOFINS_OUTRASOP, dojo.ready, dojo.store.Memory,
                                                                                             dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                                        }
                                                        if (hasValue(dojo.byId("cd_cfop_nf").value)) {
                                                            dojo.byId("cd_CFOP_item").value = dojo.byId("cd_cfop_nf").value;
                                                            dijit.byId("descCFOPItem").set("value", dijit.byId("CFOP").value);
                                                            dojo.byId("descCFOPItem").value = dijit.byId("CFOP").value;
                                                        }
                                                        dijit.byId("operacaoCFOPItem").set("value", dijit.byId("operacaoCFOP").value);

                                                        if (TIPOMOVIMENTO == SAIDA && regime_tributario != REGIME_NORMAL)
                                                            dijit.byId("pc_aliquota_ap_item").set("value", pcAliqAproxSaida);

                                                        //dijit.byId("dialogItem").show();
                                                        abrirDialogIncluirItem(xhr, Memory, FilteringSelect, array, ready);

                                                    }
                                                    catch (e) {
                                                        postGerarLog(e);
                                                    }
                                                });
                                                break;
                                            case SERVICO:
                                                if (!dijit.byId("formCadMovimento").validate()) {
                                                    setarTabCadMovimento();
                                                    return false;
                                                }
                                                dadosFiscaisNFServico(function () {
                                                    if (regime_tributario != REGIME_NORMAL)
                                                        dijit.byId("pc_aliquota_ap_item").set("value", pcAliqAproxServico);
                                                    dijit.byId("dialogItem").show();
                                                });
                                                break;
                                        }
                                    } else
                                        //dijit.byId("dialogItem").show();
                                        abrirDialogIncluirItem(xhr, Memory, FilteringSelect, array, ready);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, 'btnAddItem');

                        //Botao Escolher Grade
                        var menuT = new DropDownMenu({ style: "height: 25px" });
                        var acaoMenu = new MenuItem({
                            label: "Kits da Nota",
                            onClick: function () {
                                dojo.byId('divKit').style.display = "block";
                                dojo.byId('divItem').style.display = "none";

                                dojo.style(dojo.byId('linkEditarKit'), "display", "block");
                                dojo.style(dojo.byId('linkItem'), "display", "none");
                                gridKit.update();
                            }
                        });
                        menuT.addChild(acaoMenu);
                        var acaoMenu = new MenuItem({
                            label: "Itens da Nota",
                            onClick: function () {
                                dojo.byId('divKit').style.display = "none";
                                dojo.byId('divItem').style.display = "block";

                                dojo.style(dojo.byId('linkEditarKit'), "display", "none");
                                dojo.style(dojo.byId('linkItem'), "display", "block");
                                gridItem.update();
                            }
                        });
                        menuT.addChild(acaoMenu);
                        var button = new DropDownButton({
                            label: "Escolher grade",
                            name: "btnKit",
                            dropDown: menuT,
                            id: "btKit"                            
                        });
                        dom.byId("linkKit").appendChild(button.domNode);

                        //Grid Kit
                        var gridKit = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure:
                                [
                                    { name: "<input id='selecionaTodosKit' style='display:none'/>", field: "selecionadoKit", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxKit },
                                    { name: "Kit", field: "no_item_kit", width: "85%" },

                                    {
                                        name: "Qtde",
                                        field: "qt_item_kit",
                                        width: "10%",
                                        styles: "min-width:80px;text-align:center;",
                                        formatter: formatTextQtdKit
                                    },

                                    //{ name: "Qtde.", field: "qt_item_kit", width: "7%", styles: "text-align:center; min-width:15px;" },
                                    { name: "<input id='cd_movimento'/>", field: "cd_movimento" },
                                    { name: "<input id='cd_item_movimento_kit'/>", field: "cd_item_movimento_kit" }
                                ],
                            canSort: true,
                            contentEditable: function (col) { return Math.abs(col) > 2; },
                            noDataMessage: "Nenhum registro encontrado.",
                        }, "gridKit");

                        //HIDE colunas cd_movimento/cd_item_movimento_kit grid Kit.
                        gridKit.layout.setColumnVisibility(3, 0);
                        gridKit.layout.setColumnVisibility(4, 0);
                        gridKit.canSort = function (col) { return Math.abs(col) != 1; };
                        gridKit.startup();

                        require(["dojo/aspect"],
                            function (aspect) {
                                aspect.after(gridKit,
                                    "_onFetchComplete",
                                    function () {
                                        // Configura o check de todos:
                                        if (dojo.byId('selecionaTodosKit').type == 'text')
                                            setTimeout(
                                                "configuraCheckBox(false, 'cd_item', 'selecionadoKit', -1, 'selecionaTodosKit', 'selecionaTodosKit', 'gridKit')",
                                                gridKit.rowsPerPage * 3);
                                    });
                            });

                        if (TIPOMOVIMENTO != SAIDA) {
                            dom.byId("linkKit").style.display = "none";
                        }

                        new Button({
                            label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                                dojo.byId("cd_nfDev").value = 0;
                                dojo.byId("id_tipo_movimento").value = 0;
                                dijit.byId("tpNfDev").set("value", "");
                                dijit.byId('limparNFDev').set("disabled", true);
                                dijit.byId("cbMovtoFK").reset();
                                dijit.byId("cbMovtoFK").set("disabled", false);
                                dojo.byId("id_tipo_movimento").value = 0;
                                if (hasValue(gridItem)) {
                                    gridItem.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                                    gridItem.itensSelecionados = [];
                                }
                                dojo.byId("cdPessoaMvtoCad").value = 0;
                                dijit.byId("noPessoaMovto").set("value", "");
                            }
                        }, "limparNFDev");
                        if (hasValue(document.getElementById("limparNFDev"))) {
                            document.getElementById("limparNFDev").parentNode.style.minWidth = '40px';
                            document.getElementById("limparNFDev").parentNode.style.width = '40px';
                        }
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("selecionaMovimentoFK"))) {
                                        montarGridPesquisaMovtoFK(function () {
                                            limparFiltrosTurmaFK();
                                            pesquisarMovimentoFK(true, true);
                                            dijit.byId("proMvtoFK").show();
                                        });
                                    }
                                    else {
                                        limparFiltrosTurmaFK();
                                        pesquisarMovimentoFK(true, true);
                                        dijit.byId("proMvtoFK").show();
                                    }
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesNFDev");
                        var buttonFkArray = ['proPoliticaComercial', 'cadBanco', 'cadItem', 'cadPessoa', 'cadPlanoConta', 'cadPessoaResponsavelTit', 'pesPessoaPesq', 'pesItemPesq',
                           'pesPlanoPesq', 'cadTpNF', 'cadCFOPItem', 'pesNFDev'];

                        new dijit.form.Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false, function () {
                                            abrirAlunoFK();
                                        });
                                    }
                                    else
                                        abrirAlunoFK();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesAlunoFKMovimento");
                        new dijit.form.Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                try {
                                    dojo.byId("cdAlunoFKMovimento").value = 0;
                                    dojo.byId("cdPessoaAlunoFKMovimento").value = 0;
                                    dojo.byId("noAlunoFKMovimento").value = "";
                                    dijit.byId("limparAlunoFKMovimento").set('disabled', true);
                                    gerar_titulo = true;


                                    var parametrosTela = getParamterosURL();
                                    if ((parametrosTela['id_material_didatico'] != null) &&
                                        (parametrosTela['id_material_didatico'] != undefined) &&
                                        eval(parametrosTela['id_material_didatico']) == 1) {

                                        dojo.byId("cdCursoFKMovimento").value = 0;
                                        dojo.byId("noCursoFKMovimento").value = "";
                                        dijit.byId("limparCursoFKMovimento").set('disabled', true);
                                        dijit.byId("tpContrato").reset();
                                        criarOuCarregarCompFiltering("tpContrato", [], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_contrato', 'no_contrato');

                                    }

                                    
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparAlunoFKMovimento");
                        decreaseBtn(document.getElementById("limparAlunoFKMovimento"), '40px');
                        decreaseBtn(document.getElementById("pesAlunoFKMovimento"), '18px');

                        new dijit.form.Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (dijit.byId("tpContrato").value == null ||
                                        dijit.byId("tpContrato").value == undefined ||
                                        dijit.byId("tpContrato").value == 0 || !hasValue(dojo.byId("noAlunoFKMovimento").value)) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAlunoOrCursoNotFound);
                                        apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                                    } else
                                    {
                                        apresentaMensagem('apresentadorMensagemMovto', null);
                                        if (!hasValue(dijit.byId("gridPesquisaCurso"))) {
                                            montargridPesquisaCurso(true, function () {
                                                dijit.byId("pesquisarCursoFK").on("click", function (e) {
                                                    apresentaMensagem("apresentadorMensagemFks", null);
                                                    pesquisarCursoByContratoMovimento();
                                                });
                                                abrirCursoFK();
                                            });
                                        }
                                        else
                                            abrirCursoFK();
                                    }
                                    
                                    
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesCursoFKMovimento");
                        new dijit.form.Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                                try {
                                    dojo.byId("cdCursoFKMovimento").value = 0;
                                    dojo.byId("noCursoFKMovimento").value = "";
                                    dijit.byId("limparCursoFKMovimento").set('disabled', true);
                                    gerar_titulo = true;
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparCursoFKMovimento");
                        decreaseBtn(document.getElementById("limparCursoFKMovimento"), '40px');
                        decreaseBtn(document.getElementById("pesCursoFKMovimento"), '18px');

                        // Adiciona link de ações Movimento:
                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () {
                                eventoEditarMovimento(gridMovimento.itensSelecionados, (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false));
                            }
                        });
                        menu.addChild(acaoEditar);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () { eventoRemoverMovimentos(gridMovimento.itensSelecionados); }
                        });
                        menu.addChild(acaoRemover);
                        menu.addChild(new MenuSeparator());

                        var acaoRelatorio = new MenuItem({
                            label: getNomeLabelRelatorio(),
                            onClick: function () {
                                try {
                                    if (!hasValue(gridMovimento.itensSelecionados) || gridMovimento.itensSelecionados.length <= 0)
                                        caixaDialogo(DIALOGO_AVISO, 'Selecione alguma ' + document.getElementById("titulo").innerHTML.substring(0, document.getElementById("titulo").innerHTML.length - 1).toLowerCase()
                                                                    + ' para emitir o relatório .', null);
                                    else if (gridMovimento.itensSelecionados.length > 1)
                                        caixaDialogo(DIALOGO_ERRO, 'Selecione somente uma ' + document.getElementById("titulo").innerHTML.substring(0, document.getElementById("titulo").innerHTML.length - 1).toLowerCase()
                                                                    + ' para emitir o relatório.', null);
                                    else {
                                        apresentaMensagem('apresentadorMensagem', null);
                                        var cd_movimento = gridMovimento.itensSelecionados[0].cd_movimento;
                                        xhr.get({
                                            url: Endereco() + "/api/fiscal/getUrlRelatorioEspelho?cd_movimento=" + cd_movimento,
                                            preventCache: true,
                                            handleAs: "json",
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                        }).then(function (data) {
                                            abrePopUp(Endereco() + '/Relatorio/RelatorioEspelhoMovimento?' + data, '765px', '771px', 'popRelatorio');
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
                        menu.addChild(acaoRelatorio);

                        var acaoImprimir = new MenuItem({
                            label: "Imprimir Cópia",
                            onClick: function () {
                                try {
                                    if (!hasValue(gridMovimento.itensSelecionados) || gridMovimento.itensSelecionados.length <= 0)
                                        caixaDialogo(DIALOGO_AVISO, 'Selecione alguma ' + document.getElementById("titulo").innerHTML.substring(0, document.getElementById("titulo").innerHTML.length - 1).toLowerCase()
                                                                    + ' para emitir o relatório .', null);
                                    else if (gridMovimento.itensSelecionados.length > 1)
                                        caixaDialogo(DIALOGO_ERRO, 'Selecione somente uma ' + document.getElementById("titulo").innerHTML.substring(0, document.getElementById("titulo").innerHTML.length - 1).toLowerCase()
                                                                    + ' para emitir o relatório.', null);
                                    else {
                                        apresentaMensagem('apresentadorMensagem', null);
                                        var cd_movimento = gridMovimento.itensSelecionados[0].cd_movimento;
                                        xhr.get({
                                            url: Endereco() + "/api/fiscal/getUrlRelatorioEspelho?cd_movimento=" + cd_movimento,
                                            preventCache: true,
                                            handleAs: "json",
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                        }).then(function (data) {
                                            abrePopUp(Endereco() + '/Relatorio/RelatorioCopiaEspelhoMovimento?' + data, '765px', '771px', 'popRelatorio');
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
                        menu.addChild(acaoImprimir);

                        menu.addChild(new MenuSeparator());
                        var parametros = getParamterosURL();
                        if (hasValue(parametros['tipo'])) {
                            if (eval(parametros['tipo']) == SERVICO || eval(parametros['tipo']) == ENTRADA || eval(parametros['tipo']) == SAIDA || eval(parametros['tipo']) == DEVOLUCAO) {
                                var acaoProcessar = new MenuItem({
                                    label: "Processar",
                                    onClick: function () { eventoProcessarMovimento(gridMovimento.itensSelecionados); }
                                });
                                menu.addChild(acaoProcessar);

                                var acaoCancelar = new MenuItem({
                                    label: "Cancelar",
                                    onClick: function () { eventoCancelarMovimentos(dijit.byId('gridMovimento').itensSelecionados); }
                                });
                                menu.addChild(acaoCancelar);
                            }
                            if (!empresa_propria) {
                                if (eval(parametros['tipo']) == SERVICO || eval(parametros['tipo']) == SAIDA || eval(parametros['tipo']) == DEVOLUCAO) {
                                    var acaoGerarXML = new MenuItem({
                                        label: "Gerar XML",
                                        onClick: function () { eventoGerarXML(dijit.byId('gridMovimento').itensSelecionados); }
                                    });
                                    menu.addChild(acaoGerarXML);
                                }
                            }
                            if (empresa_propria && eval(parametros['tipo']) != ENTRADA) {
                                var acaoReenviarMasterSaf = new MenuItem({
                                    label: "Reenviar MasterSaf",
                                    onClick: function () { eventoReenviarMasterSafMovimento(dijit.byId('gridMovimento').itensSelecionados); }
                                });
                                menu.addChild(acaoReenviarMasterSaf);
                            }
                        }

                        if (eval(parametros['tipo']) == SERVICO || eval(parametros['tipo']) == SAIDA) {
                            var acaoCarne = new MenuItem({
                                label: "Carnê",
                                onClick: function () {
                                    try {
                                        var gridMovimento = dijit.byId('gridMovimento');
                                        var itensSelecionados = gridMovimento.itensSelecionados;
                                        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                                            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdateCarne, null);
                                        else if (itensSelecionados.length > 1)
                                            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdateCarne, null);
                                        else
                                            emitirRelatorioCarneMovto(itensSelecionados[0].cd_movimento, permissoes, xhr);
                                    } catch (e) {
                                        postGerarLog(e);
                                    }
                                }
                            });
                            menu.addChild(acaoCarne);
                        }

                        if (eval(parametros['tipo']) == SERVICO || eval(parametros['tipo']) == SAIDA) {
                            menu.addChild(new MenuSeparator());
                            var acaoReciboConfirmacaoMovimento = new MenuItem({
                                label: "Recibo Confirmacao",
                                onClick: function () {
                                    var gridMovimento = dijit.byId('gridMovimento');
                                    if (!hasValue(gridMovimento.itensSelecionados) || gridMovimento.itensSelecionados.length <= 0)
                                        caixaDialogo(DIALOGO_AVISO, 'Selecione algum movimento para emitir o recibo de confirmação.', null);
                                    else if (gridMovimento.itensSelecionados.length > 1)
                                        caixaDialogo(DIALOGO_ERRO, 'Selecione somente um movimento para emitir o recibo de confirmação.', null);
                                    else {
                                        apresentaMensagem('apresentadorMensagem', null);
                                        var cd_movimento = gridMovimento.itensSelecionados[0].cd_movimento;
                                        xhr.get({
                                            url: Endereco() + "/api/coordenacao/getUrlRelatorioReciboConfirmacaoMovimento?cd_movimento=" + cd_movimento,
                                            preventCache: true,
                                            handleAs: "json",
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                        }).then(function (data) {
                                                abrePopUp(Endereco() + '/Relatorio/RelatorioReciboConfirmacao?' + data, '765px', '771px', 'popRelatorio');
                                            },
                                            function (error) {
                                                apresentaMensagem('apresentadorMensagem', error);
                                            });
                                    }
                                }
                            });
                            menu.addChild(acaoReciboConfirmacaoMovimento);
                        }
                        


                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadas",
                            dropDown: menu,
                            id: "acoesRelacionadas"
                        });
                        dom.byId("linkAcoes").appendChild(button.domNode);

                        // Adiciona link de ações Item
                        var menuT = new DropDownMenu({ style: "height: 25px" });

                        var acaoEditarT = new MenuItem({
                            label: "Editar",
                            onClick: function () { eventoEditarItem(gridItem.itensSelecionados); }
                        });
                        menuT.addChild(acaoEditarT);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarItemServicoSelecionadoGrid(Memory, ObjectStore, 'id', gridItem);
                            }
                        });
                        menuT.addChild(acaoRemover);

                        var buttonR = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasItem",
                            dropDown: menuT,
                            id: "acoesRelacionadasItem"
                        });
                        dom.byId("linkItem").appendChild(buttonR.domNode);

                        //Ações relaciondas Grid Kit
                        var menuT = new DropDownMenu({ style: "height: 25px" });
                        var acaoRemoverKit = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarKit(Memory, ObjectStore, gridKit);
                            }
                        });
                        menuT.addChild(acaoRemoverKit);

                        var buttonR = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasKit",
                            dropDown: menuT,
                            id: "acoesRelacionadasKit"
                        });
                        dom.byId("linkEditarKit").appendChild(buttonR.domNode);
                        dojo.style(dojo.byId('linkEditarKit'), "display", "none");

                        //------------------

                        //#region Pesquisa plano contas
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    TIPO_PESQUISA = PESQUISA;
                                    if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                        montarFKPlanoContas(
                                            function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                            'apresentadorMensagem',
                                            MOVIMENTO, TIPO_PESQUISA);
                                    }
                                    else
                                        funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, MOVIMENTO);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesPlanoPesq");

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true,
                            onClick: function () {
                                dojo.byId('noPlanoContaPesq').value = "";
                                dojo.byId('cdPlanoContaPesq').value = 0;
                                dijit.byId("limparPlanoContaPesq").set('disabled', true);
                            }
                        }, "limparPlanoContaPesq");

                        if (hasValue(document.getElementById("limparPlanoContaPesq"))) {
                            document.getElementById("limparPlanoContaPesq").parentNode.style.minWidth = '40px';
                            document.getElementById("limparPlanoContaPesq").parentNode.style.width = '40px';
                        }
                        //#endregion

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });

                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () { buscarTodosItens(gridMovimento, 'todosItens', ['pesquisarMovto', 'relatorioMovto']); pesquisarMovimento(false); }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridMovimento', 'selecionadoMovto', 'cd_movimento', 'selecionaTodos', ['pesquisarMovto', 'relatorioMovto'], 'todosItens'); }
                        });
                        menu.addChild(menuItensSelecionados);

                        var button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItens",
                            dropDown: menu,
                            id: "todosItens"
                        });
                        dom.byId("linkSelecionados").appendChild(button.domNode);

                        //Metodo para a criação do dropDown de link
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoMovto', -1, 'selecionaTodos', 'selecionaTodos', 'gridMovimento')", gridMovimento.rowsPerPage * 3);

                        //Ações Relacionadas Titulos
                        var menuT = new dijit.DropDownMenu({ style: "height: 25px" });

                        var acaoEditar = new dijit.MenuItem({
                            label: "Editar",
                            onClick: function () { eventoEditarTitulo(dijit.byId("gridTitulo").itensSelecionados); }
                        });
                        menuT.addChild(acaoEditar);

                        var acaoRemover = new dijit.MenuItem({
                            label: "Excluir",
                            onClick: function () { deletarItemSelecionadoTitulo(Memory, ObjectStore); }
                        });
                        menuT.addChild(acaoRemover);

                        var acaoBanco = new dijit.MenuItem({
                            label: "Local de Movimento",
                            onClick: function () { alterarLocalMovtoTitulos(); }
                        });
                        menuT.addChild(acaoBanco);

                        var acaoBaixar = new MenuItem({
                            label: "Baixar Titulo(s)",
                            onClick: function () {
                                try {
                                    var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                                    var gridTitulo = dijit.byId('gridTitulo');
                                    if (hasValue(gridTitulo.itensSelecionados))
                                        gridTitulo.itemSelecionado = gridTitulo.itensSelecionados[0];
                                    //mostrarCadastroBaixaFinanceira(true, gridTitulo, null, xhr, ref, on);
                                    if (!hasValue(dijit.byId('incluirBaixa')))
                                        montaCadastroBaixaFinanceira(function () {
                                            setarEventosBotoesPrincipaisCadTransacao(dojo.xhr, dojo.on, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, dojo.data.ObjectStore);
                                            abrirBaixa();
                                        }, Permissoes);
                                    else
                                        abrirBaixa();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        });

                        menuT.addChild(acaoBaixar);

                        var buttonT = new dijit.form.DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasTit",
                            dropDown: menuT,
                            id: "acoesRelacionadasTit"
                        });
                        dojo.byId("linkTitulo").appendChild(buttonT.domNode);

                        //Ações Relacionadas Baixa
                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () {
                                eventoEditarBaixaTitulo(gridBaixa.itensSelecionados, xhr, ready, Memory, FilteringSelect, on);
                                //mostrarCadastroBaixaFinanceira(false, null, dijit.byId('gridBaixa'), xhr, ref);
                            }
                        });
                        menu.addChild(acaoEditar);

                        var acaoHist = new MenuItem({
                            label: "Histórico",
                            onClick: function () {
                                eventoHistoricoBaixaTitulo(gridBaixa.itensSelecionados, xhr);
                            }
                        });
                        menu.addChild(acaoHist);

                        var acaoRecibo = new MenuItem({
                            label: "Recibo",
                            onClick: function () {
                                if (!hasValue(gridBaixa.itensSelecionados) || gridBaixa.itensSelecionados.length <= 0)
                                    caixaDialogo(DIALOGO_AVISO, 'Selecione alguma baixa para emitir o recibo.', null);
                                else if (gridBaixa.itensSelecionados.length > 1)
                                    caixaDialogo(DIALOGO_ERRO, 'Selecione somente uma baixa para emitir o recibo.', null);
                                else {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    var cdBaixaTitulo = gridBaixa.itensSelecionados[0].cd_baixa_titulo;
                                    var cd_movimento = hasValue(dojo.byId("cd_movimento").value) ? dojo.byId("cd_movimento").value : 0;
                                    xhr.get({
                                        url: Endereco() + "/api/financeiro/getUrlRelatorioReciboMovimento?cd_baixa_titulo=" + cdBaixaTitulo + "&cd_movimento=" + cd_movimento,
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                    }).then(function (data) {
                                        abrePopUp(Endereco() + '/Relatorio/RelatorioReciboMovimento?' + data, '765px', '771px', 'popRelatorio');
                                    },
                                    function (error) {
                                        apresentaMensagem('apresentadorMensagem', error);
                                    });
                                }
                            }
                        });
                        menu.addChild(acaoRecibo);

                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadas",
                            dropDown: menu,
                            id: "acoesRelacionadasBaixa"
                        });
                        dom.byId("linkAcoesBaixa").appendChild(button.domNode);
                        //Fim cadBaixa

                        new Button({
                            label: "",
                            iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                            onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridTipoNFFK"))) {
                                        montarTipoNFFK(function () {
                                            loadTpMovtoFK(TIPOMOVIMENTO);
                                            dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                                if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                            });
                                            dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                                apresentaMensagem("apresentadorMensagemTipoNFF", null);
                                                if (TIPOMOVIMENTO == SAIDA) {
                                                    pesquisaTipoNFFK(true, false);
                                                } else {
                                                    pesquisaTipoNFFK(true, TIPOMOVIMENTO == SERVICO ? true : false);
                                                }
                                                
                                            });
                                            dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                                dijit.byId("fkTipoNF").hide();
                                            });
                                            abrirTipoNFFK();
                                        });
                                        criarEventoSelecionaTipoNF();
                                    }
                                    else
                                        abrirTipoNFFK();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cadTpNF");
                        decreaseBtn(document.getElementById("cadPessoaResponsavelTit"), '18px');
                        //Lista de botões a serem diminuidos.

                        diminuirBotoes(buttonFkArray);
                        dijit.byId("tpFinanceiro").on("change", function (e) {
                            try {
                                console.log("tpFinanceiro");
                                gerar_titulo = true;
                                if (e != TIPOFINANCEIROCHEQUE)
                                    habilitarTagCheque(false)
                                else
                                    habilitarTagCheque(true);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("nfEsc").on("change", function (e) {
                            try {
                                if (empresa_propria && (dijit.byId("ckNotaFiscal").checked)) {
                                    dijit.byId("dc_key_nfe").set("disabled", false);
                                    dijit.byId('dc_key_nfe').set('required', true);
                                }
                                else {
                                    dijit.byId("dc_key_nfe").set("disabled", true);
                                    dijit.byId('dc_key_nfe').set('required', false);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("cadMovimento").on("show", function (e) {
                            dijit.byId("gridItem").update();
                        });
                        //chamadas de eventos do cadastro de item.
                        dijit.byId("qtd_item").on("change", function (qtd) {
                            try {
                                calcularItem(qtd);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("nroPrimeiroCheque").on("change", function (e) {
                            gerar_titulo = true;
                        });

                        
                        dijit.byId("tpContrato").on("change", function(e) {
                           
                                dojo.byId("cdCursoFKMovimento").value = 0;
                                dojo.byId("noCursoFKMovimento").value = "";
                            dijit.byId("limparCursoFKMovimento").set('disabled', true);
                            dojo.byId("cd_origem_movimento").value = hasValue(e)? e : 0;

                        })
                        dijit.byId("vlUnitario").on("change", function (vlUnit) {
                            try {
                                var vlUnitario = dijit.byId("vlUnitario");
                                var compVlLiquido = dijit.byId("vlLiquido");
                                var compvlTotal = dijit.byId("vlTotalMovimento");
                                var item = null;
                                var totalItens = null;
                                if (isNaN(vlUnit)) {
                                    // vlUnitario._onChangeActive = true;
                                    vlUnitario.set("value", 0);
                                    vlUnitario.value = 0;
                                    vlUnitario.oldValue = 1;
                                } else {
                                    if (unmaskFixed(vlUnitario.oldValue, 2) != unmaskFixed(vlUnit, 2)) {
                                        if (vlUnit > 0) {
                                            totalItens = unmaskFixed(dijit.byId("qtd_item").value * vlUnit, 2);
                                            if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                                                dijit.byId('valDescontoItem').set("value", (totalItens * dijit.byId('perDescontoItem').value) / 100);
                                                dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                            }
                                            else
                                                if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                                                    dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / totalItens);
                                                    dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                                }
                                                else
                                                    item = calcularDescAcrescItemView(dijit.byId("qtd_item").value, totalItens, null, vlUnit);
                                        } else
                                            if (compvlTotal.value > 0) {
                                                vlUnit = compvlTotal.value / dijit.byId("qtd_item").value;
                                                totalItens = compvlTotal.value;
                                                if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                                                    dijit.byId('valDescontoItem').set("value", (totalItens * dijit.byId('perDescontoItem').value) / 100);
                                                    dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                                }
                                                else
                                                    if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                                                        dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / totalItens);
                                                        dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                                    }
                                                    else
                                                        item = calcularDescAcrescItemView(dijit.byId("qtd_item").value, totalItens, null, vlUnit);
                                            }
                                        vlUnitario._onChangeActive = false;
                                        vlUnitario.set("value", unmaskFixed(vlUnit, 2));
                                        vlUnitario.value = vlUnit;
                                        vlUnitario.oldValue = unmaskFixed(vlUnit, 2);
                                        vlUnitario._onChangeActive = true;

                                        compvlTotal._onChangeActive = false;
                                        compvlTotal.set("value", unmaskFixed(totalItens, 2));
                                        compvlTotal.value = totalItens;
                                        compvlTotal.oldValue = unmaskFixed(totalItens, 2);
                                        compvlTotal._onChangeActive = true;
                                        compVlLiquido._onChangeActive = false;
                                        if (hasValue(item)) {
                                            compVlLiquido.set("value", item.vl_liquido_item);
                                            compVlLiquido.value = item.vl_liquido_item;
                                            compVlLiquido.oldValue = unmaskFixed(item.vl_liquido_item, 2);
                                            compVlLiquido._onChangeActive = true;
                                        }
                                        if (dijit.byId("ckNotaFiscal").checked) {
                                            switch (TIPOMOVIMENTO) {
                                                case ENTRADA:
                                                case SAIDA:
                                                case DEVOLUCAO:
                                                    dijit.byId("baseCalcPISItem").set("value", compVlLiquido.value);
                                                    dijit.byId("baseCalcCOFINSItem").set("value", compVlLiquido.value);
                                                    dijit.byId("baseCalcIPIItem").set("value", compVlLiquido.value);
                                                    var reducao = dijit.byId('tpNf').reducao;
                                                    if (hasValue(reducao) && reducao > 0) {
                                                        var baseReduzida = compVlLiquido.value - ((compVlLiquido.value * reducao) / 100);
                                                        dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                    }
                                                    else {
                                                        dijit.byId("baseCalcICMSItem").set("value", compVlLiquido.value);
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                    }
                                                    if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                        dijit.byId("baseCalcICMSItem").set("value", 0);
                                                    }
                                                    break;
                                                case SERVICO:
                                                    var baseISS = dijit.byId("baseCalcISSItem");
                                                    baseISS.set("value", compVlLiquido.value);

                                                    break;
                                            }
                                            if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                                var vlAprox = compVlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                                dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                            }
                                        }
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("baseCalcISSItem").on("change", function (baseISS) {
                            try {
                                var pcAliqISS = dijit.byId("aliquotaISSItem");
                                var vlISS = dijit.byId("valorISSItem");
                                if (pcAliqISS.value > 0) {
                                    var valorISS = (baseISS * pcAliqISS.value) / 100;
                                    vlISS.set("value", valorISS);
                                }
                                else {
                                    var percentual = (vlISS.value * 100) / baseISS;
                                    pcAliqISS.set("value", maskFixed(percentual, 2));
                                }
                                if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                    var vlAprox = dijit.byId("vlLiquido").value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("pc_aliquota_ap_item").on("change", function (pcAliquota) {
                            try {
                                if (dijit.byId("vlLiquido").value > 0) {
                                    var vlAprox = dijit.byId("vlLiquido").value * pcAliquota / 100;
                                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("aliquotaISSItem").on("change", function (pcAliqISS) {
                            try {
                                var baseISS = dijit.byId("baseCalcISSItem");
                                var vlISS = dijit.byId("valorISSItem");
                                if (baseISS.value > 0) {
                                    var valorISS = (baseISS.value * pcAliqISS) / 100;
                                    vlISS.set("value", valorISS);
                                }
                                else {
                                    var base = (vlISS.value * 100) / pcAliqISS;
                                    baseISS.set("value", maskFixed(base, 2));
                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                        var vlAprox = dijit.byId("vlLiquido").value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("valorISSItem").on("change", function (vlISS) {
                            try {
                                var baseISS = dijit.byId("baseCalcISSItem");
                                var pcAliqISS = dijit.byId("aliquotaISSItem");
                                if (baseISS.value > 0) {
                                    var perISS = (vlISS * 100) / baseISS.value;
                                    pcAliqISS.set("value", perISS);
                                }
                                else {
                                    var base = (vlISS * 100) / pcAliqISS.value;
                                    baseISS.set("value", base);
                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                        var vlAprox = dijit.byId("vlLiquido").value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("ckNotaFiscalPesq").on("change", function (e) {
                            if (dijit.byId(ckNotaFiscalPesq).checked) {
                                dojo.byId("tdLblPesqStatusNF").style.display = "";
                                dojo.byId("tdPesqStatusNF").style.display = "";
                                if (TIPOMOVIMENTO != DESPESAS)
                                    dijit.byId("gridMovimento").layout.setColumnVisibility(10, true);
                                else
                                    dijit.byId("gridMovimento").layout.setColumnVisibility(10, false);
                            }
                            else {
                                dojo.byId("tdLblPesqStatusNF").style.display = "none";
                                dojo.byId("tdPesqStatusNF").style.display = "none";
                                dijit.byId("gridMovimento").layout.setColumnVisibility(10, false)                            
                            }

                            if (CONSULTA_INICIAL == false) {
                                pesquisarMovimento(true);
                            }
                            CONSULTA_INICIAL = false;

                        });

                        dijit.byId("vlTotalMovimento").on("change", function (vlTotal) {
                            try {
                                var compvlTotal = dijit.byId("vlTotalMovimento");
                                var vlUnitario = dijit.byId("vlUnitario");
                                var compVlLiquido = dijit.byId("vlLiquido");
                                if (unmaskFixed(compvlTotal.oldValue, 2) != unmaskFixed(vlTotal, 2)) {
                                    var item = null;
                                    var vlUnit = null;
                                    if (isNaN(vlTotal)) {
                                        compvlTotal.set('value', 0);
                                        compvlTotal.value = 0;
                                        compvlTotal.oldValue = 1;
                                    } else {
                                        if (vlTotal > 0) {
                                            vlUnit = vlTotal / dijit.byId("qtd_item").value;
                                            if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                                                dijit.byId('valDescontoItem')._onChangeActive = false;
                                                dijit.byId('vlLiquido')._onChangeActive = false;
                                                dijit.byId('valDescontoItem').set("value", (vlTotal * dijit.byId('perDescontoItem').value) / 100);
                                                dijit.byId("vlLiquido").set("value", (vlTotal - dijit.byId('valDescontoItem').value));
                                                dijit.byId('valDescontoItem')._onChangeActive = true;
                                                dijit.byId('vlLiquido')._onChangeActive = true;
                                            }
                                            else
                                                if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                                                    dijit.byId('perDescontoItem')._onChangeActive = false;
                                                    dijit.byId('vlLiquido')._onChangeActive = false;
                                                    dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / vlTotal);
                                                    dijit.byId("vlLiquido").set("value", (vlTotal - dijit.byId('valDescontoItem').value));
                                                    dijit.byId('perDescontoItem')._onChangeActive = true;
                                                    dijit.byId('vlLiquido')._onChangeActive = true;
                                                }
                                                else
                                                    item = calcularDescAcrescItemView(dijit.byId("qtd_item").value, vlTotal, null, vlUnit);
                                        } else
                                            if (vlUnitario.value > 0) {
                                                vlTotal = dijit.byId("qtd_item").value * vlUnitario.value;
                                                totalItens = compvlTotal.value;
                                                vlUnit = vlUnitario.value;
                                                if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                                                    dijit.byId('valDescontoItem')._onChangeActive = false;
                                                    dijit.byId('vlLiquido')._onChangeActive = false;
                                                    dijit.byId('valDescontoItem').set("value", (totalItens * dijit.byId('perDescontoItem').value) / 100);
                                                    dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                                    dijit.byId('valDescontoItem')._onChangeActive = true;
                                                    dijit.byId('vlLiquido')._onChangeActive = true;
                                                }
                                                else
                                                    if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                                                        dijit.byId('perDescontoItem')._onChangeActive = false;
                                                        dijit.byId('vlLiquido')._onChangeActive = false;
                                                        dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / totalItens);
                                                        dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                                                        dijit.byId('perDescontoItem')._onChangeActive = true;
                                                        dijit.byId('vlLiquido')._onChangeActive = true;
                                                    }
                                                    else
                                                        item = calcularDescAcrescItemView(dijit.byId("qtd_item").value, vlTotal, null, vlUnit);
                                                compvlTotal._onChangeActive = false;
                                                compvlTotal.set("value", unmaskFixed(vlTotal, 2));
                                                compvlTotal.value = vlTotal;
                                                compvlTotal.oldValue = unmaskFixed(vlTotal, 2);
                                                compvlTotal._onChangeActive = true;
                                            }
                                        vlUnitario._onChangeActive = false;
                                        vlUnitario.set("value", unmaskFixed(vlUnit, 2));
                                        vlUnitario.value = vlUnit;
                                        vlUnitario.oldValue = unmaskFixed(vlUnit, 2);
                                        vlUnitario._onChangeActive = true;
                                        compVlLiquido._onChangeActive = false;
                                        if (hasValue(item)) {
                                            compVlLiquido.set("value", unmaskFixed(item.vl_liquido_item, 2));
                                            compVlLiquido.value = item.vl_liquido_item;
                                            compVlLiquido.oldValue = unmaskFixed(item.vl_liquido_item, 2);
                                            compVlLiquido._onChangeActive = true;
                                        }
                                    }
                                    if (dijit.byId("ckNotaFiscal").checked) {
                                        switch (TIPOMOVIMENTO) {
                                            case ENTRADA:
                                            case SAIDA:
                                            case DEVOLUCAO:
                                                dijit.byId("baseCalcPISItem").set("value", compVlLiquido.value);
                                                dijit.byId("baseCalcCOFINSItem").set("value", compVlLiquido.value);
                                                dijit.byId("baseCalcIPIItem").set("value", compVlLiquido.value);
                                                var reducao = dijit.byId('tpNf').reducao;
                                                if (hasValue(reducao) && reducao > 0) {
                                                    var baseReduzida = compVlLiquido.value - ((compVlLiquido.value * reducao) / 100);
                                                    dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                }
                                                else {
                                                    dijit.byId("baseCalcICMSItem").set("value", compVlLiquido.value);
                                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                }

                                                if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                    dijit.byId("baseCalcICMSItem").set("value", 0);
                                                }
                                                break;
                                            case SERVICO:
                                                var baseISS = dijit.byId("baseCalcISSItem");
                                                baseISS.set("value", compVlLiquido.value);

                                                break;
                                        }
                                        if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                            var vlAprox = dijit.byId("vlLiquido").value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                            dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                        }
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("pcDesconto").on("change", function (pcDesc) {
                            try {
                                var compPcDesc = dijit.byId("pcDesconto");
                                if (isNaN(pcDesc)) {
                                    compPcDesc.set('value', 0);
                                } else
                                    if (dijit.byId("pcDesconto").oldValue != pcDesc)
                                        calcularEAplicarDesconto(pcDesc, null);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("vlDesconto").on("change", function (vlDesc) {
                            try {
                                var compVlDesc = dijit.byId("vlDesconto");
                                if (isNaN(vlDesc))
                                    compVlDesc.set('value', 0);
                                else
                                    if (dijit.byId("vlDesconto").oldValue != vlDesc)
                                        calcularEAplicarDesconto(null, vlDesc);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("perDescontoItem").on("change", function (pcDesc) {
                            try {
                                var compPcDesc = dijit.byId("perDescontoItem");
                                if (isNaN(pcDesc)) {
                                    compPcDesc.set('value', 0);
                                } else
                                    calcularEAplicarDescontoItem(pcDesc, null);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("valDescontoItem").on("change", function (vlDesc) {
                            try {
                                var compVlDesc = dijit.byId("valDescontoItem");
                                if (isNaN(vlDesc))
                                    compVlDesc.set('value', 0);
                                else
                                    if (dijit.byId("valDescontoItem").oldValue != vlDesc)
                                        calcularEAplicarDescontoItem(null, vlDesc);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("pcAcrec").on("change", function (pcAcresc) {
                            try {
                                var compPcAcrec = dijit.byId("pcAcrec");
                                if (isNaN(pcAcresc)) {
                                    compPcAcrec.set('value', 0);
                                } else
                                    if (dijit.byId("pcAcrec").oldValue != pcAcresc)
                                        calcularEAplicarAcrescimo(pcAcresc, null);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("vlAc").on("change", function (vlAcresc) {
                            try {
                                var compVlAc = dijit.byId("vlAc");
                                if (isNaN(vlAcresc))
                                    compVlAc.set('value', 0);
                                else
                                    if (dijit.byId("vlAc").oldValue != vlAcresc)
                                        calcularEAplicarAcrescimo(null, vlAcresc);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("dtaMovto").on("change", function (e) {
                            try {
                                var compDtaMovto = dijit.byId("dtaMovto");
                                var compDtaEmis = dijit.byId("dtaEmis");
                                if (hasValue(compDtaEmis.value) && hasValue(compDtaMovto.value) && dojo.date.compare(compDtaEmis.get("value"), e) > 0) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorDataVencOrMovtoMaiorDataEmissao);
                                    apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                                    compDtaMovto._onChangeActive = false;
                                    compDtaMovto.reset();
                                    compDtaMovto._onChangeActive = true;
                                } else {
                                    apresentaMensagem('apresentadorMensagemMovto', null);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("tipoDocumentoTit").on("change", function (e) {
                            try {

                                if (hasValue(e)) {
	                                var tgCartao = dojo.byId("tgCartao");
                                    if (!(validaEhCartaoOuChequeAlterouParaCartaoOuCheque() || validaNaoEhCartaoEChequeAlterouParaCartaoOuCheque()) && e == CARTAO && (TIPOMOVIMENTO == SERVICO || TIPOMOVIMENTO == ENTRADA || TIPOMOVIMENTO == SAIDA)) {
		                                tgCartao.style.display = "block";
	                                } else {
		                                tgCartao.style.display = "none";
                                    }

                                    if (e != CARTAO) {
                                        dijit.byId("nm_dias_cartao")._onChangeActive = false;
                                        dijit.byId("pc_taxa_cartao")._onChangeActive = false;
                                        //dijit.byId('vl_taxa_cartao')._onChangeActive = false;
                                        dijit.byId("pc_taxa_cartao").set("value", 0);
                                        dijit.byId("nm_dias_cartao").set("value", 0);

                                        //formata o valor
                                        vl_taxa_format = parseFloat(0).toFixed(2).replace(".", ",");
	                                        
                                        dojo.byId('vl_taxa_cartao').value = vl_taxa_format;
                                        dijit.byId("nm_dias_cartao")._onChangeActive = true;
                                        dijit.byId("pc_taxa_cartao")._onChangeActive = true;
                                        //dijit.byId('vl_taxa_cartao')._onChangeActive = true;
                                    }
                                }

                                if ((validaEhCartaoOuChequeAlterouParaCartaoOuCheque() || validaNaoEhCartaoEChequeAlterouParaCartaoOuCheque())) {
		                            dijit.byId("tipoDocumentoTit").set("value", "");

		                            apresentaMensagem('apresentadorMensagemTitulo', null);
		                            var mensagensWeb = new Array();
		                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
			                            "Não é permitido alterar este título para os tipos Cartão ou Cheque");
		                            apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                                } else {
                                    // ** para não limpar o banco (caso o combo seja cartão e o banco também)
                                    if (validaValorOriginalEhCartaoELocalEhCartao()) {
	                                    apresentaMensagem('apresentadorMensagemTitulo', null);
                                    } else {
	                                    dijit.byId("bancoTit").set("value", "");
	                                    apresentaMensagem('apresentadorMensagemTitulo', null);
                                    }
                                }

                                
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });


                        dijit.byId("bancoTit").on("change", function (e) {
                            try {

                                if ((hasValue(dijit.byId("tipoDocumentoTit").item) &&
                                    (dijit.byId("tipoDocumentoTit").item.id == CARTAO)) &&
                                    (hasValue(dijit.byId("bancoTit").item) && (dijit.byId("bancoTit").item.nm_tipo_local != 4 && dijit.byId("bancoTit").item.nm_tipo_local != 5))) {
                                    dijit.byId("bancoTit").set("value", "");

                                    apresentaMensagem('apresentadorMensagemTitulo', null);
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                        "Para títulos com tipo financeiro “cartão” somente Locais de Movimento tipo “cartão de débito/crédito poderão ser selecionados, caso contrário estes tipos não poderão.");
                                    apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                                } else

                                if ((hasValue(dijit.byId("tipoDocumentoTit").item) &&
                                    (dijit.byId("tipoDocumentoTit").item.id != CARTAO)) &&
                                    (hasValue(dijit.byId("bancoTit").item) && (dijit.byId("bancoTit").item.nm_tipo_local == 4 || dijit.byId("bancoTit").item.nm_tipo_local == 5))) {
                                    dijit.byId("bancoTit").set("value", "");

                                    apresentaMensagem('apresentadorMensagemTitulo', null);
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                        "Para títulos com tipo financeiro diferente de “cartão”, os Locais de Movimento do tipo “cartão de débito/crédito não poderão ser selecionados.");
                                    apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                                } else {
                                    apresentaMensagem('apresentadorMensagemTitulo', null);
                                }
                                if ((hasValue(dijit.byId("tipoDocumentoTit").item) &&
                                    (dijit.byId("tipoDocumentoTit").item.id == CARTAO)) &&
                                    (hasValue(dijit.byId("bancoTit").item) && (dijit.byId("bancoTit").item.nm_tipo_local == LOCALCARTAODEBITO || dijit.byId("bancoTit").item.nm_tipo_local == LOCALCARTAOCREDITO))) {
                                    aplicarTaxaBancaria(dijit.byId('gridTitulo').itemSelecionado.cd_titulo, e);
                                }
                                


                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("valorTit").on('change', function(e) {

                            if (hasValue(e)) {
	                            
                                if (hasValue(dijit.byId("tipoDocumentoTit").item) && (dijit.byId("tipoDocumentoTit").id == CARTAO)) {
	                                var pc_taxa_cartao = dijit.byId("pc_taxa_cartao").value;
	                                var vl_titulo = e;
	                                var vl_taxa_cartao = pc_taxa_cartao * (vl_titulo / 100);
	                                dojo.byId("vl_taxa_cartao").value = maskFixed(vl_taxa_cartao, 2);
                                }

	                        }
                        });


                        dijit.byId("pc_taxa_cartao").on('change', function (e) {

                            if (hasValue(e)) {
                                if (hasValue(dijit.byId("tipoDocumentoTit").item) && (dijit.byId("tipoDocumentoTit").item.id == CARTAO)) {
			                        var pc_taxa_cartao = dijit.byId("pc_taxa_cartao").value;
			                        var vl_titulo = parseFloat((dojo.byId("valorTit").value).replace(",", "."));
			                        var vl_taxa_cartao = pc_taxa_cartao * (vl_titulo / 100);
                                    dojo.byId("vl_taxa_cartao").value = maskFixed(vl_taxa_cartao, 2);
	                            }
	                        }

                        });

                        dijit.byId("vl_taxa_cartao").on('change', function (e) {
                            if (hasValue(e)) {
                                if (hasValue(dijit.byId("tipoDocumentoTit").item) && (dijit.byId("tipoDocumentoTit").item.id == CARTAO)) {
			                        //pc_taxa_cartao = vl_taxa_cartao / vl_titulo/ 100.0
			                        var vl_taxa_cartao = parseFloat((dojo.byId("vl_taxa_cartao").value).replace(",", "."));
			                        var vl_titulo = parseFloat((dojo.byId("valorTit").value).replace(",", "."));
			                        var pc_taxa_cartao = vl_taxa_cartao / (vl_titulo / 100);
			                        var pc_taxa_cartao_mask = maskFixed(pc_taxa_cartao, 2);
	                                    dijit.byId("pc_taxa_cartao").set("value", unmaskFixed(pc_taxa_cartao_mask, 2));
	                            }
	                        }
                        });

                        dijit.byId("fkPoliticaComercial").on("show", function (e) {
                            dijit.byId("gridPolicaComercialFK").update();
                        });
                        //Componetes que ao serem modificados geraram novos titulos
                        dijit.byId("dtaEmis").on("change", function (e) {
                            gerar_titulo = true;
                        });

                        dijit.byId("dtaVenc").on("change", function (e) {
                            try {
                                var compDtaVenc = dijit.byId("dtaVenc");
                                var compDtaEmis = dijit.byId("dtaEmis");
                                gerar_titulo = true;
                                if (hasValue(compDtaEmis.value) && hasValue(compDtaVenc.value) && dojo.date.compare(compDtaEmis.get("value"), e) > 0) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorDataVencOrMovtoMaiorDataEmissao);
                                    apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                                    compDtaVenc._onChangeActive = false;
                                    compDtaVenc.reset();
                                    compDtaVenc._onChangeActive = true;
                                } else {
                                    apresentaMensagem('apresentadorMensagemMovto', null);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("totalGeral").on("change", function (e) {
                            console.log("totalGeral");
                            gerar_titulo = true;
                        });

                        dijit.byId("totalItens").on("change", function (e) {
                            console.log("totalItens");
                            gerar_titulo = true;
                        });

                        //Aba fiscal item
                        dijit.byId("baseCalcICMSItem").on("change", function (baseICMS) {
                            try {
                                var pcAliqICMSItem = dijit.byId("aliquotaICMSItem");
                                var vlICMSItem = dijit.byId("valorICMSItem");
                                if (pcAliqICMSItem.value > 0) {
                                    var valorICMS = (baseICMS * pcAliqICMSItem.value) / 100;
                                    vlICMSItem.set("value", valorICMS);
                                }
                                else {
                                    var percentual = (vlICMSItem.value * 100) / baseICMS;
                                    pcAliqICMSItem.set("value", maskFixed(percentual, 2));
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("sitTribItem").on("change", function (e) {
                            try {
                                if (hasValue(e)) {
                                    var listaSituacao = dijit.byId("sitTribItem").store.data;

                                    var situacao = binaryObjSearch(listaSituacao, "id", dijit.byId("sitTribItem").value);

                                    if (listaSituacao[situacao].formaTrib == ISENTO) {

                                        dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                                        dijit.byId("aliquotaICMSItem").set("value", 0);
                                        dijit.byId("aliquotaICMSItem").set("disabled", true);
                                    }
                                    else {
                                        if (listaSituacao[situacao].formaTrib == REDUZIDO) {
                                            dijit.byId("aliquotaICMSItem").set("disabled", false);
                                            var reducao = dijit.byId('tpNf').reducao;
                                            if (hasValue(reducao) && reducao > 0) {
                                                var baseReduzida = vlLiquido.value - ((vlLiquido.value * reducao) / 100);
                                                dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                                dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;

                                            }
                                        }
                                        dijit.byId("aliquotaICMSItem").set("disabled", false);
                                        dijit.byId("aliquotaICMSItem").set("value", dijit.byId("aliquotaICMSItem").valueFixo);
                                        if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                            dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").valueFixo;
                                            dijit.byId("baseCalcICMSItem").set("value", 0);
                                        }
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }

                        });

                        dijit.byId("aliquotaICMSItem").on("change", function (pcAliqICMS) {
                            try {
                                if (dijit.byId("baseCalcICMSItem").value == 0) {
                                    if (dijit.byId("baseCalcICMSItem").old_value > 0)
                                        dijit.byId("baseCalcICMSItem").set("value", dijit.byId("baseCalcICMSItem").old_value);
                                    else
                                        dijit.byId("baseCalcICMSItem").set("value", dijit.byId("vlLiquido").value);

                                }
                                var baseICMSItem = dijit.byId("baseCalcICMSItem");
                                var vlICMSItem = dijit.byId("valorICMSItem");
                                if (baseICMSItem.value > 0) {
                                    var valorISS = (baseICMSItem.value * pcAliqICMS) / 100;
                                    vlICMSItem.set("value", valorISS);
                                }
                                else {
                                    var base = (vlICMSItem.value * 100) / pcAliqICMS;
                                    baseICMSItem.set("value", maskFixed(base, 2));
                                    baseICMSItem.old_value = baseICMSItem.value;
                                }
                                if (pcAliqICMS <= 0) {
                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                    dijit.byId("baseCalcICMSItem").set("value", 0);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("baseCalcPISItem").on("change", function (basePIS) {
                            try {
                                var pcAliqPISItem = dijit.byId("aliquotaPISItem");
                                var vlPISItem = dijit.byId("valorPISItem");
                                if (pcAliqPISItem.value > 0) {
                                    var valorICMS = (basePIS * pcAliqPISItem.value) / 100;
                                    vlPISItem.set("value", valorICMS);
                                }
                                else {
                                    var percentual = (vlPISItem.value * 100) / basePIS;
                                    pcAliqPISItem.set("value", maskFixed(percentual, 2));
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("aliquotaPISItem").on("change", function (pcAliqPIS) {
                            try {
                                if (dijit.byId("baseCalcPISItem").value == 0) {
                                    if (dijit.byId("baseCalcPISItem").old_value > 0)
                                        dijit.byId("baseCalcPISItem").set("value", dijit.byId("baseCalcPISItem").old_value);
                                    else
                                        dijit.byId("baseCalcPISItem").set("value", dijit.byId("vlLiquido").value);
                                }

                                var basePISItem = dijit.byId("baseCalcPISItem");
                                var vlPISItem = dijit.byId("valorPISItem");
                                if (basePISItem.value > 0) {
                                    var valorISS = (basePISItem.value * pcAliqPIS) / 100;
                                    vlPISItem.set("value", valorISS);
                                }
                                else {
                                    var base = (vlPISItem.value * 100) / pcAliqPIS;
                                    basePISItem.set("value", maskFixed(base, 2));
                                    basePISItem.old_value = basePISItem.value;
                                }

                                if (pcAliqPIS <= 0) {
                                    dijit.byId("baseCalcPISItem").old_value = dijit.byId("baseCalcPISItem").value;
                                    dijit.byId("baseCalcPISItem").set("value", 0);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("baseCalcCOFINSItem").on("change", function (baseCOFINS) {
                            try {
                                var pcAliqCOFINSItem = dijit.byId("aliquotaCOFINSItem");
                                var vlCOFINSItem = dijit.byId("valorCOFINSItem");
                                if (pcAliqCOFINSItem.value > 0) {
                                    var valorICMS = (baseCOFINS * pcAliqCOFINSItem.value) / 100;
                                    vlCOFINSItem.set("value", valorICMS);
                                }
                                else {
                                    var percentual = (vlCOFINSItem.value * 100) / baseCOFINS;
                                    pcAliqCOFINSItem.set("value", maskFixed(percentual, 2));
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("aliquotaCOFINSItem").on("change", function (pcAliqCOFINS) {
                            try {
                                if (dijit.byId("baseCalcCOFINSItem").value == 0) {
                                    if (dijit.byId("baseCalcCOFINSItem").old_value > 0)
                                        dijit.byId("baseCalcCOFINSItem").set("value", dijit.byId("baseCalcCOFINSItem").old_value);
                                    else
                                        dijit.byId("baseCalcCOFINSItem").set("value", dijit.byId("vlLiquido").value);

                                }
                                var baseCOFINSItem = dijit.byId("baseCalcCOFINSItem");
                                var vlCOFINSItem = dijit.byId("valorCOFINSItem");
                                if (baseCOFINSItem.value > 0) {
                                    var valorISS = (baseCOFINSItem.value * pcAliqCOFINS) / 100;
                                    vlCOFINSItem.set("value", valorISS);
                                    baseCOFINSItem.old_value = baseCOFINSItem.value;
                                }
                                else {
                                    var base = (vlCOFINSItem.value * 100) / pcAliqCOFINS;
                                    baseCOFINSItem.set("value", maskFixed(base, 2));
                                }
                                if (pcAliqCOFINS <= 0) {
                                    dijit.byId("baseCalcCOFINSItem").old_value = dijit.byId("baseCalcCOFINSItem").value;
                                    dijit.byId("baseCalcCOFINSItem").set("value", 0);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("baseCalcIPIItem").on("change", function (baseIPI) {
                            try {
                                var pcAliqIPIItem = dijit.byId("aliquotaIPIItem");
                                var vlIPIItem = dijit.byId("valorIPIItem");
                                if (pcAliqIPIItem.value > 0) {
                                    var valorICMS = (baseIPI * pcAliqIPIItem.value) / 100;
                                    vlIPIItem.set("value", valorICMS);
                                }
                                else {
                                    var percentual = (vlIPIItem.value * 100) / baseIPI;
                                    pcAliqIPIItem.set("value", maskFixed(percentual, 2));
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("aliquotaIPIItem").on("change", function (pcAliqIPI) {
                            try {
                                if (dijit.byId("baseCalcIPIItem").value == 0) {
                                    if (dijit.byId("baseCalcIPIItem").old_value > 0)
                                        dijit.byId("baseCalcIPIItem").set("value", dijit.byId("baseCalcIPIItem").old_value);
                                    else
                                        dijit.byId("baseCalcIPIItem").set("value", dijit.byId("vlLiquido").value);
                                }
                                var baseIPIItem = dijit.byId("baseCalcIPIItem");
                                var vlIPIItem = dijit.byId("valorIPIItem");
                                if (baseIPIItem.value > 0) {
                                    var valorISS = (baseIPIItem.value * pcAliqIPI) / 100;
                                    vlIPIItem.set("value", valorISS);
                                }
                                else {
                                    var base = (vlIPIItem.value * 100) / pcAliqIPI;
                                    baseIPIItem.set("value", maskFixed(base, 2));
                                    baseIPIItem.old_value = baseIPIItem.value;
                                }

                                if (pcAliqIPI <= 0) {
                                    dijit.byId("baseCalcIPIItem").old_value = dijit.byId("baseCalcIPIItem").value;
                                    dijit.byId("baseCalcIPIItem").set("value", 0);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        if (hasValue(dijit.byId("menuManual"))) {
                            dijit.byId("menuManual").on("click",
                                function (e) {
                                    try {
                                        var parametros = getParamterosURL();
                                        if (hasValue(parametros['tipo']))
                                            switch (TIPOMOVIMENTO) {
                                                case ENTRADA:
                                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323048',
                                                        '765px',
                                                        '771px');
                                                    break;
                                                case SAIDA:
                                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323049',
                                                        '765px',
                                                        '771px');
                                                    break;
                                                case SERVICO:
                                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323050',
                                                        '765px',
                                                        '771px');
                                                    break;
                                                case DESPESAS:
                                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323051',
                                                        '765px',
                                                        '771px');
                                                    break;
                                                default:
                                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323047',
                                                        '765px',
                                                        '771px');
                                                    break;
                                            }

                                    } catch (e) {
                                        postGerarLog(e);
                                    }
                                });
                        }
                        //Eventos Fiscal
                        dijit.byId("ckNotaFiscal").on("change", function (e) {
                            try {
                                var validado = verificarSeExisteFiscalItensMovimento(e);
                                if (validado) {
                                    configuraLayoutNF(e);
                                    verificaOperacaoCFOP();
                                }
                                if (empresa_propria && e && TIPOMOVIMENTO==ENTRADA) {
                                    dijit.byId("dc_key_nfe").set("disabled", false);
                                    dijit.byId('dc_key_nfe').set('required', true);
                                }
                                else {
                                    dijit.byId("dc_key_nfe").set("disabled", true);
                                    dijit.byId('dc_key_nfe').set('required', false);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("noPessoaMovto").on("change", function (e) {
                            if (hasValue(e))
                                gerar_titulo = true;
                        });

                        dijit.byId('paBaixa').on('show', function (e) {
                            if (hasValue(dijit.byId('gridBaixa')))
                                dijit.byId('gridBaixa').update();
                        })

                        dijit.byId("tabTitulo").on("show", function (e) {
                            dijit.byId('PaiGridTitulo').resize();
                            dojo.byId('gridTitulo').style.width = '1500px';

                            dijit.byId("edBanco").value = null;
                            dojo.byId("edBanco").value = null;
                            var cd_tipo_financeiro = dijit.byId("tpFinanceiro").value;
                            if (hasValue(cd_tipo_financeiro))
                                getLocalMovtoGeralOuCartaoMovimento(cd_tipo_financeiro);
                        });

                        dijit.byId("historicoBaixaTitulo").on("show", function (event) {
                            try {
                                if (hasValue(dijit.byId('gridHistoricoBaixaTitulo')))
                                    dijit.byId('gridHistoricoBaixaTitulo').update();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        dijit.byId("gridMovimento").resize();
                        var parametros = getParamterosURL();
                            if ((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1)
                                dijit.byId('novoMovto').set("disabled", true);
                            if (hasValue(parametros['idOrigemNF']))
                                configuraDadosMovimento(parametros['idOrigemNF'],
                                    (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false),
                                    (((parametrosTela['id_futura'] != null && parametrosTela['id_futura'] != undefined) && eval(parametrosTela['id_futura']) == 1) ? true : false));

                        
                        showCarregando();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
				function (error) {
				    showCarregando();
				    apresentaMensagem('apresentadorMensagem', error);
				});
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
};

function validaEhCartaoOuChequeAlterouParaCartaoOuCheque() {
    try {
        return (
	       //se tem valor no item original e no item a mudar e
		    (dojo.byId("vlOriginalTipoDocumento").value > 0 &&
				    hasValue(dijit.byId("tipoDocumentoTit").item)) &&
			    //se o item a mudar fo cartão ou cheque e
			    (dijit.byId("tipoDocumentoTit").item.id == CARTAO ||
				    dijit.byId("tipoDocumentoTit").item.id == CHEQUE) &&
			    //se o valor original do combo for cartão ou cheque e mudei para um valor diferente do original(Ex: era Cartao e mudei pra Cheque)
			    //assim permite voltar pro valor original(ex: (sem clicar em alterar) -> era Cartão e mudei pra Cartão )
			    ((dojo.byId("vlOriginalTipoDocumento").value == CARTAO ||
					    dojo.byId("vlOriginalTipoDocumento").value == CHEQUE) &&
				    dojo.byId("vlOriginalTipoDocumento").value != dijit.byId("tipoDocumentoTit").item.id)
	    );
    } catch (e) {
	    postGerarLog(e);
    }
}

function validaNaoEhCartaoEChequeAlterouParaCartaoOuCheque() {
	try {
        return (
	        //se tem valor no item original e no item a inserir e
	        (dojo.byId("vlOriginalTipoDocumento").value > 0 &&
                hasValue(dijit.byId("tipoDocumentoTit").item)) &&
		        //se o item a inserir fo cartão ou cheque e
		        (dijit.byId("tipoDocumentoTit").item.id == CARTAO ||
                dijit.byId("tipoDocumentoTit").item.id == CHEQUE) &&
		        //se o valor original do combo não for cartão && cheque
		        ((dojo.byId("vlOriginalTipoDocumento").value != CARTAO && dojo.byId("vlOriginalTipoDocumento").value != CHEQUE)
		        )
        );
	} catch (e) {
		postGerarLog(e);
	}
}


function validaValorOriginalEhCartaoELocalEhCartao() {

    
	return (dojo.byId("vlOriginalTipoDocumento").value > 0 &&
			hasValue(dijit.byId("tipoDocumentoTit").item
			)) &&
		//se o valor original for cartão, o valor atual for cartão e o local também for cartão
		(dijit.byId("tipoDocumentoTit").item.id == CARTAO &&
		(hasValue(dijit.byId("bancoTit").item) &&
		(dijit.byId("bancoTit").item.nm_tipo_local == 4 ||
			dijit.byId("bancoTit").item.nm_tipo_local == 5)));
}

function configuraLayoutNF(bool) {
    if (bool) {
        document.getElementById("trFuturaCad").style.display = "none";
        dojo.byId("tagFiscalItem").style.display = "";
        dojo.byId("paiTabFiscal").style.display = "";
        dojo.byId("trFiscalPrinc").style.display = "";
        dojo.byId("tdCadCFOPItem").style.display = "";
        dojo.byId("tdLbsCFOPItem").style.display = "";
        dijit.byId('tpNfDev').set('required', false);
        dijit.byId("tpNf").set("required", true);
        dijit.byId("descCFOPItem").set("required", true);
        if (hasValue(TIPOMOVIMENTO)) {
            switch (TIPOMOVIMENTO) {
                case SAIDA:
                    dojo.byId("tagIcms").style.display = "";
                    dojo.byId("tagPis").style.display = "";
                    dojo.byId("tagCof").style.display = "";
                    dojo.byId("tagIpi").style.display = "";
                    dojo.byId("tbFiscalServico").style.display = "none";
                    dojo.byId("lblNroMvto").innerHTML = "Número NF de Saída:";
                    document.getElementById("trMeioPgto").style.display = "";
                    dijit.byId("cad_meio_pagamento").set("required", true);
                    break;
                case SERVICO:
                    dojo.byId("tagIcms").style.display = "none";
                    dojo.byId("tagPis").style.display = "none";
                    dojo.byId("tagCof").style.display = "none";
                    dojo.byId("tagIpi").style.display = "none";
                    dojo.byId("tbFiscalServico").style.display = "";
                    dojo.byId("lblNroMvto").innerHTML = "Número NF de Serviço:";
                    break;
                case ENTRADA:
                    dojo.byId("tagIcms").style.display = "";
                    dojo.byId("tagPis").style.display = "";
                    dojo.byId("tagCof").style.display = "";
                    dojo.byId("tagIpi").style.display = "";
                    dojo.byId("tbFiscalServico").style.display = "none";
                    dojo.byId("lblNroMvto").innerHTML = "Número NF Entrada:";
                    dijit.byId("nfEsc").set("disabled", false);
                    dijit.byId("nfEsc").set("checked", false);
                    break;
                case DEVOLUCAO:
                    dojo.byId("tagIcms").style.display = "";
                    dojo.byId("tagPis").style.display = "";
                    dojo.byId("tagCof").style.display = "";
                    dojo.byId("tagIpi").style.display = "";
                    dojo.byId("tbFiscalServico").style.display = "none";
                    dojo.byId("lblNroMvto").innerHTML = "Número NF Devolução:";
                    //dijit.byId("nfEsc").set("checked", true);
                    //dijit.byId("nfEsc").set("disabled", true);
                    dojo.byId("trNFDevolucao").style.display = "";
                    dijit.byId('tpNfDev').set('required', true);
                    dojo.byId("lbNotaDevolver").innerHTML = "Nota a Devolver:";

                    break;
            }
        }
        var tipo = TIPOMOVIMENTO;
        if (tipo == DEVOLUCAO)
            tipo = dojo.byId('id_natureza_movto').value;
        if (numeracao_NF_automatica && dijit.byId("nfEsc").get("checked") && (tipo != ENTRADA && tipo != DESPESAS)) {
            dijit.byId("nrMovto").set("disabled", true);
            dijit.byId("serie").set("disabled", true);
            dijit.byId("nrMovto").set("required", false);
            dijit.byId("serie").set("required", false);
        } else {
            dijit.byId("nrMovto").set("required", true);
            dijit.byId("serie").set("required", true);
        }

    } else {
        dojo.byId("tagIcms").style.display = "none";
        dojo.byId("tagPis").style.display = "none";
        dojo.byId("tagCof").style.display = "none";
        dojo.byId("tagIpi").style.display = "none";
        dojo.byId("tagFiscalItem").style.display = "none";
        dojo.byId("tbFiscalServico").style.display = "none";
        dojo.byId("paiTabFiscal").style.display = "none";
        dojo.byId("trFiscalPrinc").style.display = "none";
        if (hasValue(TIPOMOVIMENTO) && TIPOMOVIMENTO != DEVOLUCAO)
            dojo.byId("trNFDevolucao").style.display = "none";
        else {
            dojo.byId("trNFDevolucao").style.display = "";
            dojo.byId("lbNotaDevolver").innerHTML = "Movimento a Devolver:";
        }
        dojo.byId("tdCadCFOPItem").style.display = "none";
        dojo.byId("tdLbsCFOPItem").style.display = "none";
        dijit.byId("descCFOPItem").set("required", false);
        dijit.byId("tpNf").set("required", false);
        dijit.byId("nrMovto").set("disabled", false);
        dijit.byId("serie").set("disabled", false);
        dijit.byId("nrMovto").set("required", false);
        dijit.byId("serie").set("required", false);
        document.getElementById("trMeioPgto").style.display = "none";
        dijit.byId("cad_meio_pagamento").set("required", false);
        configLayoutOpçoesMovimento(TIPOMOVIMENTO, false, false, false);
    }
}

//Função para colocar id's nos componetes de tab para poder esconder-las.
function inserirIdTabsCadastro() {
    try {
        if (hasValue(dojo.byId("tabContainer_tablist_tabFiscal")))
            dojo.byId("tabContainer_tablist_tabFiscal").parentElement.id = "paiTabFiscal";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function habilitarTagCheque(bool) {
    try {
        if (!bool)
            dojo.byId("tagCheque").style.display = "none";
        else
            dojo.byId("tagCheque").style.display = "block";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridMovimento';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_movimento", grid._by_idx[rowIndex].item.cd_movimento);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  style='height: 16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_movimento', 'selecionadoMovto', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_movimento', 'selecionadoMovto', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxItem(value, rowIndex, obj) {
    try {
        var gridName = 'gridItem';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosItem');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.id);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id', 'selecionadoItem', -1, 'selecionaTodosItem', 'selecionaTodosItem', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'id', 'selecionadoItem', " + rowIndex + ", '" + id + "', 'selecionaTodosItem', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxKit(value, rowIndex, obj) {
    var gridName = 'gridKit'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosKit');

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.cd_item);

        value = value || indice != null; // Item est� selecionado.
    }
    if (rowIndex != -1)
        icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'id', 'selecionadoKit', -1, 'selecionaTodosKit', 'selecionaTodosKit', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'id', 'selecionadoKit', " + rowIndex + ", '" + id + "', 'selecionaTodosKit', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxTit(value, rowIndex, obj) {
    try {
        var gridName = 'gridTitulo'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTit');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.id);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  style='height: 16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id', 'selecionadoTit', -1, 'selecionaTodosTit', 'selecionaTodosTit', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'id', 'selecionadoTit', " + rowIndex + ", '" + id + "', 'selecionaTodosTit', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxBaixa(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixa';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBaixa');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_baixa", grid._by_idx[rowIndex].item.cd_turma);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_desconto', 'selecionadoBaixa', -1, 'selecionaTodosBaixa', 'selecionaTodosBaixa', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_baixa', 'selecionadoBaixa', " + rowIndex + ", '" + id + "', 'selecionaTodosBaixa', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try {
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("fkPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabMovto(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTitulo') {
            if (!dijit.byId("formCadMovimento").validate()) {
                setarTabCadMovimento();
                return false;
            }
            if (!hasValue(dijit.byId("gridTitulo"))) {
                showCarregando();
                criarGradeTitulos(null);
                if (!dijit.byId("ckNotaFiscal").checked || dijit.byId("statusNFS").value == 2 || (dijit.byId("ckNotaFiscal").checked && TIPOMOVIMENTO != ENTRADA)){
                    findComponentesTagTitulos();
                } else {
                    showCarregando();
                }

            } else
                if (gerar_titulo) {
                    if (!dijit.byId("ckNotaFiscal").checked || dijit.byId("statusNFS").value == 2 || (dijit.byId("ckNotaFiscal").checked && TIPOMOVIMENTO != ENTRADA)) {
                        showCarregando();
                        criarAtualizarTitulo(null);
                    }
                }
            var gridTitulo = dojo.byId('gridTitulo');
            gridTitulo.style.height = '310px';
            dijit.byId('PaiGridTitulo').resize();
            gridTitulo.style.width = '1500px';
            dijit.byId("gridTitulo").update();
            dijit.byId("gridBaixa").update();
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabFiscal')
            habilitaCamposFiscal();
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Configuração das FK's
function chamarPesquisaItemFK(tipoPesquisa, xhr, Memory, FilteringSelect, array, ready, kit) {
    try {
        CONSULTA_ITEM = false;
        if (!kit)
            convertDialogItemKit("Pesquisar Item", true, false, false);

        if (tipoPesquisa == PESQUISA)
            abrirItemFK(xhr, Memory, FilteringSelect, array, kit);
        else
            abrirItemFKCadastro(xhr, ready, Memory, array, SETAR_TIPO);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKMovimento(isPesquisa) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = null;
                if (CONSULTA_PESSOA) {
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getPessoaMovimentoSearch?nome=" + encodeURIComponent(dojo.byId("_nomePessoaFK").value) +
                                "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                "&sexo=" + dijit.byId("sexoPessoaFK").value + "&tipoMovimento=" + TIPOMOVIMENTO,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));
                }
                CONSULTA_PESSOA = true;
                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.noDataMessage = msgNotRegEnc;
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function abrirPessoaFK(isPesquisa) {
    try {
        limparPesquisaPessoaFK();
        if (isPesquisa) pesquisaPessoaFKMovimento(isPesquisa);
        else {
            //Não mais necessário pois a consulta inicial não vai ser executada.
            //dojo.byId("_nomePessoaFK").value = 'a';
            //dijit.byId("inicioPessoaFK").set('checked', true);
            pesquisaPessoaCadFK();
        }
        dijit.byId("fkPessoaPesq").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            if (!dijit.byId("cadMovimento").open) {
                $("#cdPessoaPesq").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                dijit.byId("noPessoaPesq").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                dijit.byId("limparPessoaRelPosPes").set("disabled", false);

            } else {
                if (hasValue(origem) && origem == MOVIMENTO_DEVOLUCAO) {
                    $("#cdPessoaPesqFK").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                    dijit.byId("noPessoaPesqFK").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                    dijit.byId("limparPessoaRelPosPesFK").set("disabled", false);
                }
                else {
                    $("#cdPessoaMvtoCad").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                    dijit.byId("noPessoaMovto").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                    dijit.byId('limparPessoaFKMovimento').set("disabled", false);
                    if ((TIPOMOVIMENTO == SAIDA || TIPOMOVIMENTO == SERVICO) && hasValue(gridPessoaSelec.itensSelecionados[0].existeAluno) && gridPessoaSelec.itensSelecionados[0].existeAluno) {
                        dojo.byId("cdAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].cd_aluno;
                        dojo.byId("cdPessoaAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                        dojo.byId("noAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].no_pessoa;
                        dijit.byId('limparAlunoFKMovimento').set("disabled", false);

                        var parametros = getParamterosURL();
                        if (hasValue(parametros['id_material_didatico']) &&
                            eval(parametros['id_material_didatico']) == 1) {

                            getContratosSemTurmaByAluno();

                        }
                    }
                    gerar_titulo = true;
                    if (dijit.byId("ckNotaFiscal").checked)
                        verificaOperacaoCFOP();
                }
            }
        }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemEstoqueFK() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            var tipoItemInt = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : null;
            var id_natureza = 0;
            if (TIPOMOVIMENTO == DEVOLUCAO || TIPOMOVIMENTO == SERVICO)
                id_natureza = dojo.byId("id_natureza_movto").value;
            //if (CONSULTA_ITEM) {
                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/fiscal/getItemMovimentoSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" +
                            retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItemInt + "&grupoItem=" + grupoItem + "&id_tipo_movto=" + TIPOMOVIMENTO +
                            "&comEstoque=" + document.getElementById("comEstoque").checked + "&id_natureza_TPNF=" + parseInt(id_natureza) + "&kit=" + document.getElementById("kit").checked,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_item"
                    }
                    ), Memory({ idProperty: "cd_item" }));
                dataStore = new ObjectStore({ objectStore: myStore });
            //}
            //else
            //    dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) })
            //CONSULTA_ITEM = true;
            var grid = dijit.byId("gridPesquisaItem");
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarItemEstoqueFKCadastro(tipoPesquisa) {
    require([
          "dojo/_base/xhr",
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (xhr, JsonRest, ObjectStore, Cache, Memory) {
        try {
            var id_natureza = 0;
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            var tipoItemInt = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : null;

            var filtroVinculadoCurso = "";

            var parametrosTela = getParamterosURL();
            if (dijit.byId("tipo") != null && dijit.byId("tipo") != undefined && ((TIPOMOVIMENTO == SAIDA && (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) &&
                eval(parametrosTela['id_material_didatico']) == 1))))) {
                if (SETAR_TIPO) {
                    tipoItemInt = MATERIAL_DIDATICO;
                    SETAR_TIPO = false;
                }
                if (dijit.byId("vinculadoCurso") != null &&
                    dijit.byId("vinculadoCurso") != undefined &&
                    hasValue(dojo.byId("noCursoFKMovimento").value)) {
                    filtroVinculadoCurso = "&vinculado_curso=" + dijit.byId("vinculadoCurso").checked + "&cd_curso_material_didatico=" + dojo.byId("cdCursoFKMovimento").value;
                }
                    
            }

            var grid = dijit.byId("gridPesquisaItem");
            grid.itensSelecionados = [];
            if (TIPOMOVIMENTO == DEVOLUCAO || TIPOMOVIMENTO == SERVICO)
                id_natureza = dojo.byId("id_natureza_movto").value;
            //if (CONSULTA_ITEM) {
                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/fiscal/getItemCadastroMovimentoSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked +
                            "&status=" + retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItemInt + "&grupoItem=" + grupoItem + "&id_tipo_movto=" + TIPOMOVIMENTO +
                            "&comEstoque=" + document.getElementById("comEstoque").checked + "&id_natureza_TPNF=" + parseInt(id_natureza) + "&cd_movimento=" + dojo.byId("cd_nfDev").value + filtroVinculadoCurso,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_item"
                    }
                    ), Memory({ idProperty: "cd_item" }));
                dataStore = new ObjectStore({ objectStore: myStore });
            //}
            //else
            //    dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) })
            //CONSULTA_ITEM = true;
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirItemFK(xhr, Memory, FilteringSelect, array, kit) {
    try {
        populaTipoItem("tipo", xhr, Memory, array, TIPOMOVIMENTO, null, kit);
        pesquisarItemEstoqueFK();
        if (TIPOMOVIMENTO != DESPESAS && TIPOMOVIMENTO != SERVICO) {
            showP('comEstoqueTitulo', true);
            showP('comEstoqueCampo', true);
        }
        dijit.byId("tipo").set("disabled", false);
        dijit.byId("fkItem").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFKCadastro(xhr, ready, Memory, array, popularTipoItem) {
    try {
        if (popularTipoItem)
            populaTipoItem("tipo", xhr, Memory, array, TIPOMOVIMENTO);
        //populaTipoItemMovimento(xhr, ready, Memory, FilteringSelect)
        pesquisarItemEstoqueFKCadastro(CADASTRO);
        if (TIPOMOVIMENTO != DESPESAS && TIPOMOVIMENTO != SERVICO) {
            showP('comEstoqueTitulo', true);
            showP('comEstoqueCampo', true);
        }
        dijit.byId("tipo").set("disabled", false);
        //dijit.byId("tipo").reset();
        dijit.byId("fkItem").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaItemRetornoMaterialDidatico(item)
{
    try {
        var retorno = { valid: true, message: '' };


        if ((hasValue(item) && (item.id_material_didatico == true))) {
            

            if ((item.cd_curso <= 0) || (item.cd_curso > 0 && item.cd_curso != dojo.byId("cdCursoFKMovimento").value)) {

                retorno = { valid: false, message: msgErroCursoNaoAssociaoCursoItem };
            }
           
        }
        if ((hasValue(item) && (item.id_voucher_carga == true))) {
            retorno = { valid: false, message: "Este item não pode ser selecionado para a venda diretamente. Gera a nota na opção Coordenação - Alunos com Desistencia de Carga Horária" };
        }

         // Não vai poder incluir material didático
        if (item["cd_tipo_item"] != null &&
            item["cd_tipo_item"] != undefined &&
            item["id_material_didatico"]) {
            retorno = { valid: false, message: msgErroMaisDeUmItemMaterialDidaticoSelecionado };
        }

        return retorno;
    } catch (e) {
        postGerarLog(e);
    } 

}

function validaItemRetornoSaidaGrupoMaterialDidatico(item) {
    try {
        var retorno = { valid: true, message: '' };

        if ((hasValue(item) && (item.id_material_didatico == true)))
        {
            retorno = { valid: false, message: "Este item só pode ser selecionado na tela de Venda de Material Didático." };
        }

        if ((hasValue(item) && (item.id_voucher_carga == true))) {
            retorno = { valid: false, message: "Este item não pode ser selecionado para a venda diretamente. Gera a nota na opção Coordenação - Alunos com Desistencia de Carga Horária" };
        }

        return retorno;
    } catch (e) {
        postGerarLog(e);
    }

}


function validaEntradaMasterMaterialDidatico(item)
{
    try {
        var retorno = { valid: true, message: '' };

        if ((hasValue(item) && (item.id_voucher_carga == true) && eval(MasterGeral()) == false)) {
            retorno = { valid: false, message: "Este item somente pode ser selecionado por usuários Administradores do sistema" };
        }

        if (item.cd_tipo_item == MATERIAL_DIDATICO && (item.id_material_didatico == true) && eval(MasterGeral()) == false) {
            
            retorno = { valid: false, message: msgErroMasterCrudItemMaterialDidatico };
        }

        return retorno;
    } catch (e) {
        postGerarLog(e);
    }

}


function validateItensGreaterThanOneMaterialDidatico() {
    try {
        var retorno_qtd = 0;
        if (hasValue(dijit.byId("gridItem")) && hasValue(dijit.byId("gridItem").store.objectStore.data)) {
            retorno_qtd = dijit.byId("gridItem").store.objectStore.data.reduce(function (n, x) {
                return n +
                ((x["cd_tipo_item"] != null &&
                    x["cd_tipo_item"] != undefined &&
                        x["cd_tipo_item"] == MATERIAL_DIDATICO) &&
                 (x["id_material_didatico"] == true)
                )
            }, 0);
        }

        return retorno_qtd;
    } catch (e) {
        postGerarLog(e);
    } 
}

function retornarItemFK() {
    try {
        var parametrosTela = getParamterosURL();
        
        var gridPesquisaItem = dijit.byId("gridPesquisaItem");
        if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesquisaItem.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {

            if (!dijit.byId("cadMovimento").open) {
                dojo.byId("cdItem").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                dojo.byId("noItemPesq").value = gridPesquisaItem.itensSelecionados[0].no_item;
                dijit.byId("limparItem").set("disabled", false);
            } else {

                if (TIPOMOVIMENTO == SAIDA &&
                    ((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) &&
                    validaItemRetornoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValida = validaItemRetornoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValida.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValida.message, null);
                    }
                } else if (TIPOMOVIMENTO == ENTRADA && validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValidaCompra = validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValidaCompra.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                    }
                } else if (TIPOMOVIMENTO == DEVOLUCAO && dojo.byId('id_natureza_movto').value == ENTRADA && validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValidaCompra = validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValidaCompra.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                    }
                } else if (TIPOMOVIMENTO == DESPESAS && validaItemRetornoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValidaCompra = validaItemRetornoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValidaCompra.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                    }
                } else if (TIPOMOVIMENTO == SAIDA &&
                    !((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) &&
                    validaItemRetornoSaidaGrupoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValidaGrupo = validaItemRetornoSaidaGrupoMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValidaGrupo.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaGrupo.message, null);
                    }
                } else if (TIPOMOVIMENTO == SERVICO &&
                    ((hasValue(gridPesquisaItem.itensSelecionados[0]) && (gridPesquisaItem.itensSelecionados[0].id_voucher_carga == true))) &&
                    validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]).valid == false) {
                    var retornoValidaGrupo = validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[0]);
                    if (retornoValidaGrupo.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaGrupo.message, null);
                    }
                } else
                    if (TIPOMOVIMENTO == SAIDA && dojo.byId('divKit').style.display == "block") {
                        populaGridKitNota(gridPesquisaItem.itensSelecionados);
                    } else if (hasValue(origem) && origem == MOVIMENTO_DEVOLUCAO) {
                        dojo.byId("cdItemFK").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                        dojo.byId("noItemPesqFK").value = gridPesquisaItem.itensSelecionados[0].no_item;
                        dijit.byId("limparItemFK").set("disabled", false);
                    }
                    else {
                        var vlUnitario = dijit.byId("vlUnitario");
                        var vlTotal = dijit.byId("vlTotalMovimento");
                        var vlLiquido = dijit.byId("vlLiquido");

                        vlUnitario._onChangeActive = false;
                        vlTotal._onChangeActive = false;
                        vlLiquido._onChangeActive = false;
                        // grade de itens
                        dojo.byId("cd_item").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                        dojo.byId("desc_item").value = gridPesquisaItem.itensSelecionados[0].no_item;

                        var tipo = eval(getParamterosURL()['tipo']);
                        if (tipo == DEVOLUCAO)
                            tipo = dojo.byId('id_natureza_movto').value;

                        if (tipo != ENTRADA) {
                            vlUnitario.set("value", gridPesquisaItem.itensSelecionados[0].vl_item);
                            vlUnitario.value = gridPesquisaItem.itensSelecionados[0].vl_item;
                            vlUnitario.oldValue = gridPesquisaItem.itensSelecionados[0].vl_item;

                            vlTotal.set("value", dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item);
                            vlTotal.value = dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item;
                            vlTotal.oldValue = dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item;

                            vlLiquido.set("value", dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item);
                            vlLiquido.value = dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item;
                            vlLiquido.oldValue = dijit.byId('qtd_item').get('value') * gridPesquisaItem.itensSelecionados[0].vl_item;

                        }
                        if (hasValue(gridPesquisaItem.itensSelecionados[0].cd_plano_conta) && gridPesquisaItem.itensSelecionados[0].cd_plano_conta > 0) {
                            dojo.byId("cd_plano_contas").value = gridPesquisaItem.itensSelecionados[0].cd_plano_conta;
                            dojo.byId("descPlanoConta").value = gridPesquisaItem.itensSelecionados[0].desc_plano_conta;
                            dijit.byId("cadPlanoConta").set("disabled", true);
                            plano_conta_automatico = true;
                        }
                        else
                            plano_conta_automatico = false;
                        vlUnitario._onChangeActive = true;
                        vlTotal._onChangeActive = true;
                        vlLiquido._onChangeActive = true;
                        if (dijit.byId("ckNotaFiscal").checked)
                            switch (TIPOMOVIMENTO) {
                                case ENTRADA:
                                case SAIDA:
                                case DEVOLUCAO:
                                    dijit.byId("baseCalcPISItem").set("value", vlLiquido.value);
                                    dijit.byId("baseCalcCOFINSItem").set("value", vlLiquido.value);
                                    dijit.byId("baseCalcIPIItem").set("value", vlLiquido.value);
                                    var reducao = dijit.byId('tpNf').reducao;
                                    if (hasValue(reducao) && reducao > 0) {
                                        var baseReduzida = vlLiquido.value - ((vlLiquido.value * reducao) / 100);
                                        dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                    }
                                    else {
                                        dijit.byId("baseCalcICMSItem").set("value", vlLiquido.value);
                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                    }
                                    if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                        dijit.byId("baseCalcICMSItem").set("value", 0);
                                    }
                                    situacaoTributariaItem(gridPesquisaItem.itensSelecionados[0].cd_grupo_estoque);
                                    break;
                                case SERVICO:
                                    var baseISS = dijit.byId("baseCalcISSItem");
                                    baseISS.set("value", vlLiquido.value);
                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                        var vlAprox = baseISS.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                    }
                                    break;
                            }


                    }
                if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                    var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                }

                if (TIPOMOVIMENTO == SAIDA && (parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) &&
                    eval(parametrosTela['id_material_didatico']) == 1) {

                    dojo.byId("cd_tipo_item_material_didatico").value = gridPesquisaItem.itensSelecionados[0].cd_tipo_item;
                    dojo.byId("cd_grupo_estoque").value = gridPesquisaItem.itensSelecionados[0].cd_grupo_estoque;
                    dojo.byId("id_material_didatico").value = gridPesquisaItem.itensSelecionados[0].id_material_didatico;
                }
                if (TIPOMOVIMENTO == SERVICO || TIPOMOVIMENTO == DESPESAS) {
                    dojo.byId("id_voucher_carga").value = gridPesquisaItem.itensSelecionados[0].id_voucher_carga;
                }
            }

        }
        limparPesquisaCursoFK(false);
        dijit.byId("fkItem").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function situacaoTributariaItem(cd_grupo_estoque) {
    try {
        var cd_sit_trib_ICMS_tp_nt = 0;
        var cd_tipo_nf = hasValue(dojo.byId("cd_tp_nf").value) ? dojo.byId("cd_tp_nf").value : 0;
        if (cd_tipo_nf > 0)
            cd_sit_trib_ICMS_tp_nt = hasValue(dojo.byId("cd_sit_trib_ICMS_tp_nt").value) ? dojo.byId("cd_sit_trib_ICMS_tp_nt").value : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getSituacaoTributariaItem?cd_grupo_estoque=" + cd_grupo_estoque + "&id_regime_tributario=" + regime_tributario + "&cdSitTrib=" + cd_sit_trib_ICMS_tp_nt,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                //if (hasValue(pFuncao))
                //    pFuncao.call();
                var sitTribGrupItem = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemItem", null);

                if (hasValue(sitTribGrupItem)) {

                    dijit.byId("sitTribItem").set("value", sitTribGrupItem.cd_situacao_tributaria);

                    //dijit.byId("baseCalcICMSItem").set("value", 0);
                    //dijit.byId("aliquotaICMSItem").set("value", 0);
                    //dijit.byId("aliquotaICMSItem").set("disabled", true);
                    //dijit.byId("valorICMSItem").set("value", 0);
                    if (hasValue(dijit.byId("aliquotaICMSItem").old_value))
                        dijit.byId("aliquotaICMSItem").valueFixo = dijit.byId("aliquotaICMSItem").old_value;
                    if (hasValue(dijit.byId("baseCalcICMSItem").value))
                        dijit.byId("baseCalcICMSItem").valueFixo = dijit.byId("baseCalcICMSItem").value;

                    if (sitTribGrupItem.id_forma_tributacao == ISENTO) {
                        dijit.byId("baseCalcICMSItem").set("value", 0);
                        dijit.byId("baseCalcICMSItem").old_value = 0;
                        dijit.byId("baseCalcICMSItem").set("disabled", true);
                        dijit.byId("aliquotaICMSItem").set("value", 0);
                        dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                        dijit.byId("aliquotaICMSItem").set("disabled", true);
                        dijit.byId("valorICMSItem").set("value", 0);
                        dijit.byId("valorICMSItem").set("disabled", true);
                    }
                    if (sitTribGrupItem.id_forma_tributacao == REDUZIDO) {
                        var baseICMS = dijit.byId("baseCalcICMSItem").value;
                        var valorReduzido = (baseICMS * dijit.byId('tpNf').reducao) / 100;
                        baseICMS = baseICMS - valorReduzido;
                        dijit.byId("baseCalcICMSItem").set("value", baseICMS);
                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                        if (dijit.byId("aliquotaICMSItem").value <= 0) {
                            dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                            dijit.byId("baseCalcICMSItem").set("value", 0);
                        }
                        dijit.byId("aliquotaICMSItem").set("disabled", false);
                        dijit.byId("valorICMSItem").set("disabled", false);
                    }
                }
                else {
                    dijit.byId("sitTribItem").set("value", cd_sit_trib_ICMS_tp_nt);
                    dijit.byId("aliquotaICMSItem").set("disabled", false);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemItem", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function tiposItemByCadastroEConsultaMovimento(tipoPesquisa) {
    try {
        var tiposItem = new Array();
        if (tipoPesquisa == CADASTRO) {
            if (TIPOMOVIMENTO != SAIDA) {
                tiposItem.push({ cd_tipo_item: TIPOITEMSERVICO });
                tiposItem.push({ cd_tipo_item: APLICACAODIRETA });
            }
        }
        return tiposItem;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCadMovimento() {
    try {
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabPrincipalMvto");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function setarTabCadFiscal() {
    try {
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabFiscal");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region funções para Fks

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try {
        if (load)
            loadPlanoContasMovimento(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKPlanoContasCadastro(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try {
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);
        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPlanoContasMovimento(ObjectStore, Memory, xhr, tipoRetorno) {
    dojo.byId('tipoRetorno').value = tipoRetorno;
    xhr.get({
        url: Endereco() + "/api/financeiro/getPlanoContasWithMovimento?tipoMovimento=" + TIPOMOVIMENTO,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (treeData) {
        try {
            var gridPesquisaPlanoContas = dijit.byId("gridPesquisaPlanoContas");
            //Cria a grid de Plano de Contas:
            var visionData = mudaEstrutura(treeData.retorno);
            var data_plano = {
                identifier: 'id',
                label: 'name',
                items: visionData
            };
            var store = new dojo.data.ItemFileWriteStore({ data: data_plano });
            gridPesquisaPlanoContas.setStore(store);
            gridPesquisaPlanoContas.itensSelecionados = new Array();

            if ((!hasValue(treeData) || treeData.retorno.length <= 0)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Nenhum registro Encontrado");
                apresentaMensagem("apresentadorMensagemPlanoContasFK", mensagensWeb);
                dojo.byId('apresentadorMensagemPlanoContasFK').style.display = "";
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    }, function (error) {
        apresentaMensagem("apresentadorMensagemPlanoContasFK", error);
    });
}

function abrirPoliticaComercialFK() {
    try {
        limparPesquisaPoliticaComercialFK();
        pesquisaPolicaComercialFK(true);
        dijit.byId("fkPoliticaComercial").show();
        apresentaMensagem('apresentadorMensagemPoliticaComercialFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarEventoSelecionaPolitica() {
    try {
        dijit.byId("selecionaPolicaComercialFK").on("click", function (e) {
            if (hasValue(dijit.byId("gridPolicaComercialFK")) && (dijit.byId("gridPolicaComercialFK")._by_idx.length > 0)) {
                var gridPolicaComercialFK = dijit.byId("gridPolicaComercialFK");
                if (gridPolicaComercialFK.itensSelecionados != null && gridPolicaComercialFK.itensSelecionados.length > 1) {
                    caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    return false;
                } else {
                    if (gridPolicaComercialFK.itensSelecionados == null || gridPolicaComercialFK.itensSelecionados.length <= 0) {
                        caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
                        return false
                    }
                }
            }
            gerar_titulo = true;
            dojo.byId("noPoliticaCom").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].dc_politica_comercial;
            dojo.byId("cdPoliticaComercial").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].cd_politica_comercial;
            dijit.byId("fkPoliticaComercial").hide();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaCadFK() {
    require([
     "dojo/store/JsonRest",
     "dojo/data/ObjectStore",
     "dojo/store/Cache",
     "dojo/store/Memory",
     "dojo/ready",
     "dojo/domReady!",
     "dojo/parser"],
 function (JsonRest, ObjectStore, Cache, Memory, ready) {
     ready(function () {
         try {
             var cd_pessoa_aluno = hasValue(dojo.byId("cdPessoaAlunoFKMovimento").value) ? dojo.byId("cdPessoaAlunoFKMovimento").value : 0;
             if (TIPOMOVIMENTO == SAIDA && cd_pessoa_aluno > 0) {
                 var myStore = null;
                 if (CONSULTA_PESSOA) {
                     var myStore = dojo.store.Cache(
                         dojo.store.JsonRest({
                             target: Endereco() + "/api/Pessoa/GetPessoaResponsavelSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                 "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                 "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                 "&cnpjCpf=" + dojo.byId("CnpjCpf").value + "&cdPai=" + cd_pessoa_aluno +
                                 "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=0",
                             handleAs: "json",
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), dojo.store.Memory({}));
                     dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
                 }
                 else
                    dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) })
                 CONSULTA_PESSOA = true;
                 var grid = dijit.byId("gridPesquisaPessoa");
                 grid.noDataMessage = msgNotRegEnc;
                 grid.setStore(dataStore);
             } else {
                 var myStore = null;
                 if (CONSULTA_PESSOA) {
                     var myStore = Cache(
                         JsonRest({
                             target: Endereco() + "/api/pessoa/getTdsPessoaSearchEscolaCadMovimento?nome=" + encodeURIComponent(dojo.byId("_nomePessoaFK").value) +
                                 "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                 "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                 "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                 "&sexo=" + dijit.byId("sexoPessoaFK").value,
                             handleAs: "json",
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), Memory({}));

                     dataStore = new ObjectStore({ objectStore: myStore });
                 }
                 else
                     dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) })
                 CONSULTA_PESSOA = true;
                 var grid = dijit.byId("gridPesquisaPessoa");
                 grid.noDataMessage = msgNotRegEnc;
                 grid.setStore(dataStore);
             }
         }
         catch (e) {
             postGerarLog(e);
         }
     });
 });
}

//#endregion

//Calculos dos valores
function calcularEAplicarDesconto(pc_desconto, vl_desconto) {
    try {
        var compPcDesc = dijit.byId("pcDesconto");
        var compVlDesc = dijit.byId("vlDesconto");
        var compVlTotalItens = dijit.byId("totalItens");
        var compVlTotalGeral = dijit.byId("totalGeral");
        var gridItem = dijit.byId("gridItem");
        var totalLiquidoItens = 0;
        var vlDesc = 0;
        //alterãção no percentual de desconto.
        if (pc_desconto != null && vl_desconto == null) {
            //var vlDesc = 0;
            var data = hasValue(gridItem.store.objectStore.data) ? gridItem.store.objectStore.data : new Array();
            //Arrendondamento 2 casas
            //vlDesc = unmaskFixed((parseFloat(compVlTotalItens.value) * (parseFloat(pc_desconto) / 100)), 2);
            $.each(data, function (index, value) {
                //Arrendondamento 2 casas
                var vlDescItem = unmaskFixed((parseFloat(value.vl_total_item) * (parseFloat(pc_desconto) / 100)), 2);
                if (vlDesc == 0 && vlDescItem == 0)
                    value.vl_liquido_item = value.vl_total_item + value.vl_acrescimo_item;
                else
                    value.vl_liquido_item = unmaskFixed((value.vl_total_item + value.vl_acrescimo_item) - vlDescItem, 2);
                value.vlLiquidoItem = maskFixed(value.vl_liquido_item, 2);
                value.vl_desconto_item = vlDescItem;
                value.vlDescontoItem = maskFixed(vlDescItem, 2);

                totalLiquidoItens += value.vl_liquido_item;
                vlDesc += vlDescItem;
            });
            compVlDesc._onChangeActive = false;
            compVlDesc.set("value", vlDesc);
            compVlDesc.oldValue = vlDesc;
            compVlDesc._onChangeActive = true;
            compVlTotalGeral.set("value", totalLiquidoItens);
            compPcDesc.oldValue = pc_desconto;
        }
        //alterãção no valor de desconto.
        if (vl_desconto != null && pc_desconto == null) {
            pc_desconto = 0;
            //var vlDesc = 0;
            var data = hasValue(gridItem.store.objectStore.data) ? gridItem.store.objectStore.data : new Array();
            //Arrendondamento 2 casas
            pc_desconto = unmaskFixed(100 * (parseFloat(vl_desconto) / compVlTotalItens.value), 4);
            var provaReal = unmaskFixed((parseFloat(compVlTotalItens.value) * ((parseFloat(pc_desconto) / 100))), 2);
            $.each(data, function (index, value) {
                //Arrendondamento 2 casas
                var vlDescItem = unmaskFixed((parseFloat(value.vl_total_item) * (parseFloat(pc_desconto) / 100)), 2);
                if (vlDesc == 0 && pc_desconto == 0)
                    value.vl_liquido_item = value.vl_total_item + value.vl_acrescimo_item;
                else
                    value.vl_liquido_item = unmaskFixed((value.vl_total_item + value.vl_acrescimo_item) - vlDescItem, 2);
                value.vlLiquidoItem = maskFixed(value.vl_liquido_item, 2);
                value.vl_desconto_item = vlDescItem;
                value.vlDescontoItem = maskFixed(vlDescItem, 2);
                totalLiquidoItens += value.vl_liquido_item;
            });
            compPcDesc._onChangeActive = false;
            compPcDesc.set("value", pc_desconto);
            compPcDesc.oldValue = pc_desconto;
            compPcDesc._onChangeActive = true;
            compVlTotalGeral.set("value", totalLiquidoItens);
            compVlDesc.oldValue = vl_desconto;
        }
        gridItem.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Calculos dos valores Desconto Item
function calcularEAplicarDescontoItem(pc_desconto, vl_desconto) {
    try {
        var compPcDesc = dijit.byId("perDescontoItem");
        var compVlDesc = dijit.byId("valDescontoItem");
        var compVlTotalGeral = dijit.byId("vlLiquido");
        var compVlTotalItens = dijit.byId("vlTotalMovimento");
        var gridItem = dijit.byId("gridItem");
        var totalLiquidoItens = 0;

        var vlDesc = 0;
        //alterãção no percentual de desconto.
        if (pc_desconto != null && vl_desconto == null) {
            //var vlDesc = 0;
            //Arrendondamento 2 casas
            var vlDescItem = unmaskFixed((parseFloat(dojo.byId("vlTotalMovimento").value.replace(",", ".")) * (parseFloat(pc_desconto) / 100)), 2);
            if (vlDesc == 0 && vlDescItem == 0)
                compVlTotalGeral.set("value", dojo.byId("vlTotalMovimento").value);
            else
                compVlTotalGeral.set("value", unmaskFixed(compVlTotalItens.value - vlDescItem, 2));
            compVlDescm = vlDescItem;

            compVlDesc._onChangeActive = false;
            compVlDesc.set("value", vlDescItem);

            compVlDesc._onChangeActive = true;
            compVlTotalGeral.set("value", compVlTotalGeral.value);
            compPcDesc.oldValue = pc_desconto;
        }
        //alterãção no valor de desconto.
        if (vl_desconto != null && pc_desconto == null) {
            pc_desconto = 0;
            //var vlDesc = 0;
            //Arrendondamento 2 casas
            pc_desconto = unmaskFixed(100 * (parseFloat(vl_desconto) / compVlTotalItens.value), 4);
            var provaReal = unmaskFixed((parseFloat(compVlTotalItens.value) * ((parseFloat(pc_desconto) / 100))), 2);
            //Arrendondamento 2 casas
            var vlDescItem = unmaskFixed((parseFloat(dojo.byId("vlTotalMovimento").value) * (parseFloat(pc_desconto) / 100)), 2);
            if (vlDesc == 0 && pc_desconto == 0)
                compVlTotalGeral.value = dojo.byId("vlTotalMovimento").value;
            else
                compVlTotalGeral.value = unmaskFixed(dijit.byId("vlTotalMovimento").value - vlDescItem, 2);
            compPcDesc._onChangeActive = false;
            compPcDesc.set("value", pc_desconto);
            compPcDesc.oldValue = pc_desconto;
            compPcDesc._onChangeActive = true;
            compVlTotalGeral.set("value", compVlTotalGeral.value);

        }
        if (dijit.byId("ckNotaFiscal").checked) {
            switch (TIPOMOVIMENTO) {
                case ENTRADA:
                case SAIDA:
                case DEVOLUCAO:
                    dijit.byId("baseCalcPISItem").set("value", compVlTotalGeral.value);
                    dijit.byId("baseCalcCOFINSItem").set("value", compVlTotalGeral.value);
                    dijit.byId("baseCalcIPIItem").set("value", compVlTotalGeral.value);
                    var reducao = dijit.byId('tpNf').reducao;
                    if (hasValue(reducao) && reducao > 0) {
                        var baseReduzida = compVlTotalGeral.value - ((compVlTotalGeral.value * reducao) / 100);
                        dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                    }
                    else {
                        dijit.byId("baseCalcICMSItem").set("value", compVlTotalGeral.value);
                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                    }
                    if (dijit.byId("aliquotaICMSItem").value <= 0) {
                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                        dijit.byId("baseCalcICMSItem").set("value", 0);
                    }
                    break;
                case SERVICO:
                    dijit.byId("baseCalcISSItem").set("value", compVlTotalGeral.value);
                    break;
            }
            if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                var vlAprox = compVlTotalGeral * dijit.byId("pc_aliquota_ap_item").value / 100;
                dijit.byId("vl_aproximado_item").set("value", vlAprox);
            }
        }

        gridItem.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function calcularEAplicarAcrescimo(pc_acrescimo, vl_acrescimo) {
    try {
        var compPcAcresc = dijit.byId("pcAcrec");
        var compVlAcresc = dijit.byId("vlAc");
        var compVlTotalItens = dijit.byId("totalItens");
        var compVlTotalGeral = dijit.byId("totalGeral");
        var gridItem = dijit.byId("gridItem");
        var totalLiquidoItens = 0;
        var totalGeralIens = 0;
        var vlAcresc = 0;
        //alterãção no percentual de desconto.
        if (pc_acrescimo != null && vl_acrescimo == null) {
            var data = hasValue(gridItem.store.objectStore.data) ? gridItem.store.objectStore.data : new Array();
            //Arrendondamento 2 casas
            //vlAcresc = unmaskFixed((parseFloat(compVlTotalItens.value) * (parseFloat(pc_acrescimo) / 100)), 2);
            $.each(data, function (index, value) {
                //Arrendondamento 2 casas
                var vlAcrescItem = unmaskFixed((parseFloat(value.vl_total_item) * (parseFloat(pc_acrescimo) / 100)), 2);
                if (vlAcresc == 0 && vlAcrescItem == 0)
                    value.vl_liquido_item = value.vl_total_item - value.vl_desconto_item;
                else
                    value.vl_liquido_item = unmaskFixed((value.vl_total_item - value.vl_desconto_item) + vlAcrescItem, 2);
                value.vlLiquidoItem = maskFixed(value.vl_liquido_item, 2);
                value.vl_acrescimo_item = vlAcrescItem;
                compPcAcresc.oldValue = pc_acrescimo;
                totalLiquidoItens += value.vl_liquido_item;
                vlAcresc += vlAcrescItem;
            });
            compVlAcresc._onChangeActive = false;
            compVlAcresc.set("value", vlAcresc);
            compVlAcresc.oldValue = vlAcresc;
            compPcAcresc.oldValue = pc_acrescimo;
            compVlAcresc._onChangeActive = true;
            compVlTotalGeral.set("value", totalLiquidoItens);
        }
        //alterãção no valor de desconto.
        if (vl_acrescimo != null && pc_acrescimo == null) {
            pc_acrescimo = 0;
            //var vlDesc = 0;
            var data = hasValue(gridItem.store.objectStore.data) ? gridItem.store.objectStore.data : new Array();
            //Arrendondamento 2 casas
            pc_acrescimo = unmaskFixed(100 * (parseFloat(vl_acrescimo) / compVlTotalItens.value), 4);
            var provaReal = unmaskFixed((parseFloat(compVlTotalItens.value) * ((parseFloat(pc_acrescimo) / 100))), 2);
            $.each(data, function (index, value) {
                //Arrendondamento 2 casas
                var vlAcrescItem = unmaskFixed((parseFloat(value.vl_total_item) * (parseFloat(pc_acrescimo) / 100)), 2);
                if (vlAcresc == 0 && pc_acrescimo == 0)
                    value.vl_liquido_item = value.vl_total_item - value.vl_desconto_item;
                else
                    value.vl_liquido_item = unmaskFixed((value.vl_total_item - value.vl_desconto_item) + vlAcrescItem, 2);
                value.vlLiquidoItem = maskFixed(value.vl_liquido_item, 2);
                value.vl_acrescimo_item = vlAcrescItem;
                totalLiquidoItens += value.vl_liquido_item;
                compPcAcresc.oldValue = pc_acrescimo;
            });
            compPcAcresc._onChangeActive = false;
            compPcAcresc.set("value", pc_acrescimo);
            compPcAcresc.oldValue = pc_acrescimo;
            compPcAcresc._onChangeActive = true;
            compVlAcresc.oldValue = vl_acrescimo;
            compVlTotalGeral.set("value", totalLiquidoItens);
        }
        gridItem.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarValoresPricipais(tipoOperacao, item, oldItem) {
    try {
        var compTotalItens = dijit.byId("totalItens");
        var compTotalGeral = dijit.byId("totalGeral");
        var compVlDesc = dijit.byId("vlDesconto");
        var comPcDesc = dijit.byId("pcDesconto");

        if (hasValue(tipoOperacao)) {
            switch (tipoOperacao) {
                case 1://Novo
                    compTotalGeral.set("value", compTotalGeral.value += item.vl_liquido_item);
                    compTotalItens.set("value", compTotalItens.value += item.vl_total_item);
                    var percentual = (compVlDesc.value * 100) / compTotalItens.value;
                    comPcDesc._onChangeActive = false;
                    comPcDesc.set("value", percentual);
                    comPcDesc._onChangeActive = true;
                    break;
                case 2://Editar
                    voltarEaplicarAcrescOuDescItem(item, AlteraDesc);
                    break;
                case 3://Excluir
                    compTotalGeral.set("value", compTotalGeral.value -= item.vl_liquido_item);
                    compTotalItens.set("value", compTotalItens.value -= item.vl_total_item);
                    compVlDesc.set("value", compVlDesc.value -= item.vl_desconto_item);
                    var percentual = (compVlDesc.value * 100) / compTotalItens.value;
                    comPcDesc._onChangeActive = false;
                    comPcDesc.set("value", percentual);
                    comPcDesc._onChangeActive = true;
                    break;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function aplicarAcrescOuDescItemInsetGrade(item) {
    try {
        var compPcAcresc = dijit.byId("pcAcrec");
        var compVlAcresc = dijit.byId("vlAc");
        var compPcDesc = dijit.byId("pcDesconto");
        var compVlDesc = dijit.byId("vlDesconto");
        var gridItem = dijit.byId("gridItem");

        if (isNaN(compPcDesc.value))
            compPcDesc.value = 0;

        var vlAcrescItem = unmaskFixed((parseFloat(item.vl_total_item) * (parseFloat(compPcAcresc.value) / 100)), 2);
        var vlDescItem = (parseFloat(item.vl_total_item) * (parseFloat(compPcDesc.value) / 100));
        compVlDesc._onChangeActive = false;
        compVlDesc.set("value", parseFloat(compVlDesc.value) + vlDescItem);
        compVlDesc.oldValue = parseFloat(compVlDesc.value) + vlDescItem;
        compVlDesc._onChangeActive = true;

        compVlAcresc._onChangeActive = false;
        compVlAcresc.set("value", parseFloat(compVlAcresc.value) + vlAcrescItem);
        compVlAcresc.oldValue = parseFloat(compVlAcresc.value) + vlAcrescItem;
        compVlAcresc._onChangeActive = true;

        item.vl_liquido_item = (item.vl_total_item - vlDescItem) + vlAcrescItem;
        item.vlLiquidoItem = maskFixed(item.vl_liquido_item, 2);
        item.vl_desconto_item = vlDescItem;
        item.vlDescontoItem = unmaskFixed(vlDescItem, 2);
        item.vl_acrescimo_item = vlAcrescItem;
        return item;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function voltarEaplicarAcrescOuDescItem(itemNovo, alterarDesc) {
    try {
        var grid = dijit.byId("gridItem");
        var linkAnterior = document.getElementById('linkGridItem');
        if (hasValue(linkAnterior.value) && JSON.parse(linkAnterior.value))
            itemSelecionado = grid.itensSelecionados[0];
        else
            itemSelecionado = grid.selection.getSelected()[0];
        var compPcAcresc = dijit.byId("pcAcrec");
        var compVlAcresc = dijit.byId("vlAc");
        var compPcDesc = dijit.byId("pcDesconto");
        var compVlDesc = dijit.byId("vlDesconto");
        var compVlTotalItens = dijit.byId("totalItens");
        var compVlTotalGeral = dijit.byId("totalGeral");
        var vlAcresc = compVlAcresc.value;
        var vlDesc = compVlDesc.value;
        compVlDesc.oldValue = vlDesc;

        //Volta o item o valor antes de ser alterado.
        if (hasValue(itemSelecionado)) {
            compVlTotalItens.value -= itemSelecionado.vl_total_item;
            compVlTotalGeral.value -= itemSelecionado.vl_liquido_item;
            vlAcresc -= itemSelecionado.vl_acrescimo_item;
            if (AlteraDesc)
                vlDesc -= itemSelecionado.vl_desconto_item;
            itemSelecionado.vl_total_item = itemNovo.vl_total_item;
            itemSelecionado.vl_liquido_item = itemNovo.vl_liquido_item;
            itemSelecionado.qt_item_movimento = itemNovo.qt_item_movimento;
            itemSelecionado.vl_unitario_item = itemNovo.vl_unitario_item;
            itemSelecionado.vlUnitarioItem = maskFixed(itemNovo.vl_unitario_item, 2);
            itemSelecionado.vlTotalItem = maskFixed(itemNovo.vl_total_item, 2);
            itemSelecionado.vlLiquidoItem = maskFixed(itemNovo.vl_liquido_item, 2);
        }
        if (hasValue(itemNovo)) {
            var vlAcrescItem = unmaskFixed((parseFloat(itemNovo.vl_total_item) * (parseFloat(compPcAcresc.value) / 100)), 2);
            vlAcresc += vlAcrescItem;
            itemSelecionado.vl_acrescimo_item = vlAcrescItem;
            if (AlteraDesc) {
                var vlDescItem = (parseFloat(itemNovo.vl_total_item) * (parseFloat(compPcDesc.value) / 100));
                vlDesc += vlDescItem;
                itemSelecionado.vl_desconto_item = vlDescItem;
                itemSelecionado.vlDescontoItem = unmaskFixed(vlDescItem, 2);
            }
            //itemSelecionado.vl_liquido_item = (itemSelecionado.vl_total_item - vlDescItem) + vlAcrescItem;
        }

        compVlDesc._onChangeActive = false;
        compVlDesc.set("value", vlDesc);

        compVlDesc._onChangeActive = true;

        compVlAcresc._onChangeActive = false;
        compVlAcresc.set("value", vlAcresc);
        compVlAcresc.oldValue = vlAcresc;
        compVlAcresc._onChangeActive = true;
        compVlTotalItens.set("value", compVlTotalItens.value + itemSelecionado.vl_total_item);
        compVlTotalGeral.set("value", compVlTotalGeral.value + itemSelecionado.vl_liquido_item);

        var percentual = (vlDesc * 100) / compVlTotalItens.value;
        compPcDesc._onChangeActive = false;
        compPcDesc.set("value", percentual);
        compPcDesc._onChangeActive = true;
        return itemSelecionado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function calcularDescAcrescItemView(qtd_item, vl_total, vl_liquido, vl_unitario) {
    try {
        var cd_item_movimento = dojo.byId("cd_item_movimento");
        var itemSelecionadoGrid = dijit.byId("gridItem").selection.getSelected();
        var itemSelecionado = jQuery.extend(true, {}, itemSelecionadoGrid)[0];
        var compPcAcresc = dijit.byId("pcAcrec");
        var compPcDesc = dijit.byId("pcDesconto");
        if (hasValue(itemSelecionado)) {

            if (hasValue(cd_item_movimento.value) && cd_item_movimento.value <= 0)
                var itemSelecionado = new Object();
            if (vl_total != null) {
                itemSelecionado.vl_total_item = vl_total;
                itemSelecionado.vlTotalItem = maskFixed(vl_total, 2);
            }
            if (vl_liquido != null) {
                itemSelecionado.vl_liquido_item = vl_liquido;
                itemSelecionado.vlLiquidoItem = maskFixed(vl_liquido, 2);
            }
            if (qtd_item != null)
                itemSelecionado.qt_item_movimento = qtd_item;
            if (vl_unitario != null) {
                itemSelecionado.vl_unitario_item = vl_unitario;
                itemSelecionado.vlUnitarioItem = maskFixed(vl_unitario, 2);
            }
            var vlAcrescItem = unmaskFixed((parseFloat(vl_total) * (parseFloat(compPcAcresc.value) / 100)), 2)
            var vlDescItem = unmaskFixed((parseFloat(vl_total) * (parseFloat(compPcDesc.value) / 100)), 2);
            itemSelecionado.vl_liquido_item = (vl_total - vlDescItem) + vlAcrescItem;
            itemSelecionado.vlLiquidoItem = maskFixed(itemSelecionado.vl_liquido_item, 2);
        } else {
            if (hasValue(cd_item_movimento.value) && cd_item_movimento.value <= 0)
                itemSelecionado = new Object();
            if (vl_total != null) {
                itemSelecionado.vl_total_item = vl_total;
                itemSelecionado.vlTotalItem = maskFixed(vl_total, 2);
            }
            if (vl_liquido != null) {
                itemSelecionado.vl_liquido_item = vl_liquido;
                itemSelecionado.vlLiquidoItem = maskFixed(vl_liquido, 2);
            }
            if (qtd_item != null)
                itemSelecionado.qt_item_movimento = qtd_item;
            if (vl_unitario != null) {
                itemSelecionado.vl_unitario_item = vl_unitario;
                itemSelecionado.vlUnitarioItem = maskFixed(vl_unitario, 2);
            }
            var vlAcrescItem = unmaskFixed((parseFloat(vl_total) * (parseFloat(compPcAcresc.value) / 100)), 2)
            var vlDescItem = unmaskFixed((parseFloat(vl_total) * (parseFloat(compPcDesc.value) / 100)), 2);
            itemSelecionado.vl_liquido_item = (vl_total - vlDescItem) + vlAcrescItem;
            itemSelecionado.vlLiquidoItem = maskFixed(itemSelecionado.vl_liquido_item, 2);
        }
        return itemSelecionado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D Item

function limparItem() {
    try {
        var compVlUnit = dijit.byId('vlUnitario');
        var vlTotal = dijit.byId("vlTotalMovimento");
        var vlLiquido = dijit.byId('vlLiquido');
        apresentaMensagem("apresentadorMensagemItem", null);
        setarTabCadMovimento();
        dojo.byId("cd_item_movimento").value = 0;
        clearForm("formItem");
        dijit.byId("qtd_item")._onChangeActive = false;
        compVlUnit._onChangeActive = false;
        vlTotal._onChangeActive = false;
        vlLiquido._onChangeActive = false;
        dijit.byId("qtd_item").set('value', 1);
        compVlUnit.set('value', 0);
        compVlUnit.value = 0;
        compVlUnit.oldValue = 0;
        vlTotal.set('value', 0);
        vlTotal.value = 0;
        vlTotal.oldValue = 0;
        vlLiquido.set('value', 0);
        vlLiquido.value = 0;
        vlLiquido.oldValue = 0;
        dijit.byId("qtd_item")._onChangeActive = true;
        compVlUnit._onChangeActive = true;
        vlTotal._onChangeActive = true;
        vlLiquido._onChangeActive = true;
        dijit.byId("cadPlanoConta").set("disabled", false);
        dijit.byId("pcDesconto").value = 0;
        if (TIPOMOVIMENTO == SERVICO || TIPOMOVIMENTO == SAIDA) {
            limparTagFiscalItem();
            dijit.byId("tagFiscalItem").set("open", false);
            habilitarDesabilitarCamposEditItemNF(false);
        }
        switch (TIPOMOVIMENTO) {
            case SERVICO:
                dijit.byId("baseCalcISSItem").set("value", 0);
                dijit.byId("vl_aproximado_item").set("value", 0);
                dijit.byId("pc_aliquota_ap_item").set("value", 0);
                dijit.byId("aliquotaISSItem").set("value", 0);
                dijit.byId("valorISSItem").set("value", 0);
                break;
            case ENTRADA:
            case SAIDA:
            case DEVOLUCAO:
                dojo.byId("cd_CFOP_item").value = 0;
                dijit.byId("vl_aproximado_item").set("value", 0);
                dijit.byId("pc_aliquota_ap_item").set("value", 0);
                dijit.byId("descCFOPItem").reset();
                dijit.byId("sitTribItem").reset();
                if (regime_tributario == REGIME_NORMAL) {
                    dijit.byId("cbStTribPis").set("value", SITUACAOTRIBUTARIAPIS);
                    dijit.byId("cbStTribCof").set("value", SITUACAOTRIBUTARIACOFINS);
                }
                else {
                    dijit.byId("cbStTribPis").set("value", SITUACAOTRIBUTARIAPIS_OUTRASOP);
                    dijit.byId("cbStTribCof").set("value", SITUACAOTRIBUTARIACOFINS_OUTRASOP);
                }
                dijit.byId("baseCalcICMSItem").set("value", 0);
                dijit.byId("baseCalcICMSItem").old_value = 0;
                dijit.byId("aliquotaICMSItem").set("value", 0);
                dijit.byId("valorPISItem").set("value", 0);
                dijit.byId("baseCalcCOFINSItem").set("value", 0);
                dijit.byId("aliquotaCOFINSItem").set("value", 0);
                dijit.byId("valorCOFINSItem").set("value", 0);
                dijit.byId("baseCalcIPIItem").set("value", 0);
                dijit.byId("aliquotaIPIItem").set("value", 0);
                dijit.byId("valorIPIItem").set("value", 0);
                dijit.byId("baseCalcPISItem").set("value", 0);
                dijit.byId("valorICMSItem").set("value", 0);
                dijit.byId("aliquotaPISItem").set("value", 0);
                break;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemGrade(InsercaoKit) {
    try {
        if (!dijit.byId("formItem").validate() && !InsercaoKit) {
            return false;
        }
        if (dijit.byId("qtd_item").value == 0 && TIPOMOVIMENTO == SAIDA) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemZero, null);
            return false
        }
        if (dijit.byId("qtd_item").value > 1 && TIPOMOVIMENTO == SAIDA &&
            (dojo.byId("id_material_didatico").value == 'true')) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemmaiorque1, null);
            return false
        }
        if (dijit.byId("qtd_item").value > 1 && TIPOMOVIMENTO == SERVICO &&
            (dojo.byId("id_voucher_carga").value == 'true') && !eval(MasterGeral())) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemmaiorque1Voucher, null);
            return false
        }
        if (dijit.byId("qtd_item").value <= 1 && TIPOMOVIMENTO == SERVICO &&
            (dojo.byId("id_voucher_carga").value == 'true') &&
            (dojo.byId('serie').value != 'CEC' && !dijit.byId("ckNotaFiscal").checked)
        ) {
            caixaDialogo(DIALOGO_AVISO, 'Série para voucher tem que ser obrigatóriamente "CEC"', null);
            return false
        }
        var newItem = {
            id: geradorIdItem(dijit.byId("gridItem")),
            cd_item_movimento: 0,
            cd_movimento: 0,
            cd_item_kit: hasValue(dojo.byId("cd_item_kit").value) ? dojo.byId("cd_item_kit").value : null,
            cd_grupo_estoque: parseInt(dojo.byId("cd_grupo_estoque").value),
            cd_item: parseInt(dojo.byId("cd_item").value),
            cd_tipo_item: parseInt(dojo.byId("cd_tipo_item_material_didatico").value),
            dc_item_movimento: dojo.byId("desc_item").value,
            dc_plano_conta: dojo.byId("descPlanoConta").value,
            qt_item_movimento: dijit.byId("qtd_item").value,
            vl_unitario_item: dijit.byId("vlUnitario").value,
            vl_total_item: dijit.byId("vlTotalMovimento").value,
            vl_liquido_item: dijit.byId("vlLiquido").value,
            vl_acrescimo_item: 0,
            vl_desconto_item: dijit.byId("valDescontoItem").value,
            vlDescontoItem: maskFixed(dijit.byId("valDescontoItem").value, 2),
            pc_desconto_item: dijit.byId("perDescontoItem").value,
            pcDescontoItem: maskFixed(dijit.byId("perDescontoItem").value, 2),
            cd_plano_conta: dojo.byId("cd_plano_contas").value > 0 ? dojo.byId("cd_plano_contas").value : null,
            vlUnitarioItem: maskFixed(dijit.byId("vlUnitario").value, 2),
            vlTotalItem: maskFixed(dijit.byId("vlTotalMovimento").value, 2),
            vlLiquidoItem: maskFixed(dijit.byId("vlLiquido").value, 2),
            planoSugerido: plano_conta_automatico,
            id_nf_item: dijit.byId("ckNotaFiscal").checked,
            id_material_didatico: dojo.byId("id_material_didatico").value == 'true' ? true : false,
            id_voucher_carga: dojo.byId("id_voucher_carga").value == 'true' ? true : false
        }
        var dados = dijit.byId("gridItem").store.objectStore.data;
        var naoExisteItemGrid = dados.every(function (item) {
            return item.cd_item != newItem.cd_item
        })
        if (naoExisteItemGrid) {

            if (newItem != null && dijit.byId("ckNotaFiscal").checked) {
                switch (TIPOMOVIMENTO) {
                    case SERVICO:
                        newItem.cd_cfop = dojo.byId("cd_CFOP_item").value;
                        newItem.dc_cfop = dijit.byId("operacaoCFOPItem").value;
                        newItem.nm_cfop = dijit.byId("descCFOPItem").value;
                        newItem.vl_base_calculo_ISS_item = hasValue(dijit.byId("baseCalcISSItem").value) ? unmaskFixed(dijit.byId("baseCalcISSItem").value, 2) : 0;
                        newItem.pc_aliquota_ISS = hasValue(dijit.byId("aliquotaISSItem").value) ? dijit.byId("aliquotaISSItem").value : 0;
                        newItem.vl_ISS_item = hasValue(dijit.byId("valorISSItem").value) ? unmaskFixed(dijit.byId("valorISSItem").value, 2) : 0;
                        break;
                    case ENTRADA:
                    case SAIDA:
                    case DEVOLUCAO:
                        newItem.cd_cfop = hasValue(dojo.byId("cd_CFOP_item").value) ? dojo.byId("cd_CFOP_item").value : 0;
                        newItem.dc_cfop = dojo.byId("operacaoCFOPItem").value;
                        newItem.nm_cfop = dijit.byId("descCFOPItem").value;
                        newItem.cd_situacao_tributaria_ICMS = hasValue(dijit.byId("sitTribItem").value) ? dijit.byId("sitTribItem").value : null;
                        newItem.cd_situacao_tributaria_PIS = hasValue(dijit.byId("cbStTribPis").value) ? dijit.byId("cbStTribPis").value : null;
                        newItem.cd_situacao_tributaria_COFINS = hasValue(dijit.byId("cbStTribCof").value) ? dijit.byId("cbStTribCof").value : null;
                        newItem.vl_base_calculo_ICMS_item = hasValue(dijit.byId("baseCalcICMSItem").value) ? unmaskFixed(dijit.byId("baseCalcICMSItem").value, 2) : 0;
                        newItem.vl_base_calculo_COFINS_item = hasValue(dijit.byId("baseCalcCOFINSItem").value) ? unmaskFixed(dijit.byId("baseCalcCOFINSItem").value, 2) : 0;
                        newItem.vl_base_calculo_PIS_item = hasValue(dijit.byId("baseCalcPISItem").value) ? unmaskFixed(dijit.byId("baseCalcPISItem").value, 2) : 0;
                        newItem.vl_base_calculo_IPI_item = hasValue(dijit.byId("baseCalcIPIItem").value) ? unmaskFixed(dijit.byId("baseCalcIPIItem").value, 2) : 0;
                        newItem.vl_ICMS_item = hasValue(dijit.byId("valorICMSItem").value) ? unmaskFixed(dijit.byId("valorICMSItem").value, 2) : 0;
                        newItem.vl_PIS_item = hasValue(dijit.byId("valorPISItem").value) ? unmaskFixed(dijit.byId("valorPISItem").value, 2) : 0;
                        newItem.vl_COFINS_item = hasValue(dijit.byId("valorCOFINSItem").value) ? unmaskFixed(dijit.byId("valorCOFINSItem").value, 2) : 0;
                        newItem.vl_IPI_item = hasValue(dijit.byId("valorIPIItem").value) ? unmaskFixed(dijit.byId("valorIPIItem").value, 2) : 0;
                        newItem.pc_aliquota_ICMS = hasValue(dijit.byId("aliquotaICMSItem").value) ? dijit.byId("aliquotaICMSItem").value : 0;
                        newItem.pc_aliquota_PIS = hasValue(dijit.byId("aliquotaPISItem").value) ? dijit.byId("aliquotaPISItem").value : 0;
                        newItem.pc_aliquota_COFINS = hasValue(dijit.byId("aliquotaCOFINSItem").value) ? dijit.byId("aliquotaCOFINSItem").value : 0;
                        newItem.pc_aliquota_IPI = hasValue(dijit.byId("aliquotaIPIItem").value) ? dijit.byId("aliquotaIPIItem").value : 0;
                        break;
                }

                newItem.pc_aliquota_aproximada = hasValue(dijit.byId("pc_aliquota_ap_item").value) ? unmaskFixed(dijit.byId("pc_aliquota_ap_item").value, 2) : 0;
                newItem.vl_aproximado = hasValue(dijit.byId("vl_aproximado_item").value) ? unmaskFixed(dijit.byId("vl_aproximado_item").value, 2) : 0;
            }
            if (hasValue(dijit.byId("valDescontoItem").value) && dijit.byId("valDescontoItem").value > 0) {
                dijit.byId("vlDesconto")._onChangeActive = false;
                dijit.byId("vlDesconto").set("value", dijit.byId("vlDesconto").value + dijit.byId("valDescontoItem").value);
                dijit.byId("vlDesconto")._onChangeActive = true;
                dijit.byId("vlDesconto").set("disabled", true);
                dijit.byId("pcDesconto").set("disabled", true);
            }
            else
                newItem = aplicarAcrescOuDescItemInsetGrade(newItem);
            atualizarValoresPricipais(NOVO, newItem);

            quickSortObj(dados, 'id');
            insertObjSort(dados, "id", newItem, false);
            dijit.byId("gridItem").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));

            gerar_titulo = true;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item incluído com sucesso.");
            apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);

            //FISCAL
            if (dijit.byId("ckNotaFiscal").checked) {
                switch (TIPOMOVIMENTO) {
                    case SERVICO:
                        if (dijit.byId("ckNotaFiscal").checked) {
                            var baseFiscal = hasValue(dijit.byId("baseISS").value) ? dijit.byId("baseISS").value : 0;
                            var baseItem = hasValue(dijit.byId("baseCalcISSItem").value) ? dijit.byId("baseCalcISSItem").value : 0;
                            var baseISS = baseFiscal + baseItem;
                            dijit.byId("baseISS").set("value", baseISS);
                            var valorFiscal = hasValue(dijit.byId("vl_iss").value) ? dijit.byId("vl_iss").value : 0;
                            var valorItem = hasValue(dijit.byId("valorISSItem").value) ? dijit.byId("valorISSItem").value : 0;
                            var valorISS = valorFiscal + valorItem;
                            dijit.byId("vl_iss").set("value", valorISS);
                        }
                        break;
                    case ENTRADA:
                    case SAIDA:
                    case DEVOLUCAO:
                        replicarValoresImpostosParaFiscalNFProduto(newItem, null, NOVO);
                        break;
                }
                var vlAprox = hasValue(dijit.byId("vl_aproximado").value) ? dijit.byId("vl_aproximado").value : 0;
                var vlAproxItem = hasValue(dijit.byId("vl_aproximado_item").value) ? dijit.byId("vl_aproximado_item").value : 0;
                var vlAproxTotal = vlAprox + vlAproxItem;
                dijit.byId("vl_aproximado").set("value", vlAproxTotal);

                if (vlAproxTotal > 0 && dijit.byId("totalGeral").value > 0) {
                    var pcAliquota = (vlAproxTotal / dijit.byId("totalGeral").value) * 100;
                    dijit.byId("pc_aliquota_ap").set("value", pcAliquota);
                }
                else
                    dijit.byId("pc_aliquota_ap").set("value", 0);
            }
        }
        dijit.byId("dialogItem").hide();

        dojo.byId("cd_item_kit").value = "";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function geradorIdItem(gridItem) {
    try {
        var id = gridItem.store.objectStore.data.length;
        var itensArray = gridItem.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
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

function replicarValoresImpostosParaFiscalNFProduto(itemM, oldItemM, operacao) {
    try {
        var compBaseICMS = dijit.byId("baseICMS");
        var compVlICMS = dijit.byId("vl_icms");
        var compBasePIS = dijit.byId("basePIS");
        var compVlPIS = dijit.byId("vl_pis");
        var compBaseCOFINS = dijit.byId("baseCOFINS");
        var compVlCOFINS = dijit.byId("vl_COFINS");
        var compBaseIPI = dijit.byId("baseIPI");
        var compVlIPI = dijit.byId("vl_ipi");
        var vl_base_calculo_ICMS_item, vl_ICMS_item, vl_base_calculo_PIS_item, vl_PIS_item, vl_base_calculo_COFINS_item, vl_COFINS_item, vl_base_calculo_IPI_item, vl_IPI_item;
        if (hasValue(itemM)) {
            vl_base_calculo_ICMS_item = itemM.vl_base_calculo_ICMS_item;
            vl_ICMS_item = itemM.vl_ICMS_item;
            vl_base_calculo_PIS_item = itemM.vl_base_calculo_PIS_item;
            vl_PIS_item = itemM.vl_PIS_item;
            vl_base_calculo_COFINS_item = itemM.vl_base_calculo_COFINS_item;
            vl_COFINS_item = itemM.vl_COFINS_item;
            vl_base_calculo_IPI_item = itemM.vl_base_calculo_IPI_item;
            vl_IPI_item = itemM.vl_IPI_item;
        } else {
            vl_base_calculo_ICMS_item = operacao == EXCLUIR ? -oldItemM.vl_base_calculo_ICMS_item : (dijit.byId("baseCalcICMSItem").value - oldItemM.vl_base_calculo_ICMS_item);
            vl_ICMS_item = operacao == EXCLUIR ? -oldItemM.vl_ICMS_item : (dijit.byId("valorICMSItem").value - oldItemM.vl_ICMS_item);
            vl_base_calculo_PIS_item = operacao == EXCLUIR ? -oldItemM.vl_base_calculo_PIS_item : (dijit.byId("baseCalcPISItem").value - oldItemM.vl_base_calculo_PIS_item);
            vl_PIS_item = operacao == EXCLUIR ? -oldItemM.vl_PIS_item : (dijit.byId("valorPISItem").value - oldItemM.vl_PIS_item);
            vl_base_calculo_COFINS_item = operacao == EXCLUIR ? -oldItemM.vl_base_calculo_COFINS_item : (dijit.byId("baseCalcCOFINSItem").value - oldItemM.vl_base_calculo_COFINS_item);
            vl_COFINS_item = operacao == EXCLUIR ? -oldItemM.vl_COFINS_item : (dijit.byId("vl_COFINS").value - oldItemM.vl_COFINS_item);
            vl_base_calculo_IPI_item = operacao == EXCLUIR ? -oldItemM.vl_base_calculo_IPI_item : (dijit.byId("baseCalcIPIItem").value - oldItemM.vl_base_calculo_IPI_item);
            vl_IPI_item = operacao == EXCLUIR ? -oldItemM.vl_IPI_item : (dijit.byId("valorIPIItem").value - oldItemM.vl_IPI_item);
        }
        if (hasValue(vl_base_calculo_ICMS_item))
            compBaseICMS.set("value", compBaseICMS.value + vl_base_calculo_ICMS_item);
        if (hasValue(vl_ICMS_item))
            compVlICMS.set("value", compVlICMS.value + vl_ICMS_item);
        if (hasValue(vl_base_calculo_PIS_item))
            compBasePIS.set("value", compBasePIS.value + vl_base_calculo_PIS_item);
        if (hasValue(vl_PIS_item))
            compVlPIS.set("value", compVlPIS.value + vl_PIS_item);
        if (hasValue(vl_base_calculo_COFINS_item))
            compBaseCOFINS.set("value", compBaseCOFINS.value + vl_base_calculo_COFINS_item);
        if (hasValue(vl_COFINS_item))
            compVlCOFINS.set("value", compVlCOFINS.value + vl_COFINS_item);
        if (hasValue(vl_base_calculo_IPI_item))
            compBaseIPI.set("value", compBaseIPI.value + vl_base_calculo_IPI_item);
        if (hasValue(vl_IPI_item))
            compVlIPI.set("value", compVlIPI.value + vl_IPI_item);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function altararItem(data) {
    try {
        if (!dijit.byId("formItem").validate()) {
            return false;
        }
        if (dijit.byId("qtd_item").value == 0 &&
            (TIPOMOVIMENTO == SAIDA ||
                TIPOMOVIMENTO == SERVICO && ((dojo.byId("id_voucher_carga").value == 'true') && !eval(MasterGeral())))) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemZero, null);
            return false
        }
        if (dijit.byId("qtd_item").value > 1 && TIPOMOVIMENTO == SAIDA &&
            (dojo.byId("id_material_didatico").value == 'true')) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemmaiorque1, null);
            return false
        }
        if (dijit.byId("qtd_item").value > 1 && TIPOMOVIMENTO == SERVICO &&
            (dojo.byId("id_voucher_carga").value == 'true') && !eval(MasterGeral())) {
            caixaDialogo(DIALOGO_AVISO, msgQtdItemmaiorque1Voucher, null);
            return false
        }
        var oldValue = null;
        var value = null;
        if (data != null) value = data;
        var grid = dijit.byId("gridItem");
        var linkAnterior = document.getElementById('linkGridItem'); // valor esta vindo como string;
        if (hasValue(linkAnterior.value) && JSON.parse(linkAnterior.value))
            value = grid.itensSelecionados[0];
        else
            value = grid.selection.getSelected()[0];
        //Metodo usuardo para clonar um objeto.
        oldValue = jQuery.extend(true, {}, value);
        var valueCopy = jQuery.extend(true, {}, value);
        if (valueCopy.cd_item > 0) {
            valueCopy.cd_item = dojo.byId("cd_item").value;
            value.cd_item = dojo.byId("cd_item").value;
            valueCopy.dc_item_movimento = dojo.byId("desc_item").value;
            valueCopy.dc_plano_conta = dojo.byId("descPlanoConta").value;
            valueCopy.qt_item_movimento = dijit.byId("qtd_item").value;
            valueCopy.vl_unitario_item = dijit.byId("vlUnitario").value;
            valueCopy.vl_total_item = dijit.byId("vlTotalMovimento").value;
            valueCopy.vl_liquido_item = parseFloat(dojo.byId("vlLiquido").value.toString().replace(",", "."));
            valueCopy.vl_desconto_item = dijit.byId("valDescontoItem").value;
            valueCopy.vlDescontoItem = maskFixed(dijit.byId("valDescontoItem").value, 2),
            valueCopy.cd_plano_conta = dojo.byId("cd_plano_contas").value;  //CadPlanoConta(dijit)
            value.vl_desconto_item = dijit.byId("valDescontoItem").value;
            value.vlDescontoItem = maskFixed(dijit.byId("valDescontoItem").value, 2),
            value.pc_desconto_item = dijit.byId("perDescontoItem").value;
            value.pcDescontoItem = maskFixed(dijit.byId("perDescontoItem").value, 2);
            value.cd_plano_conta = dojo.byId("cd_plano_contas").value;
            value.dc_item_movimento = dojo.byId("desc_item").value;
            value.dc_plano_conta = dojo.byId("descPlanoConta").value;
            value.planoSugerido = plano_conta_automatico;
        }
        gerar_titulo = true;
        if (hasValue(dijit.byId("perDescontoItem").value) && dijit.byId("perDescontoItem").value > 0 && dijit.byId("perDescontoItem").value != dijit.byId("pcDesconto").value) {
            AlteraDesc = false;
            dijit.byId("vlDesconto")._onChangeActive = false;
            if (!hasValue(dijit.byId("vlDesconto")))
                dijit.byId("vlDesconto").value = 0;
            dijit.byId("vlDesconto").set("value", (dijit.byId("vlDesconto").value - dijit.byId("valDescontoItem").oldValue) + dijit.byId("valDescontoItem").value);
            dijit.byId("vlDesconto").set("disabled", true);
            dijit.byId("vlDesconto")._onChangeActive = true;
            dijit.byId("pcDesconto").set("disabled", true);
        }
        else
            AlteraDesc = true;
        //Fiscal
        if (dijit.byId("ckNotaFiscal").checked) {
            switch (TIPOMOVIMENTO) {
                case SERVICO:
                    value.cd_cfop = dojo.byId("cd_CFOP_item").value;
                    value.dc_cfop = dijit.byId("operacaoCFOPItem").value;
                    value.nm_cfop = dijit.byId("descCFOPItem").value;
                    value.vl_base_calculo_ISS_item = dijit.byId("baseCalcISSItem").value;
                    value.pc_aliquota_ISS = dijit.byId("aliquotaISSItem").value;
                    value.vl_ISS_item = dijit.byId("valorISSItem").value;

                    //fiscal
                    var baseFiscal = hasValue(dijit.byId("baseISS").value) ? dijit.byId("baseISS").value : 0;
                    var baseItem = hasValue(dijit.byId("baseCalcISSItem").value) ? dijit.byId("baseCalcISSItem").value : 0;
                    baseFiscal = baseFiscal - oldValue.vl_base_calculo_ISS_item;
                    var baseISS = baseFiscal + baseItem;
                    dijit.byId("baseISS").set("value", baseISS);
                    var valorFiscal = hasValue(dijit.byId("vl_iss").value) ? dijit.byId("vl_iss").value : 0;
                    valorFiscal = valorFiscal - oldValue.vl_ISS_item;
                    var valorItem = hasValue(dijit.byId("valorISSItem").value) ? dijit.byId("valorISSItem").value : 0;
                    var valorISS = valorFiscal + valorItem;
                    dijit.byId("vl_iss").set("value", valorISS);

                    break;
                case ENTRADA:
                case SAIDA:
                case DEVOLUCAO:
                    value.cd_cfop = dojo.byId("cd_CFOP_item").value;
                    value.dc_cfop = dijit.byId("operacaoCFOPItem").value;
                    value.nm_cfop = dijit.byId("descCFOPItem").value;
                    value.cd_cfop = hasValue(dojo.byId("cd_CFOP_item").value) ? dojo.byId("cd_CFOP_item").value : 0;
                    value.cd_situacao_tributaria_ICMS = hasValue(dijit.byId("sitTribItem").value) ? dijit.byId("sitTribItem").value : null;
                    value.cd_situacao_tributaria_PIS = hasValue(dijit.byId("cbStTribPis").value) ? dijit.byId("cbStTribPis").value : null;
                    value.cd_situacao_tributaria_COFINS = hasValue(dijit.byId("cbStTribCof").value) ? dijit.byId("cbStTribCof").value : null;
                    value.vl_base_calculo_ICMS_item = hasValue(dijit.byId("baseCalcICMSItem").value) ? dijit.byId("baseCalcICMSItem").value : 0;
                    value.vl_base_calculo_COFINS_item = hasValue(dijit.byId("baseCalcCOFINSItem").value) ? dijit.byId("baseCalcCOFINSItem").value : 0;
                    value.vl_base_calculo_PIS_item = hasValue(dijit.byId("baseCalcPISItem").value) ? dijit.byId("baseCalcPISItem").value : 0;
                    value.vl_base_calculo_IPI_item = hasValue(dijit.byId("baseCalcIPIItem").value) ? dijit.byId("baseCalcIPIItem").value : 0;
                    value.vl_ICMS_item = hasValue(dijit.byId("valorICMSItem").value) ? dijit.byId("valorICMSItem").value : 0;
                    value.vl_PIS_item = hasValue(dijit.byId("valorPISItem").value) ? dijit.byId("valorPISItem").value : 0;
                    value.vl_COFINS_item = hasValue(dijit.byId("valorCOFINSItem").value) ? dijit.byId("valorCOFINSItem").value : 0;
                    value.vl_IPI_item = hasValue(dijit.byId("valorIPIItem").value) ? dijit.byId("valorIPIItem").value : 0;
                    value.pc_aliquota_ICMS = hasValue(dijit.byId("aliquotaICMSItem").value) ? dijit.byId("aliquotaICMSItem").value : 0;
                    value.pc_aliquota_PIS = hasValue(dijit.byId("aliquotaPISItem").value) ? dijit.byId("aliquotaPISItem").value : 0;
                    value.pc_aliquota_COFINS = hasValue(dijit.byId("aliquotaCOFINSItem").value) ? dijit.byId("aliquotaCOFINSItem").value : 0;
                    value.pc_aliquota_IPI = hasValue(dijit.byId("aliquotaIPIItem").value) ? dijit.byId("aliquotaIPIItem").value : 0;
                    replicarValoresImpostosParaFiscalNFProduto(null, oldValue, EDITAR);
                    break;
            }

            var vlAprox = hasValue(dijit.byId("vl_aproximado").value) ? dijit.byId("vl_aproximado").value : 0;
            var vlAproxItem = hasValue(dijit.byId("vl_aproximado_item").value) ? dijit.byId("vl_aproximado_item").value : 0;
            vlAprox = vlAprox - oldValue.vl_aproximado;
            var vlAproxTotal = vlAprox + vlAproxItem;
            dijit.byId("vl_aproximado").set("value", vlAproxTotal);

            if (vlAproxTotal > 0 && dijit.byId("totalGeral").value > 0) {
                var pcAliquota = (vlAproxTotal / dijit.byId("totalGeral").value) * 100;
                dijit.byId("pc_aliquota_ap").set("value", pcAliquota);
            }
            else
                dijit.byId("pc_aliquota_ap").set("value", 0);
            value.pc_aliquota_aproximada = hasValue(dijit.byId("pc_aliquota_ap_item").value) ? dijit.byId("pc_aliquota_ap_item").value : 0;
            value.vl_aproximado = hasValue(dijit.byId("vl_aproximado_item").value) ? dijit.byId("vl_aproximado_item").value : 0;
        }

        atualizarValoresPricipais(EDITAR, valueCopy, oldValue);
        grid.update();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item alterado com sucesso.");
        apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
        dijit.byId("dialogItem").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesItem(value, grid, ehLink) {
    try {
        limparItem();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('linkGridItem');
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];


        if (value.cd_item > 0) {
            if ((TIPOMOVIMENTO == SERVICO || TIPOMOVIMENTO == SAIDA || TIPOMOVIMENTO == ENTRADA || TIPOMOVIMENTO == DEVOLUCAO) && dijit.byId("ckNotaFiscal").get("checked"))
                findIsLoadComponetesItemMovto(TIPOMOVIMENTO, value.cd_situacao_tributaria_ICMS, value.cd_situacao_tributaria_PIS, value.cd_situacao_tributaria_COFINS,
                                              function () { loadItemMovimento(value); calculaValorItem(value); })
            else {
                loadItemMovimento(value);
            }
               
        }

        //Não possibilita a alteração de item no item de movimento, pois o mesmo já pode ter estoque. O mesmo poderá excluir o item de movimento e incluir outro:
        dijit.byId('cadItem').set('disabled', true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadItemMovimento(value) {
    try {
        var compVlUnit = dijit.byId('vlUnitario');
        var vlTotal = dijit.byId("vlTotalMovimento");
        var vlLiquido = dijit.byId('vlLiquido');

        desabilitaHabilitaItem(false);

        dojo.byId("cd_item_movimento").value = value.cd_item_movimento;
        dojo.byId("cd_item").value = value.cd_item;
        dojo.byId("desc_item").value = value.dc_item_movimento;
        dojo.byId("cd_plano_contas").value = value.cd_plano_conta;
        dojo.byId("descPlanoConta").value = value.dc_plano_conta;
        dijit.byId("qtd_item")._onChangeActive = false;
        dijit.byId("qtd_item").set("value", value.qt_item_movimento);
        dijit.byId("qtd_item").oldValue = value.qt_item_movimento_dev;
        dijit.byId("qtd_item")._onChangeActive = true;
        compVlUnit._onChangeActive = false;
        compVlUnit.set("value", unmaskFixed(value.vl_unitario_item, 2));
        compVlUnit.value = value.vl_unitario_item;
        compVlUnit.oldValue = unmaskFixed(value.vl_unitario_item, 2);
        compVlUnit._onChangeActive = true;
        vlTotal._onChangeActive = false;
        vlTotal.set("value", unmaskFixed(value.vl_total_item, 2));
        vlTotal.value = value.vl_total_item;
        vlTotal.oldValue = value.vl_total_item;
        vlTotal._onChangeActive = true;
        vlLiquido._onChangeActive = false;
        vlLiquido.set("value", unmaskFixed(value.vl_liquido_item, 2));
        vlLiquido.value = value.vl_liquido_item;
        vlLiquido.oldValue = unmaskFixed(value.vl_liquido_item, 2);
        vlLiquido._onChangeActive = true;
        dijit.byId("perDescontoItem")._onChangeActive = false;
        dijit.byId("perDescontoItem").set("value", value.pc_desconto_item);
        dijit.byId("perDescontoItem")._onChangeActive = true;

        dijit.byId("valDescontoItem")._onChangeActive = false;
        dijit.byId("valDescontoItem").set("value", value.vl_desconto_item);
        dijit.byId("valDescontoItem").oldValue = value.vl_desconto_item;
        dijit.byId("valDescontoItem")._onChangeActive = true;
        //dijit.byId("cd_plano_contas").value = value.cd_plano_conta;
        dojo.byId("id_voucher_carga").value = value.id_voucher_carga;

        if (value.planoSugerido)
            dijit.byId("cadPlanoConta").set("disabled", true);
        if (dijit.byId("ckNotaFiscal").checked) {
            dijit.byId("pc_aliquota_ap_item").set("value", value.pc_aliquota_aproximada);
            dijit.byId("vl_aproximado_item").set("value", value.vl_aproximado);
            switch (TIPOMOVIMENTO) {
                case SERVICO:
                    dijit.byId("baseCalcISSItem")._onChangeActive = false;
                    dijit.byId("baseCalcISSItem").set("value", value.vl_base_calculo_ISS_item);
                    dijit.byId("baseCalcISSItem")._onChangeActive = true;
                    dijit.byId("aliquotaISSItem")._onChangeActive = false;
                    dijit.byId("aliquotaISSItem").set("value", value.pc_aliquota_ISS);
                    dijit.byId("aliquotaISSItem")._onChangeActive = true;
                    dijit.byId("valorISSItem")._onChangeActive = false;
                    dijit.byId("valorISSItem").set("value", value.vl_ISS_item);
                    dijit.byId("valorISSItem")._onChangeActive = true;
                    if (hasValue(value.cd_cfop))
                        dojo.byId("cd_CFOP_item").value = value.cd_cfop;
                    if (hasValue(value.nm_cfop))
                        dijit.byId("descCFOPItem").set("value", value.nm_cfop);
                    if (hasValue(value.dc_cfop))
                        dijit.byId("operacaoCFOPItem").set("value", value.dc_cfop);
                    dojo.byId("id_voucher_carga").value = value.id_voucher_carga;
                    break;
                case ENTRADA:
                case SAIDA:
                    habilitarDesabilitarChangeCamposItemProduto(false);
                    if (hasValue(value.cd_cfop))
                        dojo.byId("cd_CFOP_item").value = value.cd_cfop;
                    if (hasValue(value.nm_cfop))
                        dijit.byId("descCFOPItem").set("value", value.nm_cfop);
                    if (hasValue(value.dc_cfop))
                        dijit.byId("operacaoCFOPItem").set("value", value.dc_cfop);
                    if (hasValue(value.cd_situacao_tributaria_ICMS))
                        dijit.byId("sitTribItem").set("value", value.cd_situacao_tributaria_ICMS);
                    if (hasValue(value.cd_situacao_tributaria_PIS))
                        dijit.byId("cbStTribPis").set("value", value.cd_situacao_tributaria_PIS);
                    if (hasValue(value.cd_situacao_tributaria_COFINS))
                        dijit.byId("cbStTribCof").set("value", value.cd_situacao_tributaria_COFINS);
                    dijit.byId("baseCalcICMSItem").set("value", value.vl_base_calculo_ICMS_item);
                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                    dijit.byId("baseCalcCOFINSItem").set("value", value.vl_base_calculo_COFINS_item);
                    dijit.byId("baseCalcPISItem").set("value", value.vl_base_calculo_PIS_item);
                    dijit.byId("baseCalcIPIItem").set("value", value.vl_base_calculo_IPI_item);
                    dijit.byId("valorICMSItem").set("value", value.vl_ICMS_item);
                    dijit.byId("valorPISItem").set("value", value.vl_PIS_item);
                    dijit.byId("valorCOFINSItem").set("value", value.vl_COFINS_item);
                    dijit.byId("valorIPIItem").set("value", value.vl_IPI_item);
                    dijit.byId("aliquotaICMSItem").set("value", value.pc_aliquota_ICMS);
                    dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                    dijit.byId("aliquotaPISItem").set("value", value.pc_aliquota_PIS);
                    dijit.byId("aliquotaCOFINSItem").set("value", value.pc_aliquota_COFINS);
                    dijit.byId("aliquotaIPIItem").set("value", value.pc_aliquota_IPI);
                    if (dijit.byId("sitTribItem").value > 0) {
                        var listaSituacao = dijit.byId("sitTribItem").store.data;
                        quickSortObj(listaSituacao, 'id');
                        var situacao = binaryObjSearch(listaSituacao, "id", dijit.byId("sitTribItem").value);
                        if (listaSituacao[situacao].formaTrib == ISENTO)
                            dijit.byId("aliquotaICMSItem").set("disabled", true);
                        else
                            dijit.byId("aliquotaICMSItem").set("disabled", false);
                    }
                    else
                        dijit.byId("aliquotaICMSItem").set("disabled", false);
                    dojo.byId("id_material_didatico").value = value.id_material_didatico
                    habilitarDesabilitarChangeCamposItemProduto(true);
                    break;
                case DEVOLUCAO:
                    habilitarDesabilitarChangeCamposItemProduto(false);
                    if (hasValue(value.cd_cfop))
                        dojo.byId("cd_CFOP_item").value = value.cd_cfop;
                    if (hasValue(value.nm_cfop))
                        dijit.byId("descCFOPItem").set("value", value.nm_cfop);
                    if (hasValue(value.dc_cfop))
                        dijit.byId("operacaoCFOPItem").set("value", value.dc_cfop);
                    if (hasValue(value.cd_situacao_tributaria_ICMS))
                        dijit.byId("sitTribItem").set("value", value.cd_situacao_tributaria_ICMS);
                    if (hasValue(value.cd_situacao_tributaria_PIS))
                        dijit.byId("cbStTribPis").set("value", value.cd_situacao_tributaria_PIS);
                    if (hasValue(value.cd_situacao_tributaria_COFINS))
                        dijit.byId("cbStTribCof").set("value", value.cd_situacao_tributaria_COFINS);
                    dijit.byId("baseCalcICMSItem").set("value", value.vl_base_calculo_ICMS_item);
                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                    dijit.byId("baseCalcCOFINSItem").set("value", value.vl_base_calculo_COFINS_item);
                    dijit.byId("baseCalcPISItem").set("value", value.vl_base_calculo_PIS_item);
                    dijit.byId("baseCalcIPIItem").set("value", value.vl_base_calculo_IPI_item);
                    dijit.byId("valorICMSItem").set("value", value.vl_ICMS_item);
                    dijit.byId("valorPISItem").set("value", value.vl_PIS_item);
                    dijit.byId("valorCOFINSItem").set("value", value.vl_COFINS_item);
                    dijit.byId("valorIPIItem").set("value", value.vl_IPI_item);
                    dijit.byId("aliquotaICMSItem").set("value", value.pc_aliquota_ICMS);
                    dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                    dijit.byId("aliquotaPISItem").set("value", value.pc_aliquota_PIS);
                    dijit.byId("aliquotaCOFINSItem").set("value", value.pc_aliquota_COFINS);
                    dijit.byId("aliquotaIPIItem").set("value", value.pc_aliquota_IPI);
                    desabilitaHabilitaItem(true);
                    habilitarDesabilitarChangeCamposItemProduto(true);
                    break;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarItem(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else if (TIPOMOVIMENTO == ENTRADA && validaEntradaMasterMaterialDidatico(itensSelecionados[0]).valid == false) {
            var retornoValidaCompra = validaEntradaMasterMaterialDidatico(itensSelecionados[0]);
            if (retornoValidaCompra.valid == false) {
                caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
            }
        } else if (TIPOMOVIMENTO == DEVOLUCAO && dojo.byId('id_natureza_movto').value == ENTRADA && validaEntradaMasterMaterialDidatico(itensSelecionados[0]).valid == false) {
            var retornoValidaCompra = validaEntradaMasterMaterialDidatico(itensSelecionados[0]);
            if (retornoValidaCompra.valid == false) {
                caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
            }
        }
        else {
            var gridItem = dijit.byId('gridItem');
            apresentaMensagem("apresentadorMensagemMovto", null);
            IncluirAlterar(0, 'divAlterarItem', 'divIncluirItem', 'divIncluirItem', 'apresentadorMensagemItem', 'divCancelarItem', 'divClearItem');
            (function (callback) {
                retornarItemKitFK(itensSelecionados[0], null);
                if (hasValue(itensSelecionados[0]) && hasValue(itensSelecionados[0].cd_grupo_estoque)) {
                    situacaoTributariaItem(itensSelecionados[0].cd_grupo_estoque);
                }
                keepValuesItem(null, gridItem, true);
                if (hasValue(itensSelecionados[0]) && hasValue(itensSelecionados[0].cd_grupo_estoque)) {
                    situacaoTributariaItem(itensSelecionados[0].cd_grupo_estoque);
                }
                dijit.byId("dialogItem").show();
                callback.call();
            })(function () {
                calculaValorItem(itensSelecionados[0]);
            });
            // 
            dijit.byId("dialogItem").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarItemServicoSelecionadoGrid(Memory, ObjectStore, nomeId, grid) {
    try {
        grid.store.save();
        var dados = grid.store.objectStore.data;
        gerar_titulo = true;
        apresentaMensagem("apresentadorMensagemMovto", null);
        

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--)
                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null) {
                    if (TIPOMOVIMENTO == ENTRADA && validaEntradaMasterMaterialDidatico(dados[i]).valid == false) {
                        var retornoValidaCompra = validaEntradaMasterMaterialDidatico(dados[i]);
                        if (retornoValidaCompra.valid == false) {
                            caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                        }
                        return false;
                    }
                    if (TIPOMOVIMENTO == DEVOLUCAO && dojo.byId('id_natureza_movto').value == ENTRADA && validaEntradaMasterMaterialDidatico(dados[i]).valid == false) {
                        var retornoValidaCompra = validaEntradaMasterMaterialDidatico(dados[i]);
                        if (retornoValidaCompra.valid == false) {
                            caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                        }
                        return false;
                    }
                    if (TIPOMOVIMENTO == SAIDA && dados[i].id_material_didatico == true) {
                        caixaDialogo(DIALOGO_ERRO, msgErroItemMaterialDidatico, null);
                        return false;
                    }
                    if (TIPOMOVIMENTO == SERVICO && dados[i].id_voucher_carga == true) {
                        caixaDialogo(DIALOGO_ERRO, msgErroItemVoucher, null);
                        return false;
                    }

                    atualizarValoresPricipais(EXCLUIR, dados[i]);

                    //Fiscal
                    if (dijit.byId("ckNotaFiscal").checked) {
                        switch (TIPOMOVIMENTO) {
                            case SERVICO:
                                var baseFiscal = hasValue(dijit.byId("baseISS").value) ? dijit.byId("baseISS").value : 0;
                                var baseItem = dados[i].vl_base_iss;
                                var baseISS = baseFiscal - baseItem;
                                dijit.byId("baseISS").set("value", baseISS);
                                var valorFiscal = hasValue(dijit.byId("vl_iss").value) ? dijit.byId("vl_iss").value : 0;
                                var valorItem = dados[i].vl_iss_item;
                                var valorISS = valorFiscal - valorItem;
                                dijit.byId("vl_iss").set("value", valorISS);

                                break;
                            case ENTRADA:
                            case SAIDA:
                            case DEVOLUCAO:
                                replicarValoresImpostosParaFiscalNFProduto(null, dados[i], EXCLUIR);
                                break;
                        }
                        var vlAprox = hasValue(dijit.byId("vl_aproximado").value) ? dijit.byId("vl_aproximado").value : 0;
                        var vlAproxItem = dados[i].vl_aproximado;
                        var vlAproxTotal = vlAprox - vlAproxItem;
                        dijit.byId("vl_aproximado").set("value", vlAproxTotal);

                        if (vlAproxTotal > 0 && dijit.byId("totalGeral").value > 0) {
                            var pcAliquota = (vlAproxTotal / dijit.byId("totalGeral").value) * 100;
                            dijit.byId("pc_aliquota_ap").set("value", pcAliquota);
                        }
                        else
                            dijit.byId("pc_aliquota_ap").set("value", 0);
                    }

                    dados.splice(i, 1); // Remove o item do array
                }

            grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function habilitarDesabilitarChangeCamposItemProduto(bool) {
    try {
        dijit.byId("sitTribItem")._onChangeActive = bool;
        dijit.byId("cbStTribPis")._onChangeActive = bool;
        dijit.byId("cbStTribCof")._onChangeActive = bool;
        dijit.byId("baseCalcICMSItem")._onChangeActive = bool;
        dijit.byId("baseCalcCOFINSItem")._onChangeActive = bool;
        dijit.byId("baseCalcPISItem")._onChangeActive = bool;
        dijit.byId("baseCalcIPIItem")._onChangeActive = bool;
        dijit.byId("valorICMSItem")._onChangeActive = bool;
        dijit.byId("valorPISItem")._onChangeActive = bool;
        dijit.byId("valorCOFINSItem")._onChangeActive = bool;
        dijit.byId("valorIPIItem")._onChangeActive = bool;
        dijit.byId("aliquotaICMSItem")._onChangeActive = bool;
        dijit.byId("aliquotaPISItem")._onChangeActive = bool;
        dijit.byId("aliquotaCOFINSItem")._onChangeActive = bool;
        dijit.byId("aliquotaIPIItem")._onChangeActive = bool;
    }
    catch (e) {
        postGerarLog(e);
    }
}
// Fim

function findIsLoadComponetesNovoMovto(xhr, ready, Memory, FilteringSelect) {
    xhr.get({
        url: Endereco() + "/api/escola/getComponentesNovoMovimento?tipoMovimento=" + TIPOMOVIMENTO + "&idOrigemNF=" + ORIGEMCHAMADONF,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            //apresentaMensagem("apresentadorMensagemMovto", null);
            if (data != null && data.retorno != null) {
                dijit.byId("tpFinanceiro")._onChangeActive = false;
                criarOuCarregarCompFiltering("tpFinanceiro", data.retorno.tiposFinan, "", TIPOFINANCEIROTITULO, ready, Memory, FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro');
                dijit.byId("tpFinanceiro")._onChangeActive = true;
                criarOuCarregarCompFiltering("tipoDocumentoTit", data.retorno.tiposFinan, "", null, ready, Memory, FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro');
                criarOuCarregarCompFiltering("bancoCheque", data.retorno.bancosCheque, "", null, ready, Memory, FilteringSelect,
                                             'cd_banco', 'no_banco');
                if (data.retorno.cd_politica_comercial != null && data.retorno.cd_politica_comercial > 0) {
                    dojo.byId("cdPoliticaComercial").value = data.retorno.cd_politica_comercial;
                    dojo.byId("noPoliticaCom").value = data.retorno.dc_politica_comercial;
                }
                if (hasValue(data.retorno.situacoesTributariaICMS))
                    loadSituacao(data.retorno.situacoesTributariaICMS, 0);
                if (TIPOMOVIMENTO == SAIDA)
                    loadMeioPagamento(null);
            }
            hideCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        hideCarregando();
        apresentaMensagem("apresentadorMensagemMovto", error);
    });
}

function findComponentesTagTitulos() {
    try {
        movimento = mountDataMovimentoForPost();
        movimento.titulos = [];
        movimento.titulos.push(montarTituloDefault());
        var titulos = [];
        dojo.xhr.post({
            url: Endereco() + "/api/escola/postComponentesTitulos",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(movimento)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                
                if (hasCartao()) {
                    var cd_tipo_financeiro = dijit.byId("tpFinanceiro").value;
                    if (hasValue(cd_tipo_financeiro))
                        getLocalMovtoGeralOuCartaoMovimento(cd_tipo_financeiro);
                } else {
                    if (data.bancos != null && data.bancos.length > 0) {
                        criarOuCarregarCompFiltering("edBanco", data.bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    }
                }
                if (data != null && data.tiposFinan != null)
                    criarOuCarregarCompFiltering("tipoDocumentoTit", data.tiposFinan, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro');
                if (data.titulos != null && data.titulos.length > 0)
                    titulos = data.titulos;
                dijit.byId("gridTitulo").setStore(dojo.data.ObjectStore({ objectStore: dojo.store.Memory({ data: titulos }) }));
                dijit.byId("gridTitulo").update();
                gerar_titulo = false;
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMovto', error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function findIsLoadComponetesItemMovto(id_tipo_movimento, cd_sit_ICMS, cd_sit_PIS, cd_sit_COFINS, pFuncao) {
    try {
        if (cd_sit_ICMS == null)
            cd_sit_ICMS = 0;
        if (cd_sit_PIS == null)
            cd_sit_PIS = 0;
        if (cd_sit_COFINS == null)
            cd_sit_COFINS = 0;
        showCarregando();
        dojo.xhr.get({
            url: Endereco() + "/api/fiscal/getComponentesByItemMovimentoEdit?id_tipo_movimento=" + TIPOMOVIMENTO + "&cd_sit_ICMS=" + cd_sit_ICMS + "&cd_sit_PIS=" + cd_sit_PIS +
                                                                            "&cd_sit_COFINS=" + cd_sit_COFINS + "&cdTpNF=" + dojo.byId('cd_tp_nf').value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                apresentaMensagem("apresentadorMensagemItem", null);
                if (data != null && data.retorno != null) {
                    if (hasValue(data.retorno.situacoesTributariaICMS))
                        loadSituacao(data.retorno.situacoesTributariaICMS, 0);
                    //criarOuCarregarCompFiltering("sitTribItem", data.retorno.situacoesTributariaICMS, "", hasValue(cd_sit_ICMS) ? cd_sit_ICMS : null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                    //                             'cd_situacao_tributaria', 'dc_situacao_tributaria');
                    if (hasValue(data.retorno.situacoesTributariaPIS))
                        criarOuCarregarCompFiltering("cbStTribPis", data.retorno.situacoesTributariaPIS, "", hasValue(cd_sit_PIS) ? cd_sit_ICMS : null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                     'cd_situacao_tributaria', 'dc_situacao_tributaria');
                    if (hasValue(data.retorno.situacoesTributariaCOFINS))
                        criarOuCarregarCompFiltering("cbStTribCof", data.retorno.situacoesTributariaCOFINS, "", hasValue(cd_sit_COFINS) ? cd_sit_ICMS : null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                     'cd_situacao_tributaria', 'dc_situacao_tributaria');
                }
                if (hasValue(pFuncao))
                    pFuncao.call();
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem("apresentadorMensagemItem", error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D movimento

function showEditMovimento(cdMovimento, id_nf, id_material_didatico) {
    dojo.xhr.get({
        url: Endereco() + "/api/fiscal/getComponentesByMovimentoEdit?cd_movimento=" + cdMovimento + "&id_nf=" + id_nf + "&id_tipo_movimento=" + TIPOMOVIMENTO + "&id_material_didatico=" + id_material_didatico,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            alterou_tp_nf = false;
            apresentaMensagem("apresentadorMensagemMovto", null);
            onActiveChangeCamposGerarTitulos(false);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.tiposFinan))
                    criarOuCarregarCompFiltering("tpFinanceiro", data.retorno.tiposFinan, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro');
                if (data != null && data.retorno != null && hasValue(data.retorno.bancosCheque))
                    criarOuCarregarCompFiltering("bancoCheque", data.retorno.bancosCheque, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_banco', 'no_banco');
                loadDataMovimento(data.retorno);
                onActiveChangeCamposGerarTitulos(true);
                showCarregando();
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemMovto", error);
        showCarregando();
    });
}

function loadDataMovimento(data) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridItem = dijit.byId("gridItem");
                var gridKit = dijit.byId("gridKit");

                var compPcAcresc = dijit.byId("pcAcrec");
                var compVlAcresc = dijit.byId("vlAc");
                var compPcDesc = dijit.byId("pcDesconto");
                var compVlDesc = dijit.byId("vlDesconto");
                var compVlTotalItens = dijit.byId("totalItens");
                var compVlTotalGeral = dijit.byId("totalGeral");

                dojo.byId("cd_movimento").value = data.cd_movimento;
                dojo.byId("cdPessoaMvtoCad").value = data.cd_pessoa;
                if (hasValue(data.cd_pessoa_aluno))
                    dojo.byId("cdPessoaAlunoFKMovimento").value = data.cd_pessoa_aluno;
                if (hasValue(data.cd_aluno))
                    dojo.byId("cdAlunoFKMovimento").value = data.cd_aluno;
                if (hasValue(data.no_aluno)) {
                    dojo.byId("noAlunoFKMovimento").value = data.no_aluno;
                    dijit.byId("limparAlunoFKMovimento").set("disabled", false);
                }

                dojo.byId("cdPoliticaComercial").value = data.cd_politica_comercial;
                dijit.byId("tpFinanceiro").set("value", data.cd_tipo_financeiro);
                if (hasValue(data.nm_movimento))
                    dojo.byId("nrMovto").value = data.nm_movimento;
                if (hasValue(data.dc_serie_movimento))
                    dojo.byId("serie").value = data.dc_serie_movimento;
                if (hasValue(data.dt_emissao_movimento))
                    dijit.byId("dtaEmis").set("value", data.dt_emissao_movimento);
                if (hasValue(data.dt_vcto_movimento)) {
                    dijit.byId("dtaVenc")._onChangeActive = false;
                    dijit.byId("dtaVenc").set("value", data.dt_vcto_movimento);
                    dijit.byId("dtaVenc")._onChangeActive = true;
                }
                if (hasValue(data.dt_mov_movimento)) {
                    dijit.byId("dtaMovto")._onChangeActive = false;
                    dijit.byId("dtaMovto").set("value", data.dt_mov_movimento);
                    dijit.byId("dtaMovto")._onChangeActive = true;
                }
                //Fk's
                if (hasValue(data.cd_pessoa)) {
                    dojo.byId("cdPessoaMvtoCad").value = data.cd_pessoa;
                    dijit.byId("noPessoaMovto")._onChangeActive = false;
                    dijit.byId("noPessoaMovto").set("value", data.no_pessoa);
                    dijit.byId("noPessoaMovto")._onChangeActive = true;
                }
                if (hasValue(data.cd_politica_comercial)) {
                    dojo.byId("cdPoliticaComercial").value = data.cd_politica_comercial;
                    dojo.byId("noPoliticaCom").value = data.dc_politica_comercial;
                }
                dijit.byId("ckNotaFiscal")._onChangeActive = false;
                dijit.byId("ckNotaFiscal").set("checked", data.id_nf);
                dijit.byId("ckNotaFiscal")._onChangeActive = true;
                configuraLayoutNF(data.id_nf);
                compVlAcresc._onChangeActive = false;
                compVlAcresc.set("value", data.vl_acrescimo);
                compVlAcresc.oldValue = data.vl_acrescimo;
                compVlAcresc._onChangeActive = true;

                compPcAcresc._onChangeActive = false;
                compPcAcresc.set("value", data.pc_acrescimo);
                compPcAcresc.oldValue = data.pc_acrescimo;
                compPcAcresc._onChangeActive = true;

                compPcDesc._onChangeActive = false;
                compPcDesc.set("value", data.pc_desconto);
                compPcDesc.oldValue = data.pc_desconto;
                compPcDesc._onChangeActive = true;

                compVlDesc._onChangeActive = false;
                compVlDesc.set("value", data.vl_desconto);
                compVlDesc.oldValue = data.vl_desconto;
                compVlDesc._onChangeActive = true;

                dijit.byId("idObs").set("value", data.tx_obs_movimento);
                dijit.byId("ckVendaFuturaCad").set("checked", data.id_venda_futura);

                var descDiferente = false;
                if (hasValue(data.ItensMovimento))
                    if (hasValue(data.ItensMovimento) && data.ItensMovimento.length > 0) {
                        var totalItens = 0;
                        var totalGeral = 0;

                        var gridItemMovt = dijit.byId("gridItem");
                        $.each(data.ItensMovimento, function (idx, value) {
                            var id = geradorIdItem(gridItemMovt);
                            montarObjItemMovimento(gridItemMovt, value, id);
                        });

                        gridItemMovt.setStore(new ObjectStore({ objectStore: new Memory({ data: gridItemMovt.store.objectStore.data }) }));
                        $.each(data.ItensMovimento, function (index, value) {
                            totalItens += value.vl_total_item;
                            totalGeral += value.vl_liquido_item;
                            if (!hasValue(value.pc_desconto))
                                value.pc_desconto = 0;
                            if (value.pc_desconto != data.pc_desconto)
                                descDiferente = true;
                        });
                        compVlTotalItens.set("value", totalItens);
                        compVlTotalGeral.set("value", totalGeral);
                    }
                if (hasValue(data.ItemMovimentoKit) && data.ItemMovimentoKit.length > 0)
                    setItemsMovimentoKitGrid(data.ItemMovimentoKit, data.ItensMovimento);

                if (descDiferente) {
                    compPcDesc.set("disabled", true);
                    compVlDesc.set("disabled", true);
                }
                else
                    if (dijit.byId("pcAcrec").disabled) {
                        compPcDesc.set("disabled", false);
                        compVlDesc.set("disabled", false);
                    }

                dojo.byId("cd_movimento").value = data.cd_movimento;
                if (data.cd_tipo_financeiro == TIPOFINANCEIROCHEQUE) {
                    habilitarTagCheque(true);
                    if (hasValue(data.cheque)) {
                        dojo.byId("cd_cheque").value = data.cheque.cd_cheque;
                        dojo.byId("emissorChequeName").value = data.cheque.no_emitente_cheque;
                        dojo.byId("agencia").value = data.cheque.no_agencia_cheque;
                        if (hasValue(data.cheque.nm_agencia_cheque))
                            dojo.byId("nroAgencia").value = data.cheque.nm_agencia_cheque;
                        if (hasValue(data.cheque.nm_digito_agencia_cheque))
                            dojo.byId("dgAgencia").value = data.cheque.nm_digito_agencia_cheque;
                        if (hasValue(data.cheque.nm_conta_corrente_cheque))
                            dojo.byId("nroContaCorrente").value = data.cheque.nm_conta_corrente_cheque;
                        if (hasValue(data.cheque.nm_digito_cc_cheque))
                            dojo.byId("dgContaCorrente").value = data.cheque.nm_digito_cc_cheque;
                        dojo.byId("nroPrimeiroCheque").value = data.cheque.nm_primeiro_cheque;
                        dijit.byId("bancoCheque").set("value", data.cheque.cd_banco);
                    }
                }
                else
                    habilitarTagCheque(false);

                dojo.byId("id_origem_movimento").value = data.id_origem_movimento;
                dojo.byId("cd_origem_movimento").value = data.cd_origem_movimento;

                var parametrosTela = getParamterosURL();
                if ((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) {

                    var optionsComboContrato = [];

                    if (data['contratos_combo_material_didatico'] != null &&
                        data['contratos_combo_material_didatico'] != undefined &&
                        data['contratos_combo_material_didatico'].length > 0) {
                        if (hasValue(data.no_contrato)) {
                            
                            var alreadyExistsOption = data.contratos_combo_material_didatico.some(function(x) {
                                return x.cd_contrato == data.cd_origem_movimento;
                            });

                            //se a opção de contrato não existe no combo, adiciona a mesma no itens
                            if (!alreadyExistsOption)
                            {
                                data.contratos_combo_material_didatico.push({ cd_contrato: data.cd_origem_movimento, nm_contrato: data.nm_contrato, nm_matricula_contrato: data.no_contrato , no_contrato: data.no_contrato});
                            }

                            dijit.byId("tpContrato")._onChangeActive = false;
                            criarOuCarregarCompFiltering("tpContrato", data.contratos_combo_material_didatico, "", data.cd_origem_movimento, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_contrato', 'no_contrato');
                            dijit.byId("tpContrato")._onChangeActive = true;
                        }
                    }else if (hasValue(data.no_contrato)) {

                        dijit.byId("tpContrato")._onChangeActive = false;
                        criarOuCarregarCompFiltering("tpContrato",
                            [data],
                            "",
                            data.cd_origem_movimento,
                            dojo.ready,
                            dojo.store.Memory,
                            dijit.form.FilteringSelect,
                            'cd_origem_movimento',
                            'no_contrato');
                        dijit.byId("tpContrato")._onChangeActive = true;
                    } else {

                        dijit.byId("tpContrato")._onChangeActive = false;
                        criarOuCarregarCompFiltering("tpContrato", [], "", '', dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_contrato', 'no_contrato');
                        dijit.byId("tpContrato")._onChangeActive = true;
                    }


                    

                    if (hasValue(data.no_curso)) {
                        dojo.byId("cdCursoFKMovimento").value = data.cd_curso;
                        dojo.byId("noCursoFKMovimento").value = data.no_curso;
                        dijit.byId("limparCursoFKMovimento").set('disabled', false);
                    }

                    dijit.byId("tpContrato").set("disabled", true);
                    dijit.byId("pesCursoFKMovimento").set("disabled", true);
                    dijit.byId("limparCursoFKMovimento").set("disabled", true);
                    dijit.byId("pesAlunoFKMovimento").set("disabled", true);
                    dijit.byId("limparAlunoFKMovimento").set("disabled", true);

                }

                //Fiscal
                if (data.id_nf) {
                    switch (data.id_tipo_movimento) {
                        case SERVICO:
                            if (hasValue(data.cd_tipo_nota_fiscal)) {
                                dojo.byId("cd_tp_nf").value = data.cd_tipo_nota_fiscal;
                                dojo.byId("tpNf").value = data.dc_tipo_nota;
                            }
                            if (hasValue(data.vl_base_calculo_ISS_nf))
                                dijit.byId("baseISS").set("value", data.vl_base_calculo_ISS_nf);
                            if (hasValue(data.vl_ISS_nf))
                                dijit.byId("vl_iss").set("value", data.vl_ISS_nf);
                            if (hasValue(data.dc_cfop_nf))
                                dijit.byId("operacaoCFOP").set("value", data.dc_cfop_nf);
                            if (hasValue(data.nm_cfop))
                                dijit.byId("CFOP").set("value", data.nm_cfop);
                            if (hasValue(data.cd_cfop_nf))
                                dojo.byId("cd_cfop_nf").value = data.cd_cfop_nf;
                            if (hasValue(data.tx_obs_fiscal))
                                dijit.byId("idObsNF").set("value", data.tx_obs_fiscal);
                            dijit.byId("statusNFS").set("value", data.id_status_nf);
                            dojo.byId("id_natureza_movto").value = data.TipoNF != null ? data.TipoNF.id_natureza_movimento : 0;
                            if (data.TipoNF != null && data.TipoNF.id_natureza_movimento == ENTRADA) {
                                dijit.byId("nrMovto").set("disabled", false);
                                dijit.byId("serie").set("disabled", false);
                                dijit.byId("nrMovto").set("required", true);
                                dijit.byId("serie").set("required", true);
                            } else {
                                dijit.byId("nrMovto").set("disabled", true);
                                dijit.byId("serie").set("disabled", true);
                                dijit.byId("nrMovto").set("required", false);
                                dijit.byId("serie").set("required", false);
                            }
                            if (empresa_propria) {
                                if (hasValue(data.dta_autorizacao_nfe))
                                    dijit.byId("dtaAuto").set("value", data.dta_autorizacao_nfe);
                                if (hasValue(data.nm_nfe))
                                    dijit.byId("nrNFS").set("value", data.nm_nfe);
                                if (hasValue(data.dc_url_nf))
                                    dijit.byId("urlNFS").set("value", data.dc_url_nf);
                                if (hasValue(data.dc_mensagem_retorno))
                                    dijit.byId("obsResposta").set("value", data.dc_mensagem_retorno);
                                if (hasValue(data.dta_nfe_cancel))
                                    dijit.byId("dtaCancel").set("value", data.dta_nfe_cancel);
                                if (hasValue(data.dc_protocolo_cancel))
                                    dijit.byId("nrProtocCancel").set("value", data.dc_protocolo_cancel);
                                if (hasValue(data.ds_protocolo_nfe))
                                    dijit.byId("nrProtocolo").set("value", data.ds_protocolo_nfe);
                            }
                            break;
                        case ENTRADA:
                        case SAIDA:
                            if (hasValue(data.cd_tipo_nota_fiscal)) {
                                dojo.byId("cd_tp_nf").value = data.cd_tipo_nota_fiscal;
                                regime_tributario = data.TipoNF != null ? data.TipoNF.id_regime_tributario : 0;
                                dojo.byId("tpNf").value = data.dc_tipo_nota;
                                dijit.byId('tpNf').reducao = data.pc_reduzido_nf;
                            }
                            if (hasValue(data.dc_cfop_nf))
                                dijit.byId("operacaoCFOP").set("value", data.dc_cfop_nf);
                            if (hasValue(data.nm_cfop))
                                dijit.byId("CFOP").set("value", data.nm_cfop);
                            if (hasValue(data.cd_cfop_nf))
                                dojo.byId("cd_cfop_nf").value = data.cd_cfop_nf;
                            if (hasValue(data.cd_sit_trib_ICMS))
                                dojo.byId("cd_sit_trib_ICMS_tp_nt").value = data.cd_sit_trib_ICMS;
                            dijit.byId("nfEsc").set("checked", data.id_nf_escola);
                            dijit.byId("baseICMS").set("value", data.vl_base_calculo_ICMS_nf);
                            dijit.byId("vl_icms").set("value", data.vl_ICMS_nf);
                            dijit.byId("basePIS").set("value", data.vl_base_calculo_PIS_nf);
                            dijit.byId("vl_pis").set("value", data.vl_PIS_nf);
                            dijit.byId("baseCOFINS").set("value", data.vl_base_calculo_COFINS_nf);
                            dijit.byId("vl_COFINS").set("value", data.vl_COFINS_nf);
                            dijit.byId("baseIPI").set("value", data.vl_base_calculo_IPI_nf);
                            dijit.byId("vl_ipi").set("value", data.vl_IPI_nf);
                            dijit.byId("idObsNF").set("value", data.tx_obs_fiscal);
                            dijit.byId("statusNFS").set("value", data.id_status_nf);
                            if (empresa_propria) {
                                if (hasValue(data.dta_autorizacao_nfe))
                                    dijit.byId("dtaAuto").set("value", data.dta_autorizacao_nfe);
                                if (hasValue(data.dc_key_nfe))
                                    dijit.byId("dc_key_nfe").set("value", data.dc_key_nfe);
                                if (hasValue(data.dc_mensagem_retorno))
                                    dijit.byId("obsResposta").set("value", data.dc_mensagem_retorno);
                                if (hasValue(data.dta_nfe_cancel))
                                    dijit.byId("dtaCancel").set("value", data.dta_nfe_cancel);
                                if (hasValue(data.dc_protocolo_cancel))
                                    dijit.byId("nrProtocCancel").set("value", data.dc_protocolo_cancel);
                                if (hasValue(data.ds_protocolo_nfe))
                                    dijit.byId("nrProtocolo").set("value", data.ds_protocolo_nfe);
                            }
                            gridItem.editItem = data;
                            loadMeioPagamento(data.dc_meio_pagamento);
                            break;
                        case DEVOLUCAO:
                            if (hasValue(data.cd_tipo_nota_fiscal)) {
                                dojo.byId("cd_tp_nf").value = data.cd_tipo_nota_fiscal;
                                dojo.byId("tpNf").value = data.dc_tipo_nota;
                                dijit.byId('tpNf').reducao = data.pc_reduzido_nf;
                            }
                            if (hasValue(data.dc_cfop_nf))
                                dijit.byId("operacaoCFOP").set("value", data.dc_cfop_nf);
                            if (hasValue(data.nm_cfop))
                                dijit.byId("CFOP").set("value", data.nm_cfop);
                            if (hasValue(data.cd_cfop_nf))
                                dojo.byId("cd_cfop_nf").value = data.cd_cfop_nf;
                            if (hasValue(data.cd_sit_trib_ICMS))
                                dojo.byId("cd_sit_trib_ICMS_tp_nt").value = data.cd_sit_trib_ICMS;
                            dijit.byId("nfEsc").set("checked", data.id_nf_escola);
                            dijit.byId("baseICMS").set("value", data.vl_base_calculo_ICMS_nf);
                            dijit.byId("vl_icms").set("value", data.vl_ICMS_nf);
                            dijit.byId("basePIS").set("value", data.vl_base_calculo_PIS_nf);
                            dijit.byId("vl_pis").set("value", data.vl_PIS_nf);
                            dijit.byId("baseCOFINS").set("value", data.vl_base_calculo_COFINS_nf);
                            dijit.byId("vl_COFINS").set("value", data.vl_COFINS_nf);
                            dijit.byId("baseIPI").set("value", data.vl_base_calculo_IPI_nf);
                            dijit.byId("vl_ipi").set("value", data.vl_IPI_nf);
                            dijit.byId("idObsNF").set("value", data.tx_obs_fiscal);
                            dijit.byId("statusNFS").set("value", data.id_status_nf);
                            dojo.byId("cd_nfDev").value = data.cd_nota_fiscal;
                            dojo.byId("id_natureza_movto").value = data.TipoNF != null ? data.TipoNF.id_natureza_movimento : 0;
                            if (empresa_propria) {
                                if (hasValue(data.dta_autorizacao_nfe))
                                    dijit.byId("dtaAuto").set("value", data.dta_autorizacao_nfe);
                                if (hasValue(data.dc_key_nfe))
                                    dijit.byId("dc_key_nfe").set("value", data.dc_key_nfe);
                                if (hasValue(data.dc_mensagem_retorno))
                                    dijit.byId("obsResposta").set("value", data.dc_mensagem_retorno);
                                if (hasValue(data.dta_nfe_cancel))
                                    dijit.byId("dtaCancel").set("value", data.dta_nfe_cancel);
                                if (hasValue(data.dc_protocolo_cancel))
                                    dijit.byId("nrProtocCancel").set("value", data.dc_protocolo_cancel);
                                if (hasValue(data.ds_protocolo_nfe))
                                    dijit.byId("nrProtocolo").set("value", data.ds_protocolo_nfe);
                            }
                            var dcNFDev = '';
                            if (hasValue(data.MovimentoDevolucao)) {
                                if (hasValue(data.MovimentoDevolucao.nm_movimento))
                                    dcNFDev = data.MovimentoDevolucao.nm_movimento;
                                if (hasValue(data.MovimentoDevolucao.dc_serie_movimento))
                                    dcNFDev += '-' + data.MovimentoDevolucao.dc_serie_movimento;
                                dijit.byId("tpNfDev").set("value", dcNFDev);
                                dojo.byId("id_tipo_movimento").value = data.MovimentoDevolucao.id_tipo_movimento;
                            }
                            dojo.byId("id_tipo_movimento").value = data.MovimentoDevolucao.id_tipo_movimento;
                            if (dojo.byId("cd_nfDev").value > 0)
                                dijit.byId("limparNFDev").set("disabled", false);
                            else
                                dijit.byId("limparNFDev").set("disabled", true);

                            gridItem.editItem = data;
                            break;
                    }
                    if (hasValue(data.vl_aproximado))
                        dijit.byId("vl_aproximado").set("value", data.vl_aproximado);
                    if (hasValue(data.pc_aliquota_aproximada))
                        dijit.byId("pc_aliquota_ap").set("value", data.pc_aliquota_aproximada);
                }
                else {
                    if (data.id_tipo_movimento == DEVOLUCAO) {
                        var dcNFDev = '';
                        dojo.byId("cd_nfDev").value = data.cd_nota_fiscal;
                        if (hasValue(data.MovimentoDevolucao)) {
                            if (hasValue(data.MovimentoDevolucao.nm_movimento))
                                dcNFDev = data.MovimentoDevolucao.nm_movimento;
                            if (hasValue(data.MovimentoDevolucao.dc_serie_movimento))
                                dcNFDev += '-' + data.MovimentoDevolucao.dc_serie_movimento;
                            dijit.byId("tpNfDev").set("value", dcNFDev);
                            dojo.byId("id_tipo_movimento").value = data.MovimentoDevolucao.id_tipo_movimento;
                        }
                        dojo.byId("id_natureza_movto").value = data.TipoNF != null ? data.TipoNF.id_natureza_movimento : 0;
                        dojo.byId("id_tipo_movimento").value = data.MovimentoDevolucao.id_tipo_movimento;
                        if (dojo.byId("cd_nfDev").value > 0)
                            dijit.byId("limparNFDev").set("disabled", false);
                        else
                            dijit.byId("limparNFDev").set("disabled", true);
                    }
                }
                dijit.byId("itensT").set("disabled", (data.id_tipo_movimento == DEVOLUCAO))
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function mountDataMovimentoForPost() {
    try {
        var parametrosTela = getParamterosURL();
        
        var itens = null;
        var titulos = null;
        var itensKit = null;

        var gridItem = dijit.byId("gridItem");
        var gridTitulo = dijit.byId("gridTitulo");
        var gridKit = dijit.byId("gridKit");
        
        if (hasValue(gridItem) && gridItem.store.objectStore.data != null)
            itens = gridItem.store.objectStore.data;
        if (hasValue(gridTitulo) && gridTitulo.store.objectStore.data != null)
            titulos = gridTitulo.store.objectStore.data;
        if (hasValue(gridKit) && gridKit.store.objectStore.data != null)
            itensKit = gridKit.store.objectStore.data;

        var retorno = {
            cd_movimento: hasValue(dojo.byId("cd_movimento").value) ? dojo.byId("cd_movimento").value : 0,
            cd_aluno: hasValue(dojo.byId("cdAlunoFKMovimento").value) && parseInt(dojo.byId("cdAlunoFKMovimento").value) > 0 ? dojo.byId("cdAlunoFKMovimento").value : null,
            cd_pessoa: dojo.byId("cdPessoaMvtoCad").value,
            cd_politica_comercial: dojo.byId("cdPoliticaComercial").value,
            cd_tipo_financeiro: dijit.byId("tpFinanceiro").value,
            id_tipo_movimento: TIPOMOVIMENTO,
            nm_movimento: hasValue(dojo.byId("nrMovto").value) ? dojo.byId("nrMovto").value : null,
            dc_serie_movimento: hasValue(dojo.byId("serie").value) ? dojo.byId("serie").value : null,
            dt_emissao_movimento: dojo.date.locale.parse(dojo.byId("dtaEmis").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_vcto_movimento: dojo.date.locale.parse(dojo.byId("dtaVenc").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_mov_movimento: dojo.date.locale.parse(dojo.byId("dtaMovto").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            pc_acrescimo: dijit.byId("pcAcrec").value,
            vl_acrescimo: dijit.byId("vlAc").value,
            pc_desconto: dijit.byId("pcDesconto").value,
            vl_desconto: dijit.byId("vlDesconto").value,
            tx_obs_movimento: dijit.byId("idObs").value,
            no_pessoa: dojo.byId("noPessoaMovto").value,
            ItensMovimento: itens,
            ItemMovimentoKit: itensKit,
            titulos: titulos,
            gerar_titulos: gerar_titulo,
            id_origem_movimento: hasValue(dojo.byId("id_origem_movimento").value) && dojo.byId("id_origem_movimento").value > 0 ? dojo.byId("id_origem_movimento").value : null,
            cd_origem_movimento: hasValue(dojo.byId("cd_origem_movimento").value) && dojo.byId("cd_origem_movimento").value > 0 ? dojo.byId("cd_origem_movimento").value : null,
            cd_curso: hasValue(dojo.byId("cdCursoFKMovimento").value) && parseInt(dojo.byId("cdCursoFKMovimento").value) > 0 ? dojo.byId("cdCursoFKMovimento").value : null,
            id_material_didatico: ((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false,
            id_venda_futura: dijit.byId('ckVendaFuturaCad').checked
        };
        if (retorno.cd_tipo_financeiro == TIPOFINANCEIROCHEQUE) {
            retorno.Cheques = new Array();
            retorno.Cheques.push({
                cd_cheque: hasValue(dojo.byId("cd_cheque").value) ? dojo.byId("cd_cheque").value : 0,
                cd_movimento: hasValue(dojo.byId("cd_movimento").value) ? dojo.byId("cd_movimento").value : 0,
                no_emitente_cheque: dojo.byId("emissorChequeName").value,
                no_agencia_cheque: hasValue(dojo.byId("agencia").value) ? dojo.byId("agencia").value : "",
                nm_agencia_cheque: hasValue(dojo.byId("nroAgencia").value) ? dojo.byId("nroAgencia").value : 0,
                nm_digito_agencia_cheque: hasValue(dojo.byId("dgAgencia").value) ? dojo.byId("dgAgencia").value : "",
                nm_conta_corrente_cheque: hasValue(dojo.byId("nroContaCorrente").value) ? dojo.byId("nroContaCorrente").value : 0,
                nm_digito_cc_cheque: hasValue(dojo.byId("dgContaCorrente").value) ? dojo.byId("dgContaCorrente").value : 0,
                nm_primeiro_cheque: dojo.byId("nroPrimeiroCheque").value,
                cd_banco: dijit.byId("bancoCheque").value
            });
        }
        if (dijit.byId("ckNotaFiscal").checked) {
        switch (TIPOMOVIMENTO) {
            case SERVICO:
                retorno.id_nf = dijit.byId("ckNotaFiscal").get("checked");
                retorno.id_nf_escola = dijit.byId("nfEsc").get("checked");
                retorno.vl_base_calculo_ISS_nf = hasValue(dijit.byId("baseISS").value) ? unmaskFixed(dijit.byId("baseISS").value, 2) : 0;
                retorno.vl_ISS_nf = hasValue(dijit.byId("vl_iss").value) ? unmaskFixed(dijit.byId("vl_iss").value, 2) : 0;
                retorno.cd_cfop_nf = hasValue(dojo.byId("cd_cfop_nf").value) ? dojo.byId("cd_cfop_nf").value : 0;
                retorno.dc_cfop_nf = hasValue(dojo.byId("operacaoCFOP").value) ? dojo.byId("operacaoCFOP").value : 0;
                retorno.tx_obs_fiscal = hasValue(dojo.byId("idObsNF").value) ? dojo.byId("idObsNF").value : "";
                break;
            case ENTRADA:
            case SAIDA:
                retorno.id_nf = dijit.byId("ckNotaFiscal").get("checked");
                retorno.id_nf_escola = dijit.byId("nfEsc").get("checked");
                retorno.cd_cfop_nf = hasValue(dojo.byId("cd_cfop_nf").value) ? dojo.byId("cd_cfop_nf").value : 0;
                retorno.dc_cfop_nf = hasValue(dojo.byId("operacaoCFOP").value) ? dojo.byId("operacaoCFOP").value : 0;
                retorno.vl_base_calculo_ICMS_nf = hasValue(dijit.byId("baseICMS").value) ? unmaskFixed(dijit.byId("baseICMS").value, 2) : 0;
                retorno.vl_base_calculo_COFINS_nf = hasValue(dijit.byId("baseCOFINS").value) ? unmaskFixed(dijit.byId("baseCOFINS").value, 2) : 0;
                retorno.vl_base_calculo_PIS_nf = hasValue(dijit.byId("basePIS").value) ? unmaskFixed(dijit.byId("basePIS").value, 2) : 0;
                retorno.vl_base_calculo_IPI_nf = hasValue(dijit.byId("baseIPI").value) ? unmaskFixed(dijit.byId("baseIPI").value, 2) : 0;
                retorno.vl_ICMS_nf = hasValue(dijit.byId("vl_icms").value) ? unmaskFixed(dijit.byId("vl_icms").value, 2) : 0;
                retorno.vl_PIS_nf = hasValue(dijit.byId("vl_pis").value) ? unmaskFixed(dijit.byId("vl_pis").value, 2) : 0;
                retorno.vl_COFINS_nf = hasValue(dijit.byId("vl_COFINS").value) ? unmaskFixed(dijit.byId("vl_COFINS").value, 2) : 0;
                retorno.vl_IPI_nf = hasValue(dijit.byId("vl_ipi").value) ? unmaskFixed(dijit.byId("vl_ipi").value, 2) : 0;
                retorno.tx_obs_fiscal = hasValue(dojo.byId("idObsNF").value) ? dojo.byId("idObsNF").value : "";
                retorno.dc_key_nfe = dijit.byId("dc_key_nfe").value;
                retorno.dc_meio_pagamento = hasValue(dijit.byId("cad_meio_pagamento").value) ? dijit.byId("cad_meio_pagamento").value : null;
                break;
            case DEVOLUCAO:
                retorno.id_nf = dijit.byId("ckNotaFiscal").get("checked");
                retorno.id_nf_escola = dijit.byId("nfEsc").get("checked");
                retorno.cd_cfop_nf = hasValue(dojo.byId("cd_cfop_nf").value) ? dojo.byId("cd_cfop_nf").value : 0;
                retorno.dc_cfop_nf = hasValue(dojo.byId("operacaoCFOP").value) ? dojo.byId("operacaoCFOP").value : 0;
                retorno.vl_base_calculo_ICMS_nf = hasValue(dijit.byId("baseICMS").value) ? unmaskFixed(dijit.byId("baseICMS").value, 2) : 0;
                retorno.vl_base_calculo_COFINS_nf = hasValue(dijit.byId("baseCOFINS").value) ? unmaskFixed(dijit.byId("baseCOFINS").value, 2) : 0;
                retorno.vl_base_calculo_PIS_nf = hasValue(dijit.byId("basePIS").value) ? unmaskFixed(dijit.byId("basePIS").value, 2) : 0;
                retorno.vl_base_calculo_IPI_nf = hasValue(dijit.byId("baseIPI").value) ? unmaskFixed(dijit.byId("baseIPI").value, 2) : 0;
                retorno.vl_ICMS_nf = hasValue(dijit.byId("vl_icms").value) ? unmaskFixed(dijit.byId("vl_icms").value, 2) : 0;
                retorno.vl_PIS_nf = hasValue(dijit.byId("vl_pis").value) ? unmaskFixed(dijit.byId("vl_pis").value, 2) : 0;
                retorno.vl_COFINS_nf = hasValue(dijit.byId("vl_COFINS").value) ? unmaskFixed(dijit.byId("vl_COFINS").value, 2) : 0;
                retorno.vl_IPI_nf = hasValue(dijit.byId("vl_ipi").value) ? unmaskFixed(dijit.byId("vl_ipi").value, 2) : 0;
                retorno.tx_obs_fiscal = hasValue(dojo.byId("idObsNF").value) ? dojo.byId("idObsNF").value : "";
                retorno.cd_nota_fiscal = parseInt(dojo.byId("cd_nfDev").value)

                break;

        }
        retorno.cd_tipo_nota_fiscal = hasValue(dojo.byId("cd_tp_nf").value) ? parseInt(dojo.byId("cd_tp_nf").value) : 0;
        retorno.vl_aproximado = hasValue(dojo.byId("vl_aproximado").value) ? unmaskFixed(dijit.byId("vl_aproximado").value, 2) : 0;
        retorno.pc_aliquota_aproximada = hasValue(dojo.byId("pc_aliquota_ap").value) ? unmaskFixed(dijit.byId("pc_aliquota_ap").value, 2) : 0;
    }
    else {
            if (TIPOMOVIMENTO == DEVOLUCAO)
                retorno.cd_nota_fiscal = parseInt(dojo.byId("cd_nfDev").value)

    }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparMovimento() {
    try {
        clearForm("formCadMovimento");
        onActiveChangeCamposGerarTitulos(false);
        apresentaMensagem("apresentadorMensagemMovto", null);
        setarTabCadMovimento();
        dojo.byId("cd_movimento").value = 0;
        dojo.byId("cd_item_movimento").value = 0;
        dojo.byId("cd_item").value = 0;
        dojo.byId("cd_cheque").value = 0;
        dojo.byId("cd_plano_contas").value = 0;
        dojo.byId("cdPessoaMvtoCad").value = 0;
        dojo.byId("cdPessoaAlunoFKMovimento").value = 0;
        dojo.byId("cdAlunoFKMovimento").value = 0;
        dojo.byId("pc_aliquota_ap").value = 0;
        dojo.byId("vl_aproximado").value = 0;
        dojo.byId("cdPoliticaComercial").value = 0;

        dijit.byId("dtaEmis").reset();
        dijit.byId("dtaVenc").reset();
        dijit.byId("dtaMovto").reset();
        dijit.byId("noPessoaMovto").reset();
        dijit.byId("noAlunoFKMovimento").reset();

        dijit.byId("dtaVenc").set("value", null);
        dijit.byId("tagtPrincipalMvto").set("open", true);
        dijit.byId("tagItens").set("open", true);
        dijit.byId("tagObs").set("open", false);
        dijit.byId("tagValores").set("open", false);

        dijit.byId("ckNotaFiscal").set("disabled", false);
        //if (TIPOMOVIMENTO == DEVOLUCAO)
        //    dijit.byId("ckNotaFiscalPesq").set("disabled", true);
        //else
        dijit.byId("ckNotaFiscalPesq").set("disabled", false);
        var gridItem = dijit.byId("gridItem");
        var gridTitulo = dijit.byId("gridTitulo");
        var gridKit = dijit.byId("gridKit");

        if (hasValue(gridItem)) {
            gridItem.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridItem.itensSelecionados = [];
        }
        if (hasValue(gridTitulo)) {
            gridTitulo.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridTitulo.itensSelecionados = [];
        }
        if (hasValue(gridKit)) {
            gridKit.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridKit.itensSelecionados = [];
        }

        dijit.byId("tpFinanceiro").set("value", TIPOFINANCEIROTITULO);
        dijit.byId('pcAcrec').set('value', 0);
        dijit.byId('pcAcrec').oldValue = 0;
        dijit.byId('vlAc').set('value', 0);
        dijit.byId('vlAc').oldValue = 0;
        dijit.byId('totalItens').set('value', 0);
        dijit.byId('pcDesconto').set('value', 0);
        dijit.byId('pcDesconto').oldValue = 0;
        dijit.byId('vlDesconto').set('value', 0);
        dijit.byId('vlDesconto').oldValue = 0;
        dijit.byId('totalGeral').set('value', 0);
        dijit.byId("idObs").reset();
        dijit.byId("itensT").set("disabled", (TIPOMOVIMENTO == DEVOLUCAO));

        switch (TIPOMOVIMENTO) {
            case ENTRADA:
                if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').set("checked", true);
                    configuraLayoutNF(true);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }
                else {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').reset();
                    configuraLayoutNF(false);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }
                    if (empresa_propria) {
                        dijit.byId("dtaAuto").reset();
                        dijit.byId("dc_key_nfe").reset();
                        dijit.byId("obsResposta").reset();
                        dijit.byId("dtaCancel").reset();
                        dijit.byId("nrProtocCancel").reset();
                        dijit.byId("nrProtocolo").reset();
                    }
                dijit.byId("cbMovtoFK").set("disabled", true);
                break;
            case SAIDA:
                if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').set("checked", true);
                    configuraLayoutNF(true);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }
                else {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').reset();
                    configuraLayoutNF(false);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }
                dijit.byId("cbMovtoFK").set("disabled", true);
                break;
            case SERVICO:
                if (hasValue(emitir_nf_servico) && emitir_nf_servico) {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').set("checked", true);
                    configuraLayoutNF(true);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                    if (empresa_propria) {
                        dijit.byId("dtaAuto").reset();
                        dijit.byId("nrNFS").reset();
                        dijit.byId("urlNFS").reset();
                        dijit.byId("obsResposta").reset();
                        dijit.byId("dtaCancel").reset();
                        dijit.byId("nrProtocCancel").reset();
                        dijit.byId("nrProtocolo").reset();
                    }
                } else {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').reset();
                    configuraLayoutNF(false);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;

                }
                dijit.byId("cbMovtoFK").set("disabled", false);
                break;
            case DESPESAS:
                configuraLayoutNF(false);
                break;
            case DEVOLUCAO:
                if (hasValue(emitir_nf_mercantil) && emitir_nf_mercantil) {
                dijit.byId("ckNotaFiscal")._onChangeActive = false;
                dijit.byId('ckNotaFiscal').set("checked", true);
                //dijit.byId('ckNotaFiscal').set("disabled", true);
                configuraLayoutNF(true);
                dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }
                else {
                    dijit.byId("ckNotaFiscal")._onChangeActive = false;
                    dijit.byId('ckNotaFiscal').reset();
                    configuraLayoutNF(false);
                    dijit.byId("ckNotaFiscal")._onChangeActive = true;
                }

                dojo.byId('id_natureza_movto').value = 0;
                dijit.byId('limparNFDev').set("disabled", true);
                dijit.byId("cbMovtoFK").reset();
                dijit.byId("cbMovtoFK").set("disabled", false);
                dojo.byId("id_tipo_movimento").value = 0;
        }

        clearForm("formCheque");
        dijit.byId("bancoCheque").reset();
        gridItem.editItem = null;
        //default dos valores de item
        limparItem();
        if (dojo.byId("cd_movimento").value <= 0)
            dojo.addOnLoad(function () {
                try {
                    var compEmis = dijit.byId('dtaEmis');
                    var compMovto = dijit.byId('dtaMovto');
                    compEmis._onChangeActive = false;
                    compMovto._onChangeActive = false;
                    compEmis.attr("value", new Date(ano, mes, dia));
                    compMovto.attr("value", new Date(ano, mes, dia));
                    compEmis._onChangeActive = true;
                    compMovto._onChangeActive = true;
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        onActiveChangeCamposGerarTitulos(true);
        gerar_titulo = false;
        switch (TIPOMOVIMENTO) {
            case SERVICO:
                dojo.byId("cd_tp_nf").value = 0;
                dojo.byId("cd_sit_trib_ICMS_tp_nt").value = 0;
                dojo.byId("tpNf").value = "";
                dijit.byId("CFOP").reset();
                dijit.byId("vl_iss").reset();
                dijit.byId("baseISS").reset();
                dijit.byId("nfEsc").reset();
                dijit.byId("pc_aliquota_ap").set("value", 0);
                dijit.byId("vl_aproximado").set("value", 0);
                break;
            case ENTRADA:
                dojo.byId("cd_tp_nf").value = 0;
                dojo.byId("cd_sit_trib_ICMS_tp_nt").value = 0;
                dojo.byId("tpNf").value = "";
                dijit.byId("nfEsc").reset();
                dijit.byId("nfEsc").set("checked", false);
                dijit.byId("operacaoCFOP").reset();
                dijit.byId("CFOP").reset();
                dijit.byId("baseICMS").set("value", 0);
                dijit.byId("vl_icms").set("value", 0);
                dijit.byId("basePIS").set("value", 0);
                dijit.byId("vl_pis").set("value", 0);
                dijit.byId("baseCOFINS").set("value", 0);
                dijit.byId("vl_COFINS").set("value", 0);
                dijit.byId("baseIPI").set("value", 0);
                dijit.byId("vl_ipi").set("value", 0);
                dijit.byId("pc_aliquota_ap").set("value", 0);
                dijit.byId("vl_aproximado").set("value", 0);
                break;
            case SAIDA:
            case DEVOLUCAO:
                dojo.byId("cd_tp_nf").value = 0;
                dojo.byId("cd_sit_trib_ICMS_tp_nt").value = 0;
                dojo.byId("tpNf").value = "";
                dijit.byId("nfEsc").reset();
                dijit.byId("operacaoCFOP").reset();
                dijit.byId("CFOP").reset();
                dijit.byId("baseICMS").set("value", 0);
                dijit.byId("vl_icms").set("value", 0);
                dijit.byId("basePIS").set("value", 0);
                dijit.byId("vl_pis").set("value", 0);
                dijit.byId("baseCOFINS").set("value", 0);
                dijit.byId("vl_COFINS").set("value", 0);
                dijit.byId("baseIPI").set("value", 0);
                dijit.byId("vl_ipi").set("value", 0);
                dijit.byId("pc_aliquota_ap").set("value", 0);
                dijit.byId("vl_aproximado").set("value", 0);
                break;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarMovimento(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dojo/domReady!",
          "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var parametrosTela = getParamterosURL();
            var grid = dijit.byId("gridMovimento");
            var cdPessoaPesq = hasValue(dojo.byId("cdPessoaPesq").value) ? dojo.byId("cdPessoaPesq").value : 0;
            var cdItem = hasValue(dojo.byId("cdItem").value) ? dojo.byId("cdItem").value : 0;
            var cdPlanoContaPesq = hasValue(dojo.byId("cdPlanoContaPesq").value) ? dojo.byId("cdPlanoContaPesq").value : 0;
            var numeroPesq = hasValue(dojo.byId("numeroPesq").value) ? dojo.byId("numeroPesq").value : 0;
            var serie = hasValue(dojo.byId("numeroSeriePesq").value) ? dojo.byId("numeroSeriePesq").value : "";
            var myStore =
                Cache(
                        JsonRest({
                            target: Endereco() + "/api/fiscal/getMovimentoSearch?id_tipo_movimento=" + parseInt(TIPOMOVIMENTO) + "&cd_pessoa=" + (hasValue(dijit.byId("noPessoaPesq").value) ? parseInt(cdPessoaPesq) : 0) +
                            "&cd_item=" + (hasValue(dojo.byId("noItemPesq").value) ? parseInt(cdItem) : 0) + "&cd_plano_conta=" + parseInt(cdPlanoContaPesq) + "&numero=" + parseInt(numeroPesq) + "&serie=" + serie +
                            "&emissao=" + document.getElementById("ckEmissao").checked + "&movimento=" + document.getElementById("ckMovimento").checked +
                            "&dtInicial=" + dojo.byId("dtaInicial").value + "&dtFinal=" + dojo.byId("dtaFinal").value + "&nota_fiscal=" + document.getElementById("ckNotaFiscalPesq").checked + "&statusNF=" + dijit.byId("statusNFPesq").value +
                                "&isImportXML=" + (TIPOMOVIMENTO != ENTRADA ? 0 : (dijit.byId("ckImportNota").checked) ? 1 : 0) +
                                "&id_material_didatico=" + (((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) && eval(parametrosTela['id_material_didatico']) == 1) ? true : false) +
                                "&id_venda_futura=" + dijit.byId('ckVendaFutura').checked,
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                grid.itensSelecionados = [];
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function validarMovimentoCadastro() {
    try {
        var gridTitulos = dijit.byId("gridTitulo");
        var gridItens = dijit.byId("gridItem");
        var retorno = true;

        

        if (!dijit.byId("formCadMovimento").validate()) {
            setarTabCadMovimento();
            retorno = false;
        }

        if (empresa_propria && TIPOMOVIMENTO == ENTRADA) {
            if (!dijit.byId("dc_key_nfe").validate()) {
                setarTabCadFiscal();
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "A chave de acesso é obrigatória.");
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                retorno = false;
            }
        }

        var parametrosTela = getParamterosURL();
        if ((TIPOMOVIMENTO == SAIDA && (parametrosTela['id_material_didatico'] != null &&
                parametrosTela['id_material_didatico'] != undefined) &&
            eval(parametrosTela['id_material_didatico']) == 1)) {

            if (!hasValue(dojo.byId("noAlunoFKMovimento").value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgAlunoNotFoundSaveMovimentoSaidaMaterial);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                retorno = false;
            } else if (!hasValue(dojo.byId("noCursoFKMovimento").value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgCursoNotFoundSaveMovimentoSaidaMaterial);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                retorno = false;
            } else if (!hasValue(dijit.byId("tpContrato").value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgContratoNotFoundSaveMovimentoSaidaMaterial);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                retorno = false;
            }

            var qtd = validateItensGreaterThanOneMaterialDidatico();
            if (qtd <= 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgErrorSaveMovimentoMaterialSemItemMaterialDidatico);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                retorno = false;
            }
            //Não consegue colocar mais de um
        //    else if (qtd > 1) {
        //        var mensagensWeb = new Array();
        //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgErrorSaveMovimentoMaterialMaisDeUmItemMaterialDidatico);
        //        apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
        //        retorno = false;
        //    }
        }

        if (dijit.byId("tpFinanceiro").value == TIPOFINANCEIROCHEQUE)
            if (!dijit.byId("formCheque").validate()) {
                setarTabCadMovimento();
                dijit.byId("tagCheque").set("open", true);
                retorno = false;
            }

        if (hasValue(gridTitulos) && gridTitulos.store.objectStore.data.length > 0) {
            var vlTotalGeralTitulos = 0;
            var totalGeral = dijit.byId("totalGeral").value;
            $.each(gridTitulos.store.objectStore.data, function (index, value) {
                vlTotalGeralTitulos += value.vl_titulo;
            });
            if (unmaskFixed(vlTotalGeralTitulos, 2) != unmaskFixed(totalGeral, 2)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorTotalTitulosDifTotalItens);
                apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                retorno = false;
            }
        }
        if (obrigar_plano_conta && hasValue(gridItens) && gridItens.store.objectStore.data.length > 0)
            $.each(gridItens.store.objectStore.data, function (index, value) {
                if (!hasValue(value.cd_plano_conta) && value.cd_plano_conta == null || (value.cd_plano_conta == 0)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorObrigPlanoConta);
                    apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                    retorno = false;
                    return false;
                }
            });
        if (dijit.byId("ckNotaFiscal").checked)
            if (!dijit.byId("formFiscalMovto").validate())
                return false;
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesMovimento(value, grid, ehLink, id_material_didatico) {
    try {
        limparMovimento();
        showCarregando();
        setarTabCadMovimento();
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

        if (value != null && value.cd_movimento > 0)
            showEditMovimento(value.cd_movimento, value.id_nf, id_material_didatico);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarMovimento() {
    try {
        var movimento = null;
        if (!gerar_titulo && !validarMovimentoCadastro()) {
            return false;
        }
        var mensagensWeb = new Array();
        
        apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);

        if (!dijit.byId("ckNotaFiscal").checked || dijit.byId("statusNFS").value == 2 || (dijit.byId("ckNotaFiscal").checked && TIPOMOVIMENTO != ENTRADA)) {
            showCarregando();
            criarAtualizarTitulo(function () {
                if (!validarMovimentoCadastro()) {
                    return false;
                }
                showCarregando();
                movimento = mountDataMovimentoForPost();
                dojo.xhr.post({
                    url: Endereco() + "/api/escola/postInsertMovimento",
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    postData: JSON.stringify(movimento)
                }).then(function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridMovimento';
                            var grid = dijit.byId(gridName);

                            apresentaMensagem('apresentadorMensagem', data);
                            data = data.retorno;

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];
                            insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                            buscarItensSelecionados(gridName, 'selecionadoMovto', 'cd_movimento', 'selecionaTodos', ['pesquisarMovto', 'relatorioMovto'], 'todosItens');
                            grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                            if (hasValue(itemAlterado) && hasValue(itemAlterado.cd_movimento))
                                setGridPagination(grid, itemAlterado, "cd_movimento");
                            showCarregando();
                            dijit.byId("cadMovimento").hide();
                        } else {
                            apresentaMensagem('apresentadorMensagemMovto', data);
                            showCarregando();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem('apresentadorMensagemMovto', error);
                    showCarregando();
                });
            });
        } else {
            if (!validarMovimentoCadastro()) {
                return false;
            }
            showCarregando();
            movimento = mountDataMovimentoForPost();
            dojo.xhr.post({
                url: Endereco() + "/api/escola/postInsertMovimento",
                handleAs: "json",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify(movimento)
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridMovimento';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        data = data.retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoMovto', 'cd_movimento', 'selecionaTodos', ['pesquisarMovto', 'relatorioMovto'], 'todosItens');
                        grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                        if (hasValue(itemAlterado) && hasValue(itemAlterado.cd_movimento))
                            setGridPagination(grid, itemAlterado, "cd_movimento");
                        showCarregando();
                        dijit.byId("cadMovimento").hide();
                    } else {
                        apresentaMensagem('apresentadorMensagemMovto', data);
                        showCarregando();
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagemMovto', error);
                showCarregando();
            });
        }
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarMovimento() {
    try {
        var movimento = null;
        if (!validarMovimentoCadastro()) {
            return false;
        }
        showCarregando();
        if (!dijit.byId("ckNotaFiscal").checked || dijit.byId("statusNFS").value == 2 || (dijit.byId("ckNotaFiscal").checked && TIPOMOVIMENTO != ENTRADA)) {
            criarAtualizarTitulo(function() {
                if (!validarMovimentoCadastro()) {
                    return false;
                }
                showCarregando();
                movimento = mountDataMovimentoForPost();
                dojo.xhr.post({
                    url: Endereco() + "/api/escola/postUpdateMovimento",
                    handleAs: "json",
                    preventCache: true,
                    headers: {
                        "Accept": "application/json",
                        "Content-Type": "application/json",
                        "Authorization": Token()
                    },
                    postData: JSON.stringify(movimento)
                }).then(function(data) {
                        try {
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridMovimento';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                if (!hasValue(grid.itensSelecionados))
                                    grid.itensSelecionados = [];
                                removeObjSort(grid.itensSelecionados, "cd_movimento", dojo.byId("cd_movimento").value);
                                insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                                buscarItensSelecionados(gridName,
                                    'selecionaTodos',
                                    'cd_movimento',
                                    'selecionaTodos',
                                    ['pesquisarMovto', 'relatorioMovto'],
                                    'todosItens');
                                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_movimento");
                                showCarregando();
                                dijit.byId("cadMovimento").hide();
                            } else {
                                apresentaMensagem('apresentadorMensagemMovto', data);
                                showCarregando();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function(error) {
                        apresentaMensagem('apresentadorMensagemMovto', error);
                        showCarregando();
                    });
            });
        } else {
            if (!validarMovimentoCadastro()) {
                return false;
            }
            //showCarregando();
            movimento = mountDataMovimentoForPost();
            dojo.xhr.post({
                url: Endereco() + "/api/escola/postUpdateMovimento",
                handleAs: "json",
                preventCache: true,
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                },
                postData: JSON.stringify(movimento)
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridMovimento';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        removeObjSort(grid.itensSelecionados, "cd_movimento", dojo.byId("cd_movimento").value);
                        insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                        buscarItensSelecionados(gridName,
                            'selecionaTodos',
                            'cd_movimento',
                            'selecionaTodos',
                            ['pesquisarMovto', 'relatorioMovto'],
                            'todosItens');
                        grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_movimento");
                        showCarregando();
                        dijit.byId("cadMovimento").hide();
                    } else {
                        apresentaMensagem('apresentadorMensagemMovto', data);
                        showCarregando();
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemMovto', error);
                    showCarregando();
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarMovimentos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId("cd_movimento").value != 0)
                itensSelecionados = [{
                    cd_movimento: dojo.byId("cd_movimento").value
                }];
        dojo.xhr.post({
            url: Endereco() + "/api/fiscal/postDeleteMovimentos",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridMovimento').itensSelecionados, "cd_movimento", itensSelecionados[r].cd_movimento);
                pesquisarMovimento(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarMovto").set('disabled', false);
                dijit.byId("relatorioMovto").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadMovimento").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadMovimento").style.display))
                apresentaMensagem('apresentadorMensagemMovto', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarMovimento(item, id_material_didatico) {
    try {
        destroyCreateGridTitulos();
        keepValuesMovimento(item, dijit.byId('gridMovimento'), false, id_material_didatico);
        IncluirAlterar(0, 'divAlterarMovto', 'divIncluirMovto', 'divExcluirMovto', 'apresentadorMensagemMovto', 'divCancelarMovto', 'divClearMovto');
        dijit.byId("cadMovimento").show();
        if (id_material_didatico)
            dojo.byId("trFuturaCad").style.display = "";
        else
            dojo.byId("trFuturaCad").style.display = "none";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMovimento(itensSelecionados, id_material_didatico) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridMovimento = dijit.byId('gridMovimento');
            apresentaMensagem('apresentadorMensagem', '');
            keepValuesMovimento(null, gridMovimento, true, id_material_didatico);
            IncluirAlterar(0, 'divAlterarMovto', 'divIncluirMovto', 'divExcluirMovto', 'apresentadorMensagemMovto', 'divCancelarMovto', 'divClearMovto');
            dijit.byId("cadMovimento").show();
            if (id_material_didatico)
                dojo.byId("trFuturaCad").style.display = "";
            else
                dojo.byId("trFuturaCad").style.display = "none";
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverMovimentos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarMovimentos(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoGerarXML(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else
            if (!hasValue(dijit.byId("cad_meio_pagamento").value) && TIPOMOVIMENTO == SAIDA)
                caixaDialogo(DIALOGO_AVISO, 'Favor informar meio de pagamento', null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            var mensagensWeb = new Array();
            /* Não pode fazer isso no front end, pois o objeto está vindo desatualizado na edição.
            if (hasValue(itensSelecionados[0].id_nf) && !itensSelecionados[0].id_nf) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroGerarXMLTerceiro);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }
            if (hasValue(itensSelecionados[0].id_status_nf) && itensSelecionados[0].id_status_nf == NF_ABERTO) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEmitirXMLNFAberta);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }*/

            gerarXML(itensSelecionados);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarXML(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    var id_tipo_movimento = itensSelecionados[0].id_tipo_movimento;

    dojo.xhr.get({
        url: Endereco() + "/fiscal/postGerarXML?cd_movimento=" + cd_movimento + "&id_tipo_movimento=" + id_tipo_movimento,
        sync: true,
        headers: { "Accept": "application/xml", "Content-Type": "application/xml", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (hasValue(data.erro))
                apresentaMensagem('apresentadorMensagem', data);
            else
                window.open(data.retorno);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function eventoProcessarMovimento(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else
            if (TIPOMOVIMENTO == SAIDA && (!hasValue(itensSelecionados[0]['dc_meio_pagamento'])))
                caixaDialogo(DIALOGO_AVISO, 'Favor informar meio de pagamento', null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmProcessamento, function () { processarMovimento(itensSelecionados) });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function processarMovimento(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    var id_tipo_movimento = itensSelecionados[0].id_tipo_movimento;
    showCarregando();
    dojo.xhr.post({
        url: Endereco() + "/escola/postGerarTitulosProcessarNF",
        handleAs: "json",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify({
            cd_movimento:cd_movimento,
            id_tipo_movimento: id_tipo_movimento
        })
    }).then(function (dataMovimento) {
            var ret = dataMovimento;
            if (hasValue(ret.retorno)) {
                var movimento = ret.retorno;

                dojo.xhr.post({
                    url: Endereco() + "/escola/postProcessarNF",
                    headers: {
                        "Accept": "application/json",
                        "Content-Type": "application/json",
                        "Authorization": Token()
                    },
                    handleAs: "json",
                    postData: JSON.stringify({
                        cd_movimento: movimento.cd_movimento,
                        cd_pessoa_empresa: movimento.cd_pessoa_empresa,
                        cd_pessoa: movimento.cd_pessoa,
                        cd_politica_comercial: movimento.cd_politica_comercial,
                        cd_tipo_financeiro: movimento.cd_tipo_financeiro,
                        id_tipo_movimento: movimento.id_tipo_movimento,
                        nm_movimento: movimento.nm_movimento,
                        dc_serie_movimento: movimento.dc_serie_movimento,
                        dt_emissao_movimento: movimento.dt_emissao_movimento,
                        dt_vcto_movimento: movimento.dt_vcto_movimento,
                        dt_mov_movimento: movimento.dt_mov_movimento,
                        pc_acrescimo: movimento.pc_acrescimo,
                        vl_acrescimo: movimento.vl_acrescimo,
                        pc_desconto: movimento.pc_desconto,
                        vl_desconto: movimento.vl_desconto,
                        tx_obs_movimento: movimento.tx_obs_movimento,
                        id_nf: movimento.id_nf,
                        id_nf_escola: movimento.id_nf_escola,
                        cd_tipo_nota_fiscal: movimento.cd_tipo_nota_fiscal,
                        id_status_nf: movimento.id_status_nf,
                        vl_base_calculo_ICMS_nf: movimento.vl_base_calculo_ICMS_nf,
                        vl_base_calculo_PIS_nf: movimento.vl_base_calculo_PIS_nf,
                        vl_base_calculo_COFINS_nf: movimento.vl_base_calculo_COFINS_nf,
                        vl_base_calculo_IPI_nf: movimento.vl_base_calculo_IPI_nf,
                        vl_base_calculo_ISS_nf: movimento.vl_base_calculo_ISS_nf,
                        vl_ICMS_nf: movimento.vl_ICMS_nf,
                        vl_PIS_nf: movimento.vl_PIS_nf,
                        vl_COFINS_nf: movimento.vl_COFINS_nf,
                        vl_IPI_nf: movimento.vl_IPI_nf,
                        vl_ISS_nf: movimento.vl_ISS_nf,
                        tx_obs_fiscal: movimento.tx_obs_fiscal,
                        dc_justificativa_nf: movimento.dc_justificativa_nf,
                        dc_cfop_nf: movimento.dc_cfop_nf,
                        id_origem_movimento: movimento.id_origem_movimento,
                        cd_origem_movimento: movimento.cd_origem_movimento,
                        cd_nota_fiscal: movimento.cd_nota_fiscal,
                        cd_cfop_nf: movimento.cd_cfop_nf,
                        pc_aliquota_aproximada: movimento.pc_aliquota_aproximada,
                        vl_aproximado: movimento.vl_aproximado,
                        nm_nfe: movimento.nm_nfe,
                        ds_protocolo_nfe: movimento.ds_protocolo_nfe,
                        dt_autorizacao_nfe: movimento.dt_autorizacao_nfe,
                        dt_nfe_cancel: movimento.dt_nfe_cancel,
                        dc_key_nfe: movimento.dc_key_nfe,
                        dc_url_nf: movimento.dc_url_nf,
                        dc_mensagem_retorno: movimento.dc_mensagem_retorno,
                        dc_protocolo_cancel: movimento.dc_protocolo_cancel,
                        cd_aluno: movimento.cd_aluno,
                        id_exportado: movimento.id_exportado,
                        dc_meio_pagamento: movimento.dc_meio_pagamento,
                        id_importacao_xml: movimento.id_importacao_xml,
                        titulos: montarTitulosProcessarNF(movimento.titulos),//movimento.titulos,
                        ItensMovimento: movimento.ItensMovimento,
                        ItemMovimentoKit: movimento.ItemMovimentoKit
                        //Pessoa: { no_pessoa: movimento.Pessoa.no_pessoa}

            })
                }).then(function(data) {
                    try {

                        if ((data.indexOf == undefined)) {
                            //data = jQuery.parseJSON(data);
                            if (data.erro == undefined) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridMovimento';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                if (!hasValue(grid.itensSelecionados))
                                    grid.itensSelecionados = [];
                                removeObjSort(grid.itensSelecionados, "cd_movimento", data.retorno.cd_movimento);
                                insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_movimento', 'selecionaTodos', ['pesquisarMovto', 'relatorioMovto'], 'todosItens');
                                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_movimento");
                            }
                            var parametros = getParamterosURL();
                            var tipo = parametros['tipo'];
                            apresentaMensagem('apresentadorMensagem', data);
                            //LBM Liberei estas linhas em 11/03/2022 pois estava dando erro neste relatorio de emitirNF
                            //if (!empresa_propria && (tipo == SAIDA || tipo == SERVICO) && !hasValue(data.erro))
                            //    window.open(data.retorno.url_relatorio);
                            var gridMvto = dijit.byId("gridMovimento");
                            gridMvto.itensSelecionados[0].id_status_nf = STATUS_NF_FECHADO;
                            gridMvto.itensSelecionados[0].status_nf_pesq = 'Fechada';
                            gridMvto.update();
                        }
                        else if (data.indexOf != undefined && data.indexOf('<META HTTP-EQUIV=\"Pragma\" CONTENT="no-cache">') < 0) {
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridMovimento';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                if (!hasValue(grid.itensSelecionados))
                                    grid.itensSelecionados = [];
                                removeObjSort(grid.itensSelecionados, "cd_movimento", data.retorno.cd_movimento);
                                insertObjSort(grid.itensSelecionados, "cd_movimento", itemAlterado);
                                buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_movimento', 'selecionaTodos', ['pesquisarMovto', 'relatorioMovto'], 'todosItens');
                                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_movimento");
                            }
                            var parametros = getParamterosURL();
                            var tipo = parametros['tipo'];
                            apresentaMensagem('apresentadorMensagem', data);
                            if (!empresa_propria && (tipo == SAIDA || tipo == SERVICO) && !hasValue(data.erro))
                                window.open(data.retorno.url_relatorio);
                            var gridMvto = dijit.byId("gridMovimento");
                            gridMvto.itensSelecionados[0].id_status_nf = STATUS_NF_FECHADO;
                            gridMvto.itensSelecionados[0].status_nf_pesq = 'Fechada';
                            gridMvto.update();
                        }
                        else {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada2);
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        }
                        showCarregando();
                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }

                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', error);
                });
            }

        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagem', error);
        });
    

}

function montarTitulosProcessarNF(titulos) {
    try {
        var titulosResult = [];
        if (hasValue(titulos) && titulos.length > 0) {
            $.each(titulos, function (index, value) {
                var tituloValue = criaTituloProcessarNF(value);
                if (hasValue(tituloValue)) {
                    titulosResult.push(tituloValue);
                }
                
            });
        }

        if (hasValue(titulosResult) && titulosResult.length > 0) {
            return titulosResult;
        } else {
            return null;
        }

    } catch (e) {

    } 
}

function criaTituloProcessarNF(titulo) {
    if (hasValue(titulo)) {
        return {
            cd_titulo: titulo.cd_titulo,
            cd_pessoa_empresa: titulo.cd_pessoa_empresa,
            cd_pessoa_titulo: titulo.cd_pessoa_titulo,
            cd_pessoa_responsavel: titulo.cd_pessoa_responsavel,
            cd_local_movto: titulo.cd_local_movto,
            dt_emissao_titulo: titulo.dt_emissao_titulo,
            cd_origem_titulo: titulo.cd_origem_titulo,
            dt_vcto_titulo: titulo.dt_vcto_titulo,
            dh_cadastro_titulo: titulo.dh_cadastro_titulo,
            vl_titulo: titulo.vl_titulo,
            dt_liquidacao_titulo: titulo.dt_liquidacao_titulo,
            dc_codigo_barra: titulo.dc_codigo_barra,
            dc_tipo_titulo: titulo.dc_tipo_titulo,
            dc_nosso_numero: titulo.dc_nosso_numero,
            dc_num_documento_titulo: titulo.dc_num_documento_titulo,
            vl_saldo_titulo: titulo.vl_saldo_titulo,
            nm_titulo: titulo.nm_titulo,
            nm_parcela_titulo: titulo.nm_parcela_titulo,
            cd_tipo_financeiro: titulo.cd_tipo_financeiro,
            id_status_titulo: titulo.id_status_titulo,
            id_status_cnab: titulo.id_status_cnab,
            id_origem_titulo: titulo.id_origem_titulo,
            id_natureza_titulo: titulo.id_natureza_titulo,
            vl_multa_titulo: titulo.vl_multa_titulo,
            vl_juros_titulo: titulo.vl_juros_titulo,
            vl_desconto_titulo: titulo.vl_desconto_titulo,
            vl_liquidacao_titulo: titulo.vl_liquidacao_titulo,
            vl_multa_liquidada: titulo.vl_multa_liquidada,
            vl_juros_liquidado: titulo.vl_juros_liquidado,
            vl_desconto_juros: titulo.vl_desconto_juros,
            vl_desconto_multa: titulo.vl_desconto_multa,
            pc_juros_titulo: titulo.pc_juros_titulo,
            pc_multa_titulo: titulo.pc_multa_titulo,
            cd_plano_conta_tit: titulo.cd_plano_conta_tit,
            vl_material_titulo: titulo.vl_material_titulo,
            vl_abatimento: titulo.vl_abatimento,
            vl_desconto_contrato: titulo.vl_desconto_contrato,
            pc_taxa_cartao: titulo.pc_taxa_cartao,
            nm_dias_cartao: titulo.nm_dias_cartao
        }
    } else {
        return null;
    }

    
}

function eventoCancelarMovimentos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            apresentaMensagem('apresentadorMensagemCancelamentoNF', '');
            var mensagensWeb = new Array();
            verificarCancelamentoMovimento(itensSelecionados);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoReenviarMasterSafMovimento(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            apresentaMensagem('apresentadorMensagemCancelamentoNF', '');
            reenviarMasterSafMovimento(itensSelecionados);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarCancelamentoMovimento(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    dojo.xhr.get({
        url: Endereco() + "/api/escola/postVerificarCancelamentoNF?cd_movimento=" + cd_movimento,
        sync: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        apresentaMensagem('apresentadorMensagem', data);

        var parametros = getParamterosURL();
        var tipo = parametros['tipo'];

        if (tipo == DEVOLUCAO)
            tipo = itensSelecionados[0].id_tipo_mvto_nf_dev;
        if (tipo == ENTRADA)
            cancelarMovimento(dijit.byId('gridMovimento').itensSelecionados, tipo);
        else {

            //Saída
            if (!hasValue(dijit.byId('fecharCancelamentoNF')))
                montaCancelamentoNF();
            limparCancelamentoNF();
            dijit.byId("cadCancelamentoNF").show();
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function reenviarMasterSafMovimento(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    dojo.xhr.get({
        url: Endereco() + "/api/escola/postReenviarMovimentoParaMasterSaf?cd_movimento=" + cd_movimento + "&id_tipo_movimento=" + TIPOMOVIMENTO,
        sync: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        apresentaMensagem('apresentadorMensagem', data);
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function montaCancelamentoNF() {
    require([
        "dojo/_base/xhr",
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dojo/dom-attr",
        "dijit/form/Button",
        "dojo/ready",
        "dojo/on",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dijit/form/FilteringSelect",
        "dojo/_base/array"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, domAttr, Button, ready, on, ready, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, array) {
        ready(function () {
            try {
                new Button({
                    label: "Cancelar Nota Fiscal", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        cancelarMovimento(dijit.byId('gridMovimento').itensSelecionados, null);
                    }
                }, "cancelarNF");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadCancelamentoNF").hide(); } }, "fecharCancelamentoNF");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function limparCancelamentoNF() {
    dijit.byId("dc_justificativa_nf").reset();
}

function cancelarMovimento(itensSelecionados, tipo) {
    try {
        if (tipo == ENTRADA)
            postCancelaNF(itensSelecionados);
        else if ((hasValue(dojo.byId('dc_justificativa_nf').value) && dojo.byId('dc_justificativa_nf').value.length > 15)) {
            caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmarCancelamentoNF, function executaRetorno() {
                postCancelaNF(itensSelecionados);
            });
        }
        else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTamanhoJustificativaCancelamentoNF);
            apresentaMensagem("apresentadorMensagemCancelamentoNF", mensagensWeb);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function postCancelaNF(itensSelecionados) {
    var cd_movimento = itensSelecionados[0].cd_movimento;
    var tipo = TIPOMOVIMENTO;
    if (TIPOMOVIMENTO == DEVOLUCAO)
        tipo = itensSelecionados[0].id_tipo_mvto_nf_dev;
    var movimento = {
        cd_movimento: cd_movimento,
        dc_justificativa_nf: dojo.byId('dc_justificativa_nf').value,
        id_tipo_movimento: tipo
    };
    dojo.xhr.post({
        url: Endereco() + "/api/escola/postCancelarNF",
        handleAs: "json",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify(movimento)
    }).then(function (data) {
        if (tipo == SAIDA)
            apresentaMensagem('apresentadorMensagemCancelamentoNF', data);
        else //Entrada
            apresentaMensagem('apresentadorMensagem', data);
        var gridMvto = dijit.byId("gridMovimento");
        gridMvto.itensSelecionados[0].id_status_nf = STATUS_NF_CANCELADO;
        gridMvto.itensSelecionados[0].status_nf_pesq = 'Cancelada';
        gridMvto.update();
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemCancelamentoNF', error);
    });
}

function limparTagFiscalItem() {
    dijit.byId("pc_aliquota_ap_item").set("value", 0);
    dijit.byId("vl_aproximado_item").set("value", 0);
    //Produto
    dijit.byId("sitTribItem").reset();
    dijit.byId("baseCalcICMSItem").set("value", 0);
    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
    dijit.byId("aliquotaICMSItem").set("value", 0);
    dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
    dijit.byId("valorICMSItem").set("value", 0);
    dijit.byId("baseCalcPISItem").set("value", 0);
    dijit.byId("aliquotaPISItem").set("value", 0);
    dijit.byId("valorPISItem").set("value", 0);
    dijit.byId("baseCalcCOFINSItem").set("value", 0);
    dijit.byId("aliquotaCOFINSItem").set("value", 0);
    dijit.byId("valorCOFINSItem").set("value", 0);
    dijit.byId("baseCalcIPIItem").set("value", 0);
    dijit.byId("aliquotaIPIItem").set("value", 0);
    dijit.byId("valorIPIItem").set("value", 0);
    //serviço 

    dijit.byId("baseCalcISSItem")._onChangeActive = false;
    dijit.byId("baseCalcISSItem").set("value", 0);
    dijit.byId("baseCalcISSItem")._onChangeActive = true;
    dijit.byId("pc_aliquota_ap_item")._onChangeActive = false;
    dijit.byId("pc_aliquota_ap_item").set("value", 0);
    dijit.byId("pc_aliquota_ap_item")._onChangeActive = true;
    dijit.byId("vl_aproximado_item").set("value", 0);

    dijit.byId("aliquotaISSItem")._onChangeActive = false;
    dijit.byId("aliquotaISSItem").set("value", 0);
    dijit.byId("aliquotaISSItem")._onChangeActive = true;

    dijit.byId("valorISSItem")._onChangeActive = false;
    dijit.byId("valorISSItem").set("value", 0);
    dijit.byId("valorISSItem")._onChangeActive = true;
    dijit.byId("tagIpi").set("open", false);
    dijit.byId("tagCof").set("open", false);
    dijit.byId("tagPis").set("open", false);
}

//Fim

//Metodos Titulo

function criarGradeTitulos(titulos) {
    try {
        var data = null;
        if (titulos != null)
            data = titulos;
        //Grade de titulo
        var gridTitulo = dojox.grid.EnhancedGrid({
            store: dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }),
            structure:
            [
                { name: "<input id='selecionaTodosTit' style='display:none'/>", field: "selecionadoTit", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTit },
                 { name: "TP", field: "dc_tipo_titulo", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", id: 'iTP' },
                 { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:40px;text-align:center;" },
                 { name: "Tipo Financeiro", field: "tipoDoc", width: "8%", styles: "text-align:center;min-width:50px;" },
                 { name: "Nro Doc.", field: "dc_num_documento_titulo", width: "6%", styles: "min-width:60px;" },
                 { name: "Emissão", field: "dt_emissao", width: "7%", styles: "text-align:center;min-width:60px; max-width:60px;" },
                 { name: "Vencimento", field: "dt_vcto", width: "7%", styles: "text-align:center;min-width:60px; max-width:60px;" },
                 { name: "Emitido CNAB", field: "id_emitido_CNAB", width: "7%", styles: "text-align:center;min-width:60px; max-width:60px;", formatter: formatCheckEmitidoCNAB },
                 { name: "Valor", field: "vlTitulo", width: "6%", styles: "text-align:right; min-width:60px;" },
	             { name: "Taxa Cartão", field: "vl_taxa_cartao", width: "6%", styles: "text-align:right; min-width:60px;", formatter: formatTextVlTaxaCartaoMovimento},
                 { name: "Saldo", field: "vlSaldoTitulo", width: "6%", styles: "text-align:right; min-width:60px;" },
                 { name: "Responsável", field: "nomeResponsavel", width: "15%", styles: "min-width:70px;" },
                 { name: "Local de Movimento", field: "descLocalMovto", width: "25%", styles: "min-width:70px;" }
            ],
            canSort: true,
            noDataMessage: "Nenhum registro encontrado.",
            plugins: {
                pagination: {
                    pageSizes: ["11", "22", "44", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "11",
                    gotoButton: true,
                    /*page step to be displayed*/
                    maxPageStep: 7,
                    /*position of the pagination bar*/
                    position: "button",
                    plugins: { nestedSorting: true }
                }
            }
        }, "gridTitulo");
        gridTitulo.canSort = function (col) { return false };
        gridTitulo.startup();
        gridTitulo.itemSelecionado = [];
        gridTitulo.on("RowClick", function (evt) {
            try {
                var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                if (hasValue(gridTitulo.itemSelecionado) && gridTitulo.itemSelecionado.cd_titulo == item.cd_titulo)
                    return false;
                if (!item.possuiBaixa) {
                    gridTitulo.itemSelecionado = item;
                    limparGridBaixas();
                    return false;
                }
                showCarregando();
                gridTitulo.itemSelecionado = item;
                dijit.byId("gridBaixa").itenSelecionado = item;
                buscarBaixasTitulos(item);
            } catch (e) {
                postGerarLog(e);
            }
        }, true);
        gridTitulo.on("RowDblClick", function (evt) {
            var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
            if (item.vl_liquidacao_titulo > 0 || (hasValue(item.possuiBaixa) && item.possuiBaixa)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloBaixado);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                return false;
            }
            apresentaMensagem('apresentadorMensagem', '');
            //gridTitulo.itemSelecionado = item;
            
            keepValuesTitulo(item, gridTitulo, false);
            IncluirAlterar(0, 'divAlterarTitulo', 'divIncluirTitulo', 'divExcluirTitulo', 'apresentadorMensagemTitulo', 'divCancelarTitulo', 'divClearTitulo');
            
            dijit.byId("dialogTitulo").show();
        }, true);
        dojo.byId('gridTitulo').style.width = '1500px';
        //dojo.byId('gridTitulo').style.height = '200px';
    }
    catch (e) {
        postGerarLog(e);
    }
}




function formatTextVlTaxaCartaoMovimento(value, rowIndex, obj) {
    var gridTitulo = dijit.byId("gridTitulo");
    var icon;
    var desc = obj.field + '_input_' + gridTitulo._by_idx[rowIndex].item.id;

    if (hasValue(dijit.byId(desc), true))
        dijit.byId(desc).destroy();
    if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

    setTimeout("configuraTextBoxVlTaxaCartaoMovimento('" + value + "', '" + desc + "','" + gridTitulo._by_idx[rowIndex].item.id + "'," + rowIndex + "," + gridTitulo._by_idx[rowIndex].item.possuiBaixa + "," + gridTitulo._by_idx[rowIndex].item.cd_tipo_financeiro + ")", 1);
    return icon;
}

function configuraTextBoxVlTaxaCartaoMovimento(value, desc, id, rowIndex, possuiBaixa, cd_tipo_financeiro) {
	var parametros = getParamterosURL();
    var tipo = parametros['tipo'];

    if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
    else value = value.toString().replace('.', ',');

    if (!hasValue(dijit.byId(desc))) {
        require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
            var newTextBox = new dijit.form.NumberTextBox({
                name: "textBox" + desc,
                //value: unmaskFixed(value, 2),
                old_value: unmaskFixed(value, 2),
                //diff_value: !vl_taxa_cartao ? 0 : unmaskFixed(value, 2),
                disabled: (cd_tipo_financeiro == CARTAO && possuiBaixa == false && (tipo == SERVICO || tipo == ENTRADA || tipo == SAIDA) ) ? false : true,
                maxlength: 9,
                style: "width: 100%;",
                onBlur: function (b) {
                    $('#' + desc).focus();
                },
                onChange: function (b) {
                    atualizarValoresVlTaxaCartaoMovimento(desc, this, rowIndex, id);
                    //calcularValorTroco();
                },
                smallDelta: 1,
                constraints: { min: 0, pattern: '##.00#' }
            }, desc);
            newTextBox._onChangeActive = false;
            newTextBox.set('value', unmaskFixed(value, 2));
            newTextBox.value = unmaskFixed(value, 2);
            newTextBox._onChangeActive = true;
            //dijit.byId(desc).set('value', unmaskFixed(value, 2));
        });
    }
    if (hasValue(dijit.byId(desc))) {
        dijit.byId(desc).on("keypress", function (e) {
            mascaraFloat(document.getElementById(desc));
        });
    }
}

function atualizarValoresVlTaxaCartaoMovimento(desc, obj, rowIndex, id) {
    try {

        var gridTitulo = dijit.byId('gridTitulo');
        var item = getItemStoreTaxaCartaoMovimento(gridTitulo, id);
        var objDijit = dijit.byId(obj.id);

        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        consisteVlTaxaCartaoMovimento(item, objDijit.old_value, objDijit.value);
        gridTitulo.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getItemStoreTaxaCartaoMovimento(grid, id) {
    try {
        for (var i = 0; i < grid.store.objectStore.data.length; i++) {

            var _id = grid.store.objectStore.data[i].id;
            if (_id == id)
                return grid.store.objectStore.data[i];
        }
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}



function consisteVlTaxaCartaoMovimento(item, valorAntigo, valorAtual) {
    try {


        if (isNaN(valorAtual) || !hasValue(valorAtual, true)) {


            item.vl_taxa_cartao = valorAntigo;
            item.vlTaxaCartao = maskFixed(item.vl_taxa_cartao + "", 2);
            item.pc_taxa_cartao = calculaPcTaxaCartaoMovimento(item);



            return;
        }

        item.vl_taxa_cartao = valorAtual;
        item.vlTaxaCartao = maskFixed(item.vl_taxa_cartao + "", 2);
        item.pc_taxa_cartao = calculaPcTaxaCartaoMovimento(item);

        //var total = dijit.byId("vlTotal").value - valorAntigo + valorAtual;
        //unmaskFixed(dijit.byId("vlTotal").set("value", total) + "", 2);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function calculaPcTaxaCartaoMovimento(item) {
    try {
        var vl_taxa_cartao = item.vl_taxa_cartao;
        var vl_titulo = item.vl_titulo;
        var pc_taxa_cartao = vl_taxa_cartao / (vl_titulo / 100);

        return unmaskFixed(pc_taxa_cartao, 2);
    } catch (e) {
        postGerarLog(e);
    }

}

function montarTituloDefault() {
    try {
        var naturezaTitulo = PAGAR;
        if (TIPOMOVIMENTO == SAIDA || (TIPOMOVIMENTO == SERVICO && dojo.byId('serie').value != 'CEC'))
            naturezaTitulo = RECEBER;
        var cd_pessoa = parseInt(dojo.byId("cdPessoaMvtoCad").value);
        var no_responsavel = dijit.byId("noPessoaMovto").value;
        if (hasValue(dojo.byId("cdPessoaAlunoFKMovimento").value) && parseInt(dojo.byId("cdPessoaAlunoFKMovimento").value) > 0 && parseInt(dojo.byId("cdPessoaAlunoFKMovimento").value) != parseInt(dojo.byId("cdPessoaMvtoCad").value))
            cd_pessoa = dojo.byId("cdPessoaAlunoFKMovimento").value;

        var titulo = {
            cd_pessoa_responsavel: parseInt(dojo.byId("cdPessoaMvtoCad").value),
            cd_origem_titulo: 0,
            cd_pessoa_titulo: cd_pessoa,
            cd_tipo_financeiro: dijit.byId("tpFinanceiro").value,
            cd_local_movto: 0,
            cd_plano_conta_tit: 0,
            nomeResponsavel: dijit.byId("noPessoaMovto").value,
            tipoDoc: dojo.byId("tpFinanceiro").value,
            descLocalMovto: "",
            dc_tipo_titulo: "NF",
            nm_titulo: null,
            nm_parcela_titulo: 0,
            dc_num_documento_titulo: null,
            dt_emissao_titulo: dojo.date.locale.parse(dojo.byId("dtaEmis").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_vcto_titulo: dojo.date.locale.parse(dojo.byId("dtaVenc").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            vl_titulo: 0,
            vl_saldo_titulo: 0,
            id_origem_titulo: MOVIMENTOORIGEM,
            id_status_titulo: TITULO_ABERTO,
            id_natureza_titulo: naturezaTitulo,
            dh_cadastro_titulo: null
        }

        return titulo;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarItemSelecionadoTitulo(Memory, ObjectStore) {
    try {
        var grid = dijit.byId("gridTitulo");
        grid.store.save();
        var dados = grid.store.objectStore.data;
        var restaurarTitulos = false;

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            var cloneTitulos = grid.store.objectStore.data.slice();
            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--) {
                if (hasValue(dados[i].possuiBaixa) && dados[i].possuiBaixa) {
                    dados = cloneTitulos;
                    restaurarTitulos = true;
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNotExclItulo);
                    apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                    return false;
                }
                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                if (binaryObjSearch(grid.itensSelecionados, 'id', eval('dados[i].' + 'id')) != null)
                    dados.splice(i, 1); // Remove o item do array
            }
            if (!restaurarTitulos)
                grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarAtualizarTitulo(funcao) {
    try {
        //Caso click na aba de títulos e não esteja preenchido os campos obrigatórios
        if (!dijit.byId("formCadMovimento").validate()) {
            setarTabCadMovimento();
            showCarregando();
            return false;
        }
        var gridTitulo = dijit.byId("gridTitulo");
        //Só irá gerar o títulos se o parametro estiver verdadeiro.
        if (gerar_titulo) {
            var gridItem = dijit.byId("gridItem");
            //verifica se esta preenchidos todos os campos necessarios para gerar o título.
            if (hasValue(gridItem) && hasValue(gridItem.store.objectStore.data) && gridItem.store.objectStore.data.length > 0 && parseFloat((dojo.byId("totalGeral").value).replace(",", ".")) > 0) {
                //Verifica se existe local de movimento pois e obrigatorio no titulo.
                if (hasValue(gridTitulo) && gridTitulo.store.objectStore.data.length > 0) {
                    for (var i = 0; i < gridTitulo.store.objectStore.data.length; i++)
                        if (!hasValue(gridTitulo.store.objectStore.data[i].cd_local_movto) || gridTitulo.store.objectStore.data[i].cd_local_movto <= 0) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroLocalMovtoObrigatorio);
                            apresentaMensagem('apresentadorMensagemMovto', mensagensWeb);
                            showCarregando();
                            return false;
                        }
                }
                movimento = mountDataMovimentoForPost();
                movimento.titulos = [];
                movimento.titulos.push(montarTituloDefault());
                var titulos = [];
                dojo.xhr.post({
                    url: Endereco() + "/api/escola/postComponentesTitulos",
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    postData: JSON.stringify(movimento)
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data).retorno;
                        if (data.titulos != null && data.titulos.length > 0)
                            titulos = data.titulos;
                        apresentaMensagem("apresentadorMensagemMovto", null);
                        gerar_titulo = false;
                        if (hasValue(dijit.byId("gridTitulo")))
                            dijit.byId("gridTitulo").setStore(dojo.data.ObjectStore({ objectStore: dojo.store.Memory({ data: titulos }) }));
                        else
                            criarGradeTitulos(titulos);
                        dijit.byId("gridTitulo").update();
                        if (hasValue(funcao))
                            funcao.call();
                        showCarregando();
                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemMovto', error);
                });
            } else {
                if (hasValue(dijit.byId("gridTitulo"))) {
                    var dataNull = Array();
                    var dataStore = dojo.data.ObjectStore({ objectStore: dojo.store.Memory({ data: dataNull }) });
                    dijit.byId("gridTitulo").setStore(dataStore);
                }
                if (hasValue(funcao))
                    funcao.call();
                showCarregando();
            }
        } else {
            if (hasValue(funcao))
                funcao.call();
            showCarregando();
        }
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function keepValuesTitulo(value, grid, ehLink) {
    try {
        limparTitulo();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('elinkTitulo');

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
            var cd_localMvto = 0;
            if (hasValue(value.cd_local_movto))
                cd_localMvto = value.cd_local_movto;
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/getLocalMovtoByEscolaLogin?cd_local_movto=" + cd_localMvto,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem("apresentadorMensagemTitulo", null);
                    //loadSelect(data, "edLocalMovto", 'cd_local_movto', 'nomeLocal', cdLocalMovto);
                    dojo.byId("cd_titulo").value = value.cd_titulo;
                    dojo.byId("cd_pessoa_titulo").value = value.cd_pessoa_titulo;
                    dojo.byId("noPessoaTitulo").value = value.nomeAluno;
                    dojo.byId("cd_pessoa_responsavelTit").value = value.cd_pessoa_responsavel;
                    dojo.byId("pessoaResponsavelTit").value = value.nomeResponsavel;
                    dojo.byId("parcTit").value = value.nm_parcela_titulo;
                    dojo.byId("nrtitulo").value = value.nm_titulo;
                    dojo.byId("tipoTit").value = value.dc_tipo_titulo;
                    dijit.byId("tipoDocumentoTit")._onChangeActive = false;
                    dijit.byId("tipoDocumentoTit").set("value", value.cd_tipo_financeiro);
                    dijit.byId("tipoDocumentoTit")._onChangeActive = true;
                    dojo.byId("vlOriginalTipoDocumento").value = value.cd_tipo_financeiro;
                    dojo.byId("nrDoc").value = value.dc_num_documento_titulo;
                    dojo.byId("dtaEmisTit").value = value.dt_emissao;
                    dojo.byId("dtaVencTit").value = value.dt_vcto;
                    dojo.byId("valorTit").value = value.vlTitulo;
                    dojo.byId("cd_bancoTit").value = value.cd_local_movto;
                    
                    // dijit.byId("bancoTit").set("value", value.cd_local_movto);
                    //criarOuCarregarCompFiltering("bancoTit", data, "", value.cd_local_movto, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    loadSelectLocalMovimento(data, "bancoTit", 'cd_local_movto', 'nomeLocal', 'nm_tipo_local', value.cd_local_movto);
                    /*
                    if (hasValue(dijit.byId("tipoDocumentoTit").item) &&
                    (dijit.byId("tipoDocumentoTit").item.id == CARTAO ||
                        dijit.byId("tipoDocumentoTit").item.id == CHEQUE)) {
                        dijit.byId("tipoDocumentoTit").set("disabled", true);
                    } else {
                        dijit.byId("tipoDocumentoTit").set("disabled", false);
                       
                    }*/
                    
                    dijit.byId("nm_dias_cartao")._onChangeActive = false;
                    dijit.byId("pc_taxa_cartao")._onChangeActive = false;
                    dijit.byId("pc_taxa_cartao").set("value", value.pc_taxa_cartao);
                    dijit.byId("nm_dias_cartao").set("value", value.nm_dias_cartao);
                    vl_taxa_format = value.vl_taxa_cartao >= 0
                        ? parseFloat(value.vl_taxa_cartao).toFixed(2).replace(".", ",")
	                    : "0,0";
                    dojo.byId('vl_taxa_cartao').value = vl_taxa_format;
                    dijit.byId("nm_dias_cartao")._onChangeActive = true;
                    dijit.byId("pc_taxa_cartao")._onChangeActive = true;
                   


                    var parametros = getParamterosURL();
                    var tipo = parametros['tipo'];

                    var tgCartao = dojo.byId("tgCartao");
                    if (value.cd_tipo_financeiro == CARTAO && (tipo == SERVICO || tipo == ENTRADA || tipo == SAIDA)) {
	                    tgCartao.style.display = "block";
                    } else {
	                    tgCartao.style.display = "none";
                    }
                    $('#' + "nrDoc").focus();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemTitulo", error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarTitulo() {
    try {
        if (!dijit.byId("formTitulo").validate())
            return false;

        if (dijit.byId("tpFinanceiro").value != CHEQUE && dijit.byId("tipoDocumentoTit").value == CHEQUE) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgTipoFinanceiroDiferente);
            apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
            return false;
        }


        if ((hasValue(dijit.byId("tipoDocumentoTit").item) &&
            (dijit.byId("tipoDocumentoTit").item.id == CARTAO)) &&
            (hasValue(dijit.byId("bancoTit").item) && (dijit.byId("bancoTit").item.nm_tipo_local != 4 && dijit.byId("bancoTit").item.nm_tipo_local != 5))) {
	        dijit.byId("bancoTit")._onChangeActive = false;
            dijit.byId("bancoTit").set("value", "");
            dijit.byId("bancoTit")._onChangeActive = true;

            apresentaMensagem('apresentadorMensagemTitulo', null);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                "Para títulos com tipo financeiro “cartão” somente Locais de Movimento tipo “cartão de débito/crédito poderão ser selecionados, caso contrário estes tipos não poderão.");
            apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
            return false;
        } else if ((hasValue(dijit.byId("tipoDocumentoTit").item) &&
                (dijit.byId("tipoDocumentoTit").item.id != CARTAO)) &&
                (hasValue(dijit.byId("bancoTit").item) && (dijit.byId("bancoTit").item.nm_tipo_local == 4 || dijit.byId("bancoTit").item.nm_tipo_local == 5))) {
	            dijit.byId("bancoTit")._onChangeActive = false;
                dijit.byId("bancoTit").set("value", "");
                dijit.byId("bancoTit")._onChangeActive = true;

                apresentaMensagem('apresentadorMensagemTitulo', null);
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                    "Para títulos com tipo financeiro diferente de “cartão”, os Locais de Movimento do tipo “cartão de débito/crédito não poderão ser selecionados.");
                apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                return false;
            }

        var oldValue = null;
        var value = null;
        var grid = dijit.byId("gridTitulo");
        var ehLink = false;
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('elinkTitulo');
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];
        //Metodo usuardo para clonar um objeto.
        oldValue = jQuery.extend(true, {}, value);
        //value = jQuery.extend(true, {}, value);
        var vl_titulo = parseFloatMascara(dojo.byId("valorTit").value);
        var titulo = dijit.byId("gridTitulo").itemSelecionado;
        if (value.id > 0) {
            value.cd_titulo = dojo.byId("cd_titulo").value,
            value.cd_pessoa_titulo = dojo.byId("cd_pessoa_titulo").value,
            value.nomeAluno = dojo.byId("noPessoaTitulo").value,
            value.cd_pessoa_responsavel = dojo.byId("cd_pessoa_responsavelTit").value,
            value.nomeResponsavel = dojo.byId("pessoaResponsavelTit").value,
            value.nm_parcela_titulo = dojo.byId("parcTit").value,
            value.nm_titulo = dojo.byId("nrtitulo").value,
            value.cd_local_movto = dijit.byId("bancoTit").value,
            value.dc_tipo_titulo = dojo.byId("tipoTit").value,
            value.cd_tipo_financeiro = dijit.byId("tipoDocumentoTit").value,
            value.tipoDoc = dojo.byId("tipoDocumentoTit").value,
            value.dc_num_documento_titulo = dojo.byId("nrDoc").value,
            value.dt_emissao = dojo.byId("dtaEmisTit").value,
            value.dt_emissao_titulo = dojo.date.locale.parse(dojo.byId("dtaEmisTit").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            value.dt_vcto_titulo = dojo.date.locale.parse(dojo.byId("dtaVencTit").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            value.dt_vcto = dojo.byId("dtaVencTit").value,
            value.vlTitulo = dojo.byId("valorTit").value,
            value.vl_titulo = vl_titulo,
            value.vl_saldo_titulo = vl_titulo,
            value.vlSaldoTitulo = dojo.byId("valorTit").value,
            value.cd_local_movto = dojo.byId("cd_bancoTit").value,
            value.tituloEdit = true,
            value.cd_local_movto = dijit.byId("bancoTit").value,
            value.descLocalMovto = dojo.byId("bancoTit").value,
            titulo.nm_dias_cartao = dijit.byId("nm_dias_cartao").value,
            titulo.pc_taxa_cartao = dijit.byId("pc_taxa_cartao").value,
            titulo.vl_taxa_cartao = parseFloat((dojo.byId("vl_taxa_cartao").value).replace(",", ".")),
            titulo.VlTaxaCartao = dojo.byId("vl_taxa_cartao").value
        }
        dijit.byId("gridTitulo").update();
        dijit.byId("dialogTitulo").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTitulo(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridTitulo = dijit.byId('gridTitulo');

            if (hasValue(itensSelecionados[0].possuiBaixa) && itensSelecionados[0].possuiBaixa) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloBaixado);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                return false;
            }
            apresentaMensagem('apresentadorMensagem', '');
            //gridTitulo.itemSelecionado = itensSelecionados[0];
            keepValuesTitulo(null, gridTitulo, true);
            IncluirAlterar(0, 'divAlterarTitulo', 'divIncluirTitulo', 'divExcluirTitulo', 'apresentadorMensagemTitulo', 'divCancelarTitulo', 'divClearTitulo');
            dijit.byId("dialogTitulo").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTitulo() {
    try {
        clearForm("formTitulo");
        dojo.byId("cd_titulo").value = 0;
        dojo.byId("cd_pessoa_titulo").value = 0;
        dojo.byId("cd_pessoa_responsavelTit").value = 0;
        dojo.byId("cd_bancoTit").value = 0;
        dijit.byId("dtaEmisTit").reset();
        dijit.byId("dtaVencTit").reset();
        dijit.byId('bancoTit').reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function onActiveChangeCamposGerarTitulos(bool) {
    try {
        //Limpar valores obrigatorios geração de titulos
        dijit.byId("dtaEmis")._onChangeActive = bool;
        dijit.byId("dtaVenc")._onChangeActive = bool;
        dijit.byId("tpFinanceiro")._onChangeActive = bool;
        dijit.byId("totalGeral")._onChangeActive = bool;
        dijit.byId("totalItens")._onChangeActive = bool;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarLocalMovtoTitulos() {
    try {
        var itensAlterar = dijit.byId("gridTitulo").itensSelecionados;
        
        if (!hasValue(itensAlterar) || itensAlterar.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else {
        var titulosRollback = cloneArray(itensAlterar);
            gerar_titulo = false;

            if (dijit.byId("edBanco").value > 0) {
                apresentaMensagem("apresentadorMensagemMovto", null);

                if (itensAlterar.length > 0) {
                    for (var i = 0; i < itensAlterar.length; i++) {

                        if (itensAlterar[i].cd_tipo_financeiro == 5 && (dijit.byId("edBanco").item.nm_tipo_local != 5 && dijit.byId("edBanco").item.nm_tipo_local != 4)) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Para títulos com tipo financeiro “cartão” somente Locais de Movimento tipo “cartão de débito/crédito poderão ser selecionados, caso contrário estes tipos não poderão.");
                            apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                            return;
                        }

                        if (itensAlterar[i].cd_tipo_financeiro != 5 && (dijit.byId("edBanco").item.nm_tipo_local == 5 || dijit.byId("edBanco").item.nm_tipo_local == 4)) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Para títulos com tipo financeiro diferente de “cartão”, os Locais de Movimento do tipo “cartão de débito/crédito não poderão ser selecionados.");
                            apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                            return;
                        }

                    }

                    var titulosComBaixa = "";
                    var titulosCartao = false;
                    for (var i = 0; i < itensAlterar.length; i++) {
                        if (itensAlterar[i].vl_saldo_titulo > 0 &&
                            itensAlterar[i].vl_saldo_titulo == itensAlterar[i].vl_titulo &&
                            itensAlterar[i].vl_liquidacao_titulo === 0 &&
                            (itensAlterar[i].possuiBaixa != undefined &&
                                itensAlterar[i].possuiBaixa != null &&
                                itensAlterar[i].possuiBaixa === false)) {
                            itensAlterar[i].cd_local_movto = dijit.byId("edBanco").value;
                            itensAlterar[i].descLocalMovto = dojo.byId("edBanco").value;
                            itensAlterar[i].alterou_local_movto = true;

                            var titulosGrid = dijit.byId('gridTitulo').store.objectStore.data;

                            jQuery.grep(titulosGrid, function (titulo) {
                                if (((titulo.cd_titulo > 0 && titulo.cd_titulo === itensAlterar[i].cd_titulo) || (titulo.cd_titulo === 0 && titulo.nm_parcela_titulo === itensAlterar[i].nm_parcela_titulo)) &&
                                    titulo.vl_saldo_titulo > 0 &&
                                    titulo.vl_titulo == titulo.vl_saldo_titulo) {
                                    titulo.cd_local_movto = itensAlterar[i].cd_local_movto;
                                    titulo.descLocalMovto = itensAlterar[i].descLocalMovto;
                                }
                            });
                        } else {
                            titulosComBaixa += "Parcela:" + itensAlterar[i].nm_parcela_titulo + "<br>";
                        }
                        if (!titulosCartao) titulosCartao = itensAlterar[i].cd_tipo_financeiro == CARTAO
                    }

                    dijit.byId("gridTitulo")._refresh();

                    if (hasCartao()) {
                        alterarLocalMovtoAplicarTaxaBancaria(itensAlterar, titulosRollback);
                    } else {
                        if (dijit.byId("ckNotaFiscal").checked && dijit.byId("statusNFS").value == 2) {
                            postAlterarLocalMovtoTitulosNFFechada(itensAlterar, titulosRollback);
                        } 
                    }

                    if (hasValue(titulosComBaixa)) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgTitulosComBaixa + "<br>" + titulosComBaixa);
                        apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                    }

                }
            }
            else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgEscolherLocalMovto);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consultarPessoaMovimento() {
    try {
        if (dijit.byId("cadMovimento").open) {
            apresentaMensagem("apresentadorMensagemProPessoa", null);
            pesquisaPessoaCadFK();
        } else {
            apresentaMensagem("apresentadorMensagemProPessoa", null);
            pesquisaPessoaFKMovimento(true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckEmitidoCNAB(value, rowIndex, obj) {
    try {
        var gridTitulo = dijit.byId("gridTitulo");
        var icon;
        var id = obj.field + '_Selected_' + gridTitulo._by_idx[rowIndex].item.id + '-8';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxEmitidoCNAB(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxEmitidoCNAB(value, rowIndex, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                disabled: true,
                name: "checkBox",
                checked: value,
            }, id);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//FISCAL

function loadSituacao(items, cdSituacao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var itemsCb = [];
	        var cbSitTrib = dijit.byId("sitTribItem");

	        Array.forEach(items, function (value, i) {
	            itemsCb.push({ id: value.cd_situacao_tributaria, name: value.dcSituacao, formaTrib: value.id_forma_tributacao });
	        });
	        var stateStore = new Memory({
	            data: itemsCb
	        });
	        cbSitTrib.store = stateStore;

	        if (hasValue(cdSituacao) && cdSituacao > 0) {
	            cbSitTrib._onChangeActive = false;
	            cbSitTrib.set("value", cdSituacao);
	            cbSitTrib._onChangeActive = true;
	            quickSortObj(cbStTribFiscal.store, 'id');
	            var posicao = binaryObjSearch(cbStTribFiscal.store.data, 'id', cbStTribFiscal.value);

	        }
	        cbSitTrib.set("required", false);
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}

function habilitaCamposFiscal() {
    var parametros = getParamterosURL();
    var tipo = parametros['tipo'];
    if (tipo == SERVICO) {

        document.getElementById("trICMS").style.display = "none";
        document.getElementById("trPIS").style.display = "none";
        document.getElementById("trCOFINS").style.display = "none";
        document.getElementById("trIPI").style.display = "none";
        document.getElementById("trISS").style.display = "";
        document.getElementById("trNFS").style.display = "";
        document.getElementById("trURL").style.display = "";
        document.getElementById("trChavaAcesso").style.display = "none";
    }
    else
        if (tipo == SAIDA || tipo == ENTRADA || tipo == DEVOLUCAO) {

            document.getElementById("trICMS").style.display = "";
            document.getElementById("trPIS").style.display = "";
            document.getElementById("trCOFINS").style.display = "";
            document.getElementById("trIPI").style.display = "";
            document.getElementById("trISS").style.display = "none";
            document.getElementById("trNFS").style.display = "none";
            document.getElementById("trURL").style.display = "none";
            document.getElementById("trChavaAcesso").style.display = "";
        }

}
//retorno FK tipo nota fiscal
function criarEventoSelecionaTipoNF() {
    try {
        dijit.byId("selecionaTipoNFFK").on("click", function (e) {
            var gridTipoNFFK = dijit.byId("gridTipoNFFK");
            if (hasValue(dijit.byId("gridTipoNFFK")) && (dijit.byId("gridTipoNFFK")._by_idx.length > 0))
                if (gridTipoNFFK.itensSelecionados != null && gridTipoNFFK.itensSelecionados.length > 1) {
                    caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    return false;
                } else {
                    if (gridTipoNFFK.itensSelecionados == null || gridTipoNFFK.itensSelecionados.length <= 0) {
                        caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
                        return false
                    }
                }
            var tipoNFSelecionado = gridTipoNFFK.itensSelecionados[0];
            gerar_titulo = true;
            showCarregando();
            dojo.byId('cd_tp_nf').value = tipoNFSelecionado.cd_tipo_nota_fiscal;
            dojo.byId('id_natureza_movto').value = tipoNFSelecionado.id_natureza_movimento;
            dijit.byId('tpNf').set("value", tipoNFSelecionado.dc_tipo_nota_fiscal);
            dijit.byId('CFOP').set("value", tipoNFSelecionado.dc_CFOP);
            dojo.byId('cd_cfop_nf').value = tipoNFSelecionado.cd_cfop;
            dijit.byId('idObsNF').set("value", tipoNFSelecionado.tx_obs_tipo_nota);
            dijit.byId('tpNf').reducao = tipoNFSelecionado.pc_reducao;
            regime_tributario = tipoNFSelecionado.id_regime_tributario;
            //dijit.byId("cd_sit_trib_ICMS_tp_nt").set("value", tipoNFSelecionado.cd_situacao_tributaria);
            dojo.byId("cd_sit_trib_ICMS_tp_nt").value = tipoNFSelecionado.cd_situacao_tributaria;
            loadSitTributariaTipoNF(dojo.byId('cd_tp_nf').value);
            //Limpa a grade de título:
            gerar_titulo = true;
            if (TIPOMOVIMENTO == SERVICO) {
                dojo.byId('id_natureza_movto').value = tipoNFSelecionado.id_natureza_movimento;
                if (tipoNFSelecionado.id_natureza_movimento == ENTRADA) {
                    dijit.byId("nrMovto").set("disabled", false);
                    dijit.byId("serie").set("disabled", false);
                    dijit.byId("nrMovto").set("required", true);
                    dijit.byId("serie").set("required", true);
                } else {
                    dijit.byId("nrMovto").set("disabled", true);
                    dijit.byId("serie").set("disabled", true);
                    dijit.byId("nrMovto").set("required", false);
                    dijit.byId("serie").set("required", false);
                }
            }
            verificaOperacaoCFOP();
            dijit.byId("fkTipoNF").hide();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTipoNFFK() {
    try {
        switch (TIPOMOVIMENTO) {
            case SERVICO:
                //dijit.byId("cbMovtoFK").set("value", TIPO_SAIDA);
                if (dojo.byId("id_natureza_movto").value == TIPO_ENTRADA)
                    dijit.byId("cbMovtoFK").set("value", TIPO_ENTRADA);
                else
                    dijit.byId("cbMovtoFK").set("value", TIPO_SAIDA);
                dijit.byId("cbMovtoFK").set("disabled", false);
                break;
            case SAIDA:
                dijit.byId("cbMovtoFK").set("value", TIPO_SAIDA);
                dijit.byId("cbMovtoFK").set("disabled", true);
                break;
            case ENTRADA:
                dijit.byId("cbMovtoFK").set("value", TIPO_ENTRADA);
                dijit.byId("cbMovtoFK").set("disabled", true);
                break;
            case DEVOLUCAO:
                if (dojo.byId("id_tipo_movimento").value == TIPO_ENTRADA)
                    dijit.byId("cbMovtoFK").set("value", TIPO_SAIDA);
                else
                    dijit.byId("cbMovtoFK").set("value", TIPO_ENTRADA);
                if (dojo.byId("id_tipo_movimento").value > 0)
                    dijit.byId("cbMovtoFK").set("disabled", true);
                else
                    dijit.byId("cbMovtoFK").set("disabled", false);
                break;
        }

        limparPesquisaTipoNFFK();
        pesquisaTipoNFFK(true, TIPOMOVIMENTO == SERVICO ? true : false);
        dijit.byId("fkTipoNF").show();
        apresentaMensagem('apresentadorMensagemTipoNFFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function statusNF() {
    var statusStore = new dojo.store.Memory({
        data: [
          { name: "Aberto", id: "1" },
          { name: "Fechado", id: "2" },
          { name: "Cancelado", id: "3" }
        ]
    });
    var statusNF = new dijit.form.FilteringSelect({
        id: "statusNFS",
        name: "statusNFS",
        value: 1,
        disabled: true,
        store: statusStore,
        searchAttr: "name",
        style: "max-width:300px;width: 100%;"
    }, "statusNF");
}

function dadosFiscaisNFServico(pFuncao) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getISSEscola",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (hasValue(pFuncao))
                pFuncao.call();
            var pcISS = 0;
            if (regime_tributario == REGIME_NORMAL)
                pcISS = jQuery.parseJSON(data).retorno;
            apresentaMensagem("apresentadorMensagemItem", null);
            dijit.byId("aliquotaISSItem").set("value", pcISS);
            var baseISS = dijit.byId("baseCalcISSItem");
            if (baseISS.value > 0 && pcISS > 0) {
                var vlISS = (baseISS.value * pcISS) / 100;
                dijit.byId("valorISSItem").set("value", vlISS);
            }
            if (hasValue(dojo.byId("cd_cfop_nf").value)) {
                dojo.byId("cd_CFOP_item").value = dojo.byId("cd_cfop_nf").value;
                dijit.byId("descCFOPItem").set("value", dijit.byId("CFOP").value);
                dojo.byId("descCFOPItem").value = dijit.byId("CFOP").value;
            }
            dijit.byId("operacaoCFOPItem").set("value", dijit.byId("operacaoCFOP").value);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemItem", error);
    });
}

function dadosFiscaisNFProduto(pFuncao) {
    var cdPessoaMovimento = hasValue(dojo.byId("cdPessoaMvtoCad").value) && dojo.byId("cdPessoaMvtoCad").value > 0 ? dojo.byId("cdPessoaMvtoCad").value : 0;
    if (cdPessoaMovimento > 0)
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getTributacaoNFProduto?cd_pessoa_movimento=" + cdPessoaMovimento,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                if (hasValue(pFuncao))
                    pFuncao.call();
                var aliqICMS = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemItem", null);
                if (hasValue(aliqICMS)) {
                    dijit.byId("aliquotaICMSItem").set("value", aliqICMS.pc_aliq_icms_padrao);
                    dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                    var baseICMS = dijit.byId("baseCalcICMSItem");
                    if (baseICMS.value > 0 && aliqICMS.pc_aliq_icms_padrao > 0) {
                        var vlICMS = (baseICMS.value * aliqICMS.pc_aliq_icms_padrao) / 100;
                        dijit.byId("valorICMSItem").set("value", vlICMS);
                    }
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemItem", error);
        });
    else
        if (hasValue(pFuncao))
            pFuncao.call();
}



function habilitarDesabilitarCamposEditItemNF(bool) {
    dijit.byId("cadItem").set("disabled", bool);
    dijit.byId("desc_item").set("disabled", bool);
    dijit.byId("qtd_item").set("disabled", bool);
    dijit.byId("vlUnitario").set("disabled", bool);
    dijit.byId("perDescontoItem").set("disabled", bool);
    dijit.byId("valDescontoItem").set("disabled", bool);
    dijit.byId("vlTotalMovimento").set("disabled", bool);
    dijit.byId("descPlanoConta").set("disabled", bool);
    dijit.byId("cadPlanoConta").set("disabled", bool);
    dijit.byId("pcAcrec").set("disabled", bool);
    dijit.byId("vlAc").set("disabled", bool);
    dijit.byId("pcDesconto").set("disabled", bool);
    dijit.byId("vlDesconto").set("disabled", bool);
    dijit.byId("itensT").set("disabled", bool);
}

function configuraDadosMovimento(ORIGEMCHAMADONF, id_material_didatico, id_futura) {
    var endereco = "";
    var parametros = getParamterosURL();
    switch (parseInt(ORIGEMCHAMADONF)) {
        case ORIGMATRICULA:
            if (hasValue(parametros['cdContrato'])) {
                var cd_contrato = parametros['cdContrato'];
                endereco = Endereco() + "/api/escola/getMontaNFMaterial?cd_contrato=" + cd_contrato + "&id_futura=" + id_futura;
            }
            break;
        case BIBLIOTECA:
            if (hasValue(parametros['cdBiblioteca'])) {
                var cd_biblioteca = parametros['cdBiblioteca'];
                endereco = Endereco() + "/api/escola/getMontaNFBiblioteca?cd_biblioteca=" + cd_biblioteca;
            }
            break;
        case BAIXAFINANCEIRA:
            if (hasValue(parametros['cdBaixa'])) {
                var cd_baixa_titulo = parametros['cdBaixa'];
                endereco = Endereco() + "/api/escola/getMontaNFBaixaFinanceira?cd_baixa_titulo=" + cd_baixa_titulo;
            }
            break;
    }
    if (hasValue(endereco)) {
        dojo.ready(function () {
            dojo.xhr.get({
                url: endereco,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {

                    if (data.retorno != null) {
                        hideCarregando();

                        if (data.retorno.cd_movimento > 0) {
                            var gridMovimento = dijit.byId('gridMovimento');
                            apresentaMensagem('apresentadorMensagem', '');
                            keepValuesMovimento(data.retorno, gridMovimento, true, id_material_didatico);
                            IncluirAlterar(0, 'divAlterarMovto', 'divIncluirMovto', 'divExcluirMovto', 'apresentadorMensagemMovto', 'divCancelarMovto', 'divClearMovto');
                            dijit.byId("gridMovimento").layout.setColumnVisibility(11, true);
                            dojo.byId("trFutura").style.display = "";
                            dojo.byId("trFuturaCad").style.display = "";
                            dijit.byId("cadMovimento").show();
                        }
                        else {
                            limparMovimento();
                            findIsLoadComponetesNovoMovto(dojo.xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect);
                            IncluirAlterar(1, 'divAlterarMovto', 'divIncluirMovto', 'divExcluirMovto', 'apresentadorMensagemMovto', 'divCancelarMovto', 'divClearMovto');
                            apresentaMensagem("apresentadorMensagemMovto", null);
                            onActiveChangeCamposGerarTitulos(false);
                            loadDataMovimento(data.retorno);
                            onActiveChangeCamposGerarTitulos(true);
                            destroyCreateGridTitulos();
                            IncluirAlterar(1, 'divAlterarMovto', 'divIncluirMovto', 'divExcluirMovto', 'apresentadorMensagemMovto', 'divCancelarMovto', 'divClearMovto');
                            dijit.byId("cadMovimento").show();
                        }
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
}

//Nota Fiscal 

function abrirCFOPFK() {
    try {
        apresentaMensagem('apresentadorMensagemCFOPFK', null);
        limparPesquisaCFOPFK();
        var id_natureza = 0;
        var parametros = getParamterosURL();
        if (hasValue(parametros['tipo'])) {
            var tipo = eval(parametros['tipo']);
            if (tipo == DEVOLUCAO)
                tipo = dojo.byId('id_natureza_movto').value;
            switch (tipo) {
                case ENTRADA:
                    id_natureza = ENTRADACFOP;
                    break;
                case SAIDA:
                    id_natureza = SAIDACFOP;
                    break;
                case SERVICO:
                    id_natureza = SERVICOCFOP;
                    break;
            }
        }
        searchCFOP(id_natureza);
        dijit.byId("fkCFOP").show();
    }
    catch (e) {
        postGerarLog(e);
    }

}

function retornarCFOPFK() {
    try {
        var valido = true;
        var gridCFOPFK = dijit.byId("gridCFOPFK");
        if (!hasValue(gridCFOPFK.itensSelecionados))
            gridCFOPFK.itensSelecionados = [];
        if (!hasValue(gridCFOPFK.itensSelecionados) || gridCFOPFK.itensSelecionados.length <= 0 || gridCFOPFK.itensSelecionados.length > 1) {
            if (gridCFOPFK.itensSelecionados != null && gridCFOPFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridCFOPFK.itensSelecionados != null && gridCFOPFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            $("#cd_CFOP_item").val(gridCFOPFK.itensSelecionados[0].cd_cfop);
            dijit.byId("descCFOPItem").set("value", gridCFOPFK.itensSelecionados[0].nm_cfop);
        }

        if (!valido)
            return false;
        dijit.byId("fkCFOP").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarSeExisteFiscalItensMovimento(bool) {
    apresentaMensagem("apresentadorMensagemMovto", null);
    var gridItem = dijit.byId("gridItem");
    var compCkNF = dijit.byId("ckNotaFiscal");
    var validado = true;
    var tipoIdNF = false;
    if (hasValue(gridItem) && hasValue(gridItem.store.objectStore.data) && gridItem.store.objectStore.data.length > 0) {
        $.each(gridItem.store.objectStore.data, function (index, value) {
            if (value.id_nf_item != dijit.byId("ckNotaFiscal").checked) {
                validado = false;
                return validado;
            }
        });
    }
    if (!validado) {
        var mensagemUser = "";
        if (compCkNF.checked)
            mensagemUser = msgErroMovimentoItensJaTemTributacao;
        else
            mensagemUser = msgErroMovimentoItensNaoTemTributacao;
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemUser);
        apresentaMensagem("apresentadorMensagemMovto", mensagensWeb, true);
        compCkNF._onChangeActive = false;
        compCkNF.set("checked", !compCkNF.checked);
        compCkNF._onChangeActive = true;
    }
    return validado;
}

function verificaOperacaoCFOP() {
    var cdPessoa = dojo.byId("cdPessoaMvtoCad").value;
    var operacao = dijit.byId("operacaoCFOP");
    operacao.set("value", null);
    var tipo = TIPOMOVIMENTO;
    if (tipo == DEVOLUCAO)
        tipo = dojo.byId('id_natureza_movto').value;
    if (hasValue(cdPessoa) && cdPessoa > 0)
        dojo.xhr.get({
            url: Endereco() + "/api/escola/getVerifcaEstadoEscAluno?cd_pessoa=" + cdPessoa + "&tpMovto=" + tipo,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                apresentaMensagem("apresentadorMensagemMovto", null, true);
                operacao.set("value", data.retorno);

            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemMovto", error);
        });
}

function loadSitTributariaTipoNF(cdTpNF) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getSituacaoTributariaTpNF?cdTpNF=" + cdTpNF,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            alterou_tp_nf = true;
            if (data != null && data.retorno != null) {
                if (hasValue(data.retorno.situacoesTributariaICMS))
                    loadSituacao(data.retorno.situacoesTributariaICMS, 0);
                if (hasValue(data.retorno.situacoesTributariaPIS))
                    criarOuCarregarCompFiltering("cbStTribPis", data.retorno.situacoesTributariaPIS, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                if (hasValue(data.retorno.situacoesTributariaCOFINS))
                    criarOuCarregarCompFiltering("cbStTribCof", data.retorno.situacoesTributariaCOFINS, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_situacao_tributaria', 'dc_situacao_tributaria');
            }
            showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagemMovto", error);
    });
}

function retornaFKMovto() {
    try {
        var valido = true;
        var gridMovtoFK = dijit.byId("gridPesquisaMovtoFK");
        if (!hasValue(gridMovtoFK.itensSelecionados))
            gridMovtoFK.itensSelecionados = [];
        if (!hasValue(gridMovtoFK.itensSelecionados) || gridMovtoFK.itensSelecionados.length <= 0 || gridMovtoFK.itensSelecionados.length > 1) {
            if (gridMovtoFK.itensSelecionados != null && gridMovtoFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridMovtoFK.itensSelecionados != null && gridMovtoFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dijit.byId("gridItem").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            dojo.byId("cd_nfDev").value = gridMovtoFK.itensSelecionados[0].cd_movimento;
            dijit.byId("tpNfDev").set("value", gridMovtoFK.itensSelecionados[0].dc_numero_serie);
            $("#id_tipo_movimento").val(gridMovtoFK.itensSelecionados[0].id_tipo_movimento);
            dojo.byId('id_natureza_movto').value = gridMovtoFK.itensSelecionados[0].id_tipo_movimento == ENTRADA ? SAIDA : ENTRADA;
            dijit.byId("limparNFDev").set("disabled", false);
            dojo.xhr.get({
                url: Endereco() + "/api/fiscal/getRetMovimentoDevolucao?cd_movimento=" + dojo.byId("cd_nfDev").value + "&id_tipo_movimento=" + dojo.byId("id_tipo_movimento").value +
                    "&isMaster=" + eval(MasterGeral()),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                data = data.retorno;
                var compPcAcresc = dijit.byId("pcAcrec");
                var compVlAcresc = dijit.byId("vlAc");
                var compPcDesc = dijit.byId("pcDesconto");
                var compVlDesc = dijit.byId("vlDesconto");
                dojo.byId("cdPessoaMvtoCad").value = data.cd_pessoa;
                dojo.byId("noPessoaMovto").value = data.no_pessoa;
                compVlAcresc._onChangeActive = false;
                compVlAcresc.set("value", data.vl_acrescimo);
                compVlAcresc.oldValue = data.vl_acrescimo;
                compVlAcresc._onChangeActive = true;
                verificaOperacaoCFOP();
                compPcAcresc._onChangeActive = false;
                compPcAcresc.set("value", data.pc_acrescimo);
                compPcAcresc.oldValue = data.pc_acrescimo;
                compPcAcresc._onChangeActive = true;

                compPcDesc._onChangeActive = false;
                compPcDesc.set("value", data.pc_desconto);
                compPcDesc.oldValue = data.pc_desconto;
                compPcDesc._onChangeActive = true;

                compVlDesc._onChangeActive = false;
                compVlDesc.set("value", data.vl_desconto);
                compVlDesc.oldValue = data.vl_desconto;
                compVlDesc._onChangeActive = true;
                //Aba fiscal
                if (TIPOMOVIMENTO != DEVOLUCAO) {
                    dijit.byId("nfEsc").set("checked", data.id_nf_escola);
                } 
                dijit.byId("baseICMS").set("value", data.vl_base_calculo_ICMS_nf);
                dijit.byId("vl_icms").set("value", data.vl_ICMS_nf);
                dijit.byId("basePIS").set("value", data.vl_base_calculo_PIS_nf);
                dijit.byId("vl_pis").set("value", data.vl_PIS_nf);
                dijit.byId("baseCOFINS").set("value", data.vl_base_calculo_COFINS_nf);
                dijit.byId("vl_COFINS").set("value", data.vl_COFINS_nf);
                dijit.byId("baseIPI").set("value", data.vl_base_calculo_IPI_nf);
                dijit.byId("vl_ipi").set("value", data.vl_IPI_nf);
                dijit.byId("idObsNF").set("value", data.tx_obs_fiscal);
                dijit.byId("statusNFS").set("value", data.id_status_nf);
                dojo.byId("id_origem_movimento").value = data.id_origem_movimento;
                dojo.byId("cd_origem_movimento").value = data.cd_origem_movimento;
                if (hasValue(data.vl_aproximado))
                    dijit.byId("vl_aproximado").set("value", data.vl_aproximado);
                if (hasValue(data.pc_aliquota_aproximada))
                    dijit.byId("pc_aliquota_ap").set("value", data.pc_aliquota_aproximada);
                populaItensDevolucao(data.ItensMovimento);
                dijit.byId("itensT").set("disabled", (TIPOMOVIMENTO == DEVOLUCAO))
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemMovto', error);
            });
        }
        if (!valido)
            return false;
        dijit.byId("proMvtoFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaItensDevolucao(data) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var grid = dijit.byId("gridItem");
                if (hasValue(data)) {
                    if (hasValue(data) && data.length > 0) {
                        for (var i = data.length - 1; i >= 0; i--) {
                            data[i].cd_item_movimento = 0;
                            var id = geradorIdItem(grid);
                            montarObjItemMovimento(grid, data[i], id)
                        }

                        grid.setStore(new ObjectStore({ objectStore: new Memory({ data: grid.store.objectStore.data }) }));
                        grid.store.save();

                        var compVlTotalItens = dijit.byId("totalItens");
                        var compVlTotalGeral = dijit.byId("totalGeral");
                        var totalItens = 0;
                        var totalGeral = 0;
                        $.each(data, function (index, value) {
                            totalItens += value.vl_total_item;
                            totalGeral += value.vl_liquido_item;
                            if (!hasValue(value.pc_desconto))
                                value.pc_desconto = 0;
                            if (value.pc_desconto != data.pc_desconto)
                                descDiferente = true;
                        });
                        compVlTotalItens.set("value", totalItens);
                        compVlTotalGeral.set("value", totalGeral);
                    }
                }

            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function desabilitaHabilitaItem(bool) {
    dijit.byId("vlUnitario").set("disabled", bool);
    dijit.byId("perDescontoItem").set("disabled", bool);
    dijit.byId("valDescontoItem").set("disabled", bool);
    dijit.byId("vlTotalMovimento").set("disabled", bool);
    dijit.byId("cadPlanoConta").set("disabled", bool);
    dijit.byId("descCFOPItem").set("disabled", bool);

    dijit.byId("sitTribItem").set("disabled", bool);
    dijit.byId("baseCalcICMSItem").set("disabled", bool);
    dijit.byId("aliquotaICMSItem").set("disabled", bool);
    dijit.byId("valorICMSItem").set("disabled", bool);

    dijit.byId("cbStTribPis").set("disabled", bool);
    dijit.byId("baseCalcPISItem").set("disabled", bool);
    dijit.byId("aliquotaPISItem").set("disabled", bool);
    dijit.byId("valorPISItem").set("disabled", bool);

    dijit.byId("cbStTribCof").set("disabled", bool);
    dijit.byId("baseCalcCOFINSItem").set("disabled", bool);
    dijit.byId("aliquotaCOFINSItem").set("disabled", bool);
    dijit.byId("valorCOFINSItem").set("disabled", bool);

    dijit.byId("baseCalcIPIItem").set("disabled", bool);
    dijit.byId("aliquotaIPIItem").set("disabled", bool);
    dijit.byId("valorIPIItem").set("disabled", bool);
}

function loadSituacaoPesquisa() {
    try {
        var statusStore = new dojo.store.Memory({
            data: [
               { name: "Todos", id: 0 },
               { name: "Aberto", id: 1 },
               { name: "Fechado", id: 2 },
               { name: "Cancelado", id: 3 }

            ]
        });

        var situacao = new dijit.form.FilteringSelect({
            id: "statusNFPesq",
            name: "statusNFPesq",
            store: statusStore,
            value: 0,
            searchAttr: "name",
            style: "width:90px;"
        }, "statusNFPesq");
    } catch (e) {
        postGerarLog(e);
    }
}

function emitirRelatorioCarneMovto(cdMovimento, Permissoes, xhr) {
    var endereco = Endereco() + "/api/financeiro/getUrlRelatorioCarneMovto?cdMovimento=";
    if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes))
        endereco = Endereco() + "/api/financeiro/getUrlRelatorioCarneGeralMovto?cdMovimento=";
    xhr.get({
        url: endereco + cdMovimento,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            abrePopUp(Endereco() + '/Relatorio/RelatorioCarneMovto?' + data, '1024px', '750px', 'popRelatorio');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function abrirAlunoFK() {
    try {
        limparPesquisaAlunoFK();
        apresentaMensagem("apresentadorMensagem", null);
        dojo.byId('tipoRetornoAlunoFK').value = CADMOVIMENTO;
        pesquisarAlunoFKMovimento(true);
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirCursoFK() {
    try {
        limparPesquisaCursoFK(true);
        apresentaMensagem("apresentadorMensagem", null);
        dijit.byId("statusCursoFK").set("disabled", true);
        var cd_contrato = (dijit.byId("tpContrato").value != null && dijit.byId("tpContrato").value != undefined && dijit.byId("tpContrato").value > 0) ? dijit.byId("tpContrato").value : 0;
        pesquisarCursoByContratoMovimento();
        dijit.byId("proCurso").show();
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Aluno FK
function retornarAlunoFK() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else {
            dojo.byId("cdAlunoFKMovimento").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("cdPessoaAlunoFKMovimento").value = gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno;
            dojo.byId("noAlunoFKMovimento").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            if (hasValue(gridPesquisaAluno.itensSelecionados[0].cd_pessoa_dependente) &&
                gridPesquisaAluno.itensSelecionados[0].cd_pessoa_dependente != gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno) {
                $("#cdPessoaMvtoCad").val(gridPesquisaAluno.itensSelecionados[0].cd_pessoa_dependente);
                dijit.byId("noPessoaMovto").set("value", gridPesquisaAluno.itensSelecionados[0].no_pessoa_dependente);
            } else {
                $("#cdPessoaMvtoCad").val(gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno);
                dijit.byId("noPessoaMovto").set("value", gridPesquisaAluno.itensSelecionados[0].no_pessoa);
            }

            dijit.byId("limparPessoaFKMovimento").set("disabled", false);
            dijit.byId('limparAlunoFKMovimento').set("disabled", false);
            dijit.byId("proAluno").hide();

            gerar_titulo = true;
            if (dijit.byId("ckNotaFiscal").checked) {
                verificaOperacaoCFOP();
            }

            getContratosSemTurmaByAluno();

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getContratosSemTurmaByAluno() {
    try {
        var cdAluno = dojo.byId("cdAlunoFKMovimento").value;
        dojo.xhr.get({
            url: Endereco() + "/api/Secretaria/getContratosSemTurmaByAlunoSearch?cd_aluno=" + cdAluno + "&semTurma=true&situacaoTurma=1&nmContrato=0&tipo=0&tipoC=4&status=1",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                debugger
                    apresentaMensagem("apresentadorMensagemMovto", null, true);
                    data = jQuery.parseJSON(data).retorno;
                    if (hasValue(data)) {
                        dijit.byId("tpContrato")._onChangeActive = false;
                        criarOuCarregarCompFiltering("tpContrato", data, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_contrato', 'no_contrato');
                        dijit.byId("tpContrato")._onChangeActive = true;
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
                showCarregando();
            });




        
            
    } catch (e) {

    } 
}


function retornarCursoFK() {
    try {
        var gridPesquisaCurso = dijit.byId("gridPesquisaCurso");
        if (!hasValue(gridPesquisaCurso.itensSelecionados) || gridPesquisaCurso.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaCurso.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else {
            dojo.byId("cdCursoFKMovimento").value = gridPesquisaCurso.itensSelecionados[0].cd_curso;
            dojo.byId("noCursoFKMovimento").value = gridPesquisaCurso.itensSelecionados[0].no_curso;
            

            dijit.byId('limparCursoFKMovimento').set("disabled", false);
            dijit.byId("proCurso").hide();
            gerar_titulo = true;
            if (dijit.byId("ckNotaFiscal").checked)
                verificaOperacaoCFOP();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}




function pesquisarCursoByContratoMovimento() {
    try {
        var cd_contrato = (hasValue(dijit.byId('tpContrato').value)) ? dijit.byId('tpContrato').value : 0;
        require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try {

                if (!hasValue(dijit.byId("cbPesqProdutoFK").value) && hasValue(dijit.byId("cbPesqProdutoFK").produto_selecionado))
                    dijit.byId("cbPesqProdutoFK").set("value", dijit.byId("cbPesqProdutoFK").produto_selecionado);
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/curso/getCursoByContratoSearch?cd_contrato=" + cd_contrato + "&desc=" + encodeURIComponent(document.getElementById("descCursoFK").value) + "&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=" + retornaStatus("statusCursoFK") + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                    ), Memory({}));
                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaCurso");
                grid.setStore(dataStore);
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

function pesquisarAlunoFKMovimento(pesquisaHabilitada) {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var cd_pessoa_responsavel = hasValue(dojo.byId("cdPessoaMvtoCad").value) ? dojo.byId("cdPessoaMvtoCad").value : 0;
        if (pesquisaHabilitada)
            require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getAlunoSearchFKPesquisas?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" +
                                dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" +
                                dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked +
                                "&origemFK=0" +
                                "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_pessoa_responsavel=" + cd_pessoa_responsavel,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_aluno"
                        }), Memory({ idProperty: "cd_aluno" }));
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var gridAluno = dijit.byId("gridPesquisaAluno");
                    gridAluno.itensSelecionados = [];
                    gridAluno.setStore(dataStore);
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

//Métodos da baixa
function setarEventosBotoesPrincipaisCadTransacao(xhr, on) {
    try {
        dijit.byId("incluirBaixa").on("click", function () {
            incluirBaixa(xhr, dojox.json.ref);
        });

        dijit.byId("alterarBaixa").on("click", function () {
            alterarBaixa(xhr, dojox.json.ref);
        });

        dijit.byId("deleteBaixa").on("click", function () {
            caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarTransFinan(xhr, dojox.json.ref, dojo.store.Memory, dojo.data.ObjectStore) });
        });

        dijit.byId("fecharBaixa").on("click", function () {
            dijit.byId("cadBaixaFinanceira").hide();
        })

        dijit.byId("cancelarBaixa").on("click", function () {
            limparCamposBaixaCad();
            eventoEditarBaixaTitulo(dijit.byId("gridBaixa").itensSelecionados, xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, on);
        });

        dijit.byId("limparBaixa").on("click", function () {
            var gridTitulo = dijit.byId('gridTitulo');
            var itensSelecionados = gridTitulo.itensSelecionados;
            showCarregando();
            limparCamposBaixaCad();
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            simularBaixaTitulos(itensSelecionados, xhr, dojox.json.ref, Permissoes);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function buscarBaixasTitulos(item) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getBaixaTituloByCodTitulo?cd_titulo=" + item.cd_titulo,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            limparGridBaixas();
            apresentaMensagem("apresentadorMensagem", null);
            var gridBaixa = dijit.byId("gridBaixa");
            data = jQuery.parseJSON(data).retorno;
            if (!hasValue(data) || data.length <= 0)
                data = null;
            gridBaixa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
        showCarregando();
    });
}

function setarTagTitulo(value) {
    require([
    "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                if (value == true) {
                    dojo.byId('paBaixa').style.height = '200px';
                    dojo.byId("gridTitulo").style.height = '250px';
                    dijit.byId("gridTitulo").set("height", '250px');
                    dijit.byId("gridTitulo").attr("height", '250px');
                    dijit.byId("gridTitulo").currentPageSize(7);
                    dijit.byId('gridTitulo').resize(true);
                }
                else {
                    dojo.byId('paBaixa').style.height = '27px';
                    dojo.byId("gridTitulo").style.height = '310px';
                    dijit.byId("gridTitulo").set("height", '3100px');
                    dijit.byId("gridTitulo").attr("height", '3100px');
                    dijit.byId("gridTitulo").currentPageSize(11);
                    dijit.byId('gridTitulo').resize(true);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function incluirBaixa(xhr, ref) {
    try {
        apresentadorMensagem = "apresentadorMensagemMat";
        apresentaMensagem(apresentadorMensagem, null);

        if (!dijit.byId("formCadBaixa").validate()) {
            dijit.byId("tgCheque").set("open", true);
            return false;
        }
        var bool_troca = false;
        var msg_aviso = "";

        var transacao = montaListaBaixa();
        if (transacao.Baixas[0].Titulo.cd_tipo_financeiro != CARTAO && transacao.Baixas[0].Titulo.cd_tipo_financeiro != CHEQUE) {
            if (transacao.cd_tipo_liquidacao === CARTAOCREDITO || transacao.cd_tipo_liquidacao === CARTAODEBITO || transacao.cd_tipo_liquidacao === CHEQUEVISTA || transacao.cd_tipo_liquidacao === CHEQUEPREDATADO) {
                bool_troca = true;
                msg_aviso = transacao.cd_tipo_liquidacao === CARTAOCREDITO || transacao.cd_tipo_liquidacao === CARTAODEBITO ? "cartão" : "cheque"
                transacao.cd_tipo_liquidacao = TROCA_FINANCEIRA;
            }
        }
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postIncluirTransacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(transacao)
        }).then(function (data) {
            try {
                apresentaMensagem(apresentadorMensagem, data);
                data = jQuery.parseJSON(data).retorno;

                //Atualizar os titulos dos itens baixados para configurar fechados:
                var itemAlterado = data;
                var todos = dojo.byId("todosItens_label");
                var gridName = 'gridTitulo';
                var grid = dijit.byId(gridName);

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = new Array();

                dijit.byId("cadBaixaFinanceira").hide();

                for (var i = 0; i < data.titulosBaixa.length; i++) {
                    removeObjSort(grid.itensSelecionados, "cd_titulo", data.titulosBaixa[i].cd_titulo);
                    insertObjSort(grid.itensSelecionados, "cd_titulo", data.titulosBaixa[i]);
                    for (var j = 0; j < grid.store.objectStore.data.length; j++) {
                        var cd_titulo = grid.store.objectStore.data[j].cd_titulo;
                        if (cd_titulo == data.titulosBaixa[i].cd_titulo) {
                            grid.store.objectStore.data[j].vl_saldo_titulo = data.titulosBaixa[i].vl_saldo_titulo;
                            grid.store.objectStore.data[j].vlSaldoTitulo = data.titulosBaixa[i].vlSaldoTitulo;
                            grid.store.objectStore.data[j].possuiBaixa = true;

                        }
                    }
                }
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                dijit.byId(grid).itemSelecionado = null;
                dijit.byId(grid).itensSelecionados = null;
                grid.itemSelecionado = data.titulosBaixa[0];
                buscarBaixasTitulos(data.titulosBaixa[0]);
                dijit.byId(grid).update();
                if (bool_troca)
                    caixaDialogo(DIALOGO_AVISO, 'Foi realizada uma troca financeira e gerado um titulo com o mesmo numero, em aberto e tipo ' + msg_aviso + '.', null);
            } catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCadBaixa', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarBaixa(xhr, ref) {
    try {
        apresentadorMensagem = "apresentadorMensagemMat";
        apresentaMensagem(apresentadorMensagem, null);

        if (!dijit.byId("formCadBaixa").validate()) {
            dijit.byId("tgCheque").set("open", true);
            return false;
        }

        var transacao = montaListaBaixa();
        transacao.cd_movimento = dojo.byId("cd_movimento").value;
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postUpdateTransacaoReturnTitulos",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(transacao)
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagemMat', data);
                data = jQuery.parseJSON(data).retorno;
                //Atualizar os titulos dos itens baixados para configurar fechados:
                var itemAlterado = data;
                var todos = dojo.byId("todosItens_label");
                var gridName = 'gridTitulo';
                var grid = dijit.byId(gridName);

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = new Array();

                dijit.byId("cadBaixaFinanceira").hide();
                if (hasValue(data) && hasValue(data.titulosBaixa))
                    for (var i = 0; i < data.titulosBaixa.length; i++) {
                        removeObjSort(grid.itensSelecionados, "cd_titulo", data.titulosBaixa[i].cd_titulo);
                        insertObjSort(grid.itensSelecionados, "cd_titulo", data.titulosBaixa[i]);
                        for (var j = 0; j < grid.store.objectStore.data.length; j++) {
                            var cd_titulo = grid.store.objectStore.data[j].cd_titulo;
                            if (cd_titulo == data.titulosBaixa[i].cd_titulo) {
                                grid.store.objectStore.data[j].vl_saldo_titulo = data.titulosBaixa[i].vl_saldo_titulo;
                                grid.store.objectStore.data[j].vlSaldoTitulo = data.titulosBaixa[i].vlSaldoTitulo;
                            }
                        }
                    }

                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_titulo");
                grid.itemSelecionado = null;
                grid.itensSelecionados = null;
                grid.update();
                limparGridBaixas();
                if (hasValue(data) && hasValue(data.titulosBaixa) && hasValue(data.titulosBaixa[0])) {
                    grid.itemSelecionado = data.titulosBaixa[0];
                    buscarBaixasTitulos(data.titulosBaixa[0]);
                } else
                    showCarregando();
            } catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCadBaixa', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function deletarTransFinan(xhr, ref) {
    try {
        if (dojo.byId('cd_tran_finan').value != 0)
            itensSelecionados = {
                cd_tran_finan: dojo.byId("cd_tran_finan").value,
                cd_movimento: dojo.byId("cd_movimento").value
            };
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/financeiro/postDeleteTransFinanceiraBaixaReturnTitulos",
            headers: { "Accept": "applicatsion/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagemMovto', data);
                limparGridBaixas();
                dijit.byId("cadBaixaFinanceira").hide();
                if ($.parseJSON(data).retorno.titulosBaixa != null && $.parseJSON(data).retorno.titulosBaixa.length > 0) {
                    var gridTitulo = dijit.byId("gridTitulo");

                    titulos = $.parseJSON(data).retorno.titulosBaixa;
                    var dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: titulos }) });
                    gridTitulo.setStore(dataStore);
                    for (var i = 0; i < titulos.length; i++) {
                        removeObjSort(gridTitulo.itensSelecionados, "cd_titulo", titulos[i].cd_titulo);
                        insertObjSort(gridTitulo.itensSelecionados, "cd_titulo", titulos[i]);
                    }
                    gridTitulo.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    gridTitulo.itemSelecionado = null;
                    gridTitulo.itensSelecionados = null;
                    gridTitulo.update();
                }
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            if (!hasValue(dojo.byId("cd_tran_finan").style.display))
                apresentaMensagem('apresentadorMensagemCadBaixa', error);
            else
                apresentaMensagem('apresentadorMensagemMovto', error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function eventoHistoricoBaixaTitulo(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            montarGridHistoricoBaixaTitulo(itensSelecionados[0], xhr);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGridHistoricoBaixaTitulo(baixa, xhr) {
    try {
        xhr.get({
            url: Endereco() + "/api/escola/getLogGeralBaixaTitulo?cd_baixa_titulo=" + baixa.cd_baixa_titulo,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                destroyCreateHistoricoBaixaTitulo();
                apresentaMensagem("apresentadorMensagem", null);
                var gridBaixa = dijit.byId("gridBaixa");
                data = jQuery.parseJSON(data).retorno;
                if (!hasValue(data) || data.length <= 0)
                    data = [];
                else
                    data = clearChildrenLenthZero(data);
                var data = {
                    identifier: 'id',
                    label: 'descricao',
                    items: data
                };

                var store = new dojo.data.ItemFileWriteStore({ data: data });

                var model = new dijit.tree.ForestStoreModel({
                    store: store, childrenAttrs: ['children']
                });

                var layout = [
                  { name: 'Usuário', field: 'descricao', width: '30%' },
                  { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
                  { name: 'Vl.Antigo', field: 'dc_valor_antigo', width: '20%', styles: "text-align: center;" },
                  { name: 'Vl.Novo', field: 'dc_valor_novo', width: '20%', styles: "text-align: center;" },
                  { name: 'Operação', field: 'dc_tipo_log', width: '10%', styles: "text-align: center;" },
                  { name: '', field: 'id', width: '0%', styles: "display: none;" }
                ];

                var gridHistoricoBaixaTitulo = new dojox.grid.LazyTreeGrid({
                    id: 'gridHistoricoBaixaTitulo',
                    treeModel: model,
                    structure: layout,
                    noDataMessage: msgNotRegEnc
                }, document.createElement('div'));

                dojo.byId("gridHistoricoBaixaTitulo").appendChild(gridHistoricoBaixaTitulo.domNode);
                gridHistoricoBaixaTitulo.canSort = function (col) { return false; };
                gridHistoricoBaixaTitulo.startup();
                if (hasValue(dojo.byId("historicoBaixaTitulo_title")) && hasValue(dijit.byId("gridBaixa").itenSelecionado))
                    dojo.byId("historicoBaixaTitulo_title").innerHTML = "Histórico Baixa de : " + baixa.dta_baixa + ", " + "Título: " + dijit.byId("gridBaixa").itenSelecionado.nm_titulo + " Parcela " +
                        dijit.byId("gridBaixa").itenSelecionado.nm_parcela_titulo;
                else
                    dojo.byId("historicoBaixaTitulo_title").innerHTML = "Histórico Baixa";
                dijit.byId("historicoBaixaTitulo").show();
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateHistoricoBaixaTitulo() {
    try {
        if (hasValue(dijit.byId("gridHistoricoBaixaTitulo"))) {
            dijit.byId("gridHistoricoBaixaTitulo").destroy();
            //$('<div>').attr('id', 'gridHistoricoBaixaTitulo').attr('style', 'height:100%;').attr('style', 'min-height:200px;').appendTo('#paiGridHistBaixaTitulo');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clearChildrenLenthZero(dataRetorno) {
    try {
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

function buscarBaixasTitulos(item) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getBaixaTituloByCodTitulo?cd_titulo=" + item.cd_titulo,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            limparGridBaixas();
            apresentaMensagem("apresentadorMensagem", null);
            var gridBaixa = dijit.byId("gridBaixa");
            data = jQuery.parseJSON(data).retorno;
            if (!hasValue(data) || data.length <= 0)
                data = null;
            gridBaixa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
        showCarregando();
    });
}

function abrirBaixa() {
    var apresentadorMensagem = "apresentadorMensagemMovto";
    apresentaMensagem(apresentadorMensagem, null);
    var mensagensWeb = new Array();

    var gridTitulo = dijit.byId('gridTitulo');
    var titulos = gridTitulo.store.objectStore.data;
    for (var i = 0; i < titulos.length; i++) {
        if (titulos[i].cd_titulo <= 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSalvo);
            apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
            return false;
        }
    }
    if (hasValue(gridTitulo.itensSelecionados)) {
        gridTitulo.itemSelecionado = gridTitulo.itensSelecionados[0];
        for (var j = 0; j < gridTitulo.itensSelecionados.length; j++) {
            if (gridTitulo.itensSelecionados[j].vl_saldo_titulo <= 0) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloBaixado);
                apresentaMensagem("apresentadorMensagemMovto", mensagensWeb);
                return false;
            }
        }
    }
    mostrarCadastroBaixaFinanceira(true, gridTitulo, null, dojo.xhr, dojox.json.ref);
}

function montarObjItemMovimento(gridItemMovt, value, id) {
    insertObjSort(gridItemMovt.store.objectStore.data, "id", {
        CFOP: value.CFOP,
        Item: value.Item,
        Movimento: value.Movimento,
        PlanoConta: value.PlanoConta,
        SituacaoTribCOFINS: value.SituacaoTribCOFINS,
        SituacaoTribICMS: value.SituacaoTribICMS,
        SituacaoTribPIS: value.SituacaoTribPIS,
        cd_cfop: value.cd_cfop,
        cd_item: value.cd_item,
        cd_item_kit: value.cd_item_kit > 0 ? value.cd_item_kit : null,
        cd_item_movimento: value.cd_item_movimento,
        cd_movimento: value.cd_movimento,
        cd_pessoa_cliente: value.cd_pessoa_cliente,
        cd_plano_conta: value.cd_plano_conta,
        cd_grupo_estoque: value.cd_grupo_estoque,
        cd_tipo_item: value.cd_tipo_item,
        cd_situacao_tributaria_COFINS: value.cd_situacao_tributaria_COFINS,
        cd_situacao_tributaria_ICMS: value.cd_situacao_tributaria_ICMS,
        cd_situacao_tributaria_PIS: value.cd_situacao_tributaria_PIS,
        dc_cfop: value.dc_cfop,
        dc_item_movimento: value.dc_item_movimento,
        dc_plano_conta: value.dc_plano_conta,
        dt_emissao_movimento: value.dt_emissao_movimento,
        dta_emissao_movimento: value.dta_emissao_movimento,
        id: id,
        id_nf_item: value.id_nf_item,
        nm_cfop: value.nm_cfop,
        nm_movimento: value.nm_movimento,
        no_item: value.no_item,
        pcAliquotaICMSInvariante: value.pcAliquotaICMSInvariante,
        pcDescontoItem: value.pcDescontoItem,
        pc_aliquota_COFINS: value.pc_aliquota_COFINS,
        pc_aliquota_ICMS: value.pc_aliquota_ICMS,
        pc_aliquota_IPI: value.pc_aliquota_IPI,
        pc_aliquota_ISS: value.pc_aliquota_ISS,
        pc_aliquota_PIS: value.pc_aliquota_PIS,
        pc_aliquota_aproximada: value.pc_aliquota_aproximada,
        pc_desconto: value.pc_desconto,
        pc_desconto_item: value.pc_desconto_item,
        planoSugerido: value.planoSugerido,
        qtItemMovimento: value.qtItemMovimento,
        qt_item_movimento: value.qt_item_movimento,
        qt_item_movimento_dev: value.qt_item_movimento_dev,
        vlAcrescimoItem: value.vlAcrescimoItem,
        vlAproximado: value.vlAproximado,
        vlBaseCalculoICMSItemInvariante: value.vlBaseCalculoICMSItemInvariante,
        vlDescontoItem: value.vlDescontoItem,
        vlICMSItemInvariante: value.vlICMSItemInvariante,
        vlLiquidoItem: value.vlLiquidoItem,
        vlTotalItem: value.vlTotalItem,
        vlUnitarioItem: value.vlUnitarioItem,
        vlUnitarioItemInvariante: value.vlUnitarioItemInvariante,
        vl_COFINS_item: value.vl_COFINS_item,
        vl_ICMS_item: value.vl_ICMS_item,
        vl_IPI_item: value.vl_IPI_item,
        vl_ISS_item: value.vl_ISS_item,
        vl_PIS_item: value.vl_PIS_item,
        vl_acrescimo_item: value.vl_acrescimo_item,
        vl_aproximado: value.vl_aproximado,
        vl_base_calculo_COFINS_item: value.vl_base_calculo_COFINS_item,
        vl_base_calculo_ICMS_item: value.vl_base_calculo_ICMS_item,
        vl_base_calculo_IPI_item: value.vl_base_calculo_IPI_item,
        vl_base_calculo_ISS_item: value.vl_base_calculo_ISS_item,
        vl_base_calculo_PIS_item: value.vl_base_calculo_PIS_item,
        vl_desconto_item: value.vl_desconto_item,
        vl_liquido_item: value.vl_liquido_item,
        vl_total_item: value.vl_total_item,
        vl_unitario_item: value.vl_unitario_item,
        vlr_liquido_item: value.vlr_liquido_item,
        id_material_didatico: value.id_material_didatico,
        id_voucher_carga: value.id_voucher_carga
    });
}

function loadMeioPagamento(cd_meio_pagamento) {
    var dados = [
    { id: "01", name: "Dinheiro" },
    { id: "02", name: "Cheque" },
    { id: "03", name: "Cartão de Crédito" },
    { id: "04", name: "Cartão de Débito" },
    { id: "05", name: "Crédito Loja" },
    { id: "10", name: "Vale Alimentação" },
    { id: "11", name: "Vale Refeição" },
    { id: "12", name: "Vale Presente" },
    { id: "13", name: "Vale Combustível" },
    { id: "15", name: "Boleto Bancário" },
    { id: "90", name: "Sem Pagamento" }
    ];
    criarOuCarregarCompFiltering("cad_meio_pagamento", dados, "", cd_meio_pagamento, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
}

function abrirDialogIncluirItem(xhr, Memory, FilteringSelect, array, ready) {
    try {
        if (TIPOMOVIMENTO == SAIDA && dojo.byId('divKit').style.display == "block") {
            abrirKitFK(xhr, Memory, FilteringSelect, array, ready);
        } else {
            dijit.byId("dialogItem").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirKitFK(xhr, Memory, FilteringSelect, array, ready) {
    dojo.byId("tipoPesquisaFKItem").value = PESQUISA;
    if (!hasValue(dijit.byId("gridPesquisaItem"))) {

        convertDialogItemKit("Pesquisar Kit", true, true, true);
        montargridPesquisaItem(function () {
            try {
                dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                limparPesquisaCursoFK(false);
                dijit.byId("tipo").reset();
                chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, true);
                //abrirItemFK(xhr, Memory, FilteringSelect, array)
                dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, true); });
                dijit.byId("pesquisarItemFK").on("click", function (e) {
                    try {
                        apresentaMensagem("apresentadorMensagemItemFK", null);
                        var tipoPesquisaFKItem = dojo.byId("tipoPesquisaFKItem");
                        if (hasValue(tipoPesquisaFKItem.value))
                            chamarPesquisaItemFK(tipoPesquisaFKItem.value, xhr, Memory, FilteringSelect, array, ready, true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        }, false, true, true);
    }
    else {
        convertDialogItemKit("Pesquisar Kit", true, true, true);
        limparPesquisaCursoFK(false);
        dijit.byId("tipo").reset();
        chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, true);
        populaGrupoEstoque(null, 'grupoPes', true);
    }
}

function populaGridKitNota(kitsNota) {


    var newItem = {
        cd_item_kit: kitsNota[0].cd_item,
        cd_movimento: hasValue(dojo.byId("cd_movimento").value) ? dojo.byId("cd_movimento").value : 0,
        no_item_kit: kitsNota[0].no_item,
        qt_item_kit: 1,
        cd_item_movimento_kit: 0
    };

    var dados = dijit.byId("gridKit").store.objectStore.data;

    if (binaryObjSearch(dados, 'cd_item_kit', eval('kitsNota[0].cd_item')) == null) {

        quickSortObj(dados, 'cd_item_kit');
        insertObjSort(dados, "cd_item_kit", newItem, false);
        dijit.byId("gridKit")
            .setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));

        obterItemsKit(kitsNota[0].cd_item);
    }
}

function obterItemsKit(cd_item_kit) {
    var id_natureza_mov = 0;
    if (TIPOMOVIMENTO == DEVOLUCAO || TIPOMOVIMENTO == SERVICO)
        id_natureza_mov = dojo.byId("id_natureza_movto").value;
    showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/escola/obterListaItemsKitMov?cd_item_kit=" + cd_item_kit + "&id_tipo_movto=" + TIPOMOVIMENTO + "&id_natureza_TPNF=" + parseInt(id_natureza_mov),
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (items) {

        try {
            hideCarregando();
            if (hasValue(items)) {
                var itensKit = items.retorno;
                $.each(itensKit, function (k, value) {
                    //preencheImpostos();
                    if (dijit.byId("ckNotaFiscal").checked) {
                        pushItemGrid(itensKit[k], NOVO);

                    } else {
                        populaObjetoItem(itensKit[k]);
                        if (itensKit[k].qt_item_kit != undefined) {
                            calcularItem(itensKit[k].qt_item_kit);

                        } else if (itensKit[k].qt_item_movimento != undefined) {
                            calcularItem(itensKit[k].qt_item_movimento);

                        }
                        incluirItemGrade(true);
                    }
                });
            }
        } catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        hideCarregando();
        apresentaMensagem('apresentadorMensagemItemFK', error);
    });
}

function pushItemGrid(itemKit, operacao) {
    /*
     * Começo
     */
    var cdPessoaMovimento = hasValue(dojo.byId("cdPessoaMvtoCad").value) && dojo.byId("cdPessoaMvtoCad").value > 0 ? dojo.byId("cdPessoaMvtoCad").value : 0;
    if (cdPessoaMovimento > 0)
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getTributacaoNFProduto?cd_pessoa_movimento=" + cdPessoaMovimento,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataTributacaoNFProduto) {
            var cd_sit_trib_ICMS_tp_nt = 0;
            var cd_tipo_nf = hasValue(dojo.byId("cd_tp_nf").value) ? dojo.byId("cd_tp_nf").value : 0;
            if (cd_tipo_nf > 0)
                cd_sit_trib_ICMS_tp_nt = hasValue(dojo.byId("cd_sit_trib_ICMS_tp_nt").value) ? dojo.byId("cd_sit_trib_ICMS_tp_nt").value : 0;
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/getSituacaoTributariaItem?cd_grupo_estoque=" + itemKit.cd_grupo_estoque + "&id_regime_tributario=" + regime_tributario + "&cdSitTrib=" + cd_sit_trib_ICMS_tp_nt,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataSituacaoTributariaItem) {
                try {
                    //if (hasValue(pFuncao))
                    //    pFuncao.call();
                    var sitTribGrupItem = jQuery.parseJSON(dataSituacaoTributariaItem).retorno;
                    apresentaMensagem("apresentadorMensagemItem", null);
                    try {

                        /*
                         * Funcao Anonima
                         */
                        try {
                            var gridItem = dijit.byId("gridItem");
                            if (hasValue(gridItem.editItem)) {
                                loadSituacao(gridItem.editItem.situacoesTributariaICMS, 0);
                                //criarOuCarregarCompFiltering("sitTribItem", gridItem.editItem.situacoesTributariaICMS, "", null, dojo.ready, dojo.store.Memory,
                                //                              dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                if (hasValue(gridItem.editItem.situacoesTributariaPIS))
                                    if (regime_tributario == REGIME_NORMAL)
                                        criarOuCarregarCompFiltering("cbStTribPis",
                                            gridItem.editItem.situacoesTributariaPIS,
                                            "",
                                            SITUACAOTRIBUTARIAPIS,
                                            dojo.ready,
                                            dojo.store.Memory,
                                            dijit.form.FilteringSelect,
                                            'cd_situacao_tributaria',
                                            'dc_situacao_tributaria');
                                    else
                                        criarOuCarregarCompFiltering("cbStTribPis",
                                            gridItem.editItem.situacoesTributariaPIS,
                                            "",
                                            SITUACAOTRIBUTARIAPIS_OUTRASOP,
                                            dojo.ready,
                                            dojo.store.Memory,
                                            dijit.form.FilteringSelect,
                                            'cd_situacao_tributaria',
                                            'dc_situacao_tributaria');
                                if (hasValue(gridItem.editItem.situacoesTributariaCOFINS))
                                    if (regime_tributario == REGIME_NORMAL)
                                        criarOuCarregarCompFiltering("cbStTribCof",
                                            gridItem.editItem.situacoesTributariaCOFINS,
                                            "",
                                            SITUACAOTRIBUTARIACOFINS,
                                            dojo.ready,
                                            dojo.store.Memory,
                                            dijit.form.FilteringSelect,
                                            'cd_situacao_tributaria',
                                            'dc_situacao_tributaria');
                                    else
                                        criarOuCarregarCompFiltering("cbStTribCof",
                                            gridItem.editItem.situacoesTributariaCOFINS,
                                            "",
                                            SITUACAOTRIBUTARIACOFINS_OUTRASOP,
                                            dojo.ready,
                                            dojo.store.Memory,
                                            dijit.form.FilteringSelect,
                                            'cd_situacao_tributaria',
                                            'dc_situacao_tributaria');
                            }
                            if (hasValue(dojo.byId("cd_cfop_nf").value)) {
                                dojo.byId("cd_CFOP_item").value = dojo.byId("cd_cfop_nf").value;
                                dijit.byId("descCFOPItem").set("value", dijit.byId("CFOP").value);
                                dojo.byId("descCFOPItem").value = dijit.byId("CFOP").value;
                            }
                            dijit.byId("operacaoCFOPItem")
                                .set("value", dijit.byId("operacaoCFOP").value);

                            if (TIPOMOVIMENTO == SAIDA && regime_tributario != REGIME_NORMAL)
                                dijit.byId("pc_aliquota_ap_item").set("value", pcAliqAproxSaida);



                        } catch (e) {
                            postGerarLog(e);
                        }
                        /*
                        * Fim Funcao Anonima
                        */
                        var aliqICMS = jQuery.parseJSON(dataTributacaoNFProduto).retorno;
                        apresentaMensagem("apresentadorMensagemItem", null);
                        if (hasValue(aliqICMS)) {
                            dijit.byId("aliquotaICMSItem").set("value", aliqICMS.pc_aliq_icms_padrao);
                            dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                            var baseICMS = dijit.byId("baseCalcICMSItem");
                            if (baseICMS.value > 0 && aliqICMS.pc_aliq_icms_padrao > 0) {
                                var vlICMS = (baseICMS.value * aliqICMS.pc_aliq_icms_padrao) / 100;
                                dijit.byId("valorICMSItem").set("value", vlICMS);
                            }

                        }

                        /*
                         * Return ItemKit
                         */
                        try {
                            if (!hasValue(itemKit))
                                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                            else {
                                if (!dijit.byId("cadMovimento").open) {
                                    dojo.byId("cdItem").value = itemKit.cd_item.cd_item;
                                    dojo.byId("noItemPesq").value = gridPesquisaItem.itensSelecionados[0].no_item;
                                    dijit.byId("limparItem").set("disabled", false);
                                } else {
                                    if (hasValue(origem) && origem == MOVIMENTO_DEVOLUCAO) {
                                        dojo.byId("cdItemFK").value = itemKit.cd_item;
                                        if (hasValue(itemKit.no_item)) {
                                            dojo.byId("noItemPesqFK").value = itemKit.no_item;
                                        } else if (hasValue(itemKit.dc_item_movimento)) {
                                            dojo.byId("noItemPesqFK").value = itemKit.dc_item_movimento;
                                        }

                                        dijit.byId("limparItemFK").set("disabled", false);
                                        if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                            var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                            dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                        }
                                    }
                                    else {
                                        var vlUnitario = dijit.byId("vlUnitario");
                                        var vlTotal = dijit.byId("vlTotalMovimento");
                                        var vlLiquido = dijit.byId("vlLiquido");

                                        vlUnitario._onChangeActive = false;
                                        vlTotal._onChangeActive = false;
                                        vlLiquido._onChangeActive = false;
                                        // grade de itens
                                        dojo.byId("cd_item").value = itemKit.cd_item;
                                        dojo.byId("cd_item_kit").value = itemKit.cd_item_kit;
                                        if (hasValue(itemKit.no_item)) {
                                            dojo.byId("desc_item").value = itemKit.no_item;
                                        } else if (hasValue(itemKit.dc_item_movimento)) {
                                            dojo.byId("desc_item").value = itemKit.dc_item_movimento;
                                        }

                                        var tipo = eval(getParamterosURL()['tipo']);
                                        if (tipo == DEVOLUCAO)
                                            tipo = dojo.byId('id_natureza_movto').value;

                                        if (tipo != ENTRADA) {



                                            if (hasValue(itemKit.vl_item)) {

                                                vlUnitario.set("value", itemKit.vl_item);
                                                vlUnitario.value = itemKit.vl_item;
                                                vlUnitario.oldValue = itemKit.vl_item;

                                                vlTotal.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_item);
                                                vlTotal.value = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                                                vlTotal.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_item;

                                                vlLiquido.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_item);
                                                vlLiquido.value = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                                                vlLiquido.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                                            } else if (hasValue(itemKit.vl_unitario_item)) {
                                                vlUnitario.set("value", itemKit.vl_unitario_item);
                                                vlUnitario.value = itemKit.vl_unitario_item;
                                                vlUnitario.oldValue = itemKit.vl_unitario_item;

                                                vlTotal.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item);
                                                vlTotal.value = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                                                vlTotal.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;

                                                vlLiquido.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item);
                                                vlLiquido.value = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                                                vlLiquido.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                                            }




                                        }
                                        if (hasValue(itemKit.cd_plano_conta) && itemKit.cd_plano_conta > 0) {
                                            dojo.byId("cd_plano_contas").value = itemKit.cd_plano_conta;
                                            if (itemKit.desc_plano_conta) {
                                                dojo.byId("descPlanoConta").value = itemKit.desc_plano_conta;

                                            } else if (itemKit.dc_plano_conta) {
                                                dojo.byId("descPlanoConta").value = itemKit.dc_plano_conta;
                                            }
                                            dijit.byId("cadPlanoConta").set("disabled", true);
                                            plano_conta_automatico = true;

                                        }
                                        else
                                            plano_conta_automatico = false;
                                        vlUnitario._onChangeActive = true;
                                        vlTotal._onChangeActive = true;
                                        vlLiquido._onChangeActive = true;
                                        if (dijit.byId("ckNotaFiscal").checked)
                                            switch (TIPOMOVIMENTO) {
                                                case ENTRADA:
                                                case SAIDA:
                                                case DEVOLUCAO:
                                                    dijit.byId("baseCalcPISItem").set("value", vlLiquido.value);
                                                    dijit.byId("baseCalcCOFINSItem").set("value", vlLiquido.value);
                                                    dijit.byId("baseCalcIPIItem").set("value", vlLiquido.value);
                                                    var reducao = dijit.byId('tpNf').reducao;
                                                    if (hasValue(reducao) && reducao > 0) {
                                                        var baseReduzida = vlLiquido.value - ((vlLiquido.value * reducao) / 100);
                                                        dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                    }
                                                    else {
                                                        dijit.byId("baseCalcICMSItem").set("value", vlLiquido.value);
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                    }
                                                    if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                        dijit.byId("baseCalcICMSItem").set("value", 0);
                                                    }

                                                    /**
                                                     * **Situacao
                                                     */
                                                    try {


                                                        if (hasValue(sitTribGrupItem)) {

                                                            dijit.byId("sitTribItem").set("value", sitTribGrupItem.cd_situacao_tributaria);

                                                            //dijit.byId("baseCalcICMSItem").set("value", 0);
                                                            //dijit.byId("aliquotaICMSItem").set("value", 0);
                                                            //dijit.byId("aliquotaICMSItem").set("disabled", true);
                                                            //dijit.byId("valorICMSItem").set("value", 0);
                                                            if (hasValue(dijit.byId("aliquotaICMSItem").old_value))
                                                                dijit.byId("aliquotaICMSItem").valueFixo = dijit.byId("aliquotaICMSItem").old_value;
                                                            if (hasValue(dijit.byId("baseCalcICMSItem").value))
                                                                dijit.byId("baseCalcICMSItem").valueFixo = dijit.byId("baseCalcICMSItem").value;

                                                            if (sitTribGrupItem.id_forma_tributacao == ISENTO) {
                                                                dijit.byId("baseCalcICMSItem").set("value", 0);
                                                                dijit.byId("baseCalcICMSItem").old_value = 0;
                                                                dijit.byId("baseCalcICMSItem").set("disabled", true);
                                                                dijit.byId("aliquotaICMSItem").set("value", 0);
                                                                dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                                                                dijit.byId("aliquotaICMSItem").set("disabled", true);
                                                                dijit.byId("valorICMSItem").set("value", 0);
                                                                dijit.byId("valorICMSItem").set("disabled", true);
                                                            }
                                                            if (sitTribGrupItem.id_forma_tributacao == REDUZIDO) {
                                                                var baseICMS = dijit.byId("baseCalcICMSItem").value;
                                                                var valorReduzido = (baseICMS * dijit.byId('tpNf').reducao) / 100;
                                                                baseICMS = baseICMS - valorReduzido;
                                                                dijit.byId("baseCalcICMSItem").set("value", baseICMS);
                                                                dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                                if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                                                    dijit.byId("baseCalcICMSItem").set("value", 0);
                                                                }
                                                                dijit.byId("aliquotaICMSItem").set("disabled", false);
                                                                dijit.byId("valorICMSItem").set("disabled", false);
                                                            }
                                                        }
                                                        else {
                                                            dijit.byId("sitTribItem").set("value", cd_sit_trib_ICMS_tp_nt);
                                                            dijit.byId("aliquotaICMSItem").set("disabled", false);
                                                        }

                                                        /*
                                                         * Popula Itens
                                                         */
                                                        if (operacao == NOVO) {
                                                            populaObjetoItem(itemKit);
                                                        } else {
                                                            populaMovimentoItem(itemKit);
                                                        }

                                                        if (hasValue(itemKit.qt_item_kit)) {
                                                            calcularItem(itemKit.qt_item_kit);
                                                        } else if (hasValue(itemKit.qt_item_movimento)) {
                                                            calcularItem(itemKit.qt_item_movimento);
                                                        }

                                                        calculaValorItem(itemKit);
                                                        incluirItemGrade(true);

                                                    }
                                                    catch (e) {
                                                        postGerarLog(e);
                                                    }

                                                    /***
                                                     * Fim Sit
                                                     */
                                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                                        var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                                    }
                                                    break;
                                                case SERVICO:
                                                    var baseISS = dijit.byId("baseCalcISSItem");
                                                    baseISS.set("value", vlLiquido.value);
                                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                                        var vlAprox = baseISS.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                                    }

                                                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                                        var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                                    }


                                                    if (operacao == NOVO) {
                                                        populaObjetoItem(itemKit);
                                                    } else {
                                                        populaMovimentoItem(itemKit);
                                                    }

                                                    if (hasValue(itemKit.qt_item_kit)) {
                                                        calcularItem(itemKit.qt_item_kit);
                                                    } else if (hasValue(itemKit.qt_item_movimento)) {
                                                        calcularItem(itemKit.qt_item_movimento);
                                                    }
                                                    calculaValorItem(itemKit);
                                                    incluirItemGrade(true);
                                                    break;
                                            }


                                    }

                                }

                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                        /*
                         * Fim return ItemKit
                        */
                    }
                    catch (e) {
                        postGerarLog(e);
                    }

                }
                catch (e) {
                    postGerarLog(e);
                }

            },
        function (error) {
            apresentaMensagem("apresentadorMensagemItem", error);
        });
        },
    function (error) {
        apresentaMensagem("apresentadorMensagemItem", error);
    });

    /*Fim*/
}

function populaObjetoItem(item) {
    dojo.byId("cdItem").value = item.cd_item;
    dojo.byId("cd_item").value = item.cd_item;

    dojo.byId("cd_item_kit").value = item.cd_item_kit > 0 ? item.cd_item_kit : null;
    dojo.byId("desc_item").value = item.no_item;

    if (hasValue(item.ItemSubgrupoDoItemNoKit) && hasValue(item.ItemSubgrupoDoItemNoKit.no_subgrupo)) {
        dijit.byId("descPlanoConta").set("value", item.ItemSubgrupoDoItemNoKit.no_subgrupo);
    }
    if (hasValue(item.ItemSubgrupoDoItemNoKit) && hasValue(item.ItemSubgrupoDoItemNoKit.cd_plano_conta)) {
        dojo.byId("cd_plano_contas").value = item.ItemSubgrupoDoItemNoKit.cd_plano_conta;
    }

    if (hasValue(item.desc_plano_conta)) {
        dijit.byId("descPlanoConta").set("value", item.desc_plano_conta);
    }
    if (hasValue(item.cd_plano_conta)) {
        dojo.byId("cd_plano_contas").value = item.cd_plano_conta;
    }
    if (hasValue(item.cd_grupo_estoque > 0)) {
        dojo.byId("cd_grupo_estoque").value = item.cd_grupo_estoque;
    }
    dijit.byId("qtd_item").set("value", item.qt_item_kit);
    dijit.byId("vlUnitario").set("value", item.vl_item);
    dijit.byId("perDescontoItem").set("value", 0);
    dijit.byId("valDescontoItem").set("value", 0);
    dijit.byId("vlTotalMovimento").set("value", item.vl_item);
    dijit.byId("vlLiquido").set("value", item.vl_item);
}


function calcularItem(qtd) {
    var compQtd = dijit.byId("qtd_item");
    var vlUnitario = dijit.byId("vlUnitario");
    var compvlTotal = dijit.byId("vlTotalMovimento");
    var compVlLiquido = dijit.byId("vlLiquido");
    var totalItens = 0;
    var vlUnit = 0;
    var item;
    if (isNaN(qtd)) {
        compQtd.set('value', 1);
    } else {
        if (TIPOMOVIMENTO == DEVOLUCAO && qtd > dijit.byId("qtd_item").oldValue) {   //Tirei dijit.byId("ckNotaFiscal").checked && 
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroQtdItemDevMaior);
            apresentaMensagem('apresentadorMensagemItem', mensagensWeb);
            dijit.byId("qtd_item").set("value", dijit.byId("qtd_item").oldValue);
            return false;
        }
        if (vlUnitario.value > 0) {
            totalItens = (qtd * vlUnitario.value);
            if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                dijit.byId('valDescontoItem').set("value", (totalItens * dijit.byId('perDescontoItem').value) / 100);
                dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
            }
            else
                if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                    dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / totalItens);
                    dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                }
                else
                    item = calcularDescAcrescItemView(qtd, totalItens, null, vlUnitario.value);
            compvlTotal._onChangeActive = false;
            compvlTotal.set("value", unmaskFixed(totalItens, 2));
            compvlTotal.value = totalItens;
            compvlTotal.oldValue = unmaskFixed(totalItens, 2);
            compvlTotal._onChangeActive = true;
        } else {
            if (compvlTotal.value > 0) {
                vlUnit = compvlTotal.value / qtd;
                if (hasValue(dijit.byId('perDescontoItem').value) && hasValue(dijit.byId('perDescontoItem').value) > 0) {
                    dijit.byId('valDescontoItem').set("value", (totalItens * dijit.byId('perDescontoItem').value) / 100);
                    dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                }
                else
                    if (hasValue(dijit.byId('valDescontoItem').value) && hasValue(dijit.byId('valDescontoItem').value) > 0) {
                        dijit.byId('perDescontoItem').set("value", (dijit.byId('valDescontoItem').value * 100) / totalItens);
                        dijit.byId("vlLiquido").set("value", (totalItens - dijit.byId('valDescontoItem').value));
                    }
                    else
                        item = calcularDescAcrescItemView(qtd, compvlTotal.value, null, vlUnit);
                vlUnitario._onChangeActive = false;
                vlUnitario.set("value", unmaskFixed(vlUnit, 2));
                vlUnitario.value = vlUnit;
                vlUnitario.oldValue = unmaskFixed(vlUnit, 2);
                vlUnitario._onChangeActive = true;
            }
        }
        if (hasValue(item)) {
            compVlLiquido._onChangeActive = false;
            compVlLiquido.set("value", unmaskFixed(item.vl_liquido_item, 2));
            compVlLiquido.value = item.vl_liquido_item;
            compVlLiquido.oldValue = unmaskFixed(item.vl_liquido_item, 2);
            compVlLiquido._onChangeActive = true;
        }
    }
    if (dijit.byId("ckNotaFiscal").checked) {
        switch (TIPOMOVIMENTO) {
            case ENTRADA:
            case SAIDA:
            case DEVOLUCAO:
                dijit.byId("baseCalcPISItem").set("value", compVlLiquido.value);
                dijit.byId("baseCalcCOFINSItem").set("value", compVlLiquido.value);
                dijit.byId("baseCalcIPIItem").set("value", compVlLiquido.value);
                var reducao = dijit.byId('tpNf').reducao;
                if (hasValue(reducao) && reducao > 0) {
                    var baseReduzida = compVlLiquido.value - ((compVlLiquido.value * reducao) / 100);
                    dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                }
                else {
                    dijit.byId("baseCalcICMSItem").set("value", compVlLiquido.value);
                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                }
                if (dijit.byId("aliquotaICMSItem").value <= 0) {
                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                    dijit.byId("baseCalcICMSItem").set("value", 0);
                }
                break;
            case SERVICO:
                var baseISS = dijit.byId("baseCalcISSItem");
                baseISS.set("value", compVlLiquido.value);
                break;
        }
        if (dijit.byId("pc_aliquota_ap_item").value > 0) {
            var vlAprox = compVlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
            dijit.byId("vl_aproximado_item").set("value", vlAprox);
        }
    }
}



function convertDialogItemKit(title, disabledKit, checkedKit, disabledEstoque) {
    dijit.byId("fkItem").set('title', title);
    dijit.byId("comEstoque").set("disabled", disabledEstoque);
    dijit.byId("kit").set("checked", checkedKit);
    dijit.byId("kit").set("disabled", disabledKit);
}

function setItemsMovimentoKitGrid(ItemsMovimentoKit, ItensMovimento) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dojo/domReady!",
          "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridItemMovt = dijit.byId("gridKit");
            var items = [];
            $.each(ItemsMovimentoKit, function (idx, value) {
                var id = geradorIdItem(gridItemMovt);
                insertObjSort(gridItemMovt.store.objectStore.data, "id", {
                    id: id,
                    cd_item_movimento_kit: value.cd_item_movimento_kit,
                    qt_item_kit: value.qt_item_kit,
                    no_item_kit: value.no_item_kit,
                    cd_item_kit: value.cd_item_kit
                });
            });

            gridItemMovt.setStore(new ObjectStore({ objectStore: new Memory({ data: gridItemMovt.store.objectStore.data }) }));
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function calculaValorItem(item) {

    try {
        baseICMS = dijit.byId("baseCalcICMSItem");
        var vlUnitario = dijit.byId("vlUnitario");
        var vlICMSItem = dijit.byId("valorICMSItem");
        if (baseICMS > 0 && vlUnitario > 0) {
            var pcAliqICMSItem = dijit.byId("aliquotaICMSItem");
            if (pcAliqICMSItem.value > 0) {
                var valorICMS = (baseICMS * pcAliqICMSItem.value) / 100;
                vlICMSItem.set("value", valorICMS);
            } /*else {
                var percentual = (vlICMSItem.value * 100) / baseICMS;
                pcAliqICMSItem.set("value", maskFixed(percentual, 2));
            }*/
        } else {
            vlICMSItem.set("value", 0);
        }

    }
    catch (e) {
        postGerarLog(e);
    }

}

function situacaoTributariaItemKit(cd_grupo_estoque, callback) {
    try {
        var cd_sit_trib_ICMS_tp_nt = 0;
        var cd_tipo_nf = hasValue(dojo.byId("cd_tp_nf").value) ? dojo.byId("cd_tp_nf").value : 0;
        if (cd_tipo_nf > 0)
            cd_sit_trib_ICMS_tp_nt = hasValue(dojo.byId("cd_sit_trib_ICMS_tp_nt").value) ? dojo.byId("cd_sit_trib_ICMS_tp_nt").value : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getSituacaoTributariaItem?cd_grupo_estoque=" + cd_grupo_estoque + "&id_regime_tributario=" + regime_tributario + "&cdSitTrib=" + cd_sit_trib_ICMS_tp_nt,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                //if (hasValue(pFuncao))
                //    pFuncao.call();
                var sitTribGrupItem = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemItem", null);

                if (hasValue(sitTribGrupItem)) {

                    dijit.byId("sitTribItem").set("value", sitTribGrupItem.cd_situacao_tributaria);

                    //dijit.byId("baseCalcICMSItem").set("value", 0);
                    //dijit.byId("aliquotaICMSItem").set("value", 0);
                    //dijit.byId("aliquotaICMSItem").set("disabled", true);
                    //dijit.byId("valorICMSItem").set("value", 0);
                    if (hasValue(dijit.byId("aliquotaICMSItem").old_value))
                        dijit.byId("aliquotaICMSItem").valueFixo = dijit.byId("aliquotaICMSItem").old_value;
                    if (hasValue(dijit.byId("baseCalcICMSItem").value))
                        dijit.byId("baseCalcICMSItem").valueFixo = dijit.byId("baseCalcICMSItem").value;

                    if (sitTribGrupItem.id_forma_tributacao == ISENTO) {
                        dijit.byId("baseCalcICMSItem").set("value", 0);
                        dijit.byId("baseCalcICMSItem").old_value = 0;
                        dijit.byId("baseCalcICMSItem").set("disabled", true);
                        dijit.byId("aliquotaICMSItem").set("value", 0);
                        dijit.byId("aliquotaICMSItem").old_value = dijit.byId("aliquotaICMSItem").value;
                        dijit.byId("aliquotaICMSItem").set("disabled", true);
                        dijit.byId("valorICMSItem").set("value", 0);
                        dijit.byId("valorICMSItem").set("disabled", true);
                    }
                    if (sitTribGrupItem.id_forma_tributacao == REDUZIDO) {
                        var baseICMS = dijit.byId("baseCalcICMSItem").value;
                        var valorReduzido = (baseICMS * dijit.byId('tpNf').reducao) / 100;
                        baseICMS = baseICMS - valorReduzido;
                        dijit.byId("baseCalcICMSItem").set("value", baseICMS);
                        dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                        if (dijit.byId("aliquotaICMSItem").value <= 0) {
                            dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                            dijit.byId("baseCalcICMSItem").set("value", 0);
                        }
                        dijit.byId("aliquotaICMSItem").set("disabled", false);
                        dijit.byId("valorICMSItem").set("disabled", false);
                    }
                }
                else {
                    dijit.byId("sitTribItem").set("value", cd_sit_trib_ICMS_tp_nt);
                    dijit.byId("aliquotaICMSItem").set("disabled", false);
                }

                if (hasValue(callback)) {
                    callback.call();
                }
            }
            catch (e) {
                postGerarLog(e);
            }

        },
            function (error) {
                apresentaMensagem("apresentadorMensagemItem", error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}



function retornarItemKitFK(itemKit, callback) {
    try {
        if (!hasValue(itemKit))
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else {
            if (!dijit.byId("cadMovimento").open) {
                dojo.byId("cdItem").value = itemKit.cd_item.cd_item;
                dojo.byId("noItemPesq").value = gridPesquisaItem.itensSelecionados[0].no_item;
                dijit.byId("limparItem").set("disabled", false);
            } else {
                if (hasValue(origem) && origem == MOVIMENTO_DEVOLUCAO) {
                    dojo.byId("cdItemFK").value = itemKit.cd_item;
                    if (hasValue(itemKit.no_item)) {
                        dojo.byId("noItemPesqFK").value = itemKit.no_item;
                    } else if (hasValue(itemKit.dc_item_movimento)) {
                        dojo.byId("noItemPesqFK").value = itemKit.dc_item_movimento;
                    }

                    dijit.byId("limparItemFK").set("disabled", false);
                    if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                        var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                        dijit.byId("vl_aproximado_item").set("value", vlAprox);
                    }
                }
                else {
                    var vlUnitario = dijit.byId("vlUnitario");
                    var vlTotal = dijit.byId("vlTotalMovimento");
                    var vlLiquido = dijit.byId("vlLiquido");

                    vlUnitario._onChangeActive = false;
                    vlTotal._onChangeActive = false;
                    vlLiquido._onChangeActive = false;
                    // grade de itens
                    dojo.byId("cd_item").value = itemKit.cd_item;
                    if (hasValue(itemKit.no_item)) {
                        dojo.byId("desc_item").value = itemKit.no_item;
                    } else if (hasValue(itemKit.dc_item_movimento)) {
                        dojo.byId("desc_item").value = itemKit.dc_item_movimento;
                    }

                    var tipo = eval(getParamterosURL()['tipo']);
                    if (tipo == DEVOLUCAO)
                        tipo = dojo.byId('id_natureza_movto').value;

                    if (tipo != ENTRADA) {



                        if (hasValue(itemKit.vl_item)) {

                            vlUnitario.set("value", itemKit.vl_item);
                            vlUnitario.value = itemKit.vl_item;
                            vlUnitario.oldValue = itemKit.vl_item;

                            vlTotal.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_item);
                            vlTotal.value = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                            vlTotal.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_item;

                            vlLiquido.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_item);
                            vlLiquido.value = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                            vlLiquido.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_item;
                        } else if (hasValue(itemKit.vl_unitario_item)) {
                            vlUnitario.set("value", itemKit.vl_unitario_item);
                            vlUnitario.value = itemKit.vl_unitario_item;
                            vlUnitario.oldValue = itemKit.vl_unitario_item;

                            vlTotal.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item);
                            vlTotal.value = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                            vlTotal.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;

                            vlLiquido.set("value", dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item);
                            vlLiquido.value = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                            vlLiquido.oldValue = dijit.byId('qtd_item').get('value') * itemKit.vl_unitario_item;
                        }




                    }
                    if (hasValue(itemKit.cd_plano_conta) && itemKit.cd_plano_conta > 0) {
                        dojo.byId("cd_plano_contas").value = itemKit.cd_plano_conta;
                        dojo.byId("descPlanoConta").value = itemKit.desc_plano_conta;
                        dijit.byId("cadPlanoConta").set("disabled", true);
                        plano_conta_automatico = true;
                    }
                    else
                        plano_conta_automatico = false;
                    vlUnitario._onChangeActive = true;
                    vlTotal._onChangeActive = true;
                    vlLiquido._onChangeActive = true;
                    if (dijit.byId("ckNotaFiscal").checked)
                        switch (TIPOMOVIMENTO) {
                            case ENTRADA:
                            case SAIDA:
                            case DEVOLUCAO:
                                dijit.byId("baseCalcPISItem").set("value", vlLiquido.value);
                                dijit.byId("baseCalcCOFINSItem").set("value", vlLiquido.value);
                                dijit.byId("baseCalcIPIItem").set("value", vlLiquido.value);
                                var reducao = dijit.byId('tpNf').reducao;
                                if (hasValue(reducao) && reducao > 0) {
                                    var baseReduzida = vlLiquido.value - ((vlLiquido.value * reducao) / 100);
                                    dijit.byId("baseCalcICMSItem").set("value", baseReduzida);
                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                }
                                else {
                                    dijit.byId("baseCalcICMSItem").set("value", vlLiquido.value);
                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                }
                                if (dijit.byId("aliquotaICMSItem").value <= 0) {
                                    dijit.byId("baseCalcICMSItem").old_value = dijit.byId("baseCalcICMSItem").value;
                                    dijit.byId("baseCalcICMSItem").set("value", 0);
                                }
                                situacaoTributariaItemKit(itemKit.cd_grupo_estoque, callback);
                                if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                    var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                }
                                break;
                            case SERVICO:
                                var baseISS = dijit.byId("baseCalcISSItem");
                                baseISS.set("value", vlLiquido.value);
                                if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                    var vlAprox = baseISS.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                }

                                if (dijit.byId("pc_aliquota_ap_item").value > 0) {
                                    var vlAprox = vlLiquido.value * dijit.byId("pc_aliquota_ap_item").value / 100;
                                    dijit.byId("vl_aproximado_item").set("value", vlAprox);
                                }
                                if (hasValue(callback)) {
                                    callback.call();
                                }
                                break;
                        }


                }

            }

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function preencheImpostos() {
    if (dijit.byId("ckNotaFiscal").checked) {
        switch (TIPOMOVIMENTO) {
            case ENTRADA:
            case SAIDA:
            case DEVOLUCAO:
                dadosFiscaisNFProduto(function () {
                    try {
                        var gridItem = dijit.byId("gridItem");
                        if (hasValue(gridItem.editItem) && !alterou_tp_nf) {
                            loadSituacao(gridItem.editItem.situacoesTributariaICMS, 0);
                            criarOuCarregarCompFiltering("sitTribItem", gridItem.editItem.situacoesTributariaICMS, "", null, dojo.ready, dojo.store.Memory,
                                                          dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                            if (hasValue(gridItem.editItem.situacoesTributariaPIS))
                                if (regime_tributario == REGIME_NORMAL)
                                    criarOuCarregarCompFiltering("cbStTribPis", gridItem.editItem.situacoesTributariaPIS, "", SITUACAOTRIBUTARIAPIS, dojo.ready, dojo.store.Memory,
                                                                  dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                else
                                    criarOuCarregarCompFiltering("cbStTribPis", gridItem.editItem.situacoesTributariaPIS, "", SITUACAOTRIBUTARIAPIS_OUTRASOP, dojo.ready, dojo.store.Memory,
                                                              dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                            if (hasValue(gridItem.editItem.situacoesTributariaCOFINS))
                                if (regime_tributario == REGIME_NORMAL)
                                    criarOuCarregarCompFiltering("cbStTribCof", gridItem.editItem.situacoesTributariaCOFINS, "", SITUACAOTRIBUTARIACOFINS, dojo.ready, dojo.store.Memory,
                                                                 dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                                else
                                    criarOuCarregarCompFiltering("cbStTribCof", gridItem.editItem.situacoesTributariaCOFINS, "", SITUACAOTRIBUTARIACOFINS_OUTRASOP, dojo.ready, dojo.store.Memory,
                                                             dijit.form.FilteringSelect, 'cd_situacao_tributaria', 'dc_situacao_tributaria');
                        }
                        if (hasValue(dojo.byId("cd_cfop_nf").value)) {
                            dojo.byId("cd_CFOP_item").value = dojo.byId("cd_cfop_nf").value;
                            dijit.byId("descCFOPItem").set("value", dijit.byId("CFOP").value);
                            dojo.byId("descCFOPItem").value = dijit.byId("CFOP").value;
                        }
                        dijit.byId("operacaoCFOPItem").set("value", dijit.byId("operacaoCFOP").value);

                        if (TIPOMOVIMENTO == SAIDA && regime_tributario != REGIME_NORMAL)
                            dijit.byId("pc_aliquota_ap_item").set("value", pcAliqAproxSaida);

                        //dijit.byId("dialogItem").show();
                        //abrirDialogIncluirItem(xhr, Memory, FilteringSelect, array, ready);

                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                break;

        }

    }
}

function calcularQuantidadeItemKit(cd_item_kit, qt_item_kit, old_qt_item_kit, ultimo_valor_kit) {

    try {
        var gridItem = dijit.byId("gridItem");
        items = gridItem.store.objectStore.data;
        showCarregando();

        var post = {
            items: items,
            cd_item_kit: cd_item_kit,
            ultimo_valor_kit: ultimo_valor_kit,
            qt_item_kit: qt_item_kit
        }
        dojo.xhr.post({
            url: Endereco() + "/api/escola/calcularQuantidadeItemKit",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(post)
        }).then(function (movimento) {
            try {
                console.log(movimento.retorno);
                gridItem.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                dijit.byId("totalItens").set("value", 0);
                dijit.byId("totalGeral").set("value", 0);

                limpaFormFiscal();
                limparItem();

                pushItemMovimentoGrid(movimento.retorno.ItensMovimento);
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMovto', error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function limpaFormFiscal() {
    //dojo.byId("cd_tp_nf").value = 0;
    //dojo.byId("cd_sit_trib_ICMS_tp_nt").value = 0;
    //dojo.byId("tpNf").value = "";
    //dijit.byId("nfEsc").reset();
    //dijit.byId("operacaoCFOP").reset();
    //dijit.byId("CFOP").reset();
    dijit.byId("baseICMS").set("value", 0);
    dijit.byId("vl_icms").set("value", 0);
    dijit.byId("basePIS").set("value", 0);
    dijit.byId("vl_pis").set("value", 0);
    dijit.byId("baseCOFINS").set("value", 0);
    dijit.byId("vl_COFINS").set("value", 0);
    dijit.byId("baseIPI").set("value", 0);
    dijit.byId("vl_ipi").set("value", 0);
    dijit.byId("pc_aliquota_ap").set("value", 0);
    dijit.byId("vl_aproximado").set("value", 0);
}

function excluirKitDoGrid(cd_item_kit, qt_item_kit) {
    try {
        var gridItem = dijit.byId("gridItem");
        items = gridItem.store.objectStore.data;
        showCarregando();

        var post = {
            items: items,
            cd_item_kit: cd_item_kit,
            qt_item_kit: qt_item_kit
        }

        dojo.xhr.post({
            url: Endereco() + "/api/escola/excluirKitDoGrid",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(post)
        }).then(function (movimento) {
            try {
                console.log(movimento.retorno);
                gridItem.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                dijit.byId("totalItens").set("value", 0);
                dijit.byId("totalGeral").set("value", 0);
                //dijit.byId("vlDesconto").set("value", 0);
                //dijit.byId("pcDesconto").set("value", 0);

                limpaFormFiscal();
                limparItem();
                pushItemMovimentoGrid(movimento.retorno.ItensMovimento);
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMovto', error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}


function pushItemMovimentoGrid(itensKit) {
    if (hasValue(itensKit)) {
        $.each(itensKit, function (k, value) {
            //preencheImpostos();
            if (dijit.byId("ckNotaFiscal").checked) {
                pushItemGrid(itensKit[k], EDITAR);
                //populaMovimentoItem(itensKit[k]);
                //calcularItem(itensKit[k].qt_item_kit);
                //incluirItemGrade(true);

            } else {
                populaMovimentoItem(itensKit[k]);
                if (itensKit[k].qt_item_kit != undefined) {
                    calcularItem(itensKit[k].qt_item_kit);

                } else if (itensKit[k].qt_item_movimento != undefined) {
                    calcularItem(itensKit[k].qt_item_movimento);

                }
                incluirItemGrade(true);
            }
        });
    }
}



function populaMovimentoItem(item) {
    dojo.byId("cdItem").value = item.cd_item;
    dojo.byId("cd_item").value = item.cd_item;
    dojo.byId("cd_item_kit").value = item.cd_item_kit > 0 ? item.cd_item_kit : null;
    dojo.byId("desc_item").value = item.dc_item_movimento;

    //dojo.byId("cd_CFOP_item").value = value.cd_cfop;
    if (hasValue(item.dc_cfop)) {
        dijit.byId("operacaoCFOPItem").value = item.dc_cfop;
    }
    if (hasValue(item.nm_cfop)) {
        dijit.byId("descCFOPItem").set("value", item.nm_cfop);
    }
    if (hasValue(item.cd_cfop)) {
        dojo.byId("cd_CFOP_item").value = item.cd_cfop;
    }

    dijit.byId("qtd_item").set("value", item.qt_item_movimento);
    dijit.byId("vlUnitario").set("value", item.vl_unitario_item);
    dijit.byId("perDescontoItem").set("value", item.pc_desconto_item);
    dijit.byId("valDescontoItem").set("value", item.vl_desconto_item);
    dijit.byId("vlTotalMovimento").set("value", item.vl_liquido_item);
    dijit.byId("vlLiquido").set("value", item.vl_liquido_item);

    dijit.byId("descPlanoConta").set("value", item.dc_plano_conta);
    dojo.byId("cd_plano_contas").value = item.cd_plano_conta;
}

function deletarKit(Memory, ObjectStore, gridKit) {
    var listaKit = gridKit.store.objectStore.data;
    apresentaMensagem("apresentadorMensagemMovto", null);

    if (listaKit.length > 0 && hasValue(gridKit.itensSelecionados) && gridKit.itensSelecionados.length > 0) {
        for (var i = listaKit.length - 1; i >= 0; i--) {
            if (binaryObjSearch(gridKit.itensSelecionados, 'cd_item_kit', eval('listaKit[i].cd_item_kit')) != null) {

                excluirKitDoGrid(listaKit[i].cd_item_kit, listaKit[i].qt_item_kit);
                listaKit.splice(i, 1); // Remove o item do array
            }
        }

        gridKit.itensSelecionados = new Array();
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: listaKit }) });
        gridKit.setStore(dataStore);
        gridKit.update();
    } else {
        caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
}

function getLocalMovtoGeralOuCartaoMovimento(cd_tipo_financeiro) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getAllLocalMovtoGeralOuCartao?cd_tipo_financeiro=" + cd_tipo_financeiro,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            response = jQuery.parseJSON(data);
            loadSelectLocalMovimento(response.retorno, "edBanco", 'cd_local_movto', 'no_local_movto', 'nm_tipo_local');
        } catch (e) {
            postGerarLog(e);
        }

    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function loadSelectLocalMovimento(items, link, idName, valueName, valueTipoLocalName, id) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            var itemsCb = [];
            Array.forEach(items,
                function (value, i) {
                    itemsCb.push({
                        id: eval("value." + idName),
                        name: eval("value." + valueName),
                        nm_tipo_local: eval("value." + valueTipoLocalName)
                    });
                });
            var stateStore = new Memory({
                data: itemsCb
            });
            var componente = dijit.byId(link);

            componente._onChangeActive = false;
            componente.store = stateStore;
            if (hasValue(id))
                componente.set("value", id);
            componente._onChangeActive = true;
        });
}

function hasCartao() {
    return (dijit.byId("tpFinanceiro").value == CARTAO);
}

function alterarLocalMovtoAplicarTaxaBancaria(titulosAlterarLocalMovto, titulosRollback) {
    require([
        "dojo/_base/xhr",
        "dojox/json/ref"],
        function (xhr, ref) {
            showCarregando();
            var politica_comercial = dojo.byId("cdPoliticaComercial").value;
            apresentaMensagem("apresentadorMensagemMatAdt", null);

            xhr.post({
                url: Endereco() + "/api/escola/alterarLocalMovtoTitulos",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    titulos: titulosAlterarLocalMovto,
                    cd_politica_comercial: politica_comercial
                })
            }).then(function (data) {
                hideCarregando();

                //titulos = $.parseJSON(data).retorno;
                titulosData = jQuery.parseJSON(data).retorno;
                var titulosGrid = dijit.byId('gridTitulo').store.objectStore.data;
                for (var i = 0; i < titulosData.length; i++) {
                    jQuery.grep(titulosGrid, function (titulo) {
                        if (((titulo.cd_titulo > 0 && titulo.cd_titulo == titulosData[i].cd_titulo) ||
                            (titulo.cd_titulo === 0 && titulo.nm_parcela_titulo === titulosData[i].nm_parcela_titulo)) &&
                            titulo.vl_saldo_titulo > 0 &&
                            titulo.vl_titulo == titulo.vl_saldo_titulo &&
                            titulo.vl_liquidacao_titulo === 0 &&
                            (titulo.possuiBaixa != undefined &&
                                titulo.possuiBaixa != null &&
                                titulo.possuiBaixa === false)) {
                                for (var z = 0; z < titulosAlterarLocalMovto.length; z++) {
                                    if ((titulosData[i].cd_titulo > 0 &&
                                        titulosAlterarLocalMovto[z].cd_titulo == titulosData[i].cd_titulo) ||
                                        (titulosData[i].cd_titulo === 0 &&
                                            titulosAlterarLocalMovto[z].nm_parcela_titulo ===
                                            titulosData[i].nm_parcela_titulo)) {
                                        titulo.cd_local_movto = titulosData[i].cd_local_movto;
                                        titulo.pc_taxa_cartao = titulosData[i].pc_taxa_cartao;
                                        titulo.vl_taxa_cartao = titulosData[i].vl_taxa_cartao;
                                        titulo.nm_dias_cartao = titulosData[i].nm_dias_cartao;
                                        titulo.dt_vcto_titulo = titulosData[i].dt_vcto_titulo;
                                        titulo.dt_vcto = titulosData[i].dt_vcto;
                                        //titulo.tituloEdit = true //LBM 290623: Analisar se para movimento tem sentido
                                    }

                                }

                            }
                        
                    });
                }
                dijit.byId("gridTitulo").update();

                //var dataStore = new ObjectStore({ objectStore: new Memory({ data: titulos }) });
                //gridTitulo.setStore(dataStore);
                postAlterarLocalMovtoTitulosNFFechada(titulosData, titulosRollback);
            }, function (error) {
                hideCarregando();
                rollbackLocalMovto(titulosRollback);

                if (error) {
                    apresentaMensagem("apresentadorMensagemMovto", error);
                }


            });
        });
}

function postAlterarLocalMovtoTitulosNFFechada(titulosAlterarLocalMovto, titulosRollback) {
    require([
            "dojo/_base/xhr",
            "dojox/json/ref"],
        function (xhr, ref) {
            showCarregando();
            apresentaMensagem("apresentadorMensagemMatAdt", null);

            xhr.post({
                url: Endereco() + "/api/escola/postAlterarLocalMovtoTitulosNFFechada",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson( titulosAlterarLocalMovto)
            }).then(function (data) {
                hideCarregando();


            }, function (error) {
                hideCarregando();
                rollbackLocalMovto(titulosRollback);

                if (error) {
                    apresentaMensagem("apresentadorMensagemMovto", error);
                }


            });
        });
}

function rollbackLocalMovto(titulosRollback) {
    var titulos = dijit.byId('gridTitulo').store.objectStore.data;
    for (var i = 0; i < titulosRollback.length; i++) {

        jQuery.grep(titulos, function (titulo) {
            if (titulo.cd_titulo == titulosRollback[i].cd_titulo) {
                titulo.cd_local_movto = titulosRollback[i].cd_local_movto;
                titulo.descLocalMovto = titulosRollback[i].descLocalMovto;
            }
        });

    }

    dijit.byId("gridTitulo").update();
}
function aplicarTaxaBancaria(cd_titulo, cd_local_movto) {
    try {
        dojo.xhr.get({
            url: Endereco() +
                "/api/financeiro/getTituloAplicadoTaxaCartao?cd_titulo=" + cd_titulo + "&cd_local_movto=" + cd_local_movto,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemMovto", null);


                dijit.byId("pc_taxa_cartao")._onChangeActive = false;
                dijit.byId("pc_taxa_cartao").set("value", data.pc_taxa_cartao);
                dijit.byId("pc_taxa_cartao")._onChangeActive = true;

                dijit.byId("nm_dias_cartao")._onChangeActive = false;
                dijit.byId("nm_dias_cartao").set("value", data.nm_dias_cartao);
                dijit.byId("nm_dias_cartao")._onChangeActive = true;


                dojo.byId('vl_taxa_cartao').value = data.vlTaxaCartao;
                dojo.byId("dtaVencTit").value = data.dt_vcto;
            } catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem("apresentadorMensagemMovto", error);
            });

    } catch (e) {
        postGerarLog(e);
    }
}
