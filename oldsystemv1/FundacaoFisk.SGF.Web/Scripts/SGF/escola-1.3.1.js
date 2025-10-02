//#region - declaração de constantes/variáveis - setDefaultHorarios - getNameFoto
//mensagens
msgCodigoInvalido = 'Código inválido';
// Constantes
var AUTOCOMPLET_FECHADO = 0, AUTOCOMPLETE_ABERTO = 1;
var JURIDICA = 2;
var TIPO_PAPEL_JURIDICA = 1;
var PRINCIPAL_MATRICULA = 1;
var SERVICO = 5, SAIDA_TIPO = 2;
var TIPOMOVIMENTO = null;
var itemFk;
var ITEMTXMAT = 1, ITEMMENSALIDADE = 2, ITEMBIBLIOTECA = 3, ITEMSERVICO = 4;
var tipoNFFk;
var TPNFTXMENS = 1, TPNFBIBLIO = 2, TPNFMAT = 3, TPNFSERV = 4;
var ENTRADA = 1, SAIDA = 2;
var REGIME_NORMAL = 3;
var CADASTRO = 1, EDICAO = 2;

var isTipoSaida = false;

var numeroMatricula = new Array(
     { name: "Igual ao Contrato", id: "1" },
     { name: "Automático", id: "2" },
     { name: "Manual", id: 3 }
    );

function mascarar() {
    require([
           "dojo/ready",
           "dojo/store/Memory",
           "dijit/form/FilteringSelect",
           "dojo/on"
    ], function (ready, Memory, FilteringSelect, on) {
        ready(function () {
            try{
                $("#timeIni").mask("99:99");
                $("#timeFim").mask("99:99");
                $("#cgc").mask("99.999.999/9999-99");
                montarStatus("statusEscola");
                criarOuCarregarCompFiltering('numeroMatricula', numeroMatricula, '', 1, ready, Memory, FilteringSelect, 'id', 'name', null);
                $('#panelPessoaFisica').css("display", "none");

                var statusStore = new Memory({
                    data: [
                      { name: "Simples Nacional", id: "1" },
                      { name: "Simples Nacional - excesso de sublimite da receita bruta", id: "2" },
                      { name: "Regime Normal", id: "3" }
                    ]
                });

                new FilteringSelect({
                    id: "regimeTrib",
                    name: "regimeTrib",
                    store: statusStore,
                    value: "1",
                    required: false,
                    searchAttr: "name",
                    style: "width:100%;"
                }, "regimeTrib");
                dijit.byId("regimeTrib").oldValue = 1;
                dijit.byId("regimeTrib").on("change", function (e) {
                    if (hasValue(e)){
                        if(parseInt(e) != dijit.byId("regimeTrib").oldValue) {
                            limparTipoNFEscola();
                            habilitarDesabilitarCamposTipoNFFK(false);
                        }
                    } else {
                        limparTipoNFEscola();
                        habilitarDesabilitarCamposTipoNFFK(true);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadGrupoEstoqueMaterial(items, linkGrupo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var itemsCb = [];
                var cbGrupo = dijit.byId(linkGrupo);
                Array.forEach(items, function (value, i) {
                    itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
                });
                var stateStore = new Memory({
                    data: itemsCb
                });
                cbGrupo.store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function limparTipoNFEscola() {
    dijit.byId("tpNFTxMens").reset();
    dijit.byId("tpNFBiblio").reset();
    dijit.byId("tpNFMaterial").reset();
    dijit.byId("tpNFMaterialS").reset();
    dojo.byId("cd_tpnf_txmens").value = 0;
    dojo.byId("cd_tpnf_biblio").value = 0;
    dojo.byId("cd_tpnf_material").value = 0;
    dojo.byId("cd_tpnf_materialS").value = 0;
}

function habilitarDesabilitarCamposTipoNFFK(bool) {
    dijit.byId("btTpNFTxMens").set("disabled", bool);
    dijit.byId("btTpNFBiblio").set("disabled", bool);
    dijit.byId("btTpNFMaterial").set("disabled", bool);
    dijit.byId("btTpNFMaterialS").set("disabled", bool);
}

function setDefaultHorarios() {
    try{
        dojo.byId("timeIni").value = "08:00";
        dojo.byId("timeFim").value = "18:00";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparParametrosEscola() {
    try{
        dojo.attr('itemTxMat', "title", "");
        dojo.attr('itemMensalidade', "title", "");
        dojo.attr('itemBiblioteca', "title", "");
        dojo.attr('tpNFBiblio', "title", "");
        dojo.attr('tpNFTxMens', "title", "");
        dojo.attr('tpNFMaterial', "title", "");
        dojo.attr('tpNFMaterialS', "title", "");
        clearForm('formParametros');
        dijit.byId("qtdDiasTitulosAberto").reset();
        dijit.byId("qtdFaltasAluno").reset();
        dijit.byId("aulaMaterial").reset();
        dijit.byId("recibo").reset();
        dijit.byId("bloquearLiquidacao").reset();
        dijit.byId("somarDesconto").reset();
        dijit.byId("permitirPermanencia").reset();
        dijit.byId("calcularJuros").reset();
        dijit.byId("alterarVencimento").reset();
        dijit.byId("bloquearVenda").reset();
        dijit.byId("bloquerCompras").reset();
        dijit.byId("bloquerImpressao").reset();
        dijit.byId("liberarInsercao").reset();
        dijit.byId("bloquerMatricula").reset();
        dijit.byId("numeracaoContratoAutomatico").reset();
        dijit.byId("ckImprimir3Boletos").reset();
        dijit.byId("matricula").reset();
        dijit.byId("contrato").reset();
        dijit.byId("recibo").reset();
        dijit.byId("diaVencimento").reset();
        dijit.byId("diaUtil").reset();
        dijit.byId("numeroDias").reset();
        dijit.byId("nm_media_certificado").reset();
        dijit.byId("pc_falta_certificado").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparDatas() {
    try {
        dijit.byId("dtaInicio").reset()
        dijit.byId("dtaAbertura").reset();
    }
    catch (e) {
        postGerarLog(e);
    }

}

function getNameFoto(files) {
    try{
        if (hasValue(files.name))
            return files.name;
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarHorario() {
    try{
        if ((dojo.byId('timeFim').value > '23:59')
              || (dojo.byId('timeIni').value > '23:59')) {
            caixaDialogo(DIALOGO_AVISO, "Horário(s) fora do intervalo.", 0, 0, 0);
            setarTabCad();
            dijit.byId('panelHorario').set('open', true);
            $('#timeIni').focus();
            return false;
        } else return true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region - formatCheckBoxEscola
// inicio da formatação da Escola
function formatCheckBoxEscola(value, rowIndex, obj) {
    try{
        var gridName = 'gridEscola';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscola');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEscola', -1, 'selecionaTodosEscola', 'selecionaTodosEscola', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEscola', " + rowIndex + ", '" + id + "', 'selecionaTodosEscola', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region keepValues
// Cancela a alteração do registro voltando o original
function keepValues(value, grid, ehLink, xhr) {
    try{
        var valorCacelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true)) 
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        //Quando for cancelamento
        if (!hasValue(value) && hasValue(valorCacelamento) && !ehLink) 
            value = valorCacelamento[0];

        var papelRelac = new Array();
        papelRelac[0] = TIPO_PAPEL_JURIDICA;
        showPessoaFK(JURIDICA, value.cd_pessoa, papelRelac);
        showFoto(value);

        clearForm('formParametros');
        clearForm('formEscola');
        clearForm('formFiscal');
        clearForm('formPlanoContas');
        clearForm('formDadosNotaServico');
        dijit.byId("timeIni").reset();
        dijit.byId("timeFim").reset();

        var cdEscola = Escola();

        if (hasValue(value)) {
            habilitarPlanosContas(cdEscola, value.cd_pessoa);            
            require(["dojo/_base/xhr"], function (xhr) {
                xhr.get({
                    url: Endereco() + "/api/escola/getEscolaForEdit?cd_pessoa_empresa=" + value.cd_pessoa,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataEscola) {
                    try{
                        var dados = dataEscola;
                        if (dados != null) {
                            apresentaMensagem("apresentadorMensagem", null);

                            if (dados.nm_cliente_integracao == 21 && dados.id_empresa_propria == true) {
                                dojo.byId('panelDadosNotaServico').style.display = 'block';
                            }

                            dados.hr_cadastro != null ? dojo.byId("horaCadastro").value = dados.hr_cadastro : "";
                            dojo.byId("timeIni").value = dados.hr_inicial_escola;
                            dojo.byId("timeFim").value = dados.hr_final_escola;

                            dojo.byId("dtaInicio").value = dados.dtaInicio;
                            dojo.byId("dtaAbertura").value = dados.dtaAbertura;
                            dojo.byId("idPessoaAtiva").value = dados.id_pessoa_ativa == true ? dijit.byId("idPessoaAtiva").set("value", true) : dijit.byId("idPessoaAtiva").set("value", false);

                            var cdEscolaAnterior = dijit.byId("escolaAnterior");
                            cdEscolaAnterior._onChangeActive = false;
                            cdEscolaAnterior.set("value", dados.cd_empresa_coligada);
                            cdEscolaAnterior._onChangeActive = true;

                            //keep parametro
                            dijit.byId('bloquearMovimento').set('value', dados.parametro.id_retroativo_caixa);
                            dijit.byId('bloquearLiquidacao').set('value', dados.parametro.id_liquidacao_tit_ant_aberto);
                            dijit.byId('somarDesconto').set('value', dados.parametro.id_somar_descontos_financeiros);
                            dijit.byId('permitirPermanencia').set('value', dados.parametro.id_permitir_desc_apos_politica);
                            dijit.byId('calcularJuros').set('value', dados.parametro.id_juros_final_semana);
                            dijit.byId('alterarVencimento').set('value', dados.parametro.id_alterar_venc_final_semana);
                            dijit.byId('bloquearVenda').set('value', dados.parametro.id_bloquear_venda_sem_estoque);
                            dijit.byId('bloquerCompras').set('value', dados.parametro.id_bloquear_mov_retr_estoque);
                            dijit.byId('obrigarPreenchimento').set('value', dados.parametro.id_requer_plano_contas_mov);
                            dijit.byId('bloquerImpressao').set('value', dados.parametro.id_bloquear_imprimir_nf_data_atual);
                            dijit.byId('liberarInsercao').set('value', dados.parametro.id_liberar_habilitacao_professor);
                            dijit.byId('bloquerMatricula').set('value', dados.parametro.id_bloquear_matricula_vaga);
                            dijit.byId('ckImprimir3Boletos').set('value', dados.parametro.id_mostrar_3_boletos_pagina);
                            dijit.byId('numeracaoContratoAutomatico').set('value', dados.parametro.id_nro_contrato_automatico);
                            dijit.byId('idBaixaCartaoCk').set('value', dados.parametro.id_baixa_automatica_cartao);
                            dijit.byId('idBaixaChequeCk').set('value', dados.parametro.id_baixa_automatica_cheque);

                            if (hasValue(dijit.byId('numeroMatricula'), true))
                                hasValue(dados.parametro.id_tipo_numero_contrato) ? dijit.byId('numeroMatricula').set("value", dados.parametro.id_tipo_numero_contrato) : dijit.byId('numeroMatricula').set("value", 1);
                            if (hasValue(dijit.byId('diaVencimento'), true))
                                hasValue(dados.parametro.nm_dia_vencimento) ? dijit.byId('diaVencimento').set('value', dados.parametro.nm_dia_vencimento) : 0;
                            dijit.byId('diaUtil').set('value', dados.parametro.id_dia_util_vencimento);
                            if (hasValue(dojo.byId('contrato').value, true))
                                hasValue(dados.parametro.nm_ultimo_contrato) ? $("#contrato").val(dados.parametro.nm_ultimo_contrato) : 0;
                            if (hasValue(dojo.byId('matricula').value, true)) 
                                hasValue(dados.parametro.nm_ultimo_matricula) ? $("#matricula").val(dados.parametro.nm_ultimo_matricula) : 0;
                            if (hasValue(dojo.byId('turma').value, true)) 
                                hasValue(dados.parametro.nm_ultima_turma) ? $("#turma").val(dados.parametro.nm_ultima_turma) : 0;
                            if (hasValue(dojo.byId('recibo').value, true)) 
                                hasValue(dados.parametro.nm_ultimo_recibo) ? $("#recibo").val(dados.parametro.nm_ultimo_recibo) : 0;
                            dijit.byId('jurosMulta').set('value', dados.parametro.id_cobrar_juros_multa);

                            //Tag Financeiro - titulos atrasados - Pescentuais
                            if (hasValue(dojo.byId('jurosDia').value), true) {
                                if (hasValue(dados.parametro.pc_juros_dia))
                                    dijit.byId('jurosDia').set("value", parseFloat(dados.parametro.pc_juros_dia));
                                else {
                                    //dojo.byId('jurosDia').value = 0;
                                    dijit.byId('jurosDia').set("value", 0);
                                }
                            }
                            if (hasValue(dojo.byId('multa').value), true) {
                                if (hasValue(dados.parametro.pc_multa))
                                    dojo.byId('multa').value = parseFloat(dados.parametro.pc_multa).toFixed(2).toString().replace(".", ",");
                                else dojo.byId('multa').value = '0,00';
                            }
                            //Fim
                            if (hasValue(dojo.byId('taxaMulta').value), true) {
                                if (hasValue(dados.parametro.pc_taxa_dia_biblioteca))
                                    dojo.byId('taxaMulta').value = parseFloat(dados.parametro.pc_taxa_dia_biblioteca).toFixed(2).toString().replace(".", ",");
                                else dojo.byId('taxaMulta').value = '0,00';
                            }

                            if (hasValue(dados.parametro.per_desconto_maximo, true))
                                dijit.byId('limitadorPercentual').set("value", dados.parametro.per_desconto_maximo);
                            else dijit.byId('limitadorPercentual').set('value', '100,00');

                            if (hasValue(dados.parametro.nm_niveis_plano_contas))
                                dijit.byId('niveisPlanoContas').set('value', dados.parametro.nm_niveis_plano_contas);
                            else
                                dijit.byId('niveisPlanoContas').set('value', '');

                            if (hasValue(dojo.byId('carencia').value, true)) 
                                hasValue(dados.parametro.nm_dias_carencia) ? $("#carencia").val(dados.parametro.nm_dias_carencia) : 0;
                            if (hasValue(dojo.byId('aulaMaterial').value, true)) 
                                hasValue(dados.parametro.nm_aulas_sem_material) ? $("#aulaMaterial").val(dados.parametro.nm_aulas_sem_material) : 0;
                            if (hasValue(dojo.byId('qtdFaltasAluno').value, true))
                                hasValue(dados.parametro.nm_faltas_aluno) ? $("#qtdFaltasAluno").val(dados.parametro.nm_faltas_aluno) : 0;
                            if (hasValue(dojo.byId('qtdDiasTitulosAberto').value, true))
                                hasValue(dados.parametro.nm_dias_titulos_abertos) ? $("#qtdDiasTitulosAberto").val(dados.parametro.nm_dias_titulos_abertos) : 0;

                            if (hasValue(dojo.byId('nm_media_certificado').value, true))
                                hasValue(dados.parametro.nm_media_minima) ? $("#nm_media_certificado").val(dados.parametro.nm_media_minima) : 0;
                            if (hasValue(dojo.byId('pc_falta_certificado').value, true))
                                hasValue(dados.parametro.pc_falta_permitida) ? $("#pc_falta_certificado").val(dados.parametro.pc_falta_permitida) : 0;

                            if (hasValue(dojo.byId('minutosConcedidos').value, true))
                                hasValue(dados.parametro.nm_minutos_concedidos) ? $("#minutosConcedidos").val(dados.parametro.nm_minutos_concedidos) : 0;
                            dijit.byId('alteracaoData').set('value', dados.parametro.id_bloquear_alt_dta_biblio);

                            if (hasValue(dojo.byId('numeroDias'), true)) 
                                hasValue(dados.parametro.nm_dias_biblioteca) ? $("#numeroDias").val(dados.parametro.nm_dias_biblioteca) : 0;
                            if (hasValue(dados.nm_escola_integracao))
                                dijit.byId('nmEmpresaIntegracao').set('value', dados.nm_escola_integracao);

                            if (hasValue(dados.nm_cliente_integracao))
                                dijit.byId('nmClienteIntegracao').set('value', dados.nm_cliente_integracao);

                            dojo.byId('cd_plano_matricula').value = dados.parametro.cd_plano_conta_mat;
                            dojo.byId('cd_plano_taxa').value = dados.parametro.cd_plano_conta_tax;
                            dojo.byId('cd_plano_conta_juros').value = dados.parametro.cd_plano_conta_trf;
                            dojo.byId('cd_plano_conta_multa').value = dados.parametro.cd_plano_conta_trf;
                            dojo.byId('cd_plano_conta_desc').value = dados.parametro.cd_plano_conta_trf;
                            dojo.byId('cd_plano_taxa_bco').value = dados.parametro.cd_plano_conta_taxbco;
                            dojo.byId('cd_plano_conta_material').value = dados.parametro.cd_plano_conta_material;

                            dijit.byId('id_gerar_financeiro_contrato').set("checked", dados.parametro.id_gerar_financeiro_contrato);
                            dijit.byId('id_empresa_propria').set("checked", dados.id_empresa_propria);
                            dijit.byId('id_empresa_internacional').set("checked", dados.id_empresa_internacional);
                            dijit.byId('nm_dia_gerar_nfs').set("value", dados.nm_dia_gerar_nfs);
                            dijit.byId('nm_dia_gerar_nf_servico').set("value", dados.nm_dia_gerar_nf_servico);

                            //Fiscal
                            dijit.byId("emiteNFS").set("checked", dados.parametro.id_emitir_nf_servico);
                            
                            dijit.byId("emiteNFM").set("checked", dados.parametro.id_emitir_nf_mercantil);
                            dijit.byId("nrNFAuto").set("checked", dados.parametro.id_numero_nf_automatico);
                            dijit.byId("permiteSaldoMovFinNegativa").set("checked", dados.parametro.id_financeiro_negativo);
                            dijit.byId("regimeTrib").set("value", dados.parametro.id_regime_tributario);
                            dijit.byId("regimeTrib").oldValue = dados.parametro.id_regime_tributario;
                            dojo.byId('nmNFS').value = dados.parametro.nm_nf_servico;
                            dojo.byId('serieNFS').value = dados.parametro.dc_serie_nf_servico;
                            dojo.byId('nrNFM').value = dados.parametro.nm_nf_mercantil;
                            dojo.byId('serieNFM').value = dados.parametro.dc_serie_nf_mercantil;
                            dojo.byId('pc_aliquota_ap_saida').value = parseFloat(dados.parametro.pc_aliquota_ap_saida).toFixed(2).toString().replace(".", ",");
                            dojo.byId('pc_aliquota_ap_servico').value = parseFloat(dados.parametro.pc_aliquota_ap_servico).toFixed(2).toString().replace(".", ",");
                            dojo.byId('nrNMM').value = dados.parametro.nm_nf_material;
                            dojo.byId("cd_item").value = dados.parametro.cd_item_taxa_matricula;
                            dojo.byId("cd_item_mensalidade").value =  dados.parametro.cd_item_mensalidade;
                            dojo.byId("cd_item_biblioteca").value =  dados.parametro.cd_item_biblioteca;
                            dojo.byId("cd_tpnf_txmens").value =  dados.parametro.cd_tipo_nf_matricula;
                            dojo.byId("cd_tpnf_material").value =  dados.parametro.cd_tipo_nf_material;
                            dojo.byId("cd_tpnf_materialS").value = dados.parametro.cd_tipo_nf_material_saida;
                            dojo.byId("cd_tpnf_biblio").value =  dados.parametro.cd_tipo_nf_biblioteca;
                            dojo.byId("cd_polComercialNF").value = dados.parametro.cd_politica_comercial_nf;

                            if (dojo.byId('panelDadosNotaServico').style.display == 'block') {
                                dojo.byId('cd_item_servico').value = dados.parametro.cd_item_servico;
                                dojo.byId('cd_tpnf_servico').value = dados.parametro.cd_tipo_nf_servico;
                                dojo.byId('cd_plano_servico').value = dados.parametro.cd_plano_conta_servico;
                                dojo.byId('cd_polComercialServico').value = dados.parametro.cd_politica_servico;


                                dojo.byId('itemServico').value = dados.dc_item_servico;
                                if (dados.dc_item_servico != null &&
                                    dados['dc_item_servico'] != undefined &&
                                    dados.dc_item_servico != "") {
                                    dijit.byId("limparItemServico").set('disabled', false);
                                }
                                dojo.byId('tpNFServico').value = dados.dc_tp_nf_servico;
                                if (dados.dc_tp_nf_servico != null &&
                                    dados['dc_tp_nf_servico'] != undefined &&
                                    dados.dc_tp_nf_servico != "") {
                                    dijit.byId("limparTpNFServico").set('disabled', false);
                                }
                                dojo.byId('planoContasServico').value = dados.dc_plano_contas_servico;
                                if (dados.dc_plano_contas_servico != null &&
                                    dados['dc_plano_contas_servico'] != undefined &&
                                    dados.dc_plano_contas_servico != "") {
                                    dijit.byId("limparPlanoServico").set('disabled', false);
                                }
                                
                                dojo.byId('dc_pol_comercial_servico').value = dados.dc_pol_comercial_servico;
                                if (dados.dc_pol_comercial_servico != null &&
                                    dados['dc_pol_comercial_servico'] != undefined &&
                                    dados.dc_pol_comercial_servico != "") {
                                    dijit.byId("limparPolServico").set('disabled', false);
                                }
                            }

                            if (hasValue(dados.empresaValorServico)) {

                                setValueValorServico(dados.empresaValorServico);
                            }

                            dojo.byId('itemTxMat').value = dados.dc_item_taxa_mat,
                            dojo.byId('itemMensalidade').value = dados.dc_item_mensalidade,
                            dojo.byId('itemBiblioteca').value = dados.dc_item_biblioteca,
                            dojo.byId('tpNFBiblio').value = dados.dc_tpnf_biblioteca,
                            dojo.byId('tpNFTxMens').value = dados.dc_tpnf_taxa_mensalidade,
                            dojo.byId('tpNFMaterial').value = dados.dc_tpnf_material,
                            dojo.byId('tpNFMaterialS').value = dados.dc_tpnf_material_saida,
                            dojo.byId('dc_pol_comercial').value = dados.dc_pol_comercial,
                            //

                            dojo.byId('planoContasMat').value = dados.dc_plano_mat;
                            dojo.byId('planoContasTaxa').value = dados.dc_plano_taxa;
                            dojo.byId('planoContasJuros').value = dados.dc_plano_juros;
                            dojo.byId('planoContasMulta').value = dados.dc_plano_multa;
                            dojo.byId('planoContasDesconto').value = dados.dc_plano_desconto;
                            dojo.byId('planoContasTaxasBancarias').value = dados.dc_plano_taxa_bco;
                            dojo.byId('planoContasMaterial').value = dados.dc_plano_material;

                            dojo.attr('itemTxMat', "title", dados.dc_item_taxa_mat);
                            dojo.attr('itemMensalidade', "title", dados.dc_item_mensalidade);
                            dojo.attr('itemBiblioteca', "title", dados.dc_item_biblioteca);
                            dojo.attr('tpNFBiblio', "title", dados.dc_tpnf_biblioteca);
                            dojo.attr('tpNFTxMens', "title", dados.dc_tpnf_taxa_mensalidade);
                            dojo.attr('tpNFMaterial', "title", dados.dc_tpnf_material);
                            dojo.attr('tpNFMaterialS', "title", dados.dc_tpnf_material_saida);

                            if (Escola() == dados.cd_pessoa) {
                                if ((dados.parametro.cd_plano_conta_tax > 0))
                                    dijit.byId("limparPlanoTaxa").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoTaxa").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_mat > 0))
                                    dijit.byId("limparPlanoMat").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoMat").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_mat > 0))
                                    dijit.byId("limparPlanoContaJuros").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoContaJuros").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_mat > 0))
                                    dijit.byId("limparPlanoContasMulta").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoContasMulta").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_mat > 0))
                                    dijit.byId("limparPlanoContasDesconto").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoContasDesconto").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_taxbco > 0))
                                    dijit.byId("limparPlanoContasTaxasBancarias").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoContasTaxasBancarias").set('disabled', true);

                                if ((dados.parametro.cd_plano_conta_material > 0))
                                    dijit.byId("limparPlanoContasMaterial").set('disabled', false);
                                else
                                    dijit.byId("limparPlanoContasMaterial").set('disabled', true);
                            } else {
                                dijit.byId("limparPlanoTaxa").set('disabled', true);
                                dijit.byId("limparPlanoContasDesconto").set('disabled', true);
                                dijit.byId("limparPlanoContasTaxasBancarias").set('disabled', true);
                                dijit.byId("limparPlanoContasMulta").set('disabled', true);
                                dijit.byId("limparPlanoContaJuros").set('disabled', true);
                                dijit.byId("limparPlanoMat").set('disabled', true);
                                dijit.byId("limparPlanoContasMaterial").set('disabled', true);
                                
                            }

                            if ((dados.parametro.cd_item_taxa_matricula > 0))
                                dijit.byId("limparItemTxMat").set('disabled', false);
                            else
                                dijit.byId("limparItemTxMat").set('disabled', true);

                            if ((dados.parametro.cd_item_mensalidade > 0))
                                dijit.byId("limparItemMens").set('disabled', false);
                            else
                                dijit.byId("limparItemMens").set('disabled', true);

                            if ((dados.parametro.cd_item_biblioteca > 0))
                                dijit.byId("limparItemBiblio").set('disabled', false);
                            else
                                dijit.byId("limparItemBiblio").set('disabled', true);

                            if ((dados.parametro.cd_tipo_nf_biblioteca > 0))
                                dijit.byId("limparTpNFBiblio").set('disabled', false);
                            else
                                dijit.byId("limparTpNFBiblio").set('disabled', true);

                            if ((dados.parametro.cd_tipo_nf_matricula > 0))
                                dijit.byId("limparTpNFTxMens").set('disabled', false);
                            else
                                dijit.byId("limparTpNFTxMens").set('disabled', true);

                            if ((dados.parametro.cd_tipo_nf_material > 0))
                                dijit.byId("limparTpNFMaterial").set('disabled', false);
                            else
                                dijit.byId("limparTpNFMaterial").set('disabled', true);

                            if ((dados.parametro.cd_politica_comercial_nf > 0))
                                dijit.byId("limparPolCom").set('disabled', false);
                            else
                                dijit.byId("limparPolCom").set('disabled', true);

                            if ((dados.parametro.cd_tipo_nf_material_saida > 0))
                                dijit.byId("limparTpNFMaterialS").set('disabled', false);
                            else
                                dijit.byId("limparTpNFMaterialS").set('disabled', true);

                            if (hasValue(dojo.byId('nm_investimento').value), true) {
                                if (hasValue(dados.nm_investimento))
                                    dojo.byId('nm_investimento').value = parseFloat(dados.nm_investimento).toFixed(2).toString().replace(".", ",");
                                else dojo.byId('nm_investimento').value = '0,00';
                            }

                            if (hasValue(dojo.byId('nm_patrimonio').value), true) {
                                if (hasValue(dados.nm_patrimonio))
                                    dojo.byId('nm_patrimonio').value = parseFloat(dados.nm_patrimonio).toFixed(2).toString().replace(".", ",");
                                else dojo.byId('nm_patrimonio').value = '0,00';
                            }

                            loadSelect(dados.localMovto, "edLocalMovto", 'cd_local_movto', 'nomeLocal', dados.parametro.cd_local_movto);
                            dijit.byId("nomAssinaturaCertificado").set("value", dados.nome_assinatura_certificado);
                            desabilitarCampos(eval(MasterGeral()));
                        }
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                        apresentaMensagem('apresentadorMensagemEscola', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
};
//#endregion

//#region montarCadastroEscola
function montarCadastroEscola() {
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
           "dijit/form/Button",
           "dojo/ready",
           "dijit/form/DropDownButton",
           "dijit/DropDownMenu",
           "dijit/MenuItem",
           "dojo/_base/array",
           "dijit/Dialog",
           "dijit/Tooltip",
           "dojo/window"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button,
                  ready, DropDownButton, DropDownMenu, MenuItem, array, Tooltip, windowUtils) {
        
        ready(function () {
            try {
                dojo.byId('panelDadosNotaServico').style.display = 'none';
                mascarar();
                loadEscolaAnteriorPesq(0, 0);
                dojo.byId("descApresMsg").value = 'apresentadorMensagemEscola';
                $('#labelTipoPessoa')[0].innerHTML = 'Fantasia:';
                $('#lbFoto')[0].innerHTML = 'Escola:';
                dijit.byId("nomFantasia").set("required", true);
                //Mostra as datas da escola:
                show('trDatasEscola');
                if (eval(MasterGeral()))
                    showP("divPlanoIntegracao", true);
                dojo.byId('widget_bloquearMovimento').style.width = "20%";

                var myStore = Cache(
                  JsonRest({
                      target: Endereco() + "/api/escola/getescolasearch?desc=&inicio=false&status=1&cnpj=&fantasia=",
                      handleAs: "json",
                      preventCache: true,
                      headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                  }), Memory({}));

                var gridEscola = new EnhancedGrid({
                    // store: ObjectStore({ objectStore: myStore }),
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosEscola' style='display:none'/>", field: "selecionadoEscola", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEscola },
                    //    { name: "Código", field: "cd_pessoa", width: "10%", minwidth: "10%" },
                        { name: "Nome", field: "no_pessoa", width: "40%", minwidth: "10%" },
                        { name: "Nome Fantasia", field: "dc_reduzido_pessoa", width: "20%", minwidth: "10%" },
                        { name: "CNPJ", field: "dc_num_cgc", width: "15%", minwidth: "10%" },
                        { name: "Data Cadastro", field: "dt_cadastro", width: "10%", minwidth: "10%" },
                        { name: "Telefone", field: "dc_fone_email", width: "10%", minwidth: "10%" },
                        { name: "Ativo", field: "escola_ativa", width: "5%", styles: "text-align: center;", minwidth: "12%" }
                      ],
                    noDataMessage: msgNotRegEncFiltro,
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
                }, "gridEscola");
                gridEscola.pagination.plugin._paginator.plugin.connect(gridEscola.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridEscola, 'cd_pessoa', 'selecionaTodosEscola');
                });
                gridEscola.canSort = function (col) { return Math.abs(col) != 1; };
                gridEscola.startup();
                gridEscola.on("RowDblClick", function (evt) {
                    try{
                        apresentaMensagem('apresentadorMensagem', '');

                        var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                        if (!hasValue(item.cd_empresa))
                            item.cd_empresa = 0;
                        loadEscolaAnteriorPesq(item.cd_pessoa, item.cd_empresa_coligada);
                        limparEnderecoPrincipal();
                        limparCadPessoaFK();
                        limparParametrosEscola();
                        dojo.byId('panelDadosNotaServico').style.display = 'none';
                        //setDefaultHorarios();
                        keepValues(item, gridEscola, false, xhr);
                        var tabContainer = dijit.byId("tabContainer");
                        tabContainer.selectChild(tabContainer.getChildren()[0]);
                        IncluirAlterar(0, 'divAlterarEscola', 'divIncluirEscola', 'divExcluirEscola', 'apresentadorMensagemEscola', 'divCancelarEscola', 'divLimparEscola');
                        dijit.byId('naturezaPessoa').set("disabled", true);
                        dijit.byId("cadEscola").show();
                        dijit.byId('tabContainer').resize();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);


                //Criação da grade ValorServico
                var gridValorServico = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: " ", field: "ehSelecionado", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxValorServico },
                        { name: "Data Inicial", field: "dta_inicio_valor_servico", width: "50%" },
                            { name: "Valor Unitário", field: "vl_unitario_servico", width: "24%", formatter: valorUnitarioServicoFormatter }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["5", "10", "20", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "5",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridValorServico");
                gridValorServico.startup();
                gridValorServico.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        apresentaMensagem(dojo.byId("descApresMsg").value, null);
                        IncluirAlterar(0, 'divAlterarValorServico', 'divIncluirValorServico', 'divExcluirValorServico', dojo.byId("descApresMsg").value, 'divCancelarValorServico', 'divClearValorServico');
                        limparValorServico();
                        keepValueValorServico();
                        dijit.byId("dialogValorServico").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarEscola(gridEscola.itensSelecionados, xhr); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    id: "excluirLink",
                    onClick: function () { eventoRemover(gridEscola.itensSelecionados, 'deletarEscola(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

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
                        buscarTodosItens(gridEscola, 'todosItens', ['pesquisarEscola', 'relatorioEscola']);
                        searchEscola(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridEscola', 'selecionadoEscola', 'cd_pessoa', 'selecionaTodosEscola', ['pesquisarEscola', 'relatorioEscola'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                var menuValorServico = new DropDownMenu({ style: "height: 25px" });

                var acaoEditarValorServico = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        var itensSelecionadosValorServico = dijit.byId("gridValorServico")._by_idx.filter(function(x) {
                            if (x != null && x != undefined && x.item != null && x.item != undefined &&
                                x.item.ehSelecionado == true) {
                                return x.item;
                            }

                        });
                        itensSelecionadosValorServico = itensSelecionadosValorServico.map(function (x) {
                            if (x != null && x != undefined) {
                                return x.item;
                            }
                            
                        })
                        eventoEditarValorServico(itensSelecionadosValorServico); }
                });
                menuValorServico.addChild(acaoEditarValorServico);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { excluirItemValorServico(); }
                });
                menuValorServico.addChild(acaoRemover);

                var buttonARL = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasValorServico",
                    dropDown: menuValorServico,
                    id: "acoesRelacionadasValorServico"
                });
                dojo.byId("linkAcoesValorServico").appendChild(buttonARL.domNode);

                new Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () { incluirValorServico(dom); } }, "incluirValorServico");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () { limparValorServico(); } }, "clearValorServico");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogValorServico").hide(); } }, "fecharValorServico");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                        try {
                            IncluirAlterar(1, 'divAlterarValorServico', 'divIncluirValorServico', 'divExcluirValorServico', dojo.byId("descApresMsg").value, 'divCancelarValorServico', 'divClearValorServico');
                            apresentaMensagem(dojo.byId("descApresMsg").value, null);
                            limparValorServico();
                            dijit.byId("dialogValorServico").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnNovoValorServico");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValueValorServico(); } }, "cancelarValorServico");
                new Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { alterarValorServico(dom); } }, "alterarValorServico");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { excluirValorServico(); } }, "deleteValorServico");
                

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        cadastrarOuAlterarEscola(windowUtils, CADASTRO);
                    }
                }, "incluirEscola");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        cadastrarOuAlterarEscola(windowUtils, EDICAO);
                    }
                }, "alterarEscola");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            deletarEscola();
                        });
                    }
                }, "deleteEscola");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try {
                            dijit.byId("regimeTrib")._onChangeActive = false;
                            dijit.byId("regimeTrib").reset();
                            dijit.byId("id_empresa_propria").set("checked", false);
                            dijit.byId("id_empresa_internacional").set("checked", false);
                            clearForm('formParametros');
                            clearForm('formPlanoContas');
                            clearForm('formFiscal');
                            dojo.byId("escolaAnterior").value = "";
                            dijit.byId("escolaAnterior").set("value", 0);
                            dijit.byId("timeIni").reset();
                            dijit.byId("timeFim").reset();
                            dijit.byId("regimeTrib")._onChangeActive = true;
                            dijit.byId("regimeTrib").set("value", REGIME_NORMAL);
                            dijit.byId("regimeTrib").oldValue = REGIME_NORMAL;
                            limparCadPessoaFK();
                            limparParametrosEscola();
                           // setDefaultHorarios();
                            limparOutrosContatos();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparEscola");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        try{
                            keepValues(null, gridEscola, null, xhr);
                            var codPessoa = hasValue(dojo.byId('cd_pessoa').value) ? dojo.byId('cd_pessoa').value : 0;
                            var papelRelac = new Array();
                            papelRelac[0] = TIPO_PAPEL_JURIDICA;
                            //showPessoaFK(JURIDICA, codPessoa, papelRelac);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarEscola");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadEscola").hide();
                    }
                }, "fecharEscola");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchEscola(true);
                    }
                }, "pesquisarEscola");
                decreaseBtn(document.getElementById("pesquisarEscola"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            IncluirAlterar(1, 'divAlterarEscola', 'divIncluirEscola', 'divExcluirEscola', 'apresentadorMensagemEscola', 'divCancelarEscola', 'divLimparEscola');
                            dijit.byId('naturezaPessoa').set("disabled", true);
                            var papelRelac = new Array();
                            papelRelac[0] = TIPO_PAPEL_JURIDICA;
                            showPessoaFK(JURIDICA, 0, papelRelac, function () { desabilitarCampos(eval(MasterGeral())) });
                            dijit.byId("regimeTrib")._onChangeActive = false;
                            dijit.byId("regimeTrib").reset();
                            limparCadPessoaFK();
                            limparParametrosEscola();
                            limparDatas();
                            clearForm('formParametros');
                            clearForm('formEscola');
                            clearForm('formFiscal');
                            clearForm('formPlanoContas');
                            dijit.byId("regimeTrib")._onChangeActive = true;
                            dijit.byId("regimeTrib").set("value", REGIME_NORMAL);
                            dijit.byId("regimeTrib").oldValue = REGIME_NORMAL;
                            
                            //habilitarDesabilitarCamposTipoNFFK(true);   
                           // setDefaultHorarios();
                            var tabContainer = dijit.byId("tabContainer");
                            tabContainer.selectChild(tabContainer.getChildren()[0]);

                            habilitarPlanosContas(Escola(), 0);

                            dijit.byId("cadEscola").show();
                            dijit.byId('tabContainer').resize();
                            dijit.byId('excluirFoto').setAttribute('disabled', 1);
                            dojo.byId("escolaAnterior").value = "";
                            dijit.byId("escolaAnterior").set("value", 0);

                            populaLocalMovto(xhr, 0, 0);

                            dojo.byId('panelDadosNotaServico').style.display = 'none';
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novaEscola");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
					var statusEscola = retornaStatus("statusEscola");
                        require(["dojo/_base/xhr"], function (xhr) {
                            xhr.get({
                                url: Endereco() + "/api/escola/geturlrelatorioescola?" + getStrGridParameters('gridEscola') + "desc=" + document.getElementById("descEscola").value + "&inicio=" + document.getElementById("inicioEscola").checked + "&status=" + statusEscola + "&cnpj=" + dijit.byId("cgc").get("value") + "&fantasia=" + document.getElementById("reduzido").value,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        })
                    }
                }, "relatorioEscola");
                dojo.byId("descApresMsg").value == 'apresentadorMensagemEscola';
                if (hasValue(dijit.byId("cnpj")))
                    dijit.byId("cnpj").on("blur", function (evt) {
                        if (trim(dojo.byId("cnpj").value) != "" && dojo.byId("cnpj").value != "__.___.___/____-__")
                            if (validarCnpj("#cnpj", "apresentadorMensagemEscola"))
                                //validarCPF() ?
                                ExisitsEmpresaByCnpj();
                    });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322980', '765px', '771px');
                        });
                }


                dijit.byId('timeIni').on("change", function (e) {
                    if (!validarHoraView('timeIni', 'timeFim', 'apresentadorMensagemEscola'))
                        dijit.byId('timeIni').reset()
                });

                dijit.byId('timeFim').on("change", function (e) {
                    if (!validarHoraView('timeIni', 'timeFim', 'apresentadorMensagemEscola'))
                        dijit.byId('timeFim').reset()
                })

                //Aumentando o tamanho do campo do nome fantasia:
                dojo.byId('nomFantasia').maxLength = 64;
                //Botões de link para plano de contas - integração

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_MATRICULA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_MATRICULA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasMat");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_SERVICO);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_SERVICO);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasServico");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaItem")))
                                montargridPesquisaItem(function () {
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                    limparPesquisaCursoFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        abrirItemFK();

                                    });
                                    abrirItemFK();
                                    setTimeout(dijit.byId("tipo").set("value", SERVICO), 1000);
                                }, true, true);
                            else {
                                limparPesquisaCursoFK();
                                abrirItemFK();
                            }
                            itemFk = ITEMTXMAT;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btItemTxMat");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaItem")))
                                montargridPesquisaItem(function () {
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                    limparPesquisaCursoFK();
                                    abrirItemFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        abrirItemFK();
                                    });
                                }, true, true);
                            else {
                                limparPesquisaCursoFK();
                                abrirItemFK();
                            }
                            itemFk = ITEMMENSALIDADE;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btItemMens");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaItem")))
                                montargridPesquisaItem(function () {
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                    limparPesquisaCursoFK();
                                    abrirItemFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        abrirItemFK();
                                    });
                                }, true, true);
                            else {
                                limparPesquisaCursoFK();
                                abrirItemFK();
                            }
                            itemFk = ITEMBIBLIOTECA;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btItemBiblio");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            tipoNFFk = TPNFBIBLIO;
                            isTipoSaida = false;
                            if (!hasValue(dijit.byId("gridTipoNFFK"))) {
                                
                                montarTipoNFFK(function () {
                                    loadTpMovtoFK(SAIDA_TIPO);
                                    abrirTipoNFFK();
                                    dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                    });
                                    dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemTipoNFF", null);
                                        pesquisaTipoNFPorTipo();
                                    });
                                    dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                        dijit.byId("fkTipoNF").hide();
                                    });
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
                }, "btTpNFBiblio");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaItem")))
                                montargridPesquisaItem(function () {
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                    limparPesquisaCursoFK();
                                    abrirItemFK();
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        abrirItemFK();
                                    });
                                }, true, true);
                            else {
                                limparPesquisaCursoFK();
                                abrirItemFK();
                            }
                            itemFk = ITEMSERVICO;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btItemServico");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            tipoNFFk = TPNFSERV;
                            isTipoSaida = false;
                            if (!hasValue(dijit.byId("gridTipoNFFK"))) {

                                montarTipoNFFK(function () {
                                    loadTpMovtoFK(SAIDA_TIPO);
                                    abrirTipoNFFKServicoMatriz();
                                    dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                    });
                                    dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemTipoNFF", null);
                                        pesquisaTipoNFPorTipo();
                                    });
                                    dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                        dijit.byId("fkTipoNF").hide();
                                    });
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
                }, "btTpNFServico");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            isTipoSaida = false;
                            tipoNFFk = TPNFTXMENS;
                            if (!hasValue(dijit.byId("gridTipoNFFK"))) {
                             
                                montarTipoNFFK(function () {
                                    loadTpMovtoFK(SAIDA_TIPO);
                                    abrirTipoNFFK();
                                    dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                    });
                                    dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                        pesquisaTipoNFPorTipo();
                                    });
                                    dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                        dijit.byId("fkTipoNF").hide();
                                    });
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
                }, "btTpNFTxMens");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            isTipoSaida = false;
                            tipoNFFk = TPNFMAT;
                            if (!hasValue(dijit.byId("gridTipoNFFK"))) {
                             
                                montarTipoNFFK(function () {
                                    loadTpMovtoFK(SAIDA_TIPO);
                                    abrirTipoNFFK(ENTRADA);
                                    dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                    });
                                    dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                        pesquisaTipoNFPorTipo();
                                    });
                                    dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                        dijit.byId("fkTipoNF").hide();
                                    });
                                });

                                criarEventoSelecionaTipoNF();
                            }
                            else 
                                abrirTipoNFFK(ENTRADA);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btTpNFMaterial");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            isTipoSaida = true;
                            tipoNFFk = TPNFMAT;
                            if (!hasValue(dijit.byId("gridTipoNFFK"))) {
                               
                                montarTipoNFFK(function () {
                                    loadTpMovtoFK(SAIDA_TIPO);
                                    abrirTipoNFFK(SAIDA);
                                    dojo.query("#desTipoNFFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisaPolicaComercialFK(true);
                                    });
                                    dijit.byId("pesquisarTipoNFFK").on("click", function (e) {
                                        pesquisaTipoNFPorTipo();
                                    });
                                    dijit.byId("fecharTipoNFFK").on("click", function (e) {
                                        dijit.byId("fkTipoNF").hide();
                                    });
                                });

                                criarEventoSelecionaTipoNF();
                            }
                            else
                                abrirTipoNFFK(SAIDA);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btTpNFMaterialS");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPolicaComercialFK"))) {
                                montarPoliticaComercial(function () {
                                    abrirPoliticaComercialFK();
                                    dojo.query("#desPoliticaComercialFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) loadPoliticaComercialEsc();
                                    });
                                    dijit.byId("pesquisarPolicaComercialFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        loadPoliticaComercialEsc();
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
                }, "btPolCom");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPolicaComercialFK"))) {
                                montarPoliticaComercial(function () {
                                    abrirPoliticaComercialFK();
                                    dojo.query("#desPoliticaComercialFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) loadPoliticaComercialEsc();
                                    });
                                    dijit.byId("pesquisarPolicaComercialFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        loadPoliticaComercialEsc();
                                    });
                                    dijit.byId("fecharPolicaComercialFK").on("click", function (e) {
                                        dijit.byId("fkPoliticaComercial").hide();
                                    });
                                });

                                criarEventoSelecionaPoliticaServico();
                            }
                            else
                                abrirPoliticaComercialFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btPolServico");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_TAXA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_TAXA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasTaxa");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_JUROS);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_JUROS);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasJuros");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_MULTA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_MULTA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkplanoContasMulta");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_MATERIAL);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_MATERIAL);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasMaterial");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_DESCONTO);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_DESCONTO);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkplanoContasDesconto");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagemEscola',
                                    ESCOLA_TAXA_BANCARIA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, ESCOLA_TAXA_BANCARIA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkplanoContasTaxasBancarias");

                var buttonFkArray = ['fkPlanoContasMat', 'btItemTxMat', 'fkPlanoContasTaxa', 'fkPlanoContasJuros', 'fkplanoContasMulta', 'fkplanoContasDesconto', 'fkplanoContasTaxasBancarias', 'fkPlanoContasServico',
                    'btItemMens', 'btItemBiblio', 'btTpNFBiblio', 'btTpNFTxMens', 'btTpNFMaterial', 'btTpNFMaterialS', 'btPolCom', 'btPolServico', 'fkPlanoContasMaterial',
                    'btItemServico', 'btTpNFServico'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_matricula').value = 0;
                        dojo.byId("planoContasMat").value = "";
                        dijit.byId("limparPlanoMat").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoMat");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_servico').value = 0;
                        dojo.byId("planoContasServico").value = "";
                        dijit.byId("limparPlanoServico").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoServico");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item_servico').value = 0;
                        dojo.byId("itemServico").value = "";
                        dijit.byId("limparItemServico").set('disabled', true);
                    },
                    disabled: true
                }, "limparItemServico");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_tpnf_servico').value = 0;
                        dojo.byId("tpNFServico").value = "";
                        dijit.byId("limparTpNFServico").set('disabled', true);
                    },
                    disabled: true
                }, "limparTpNFServico");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_polComercialServico').value = 0;
                        dojo.byId("dc_pol_comercial_servico").value = "";
                        dijit.byId("limparPolServico").set('disabled', true);
                    },
                    disabled: true
                }, "limparPolServico");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_taxa').value = 0;
                        dojo.byId("planoContasTaxa").value = "";
                        dijit.byId("limparPlanoTaxa").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoTaxa");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_conta_juros').value = 0;
                        dojo.byId("planoContasJuros").value = "";
                        dijit.byId("limparPlanoContaJuros").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoContaJuros");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_conta_multa').value = 0;
                        dojo.byId("planoContasMulta").value = "";
                        dijit.byId("limparPlanoContasMulta").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoContasMulta");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_conta_desc').value = 0;
                        dojo.byId("planoContasDesconto").value = "";
                        dijit.byId("limparPlanoContasDesconto").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoContasDesconto");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_taxa_bco').value = 0;
                        dojo.byId("planoContasTaxasBancarias").value = "";
                        dijit.byId("limparPlanoContasTaxasBancarias").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoContasTaxasBancarias");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_plano_conta_material').value = 0;
                        dojo.byId("planoContasMaterial").value = "";
                        dijit.byId("limparPlanoContasMaterial").set('disabled', true);
                    },
                    disabled: true
                }, "limparPlanoContasMaterial");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item').value = 0;
                        dojo.byId("itemTxMat").value = "";
                        dijit.byId("limparItemTxMat").set('disabled', true);
                    },
                    disabled: true
                }, "limparItemTxMat");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item_mensalidade').value = 0;
                        dojo.byId("itemMensalidade").value = "";
                        dijit.byId("limparItemMens").set('disabled', true);
                    },
                    disabled: true
                }, "limparItemMens");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item_biblioteca').value = 0;
                        dojo.byId("itemBiblioteca").value = "";
                        dijit.byId("limparItemBiblio").set('disabled', true);
                    },
                    disabled: true
                }, "limparItemBiblio");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_tpnf_biblio').value = 0;
                        dojo.byId("tpNFBiblio").value = "";
                        dijit.byId("limparTpNFBiblio").set('disabled', true);
                    },
                    disabled: true
                }, "limparTpNFBiblio");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_tpnf_txmens').value = 0;
                        dojo.byId("tpNFTxMens").value = "";
                        dijit.byId("limparTpNFTxMens").set('disabled', true);
                    },
                    disabled: true
                }, "limparTpNFTxMens");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_tpnf_material').value = 0;
                        dojo.byId("tpNFMaterial").value = "";
                        dijit.byId("limparTpNFMaterial").set('disabled', true);
                    },
                    disabled: true
                }, "limparTpNFMaterial");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_polComercialNF').value = 0;
                        dojo.byId("dc_pol_comercial").value = "";
                        dijit.byId("limparPolCom").set('disabled', true);
                    },
                    disabled: true
                }, "limparPolCom");

                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_tpnf_materialS').value = 0;
                        dojo.byId("tpNFMaterialS").value = "";
                        dijit.byId("limparTpNFMaterialS").set('disabled', true);
                    },
                    disabled: true
                }, "limparTpNFMaterialS");

                var buttonLimparFkArray = ['limparPlanoMat', 'limparPlanoServico', 'limparItemServico', 'limparTpNFServico', 'limparPolServico', 'limparPlanoTaxa', 'limparPlanoContaJuros', 'limparPlanoContasMulta', 'limparPlanoContasDesconto', 'limparPlanoContasTaxasBancarias', 'limparPlanoContasMaterial',
                    'limparItemTxMat', 'limparItemMens', 'limparItemBiblio', 'limparTpNFBiblio', 'limparTpNFTxMens', 'limparTpNFMaterial', 'limparPolCom', 'limparTpNFMaterialS'];

                for (var p = 0; p < buttonLimparFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonLimparFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '40px';
                        buttonFk.parentNode.style.width = '40px';
                    }
                }
                
                dijit.byId("emiteNFS").on("change", function (e) {
                    try {
                        if (e) {
                            dijit.byId("itemTxMat").set("required", true);
                            dijit.byId("itemMensalidade").set("required", true);
                            dijit.byId("tpNFTxMens").set("required", true);
                            dijit.byId("dc_pol_comercial").set("required", true);
                            if (dijit.byId("nrNFAuto").checked) {
                                dijit.byId("nmNFS").set("required", true);
                                dijit.byId("serieNFS").set("required", true);
                            }
                            else {
                                dijit.byId("nmNFS").set("required", false);
                                dijit.byId("serieNFS").set("required", false);
                            }
                        }
                        else {
                            dijit.byId("itemTxMat").set("required", false);
                            dijit.byId("itemMensalidade").set("required", false);
                            dijit.byId("tpNFTxMens").set("required", false);
                            dijit.byId("dc_pol_comercial").set("required", false);
                            if (dijit.byId("nrNFAuto").checked) {
                                dijit.byId("nmNFS").set("required", false);
                                dijit.byId("serieNFS").set("required", false);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("emiteNFM").on("change", function (e) {
                    try {
                        if (e) {
                            dijit.byId("tpNFMaterial").set("required", true);
                            dijit.byId("tpNFMaterialS").set("required", true);
                            dijit.byId("dc_pol_comercial").set("required", true);
                            if (dijit.byId("nrNFAuto").checked) {
                                dijit.byId("nrNFM").set("required", true);
                                dijit.byId("serieNFM").set("required", true);
                            }
                            else {
                                dijit.byId("nrNFM").set("required", false);
                                dijit.byId("serieNFM").set("required", false);
                            }
                           
                        }
                        else {
                            dijit.byId("tpNFMaterial").set("required", false);
                            dijit.byId("tpNFMaterialS").set("required", false);
                            dijit.byId("dc_pol_comercial").set("required", false);
                            if (dijit.byId("nrNFAuto").checked) {
                                dijit.byId("nrNFM").set("required", false);
                                dijit.byId("serieNFM").set("required", false);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("nrNFAuto").on("change", function (e) {
                    try {
                        if (e) {
                            if (dijit.byId("emiteNFM").checked) {
                                dijit.byId("nrNFM").set("required", true);
                                dijit.byId("serieNFM").set("required", true);
                            }
                            if (dijit.byId("emiteNFS").checked) {
                                dijit.byId("nmNFS").set("required", true);
                                dijit.byId("serieNFS").set("required", true);
                            }
                        }
                        else {
                            dijit.byId("nrNFM").set("required", false);
                            dijit.byId("serieNFM").set("required", false);
                            dijit.byId("nmNFS").set("required", false);
                            dijit.byId("serieNFS").set("required", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['descEscola', 'reduzido', 'cgc', 'statusEscola'], 'pesquisarEscola', ready);
                decreaseBtn(document.getElementById("uploaderAssinatura"), '18px');
                new Tooltip({
                    connectId: ["uploaderAssinatura"],
                    label: "Upload",
                    position: ['above']
                });
                //new Tooltip({
                //    connectId: ["nomAssinaturaCertificado"],
                //    label: "sdfasdfasdfa"//,
                //    //position: ['above']
                //});
                dijit.byId('uploaderAssinatura').on("change", function (evt) {
                    try {
                        var mensagensWeb = new Array();
                        var files = dijit.byId("uploaderAssinatura")._files;
                        apresentaMensagem("apresentadorMensagemEscola", null);
                        if (hasValue(files) && files.length > 0) {
                            if (hasValue(files[0]) && files[0].size > 400000) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanho);
                                apresentaMensagem("apresentadorMensagemEscola", mensagensWeb);
                                return false;
                            }
                            if (!verificarExtensaoArquivo(files[0].name)) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtesaoErradaArquivo);
                                apresentaMensagem("apresentadorMensagemEscola", mensagensWeb);
                                return false;
                            }
                            dojo.byId("nomAssinaturaCertificado").value = files[0].name;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                //Chamado 312099 - Setar valor 4 e desabilitar. Foi feita planilha com valores antigos.
                dijit.byId('aulaMaterial').set("disabled", true);

                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
};
// ** fim da grade de Escola **\\


function pesquisaTipoNFFKEscola(id_servico) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var movimento = hasValue(dijit.byId("cbMovtoFK").value) ? dijit.byId("cbMovtoFK").value : 0;
            var id_regime = hasValue(dijit.byId("regimeTrib").value) ? dijit.byId("regimeTrib").value : 0;
            var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getTipoNotaFiscalSearch?desc=" + dijit.byId('descTpNFFK').get('value') + "&natOp=&inicio=" + dijit.byId('inicioTpNFFK').checked + "&status=" + retornaStatus("statusTpNFFK") + "&movimento=" + movimento + "&devolucao=null&escola=false&id_regime_trib=" + id_regime + "&id_servico=" + id_servico ,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_tipo_nota_fiscal"
               }
            ), Memory({ idProperty: "cd_tipo_nota_fiscal" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridTipoNFFK");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region - métodos auziliarares e de persistencia - ExisitsEmpresaByCnpj -  searchEscola - alterarEscola - incluirEscola - deletarEscola
// ******************* Início da persistência da Escola *******************\\
function ExisitsEmpresaByCnpj() {
    var mensagem = ' já esta com este CNPJ, deseja cadastrá-la como Escola? Os dados desta pessoa serão preenchidos automaticamente.';
    require(["dojo/_base/xhr"], function (xhr) {
        var mensagensWeb = new Array();
        if ($("#cnpj").val()) {
            xhr.get({
                url: Endereco() + "/api/escola/getExistsEscolaOrCNPJ?cnpj=" + $("#cnpj").val(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataPopula) {
                try{
                    if (hasValue(dataPopula.retorno) && hasValue(dataPopula.retorno.pessoaJuridica) && dataPopula.retorno.pessoaJuridica.cd_pessoa > 0) {
                        apresentaMensagem("apresentadorMensagemEscola", mensagensWeb);
                        caixaDialogo(DIALOGO_CONFIRMAR, dataPopula.retorno.pessoaJuridica.no_pessoa + mensagem,
                            function executaRetorno() { setarValuePessoaJuridica(dataPopula.retorno); })
                        if (dataPopula.retorno.pessoaJuridica != null && dataPopula.retorno.pessoaJuridica.length < 0)
                            limparCadPessoaFK();
                    } else {
                        if (hasValue(dataPopula) && hasValue(dataPopula.retorno)) {
                        apresentaMensagem(dojo.byId("descApresMsg").value, null);
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgEscolaCNPJExistBase + " " + dataPopula.retorno.no_pessoa);
                        apresentaMensagem("apresentadorMensagemEscola", mensagensWeb);
                    }
                }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemEscola', error);
            });
        }
    });
}

// Procura sala pelo nome
function searchEscola(limparItens) {
    require([
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
            JsonRest({
                target: !hasValue(document.getElementById("descEscola").value) ? Endereco() + "/api/escola/getEscolaSearch?desc=&inicio=" + document.getElementById("inicioEscola").checked + "&status=" + retornaStatus("statusEscola") + "&cnpj=" + document.getElementById("cgc").value + "&fantasia=" + dojo.byId("reduzido").value : Endereco() + "/api/escola/getEscolaSearch?desc=" + document.getElementById("descEscola").value + "&inicio=" + document.getElementById("inicioEscola").checked + "&status=" + retornaStatus("statusEscola") + "&cnpj=" + document.getElementById("cgc").value + "&fantasia=" + dojo.byId("reduzido").value,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_pessoa"
            }), Memory({ idProperty: "cd_pessoa" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridEscola = dijit.byId("gridEscola");
            if (limparItens) {
                gridEscola.itensSelecionados = [];
            }
            gridEscola.noDataMessage = msgNotRegEnc;
            gridEscola.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};
//** fim da função searchSal

function setarTabParametros() {
    try{
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("abaParametros");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function escolaObj(dom, domAttr, nomeCertificadoTemporario) {
    try {

        var empresaValorServico = null;
        var idEmpresaValorServico = null;

        var cdPlanoMat = parseInt(dojo.byId('cd_plano_matricula').value);
        cdPlanoMat = cdPlanoMat > 0 ? cdPlanoMat : null;

        var cdPlanoTaxa = parseInt(dojo.byId('cd_plano_taxa').value);
        cdPlanoTaxa = cdPlanoTaxa > 0 ? cdPlanoTaxa : null;

        //TODO Alterrar para juros MMC
        var cdJuros = parseInt(dojo.byId('cd_plano_conta_juros').value);
        cdJuros = cdJuros > 0 ? cdJuros : null;

        var cdMulta = parseInt(dojo.byId('cd_plano_conta_multa').value);
        cdMulta = cdMulta > 0 ? cdMulta : null;

        var cdDesconto = parseInt(dojo.byId('cd_plano_conta_desc').value);
        cdDesconto = cdDesconto > 0 ? cdDesconto : null;

        var cdPlanoTaxaBancaria = parseInt(dojo.byId('cd_plano_taxa_bco').value);
        cdPlanoTaxaBancaria = cdPlanoTaxaBancaria > 0 ? cdPlanoTaxaBancaria : null;

        var cdPlanoMaterial = parseInt(dojo.byId('cd_plano_conta_material').value);
        cdPlanoMaterial = cdPlanoMaterial > 0 ? cdPlanoMaterial : null;

        if (dojo.byId('panelDadosNotaServico').style.display == 'block' &&
            hasValue(dijit.byId("gridValorServico")) &&
            hasValue(dijit.byId("gridValorServico").store.objectStore) &&
            hasValue(dijit.byId("gridValorServico").store.objectStore.data)) {
            empresaValorServico = dijit.byId("gridValorServico").store.objectStore.data;

        }
            
        var escola = {
            pessoaJuridica: montarDadosPessoaJuridica(dom, domAttr),
            parametro: {
                id_retroativo_caixa: dijit.byId('bloquearMovimento').value,
                id_liquidacao_tit_ant_aberto: dijit.byId('bloquearLiquidacao').checked,
                id_somar_descontos_financeiros: dijit.byId('somarDesconto').checked,
                id_permitir_desc_apos_politica: dijit.byId('permitirPermanencia').checked,
                id_juros_final_semana: dijit.byId('calcularJuros').checked,
                id_alterar_venc_final_semana: dijit.byId('alterarVencimento').checked,
                id_bloquear_venda_sem_estoque: dijit.byId('bloquearVenda').checked,
                id_bloquear_mov_retr_estoque: dijit.byId('bloquerCompras').checked,
                id_requer_plano_contas_mov: dijit.byId('obrigarPreenchimento').checked,
                id_bloquear_imprimir_nf_data_atual: dijit.byId('bloquerImpressao').checked,
                id_liberar_habilitacao_professor: dijit.byId('liberarInsercao').checked,
                id_bloquear_matricula_vaga: dijit.byId('bloquerMatricula').checked,
                id_nro_contrato_automatico: dijit.byId('numeracaoContratoAutomatico').checked,
                id_baixa_automatica_cartao: dijit.byId('idBaixaCartaoCk').checked,
                id_baixa_automatica_cheque: dijit.byId('idBaixaChequeCk').checked,
                id_mostrar_3_boletos_pagina: dijit.byId('ckImprimir3Boletos').checked,
                id_tipo_numero_contrato: hasValue(dijit.byId('numeroMatricula').get("value")) ? dijit.byId('numeroMatricula').get("value") : 1,
                nm_dia_vencimento: hasValue(dijit.byId('diaVencimento').get("value")) ? dijit.byId('diaVencimento').get("value") : 0,
                id_dia_util_vencimento: dijit.byId('diaUtil').checked,
                nm_ultimo_contrato: hasValue(dojo.byId('contrato').value) ? dojo.byId('contrato').value : 0,
                nm_ultimo_matricula: hasValue(dojo.byId('matricula').value) ? dojo.byId('matricula').value : 0,
                nm_ultima_turma: hasValue(dojo.byId('turma').value) ? dojo.byId('turma').value : 0,
                nm_ultimo_recibo: hasValue(dojo.byId('recibo').value) ? dojo.byId('recibo').value : 0,
                id_cobrar_juros_multa: dijit.byId('jurosMulta').checked,
                pc_juros_dia: hasValue(dojo.byId('jurosDia').value) ? dojo.number.parse(dom.byId('jurosDia').value) : 0,
                cd_local_movto: hasValue(dijit.byId('edLocalMovto').value) ? dijit.byId('edLocalMovto').value : null,

                per_desconto_maximo: hasValue(dijit.byId('limitadorPercentual').value) ? parseFloat(dijit.byId('limitadorPercentual').value) : 0,
                nm_niveis_plano_contas: hasValue(dojo.byId('niveisPlanoContas').value) ? dom.byId('niveisPlanoContas').value : 0,

                pc_multa: hasValue(dojo.byId('multa').value) ? dojo.number.parse(dom.byId('multa').value) : 0,
                nm_dias_carencia: hasValue(dojo.byId('carencia').value) ? dojo.byId('carencia').value : 0,
                nm_aulas_sem_material: hasValue(dojo.byId('aulaMaterial').value) ? dojo.byId('aulaMaterial').value : 0,
                nm_faltas_aluno: hasValue(dojo.byId('qtdFaltasAluno').value) ? dojo.byId('qtdFaltasAluno').value : 0,
                nm_dias_titulos_abertos: hasValue(dojo.byId('qtdDiasTitulosAberto').value) ? dojo.byId('qtdDiasTitulosAberto').value : 0,
                nm_minutos_concedidos: hasValue(dojo.byId('minutosConcedidos').value) ? dojo.byId('minutosConcedidos').value : 0,
                nm_media_minima: hasValue(dojo.byId('nm_media_certificado').value) ? dojo.byId('nm_media_certificado').value : 0,
                pc_falta_permitida: hasValue(dojo.byId('pc_falta_certificado').value) ? dojo.byId('pc_falta_certificado').value : 0,

                id_bloquear_alt_dta_biblio: dijit.byId('alteracaoData').checked,
                nm_dias_biblioteca: hasValue(dojo.byId('numeroDias').value) ? dojo.byId('numeroDias').value : 0,
                pc_taxa_dia_biblioteca: hasValue(dojo.byId('taxaMulta').value) ? dojo.number.parse(dom.byId('taxaMulta').value) : 0,

                cd_plano_conta_mat: cdPlanoMat,
                cd_plano_conta_tax: cdPlanoTaxa,
                cd_plano_conta_juros: cdJuros,
                cd_plano_conta_multa: cdMulta,
                cd_plano_conta_desc: cdDesconto,
                cd_plano_conta_taxbco: cdPlanoTaxaBancaria,
                cd_plano_conta_material: cdPlanoMaterial,

                id_gerar_financeiro_contrato: dijit.byId('id_gerar_financeiro_contrato').checked,
                
                //Fiscal
                id_emitir_nf_servico: dijit.byId('emiteNFS').checked,
                id_emitir_nf_mercantil: dijit.byId('emiteNFM').checked,
                id_numero_nf_automatico: dijit.byId('nrNFAuto').checked,
                id_financeiro_negativo: dijit.byId('permiteSaldoMovFinNegativa').checked,
                id_regime_tributario: parseInt(dijit.byId("regimeTrib").value),
                nm_nf_servico: dojo.byId('nmNFS').value,
                dc_serie_nf_servico: dojo.byId('serieNFS').value,
                nm_nf_mercantil: dojo.byId('nrNFM').value,
                dc_serie_nf_mercantil: dojo.byId('serieNFM').value,
                cd_item_taxa_matricula: dojo.byId('cd_item').value > 0 ? dojo.byId('cd_item').value : null,
                cd_item_mensalidade: dojo.byId('cd_item_mensalidade').value > 0 ? dojo.byId('cd_item_mensalidade').value : null,
                cd_item_biblioteca: dojo.byId('cd_item_biblioteca').value > 0 ? dojo.byId('cd_item_biblioteca').value : null,
                cd_tipo_nf_matricula: dojo.byId('cd_tpnf_txmens').value > 0 ? dojo.byId('cd_tpnf_txmens').value : null,
                cd_tipo_nf_material: dojo.byId('cd_tpnf_material').value > 0 ? dojo.byId('cd_tpnf_material').value : null,
                cd_tipo_nf_material_saida: dojo.byId('cd_tpnf_materialS').value > 0 ? dojo.byId('cd_tpnf_materialS').value : null,
                cd_tipo_nf_biblioteca: dojo.byId('cd_tpnf_biblio').value > 0 ? dojo.byId('cd_tpnf_biblio').value : null,
                cd_politica_comercial_nf: dojo.byId('cd_polComercialNF').value > 0 ? dojo.byId('cd_polComercialNF').value : null,
                pc_aliquota_ap_saida: hasValue(dojo.byId('pc_aliquota_ap_saida').value) ? dojo.number.parse(dom.byId('pc_aliquota_ap_saida').value) : 0,
                pc_aliquota_ap_servico: hasValue(dojo.byId('pc_aliquota_ap_servico').value) ? dojo.number.parse(dom.byId('pc_aliquota_ap_servico').value) : 0,
                nm_nf_material: dojo.byId('nrNMM').value,
                cd_item_servico: dojo.byId('panelDadosNotaServico').style.display == 'block' && dojo.byId('cd_item_servico').value > 0 ? dojo.byId('cd_item_servico').value : null, 
                cd_tipo_nf_servico: dojo.byId('panelDadosNotaServico').style.display == 'block' && dojo.byId('cd_tpnf_servico').value > 0 ? dojo.byId('cd_tpnf_servico').value : null,
                cd_plano_conta_servico: dojo.byId('panelDadosNotaServico').style.display == 'block' && dojo.byId('cd_plano_servico').value > 0 ? dojo.byId('cd_plano_servico').value : null,
                cd_politica_servico: dojo.byId('panelDadosNotaServico').style.display == 'block' && dojo.byId('cd_polComercialServico').value > 0 ? dojo.byId('cd_polComercialServico').value : null 
        
            },
            id_empresa_propria: dijit.byId('id_empresa_propria').checked,
            id_empresa_internacional: dijit.byId('id_empresa_internacional').checked,
            nm_dia_gerar_nfs: dijit.byId('nm_dia_gerar_nfs').value,
            nm_dia_gerar_nf_servico: dijit.byId('nm_dia_gerar_nf_servico').value,
            id_pessoa_ativa: dijit.byId('idPessoaAtiva').checked,
            hr_inicial: dom.byId("timeIni").value,
            hr_final: dom.byId("timeFim").value,
            cd_empresa: null,
            cd_empresa_coligada: dijit.byId("escolaAnterior").value,
            dt_inicio: hasValue(dojo.byId("dtaInicio").value) ? dojo.date.locale.parse(dojo.byId("dtaInicio").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            dt_abertura: hasValue(dojo.byId("dtaAbertura").value) ? dojo.date.locale.parse(dojo.byId("dtaAbertura").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            nm_cliente_integracao: hasValue(dijit.byId('nmClienteIntegracao').get('value')) ? dijit.byId('nmClienteIntegracao').get('value') : null,
            nm_escola_integracao: hasValue(dijit.byId('nmEmpresaIntegracao').get('value')) ? dijit.byId('nmEmpresaIntegracao').get('value') : null,
            nm_investimento: hasValue(dijit.byId('nm_investimento').get('value')) ? dojo.number.parse(dom.byId('nm_investimento').value) : 0,
            nm_patrimonio: hasValue(dijit.byId('nm_patrimonio').get('value')) ? dojo.number.parse(dom.byId('nm_patrimonio').value) : 0,
            nome_temp_assinatura_certificado: nomeCertificadoTemporario,
            nome_assinatura_certificado: dojo.byId("nomAssinaturaCertificado").value,
            empresaValorServico: empresaValorServico,
            id_empresa_valor_servico: dojo.byId('panelDadosNotaServico').style.display == 'block' && dojo.byId("cd_pessoa").value > 0 ? dojo.byId("cd_pessoa").value : null
            
        };

        return escola;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function cadastrarOuAlterarEscola(windowUtils, tipoOperacao) {
    try {
        var validado = validarCadastroEscola(windowUtils);
        if (!validado)
            return false;
        var files = dijit.byId("uploaderAssinatura")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("UploadedImage", files[i]);
                }
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/api/professor/uploadCertificadoProfessor",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (results) {
                        try {
                            if (hasValue(results) && hasValue(results.indexOf) && results.indexOf('<') >= 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada3);
                                apresentaMensagem('apresentadorMensagemEscola', mensagensWeb);
                                return false;
                            }
                            if (hasValue(results) && !hasValue(results.erro)) {
                                if (tipoOperacao == CADASTRO)
                                    incluirEscola(results);
                                else
                                    alterarEscola(results);
                            } else
                                apresentaMensagem('apresentadorMensagemEscola', results);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemEscola', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemEscola', mensagensWeb);
            }
        } else {
            if (tipoOperacao == CADASTRO)
                incluirEscola("");
            else
                alterarEscola("");
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCadastroEscola(windowUtils) {
    var validado = true;

    if (!validateCadPessoaFK(windowUtils)) {
        setarTabCad();
        validado = false;
    }

    if (!dijit.byId('formFiscal').validate()) {
        dijit.byId("tabContainer").selectChild(dijit.byId("abaParametros"));
        dijit.byId('tagFiscal').set('open', true);
        validado = false;
    }

    if (!dijit.byId('formHorario').validate()) {
        setarTabCad();
        dijit.byId('panelHorario').set('open', true);
        $('#timeIni').focus();
        validado = false;
    }

    if (!dijit.byId("formFantasiaPessoa").validate()) {
        dijit.byId("panelPessoa").set("open", true);
        validado = false;
    }

    if (!dijit.byId("formDataInicio").validate())
        validado = false;

    if (!dijit.byId("formDataAbertura").validate())
        validado = false;


    if (!dijit.byId("formEscola").validate())
        validado =  false;

    if (!validarFormParametros(windowUtils))
        validado =  false;

    if (validarCnpj("#cnpj", dojo.byId("descApresMsg").value) == false)
        validado = false;

    return validado;
}


// Altera uma Escola
function alterarEscola(nomeCertificadoTemporario) {
    if (validarHorario()) {
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr"], function (dom, domAttr, xhr) {
            try {
                showCarregando();
                var escola = new escolaObj(dom, domAttr, nomeCertificadoTemporario);
                xhr.post({
                    url: Endereco() + "/escola/postEditEscola",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    postData: JSON.stringify(escola)
                }).then(function (data) {
                    try {
                        if (hasValue(data) && data.toString().indexOf('<html>') >= 0) {
                            caixaDialogo(DIALOGO_ERRO, msgSessaoExpiradaMVC, 0, 0, 0);
                            return false;
                        }
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridEscola';
                            var grid = dijit.byId(gridName);
                            apresentaMensagem('apresentadorMensagem', data);
                            if (!hasValue(grid.itensSelecionados)) {
                                grid.itensSelecionados = [];
                            }
                            removeObjSort(grid.itensSelecionados, "cd_pessoa", dom.byId("cd_pessoa").value);
                            insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                            buscarItensSelecionados(gridName, 'selecionadoEscola', 'cd_pessoa', 'selecionaTodosEscola', ['pesquisarEscola', 'relatorioEscola'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_pessoa");

                            dijit.byId("cadEscola").hide();
                        }
                        else apresentaMensagem('apresentadorMensagemEscola', data.erro);
                        showCarregando();
                    }
                    catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemEscola', error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    };
};

function setarTabCad() {
    try{
        var tabContainer = dijit.byId("tabContainer");
        tabContainer.selectChild(tabContainer.getChildren()[0]);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Insere uma Escola
function incluirEscola(nomeCertificadoTemporario) {
    if (validarHorario()) {
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window"], function (dom, domAttr, xhr, ref, windowUtils) {
            try{
                var validado = true;

                showCarregando();
                var escola = new escolaObj(dom, domAttr, nomeCertificadoTemporario);

                xhr.post({
                    url: Endereco() + "/escola/postInsertEscola",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    postData: JSON.stringify(escola)
                }).then(function (data) {
                    try{
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridEscola';
                            var grid = dijit.byId(gridName);

                            apresentaMensagem('apresentadorMensagem', data);
                            data = data.retorno;

                            if (!hasValue(grid.itensSelecionados)) {
                                grid.itensSelecionados = [];
                            }
                            insertObjSort(grid.itensSelecionados, "cd_pessoa", itemAlterado);
                            dijit.byId("cadEscola").hide();
                            buscarItensSelecionados(gridName, 'selecionadoEscola', 'cd_pessoa', 'selecionaTodosEscola', ['pesquisarEscola', 'relatorioEscola'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_pessoa");
                        } else apresentaMensagem('apresentadorMensagemEscola', data.erro);
                        showCarregando();
                    }
                    catch (er) {
                        showCarregando();
                        postGerarLog(er);
                    }
                }, function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemEscola', error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
};

function validarFormParametros(windowUtils) {
    try{
        var validado = true;
        var nroMatricula = hasValue(dijit.byId('matricula').get("value")) ?  dijit.byId('matricula').get("value") : 0;
        nroMatricula = parseInt(nroMatricula);

        var nroContrato = hasValue(dijit.byId('contrato').get('value')) ? dijit.byId('contrato').get('value') : 0;
        nroContrato = parseInt(nroContrato);

        var matricula = dijit.byId("matricula");
        var contrato = dijit.byId("contrato");

        if ((nroMatricula <= 0) || (nroContrato <= 0)) {
            mostrarMensagemCampoValidado(windowUtils, matricula);
            mostrarMensagemCampoValidado(windowUtils, contrato);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgContratoMatriculaEscola);
            apresentaMensagem("apresentadorMensagemEscola", mensagensWeb);

            validado = false
        }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Deleta uma Escola
function deletarEscola(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        showCarregando();
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_pessoa').value != 0)
                itensSelecionados = [{
                    cd_pessoa: dom.byId("cd_pessoa").value,
                    no_Pessoa: dom.byId("nomPessoa").value
                }];
        xhr.post({
            url: Endereco() + "/api/empresa/postDeleteEmpresa",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItens");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadEscola").hide();
                dijit.byId("descEscola").set("value", '');
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridEscola').itensSelecionados, "cd_pessoa", itensSelecionados[r].cd_pessoa);
                searchEscola(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarEscola").set('disabled', false);
                dijit.byId("relatorioEscola").set('disabled', false);
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
            showCarregando();
            if (!hasValue(dojo.byId("cadEscola").style.display))
                apresentaMensagem('apresentadorMensagemEscola', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

//#endregion

//#region  Evento do Link
function eventoEditarEscola(itensSelecionados, xhr) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            dojo.byId('panelDadosNotaServico').style.display = 'none';
            limparCadPessoaFK();
            limparParametrosEscola();
            apresentaMensagem('apresentadorMensagem', '');
            var papelRelac = new Array();
            if (!hasValue(itensSelecionados[0].cd_empresa_coligada))
                itensSelecionados[0].cd_empresa_coligada = 0;
            loadEscolaAnteriorPesq(itensSelecionados[0].cd_pessoa, itensSelecionados[0].cd_empresa_coligada);
            papelRelac[0] = TIPO_PAPEL_JURIDICA;
            //showPessoaFK(JURIDICA, itensSelecionados[0].cd_pessoa, papelRelac);
            keepValues(null, dijit.byId('gridEscola'), true, xhr);
            dijit.byId("cadEscola").show();
            IncluirAlterar(0, 'divAlterarEscola', 'divIncluirEscola', 'divExcluirEscola', 'apresentadorMensagemEscola', 'divCancelarEscola', 'divLimparEscola');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  loadEscolaAnteriorPesq -  populaEscolaAnterior

function loadEscolaAnteriorPesq(cdEscola, cdEscolaAnt) {
    require(["dojo/_base/array", "dojo/request/xhr", "dojo/store/Memory", "dojo/domReady!"], function (Array, xhr, Memory) {
        xhr(Endereco() + "/api/escola/getAllEmpresaColigada?cdEscola=" + cdEscola, {
            handleAs: "json",
            preventCache: true,
            headers: { Accept: "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                var EmpresaAnterior = data.retorno;
                //var isMasterGeral = data.retorno.isMasterGeral;
                populaEscolaAnterior(EmpresaAnterior, Memory, Array, cdEscolaAnt);
                //desabilitarCampos(isMasterGeral);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function populaLocalMovto(xhr, cdLocalMovto, cdEscola) {
    xhr.get({
        url: Endereco() + "/api/financeiro/getLocalMovtoByEscola?cdEscola=" + cdEscola,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            apresentaMensagem("apresentadorMensagemEscola", null);
            loadSelect(data, "edLocalMovto", 'cd_local_movto', 'nomeLocal', cdLocalMovto);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemEscola", error);

    });
}

function populaEscolaAnterior(data, Memory, Array, cdEscolaAnt) {
    try{
        var items = [];
        Array.forEach(data, function (value, i) {
            var nome = value.dc_reduzido_pessoa ? value.dc_reduzido_pessoa : value.no_pessoa;
            items.push({ id: value.cd_pessoa, name: nome });
        });
        var stateStore = new Memory({
            data: items
        });
        var cdEscolaAnterior = dijit.byId("escolaAnterior");

        cdEscolaAnterior.store = stateStore;
        if (cdEscolaAnt > 0) {
            cdEscolaAnterior._onChangeActive = false;
            cdEscolaAnterior.set("value", cdEscolaAnt);
            cdEscolaAnterior._onChangeActive = true;
        }
        else
            cdEscolaAnterior.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function desabilitarCampos(isMasterGeral) {
    try{
        if (!isMasterGeral) {
            dijit.byId('panelPessoaRelacionadas').set('open', true);
            dijit.byId('tagComplemento').set('open', true);

            dojo.byId('planoContas').style.display = "none";
            dojo.byId('numeracaoContrato').style.display = "none";
            dojo.byId('trGerarFinan').style.display = "none";
            dojo.byId('trEmpresaPropria').style.display = "none";
            dijit.byId('atividadePrincipal').set('disabled', true);
            dijit.byId('uploader').set('disabled', true);
            dijit.byId('excluirFoto').set('disabled', true);
            dijit.byId('cadAtividade').set('disabled', true);
            dijit.byId('enviarEmail').set('disabled', true);
            dijit.byId('dtaCadastro').set('disabled', true);
            dijit.byId('horaCadastro').set('disabled', true);
            dijit.byId('idPessoaAtiva').set('disabled', true);
            dijit.byId('estado').set('disabled', true);
            dijit.byId('cidade').set('disabled', true);
            dijit.byId('bairro').set('disabled', true);
            dijit.byId('limparEndereoPrincipal').set('disabled', true);
            dijit.byId('pesLogradouro').set('disabled', true);

            dijit.byId('btnNovoEndereco').set('disabled', true);
            dijit.byId('btnRuaOutrosEnd').set('disabled', true);
            dijit.byId('btnBairroOutrosEnd').set('disabled', true);
            dijit.byId('incluirOutrosEnd').set('disabled', true);
            dijit.byId('btnNovoContato').set('disabled', true);
            dijit.byId('incluirOutrosContatos').set('disabled', true);
            dijit.byId('deleteEscola').set('disabled', true);
            dijit.byId('excluirLink').set('disabled', true);
            dijit.byId('proAtividadePessoaFk').set('disabled', true);
            dijit.byId('limparAtividade').set('disabled', true);
            dijit.byId('niveisPlanoContas').set('disabled', true);

            dijit.byId('aulaMaterial').set("disabled", true);
            dijit.byId('bloquearVenda').set("disabled", true);
            dijit.byId("permiteSaldoMovFinNegativa").set("disabled", true);
            
            dijit.byId('aulaMaterial').set("disabled", true);
            
            dijit.byId('nm_media_certificado').set("disabled", true);

            var totalForms = document.forms;
            var idElements = "";
            for (var i = 0; i < totalForms.length; i++) {
                if (totalForms[i].id != "formParametros" && totalForms[i].id != "formCpfCnpjRelac" &&
                    totalForms[i].id != "formDepRelac" && totalForms[i].id != "formNatRelac" &&
                    totalForms[i].id != "formNoPessoaRelac" && totalForms[i].id != "formSexoRelac" &&
                    totalForms[i].id != "formHorario" && totalForms[i].id != "formFiscal") {
                    for (var j = 0; j < totalForms[i].elements.length; j++) {
                        idElements = totalForms[i].elements[j].id;
                        if (hasValue(idElements)) {
                            if (dijit.byId(idElements) != undefined) {
                                dijit.byId(idElements).set('disabled', true);
                                dojo.byId(idElements).disabled = true;
                                dojo.byId(idElements).readOnly = true;
                            }
                        }
                    }
                }
            }
        } else {

            dijit.byId('aulaMaterial').set("disabled", false);
            dijit.byId('bloquearVenda').set("disabled", false);
            dijit.byId("permiteSaldoMovFinNegativa").set("disabled", false);
        }



        
        dijit.byId('nm_investimento').set('disabled', false);
        dojo.byId('nm_investimento').disabled = false;
        dojo.byId('nm_investimento').readOnly = false;
        dijit.byId('nm_patrimonio').set('disabled', false);
        dojo.byId('nm_patrimonio').disabled = false;
        dojo.byId('nm_patrimonio').readOnly = false;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region funções auxiliares para fk plano de contas
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

function habilitarPlanosContas(cdEscolaSessao, cdEscolaBD, isHabilitar) {
    try{
        var habilitar = true;

        if (cdEscolaSessao != cdEscolaBD)
            habilitar = true
        else
            habilitar = false;

            dijit.byId('fkPlanoContasMat').set('disabled', habilitar);
            dijit.byId('fkPlanoContasTaxa').set('disabled', habilitar);
            dijit.byId('fkPlanoContasJuros').set('disabled', habilitar);
            dijit.byId('fkplanoContasMulta').set('disabled', habilitar);
            dijit.byId('fkplanoContasDesconto').set('disabled', habilitar);
            dijit.byId('fkplanoContasTaxasBancarias').set('disabled', habilitar);
            dijit.byId('fkPlanoContasMaterial').set('disabled', habilitar);

            var totalForms = document.forms;
            var idElements = "";

            for (var i = 0; i < totalForms.length; i++) {
                if (totalForms[i].id == "formPlanoContas") {
                    for (var j = 0; j < totalForms[i].elements.length; j++) {
                        idElements = totalForms[i].elements[j].id;
                        if (hasValue(idElements)) {
                            if (dijit.byId(idElements) != undefined) {
                                dijit.byId(idElements).set('disabled', habilitar);
                                dojo.byId(idElements).disabled = habilitar;
                                dojo.byId(idElements).readOnly = true;
                            }
                        }//if (hasValue(idElements))
                    }//for
                }//(totalForms[i].id == "formPlanoContas")
            }//for
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion
// Tag Fiscal

function abrirItemFK() {
    try {
        
        if (hasValue(dijit.byId("gridPesquisaItem")) && hasValue(dijit.byId("gridPesquisaItem").itensSelecionados))
            dijit.byId("gridPesquisaItem").itensSelecionados = [];
        dijit.byId("fkItem").show();
        dijit.byId("gridPesquisaItem").update();
        populaTipoItemEscola(TIPOMOVIMENTO);
        
        setTimeout(pesquisarItemFK(SERVICO), 1000);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarItemFK() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridPesquisaItem = dijit.byId("gridPesquisaItem");
                var value = gridPesquisaItem.selection.getSelected();

                if (!hasValue(value) && (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0))
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    if (hasValue(gridPesquisaItem.itensSelecionados) && gridPesquisaItem.itensSelecionados.length > 1) {
                        caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                        return false;
                    } else {
                        if (itemFk == ITEMTXMAT) {
                            dojo.byId('cd_item').value = gridPesquisaItem.itensSelecionados[0].cd_item;
                            dojo.byId('itemTxMat').value = gridPesquisaItem.itensSelecionados[0].no_item;
                            dojo.attr('itemTxMat', "title", gridPesquisaItem.itensSelecionados[0].no_item); 
                            dijit.byId("limparItemTxMat").set('disabled', false);
                        }
                        else
                            if (itemFk == ITEMMENSALIDADE) {
                                dojo.byId('cd_item_mensalidade').value = gridPesquisaItem.itensSelecionados[0].cd_item;
                                dojo.byId('itemMensalidade').value = gridPesquisaItem.itensSelecionados[0].no_item;
                                dojo.attr('itemMensalidade', "title", gridPesquisaItem.itensSelecionados[0].no_item);
                                dijit.byId("limparItemMens").set('disabled', false);
                            }
                            else
                                if (itemFk == ITEMBIBLIOTECA) {
                                    dojo.byId('cd_item_biblioteca').value = gridPesquisaItem.itensSelecionados[0].cd_item;
                                    dojo.byId('itemBiblioteca').value = gridPesquisaItem.itensSelecionados[0].no_item;
                                    dojo.attr('itemBiblioteca', "title", gridPesquisaItem.itensSelecionados[0].no_item);
                                    dijit.byId("limparItemBiblio").set('disabled', false);
                                }
                                else
                                    if (itemFk == ITEMSERVICO) {
                                        dojo.byId('cd_item_servico').value = gridPesquisaItem.itensSelecionados[0].cd_item;
                                        dojo.byId('itemServico').value = gridPesquisaItem.itensSelecionados[0].no_item;
                                        dojo.attr('itemServico', "title", gridPesquisaItem.itensSelecionados[0].no_item);
                                        dijit.byId("limparItemServico").set('disabled', false);
                                    }
                                
                    }                    
                    dijit.byId("fkItem").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function populaTipoItemEscola(tipoMovimento) {
    tipoMovimento = tipoMovimento == null ? 0 : tipoMovimento;
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getalltipoitem?tipoMovimento=" + tipoMovimento,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataTipoItem) {
        try {

            loadTipoItem(jQuery.parseJSON(dataTipoItem).retorno, 'tipo');
            dijit.byId("tipo").set("value", SERVICO);
            dijit.byId("tipo").set("disabled", true);
            dijit.byId("tipo").value = SERVICO;

        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemItemFK', error);
    });
}

function abrirPoliticaComercialFK() {
    try {
        limparPesquisaPoliticaComercialFK();
        loadPoliticaComercialEsc();
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
            dojo.byId("dc_pol_comercial").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].dc_politica_comercial;
            dojo.byId("cd_polComercialNF").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].cd_politica_comercial;
            dijit.byId("limparPolCom").set('disabled', false);
            dijit.byId("fkPoliticaComercial").hide();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function criarEventoSelecionaPoliticaServico() {
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
            dojo.byId("dc_pol_comercial_servico").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].dc_politica_comercial;
            dojo.byId("cd_polComercialServico").value = dijit.byId("gridPolicaComercialFK").itensSelecionados[0].cd_politica_comercial;
            dijit.byId("limparPolServico").set('disabled', false);
            dijit.byId("fkPoliticaComercial").hide();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTipoNFFK(tipoMov) {
    try {
        limparPesquisaTipoNFFK();
        var id_servico = tipoNFFk == TPNFMAT ? false : true;
        dijit.byId("cbMovtoFK").set("value", tipoMov != null && tipoMov != undefined ? tipoMov : SERVICO_TIPO);
        dijit.byId("cbMovtoFK").set("disabled", true);
        pesquisaTipoNFFKEscola(id_servico);
        dijit.byId("fkTipoNF").show();
        apresentaMensagem('apresentadorMensagemTipoNFFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTipoNFFKServicoMatriz(tipoMov) {
    try {
        limparPesquisaTipoNFFK();
        var id_servico = tipoNFFk == TPNFMAT ? false : true;
        dijit.byId("cbMovtoFK").set("value", tipoMov != null && tipoMov != undefined ? tipoMov : SERVICO_TIPO);
        dijit.byId("cbMovtoFK").set("disabled", false);
        pesquisaTipoNFFKEscola(id_servico);
        dijit.byId("fkTipoNF").show();
        apresentaMensagem('apresentadorMensagemTipoNFFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarEventoSelecionaTipoNF() {
    try {
        dijit.byId("selecionaTipoNFFK").on("click", function (e) {
            if (isTipoSaida == false) {
                if (hasValue(dijit.byId("gridTipoNFFK")) && (dijit.byId("gridTipoNFFK")._by_idx.length > 0)) {
                    var gridTipoNFFK = dijit.byId("gridTipoNFFK");
                    if (gridTipoNFFK.itensSelecionados != null && gridTipoNFFK.itensSelecionados.length > 1) {
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                        return false;
                    } else {
                        if (gridTipoNFFK.itensSelecionados == null || gridTipoNFFK.itensSelecionados.length <= 0) {
                            caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
                            return false
                        }
                    }
                }
                var tipoNFSelecionado = dijit.byId("gridTipoNFFK").itensSelecionados[0];
                if (tipoNFFk == TPNFBIBLIO) {
                    dojo.byId("tpNFBiblio").value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFBiblio', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dojo.byId("cd_tpnf_biblio").value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dijit.byId("limparTpNFBiblio").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFMAT) {
                    dojo.byId("tpNFMaterial").value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFMaterial', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dojo.byId("cd_tpnf_material").value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dijit.byId("limparTpNFMaterial").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFTXMENS) {
                    dojo.byId('cd_tpnf_txmens').value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dojo.byId('tpNFTxMens').value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFTxMens', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dijit.byId("limparTpNFTxMens").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFSERV) {
                    dojo.byId('cd_tpnf_servico').value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dojo.byId('tpNFServico').value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFServico', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dijit.byId("limparTpNFServico").set('disabled', false);
                }

                dijit.byId("fkTipoNF").hide();
            } else {
                if (hasValue(dijit.byId("gridTipoNFFK")) && (dijit.byId("gridTipoNFFK")._by_idx.length > 0)) {
                    var gridTipoNFFK = dijit.byId("gridTipoNFFK");
                    if (gridTipoNFFK.itensSelecionados != null && gridTipoNFFK.itensSelecionados.length > 1) {
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                        return false;
                    } else {
                        if (gridTipoNFFK.itensSelecionados == null || gridTipoNFFK.itensSelecionados.length <= 0) {
                            caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
                            return false
                        }
                    }
                }
                var tipoNFSelecionado = dijit.byId("gridTipoNFFK").itensSelecionados[0];
                if (tipoNFFk == TPNFBIBLIO) {
                    dojo.byId("tpNFBiblio").value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFBiblio', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dojo.byId("cd_tpnf_biblio").value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dijit.byId("limparTpNFBiblio").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFMAT) {
                    dojo.byId("tpNFMaterialS").value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFMaterialS', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dojo.byId("cd_tpnf_materialS").value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dijit.byId("limparTpNFMaterialS").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFTXMENS) {
                    dojo.byId('cd_tpnf_txmens').value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dojo.byId('tpNFTxMens').value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFTxMens', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dijit.byId("limparTpNFTxMens").set('disabled', false);
                }
                else
                if (tipoNFFk == TPNFSERV) {
                    dojo.byId('cd_tpnf_servico').value = tipoNFSelecionado.cd_tipo_nota_fiscal;
                    dojo.byId('tpNFServico').value = tipoNFSelecionado.dc_tipo_nota_fiscal;
                    dojo.attr('tpNFServico', "title", tipoNFSelecionado.dc_tipo_nota_fiscal);
                    dijit.byId("limparTpNFServico").set('disabled', false);
                } 

                dijit.byId("fkTipoNF").hide();
            }
            

            
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPoliticaComercialEsc() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getPoliticaComercialByEmpresa?politica=" + dijit.byId('desPoliticaComercialFK').get('value') + "&inicio=" + dijit.byId('inicioPolicaComercialFK').checked + "&cdEscola=" + dojo.byId("cd_pessoa").value,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_politica_comercial"
               }
            ), Memory({ idProperty: "cd_politica_comercial" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPolicaComercialFK");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisaTipoNFPorTipo() {
    apresentaMensagem("apresentadorMensagemTipoNFF", null);
    var id_servico = false;
    if (tipoNFFk == TPNFTXMENS || tipoNFFk == TPNFBIBLIO || tipoNFFk == TPNFSERV)
        id_servico = true;
    pesquisaTipoNFFKEscola(id_servico);
}

function limparValorServico() {
    try {
        clearForm("FormDialogOutrosServicos");
        dojo.byId("cd_empresa_valor_servico").value = 0;
        dojo.byId("cd_pessoa_empresa_valor_servico").value = 0;
        dijit.byId("dt_inicio_valor").reset();
        dijit.byId("vl_unitario_servico").reset();
        dojo.byId("vl_unitario_servico").value = 0;
        
    }
    catch (e) {
        postGerarLog(e);
    }
}


function keepValueValorServico() {
    try {
        var grid = dijit.byId("gridValorServico").selection.getSelected();
        cd_pessoa
        if (grid.length > 0) {
            dojo.byId("cd_empresa_valor_servico").value = grid[0].cd_empresa_valor_servico;
            dojo.byId("cd_pessoa_empresa_valor_servico").value = grid[0].cd_pessoa_empresa_valor_servico;
            dojo.byId("dt_inicio_valor").value = grid[0].dta_inicio_valor_servico;
            
            if (hasValue(grid[0].vl_unitario_servico)) {
                dojo.byId('vl_unitario_servico').value =
                    parseFloat(grid[0].vl_unitario_servico).toFixed(2).toString().replace(".", ",");
            } else {
                dojo.byId('vl_unitario_servico').value = '0,00';
            }
                
            
        
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValueValorServicoEdit(itensSelecionados) {
    try {
        var grid = itensSelecionados;
        cd_pessoa
        if (grid.length > 0) {
            dojo.byId("cd_empresa_valor_servico").value = grid[0].cd_empresa_valor_servico;
            dojo.byId("cd_pessoa_empresa_valor_servico").value = grid[0].cd_pessoa_empresa_valor_servico;
            dojo.byId("dt_inicio_valor").value = grid[0].dta_inicio_valor_servico;

            if (hasValue(grid[0].vl_unitario_servico)) {
                dojo.byId('vl_unitario_servico').value =
                    parseFloat(grid[0].vl_unitario_servico).toFixed(2).toString().replace(".", ",");
            } else {
                dojo.byId('vl_unitario_servico').value = '0,00';
            }



        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirValorServico(dom) {
    try {
        if (!dijit.byId("FormDialogValorServicos").validate()) {
            return false;
        }
        var newValorServico = {
            cd_empresa_valor_servico: 0,
            cd_pessoa_empresa: dojo.byId("cd_pessoa").value,
            dta_inicio_valor_servico: hasValue(dojo.byId("dt_inicio_valor").value) ? dojo.byId("dt_inicio_valor").value : null,
            dt_inicio_valor: hasValue(dojo.byId("dt_inicio_valor").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_valor").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,							
            vl_unitario_servico: hasValue(dijit.byId('vl_unitario_servico').get('value')) ? dojo.number.parse(dom.byId('vl_unitario_servico').value) : 0,
        }
        dijit.byId("gridValorServico").store.newItem(newValorServico);
        dijit.byId("gridValorServico").store.save();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item de Valor Serviço incluido com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("dialogValorServico").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function excluirValorServico() {
    try {
        var grid = dijit.byId("gridValorServico").selection.getSelected();
        if (grid.length > 0) {
            var arrayValorServico = dijit.byId("gridValorServico")._by_idx;
            arrayValorServico = jQuery.grep(arrayValorServico, function (value) {
                return value.item != grid[0];
            });
            var dados = [];
            $.each(arrayValorServico, function (index, value) {
                dados.push(value.item);
            });
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
            dijit.byId("gridValorServico").setStore(dataStore);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item de Valor Serviço excluido com sucesso.");
            apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
            dijit.byId("dialogValorServico").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarValorServico(dom) {
    try {
        if (!dijit.byId("FormDialogValorServicos").validate()) {
            return false;
        }
        var grid = dijit.byId("gridValorServico").selection.getSelected();
        if (grid.length > 0) {
            grid[0].cd_empresa_valor_servico = dojo.byId("cd_empresa_valor_servico").value,
                grid[0].cd_pessoa_empresa = dojo.byId("cd_pessoa").value,
            grid[0].dta_inicio_valor_servico = hasValue(dojo.byId("dt_inicio_valor").value) ? dojo.byId("dt_inicio_valor").value : null,
            grid[0].dt_inicio_valor = hasValue(dojo.byId("dt_inicio_valor").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_valor").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,							
            grid[0].vl_unitario_servico = hasValue(dijit.byId('vl_unitario_servico').get('value')) ? dojo.number.parse(dom.byId('vl_unitario_servico').value) : 0
        }
        dijit.byId("gridValorServico").update();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item de Valor Serviço alterado com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("dialogValorServico").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxValorServico(value, rowIndex, obj) {
    try {
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input  id='" + id + "' /> ";

        setTimeout("configuraCheckBoxValorServico(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;

    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxValorServico(value, rowIndex, id) {
    try {
        var dojoId = dojo.byId(id);

        if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
            dijit.byId(id).destroy();

        require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
            ready(function () {
                try {
                    if (hasValue(dojoId) && dojoId.type == 'text')
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            checked: value,
                            onChange: function (b) { checkBoxChangeValorServico(rowIndex) }
                        }, id);
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

function checkBoxChangeValorServico(rowIndex) {
    try {
        var grid = dijit.byId("gridValorServico");
        grid._by_idx[rowIndex].item.ehSelecionado = !grid._by_idx[rowIndex].item.ehSelecionado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function valorUnitarioServicoFormatter(id) {
    return maskFixed(id, 2).toString();
}

function excluirItemValorServico() {
    try {
        var value = null;
        var grid = dijit.byId("gridValorServico");
        var itensSelecionados = montarItensValorServicoSelecionados();
        var ehLink = document.getElementById('ehLinkEdit').value;

        if (eval(ehLink) == false) {
            value = grid.selection.getSelected();
        } else {
            value = itensSelecionados;
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
                return false;
            }
        }
        var itensSelecionado = value;
        var dados = [];

        var arrayValorServico = [];
        arrayValorServico = dijit.byId("gridValorServico")._by_idx;
        if (itensSelecionado.length > 0) {
            $.each(itensSelecionado, function (idx, value) {
                arrayValorServico = jQuery.grep(arrayValorServico, function (val) {
                    if (val && val["item"] != null && val["item"] != undefined) {
                        return val.item != value;
                    }

                });
            });
            $.each(arrayValorServico, function (index, value) {
                dados.push(value.item);
            });
        }


        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
        dijit.byId("gridValorServico").setStore(dataStore);
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Item de Valor Serviço excluido com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("DialogRelac").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarItensValorServicoSelecionados() {
    try {
        var storeRelac = [];
        var gridValorServico = dijit.byId("gridValorServico");
        if (hasValue(gridValorServico) && hasValue(gridValorServico.store.objectStore.data)) {
            storeRelac = gridValorServico.store.objectStore.data;
            if (storeRelac.length > 0) {
                storeRelac = jQuery.grep(storeRelac, function (value) {
                    return value.ehSelecionado == true;
                });
            }
        }
        return storeRelac;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValueValorServico(valorServico) {
    try {
        if (hasValue(dijit.byId("gridValorServico")) && valorServico.length > 0) {
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: valorServico }) });
            dijit.byId("gridValorServico").setStore(dataStore);
        } else {
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) });
            dijit.byId("gridValorServico").setStore(dataStore);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarValorServico(itensSelecionados)
{
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {

            apresentaMensagem(dojo.byId("descApresMsg").value, null);
            IncluirAlterar(0, 'divAlterarValorServico', 'divIncluirValorServico', 'divExcluirValorServico', dojo.byId("descApresMsg").value, 'divCancelarValorServico', 'divClearValorServico');
            limparValorServico();
            keepValueValorServicoEdit(itensSelecionados);
            dijit.byId("dialogValorServico").show();
            apresentaMensagem('apresentadorMensagem', '');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}