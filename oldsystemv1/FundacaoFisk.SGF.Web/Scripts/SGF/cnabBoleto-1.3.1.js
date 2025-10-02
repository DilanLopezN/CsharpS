var TODOS = 0, GERARBOLETOS = 1,CANCELAR_BOLETOS = 2,PEDIDO_BAIXA = 3, ABERTO = 1,FECHADO = 2, INICIAL = 0;
var CADASTRO = 1;
var IMPRESSAO_TODOS = 0, IMPRESSAO_BANCO = 1, IMPRESSAO_ESCOLA = 2;

var EnumBanco = {
    SICRED: 748
}

var EnumTipoConsultaPessoaResponsavel = {
    FILTRO: 2
}

var EnumTipoConsultaAluno = {
    CADASTRO: 1,
    FILTRO: 2
}

var EnumTipoCnab =
{
    GERAR_BOLETOS: 1,
    PEDIDO_BAIXA: 3

}
    
//listaTitulosCnab
function montarCadastroCnabBoleto(permissoes) {
    //Criação da Grade de sala
    require([
    "dojo/_base/xhr",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dijit/form/FilteringSelect",
     "dojox/json/ref"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, FilteringSelect,ref) {
        ready(function () {
            try {
                //(int cd_carteira, int cd_usuario, byte tipo_cnab, int status, string dtInicial,string dtFinal, bool emissao, bool vencimento, int cd_empresa
                // Realiza o tratamento do link proveniente da matrícula:
                var parametros = getParamterosURL();
                var tipo = 0;
                if (hasValue(parametros['tipo'])) {
                    tipo = parametros['tipo'];
                }
                if (hasValue(parametros['cd_contrato'])) {
                    configuraDadosContrato(parametros['cd_contrato'], permissoes, tipo);
                }

                findIsLoadComponetePesquisaCnab(permissoes);
                var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
                //var myStore =
                //    Cache(
                //            JsonRest({
                //                target: Endereco() + "/api/cnab/getCnabSearch?cd_carteira=" + parseInt(0) + "&cd_usuario=" + parseInt(cdUsuario) + "&tipo_cnab=" + parseInt(0) +
                //                    "&status=" + parseInt(0) + "&dtInicial=&dtFinal=&emissao=false&vencimento=false&nossoNumero=0",
                //                handleAs: "json",
                //                preventCache: true,
                //                headers: { "Accept": "application/json", "Authorization": Token() }
                //            }), Memory({}));
                var gridCnabBoleto = new EnhancedGrid({
                    store: dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosCnab' style='display:none'/>", field: "selecionadoCnab", width: "3%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCnab },
                        { name: "Carteira", field: "carteira_cnab", width: "24%", styles: "min-width:80px;" },
                        { name: "Tipo", field: "tipoCnab", width: "10%", styles: "min-width:80px;" },
                        { name: "Emissão", field: "dta_emissao_cnab", width: "8%", styles: "min-width:70px;" },
                        { name: "Vcto.Inicial", field: "dta_inicial_vencimento", width: "8%", styles: "min-width:70px;" },
                        { name: "Vcto.Final", field: "dta_final_vencimento", width: "8%", styles: "min-width:70px;" },
                        { name: "Valor", field: "vlTotalCnab", width: "10%", styles: "min-width:70px;text-align:right;" },
                        { name: "Usuário", field: "usuarioCnab", width: "10%", styles: "min-width:80px;" },
                        { name: "Data Cadastro", field: "dtah_cadastro_cnab", width: "11%", styles: "min-width:80px;" },
                        { name: "Status", field: "statusCnab", width: "8%", styles: "min-width:70px;" },
                        { name: "Contrato", field: "nro_contrato", width: "8%", styles: "min-width:70px;" }
                    ],
                    canSort: true,
                    selectionMode: "single",
                    noDataMessage: msgNotRegEncFiltro,
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridCnabBoleto");
                gridCnabBoleto.startup();
                gridCnabBoleto.itensSelecionados = [];
                gridCnabBoleto.pagination.plugin._paginator.plugin.connect(gridCnabBoleto.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridCnabBoleto, 'cd_cnab', 'selecionaTodosCnab');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridCnabBoleto, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodosCnab')) && dojo.byId('selecionaTodosCnab').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_cnab', 'selecionadoCnab', -1, 'selecionaTodosCnab', 'selecionaTodosCnab', 'gridCnabBoleto')", gridCnabBoleto.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridCnabBoleto.canSort = function (col) { return (Math.abs(col) != 1 && Math.abs(col) != 11) };
                gridCnabBoleto.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        keepValuesCnab(item, gridCnabBoleto, false);
                        IncluirAlterar(0, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
                        dijit.byId("cadCnab").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                //Ciração grade de títulos do cnab\
                var gridTitulosCnab = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosTituloCnab' style='display:none'/>", field: "selecionadoTituloCnab", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTituloCnab },
                        { name: "Nome", field: "nomePessoaTitulo", width: "20%", styles: "min-width:80px;" },
                        { name: "Responsável", field: "nomeResponsavel", width: "20%", styles: "min-width:80px;" },
                        { name: "Emissão", field: "dt_emissao", width: "10%", styles: "min-width:70px;" },
                        { name: "Vencimento", field: "dt_vcto", width: "11%", styles: "min-width:70px;" },
                        { name: "Valor", field: "vlSaldoTitulo", width: "8%", styles: "min-width:70px;text-align:right;" },
                        { name: "Local", field: "descLocalMovtoTitulo", width: "21%", styles: "min-width:80px;" },
                        { name: "N.Título", field: "nm_parcela_e_titulo", width: "10%", styles: "min-width:80px;" }
                    ],
                    //canSort: function(colIndex, field){
                    //    return colIndex !=0 && field != 'selecionadoCurso';
                    //},
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["10", "20", "50", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "10",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridTitulosCnab");
                gridTitulosCnab.canSort = function (col) { return Math.abs(col) != 1 };
                gridTitulosCnab.startup();
                gridTitulosCnab.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        keepValuesTituloCnab(item, gridTitulosCnab, false);
                        dijit.byId("cadTituloCnab").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                //fim criação
                gridTitulosCnab.listaTitulosCnab = new Array();

                //Criação da grade de descontos to título CNAB
                var gridDescontosTituloCNAB = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Data Limite", field: "dta_desconto", width: "50%", styles: "min-width:70px; text-align:center;" },
                        { name: "Valor Desconto", field: "vlDesconto", width: "50%", styles: "min-width:70px;text-align:right;" }
                    ],
                    //canSort: function(colIndex, field){
                    //    return colIndex !=0 && field != 'selecionadoCurso';
                    //},
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["3", "6", "12", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "3",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridDescontosTituloCNAB");
                gridDescontosTituloCNAB.startup();

                //Criação dos botões da tela inicial (pesquisa) do cnab.
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarCnab(true); } }, "pesquisarCnabBoletos");
                decreaseBtn(document.getElementById("pesquisarCnabBoletos"), '32px');
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        try {
                            var pesqDtaInicial = dijit.byId("pesqDtaInicial");
                            var pesqDtaFinal = dijit.byId("pesqDtaFinal");
                            if (document.getElementById("pesqCkEmissao").checked && !hasValue(dojo.byId("pesqDtaFinal").value)) {
                                pesqDtaFinal.set("value", pesqDtaInicial.value);
                            }
                            if (hasValue(dojo.byId("pesqDtaInicial").value) && hasValue(dojo.byId("pesqDtaFinal").value) && dojo.date.compare(pesqDtaInicial.get("value"), pesqDtaFinal.value) > 0) {
                                var mensagensWeb = new Array();
                                var mensagemErro = "";
                                if (document.getElementById("pesqCkEmissao").checked)
                                    mensagemErro = msgErrorDataFinMenorDataInciEmissao;
                                else
                                    mensagemErro = msgErrorDataFinMenorDataInciVencimento;
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemErro);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                pesqDtaFinal._onChangeActive = false;
                                pesqDtaFinal.reset();
                                pesqDtaFinal._onChangeActive = true;
                                return false;
                            }
                            var grid = dijit.byId("gridCnabBoleto");
                            var cdCarteira = hasValue(dijit.byId("pesqCarteira").value) ? dijit.byId("pesqCarteira").value : 0;
                            var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
                            var cdTipo = hasValue(dijit.byId("pesqTipo").value) ? dijit.byId("pesqTipo").value : 0;
                            var cdStatus = hasValue(dijit.byId("pesqStatus").value) ? dijit.byId("pesqStatus").value : 0;
                            var nossoNum = hasValue(dijit.byId("pesNossoNumero").value) ? dijit.byId("pesNossoNumero").value : "";
                            var nmContrato = hasValue(dijit.byId("pesNmContrato").value) ? dijit.byId("pesNmContrato").value : null;
                            var cdResponsavelFiltro = (hasValue(dojo.byId("cdResponsavelFiltroFKCnab").value)) ? dojo.byId("cdResponsavelFiltroFKCnab").value : 0;
                            var cdAlunoFiltro = (hasValue(dojo.byId("cdAlunoFiltroFKCnab").value)) ? dojo.byId("cdAlunoFiltroFKCnab").value : 0;

                            xhr.get({
                                url: Endereco() + "/api/cnab/getUrlRelatorioCnab?" + getStrGridParameters('gridCnabBoleto') + "&cd_carteira=" + parseInt(cdCarteira) + "&cd_usuario=" +
                                                        parseInt(cdUsuario) + "&tipo_cnab=" + parseInt(cdTipo) +
                                                       "&status=" + parseInt(cdStatus) + "&dtInicial=" + dojo.byId("pesqDtaInicial").value + "&dtFinal=" + dojo.byId("pesqDtaFinal").value +
                                                       "&emissao=" + document.getElementById("pesqCkEmissao").checked + "&vencimento=" + document.getElementById("PesqCkVencimento").checked +
                                                       "&nossoNumero=" + nossoNum + "&nro_contrato=" + nmContrato + "&icnab=" + document.getElementById("ckCnab").checked + "&iboleto=" + document.getElementById("ckBoleto").checked + 
                                                        "&cd_responsavel=" + cdResponsavelFiltro + "&cd_aluno=" + cdAlunoFiltro,
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
                }, "relatorioCnab");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                try {
                                    showCarregando();
                                    limparCnab();
                                    dijit.byId("cadTipo").set("disabled", false);
                                    IncluirAlterar(1, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
                                    findIsLoadComponetesNovoCnab();
                                    dijit.byId("cadCnab").show();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                }, "novoCnab");
                var dadosPesqTipo = [
                    { name: "Gerar Boletos", id: "1" },
                    { name: "Cancelamento", id: "2" },
                    { name: "Pedido Baixa", id: "3" }
                ];
                var dadosCadTipo = [
                    { name: "Gerar Boletos", id: "1" },
                    //{ name: "Cancelar Boletos", id: "2" },
                    { name: "Pedido Baixa", id: "3" }
                ];
                var dadosPesqStatus = [
                    { name: "Todos", id: "0" },
                    { name: "Aberto", id: "1" },
                    { name: "Fechado", id: "2" }
                ];
                var dadosStatusCanb = [
                                { name: "Inicial", id: "0" },
                                { name: "Enviado/Gerado", id: "1" },
                                { name: "Baixa Manual", id: "2" },
                                { name: "Confirmado Envio", id: "3" },
                                { name: "Baixa Manual Confirmado", id: "4" },
                                { name: "Pedido Baixa", id: "5" },
                                { name: "Confirmado Pedido Baixa", id: "6" }
                ];
                criarOuCarregarCompFiltering("pesqTipo", dadosPesqTipo, "", TODOS, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name', MASCULINO);
                criarOuCarregarCompFiltering("pesqStatus", dadosPesqStatus, "", TODOS, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');

                //Criação dos botões da tela cadastro do cnab.
                criarOuCarregarCompFiltering("cadTipo", dadosCadTipo, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                criarOuCarregarCompFiltering("cadStatus", [{ name: "Aberto", id: "1" }, { name: "Fechado", id: "2" }], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                //Criação select do status do titulo cnab.
                criarOuCarregarCompFiltering("statusTituloCanbReg", dadosStatusCanb, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { 
                    salvarCnab(); 
                } 
                }, "incluirCnab");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                        keepValuesCnab(null, gridCnabBoleto, null);
                    }
                }, "cancelarCnab");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadCnab").hide(); } }, "fecharCnab");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { alterarCnab(); } }, "alterarCnab");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            try {
                                deletarCnabs(gridCnabBoleto.itensSelecionados);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                    }
                }, "deleteCnab");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparCnab(); } }, "limparCnab");
                //Criação dos botões do modal de edição dos títulos cnab.
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTituloCnab").hide(); } }, "FecharCnabDet");
                new Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { alterarTituloCnab(); } }, "alterarTituloCnabDet");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            limparTituloCnab();
                            keepValuesTituloCnab(null, gridTitulosCnab, null);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarTituloCnabDet");
                //fim
                dijit.byId("pesqTipo").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("pesqTipo").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesqStatus").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < TODOS)
                            dijit.byId("pesqStatus").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("tagTitulosCnab").on("show", function (event) {
                    dijit.byId("gridTitulosCnab").update();
                });
                dijit.byId("cadCnab").on("show", function (event) {
                    dijit.byId("gridTitulosCnab").update();
                });
                dijit.byId("tagDescontosTituloCnab").on("show", function (event) {
                    dijit.byId("gridDescontosTituloCNAB").update();
                });
                criarAcaoRelacionadaCnabPesquisa(gridCnabBoleto);
                criarBotoesFK(Memory, permissoes);
                //FIm criação botões da tela inicial do cnab.
                dojo.byId("tagTitulosCnab_pane").style.paddingBottom = "0px";

                dijit.byId("pesqCkEmissao").on("change", function (event) {
                    try {
                        if (event)
                            dijit.byId("PesqCkVencimento").set("checked", false);
                        else
                            dijit.byId("PesqCkVencimento").set("checked", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("PesqCkVencimento").on("change", function (event) {
                    try {
                        if (event)
                            dijit.byId("pesqCkEmissao").set("checked", false);
                        else
                            dijit.byId("pesqCkEmissao").set("checked", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cadLocal").on("change", function (event) {
                    try {
                        if (hasValue(event) &&
                            hasValue(dijit.byId("cadLocal").item) &&
                            dijit.byId("cadLocal").item.nm_banco === (EnumBanco.SICRED + "")) {
                            dijit.byId("cadRemessa").set("disabled", true);
                        } else {
                            dijit.byId("cadRemessa").set("disabled", false);
                        }
                        if (!isNaN(event) && (dijit.byId("cadLocal").oldValue != event || hasValue(dojo.byId("cd_contrato").value))) {
                            dijit.byId("cadLocal").oldValue = event;
                            locaisTitulo = dijit.byId("multiSeleLocaisTituloCnab");
                            var setCbLocal = new Array();
                            setCbLocal[1] = event;
                            locaisTitulo.setStore(locaisTitulo.store, setCbLocal);
                            if ((dijit.byId("cadTipo").get("value") == CANCELAR_BOLETOS) || (dijit.byId("cadTipo").get("value") == PEDIDO_BAIXA))
                                locaisTitulo.set("disabled", true);
                            else
                                locaisTitulo.set("disabled", false);

                            limparTitulosCnab();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cadDtVencIniCnab").on("change", function (event) {
                    try {
                        var currentDate = new Date();
                        currentDate.setHours(0, 0, 0, 0);
                        if (hasValue(event) &&
                            hasValue(currentDate) &&
                            dojo.date.compare(currentDate, event) > 0 &&
                            dijit.byId("cadTipo").value === (EnumTipoCnab.GERAR_BOLETOS + "")) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgVenciIniMenorDataAtual);
                            apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
                            dijit.byId("cadDtVencIniCnab")._onChangeActive = false;
                            dijit.byId("cadDtVencIniCnab").set("value", null);
                            dijit.byId("cadDtVencIniCnab")._onChangeActive = true;
                            
                        } else {
                            apresentaMensagem('apresentadorMensagemCnab', null);
                        }
                        limparTitulosCnab();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cadDtVencFimCnab").on("change", function (event) {
                    try {
                        limparTitulosCnab();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cadTipo").on("change", function (event) {
                    try {
                        var locaisTitulo = dijit.byId("multiSeleLocaisTituloCnab");
                        if (!isNaN(event)) {
                            limparTitulosCnab();
                            if ((event == CANCELAR_BOLETOS) || (event == PEDIDO_BAIXA)) {
                                locaisTitulo.setStore(locaisTitulo.store, new Array());
                                if (hasValue(dijit.byId("cadLocal").value)) {
                                    var cbLocal = new Array();
                                    cbLocal[1] = dijit.byId("cadLocal").value;
                                    locaisTitulo.setStore(locaisTitulo.store, cbLocal);
                                }
                                locaisTitulo.set("disabled", true);
                            } else
                                dijit.byId("multiSeleLocaisTituloCnab").set("disabled", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323070', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['pesNossoNumero', 'pesqCarteira', 'pesqUsuario', 'pesqTipo', 'pesqStatus', 'pesqDtaInicial', 'pesqDtaFinal', 'pesqCkEmissao', 'PesqCkVencimento'], 'pesquisarCnabBoletos', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function criarAcaoRelacionadaCnabPesquisa(gridCnabBoleto) {
    try {
        //*** Cria os botões do link de ações  e Todos os Itens**\\
        // Adiciona link de Todos os Itens:
        menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new dijit.MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(gridCnabBoleto, 'todosItens', ['pesquisarCnabBoletos', 'relatorioCnab']); pesquisarCnab(false); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new dijit.MenuItem({
            label: "Itens Selecionados",
            onClick: function () { buscarItensSelecionados('gridCnabBoleto', 'selecionadoCnab', 'cd_cnab', 'selecionaTodosCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItensCnab'); }

        });
        menu.addChild(menuItensSelecionados);

        button = new dijit.form.DropDownButton({
            label: "Todos Itens",
            name: "todosItensCnab",
            dropDown: menu,
            id: "todosItensCnab"
        });
        dojo.byId("linkSelecionadosCnab").appendChild(button.domNode);
        //Fim

        // Adiciona link de ações para Cnab:
        var menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var acaoEditar = new dijit.MenuItem({
            label: "Editar",
            onClick: function () { eventoEditarCnab(dijit.byId("gridCnabBoleto").itensSelecionados); }
        });
        menu.addChild(acaoEditar);

        var acaoExcluir = new dijit.MenuItem({
            label: "Excluir",
            onClick: function () { eventoRemoverCnab(dijit.byId("gridCnabBoleto").itensSelecionados); }
        });
        menu.addChild(acaoExcluir);

        //var acaoEditar = new dijit.MenuItem({
        //    label: "Imprimir Titulos",
        //    id: 'ImpBoleta',
        //    onClick: function () { },
        //});
        //menu.addChild(acaoEditar);

        var acaoEditar = new dijit.MenuItem({
            label: "Gerar Cnab",
            onClick: function () {
                if (dijit.byId('gridCnabBoleto').itensSelecionados[0].nro_contrato > 0) {
                    caixaDialogo(DIALOGO_AVISO, msgInfoMenuCnab, null);
                }
                else
                    postGerarCnab(dijit.byId("gridCnabBoleto").itensSelecionados);
            }
        });
        menu.addChild(acaoEditar);

        var acaoEditar = new dijit.MenuItem({
            label: "Gerar Remessa",
            onClick: function () {
                try {
                    var itensSelecionados = dijit.byId('gridCnabBoleto').itensSelecionados;
                    apresentaMensagem('apresentadorMensagem', null);
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else {
                        if (dijit.byId('gridCnabBoleto').itensSelecionados[0].nro_contrato > 0) {
                            caixaDialogo(DIALOGO_AVISO, msgInfoMenuCnab, null);
                        }
                        else {
                            switch (itensSelecionados[0].id_tipo_cnab) {
                                case GERARBOLETOS:
                                    gerarRemessa(itensSelecionados);
                                    break;
                                case PEDIDO_BAIXA:
                                    gerarPedidoBaixa(itensSelecionados);
                                    break;
                            }
                        }
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menu.addChild(acaoEditar);

        var acaoEditar = new dijit.MenuItem({
            label: "Enviar Boletos por E-Mail",
            onClick: function () {
                showCarregando();
                setTimeout(function () { enviarBoletos(dijit.byId('gridCnabBoleto').itensSelecionados) }, 100);
            }
        });
        menu.addChild(acaoEditar);

        //var acaoEditar = new dijit.MenuItem({
        //    label: "Visualizar Boletos",
        //    onClick: function () {
        //        visualizarBoletos(dijit.byId('gridCnabBoleto').itensSelecionados);
        //    }
        //});

        //menu.addChild(acaoEditar);

        var acaoImpBoletos = new dijit.MenuItem({
            label: "Download de Boletos(pdf)",
            onClick: function () {
                baixarPDFBoletosCNAB(dijit.byId('gridCnabBoleto').itensSelecionados);
            }
        });

        menu.addChild(acaoImpBoletos);

        var acaoEditar = new dijit.MenuItem({
            label: "Relatório de Boletos",
            onClick: function () {
                imprimirTitulos(dijit.byId('gridCnabBoleto').itensSelecionados);
            }
        });
        menu.addChild(acaoEditar);

        if (eval(MasterGeral())) {
            var açãoExcluirCNABRegistrado = new dijit.MenuItem({
                label: "Excluir CNAB Registrado",
                onClick: function() {
                    //eventoExcluirCNABRegistrado(dijit.byId('gridCnabBoleto').itensSelecionados);
                    caixaDialogo(DIALOGO_CONFIRMAR, 'Deseja excluir? Ao excluir este arquivo você não mais poderá usa-lo. Os Nosso Número dos Titulos serão mantidos em uma próxima remessa.', function () { eventoExcluirCNABRegistrado(dijit.byId('gridCnabBoleto').itensSelecionados);});
                }
            });
            menu.addChild(açãoExcluirCNABRegistrado);
        }

        var opExcluirCNABRegistrado = new dijit.MenuItem({
            label: "Excluir Boletos Gerados",
            onClick: function () {
                if (dijit.byId('gridCnabBoleto').itensSelecionados[0].nro_contrato == null || dijit.byId('gridCnabBoleto').itensSelecionados[0].nro_contrato == 0) {
                    caixaDialogo(DIALOGO_AVISO, msgInfoMenuCnabExcluir, null);
                }
                else
                    eventoExcluirCNABRegistrado(dijit.byId('gridCnabBoleto').itensSelecionados);
            }
        });
        menu.addChild(opExcluirCNABRegistrado);

        button = new dijit.form.DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasCnab",
            dropDown: menu,
            id: "acoesRelacionadasCnab"
        });
        dojo.byId("linkAcoesCnab").appendChild(button.domNode);

        // Adiciona link de ações Título
        var menuTitulo = new dijit.DropDownMenu({ style: "height: 25px", id: "ActionMenuT" });
        var acaoEditar = new dijit.MenuItem({
            label: "Editar",
            onClick: function () { eventoEditarTituloCnab(dijit.byId("gridTitulosCnab").itensSelecionados); }
        });
        menuTitulo.addChild(acaoEditar);

        var acaoExcluir = new dijit.MenuItem({
            label: "Excluir",
            onClick: function () { deletarItensTituloCnab() }
        });
        menuTitulo.addChild(acaoExcluir);

        var acaoEditar = new dijit.MenuItem({
            label: "Enviar Boletos por E-Mail",
            onClick: function () {
                showCarregando();
                setTimeout(enviarBoletosTitulosCnab(dijit.byId('gridTitulosCnab').itensSelecionados), 100);
            }
        });
        menuTitulo.addChild(acaoEditar);

        var acaoBaixarBoletos = new dijit.MenuItem({
            label: "Download de Boletos(pdf)",
            onClick: function () {
                baixarTitulosBoletos(dijit.byId('gridTitulosCnab').itensSelecionados);
            }
        });
        menuTitulo.addChild(acaoBaixarBoletos);

        //var acaoEditar = new dijit.MenuItem({
        //    label: "Visualizar Boletos",
        //    onClick: function () {
        //        visualizarTitulosBoletos(dijit.byId('gridTitulosCnab').itensSelecionados);
        //    }
        //});
        //menuTitulo.addChild(acaoEditar);

        button = new dijit.form.DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasTituloCnab",
            dropDown: menuTitulo,
            id: "acoesRelacionadasTituloCnab"
        });
        dojo.byId("linkAcoesTituloCnab").appendChild(button.domNode);
        //Todos os itens
        menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new dijit.MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new dijit.MenuItem({
            label: "Itens Selecionados",
            onClick: function () { buscarItensSelecionados(); }
        });
        menu.addChild(menuItensSelecionados);


        button = new dijit.form.DropDownButton({
            label: "Todos Itens",
            name: "todosItensTitulosCanb",
            dropDown: menu,
            id: "todosItensTitulosCanb"
        });
        dojo.byId("linkSelecionadosTituloCnab").appendChild(button.domNode);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function postGerarCnab(itensSelecionados) {
    try {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegGerarCnab, null);
            showCarregando();
        }
        else if (itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegGerarCnab, null);
            showCarregando();
        }
        else {
            var gridCnabBoleto = dijit.byId('gridCnabBoleto');
            var cd_cnab = itensSelecionados[0].cd_cnab;
            var id_tipo_canb = itensSelecionados[0].id_tipo_cnab;

            dojo.xhr.get({
                url: Endereco() + "/api/cnab/gerarCnab?cd_cnab=" + cd_cnab + "&id_tipo_cnab=" + id_tipo_canb,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;

                    if (!hasValue(gridCnabBoleto.itensSelecionados))
                        gridCnabBoleto.itensSelecionados = [];
                    removeObjSort(gridCnabBoleto.itensSelecionados, "cd_cnab", cd_cnab);
                    insertObjSort(gridCnabBoleto.itensSelecionados, "cd_cnab", data);
                    pesquisarCnab(false);
                    if (data.id_tipo_cnab == GERARBOLETOS && data.cd_carteira_cnab >= 0) {
                        //Chama a geração de boleto:
                        if (itensSelecionados[0].id_impressao_carteira_cnab == IMPRESSAO_ESCOLA || itensSelecionados[0].id_impressao_carteira_cnab == IMPRESSAO_TODOS)
                            baixarPDFBoletosCNAB(itensSelecionados);
                        
                        if (data.cd_carteira_cnab > 0 && (!hasValue(data.cd_contrato) || data.cd_contrato == 0))
                            //Chama a geração de remessa:
                            gerarRemessa(itensSelecionados);
                    }
                    else if ((data.id_tipo_cnab == GERARBOLETOS || data.id_tipo_cnab == PEDIDO_BAIXA) && data.cd_carteira_cnab > 0)
                        gerarPedidoBaixa(itensSelecionados);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                    showCarregando();
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
                showCarregando();
            });
        }
    }
    catch (e) {
        postGerarLog(e);
        showCarregando();
    }
}

function gerarPedidoBaixa(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegRemessa, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegRemessa, null);
        else {
            var gridCnabBoleto = dijit.byId('gridCnabBoleto');
            var cd_cnab = itensSelecionados[0].cd_cnab;
            var cd_carteira_cnab = itensSelecionados[0].cd_carteira_cnab;

            dojo.xhr.get({
                url: Endereco() + "/relatorio/postGerarPedidoBaixa?cd_cnab=" + cd_cnab + "&cd_carteira_cnab=" + cd_carteira_cnab,
                sync: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    if (hasValue(data.erro))
                        apresentaMensagem('apresentadorMensagem', data);
                    else
                        window.open(data);
                }
                catch (e) {
                    postGerarLog(e);
                }
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

function gerarRemessa(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegRemessa, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegRemessa, null);
        else {
            var gridCnabBoleto = dijit.byId('gridCnabBoleto');
            var cd_cnab = itensSelecionados[0].cd_cnab;
            var cd_carteira_cnab = itensSelecionados[0].cd_carteira_cnab;

            dojo.xhr.get({
                url: Endereco() + "/relatorio/postGerarRemessa?cd_cnab=" + cd_cnab + "&cd_carteira_cnab=" + cd_carteira_cnab,
                sync: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    if (hasValue(data.erro))
                        apresentaMensagem('apresentadorMensagem', data);
                    else
                        window.open(data);
                }
                catch (e) {
                    postGerarLog(e);
                }
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
//Imprimir os boletos atráves dos títulos selecionados.
function baixarTitulosBoletos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegImpressaoBoleto, null);
        else if (!verificaCdCnabItensSelecionados(itensSelecionados)) {
	        caixaDialogo(DIALOGO_ERRO, "Esta operação somente pode ser realizada após a ação de gerar CNAB.", null);
        }
            /*else if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_ERRO, msgSelectOneRegBoleto, null);*/
        else {
            var value = null;
            var ehLink = null;
            var grid = dijit.byId("gridCnabBoleto");
            var gridCnabBoleto = dijit.byId('gridTitulosCnab');
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

            if (value != null && value.id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoCarteiraBoletosImpressaBanco);
                apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
                return false;
            }

            if (itensSelecionados.length > 100) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgBaixarPDFTitulosCNAB);
                apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
                return false;
            }
            var cd_titulo_cnab = '';

            for (var i = 0; i < itensSelecionados.length; i++)
                cd_titulo_cnab += itensSelecionados[i].cd_titulo_cnab + ';';

            dojo.xhr.get({
                url: Endereco() + "/escola/getBaixarPDFTitulosBoletos?cd_titulos_cnab=" + cd_titulo_cnab.substring(0, cd_titulo_cnab.length - 1),
                sync: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    if (hasValue(data.erro))
                        apresentaMensagem('apresentadorMensagemCnab', data);
                    else
                        window.open(data);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCnab', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaCdCnabItensSelecionados(itensSelecionados) {
	try {
        if (hasValue(itensSelecionados) && itensSelecionados.length > 0) {
            //valida se todos os itens atendem a condição
			return itensSelecionados.every(function (value, index) {
				return value.cd_titulo_cnab != null && value.cd_titulo_cnab != undefined && value.cd_titulo_cnab > 0;
			});
		} else {
			return false;
		}
	} catch (e) {
		postGerarLog(e);
	}
}


//Imprimir os boletos atráves dos títulos selecionados.
function visualizarTitulosBoletos(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegImpressaoBoleto, null);
            /*else if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_ERRO, msgSelectOneRegBoleto, null);*/
        else {
            var value = null;
            var ehLink = null;
            var grid = dijit.byId("gridCnabBoleto");
            var gridCnabBoleto = dijit.byId('gridTitulosCnab');
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

            if (value != null && value.id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoCarteiraBoletosImpressaBanco);
                apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
                return false;
            }
            var cd_titulo_cnab = '';

            for (var i = 0; i < itensSelecionados.length; i++)
                cd_titulo_cnab += itensSelecionados[i].cd_titulo_cnab + ';';

            dojo.xhr.get({
                url: Endereco() + "/escola/getImprimirTitulosBoletos?cd_titulos_cnab=" + cd_titulo_cnab.substring(0, cd_titulo_cnab.length - 1),
                sync: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    if (hasValue(data.erro))
                        apresentaMensagem('apresentadorMensagemCnab', data);
                    else
                        window.open(data, "WindowPopup", 'width=690,height=780');
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCnab', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarMensagemInfoCanb(cnab, mensagem) {
    try{
        var retorno = mensagem + "Carteria:" + cnab.carteira_cnab + ", Tipo:" + cnab.tipoCnab + ".";
        //+ ", Emissão:" + cnab.dta_emissao_cnab + ", Vcto.Inicial:" + cnab.dta_inicial_vencimento +
        //          " Vcto.Final:" + cnab.dta_final_vencimento + " Vcto.Final:" + cnab.dta_final_vencimento + " Valor:" + cnab.vlTotalCnab + " Status:" + cnab.statusCnab;
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//Imprimir todos os boletos dos CNABs selecionados.
function visualizarBoletos(itensSelecionados) {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegImpressaoBoleto, null);
            else if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_ERRO, msgSelectOneRegBoleto, null);
        else {
            var mensagensWeb = new Array();
            var cd_cnab = '';
            var qtdCnabs = 0;
            for (var i = 0, j=0; i < itensSelecionados.length; i++) {
                if (itensSelecionados[i].id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                    mensagensWeb[j] = new MensagensWeb(MENSAGEM_AVISO, montarMensagemInfoCanb(itensSelecionados[i], msgInfoCarteiraImpressaBanco));
                    j += 1;
                }
                else {
                    cd_cnab += itensSelecionados[i].cd_cnab + ';';
                    qtdCnabs++;
                }
            }
            if (mensagensWeb.length > 0)
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            if (qtdCnabs > 0)
                dojo.xhr.get({
                    url: Endereco() + "/escola/getVisualizarBoletos?cd_cnab=" + cd_cnab.substring(0, cd_cnab.length - 1),
                    sync: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try{
                        if (hasValue(data.erro))
                            apresentaMensagem('apresentadorMensagem', data,true);
                        else
                            window.open(data, "WindowPopup", 'width=690,height=780');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error,true);
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Imprimir todos os boletos dos CNABs selecionados.
function baixarPDFBoletosCNAB(itensSelecionados) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            var mensagensWeb = new Array();
            var cd_cnab = '';
            var qtdCnabs = 0;
            for (var i = 0, j = 0; i < itensSelecionados.length; i++) {
                if (itensSelecionados[i].id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                    mensagensWeb[j] = new MensagensWeb(MENSAGEM_AVISO, montarMensagemInfoCanb(itensSelecionados[i], msgInfoCarteiraImpressaBanco));
                    j += 1;
                }
                else {
                    cd_cnab += itensSelecionados[i].cd_cnab + ';';
                    qtdCnabs++;
                }
            }
            if (mensagensWeb.length > 0)
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            if (qtdCnabs > 0)
                dojo.xhr.get({
                    url: Endereco() + "/escola/getBaixarPDFBoletosCNAB?cd_cnab=" + cd_cnab.substring(0, cd_cnab.length - 1),
                    sync: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        if (hasValue(data.erro))
                            apresentaMensagem('apresentadorMensagem', data, true);
                        else
                            window.open(data);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error, true);
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function enviarBoletosTitulosCnab(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegTituloCnabEnviarBoletoEMail, null);
            showCarregando();
        }
        else {
            var value = null;
            var ehLink = null;
            var grid = dijit.byId("gridCnabBoleto");
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

            if (value != null && value.id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoCarteiraBoletosImpressaBanco);
                apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
                showCarregando();
                return false;
            }
            var cd_titulo_cnab = '';
            for (var i = 0; i < itensSelecionados.length; i++)
                cd_titulo_cnab += itensSelecionados[i].cd_titulo_cnab + ';';

            dojo.xhr.get({
                url: Endereco() + "/escola/getBoletoEmailTitulosCnab?cd_titulos_cnab=" + cd_titulo_cnab.substring(0, cd_titulo_cnab.length - 1),
                sync: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    dojo.xhr.get({
                        url: data.retorno,
                        handleAs: "json",
                        headers: { "X-Requested-With": null }
                    }).then(function (data) {
                        try {
                            apresentaMensagem('apresentadorMensagemCnab', data);
                            showCarregando();
                        } catch (e) {
                            showCarregando();
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        //Esta requisição somente irá funcionar no IIS
                        showCarregando();
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCnab', error);
                showCarregando();
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarBotoesFK(Memory, permissoes) {
    try{
        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconSearchSGF', onClick: function () {
                pesquisarTitulosCnabGrade();
            }
        }, "pesTituloCnab");
        decreaseBtn(document.getElementById("pesTituloCnab"), '32px');
        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                        montarGridPesquisaTurmaFK(function () {
                            funcaoFKTurma();
                            dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFKTurmaFK(true);
                                    });
                                }
                                else
                                    abrirAlunoFKTurmaFK(true);
                            });
                        });
                    else
                        funcaoFKTurma();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "turmaFKCnab");
        new dijit.form.Button({
            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                try{
                    dojo.byId("cdTurmaFKCnab").value = 0;
                    dojo.byId("descturmaFKCnab").value = "";
                    dijit.byId("limpaTurmaFKCnab").set('disabled', true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "limpaTurmaFKCnab");
        decreaseBtn(document.getElementById("limpaTurmaFKCnab"), '40px');
        decreaseBtn(document.getElementById("turmaFKCnab"), '18px');
        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                        montarGridPesquisaAluno(false,
                            function() {
                                dojo.byId("selecionaAlunoFKCnab").value = EnumTipoConsultaAluno.CADASTRO;
                                abrirAlunoFK();
                            });
                    } else {
                        dojo.byId("selecionaAlunoFKCnab").value = EnumTipoConsultaAluno.CADASTRO;
                        abrirAlunoFK();
                    }
                   
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "alunoFKCnab");
        new dijit.form.Button({
            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                try{
                    dojo.byId("cdAlunoFKCnab").value = 0;
                    dojo.byId("descAlunoFKCnab").value = "";
                    dijit.byId("limpaAlunoFKCnab").set('disabled', true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "limpaAlunoFKCnab");
        decreaseBtn(document.getElementById("limpaAlunoFKCnab"), '40px');
        decreaseBtn(document.getElementById("alunoFKCnab"), '18px');

        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                try {
                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                        montarGridPesquisaAluno(false,
                            function() {
                                dojo.byId("selecionaAlunoFiltroFKCnab").value = EnumTipoConsultaAluno.FILTRO;
                                abrirAlunoFK();
                            });
                    } else {
                        dojo.byId("selecionaAlunoFiltroFKCnab").value = EnumTipoConsultaAluno.FILTRO;
                        abrirAlunoFK();
                    }
                    
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "alunoFiltroFKCnab");
        new dijit.form.Button({
            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                try {
                    dojo.byId("cdAlunoFiltroFKCnab").value = 0;
                    dojo.byId("descAlunoFiltroFKCnab").value = "";
                    dijit.byId("limpaAlunoFiltroFKCnab").set('disabled', true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "limpaAlunoFiltroFKCnab");
        decreaseBtn(document.getElementById("limpaAlunoFiltroFKCnab"), '40px');
        decreaseBtn(document.getElementById("alunoFiltroFKCnab"), '18px');

        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                        montargridPesquisaPessoa(function () {
                            dojo.byId("selecionaRespFKCnab").value = CADASTRO;
                            abrirPessoaFK();
                            dijit.byId("pesqPessoa").on("click", function (e) {
                                apresentaMensagem("apresentadorMensagemProPessoa", null);
                                pesquisaPessoaFKTitulo(true);
                            });
                        });
                    else {
                        dojo.byId("selecionaRespFKCnab").value = CADASTRO;
                        abrirPessoaFK(true);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "responsavelFKCnab");
        new dijit.form.Button({
            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                try{
                    dojo.byId("cdResponsavelFKCnab").value = 0;
                    dijit.byId("descResponsavelFKCnab").value = "";
                    dojo.byId("descResponsavelFKCnab").value = "";
                    dijit.byId("limpaResponsavelFKCnab").set('disabled', true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "limpaResponsavelFKCnab");
        decreaseBtn(document.getElementById("limpaResponsavelFKCnab"), '40px');
        decreaseBtn(document.getElementById("responsavelFKCnab"), '18px');


        new dijit.form.Button({
            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                try {
                    if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                        montargridPesquisaPessoa(function () {
                            dojo.byId("selecionaRespFiltroFKCnab").value = EnumTipoConsultaPessoaResponsavel.FILTRO;
                            abrirPessoaFK();
                            dijit.byId("pesqPessoa").on("click", function (e) {
                                apresentaMensagem("apresentadorMensagemProPessoa", null);
                                pesquisaPessoaFKTitulo(true);
                            });
                        });
                    else {
                        dojo.byId("selecionaRespFiltroFKCnab").value = EnumTipoConsultaPessoaResponsavel.FILTRO;
                        abrirPessoaFK(true);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "responsavelFiltroFKCnab");
        new dijit.form.Button({
            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                try {
                    dojo.byId("cdResponsavelFiltroFKCnab").value = 0;
                    dijit.byId("descResponsavelFiltroFKCnab").value = "";
                    dojo.byId("descResponsavelFiltroFKCnab").value = "";
                    dijit.byId("limpaResponsavelFiltroFKCnab").set('disabled', true);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "limpaResponsavelFiltroFKCnab");
        decreaseBtn(document.getElementById("limpaResponsavelFiltroFKCnab"), '40px');
        decreaseBtn(document.getElementById("responsavelFiltroFKCnab"), '18px');


        new dijit.form.Button({
            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
            onClick: function () {
                try{
                    if (!hasValue(dijit.byId("gridPesquisaTituloFK"))) {
                        montarGridPesquisaTitulo(true, function () {
                            gridCnabBoleto.rowsPerPage = 5000;
                            abrirFKTitulo();
                            //dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready); });
                            dijit.byId("fecharTituloFK").on("click", function (e) {
                                dijit.byId("proTituloFK").hide();
                            });
                            dijit.byId("pesquisarTituloFK").on("click", function (e) {
                                apresentaMensagem("apresentadorMensagemTituloFK", null);
                                pesquisarTituloFKEspecCnab();
                            });
                            dijit.byId("proTituloFK").on("show", function (e) {
                                dijit.byId("gridPesquisaTituloFK").update();
                            });

                        }, permissoes);
                    }
                    else
                        abrirFKTitulo();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "btnIncluirTitulosCnab");
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Configurar Fks
function abrirPessoaFK(IsResponsavelFKCnab) {
    try{
        limparPesquisaPessoaFK();
        //dijit.byId("tipoPessoaFK").set("value", 1);
        //dijit.byId("tipoPessoaFK").set("disabled", true);
        pesquisaPessoaFKTitulo(IsResponsavelFKCnab);
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        dijit.byId("proPessoa").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKTitulo(IsResponsavelFKCnab) {
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
                var responsavel = (hasValue(IsResponsavelFKCnab) && IsResponsavelFKCnab) ? true : dijit.byId("ckResponsavelPesq").checked;
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value +
                                       "&responsavel=" + responsavel,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function retornarPessoa() {
    try{
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
            if (dojo.byId("selecionaRespFKCnab").value == CADASTRO) {
                $("#cdResponsavelFKCnab").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                $("#descResponsavelFKCnab").val(gridPessoaSelec.itensSelecionados[0].no_pessoa);
                dijit.byId("limpaResponsavelFKCnab").set('disabled', false);
                if (hasValue(dojo.byId("selecionaRespFKCnab"))) {
                    dojo.byId("selecionaRespFKCnab").value = 0;
                }
            } else if (dojo.byId("selecionaRespFiltroFKCnab").value == EnumTipoConsultaPessoaResponsavel.FILTRO) {
                $("#cdResponsavelFiltroFKCnab").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                $("#descResponsavelFiltroFKCnab").val(gridPessoaSelec.itensSelecionados[0].no_pessoa);
                dijit.byId("limpaResponsavelFiltroFKCnab").set('disabled', false);
                if (hasValue(dojo.byId("selecionaRespFiltroFKCnab"))) {
                    dojo.byId("selecionaRespFiltroFKCnab").value = 0;
                }
            }
            else {
                $("#cdPessoaRespFK").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                $("#pessoaTituloFK").val(gridPessoaSelec.itensSelecionados[0].no_pessoa);
                dijit.byId("limparPessoaTituloFK").set('disabled', false);
            }
            //apresentaMensagem(dojo.byId("descApresMsg").value, null);

        }

        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Título FK
function abrirFKTitulo() {
    try{
        var validado = true;
        var mensagensWeb = new Array();
        apresentaMensagem('apresentadorMensagemCnab', null);
        var compTipoCnab = dijit.byId("cadTipo");
        var compCarteira = dijit.byId("cadLocal");
        var dtInicial = dijit.byId("cadDtVencIniCnab");
        var dtFinal = dijit.byId("cadDtVencFimCnab");
        if (!(hasValue(dtInicial.get("value")) && hasValue(dtFinal.get("value")))) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrigInformarPeriodo);
            validado = false;
        }
        if (((compTipoCnab.get("value") == CANCELAR_BOLETOS) || (compTipoCnab.get("value") == PEDIDO_BAIXA)) && !hasValue(compCarteira.get("value"))) {
            if (hasValue(mensagensWeb[0]))
                mensagensWeb[1] = new MensagensWeb(MENSAGEM_ERRO, msgInfoCarteriaObrigIncluirTItulo);
            else
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoCarteriaObrigIncluirTItulo);
            validado = false;
        }
        if (!validado) {
            apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
            return false;
        }
        limparPesquisaTituloFK();
        pesquisarTituloFKEspecCnab();
        dijit.byId("dtInicial").set("disabled", true);
        dijit.byId("dtFinal").set("disabled", true);
        //dijit.byId("ckResponsavelPesq").set("disabled", true);
        dijit.byId("localMovtoPesq").set("disabled", true);
        dijit.byId("proTituloFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTituloFK(permissoes) {
		    try {
		        var valido = true;
		        var gridTituloSelec = dijit.byId("gridPesquisaTituloFK");
		        var gridTitulosCnab = dijit.byId("gridTitulosCnab");
		        if (!hasValue(gridTituloSelec.itensSelecionados))
		            gridTituloSelec.itensSelecionados = [];
		        if (!hasValue(gridTituloSelec.itensSelecionados) || gridTituloSelec.itensSelecionados.length <= 0) {
		            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
		            valido = false;
		        } else
		            if (dijit.byId("cadCnab").open) {
		                showCarregando();
		                apresentaMensagem('apresentadorMensagemCnab', null);
		                quickSortObj(gridTituloSelec.itensSelecionados, 'cd_titulo');
		                var tituloCnab = new Array();

		                for (var i = 0; i < gridTituloSelec.itensSelecionados.length; i++) {
		                    var value = gridTituloSelec.itensSelecionados[i];
		                    tituloCnab[i] = {
		                        cd_titulo_cnab: 0,
		                        cd_titulo: value.cd_titulo,
		                        nomePessoaTitulo: value.nomeCliente,
		                        nomeResponsavel: value.nomeResponsavel,
		                        dt_emissao: value.dt_emissao,
		                        dt_vcto: value.dt_vcto,
		                        Titulo: {
		                            vl_titulo: value.vl_titulo,
		                            vl_saldo_titulo: value.vl_saldo_titulo,
		                            cd_titulo: value.cd_titulo,
		                            nm_titulo: value.nm_titulo,
		                            nm_parcela_titulo: value.nm_parcela_titulo,
		                            id_natureza_titulo: value.id_natureza_titulo,
		                            dt_vcto_titulo: dojo.date.locale.parse(value.dt_vcto, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
		                            dc_tipo_titulo: value.dc_tipo_titulo
		                        },
		                        vlTitulo: value.vlTitulo,
		                        vlSaldoTitulo: value.vlSaldoTitulo,
		                        nm_parcela_e_titulo: value.nm_parcela_e_titulo,
		                        id_status_cnab_titulo: value.id_status_titulo,
		                        descLocalMovtoTitulo: value.descLocalMovto,
		                        dt_vencimento_titulo: dojo.date.locale.parse(value.dt_vcto, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
		                        cd_local_movto_titulo: value.cd_local_movto
		                    };
		                }

		                var nomeRequisicao = '/api/escola/postSimularBaixaCnabOrDadosAdicTitulos';
		                if (permissoes != null && permissoes != "" && possuiPermissao('ctsg', permissoes))
		                    nomeRequisicao = '/api/escola/postSimularBaixaCnabOrDadosAdicTitulosGeral';
		                dojo.xhr.post({
		                    url: Endereco() + nomeRequisicao,
		                    preventCache: true,
		                    handleAs: "json",
		                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
		                    postData: dojox.json.ref.toJson({ titulos: tituloCnab, dataBaixa: "", id_tipo_cnab: parseInt(dijit.byId("cadTipo").get("value")) })
		                }).then(function (titulosCnab) {
		                    try {
		                        //quickSortObj(gridTitulosCnab.store.objectStore.data, 'cd_titulo');
		                        //quickSortObj(gridTitulosCnab.listaTitulosCnab, 'cd_titulo');
		                        $.each(titulosCnab.retorno, function (idx, value) {
		                            gridTitulosCnab.store.objectStore.data.push(value);
		                            //gridTitulosCnab.listaTitulosCnab.push(value);

		                            //insertObjSort(gridTitulosCnab.store.objectStore.data, "cd_titulo", value);
		                            //insertObjSort(gridTitulosCnab.listaTitulosCnab, "cd_titulo", value);
		                        });
		                        gridTitulosCnab.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridTitulosCnab.store.objectStore.data }) }));
		                        showCarregando();
		                    }
		                    catch (e) {
		                        showCarregando();
		                        postGerarLog(e);
		                    }
		                }, function (error) {
		                    showCarregando();
		                    apresentaMensagem('apresentadorMensagemCnab', error);
		                });
		            }

		        if (!valido)
		            return false;
		        dijit.byId("proTituloFK").hide();
		    }
		    catch (e) {
		        showCarregando();
		        postGerarLog(e);
		    }
}

function pesquisarTituloFKEspecCnab() {
    try{
        apresentaMensagem('apresentadorMensagemCnab', null);
        //var elementNroContrato = dojo.byId("nroContratoCnab");
        var compProduto = dijit.byId("produtoCnab");
        var elementTurma = dojo.byId("cdTurmaFKCnab");
        var elementAluno = dojo.byId("cdAlunoFKCnab");
        var elementPessoa = dojo.byId("cdResponsavelFKCnab");
        var localMovtoPesq = dijit.byId("localMovtoPesq").value;
        var cdLocMovto = hasValue(localMovtoPesq) ? localMovtoPesq : 0;
        var cdClienteTitulo = hasValue(dojo.byId("cdPessoaRespFK").value) ? dojo.byId("cdPessoaRespFK").value : 0;
        var cdPessoaTitulo = hasValue(elementPessoa.value) ? elementPessoa.value : 0;
        var multiSeleLocaisTituloCnab = dijit.byId('multiSeleLocaisTituloCnab');
        var elementNroContrato = dojo.byId("numContratoMatricula");
        var compProduto = dijit.byId("produtoCnab");
        var elementTurma = dojo.byId("cdTurmaFKCnab");
        var elementAluno = dojo.byId("cdAlunoFKCnab");
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/financeiro/postTituloSearchGeralTCnab",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({
                cd_pessoa: dijit.byId("ckResponsavelPesq").checked ? cdClienteTitulo : cdPessoaTitulo,
                cd_pessoa_cliente: dijit.byId("ckResponsavelPesq").checked ? 0 : cdClienteTitulo,
                responsavel: dijit.byId("ckResponsavelPesq").checked,
                locMov: cdLocMovto,
                numeroTitulo: dojo.byId("numeroTitulo").value,
                parcelaTitulo: dojo.byId("parcelaTitulo").value,
                dtInicial: dojo.date.locale.parse(dojo.byId("cadDtVencIniCnab").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                dtFinal: dojo.date.locale.parse(dojo.byId("cadDtVencFimCnab").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                locais: getAllLocais(multiSeleLocaisTituloCnab.options),
                locaisEscolhidos: getLocaisSelecionados(multiSeleLocaisTituloCnab.get('value')),
                titulosGrade: getTitulosGrade(dijit.byId("gridTitulosCnab").listaTitulosCnab),
                nro_contrato: hasValue(elementNroContrato.value) ? elementNroContrato.value : 0,
                cd_produto: hasValue(compProduto.value) ? compProduto.value : 0,
                cd_turma: hasValue(elementTurma.value) ? elementTurma.value : 0,
                cd_aluno: hasValue(elementAluno.value) ? elementAluno.value : 0,
                id_tipo_cnab: dijit.byId("cadTipo").get("value"),
                cd_local_movto: dijit.byId("cadLocal").value,
                id_cnab_tipo: hasValue(dijit.byId("nroContratoCnab").value) ? 1 : 0
            })
        }).then(function (dados) {
            try{
                dados = jQuery.parseJSON(dados).retorno;
                var cloneItensSelecionados = cloneArray(dados);
                quickSortObj(cloneItensSelecionados, 'cd_titulo');
                if (dados != null && dados.length > 0)
                    for (var i = 0; i < dados.length; i++) {
                        dados[i].nm_parcela_titulo = dados[i].nm_parcela_titulo + "";
                        dados[i].nm_titulo = dados[i].nm_titulo + "";
                    }
                var gridPesquisaTituloFK = dijit.byId("gridPesquisaTituloFK");
                gridPesquisaTituloFK.itensSelecionados = [];
                //quickSortObj(dados, 'nm_titulo');
                gridPesquisaTituloFK.itensSelecionados = cloneItensSelecionados;
                gridPesquisaTituloFK.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
                gridPesquisaTituloFK.currentPage(1);
                gridPesquisaTituloFK.changePageSize(15);
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem("apresentadorMensagemTituloFK", error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function getLocaisSelecionados(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_local_movto: data[i] });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getAllLocais(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_local_movto: data[i].value });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getTitulosGrade(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_titulo: data[i].cd_titulo });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Turma
function funcaoFKTurma() {
    try{
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = CNABBOLETO;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKCnabBol() {
    try{
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == CNABBOLETO) {
            var valido = true;
            var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
            if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            }
            else {
                dojo.byId("cdTurmaFKCnab").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("descturmaFKCnab").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limpaTurmaFKCnab').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Aluno FK
function retornarAlunoFK() {
    try{
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else if (dojo.byId("selecionaAlunoFKCnab").value == EnumTipoConsultaAluno.CADASTRO) {
            dojo.byId("cdAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("descAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limpaAlunoFKCnab').set("disabled", false);
            dijit.byId("proAluno").hide();

            if (hasValue(dojo.byId("selecionaAlunoFKCnab"))) {
                dojo.byId("selecionaAlunoFKCnab").value = 0;
            }
        }else if (dojo.byId("selecionaAlunoFiltroFKCnab").value == EnumTipoConsultaAluno.FILTRO) {
            dojo.byId("cdAlunoFiltroFKCnab").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("descAlunoFiltroFKCnab").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limpaAlunoFiltroFKCnab').set("disabled", false);
            dijit.byId("proAluno").hide();

            if (hasValue(dojo.byId("selecionaAlunoFiltroFKCnab"))) {
                dojo.byId("selecionaAlunoFiltroFKCnab").value = 0;
            }
        } else {
            dojo.byId("cdAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("descAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limpaAlunoFKCnab').set("disabled", false);
            dijit.byId("proAluno").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try{
        limparPesquisaAlunoFK();
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        dojo.byId('tipoRetornoAlunoFK').value = CADCNABBOLETO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        pesquisarAlunoFK(true);
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Fim
function formatCheckBoxCnab(value, rowIndex, obj) {
    try{
        var gridName = 'gridCnabBoleto';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCnab');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_cnab", grid._by_idx[rowIndex].item.cd_cnab);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_cnab', 'selecionadoCnab', -1, 'selecionaTodosCnab', 'selecionaTodosCnab', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_cnab', 'selecionadoCnab', " + rowIndex + ", '" + id + "', 'selecionaTodosCnab', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTituloCnab(value, rowIndex, obj) {
    try{
        var gridName = 'gridTitulosCnab';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTituloCnab');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionadoTituloCnab', -1, 'selecionaTodosTituloCnab', 'selecionaTodosTituloCnab', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_titulo', 'selecionadoTituloCnab', " + rowIndex + ", '" + id + "', 'selecionaTodosTituloCnab', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findIsLoadComponetePesquisaCnab(permissoes) {
	dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesPesquisaCnab",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            apresentaMensagem("apresentadorMensagem", null);
            var cdusuario = null;
            if (data != null && data.retorno != null) {
                if (data.retorno.cd_usuario != null || data.retorno.cd_usuario > 0)
                    cdusuario = data.retorno.cd_usuario;
                criarOuCarregarCompFiltering("pesqCarteira", data.retorno.carteirasCnab, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localMvto', 'no_carteira_completa', FEMININO);
                loadUsuario(data.retorno.usuarios, data.retorno.adm);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function loadUsuario(data, adm) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var items = [];
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_usuario, name: value.no_login });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId('pesqUsuario').store = stateStore;
		        if (adm)
		            dijit.byId('pesqUsuario').set("value", 0);
		        else
		            dijit.byId('pesqUsuario').set("value", data[0].cd_usuario);
		        //pesquisarCnab(false);
		    }
		    catch (e) {
		        postGerarLog(e);
		    }
		});
}

function findIsLoadComponetesNovoCnab(cd_local_movto) {
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesNovoCnab",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try{
            apresentaMensagem("apresentadorMensagemCnab", null);
            if (dados.retorno != null && dados.retorno != null) {
                if (hasValue(dados.retorno.carteirasCnab))
                    LoadCarteirasCadastro(dados.retorno.carteirasCnab, cd_local_movto);
                //criarOuCarregarCompFiltering("cadLocal", dados.retorno.carteirasCnab, "", null, dojo.ready, dojo.store.Memory,
                //                              dijit.form.FilteringSelect, 'cd_localMvto', 'no_carteira_completa');
                if (hasValue(dados.retorno.produtos))
                    criarOuCarregarCompFiltering("produtoCnab", dados.retorno.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                 'cd_produto', 'no_produto');
                if (hasValue(dados.retorno.locaisMvto)) {
                    var w = dijit.byId("multiSeleLocaisTituloCnab");
                    var locaisCb = [];
                    if (dados.retorno.locaisMvto.length > 0)
                        $.each(dados.retorno.locaisMvto, function (index, val) {
                            locaisCb.push({
                                "cd_local_movto": val.cd_local_movto + "",
                                "nomeLocal": val.nomeLocal
                            });
                        });
                    var storeLocais = new dojo.data.ItemFileReadStore({
                        data: {
                            identifier: "cd_local_movto",
                            label: "nomeLocal",
                            items: locaisCb
                        }
                    });
                    w.setStore(storeLocais, []);
                }
                //Ao clicar em novo habilita o campo cadRemessa
                dijit.byId("cadRemessa").set("disabled", false);
                showCarregando();
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemCnab", error);
        showCarregando();
    });
}

//Funcões do Cnab
function eventoEditarCnab(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridCnabBoleto = dijit.byId('gridCnabBoleto');
            apresentaMensagem('apresentadorMensagem', '');
            keepValuesCnab(null, gridCnabBoleto, true);
            IncluirAlterar(0, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
            dijit.byId("cadCnab").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesCnab(value, grid, ehLink) {
    try{
        grid.itemSelecionado = null;
        limparCnab();
        dijit.byId("cadTipo").set("disabled", true);
        showCarregando();
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

        if (value.cd_cnab > 0){
            showEditCnab(value.cd_cnab);
            grid.itemSelecionado = value;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Funções Título Cnab
function deletarItensTituloCnab() {
    try{
        var grid = dijit.byId("gridTitulosCnab");
        var gridCnab = dijit.byId("gridCnabBoleto");
        var itensSelecionados = grid.itensSelecionados;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else {
            if (hasValue(gridCnab) && hasValue(gridCnab.itemSelecionado) && gridCnab.itemSelecionado.id_status_cnab == FECHADO) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotExcTituloCnab);
                apresentaMensagem("apresentadorMensagemCnab", mensagensWeb);
                return false;
            }
            showCarregando();
            var nomeId = 'cd_titulo';
            var arrayClone = cloneArray(grid.store.objectStore.data);
            var arrayCloneListaTotal = cloneArray(grid.listaTitulosCnab);
            quickSortObj(arrayClone, 'cd_titulo');
            quickSortObj(arrayCloneListaTotal, 'cd_titulo');
            grid.store.save();

            if (itensSelecionados.length > 0 && hasValue(arrayClone) && arrayClone.length > 0) {
                //Percorre a lista da grade para deleção (O(n)):
                for (var i = itensSelecionados.length - 1; i >= 0; i--) {
                    // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                    if (binaryObjSearch(arrayClone, nomeId, eval('itensSelecionados[i].' + nomeId)) != null) {
                        var posicao = binaryObjSearch(arrayClone, nomeId, eval('itensSelecionados[i].' + nomeId));
                        arrayClone.splice(posicao, 1); // Remove o item do array
                    }
                    if (binaryObjSearch(arrayCloneListaTotal, nomeId, eval('itensSelecionados[i].' + nomeId)) != null) {
                        var posicao = binaryObjSearch(arrayCloneListaTotal, nomeId, eval('itensSelecionados[i].' + nomeId));
                        arrayCloneListaTotal.splice(posicao, 1); // Remove o item do array
                    }
                }
                itensSelecionados = new Array();
                var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: arrayClone }) });
                grid.setStore(dataStore);
                grid.update();
                grid.itensSelecionados = [];
                grid.listaTitulosCnab = arrayCloneListaTotal;
                showCarregando();
            } else
                caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTituloCnab(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparTituloCnab();
            var gridTitulosCnab = dijit.byId('gridTitulosCnab');
            apresentaMensagem('apresentadorMensagemTituloCnab', '');
            keepValuesTituloCnab(null, gridTitulosCnab, true);
            dijit.byId("cadTituloCnab").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCnab() {
    try{
        //clearForm("formCnab");
        apresentaMensagem("apresentadorMensagemCnab", null);
        dojo.byId("cd_cnab").value = 0;
        dojo.byId("cd_contrato").value = 0;
        dijit.byId("nroContratoCnab").reset();
        //Tag Principal
        dijit.byId("cadTipo")._onChangeActive = false;
        dijit.byId("cadTipo").set("value", GERARBOLETOS);
        dijit.byId("cadTipo")._onChangeActive = true;
        dijit.byId("cadStatus").set("value", ABERTO);
        dijit.byId("cadLocal")._onChangeActive = false;
        dijit.byId("cadLocal").reset();
        dijit.byId("cadLocal")._onChangeActive = true;
        dijit.byId("cadLocal").oldValue = 0;
        //dijit.byId("cadTipo").reset();
        dijit.byId("cadDtVencIniCnab").value = ""
        dijit.byId("cadDtVencFimCnab").value = ""
        dojo.byId("cadDtVencIniCnab").value = ""
        dojo.byId("cadDtVencFimCnab").value = ""
        //dijit.byId("cadDtVencIniCnab").reset();
        //dijit.byId("cadDtVencFimCnab").reset();
        //dijit.byId("cadStatus").reset();
        dijit.byId("ckResponsavel").reset();
        dojo.byId("cadValorCnab").value = 0;
        //Filtros Especiais para aluno
        dojo.byId("cdTurmaFKCnab").value = 0;
        dojo.byId("descturmaFKCnab").value = "";
        dojo.byId("cdAlunoFKCnab").value = 0;
        dojo.byId("descAlunoFKCnab").value = "";
        dojo.byId("cdResponsavelFKCnab").value = 0;
        dojo.byId("descResponsavelFKCnab").value = "";
        dijit.byId("produtoCnab").reset();
        dijit.byId("nroContratoCnab").reset();
        dojo.byId("cadRemessa").value = "";
        dijit.byId("nmDiasProtesto").reset();
        dijit.byId("numContratoMatricula").reset();
        //Titulos
        if (dijit.byId('multiSeleLocaisTituloCnab').store != null)
            dijit.byId('multiSeleLocaisTituloCnab').setStore(dijit.byId('multiSeleLocaisTituloCnab').store, [0]);
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");
        if (hasValue(gridTitulosCnab)) {
            gridTitulosCnab.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridTitulosCnab.itensSelecionados = [];
            gridTitulosCnab.listaTitulosCnab = [];
        }

        //Ao limpar habilita o campo cadRemessa
        dijit.byId("cadRemessa").set("disabled", false);

        dijit.byId("tagTitulosCnab").set("open", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTitulosCnabGrade() {
    try{
        var grid = dijit.byId("gridTitulosCnab");
        var multiSeleLocaisTituloCnab = dijit.byId('multiSeleLocaisTituloCnab');
        var elementNroContrato = dojo.byId("nroContratoCnab");
        var compProduto = dijit.byId("produtoCnab");
        var elementTurma = dojo.byId("cdTurmaFKCnab");
        var elementAluno = dojo.byId("cdAlunoFKCnab");
        var elementPessoa = dojo.byId("cdResponsavelFKCnab");

        if (grid.listaTitulosCnab != null && grid.listaTitulosCnab.length > 0) {
            var data = new Array();
            for (var i = 0; i < grid.listaTitulosCnab.length; i++)

                if ((!hasValue(parseInt(elementNroContrato.value)) || grid.listaTitulosCnab[i].nro_contrato == parseInt(elementNroContrato.value))
                    && (!hasValue(parseInt(compProduto.value)) || grid.listaTitulosCnab[i].cd_produto_escola == parseInt(compProduto.value))
                        && (!hasValue(parseInt(elementTurma.value)) || (grid.listaTitulosCnab[i].cd_turma_titulo == parseInt(elementTurma.value) || (hasValue(grid.listaTitulosCnab[i].cd_turma_PPT && grid.listaTitulosCnab[i].cd_turma_PPT == parseInt(elementTurma.value)))))
                        && (!hasValue(parseInt(elementAluno.value)) || grid.listaTitulosCnab[i].cd_aluno == parseInt(elementAluno.value))
                        && (!hasValue(parseInt(elementPessoa.value)) || grid.listaTitulosCnab[i].cd_pessoa_responsavel == parseInt(elementPessoa.value))) {

                    var locaisEscolhidos = getLocaisSelecionados(multiSeleLocaisTituloCnab.get('value'));
                    if (hasValue(locaisEscolhidos))
                        for (var j = 0; j < locaisEscolhidos.length; j++) {
                            if (grid.listaTitulosCnab[i].cd_local_movto_titulo == parseInt(locaisEscolhidos[j].cd_local_movto)) {
                                data.push(grid.listaTitulosCnab[i]);
                                break;
                            }
                        }
                    else
                        data.push(grid.listaTitulosCnab[i]);
                }
            grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
        }
            /*dojo.xhr.post({
                url: Endereco() + "/api/financeiro/postPesquisaTituloCnab",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: dojox.json.ref.toJson({
                    nro_contrato: hasValue(elementNroContrato.value) ? elementNroContrato.value : 0,
                    cd_produto: hasValue(compProduto.value) ? compProduto.value : 0,
                    cd_turma: hasValue(elementTurma.value) ? elementTurma.value : 0,
                    cd_aluno: hasValue(elementAluno.value) ? elementAluno.value : 0,
                    cd_pessoa: hasValue(elementPessoa.value) ? elementPessoa.value : 0,
                    locaisEscolhidos: getLocaisSelecionados(multiSeleLocaisTituloCnab.get('value')),
                    titulosGrade: grid.listaTitulosCnab
                })
            }).then(function (data) {
                try{
                    data = jQuery.parseJSON(data).retorno;
                    grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemCnab", error);
                showCarregando();
            });*/
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D Cnab

function showEditCnab(cdCnab) {
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesByCnabEdit?cd_cnab=" + cdCnab,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            apresentaMensagem("apresentadorMensagemCnab", null);
            if (!hasValue(dados.erro)) {
                if (dados.retorno != null && dados.retorno != null) {
                    if (hasValue(dados.retorno.carteirasCnab))
                        LoadCarteirasCadastro(dados.retorno.carteirasCnab, dados.retorno.cd_carteira_cnab);
                    //criarOuCarregarCompFiltering("cadLocal", dados.retorno.carteirasCnab, "", dados.retorno.cd_carteira_cnab, dojo.ready, dojo.store.Memory,
                    //                              dijit.form.FilteringSelect, 'cd_localMvto', 'no_carteira_completa');
                    if (hasValue(dados.retorno.produtos))
                        criarOuCarregarCompFiltering("produtoCnab", dados.retorno.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
                                                     'cd_produto', 'no_produto');
                    if (hasValue(dados.retorno.locaisMvto)) {
                        var w = dijit.byId("multiSeleLocaisTituloCnab");
                        var locaisCb = [];
                        if (dados.retorno.locaisMvto.length > 0)
                            $.each(dados.retorno.locaisMvto, function (index, val) {
                                locaisCb.push({
                                    "cd_local_movto": val.cd_local_movto + "",
                                    "nomeLocal": val.nomeLocal
                                });
                            });
                        var storeLocais = new dojo.data.ItemFileReadStore({
                            data: {
                                identifier: "cd_local_movto",
                                label: "nomeLocal",
                                items: locaisCb
                            }
                        });
                        w.setStore(storeLocais, []);
                    }
                    loadDataCnab(dados.retorno);
                }
            } else {
                apresentaMensagem('apresentadorMensagemCnab', data);
                showCarregando();
            }
            showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemCnab", error);
        showCarregando();
    });
}

function loadDataCnab(data) {
    try {
        
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");
        var compCarteira = dijit.byId("cadLocal");
        //Tag Principal
        dojo.byId("cd_cnab").value = data.cd_cnab;
        dojo.byId("cd_contrato").value = data.cd_contrato;

        if (hasValue(data.cd_contrato)) {
            dijit.byId("nroContratoCnab").set("value", data.nro_contrato);
            dijit.byId("numContratoMatricula").set("value", data.nro_contrato);
            dijit.byId("nroContratoCnab").set('disabled', true);
        } else {
            dijit.byId("nroContratoCnab").set('disabled', false);
            dijit.byId("nroContratoCnab").set("value", null);
            dijit.byId("numContratoMatricula").set("value", null);
        }
        
        compCarteira._onChangeActive = false;
        compCarteira.set("value", data.cd_local_movto);
        if (hasValue(data.cd_local_movto)) {
            var locaisTitulo = dijit.byId("multiSeleLocaisTituloCnab");
            var setCbLocal = new Array();
            setCbLocal[1] = data.cd_local_movto;
            locaisTitulo.setStore(locaisTitulo.store, setCbLocal);
        } else
            if (dijit.byId('multiSeleLocaisTituloCnab').store != null)
                dijit.byId('multiSeleLocaisTituloCnab').setStore(dijit.byId('multiSeleLocaisTituloCnab').store, [0]);
        compCarteira.oldValue = data.cd_local_movto;
        compCarteira._onChangeActive = true;
        dijit.byId("cadTipo")._onChangeActive = false;
        dijit.byId("cadTipo").set("value", data.id_tipo_cnab);
        dijit.byId("cadTipo")._onChangeActive = true;
        if (hasValue(data.dt_inicial_vencimento)) {
            dijit.byId("cadDtVencIniCnab")._onChangeActive = false;
            dijit.byId("cadDtVencIniCnab").set("value", data.dt_inicial_vencimento);
            dijit.byId("cadDtVencIniCnab")._onChangeActive = true;
        }
        if (hasValue(data.dt_final_vencimento)) {
            dijit.byId("cadDtVencFimCnab")._onChangeActive = false;
            dijit.byId("cadDtVencFimCnab").set("value", data.dt_final_vencimento);
            dijit.byId("cadDtVencFimCnab")._onChangeActive = true;
        }
        dojo.byId("cadValorCnab").value = data.vlTotalCnab;
        dijit.byId("ckResponsavel").set("value", data.id_responsavel_cnab);
        dijit.byId("cadStatus").set("value", data.id_status_cnab);
        dojo.byId("cadRemessa").value = data.no_arquivo_remessa;
        if (hasValue(data.nm_dias_protesto))
            dijit.byId("nmDiasProtesto").set("value", data.nm_dias_protesto);
        //Titulos
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");
        if (hasValue(data.TitulosCnab)) {
            gridTitulosCnab.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.TitulosCnab }) }));
            gridTitulosCnab.itensSelecionados = [];
            gridTitulosCnab.listaTitulosCnab = data.TitulosCnab;
        }

        if (hasValue(dijit.byId("cadLocal").item) &&
            dijit.byId("cadLocal").item.nm_banco === (EnumBanco.SICRED + "")) {
            dijit.byId("cadRemessa").set("disabled", true);
        } else {
            dijit.byId("cadRemessa").set("disabled", false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarCnab(limparItens) {
    try{
        var pesqDtaInicial = dijit.byId("pesqDtaInicial");
        var pesqDtaFinal = dijit.byId("pesqDtaFinal");
        if (document.getElementById("pesqCkEmissao").checked && !hasValue(dojo.byId("pesqDtaFinal").value)) {
            pesqDtaFinal.set("value", pesqDtaInicial.value);
        }
        if (hasValue(dojo.byId("pesqDtaInicial").value) && hasValue(dojo.byId("pesqDtaFinal").value) && dojo.date.compare(pesqDtaInicial.get("value"), pesqDtaFinal.value) > 0) {
            var mensagensWeb = new Array();
            var mensagemErro = "";
            if (document.getElementById("pesqCkEmissao").checked)
                mensagemErro = msgErrorDataFinMenorDataInciEmissao;
            else
                mensagemErro = msgErrorDataFinMenorDataInciVencimento;
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemErro);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            pesqDtaFinal._onChangeActive = false;
            pesqDtaFinal.reset();
            pesqDtaFinal._onChangeActive = true;
            return false;
        }
        var grid = dijit.byId("gridCnabBoleto");
        var cdCarteira = hasValue(dijit.byId("pesqCarteira").value) ? dijit.byId("pesqCarteira").value : 0;
        var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
        var cdTipo = hasValue(dijit.byId("pesqTipo").value) ? dijit.byId("pesqTipo").value : 0;
        var cdStatus = hasValue(dijit.byId("pesqStatus").value) ? dijit.byId("pesqStatus").value : 0
        var nossoNum = hasValue(dijit.byId("pesNossoNumero").value) ? dijit.byId("pesNossoNumero").value : "";
        var nmContrato = hasValue(dijit.byId("pesNmContrato").value) ? dijit.byId("pesNmContrato").value : 0;
        var pesqDtaInicial = dijit.byId("pesqDtaInicial").validate() ? dojo.byId("pesqDtaInicial").value : "";
        var pesqDtaFinal = dijit.byId("pesqDtaFinal").validate() ? dojo.byId("pesqDtaFinal").value : "";
        var cdResponsavelFiltro = (hasValue(dojo.byId("cdResponsavelFiltroFKCnab").value))? dojo.byId("cdResponsavelFiltroFKCnab").value : 0;
        var cdAlunoFiltro = (hasValue(dojo.byId("cdAlunoFiltroFKCnab").value))? dojo.byId("cdAlunoFiltroFKCnab").value : 0;
        var myStore =
            dojo.store.Cache(
                    dojo.store.JsonRest({
                        target: Endereco() + "/api/cnab/getCnabSearch?cd_carteira=" + parseInt(cdCarteira) + "&cd_usuario=" + parseInt(cdUsuario) + "&tipo_cnab=" + parseInt(cdTipo) +
                                   "&status=" + parseInt(cdStatus) + "&dtInicial=" + pesqDtaInicial  + "&dtFinal=" + pesqDtaFinal+
                                   "&emissao=" + document.getElementById("pesqCkEmissao").checked + "&vencimento=" + document.getElementById("PesqCkVencimento").checked + "&nossoNumero=" + nossoNum
                        + "&nro_contrato=" + nmContrato + "&icnab=" + document.getElementById("ckCnab").checked + "&iboleto=" + document.getElementById("ckBoleto").checked +
                                   "&cd_responsavel=" + cdResponsavelFiltro + "&cd_aluno=" + cdAlunoFiltro,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), dojo.store.Memory({}));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });

        if (limparItens)
            grid.itensSelecionados = [];
        grid.noDataMessage = msgNotRegEnc;
        grid.setStore(dataStore);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCnab() {
    try{
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");
        var validado = true;
        if (!dijit.byId("formCnab").validate()) {
            validado = false;
        }
        gridTitulosCnab.store.save();
        var itens = gridTitulosCnab.store.objectStore.data;
        if (!hasValue(itens) || itens.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrIncluirTitulosCnab);
            apresentaMensagem('apresentadorMensagemCnab', mensagensWeb);
            dijit.byId("tagTitulosCnab").set("open", true);
            validado = false;
        }
        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarCnab() {
    try{
        var validado = true;
        var cnab = null;
        if (!validarCnab())
            return false;

        showCarregando();
        cnab = mountDataCnabForPost();
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postInsertCnab",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(cnab)
        }).then(function (data) {
            if (!hasValue(data.erro)) {
                var itemAlterado = data.retorno;
                var gridName = 'gridCnabBoleto';
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                data = data.retorno;

                if (!hasValue(grid.itensSelecionados))
                    grid.itensSelecionados = [];
                insertObjSort(grid.itensSelecionados, "cd_cnab", itemAlterado);
                buscarItensSelecionados(gridName, 'selecionadoCnab', 'cd_cnab', 'selecionaTodosCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItens');
                grid.sortInfo = -9; // era 2 Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_cnab");
                showCarregando();
                dijit.byId("cadCnab").hide();
                if (data.cd_contrato > 0)
                setTimeout(function () { postGerarCnab(dijit.byId('gridCnabBoleto').itensSelecionados) }, 100);
            } else {
                apresentaMensagem('apresentadorMensagemCnab', data);
                showCarregando();
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCnab', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarCnab() {
    try{
        var cnab = null;
        if (!validarCnab())
            return false;
        showCarregando();
        cnab = mountDataCnabForPost();
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postUpdateCnab",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(cnab)
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridCnabBoleto';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_cnab", dojo.byId("cd_cnab").value);
                    insertObjSort(grid.itensSelecionados, "cd_cnab", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoCnab', 'cd_cnab', 'selecionaTodosCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItens');
                    grid.sortInfo = -9; // era 3Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_cnab");
                    dijit.byId("cadCnab").hide();
                }
                else
                    apresentaMensagem('apresentadorMensagemCnab', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCnab', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
};

function deletarCnabs(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId("cd_cnab").value != 0)
                itensSelecionados = [{
                    cd_cnab: dojo.byId("cd_cnab").value
                }];
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postDeleteCnab",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridCnabBoleto').itensSelecionados, "cd_cnab", itensSelecionados[r].cd_cnab);
                pesquisarCnab(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarCnabBoletos").set('disabled', false);
                dijit.byId("relatorioCnab").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadCnab").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadCnab").style.display))
                apresentaMensagem('apresentadorMensagemCnab', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mountDataCnabForPost() {
    try{
        var titulosCnab = null;
        var cd_carteira = 0;
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");

        if (hasValue(gridTitulosCnab) && gridTitulosCnab.store.objectStore.data != null) {
            titulosCnab = cloneArray(gridTitulosCnab.store.objectStore.data);
            for (var i = 0; i < titulosCnab.length; i++) {
                delete titulosCnab[i].Titulo;
            }

        }
        if (dijit.byId("cadLocal").value > 0) {
            var storeCarteira = dijit.byId("cadLocal").store.data;
            quickSortObj(storeCarteira, 'id');
            var posicao = binaryObjSearch(storeCarteira, 'id', parseInt(dijit.byId("cadLocal").value));
            cd_carteira = storeCarteira[posicao].cd_carteira_cnab;
        }

        var retorno = {
            cd_cnab : hasValue(dojo.byId("cd_cnab").value) ? dojo.byId("cd_cnab").value : 0,
            cd_local_movto : dijit.byId("cadLocal").value,
            id_tipo_cnab : dijit.byId("cadTipo").get("value"),
            dt_inicial_vencimento: dojo.date.locale.parse(dojo.byId("cadDtVencIniCnab").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_final_vencimento: dojo.date.locale.parse(dojo.byId("cadDtVencFimCnab").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            id_responsavel_cnab : dijit.byId("ckResponsavel").get("checked"),
            id_status_cnab : dijit.byId("cadStatus").get("value"),
            vl_total_cnab : dojo.byId("vlTituloCnabReg").value,
            no_arquivo_remessa: dojo.byId("cadRemessa").value,
            cd_carteira_cnab: cd_carteira,
            nm_dias_protesto: hasValue(dijit.byId("nmDiasProtesto").value) ? dijit.byId("nmDiasProtesto").value : null,
            TitulosCnab: titulosCnab,
            cd_contrato: hasValue(dojo.byId("cd_contrato").value) && dojo.byId("cd_contrato").value > 0 ? dojo.byId("cd_contrato").value : null
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverCnab(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarCnabs(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function LoadCarteirasCadastro(data,value) {
    try {        
        storeData = [];
        if (hasValue(data) && data.length > 0) {
            $.each(data, function (index, value) {
                storeData.push({
                    id: value.cd_localMvto,
                    name: value.no_carteira_completa,
                    cd_carteira_cnab: value.cd_carteira_cnab,
                    nm_banco: value.localMovtoCateiraCnab.nm_banco
                });
            });
        }

        var statusStore = new dojo.store.Memory({
            data: storeData
        });
        dijit.byId("cadLocal").store = statusStore;

        if (value != null && hasValue(value)) {
            dijit.byId("cadLocal").set("value", value);
            dijit.byId("cadLocal").oldValue = value;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Metodos C.R.U.D Título Cnab
function showEditTituloCnab(cdTituloCnab, cdCnab, itemTituloCnabView) {
    try{
        showCarregando();
        dojo.xhr.get({
            url: Endereco() + "/api/cnab/getTituloCnabEdit?cd_titulo_cnab=" + cdTituloCnab + "&cd_cnab=" + cdCnab,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dados) {
            try{
                apresentaMensagem("apresentadorMensagemTituloCnab", null);
                if (dados.retorno != null && dados.retorno != null)
                    loadDataTituloCnab(dados.retorno, itemTituloCnabView);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemTituloCnab", error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTituloCnab(itemTituloCnabBase, itemTituloCnabView) {
    try{
        //Tag Titulo
        dojo.byId("responsavelTituloCnabReg").value = itemTituloCnabView.nomeResponsavel;
        dojo.byId("responsavelTituloCnabReg").value = itemTituloCnabView.nomeResponsavel;

        dojo.byId("codigoTituloCnabReg").value = itemTituloCnabView.cd_titulo;
        dojo.byId("dtaEmissaoTituloCnabReg").value = itemTituloCnabView.dt_emissao;
        dojo.byId("dtaVencTituloCnabReg").value = itemTituloCnabView.dt_vcto;
        dojo.byId("localMvtoTituloEdit").value = itemTituloCnabView.descLocalMovtoTitulo;
        dojo.byId("localMvtoTituloEdit").value = itemTituloCnabView.descLocalMovtoTitulo;
        dojo.byId("statusTituloCanbReg").value = itemTituloCnabView.Titulo.statusCnabTitulo;
        dojo.byId("nossoNroTituloCnabReg").value = itemTituloCnabView.dc_nosso_numero;
        dojo.byId("vlTituloCnabReg").value = itemTituloCnabView.Titulo.vlTitulo;
        if (itemTituloCnabView.id_alterou_txt_cnab)
            dojo.byId("descObsTituloCnabDet").value = itemTituloCnabView.tx_mensagem_cnab;
        else
            if (hasValue(itemTituloCnabBase))
                dojo.byId("descObsTituloCnabDet").value = itemTituloCnabBase.tx_mensagem_cnab;

        if (itemTituloCnabBase != null) {
            dojo.byId("pessoaTituloCnabReg").value = itemTituloCnabBase.nomePessoaTitulo;
            dojo.byId("nrParcelaTituloCnabReg").value = itemTituloCnabBase.Titulo.nm_parcela_titulo;
            dojo.byId("numTituloCnab").value = itemTituloCnabBase.Titulo.nm_titulo;
            //tag título cnab
            dojo.byId("localTituloCnabDet").value = itemTituloCnabBase.descLocalMovtoTituloCnab;
            dojo.byId("statusTituliCnabDet").value = itemTituloCnabBase.statusTituloCnab;
            dojo.byId("nossoNroTituloCnabDet").value = itemTituloCnabBase.dc_nosso_numero_titulo;
            dojo.byId("jurosTituloCnabDet").value = itemTituloCnabBase.pcJurosCalc;
            dojo.byId("edMulta").value = itemTituloCnabBase.pcMultaCalc;
            //dojo.byId("descTituloCnabDet").value = itemTituloCnabBase.vlDesconto;
            dijit.byId("dtaVenciTituloCnabDet").set("value", itemTituloCnabBase.dt_vencimento_titulo);
            dojo.byId("turmaTituloCnabDet").value = itemTituloCnabBase.no_turma_titulo;
            if (hasValue(itemTituloCnabBase.DescontoTituloCNAB) && itemTituloCnabBase.DescontoTituloCNAB.length > 0) {
                var gridDescontosTituloCNAB = dijit.byId("gridDescontosTituloCNAB");
                gridDescontosTituloCNAB.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemTituloCnabBase.DescontoTituloCNAB }) }));
            }
        } else {
            dojo.byId("jurosTituloCnabDet").value = itemTituloCnabView.pcJurosCalc;
            dojo.byId("edMulta").value = itemTituloCnabView.pcMultaCalc;
            //dojo.byId("descTituloCnabDet").value = itemTituloCnabView.vlDesconto;
            dojo.byId("descObsTituloCnabDet").value = itemTituloCnabView.tx_mensagem_cnab;
            dojo.byId("pessoaTituloCnabReg").value = itemTituloCnabView.Titulo.nomeAluno;
            //
            dojo.byId("nrParcelaTituloCnabReg").value = itemTituloCnabView.Titulo.nm_parcela_titulo;
            dojo.byId("numTituloCnab").value = itemTituloCnabView.Titulo.nm_titulo;
            dojo.byId("turmaTituloCnabDet").value = itemTituloCnabView.no_turma_titulo;
            dojo.byId("dtaVenciTituloCnabDet").value = itemTituloCnabView.dt_vcto;
            dojo.byId("nossoNroTituloCnabDet").value = itemTituloCnabView.dc_nosso_numero_titulo;
            if (hasValue(itemTituloCnabView.DescontoTituloCNAB) && itemTituloCnabView.DescontoTituloCNAB.length > 0) {
                var gridDescontosTituloCNAB = dijit.byId("gridDescontosTituloCNAB");
                gridDescontosTituloCNAB.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemTituloCnabView.DescontoTituloCNAB }) }));
            }
        }
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTituloCnab() {
    try{
        clearForm("formTituloCnab");
        apresentaMensagem("apresentadorMensagemTituloCnab", null);
        dijit.byId("tagDescontosTituloCnab").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarTituloCnab() {
    try {

        var oldValue = null;
        var value = null;
        var grid = dijit.byId("gridTitulosCnab");
        var ehLink = false;
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('linkTitulosCnab');
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        apresentaMensagem('apresentadorMensagemTituloCnab', '');
        if (value.cd_titulo_cnab > 0) {
            value.tx_mensagem_cnab = dojo.byId("descObsTituloCnabDet").value;
            value.id_alterou_txt_cnab = true;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegAltSucess);
            apresentaMensagem("apresentadorMensagemCnab", mensagensWeb);
            dijit.byId("cadTituloCnab").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesTituloCnab(value, grid, ehLink) {
    try{
        limparTituloCnab();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('linkTitulosCnab');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        //if (value.cd_cnab > 0)
        //    showEditCnab(value.cd_cnab);

        if (value.cd_titulo_cnab > 0) {
            showEditTituloCnab(value.cd_titulo_cnab, dojo.byId("cd_cnab").value, value);
        } else
            loadDataTituloCnab(null, value);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDadosContrato(cd_contrato, permissoes, tipo) {
    dijit.byId('ckCnab').set('disabled', true);
    dijit.byId('ckBoleto').set('disabled', true);
    dijit.byId('ckBoleto').set('checked', false);
    dijit.byId('ckCnab').set('checked', false);
    if (tipo == 1) {
        dojo.byId("laNumContratoPesq").style.display = '';
        dojo.byId("pesNmContrato").style.display = '';
        dojo.byId("laNumContrato").style.display = '';
        dojo.byId("numContratoMatricula").style.display = '';
        dijit.byId('ckBoleto').set('checked', true);
    }
    else {
        dojo.byId("laNumContratoPesq").style.display = "none";
        dojo.byId("pesNmContrato").style.display = "none";
        dojo.byId("laNumContrato").style.display = "none";
        dojo.byId("numContratoMatricula").style.display = "none";
        dijit.byId('ckCnab').set('checked', true);
    };
    dojo.ready(function () {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getMatriculaCnabBoleto?cd_contrato=" + cd_contrato + "&tipo=" + tipo,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                if (data.retorno != null && data.retorno.cd_contrato > 0) {
                    showCarregando();
                    limparCnab();
                    dijit.byId("cadTipo").set("disabled", false);
                    IncluirAlterar(1, 'divAlterarCnab', 'divIncluirCnab', 'divExcluirCnab', 'apresentadorMensagemCnab', 'divCancelarCnab', 'divClearCnab');
                    findIsLoadComponetesNovoCnab(data.retorno.cd_local_movto);
                    var dataHoje = new Date()
					dijit.byId("cadDtVencFimCnab").set("value", data.retorno.dt_final_contrato);
                    dijit.byId("cadDtVencIniCnab").set("value", dataHoje > dojo.date.locale.parse(dataAtualFormatada(data.retorno.dt_inicial_contrato), { formatLength: 'short', selector: 'date', locale: 'pt-br' }) ? dataHoje : data.retorno.dt_inicial_contrato);
                    dijit.byId("nroContratoCnab").set("value", data.retorno.nm_contrato);
                    if (tipo == 1) {
                        dijit.byId("numContratoMatricula").set("value", data.retorno.nm_contrato);
                        dojo.byId("cd_contrato").value = cd_contrato;
                    }
                    else {
                        dijit.byId("numContratoMatricula").set("value", null);
                        dojo.byId("cd_contrato").value = 0;
                    }
                    dijit.byId("cadCnab").show();
                    if (tipo == 1) {
                        dijit.byId("cadRemessa").set("value", "CTR " + data.retorno.nm_contrato);
                    }
                    simularBaixaCnabTitulosMatricula(permissoes, data.retorno.titulos);
                }

            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemCnab', error.response.data);
        });
    });
}

function imprimirTitulos(itensSelecionados) {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegProcessamento, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneProcessar, null);
        else {
            var mensagensWeb = new Array();

            if (mensagensWeb.length > 0)
                apresentaMensagem('apresentadorMensagem', mensagensWeb);

            dojo.xhr.get({
                url: Endereco() + "/api/cnab/getUrlRelatorioTitulosCNAB?cd_cnab=" + itensSelecionados[0].cd_cnab,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    abrePopUp(Endereco() + '/Relatorio/RelatorioTitulosCNAB?' + data, '765px', '771px', 'popRelatorio');
                }
                catch (e) {
                    postGerarLog(e);
                }
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

function eventoExcluirCNABRegistrado(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelecioneRegExcluir, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelecioneApenasUmRegistro, null);
        else {
            dojo.xhr.post({
                url: Endereco() + "/api/cnab/postExcluirCNABRegistrado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify({ cd_cnab: itensSelecionados[0].cd_cnab, cd_contrato: itensSelecionados[0].cd_contrato })
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItens_label");
                    apresentaMensagem('apresentadorMensagem', data);
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridCnabBoleto').itensSelecionados, "cd_cnab", itensSelecionados[r].cd_cnab);
                    pesquisarCnab(false);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCnabBoletos").set('disabled', false);
                    dijit.byId("relatorioCnab").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    dijit.byId("cadCnab").hide();
                }
                catch (e) {
                    postGerarLog(e);
                }
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

function enviarBoletos(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegEnviarBoletoEMail, null);
            showCarregando();
        }
        else if (itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegEnvioBoleto, null);
            showCarregando();
        }
        else {
            var gridCnabBoleto = dijit.byId('gridCnabBoleto');
            var cd_cnab = '';
            var qtdCnabs = 0;
            var mensagensWeb = new Array();
            for (var i = 0, j = 0; i < itensSelecionados.length; i++) {
                if (itensSelecionados[i].id_impressao_carteira_cnab == IMPRESSAO_BANCO) {
                    mensagensWeb[j] = new MensagensWeb(MENSAGEM_AVISO, montarMensagemInfoCanb(itensSelecionados[i], msgInfoEnvioBoletoSomenteBanco));
                    j += 1;
                } else {
                    cd_cnab += itensSelecionados[i].cd_cnab + ';';
                    qtdCnabs++;
                }
            }
            if (mensagensWeb.length > 0)
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
            if (qtdCnabs > 0) {
                dojo.xhr.get({
                    url: Endereco() + "/escola/getBoletoEmail?cd_cnab=" + cd_cnab.substring(0, cd_cnab.length - 1),
                    sync: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        dojo.xhr.get({
                            url: data.retorno,
                            handleAs: "json",
                            headers: { "X-Requested-With": null }
                        }).then(function (data) {
                            try {
                                apresentaMensagem('apresentadorMensagem', data);
                                showCarregando();
                            } catch (e) {
                                showCarregando();
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            //Esta requisição somente irá funcionar no IIS
                            showCarregando();
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                    showCarregando();
                });
            } else
                showCarregando();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function simularBaixaCnabTitulosMatricula(permissoes, titulosMatricula) {
    try {        
        var gridTitulosCnab = dijit.byId("gridTitulosCnab");       
        var dataHoje = new Date();
        if (dijit.byId("cadCnab").open) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemCnab', null);
            quickSortObj(titulosMatricula, 'cd_titulo');
            var tituloCnab = new Array();
            var j = 0;
            for (var i = 0; i < titulosMatricula.length; i++) {
                var value = titulosMatricula[i];
                if (dataHoje <= dojo.date.locale.parse(value.dt_vcto, { formatLength: 'short', selector: 'date', locale: 'pt-br' })) {
                    tituloCnab[j] = {
                        cd_titulo_cnab: 0,
                        cd_titulo: value.cd_titulo,
                        nomePessoaTitulo: value.nomeCliente,
                        nomeResponsavel: value.nomeResponsavel,
                        dt_emissao: value.dt_emissao,
                        dt_vcto: value.dt_vcto,
                        Titulo: {
                            vl_titulo: value.vl_titulo,
                            vl_saldo_titulo: value.vl_saldo_titulo,
                            cd_titulo: value.cd_titulo,
                            nm_titulo: value.nm_titulo,
                            nm_parcela_titulo: value.nm_parcela_titulo,
                            id_natureza_titulo: value.id_natureza_titulo,
                            dt_vcto_titulo: dojo.date.locale.parse(value.dt_vcto, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            dc_tipo_titulo: value.dc_tipo_titulo
                        },
                        vlTitulo: value.vlTitulo,
                        vlSaldoTitulo: value.vlSaldoTitulo,
                        nm_parcela_e_titulo: value.nm_parcela_e_titulo,
                        id_status_cnab_titulo: value.id_status_titulo,
                        descLocalMovtoTitulo: value.descLocalMovto,
                        dt_vencimento_titulo: dojo.date.locale.parse(value.dt_vcto, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                        cd_local_movto_titulo: value.cd_local_movto
                    }
                    j++
                };
            }

            var nomeRequisicao = '/api/escola/postSimularBaixaCnabOrDadosAdicTitulos';
            if (permissoes != null && permissoes != "" && possuiPermissao('ctsg', permissoes))
                nomeRequisicao = '/api/escola/postSimularBaixaCnabOrDadosAdicTitulosGeral';
            dojo.xhr.post({
                url: Endereco() + nomeRequisicao,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson({ titulos: tituloCnab, dataBaixa: "", id_tipo_cnab: parseInt(dijit.byId("cadTipo").get("value")) })
            }).then(function (titulosCnab) {
                try {
                    //quickSortObj(gridTitulosCnab.store.objectStore.data, 'cd_titulo');
                    //quickSortObj(gridTitulosCnab.listaTitulosCnab, 'cd_titulo');
                    $.each(titulosCnab.retorno, function (idx, value) {
                        gridTitulosCnab.store.objectStore.data.push(value);
                        gridTitulosCnab.listaTitulosCnab.push(value);

                        //insertObjSort(gridTitulosCnab.store.objectStore.data, "cd_titulo", value);
                        //insertObjSort(gridTitulosCnab.listaTitulosCnab, "cd_titulo", value);
                    });
                    gridTitulosCnab.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridTitulosCnab.store.objectStore.data }) }));
                    showCarregando();
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCnab', error);
            });
        }
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function limparTitulosCnab() {

    if (!hasValue(dojo.byId("cd_contrato").value)) {
        dijit.byId("gridTitulosCnab").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        dijit.byId("gridTitulosCnab").listaTitulosCnab = [];
    }
}