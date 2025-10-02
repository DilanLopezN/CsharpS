VENDASSERVICOORIGEMBAIXA = 4, ORIGEMBAIXANF = 47;
var CHEQUEPREDATADO = 4, CHEQUEVISTA = 10;
var CARTAOCREDITO = 2, CARTAODEBITO = 3;
var CHEQUE = 4, CARTAO = 5;
var TROCA_FINANCEIRA = 110;

function montarMetodosBaixaFinanceira(permissoes) {
    //Criação da Grade de sala
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojox/json/ref",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/dom",
    "dojo/data/ItemFileReadStore",
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom, ItemFileReadStore) {
        ready(function () {
            try {
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                dojo.byId("footer").style.height = '0px';
                loadSituacaoAluno(ItemFileReadStore);
                decreaseBtn(document.getElementById("limparTurmaPesqBaixaFinan"), '40px');
                var compFkTurmaPesqBaixaFinan = document.getElementById('fkTurmaPesqBaixaFinan');
                if (hasValue(compFkTurmaPesqBaixaFinan)) {
                    compFkTurmaPesqBaixaFinan.parentNode.style.minWidth = '18px';
                    compFkTurmaPesqBaixaFinan.parentNode.style.width = '18px';
                }

                var parametros = getParamterosURL();
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
	                
                    if (hasValue(parametros['cd_aluno']) && hasValue(parametros['cd_pessoa']) && hasValue(parametros['no_pessoa'])) {
                        $("#cdPessoaResp").val(parametros['cd_pessoa']);
                        $("#pessoaBaixaFinan").val(decodeURIComponent(parametros['no_pessoa']));
                        var cdResponsavel = hasValue(dojo.byId("cdPessoaResp").value) ? dojo.byId("cdPessoaResp").value : 0;
                       
                        var myStore =
                            Cache(
                                JsonRest({
                                    target: Endereco() + "/api/financeiro/getTituloSearchGeral?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=false&inicio=false&locMov=" + parseInt(0) +
                                        "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=" +
                                        "&emissao=true&baixa=false&vencimento=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=" + "&cnabStatus=" + parseInt(-1) +
                                        "&nro_recibo=&cd_turma=&cd_situacoes_aluno=&cd_tipo_financeiro=",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Authorization": Token() }
                                }), Memory({}));
                    } else {
	                    var myStore =
		                    Cache(
			                    JsonRest({
				                    target: Endereco() + "/api/financeiro/getTituloSearchGeral?cd_pessoa=" + parseInt(0) + "&responsavel=false&inicio=false&locMov=" + parseInt(0) +
					                    "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=" +
					                    "&emissao=true&baixa=false&vencimento=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=" + "&cnabStatus=" + parseInt(-1) +
					                    "&nro_recibo=&cd_turma=&cd_situacoes_aluno=&cd_tipo_financeiro=",
				                    handleAs: "json",
				                    preventCache: true,
				                    headers: { "Accept": "application/json", "Authorization": Token() }
			                    }), Memory({}));}
                    
                }
                else {
                    if (hasValue(parametros['cd_aluno']) && hasValue(parametros['cd_pessoa']) && hasValue(parametros['no_pessoa'])) {
                        $("#cdPessoaResp").val(parametros['cd_pessoa']);
                        $("#pessoaBaixaFinan").val(decodeURIComponent(parametros['no_pessoa']));
                        var cdResponsavel = hasValue(dojo.byId("cdPessoaResp").value) ? dojo.byId("cdPessoaResp").value : 0;

                        var myStore =
	                        Cache(
		                        JsonRest({
                                    target: Endereco() + "/api/financeiro/getTituloSearch?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=false&inicio=false&locMov=" + parseInt(0) +
				                        "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=" +
				                        "&emissao=true&baixa=false&vencimento=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=" + "&cnabStatus=" + parseInt(-1) +
				                        "&nro_recibo=&cd_turma=&cd_situacoes_aluno=&cd_tipo_financeiro=",
			                        handleAs: "json",
			                        preventCache: true,
			                        headers: { "Accept": "application/json", "Authorization": Token() }
		                        }), Memory({}));
                    } else {
	                    var myStore =
		                    Cache(
			                    JsonRest({
				                    target: Endereco() + "/api/financeiro/getTituloSearch?cd_pessoa=" + parseInt(0) + "&responsavel=false&inicio=false&locMov=" + parseInt(0) +
					                    "&natureza=" + parseInt(1) + "&status=" + parseInt(1) + "&numeroTitulo=&parcelaTitulo=&valorTitulo=&dtInicial=&dtFinal=" +
					                    "&emissao=true&baixa=false&vencimento=false&locMovBaixa=0&cdTipoLiquidacao=0&tipoTitulo=0&nossoNumero=" + "&cnabStatus=" + parseInt(-1) +
					                    "&nro_recibo=&cd_turma=&cd_situacoes_aluno=&cd_tipo_financeiro=",
				                    handleAs: "json",
				                    preventCache: true,
				                    headers: { "Accept": "application/json", "Authorization": Token() }
			                    }), Memory({}));}
                   
                }

                var gridTitulo = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore:  new dojo.store.Memory({ data: null }) }),
                    structure: [
                        {
                            name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;",
                            formatter: formatCheckBoxTitulo
                        },
                        { name: "Número", field: "nm_titulo", width: "8%", styles: "min-width:80px;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:80px; text-align: center;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "4%", styles: "text-align: center;" },
                        { name: "Tipo Doc.", field: "tipoDoc", width: "8%", styles: "text-align: center;" },
                        { name: "Emissão", field: "dt_emissao", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Valor", field: "vlTitulo", width: "7%", styles: "min-width:80px;text-align: right;" },
                        { name: "Saldo", field: "vlSaldoCorrigido", width: "7%", styles: "text-align: right;" },
                        { name: "Pessoa", field: "nomeResponsavel", width: "25%", styles: "text-align: left;", id: "idColPessoa", formatter: configLayoutPessoaBaixaFina },
                        { name: "Status", field: "statusTitulo", width: "7%", styles: "text-align: center;" },
                        { name: "Natureza", field: "natureza", width: "7%", styles: "text-align: center;" }
                    ],
                    noDataMessage: msgNotRegEncFiltro,
                    canSort: true,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["16", "32", "64", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "16",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTitulo");
                gridTitulo.startup();
                gridTitulo.itenSelecionado = null;
                gridTitulo.pagination.plugin._paginator.plugin.connect(gridTitulo.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try{
                        verificaMostrarTodos(evt, gridTitulo, 'cd_titulo', 'selecionaTodos');
                    }catch (e) {
                        postGerarLog(e);
                    }
                });

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTitulo, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridTitulo')", gridTitulo.rowsPerPage * 3);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridTitulo.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 && Math.abs(col) != 10; };

                gridTitulo.on("RowClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        if (hasValue(item)) {
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
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
                if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                    var ieversion = new Number(RegExp.$1)
                    if (ieversion == 7)
                        // Para IE7
                        dojo.byId('cadTitulo').childNodes[1].style.height = '100%';
                }
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
                gridBaixa.startup();

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridBaixa, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_baixa_titulo', 'selecionadoBaixa', -1, 'selecionaTodos', 'selecionaTodosBaixa', 'gridBaixa')", gridBaixa.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });

                gridBaixa.canSort = function (col) { return Math.abs(col) != 1; };

                //gridBaixa.on("RowDblClick", function (evt) {
                //    var idx = evt.rowIndex,
                //        item = this.getItem(idx);
                //    var gridBaixa = dijit.byId('gridBaixa');

                //    gridBaixa.itemSelecionado = item;
                //    mostrarCadastroBaixaFinanceira(false, null, gridBaixa, xhr, ref, on);
                //}, true);
                dijit.byId('tgBaixaPesq').set('open', false);

                var statusStore = new Memory({
                    data: [
                      { name: "Todos", id: "0" },
                      { name: "Abertos", id: "1" },
                      { name: "Fechados", id: "2" }
                    ]
                });
                var statusPesq = new FilteringSelect({
                    id: "statusPesq",
                    name: "statusPesq",
                    value: 1,
                    store: statusStore,
                    searchAttr: "name",
                    style: "max-width:300px;width: 100%;"
                }, "statusPesq");

                dijit.byId("statusPesq").on("change", function (e) {
                    try {
                        if (hasValue(e) && parseInt(e) == 2) {
                            if (dijit.byId('tgBaixaPesq').open == false)
                                dijit.byId('tgBaixaPesq').set('open', true);
                            dojo.byId("tdLblRecibo").style.display = "";
                            dojo.byId("tdRecibo").style.display = "";
                        } else {
                            dijit.byId("nroRecibo").reset();
                            dojo.byId("tdLblRecibo").style.display = "none";
                            dojo.byId("tdRecibo").style.display = "none";
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                var naturezaPesq = new Memory({
                    data: [
                      { name: "Todas", id: "0" },
                      { name: "Receber", id: "1" },
                      { name: "Pagar", id: "2" }
                    ]
                });

                var naturezaPesq = new FilteringSelect({
                    id: "naturezaPesq",
                    name: "naturezaPesq",
                    store: naturezaPesq,
                    searchAttr: "name",
                    style: "max-width:300px;width: 100%;"
                }, "naturezaPesq");

                // Botões C.R.U.D título.

                var storeNaturezaCadTitulo = new Memory({
                    data: [
                      { name: "Receber", id: "1" },
                      { name: "Pagar", id: "2" }
                    ]
                });

                dijit.byId("cbNaturezaTitulo").store = storeNaturezaCadTitulo;

                var statusTitulo = new Memory({
                    data: [
                      { name: "Aberto", id: "1" },
                      { name: "Fechado", id: "2" }
                    ]
                });
                dijit.byId("cbStatusTitulo").store = statusTitulo;

                var statusStoreTipoTitulo = new Memory({
                    data: [
                    { name: "Todos", id: 0 },
                    { name: "PP", id: 1 },
                    { name: "TM", id: 2 },
                    { name: "TA", id: 3 },
                    { name: "ME", id: 4 },
                    { name: "MA", id: 5 },
                    { name: "AD", id: 6 },
                    { name: "AA", id: 7 },
                    { name: "MM", id: 8 },
                    { name: "NF", id: 9 }
                    ]
                });
                dijit.byId("pesqTipoTitulo").store = statusStoreTipoTitulo;
                dijit.byId("pesqTipoTitulo").set("value", 0);

                var cnabStatusStore = new Memory({
                    data: [
                    { name: "Todos", id: -1 },
                    { name: "Inicial", id: 0 },
                    { name: "Envio/Gerado", id: 1 },
                    { name: "Baixa Manual", id: 2 },
                    { name: "Confirmado Envio", id: 3 },
                    { name: "Baixa Manual Confirmado", id: 4 },
                    { name: "Pedido Baixa", id: 5 },
                    { name: "Confirmado Pedido Baixa", id: 6 }
                    ]
                });
                dijit.byId("pesqCnabStatus").store = cnabStatusStore;
                dijit.byId("cnabStatus").store = cnabStatusStore;
                dijit.byId("pesqCnabStatus").set("value", -1);

                montarTipoDocumento(xhr);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    }
                }, "pesCadPessoaTitulo");
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); }, 'apresentadorMensagemTitulo', BAIXA_FINANCEIRA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, BAIXA_FINANCEIRA);
                                clearForm("formPlanoContasFK");
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoTitulo");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            limparCadTitulos();
                            setarTabCadTitulo();
                            showEditTitulo(gridTitulo.itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarTitulo");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTitulo").hide(); } }, "fecharTitulo");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        updateTitulo(xhr);
                    }
                }, "alterarTitulo");

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarTitulo(true);
                    }
                }, "pesquisaTitulos");
                decreaseBtn(document.getElementById("pesquisaTitulos"), '32px');

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdPessoaResp').value = 0;
                            dojo.byId("pessoaBaixaFinan").value = "";
                            dijit.byId('limparPessoaRespo').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaRespo");
                if (hasValue(document.getElementById("limparPessoaRespo"))) {
                    document.getElementById("limparPessoaRespo").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaRespo").parentNode.style.width = '40px';
                }


                //Metodo para a criação do dropDown de link
                //					if (dojo.byId('selecionaTodos').type == 'text')
                //						setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridDiario')", gridDiario.rowsPerPage * 3);

                // Ações Relacionadas:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarTitulo(gridTitulo.itensSelecionados, xhr, ready, Memory, FilteringSelect); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Baixar Título(s)",
                    onClick: function () {
                        try {
                            limparGridBaixas();
                            var gridTitulo = dijit.byId('gridTitulo');
                            if (hasValue(gridTitulo.itensSelecionados))
                                gridTitulo.itemSelecionado = gridTitulo.itensSelecionados[0];

                            validaTitulosTipoFinanceiroCartao(gridTitulo.itensSelecionados, gridTitulo, xhr, ref, on);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoRemover);
               
                var acaoReciboAgrupado = new MenuItem({
                    label: "Recibo Agrupado",
                    onClick: function () {
                        var gridTitulo = dijit.byId('gridTitulo');
                        if (!hasValue(gridTitulo.itensSelecionados) || gridTitulo.itensSelecionados.length <= 0)
                            caixaDialogo(DIALOGO_AVISO, msgErroNenhumTituloSelecionadoRecipoAgrupado, null);
                        else if (gridTitulo.itensSelecionados.length == 1)
                            caixaDialogo(DIALOGO_ERRO, msgErroApenasUmTituloSelecionadoRecipoAgrupado, null);
                        else if (gridTitulo.itensSelecionados.length > 12){
                            caixaDialogo(DIALOGO_ERRO, msgErroNumeroMaximoTituloReciboAgrupado, null);
                        }
                        else if (validaTitulosAReceber(gridTitulo.itensSelecionados) === false) {
                            caixaDialogo(DIALOGO_ERRO, msgErroTituloSemNaturezaReceberSelecionado, null);
                        }
                        else if (validaTitulosFechados(gridTitulo.itensSelecionados) === false) {
                            caixaDialogo(DIALOGO_ERRO, msgErroTituloAbertoSelecionado, null);
                        }
                        else {
                            apresentaMensagem('apresentadorMensagem', null);
                            var idsTitulosSelecionados = getIdsTitulosSelecionados(gridTitulo.itensSelecionados);
                            xhr.get({
                                url: Endereco() + "/api/financeiro/getUrlRelatorioReciboAgrupado?cds_titulos_selecionados=" + idsTitulosSelecionados,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioReciboAgrupado?' + data, '765px', '771px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                        }
                    }
                });

                menu.addChild(acaoReciboAgrupado);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Itens selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridTitulo, 'todosItens', ['pesquisaTitulos']); pesquisarTitulo(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridTitulo', 'selecionado', 'cd_titulo', 'selecionaTodos', ['pesquisaTitulos'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

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
                            xhr.get({
                                url: Endereco() + "/api/financeiro/getUrlRelatorioRecibo?cd_baixa_titulo=" + cdBaixaTitulo + "&id_origem_titulo=" + document.getElementById("setValueOrigemTitulo").value,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioRecibo?' + data, '765px', '771px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        }
                    }
                });

                var acaoHist = new MenuItem({
                    label: "NF de Serviço",
                    onClick: function () {
                        eventoNFServicoBaixa(gridBaixa.itensSelecionados);
                    }
                });
                menu.addChild(acaoHist);

                menu.addChild(acaoRecibo);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadasBaixa"
                });
                dom.byId("linkAcoesBaixa").appendChild(button.domNode);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        pesquisaPessoaFKTitulo();
                                    });
                                });
                            else
                                abrirPessoaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProPessoaFKPesqTitulo");

                var buttonFkArray = ['pesCadPessoaTitulo', 'pesPlanoTitulo', 'pesProPessoaFKPesqTitulo'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                dijit.byId("naturezaPesq").on("change", function (e) {
                    try {
                        if (!isNaN(e) && !isNaN(dijit.byId("statusPesq").value))
                            loadDataLocalMvto(xhr, ready, Memory, FilteringSelect, e);
                        if (hasValue(e)) {
                            configLayoutPessoaBaixaFina();
                        } else
                            dijit.byId("naturezaPesq").set("value", RECEBER);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("naturezaPesq").set("value", RECEBER);

                dijit.byId("ckVcto").set("checked", true);

                dijit.byId("ckEmissao").on("change", function (e) {
                    try {
                        if (!e && !dijit.byId("ckBaixa").checked && !dijit.byId("ckVcto").checked)
                            dijit.byId("ckVcto").set("checked", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("ckVcto").on("change", function (e) {
                    try {
                        if (!e && !dijit.byId("ckBaixa").checked && !dijit.byId("ckEmissao").checked)
                            dijit.byId("ckVcto").set("checked", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("ckBaixa").on("change", function (e) {
                    try {
                        if (!e && !dijit.byId("ckVcto").checked && !dijit.byId("ckEmissao").checked)
                            dijit.byId("ckVcto").set("checked", true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("tgBaixaPesq").on("show", function (e) {
                    try {
                        gridBaixa.update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("tgTitulo").on("show", function (e) {
                    try {
                        gridTitulo.update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
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

                dijit.byId("tipoLiquidacaoPesq").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < 0)
                            dijit.byId("tipoLiquidacaoPesq").set("value", 0);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesqTipoTitulo").on("change", function (event) {
                    try {
                        if (!hasValue(event) || event < 0)
                            dijit.byId("pesqTipoTitulo").set("value", 0);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesqCnabStatus").on("change", function (event) {
                    try {
                        if ((!hasValue(event) && !(event == 0)) || event <= -1)
                            dijit.byId("pesqCnabStatus").set("value", -1);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("ckResponsavelPesq").on("click", function (e) {
                    try {
                        if (dijit.byId("ckResponsavelPesq").checked) {
                            dojo.byId("lblPessoaBaixaFinan").innerHTML = "Responsável:";
                            dojo.byId("idColPessoa").childNodes[0].innerHTML = "Responsável";
                        }
                        else
                            configLayoutPessoaBaixaFina();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                setMenssageMultiSelect(SITUACAO, 'situacaoAlunoTurma');

                dijit.byId("fkTurmaPesqBaixaFinan").on("click", function (event) {
                    try {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFK();
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
                            abrirTurmaFK();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("limparTurmaPesqBaixaFinan").on("click", function (event) {
                    try {
                        dojo.byId("cd_turmafk_baixa_finan").value = 0;
                        dojo.byId("no_turmaFk_baixa_finan").value = "";
                        dijit.byId("limparTurmaPesqBaixaFinan").set('disabled', true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("limparTurmaPesqBaixaFinan").set("disabled", true);

                //dijit.byId("situacaoAlunoTurma").on("change", function (e) {
                //    setMenssageMultiSelect(SITUACAO, 'situacaoAlunoTurma');
                //});
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323061', '765px', '771px');
                        });
                }

                adicionarAtalhoPesquisa(['statusPesq', 'localMovtoPesq', 'localMovtoBaixaPesq', 'naturezaPesq', 'numeroTitulo', 'parcelaTitulo', 'valorPesq', 'pesNossoNumero', 'dtInicial', 'dtFinal',
                'ckResponsavelPesq', 'inicioPessoaRespPesq'], 'pesquisaTitulos', ready);

                FindIsLoadComponetesPesqBaixaFinan();
                //dijit.byId("gridTitulo").on("show", function (e) {
                //    alert("show");
                //});

                dijit.byId("nm_dias_cartao").on("change", function (dias) {
                    try {
                        if (hasValue(dias)) {
                            var dataEmissao = dijit.byId("dtaEmissaoTitulo").get("value");
                            var novaDataEmissaoTitulo = addDiasDataEmissaoTitulo(dataEmissao, dias);
                            dijit.byId("dtaEmissaoTitulo").set("value", dataEmissao);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pc_taxa_cartao").on('change', function (e) {

	                if (hasValue(e)) {
		                //pc_taxa_cartao = vl_taxa_cartao / vl_titulo/ 100.0
		                var pc_taxa_cartao = dijit.byId("pc_taxa_cartao").value;
                        var vl_titulo = dijit.byId("cadValorTitulo").value;
                        var vl_taxa_cartao = pc_taxa_cartao * (vl_titulo / 100);
                        var vl_taxa_cartao_mask = maskFixed(vl_taxa_cartao, 2);
                        dijit.byId("vl_taxa_cartao")._onChangeActive = false;
                        //dojo.byId("vl_taxa_cartao").value = maskFixed(vl_taxa_cartao, 2);
                        dijit.byId("vl_taxa_cartao").set("value", unmaskFixed(vl_taxa_cartao_mask, 2));
                        dijit.byId("vl_taxa_cartao")._onChangeActive = true;
	                }

                });

                dijit.byId("vl_taxa_cartao").on('change', function (e) {
	                if (hasValue(e)) {
		                //pc_taxa_cartao = vl_taxa_cartao / vl_titulo/ 100.0
		                var vl_taxa_cartao = parseFloat((dojo.byId("vl_taxa_cartao").value).replace(",", "."));
                        var vl_titulo = dijit.byId("cadValorTitulo").value;
		                var pc_taxa_cartao = vl_taxa_cartao / (vl_titulo / 100);
		                console.log(pc_taxa_cartao);
                        var pc_taxa_cartao_mask = maskFixed(pc_taxa_cartao, 2);
                        dijit.byId("pc_taxa_cartao")._onChangeActive = false;
                        dijit.byId("pc_taxa_cartao").set("value", unmaskFixed(pc_taxa_cartao_mask, 2));
                        dijit.byId("pc_taxa_cartao")._onChangeActive = true;
	                }
                });

                dijit.byId("cbLocalMovtoTitulo").on("change", function (e) {
                    if (!dijit.byId("cbLocalMovtoTitulo").focused) return;
                    try {
                        if ((hasValue(dijit.byId("cbTipoFinan").item) &&
                            (dijit.byId("cbTipoFinan").item.id == CARTAO)) &&
                            (hasValue(dijit.byId("cbLocalMovtoTitulo").item) &&
                                (dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local != LOCALCARTAODEBITO && dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local != LOCALCARTAOCREDITO))) {
                            dijit.byId("cbLocalMovtoTitulo").set("value", "");
                            apresentaMensagem('apresentadorMensagemTitulo', null);
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                "Para títulos com tipo financeiro “cartão” somente Locais de Movimento tipo “cartão de débito/crédito poderão ser selecionados, caso contrário estes tipos não poderão.");
                            apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                        } else

                            if ((hasValue(dijit.byId("cbTipoFinan").item) &&
                                (dijit.byId("cbTipoFinan").item.id != CARTAO)) &&
                                (hasValue(dijit.byId("cbLocalMovtoTitulo").item) &&
                                    (dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local == LOCALCARTAODEBITO || dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local == LOCALCARTAOCREDITO))) {
                                dijit.byId("cbLocalMovtoTitulo").set("value", "");

                                apresentaMensagem('apresentadorMensagemTitulo', null);
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                    "Para títulos com tipo financeiro diferente de “cartão”, os Locais de Movimento do tipo “cartão de débito/crédito não poderão ser selecionados.");
                                apresentaMensagem('apresentadorMensagemTitulo', mensagensWeb);
                            } else {
                                apresentaMensagem('apresentadorMensagemTitulo', null);
                            }
                        if ((hasValue(dijit.byId("cbTipoFinan").item) &&
                            (dijit.byId("cbTipoFinan").item.id == CARTAO)) &&
                            (hasValue(dijit.byId("cbLocalMovtoTitulo").item) && (dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local == LOCALCARTAODEBITO || dijit.byId("cbLocalMovtoTitulo").item.nm_tipo_local == LOCALCARTAOCREDITO))) {
                            aplicarTaxaBancaria(dijit.byId('gridTitulo').itensSelecionados[0].cd_titulo, e);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("cbTipoFinan").on("change", function (e) {
                    try {
                        if (!dijit.byId("cbTipoFinan").focused) return;
                        var cd_tipo_finan = hasValue(dijit.byId("cbTipoFinan").item) ? dijit.byId("cbTipoFinan").item.id : 0;
                        if (hasValue(dijit.byId("cbTipoFinan").item) &&
                            (dijit.byId("cbTipoFinan").item.id == CARTAO)) {
                            dojo.byId('tgCartao').style.display = "block";
                            cd_tipo_finan = -1 * cd_tipo_finan; //Passando negativo para mostrar apenas contas filhas
                        } else {
                            dojo.byId('tgCartao').style.display = "none";
                            dijit.byId("nm_dias_cartao")._onChangeActive = false;
                            dijit.byId("vl_taxa_cartao")._onChangeActive = false;
                            dijit.byId("pc_taxa_cartao")._onChangeActive = false;

                            dijit.byId("pc_taxa_cartao").set("value", 0);
                            dijit.byId("nm_dias_cartao").set("value", 0);
                            dojo.byId('vl_taxa_cartao').value = 0;

                            dijit.byId("vl_taxa_cartao")._onChangeActive = true;
                            dijit.byId("nm_dias_cartao")._onChangeActive = true;
                            dijit.byId("pc_taxa_cartao")._onChangeActive = true;
                        }
                        dijit.byId("cbLocalMovtoTitulo").set("value", "");
                        apresentaMensagem('apresentadorMensagemTitulo', null);
                        getLocalTipoDocumento(cd_tipo_finan);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function validaTitulosAReceber(itensSelecionados) {
    if (hasValue(itensSelecionados)) {

        //every -> todos os itens do array tem que satisfazer a condição
        return itensSelecionados.every(function(item) {
            return ((item.natureza === "Receber"));
        });

    }
}

function validaTitulosFechados(itensSelecionados) {
    if (hasValue(itensSelecionados)) {

        //every -> todos os itens do array tem que satisfazer a condição
        return itensSelecionados.every(function(item) {
            return ((item.statusTitulo === "Fechado"));
        });

    }
}

function getIdsTitulosSelecionados(itensSelecionados) {
    if (hasValue(itensSelecionados)) {

        //map -> retorna um novo array com as propriedades especificadas
        return itensSelecionados.map(function(item, index, array) {return JSON.stringify(item.cd_titulo)}).join().replaceAll(",", "|");

    } else {
        return "";
    }
}

function retornarTurmaFKBaixaFinan() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == BAIXAFINANCEIRAPESQ) {
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
                dojo.byId("cd_turmafk_baixa_finan").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("no_turmaFk_baixa_finan").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurmaPesqBaixaFinan').set("disabled", false);
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

function abrirTurmaFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = BAIXAFINANCEIRAPESQ;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK();
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoAluno(ItemFileReadStore) {
    var w = dijit.registry.byId('situacaoAlunoTurma');
    var dados = [
                    { name: "Matriculado", id: "1" },
                    { name: "Rematriculado", id: "8" },
                    { name: "Desistente", id: "2" },
                    { name: "Encerrado", id: "4" },
                    { name: "Movido", id: "0" }
    ]
    var storeTipoAluno = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    w.setStore(storeTipoAluno, []);
}

function FindIsLoadComponetesPesqBaixaFinan() {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/componentesPesquisaBaixaFinan",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.tiposLiquidacao))
                    criarOuCarregarCompFiltering("tipoLiquidacaoPesq", data.retorno.tiposLiquidacao, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_liquidacao', 'dc_tipo_liquidacao', MASCULINO);
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

function setarTagTitulo(value) {
    //Criação da Grade de sala
    require([
    "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                if (value == true) {
                    dojo.byId('tgBaixaPesq').style.height = '200px';
                    dojo.byId("gridTitulo").style.height = '280px';
                    dijit.byId("gridTitulo").set("height", '280px');
                    dijit.byId("gridTitulo").attr("height", '280px');
                    //dijit.byId("gridTitulo").currentPageSize(9);
                    dijit.byId('gridTitulo').resize(true);
                }
                else {
                    dojo.byId('tgBaixaPesq').style.height = '27px';
                    dojo.byId("gridTitulo").style.height = '455px';
                    dijit.byId("gridTitulo").set("height", '455px');
                    dijit.byId("gridTitulo").attr("height", '455px');
                    //dijit.byId("gridTitulo").currentPageSize(16);
                    dijit.byId('gridTitulo').resize(true);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxTitulo(value, rowIndex, obj) {
    try {
        var gridName = 'gridTitulo';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_titulo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_titulo', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxBaixa(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixa'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBaixa');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_baixa_titulo", grid._by_idx[rowIndex].item.cd_baixa_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_baixa_titulo', 'selecionadoBaixa', -1, 'selecionaTodosBaixa', 'selecionaTodosBaixa', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_baixa_titulo', 'selecionadoBaixa', " + rowIndex + ", '" + id + "', 'selecionaTodosBaixa', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPessoaFK() {
    try {
        limparPesquisaPessoaFK();
        dijit.byId("tipoPessoaFK").set("value", 1);
        //dijit.byId("tipoPessoaFK").set("disabled", true);
        //dijit.byId("_nomePessoaFK").set("value", "a");
        //dijit.byId("inicioPessoaFK").set("checked", true);
        pesquisaPessoaFKTitulo();
        dijit.byId("proPessoa").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKTitulo() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&responsavel=" + document.getElementById("ckResponsavelPesq").checked,
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
            $("#cdPessoaResp").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
            $("#pessoaBaixaFinan").val(gridPessoaSelec.itensSelecionados[0].no_pessoa);
            //apresentaMensagem(dojo.byId("descApresMsg").value, null);
            dijit.byId("limparPessoaRespo").set("disabled", false);
        }

        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataLocalMvto(xhr, ready, Memory, filteringSelect, natureza) {
    xhr.get({
        url: Endereco() + "/api/financeiro/getLocalMovto?cd_loc_mvto=0&natureza=" + natureza,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagem", null);
            if (hasValue(data) && hasValue(data.retorno.locaisMovtoTitulo))
                criarOuCarregarCompFiltering("localMovtoPesq", data.retorno.locaisMovtoTitulo, "", null, ready, Memory, filteringSelect, 'cd_local_movto', 'nomeLocal', MASCULINO);
            if (hasValue(data) && hasValue(data.retorno.locaisMovtoBaixa))
                criarOuCarregarCompFiltering("localMovtoBaixaPesq", data.retorno.locaisMovtoBaixa, "", null, ready, Memory, filteringSelect, 'cd_local_movto', 'nomeLocal', MASCULINO);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarTitulo(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridTitulo = dijit.byId("gridTitulo");
            var localMovtoPesq = dijit.byId("localMovtoPesq").value;
            var cdLocMovto = hasValue(localMovtoPesq) ? localMovtoPesq : 0;
            var localMovtoBaixaPesq = dijit.byId("localMovtoBaixaPesq").value;
            var cdLocMovtoBaixa = hasValue(localMovtoBaixaPesq) ? localMovtoBaixaPesq : 0;
            var cdResponsavel = hasValue(dojo.byId("cdPessoaResp").value) ? dojo.byId("cdPessoaResp").value : 0;
            var statusPesq = hasValue(dijit.byId("statusPesq").value) ? dijit.byId("statusPesq").value : 0;
            var naturezaPesq = hasValue(dijit.byId("naturezaPesq").value) ? dijit.byId("naturezaPesq").value : 0;
            var parcelaTitulo = hasValue(dijit.byId("parcelaTitulo").value) ? dijit.byId("parcelaTitulo").value : 0;
            var valorPesq = hasValue(dijit.byId("valorPesq").value) ? dijit.byId("valorPesq").value : "";
            var numeroTitulo = hasValue(dijit.byId("numeroTitulo").value) ? dijit.byId("numeroTitulo").value : 0;
            var cdTipoLiquidacao = hasValue(dijit.byId("tipoLiquidacaoPesq").value) ? dijit.byId("tipoLiquidacaoPesq").value : 0;
            var cdTipoTitulo = hasValue(dijit.byId("pesqTipoTitulo").value) ? dijit.byId("pesqTipoTitulo").value : 0;
            var cdCnabStatus = (hasValue(dijit.byId("pesqCnabStatus").value) || dijit.byId("pesqCnabStatus").value == 0) ? dijit.byId("pesqCnabStatus").value : -1;
            var cdTipoFinanceiro = hasValue(dijit.byId("pesqTipoDocumento").value) ? dijit.byId("pesqTipoDocumento").value : null;
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            var pesNossoNumero = hasValue(dijit.byId("pesNossoNumero").value) ? dijit.byId("pesNossoNumero").value : "";
            var nroRecibo = hasValue(dijit.byId("nroRecibo").value) ? dijit.byId("nroRecibo").value : 0;
            var cd_turma = hasValue(dojo.byId("cd_turmafk_baixa_finan").value) ? dojo.byId("cd_turmafk_baixa_finan").value : 0;
            if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
                var myStore =
                             Cache(
                                     JsonRest({
                                         target: Endereco() + "/api/financeiro/getTituloSearchGeral?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=" + document.getElementById("ckResponsavelPesq").checked +
                                             "&inicio=" + document.getElementById("inicioPessoaRespPesq").checked + "&locMov=" + parseInt(cdLocMovto) + "&natureza=" + parseInt(naturezaPesq) +
                                             "&status=" + parseInt(statusPesq) + "&numeroTitulo=" + numeroTitulo + "&parcelaTitulo=" + parcelaTitulo + "&valorTitulo=" + valorPesq +
                                             "&dtInicial=" + dojo.byId("dtInicial").value + "&dtFinal=" + dojo.byId("dtFinal").value + "&emissao=" + document.getElementById("ckEmissao").checked +
                                             "&vencimento=" + document.getElementById("ckVcto").checked + "&baixa=" + document.getElementById("ckBaixa").checked + "&locMovBaixa=" + parseInt(cdLocMovtoBaixa) +
                                             "&cdTipoLiquidacao=" + cdTipoLiquidacao + "&tipoTitulo=" + cdTipoTitulo + "&nossoNumero=" + pesNossoNumero + "&cnabStatus=" + cdCnabStatus +
                                             "&nro_recibo=" + nroRecibo + "&cd_turma=" + cd_turma + "&cd_situacoes_aluno=" + dijit.byId("situacaoAlunoTurma").value.toString() +
                                             "&cd_tipo_financeiro=" + cdTipoFinanceiro,
                                         handleAs: "json",
                                         preventCache: true,
                                         headers: { "Accept": "application/json", "Authorization": Token() }
                                     }), Memory({}));
            }
            else {
                var myStore =
                             Cache(
                                     JsonRest({
                                         target: Endereco() + "/api/financeiro/getTituloSearch?cd_pessoa=" + parseInt(cdResponsavel) + "&responsavel=" + document.getElementById("ckResponsavelPesq").checked +
                                             "&inicio=" + document.getElementById("inicioPessoaRespPesq").checked + "&locMov=" + parseInt(cdLocMovto) + "&natureza=" + parseInt(naturezaPesq) +
                                             "&status=" + parseInt(statusPesq) + "&numeroTitulo=" + numeroTitulo + "&parcelaTitulo=" + parcelaTitulo + "&valorTitulo=" + valorPesq +
                                             "&dtInicial=" + dojo.byId("dtInicial").value + "&dtFinal=" + dojo.byId("dtFinal").value + "&emissao=" + document.getElementById("ckEmissao").checked +
                                             "&vencimento=" + document.getElementById("ckVcto").checked + "&baixa=" + document.getElementById("ckBaixa").checked + "&locMovBaixa=" + parseInt(cdLocMovtoBaixa) +
                                             "&cdTipoLiquidacao=" + cdTipoLiquidacao + "&tipoTitulo=" + cdTipoTitulo + "&nossoNumero=" + pesNossoNumero + "&cnabStatus=" + cdCnabStatus +
                                             "&nro_recibo=" + nroRecibo + "&cd_turma=" + cd_turma + "&cd_situacoes_aluno=" + dijit.byId("situacaoAlunoTurma").value.toString() +
                                             "&cd_tipo_financeiro=" + cdTipoFinanceiro,
                                         handleAs: "json",
                                         preventCache: true,
                                         headers: { "Accept": "application/json", "Authorization": Token() }
                                     }), Memory({}));
            }
            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridTitulo.itensSelecionados = [];
            gridTitulo.itemSelecionado = null;
            gridTitulo.noDataMessage = msgNotRegEnc;
            gridTitulo.setStore(dataStore);
            var gridBaixa = dijit.byId("gridBaixa");
            gridBaixa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            configLayoutPessoaBaixaFina(dijit.byId("naturezaPesq").get("value"));
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function criarGridHistorico() {
    dojo.ready(function () {
        try {
            var dataHistoricoTt = [
                {
                    nm_usuario: 'Usuário 1', dta_historico: '19/06/2014 15:24:45', dc_antigo: '', dc_novo: '', id: 1, pai: null,
                    children: [
                        { nm_usuario: 'Vencimento', dta_historico: null, dc_antigo: '10/06/2014', dc_novo: '12/06/2014', id: 2, pai: 1 },
                        { nm_usuario: 'Valor Titulo', dta_historico: null, dc_antigo: '100,00', dc_novo: '120,00', id: 3, pai: 1 },
                        { nm_usuario: 'Local Movimento', dta_historico: null, dc_antigo: 'Bradesco', dc_novo: 'Itaú', id: 4, pai: 1 }
                    ]
                },
                {
                    nm_usuario: 'Usuário 2', dta_historico: '12/04/2014 12:20:45', dc_antigo: '', dc_novo: '', id: 8, pai: null,
                    children: [
                        { nm_usuario: 'Vencimento', dta_historico: null, dc_antigo: '10/03/2014', dc_novo: '15/03/2014', id: 9, pai: 8 }
                    ]
                }
            ];

            var data = {
                identifier: 'id',
                label: 'nm_usuario',
                items: dataHistoricoTt
            };
            var store = new dojo.data.ItemFileWriteStore({ data: data });
            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            /* set up layout */
            var layout = [
              { name: 'Usuário', field: 'nm_usuario', width: '45%' },
              { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
              { name: 'Vl.Antigo', field: 'dc_antigo', width: '15%', styles: "text-align: center;" },
              { name: 'Vl.Novo', field: 'dc_novo', width: '15%', styles: "text-align: center;" },
              { name: '', field: 'id', width: '5%', styles: "display:none;" }
            ];

            var gridHistoricoTt = new dojox.grid.LazyTreeGrid({
                id: 'gridHistoricoTitulo',
                treeModel: model,
                structure: layout
            }, document.createElement('div'));
            dojo.byId("gridHistoricoTitulo").appendChild(gridHistoricoTt.domNode);
            gridHistoricoTt.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function selecionaTabTitulo(e) {
    try {
        var tab = hasValue(e) && hasValue(dojo.query(e.target)) && hasValue(dojo.query(e.target).parents) && hasValue(dojo.query(e.target).parents('.dijitTab')[0])
            ? dojo.query(e.target).parents('.dijitTab')[0] : null;
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainerTitulo_tablist_tabHistorico' && !hasValue(dijit.byId("gridHistoricoTitulo"))) {
            var cd_titulo = dojo.byId("cd_titulo").value;
            montarGridHistoricoTitulo(parseInt(cd_titulo), dojo.xhr);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
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

// MMC  Melhoria 2705 - function retornarPlanoContaTitulo(planoContas) {
//    dojo.byId('cdPlanoContasTitulo').value = planoContas.name;
//    dojo.byId('cd_plano_contas_titulo').value = planoContas.cd_plano_conta;
//}

//Metodos Titulo.

function setarTabCadTitulo() {
    try {
        var tabs = dijit.byId("tabContainerTitulo");
        var pane = dijit.byId("tabPrincipalTitulo");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTitulo(itensSelecionados, xhr, ready, Memory, FilteringSelect) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            destroyCreateHistoricoTitulo();
            setarTabCadTitulo();
            showEditTitulo(itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
            dijit.byId("cadTitulo").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditTitulo(cd_titulo, xhr, ready, Memory, FilteringSelect) {
    try {
        showCarregando();
        xhr.get({
            url: Endereco() + "/api/financeiro/getTituloBaixaFinanComFiltrosTrocaFinanceira?cd_titulo=" + cd_titulo,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var existBaixa = false;
                data = jQuery.parseJSON(data).retorno;
                dijit.byId("cbLocalMovtoTitulo").reset();
                apresentaMensagem("apresentadorMensagemTitulo", null);
                dijit.byId('tabContainerTitulo').resize();
                dojo.byId('tabContainerTitulo_tablist').children[3].children[0].style.width = '100%';
                if ((data.id_status_titulo == TITULO_FECHADO) || (data.vl_titulo != data.vl_saldo_titulo)) {
                    loadSelectLocalMovimento(data.bancos, "cbLocalMovtoTitulo", 'cd_local_movto', 'nomeLocal', 'nm_tipo_local');
                        //criarOuCarregarCompFiltering("cbLocalMovtoTitulo", data.bancos, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    if (hasValue(data.cd_tipo_financeiro) && data.cd_tipo_financeiro > 0)
                        criarOuCarregarCompFiltering("cbTipoFinan", [{ id: data.cd_tipo_financeiro, name: data.tipoDoc }], "", data.cd_tipo_financeiro, ready, Memory, FilteringSelect, 'id', 'name');
                } else {
                    //if ((data.id_natureza_titulo == RECEBER || data.id_natureza_titulo == PAGAR) && data.id_status_titulo == TITULO_ABERTO) {
                    if (hasValue(data.bancos) && data.bancos.length > 0)
                        loadSelectLocalMovimento(data.bancos, "cbLocalMovtoTitulo", 'cd_local_movto', 'nomeLocal', 'nm_tipo_local');
                        //criarOuCarregarCompFiltering("cbLocalMovtoTitulo", data.bancos, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    //if (hasValue(data.cd_tipo_financeiro) && data.cd_tipo_financeiro > 0)
                    if (hasValue(data.tipoDocumentos) && data.tipoDocumentos.length > 0)
                        criarOuCarregarCompFiltering("cbTipoFinan", data.tipoDocumentos, "", data.cd_tipo_financeiro, ready, Memory, FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro');
                }
                loadDataTitulio(data);
                if (data.vl_titulo != data.vl_saldo_titulo)
                    existBaixa = true;
                var tp_liq_motivo_bolsa = false;
                if (data != null && data.BaixaTitulo != null && data.BaixaTitulo.length == 1 && (data.BaixaTitulo[0].cd_tipo_liquidacao == BAIXAMOTIVOBOLSA || data.BaixaTitulo[0].cd_tipo_liquidacao == BAIXAMOTIVOBOLSAADITIVO))
                    tp_liq_motivo_bolsa = true;
                configLayoutCadTitulo(data.id_status_titulo, data.id_natureza_titulo, existBaixa, tp_liq_motivo_bolsa);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemTitulo", error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTitulio(titulo) {
    try {
        dojo.byId("cd_titulo").value = titulo.cd_titulo;
        dojo.byId("cdPessoaTitulo").value = titulo.cd_pessoa_titulo;
        dojo.byId("noPessoaTitulo").value = titulo.nomeResponsavel;
        dijit.byId("cadNumeroTitulo").set("value", titulo.nm_titulo);
        dijit.byId("cadParcelaTitulo").set("value", titulo.nm_parcela_titulo);
        dijit.byId("dtaEmissaoTitulo").set("value", titulo.dt_emissao_titulo);
        dijit.byId("dtaVencTitulo").set("value", titulo.dt_vcto_titulo);
        //cd_local_movto = x.cd_local_movto,
        dijit.byId("cadValorTitulo").set("value", titulo.vl_titulo);
        dijit.byId("cadJurosPercTitulo").set("value", titulo.pc_juros_titulo);
        dijit.byId("cadMultaPercTitulo").set("value", titulo.pc_multa_titulo);
        dijit.byId("cbLocalMovtoTitulo").set("value", titulo.cd_local_movto);

        //tag cartão
        dijit.byId("nm_dias_cartao")._onChangeActive = false;
        dijit.byId("vl_taxa_cartao")._onChangeActive = false;
        dijit.byId("pc_taxa_cartao")._onChangeActive = false;

        dijit.byId("pc_taxa_cartao").set("value", titulo.pc_taxa_cartao);
        dijit.byId("nm_dias_cartao").set("value", titulo.nm_dias_cartao);
        dijit.byId("vl_taxa_cartao").set("value", titulo.vl_taxa_cartao);

        dijit.byId("vl_taxa_cartao")._onChangeActive = true;
        dijit.byId("nm_dias_cartao")._onChangeActive = true;
        dijit.byId("pc_taxa_cartao")._onChangeActive = true;
        // tag valores
        dijit.byId("cadJurosTitulo").set("value", titulo.vl_juros_titulo);
        dijit.byId("cadMultaTitulo").set("value", titulo.vl_multa_titulo);
        dijit.byId("cadDescontoTitulo").set("value", titulo.vl_desconto_titulo);
        dijit.byId("cadSaldoTitulo").set("value", titulo.vl_saldo_titulo);
        //tag valores liquidação
        dijit.byId("dtBaixa").set("value", titulo.dt_liquidacao_titulo);
        dijit.byId("valorBaixa").set("value", titulo.vl_liquidacao_titulo);
        dijit.byId("idDescJuros").set("value", titulo.vl_desconto_juros);
        dijit.byId("idDescMulta").set("value", titulo.vl_desconto_multa);
        dijit.byId("idJurosBaixa").set("value", titulo.vl_juros_liquidado);
        dijit.byId("idMultaBaixa").set("value", titulo.vl_multa_liquidada);
        dijit.byId("dtCadastroTitulo").set("value", titulo.dh_cadastro_titulo);
        dijit.byId("nossoNroTitulo").set("value", titulo.dc_nosso_numero);
        if (hasValue(titulo.hr_cadastro_titulo))
            dijit.byId("horaCadastroTitulo").set("value", "T" + titulo.hr_cadastro_titulo);

        //natureza
        dijit.byId("cbNaturezaTitulo").set("value", titulo.id_natureza_titulo);
        //Status titulo
        dijit.byId("cbStatusTitulo").set("value", titulo.id_status_titulo);

        //Cnab Status Título
        dijit.byId("cnabStatus").set("value", titulo.id_status_cnab);

        //Plano de contas
        // MMC Melhoria 2705 - if (hasValue(titulo.cd_plano_conta_tit)) {
        //    dojo.byId('cdPlanoContasTitulo').value = titulo.dc_plano_conta;
        //    dojo.byId('cd_plano_contas_titulo').value = titulo.cd_plano_conta_tit;
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configLayoutCadTitulo(status, natureza, existBaixa, tp_liq_motivo_bolsa) {
    try {
        var cbTipoFinan = dijit.byId("cbTipoFinan");
        //var tgCartao = dijit.byId("tgCartao");
        var tgCartao = dojo.byId("tgCartao");

        if ((status == TITULO_FECHADO || existBaixa) && !tp_liq_motivo_bolsa) {
            dijit.byId("pesCadPessoaTitulo").set("disabled", true);
            dijit.byId("cbTipoFinan").set("disabled", true);
            dijit.byId("dtaEmissaoTitulo").set("disabled", true);
            dijit.byId("dtaVencTitulo").set("disabled", true);
            dijit.byId("cbLocalMovtoTitulo").set("disabled", true);
            dijit.byId("cadValorTitulo").set("disabled", true);
            dijit.byId("cadJurosPercTitulo").set("disabled", true);
            dijit.byId("cadMultaPercTitulo").set("disabled", true);
            dijit.byId("pesPlanoTitulo").set("disabled", true);
            dijit.byId("dtCadastroTitulo").set("disabled", true);
            dijit.byId("horaCadastroTitulo").set("disabled", true);

            dijit.byId("alterarTitulo").set("disabled", true);
            dijit.byId("cancelarTitulo").set("disabled", true);

            dijit.byId("pc_taxa_cartao").set("disabled", true);
            dijit.byId("nm_dias_cartao").set("disabled", true);
            dijit.byId("vl_taxa_cartao").set("disabled", true);
        } else {
            //if (natureza == RECEBER && status == TITULO_ABERTO) {
            dijit.byId("pesCadPessoaTitulo").set("disabled", true);
            dijit.byId("cbTipoFinan").set("disabled", false);
            dijit.byId("dtaEmissaoTitulo").set("disabled", true);
            dijit.byId("dtaVencTitulo").set("disabled", false);
            dijit.byId("cbLocalMovtoTitulo").set("disabled", false);
            dijit.byId("cadValorTitulo").set("disabled", true);
            dijit.byId("cadJurosPercTitulo").set("disabled", false);
            dijit.byId("cadMultaPercTitulo").set("disabled", false);
            dijit.byId("pesPlanoTitulo").set("disabled", false);
            dijit.byId("dtCadastroTitulo").set("disabled", true);
            dijit.byId("horaCadastroTitulo").set("disabled", true);

            dijit.byId("alterarTitulo").set("disabled", false);
            dijit.byId("cancelarTitulo").set("disabled", false);

            dijit.byId("pc_taxa_cartao").set("disabled", false);
            dijit.byId("nm_dias_cartao").set("disabled", false);
            dijit.byId("vl_taxa_cartao").set("disabled", false);
        }
        
        if (cbTipoFinan.value == CARTAO) {
            tgCartao.style.display = "block";
        } else {
            tgCartao.style.display = "none";
        }
        
        //if (natureza == PAGAR == status == TITULO_ABERTO) {
        //    dijit.byId("pesCadPessoaTitulo").set("disabled", false);
        //    dijit.byId("cbTipoFinan").set("disabled", false);
        //    dijit.byId("dtaEmissaoTitulo").set("disabled", false);
        //    dijit.byId("dtaVencTitulo").set("disabled", false);
        //    dijit.byId("cbLocalMovtoTitulo").set("disabled", false);
        //    dijit.byId("cadValorTitulo").set("disabled", false);
        //    dijit.byId("cadJurosPercTitulo").set("disabled", false);
        //    dijit.byId("cadMultaPercTitulo").set("disabled", false);
        //    dijit.byId("pesPlanoTitulo").set("disabled", false);
        //    dijit.byId("dtCadastroTitulo").set("disabled", false);
        //    dijit.byId("horaCadastroTitulo").set("disabled", false);
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadTitulos() {
    try {
        clearForm("formTituloPrincipal");
        dojo.byId("cd_titulo").value = 0;
        dojo.byId("cdPessoaTitulo").value = 0;
        dojo.byId("cd_plano_contas_titulo").value = 0;
        dijit.byId("cadNumeroTitulo").reset();
        dijit.byId("cadParcelaTitulo").reset();
        dijit.byId("dtaEmissaoTitulo").reset();
        dijit.byId("dtaVencTitulo").reset();
        //cd_local_movto = x.cd_local_movto,
        dijit.byId("cadValorTitulo").reset();
        dijit.byId("cadJurosPercTitulo").reset();
        dijit.byId("cadMultaPercTitulo").reset();
        // tag valores
        dijit.byId("cadJurosTitulo").reset();
        dijit.byId("cadMultaTitulo").reset();
        dijit.byId("cadDescontoTitulo").reset();
        dijit.byId("cadSaldoTitulo").reset();
        //tag valores liquidação
        dijit.byId("dtBaixa").reset();
        dijit.byId("valorBaixa").reset();
        dijit.byId("idDescJuros").reset();
        dijit.byId("idDescMulta").reset();
        dijit.byId("idJurosBaixa").reset();
        dijit.byId("idMultaBaixa").reset();
        dijit.byId("dtCadastroTitulo").reset();
        dijit.byId("horaCadastroTitulo").reset();
        //natureza
        dijit.byId("cbNaturezaTitulo").reset();
        //Status titulo
        dijit.byId("cbStatusTitulo").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Metodo C.R.U.D titulo
function updateTitulo(xhr) {
    require([
       "dojo/ready",
       "dojo/date"
    ], function (ready, date) {
        ready(function () {
            try {
                var dtaVencTitulo = dijit.byId('dtaVencTitulo').value;
                if (!ValidateRangeDate(dtaVencTitulo, date, "apresentadorMensagemTitulo", msgDtaTituloVencMin, msgDtaTituloVencMax))
                    return false;

                if (!dijit.byId("formTituloPrincipal").validate()) {
                    setarTabCadTitulo();
                    return false;
                }

                showCarregando();
                xhr.post({
                    url: Endereco() + "/api/financeiro/postUpdateTituloBaixaFinan",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    postData: JSON.stringify({
                        cd_titulo: dojo.byId("cd_titulo").value,
                        dt_vcto_titulo: dojo.date.locale.parse(dojo.byId("dtaVencTitulo").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                        //** MMC - melhoria 2705 - cd_plano_conta_tit: dojo.byId('cd_plano_contas_titulo').value,
                        cd_local_movto: dijit.byId("cbLocalMovtoTitulo").get("value"),
                        pc_juros_titulo: dijit.byId("cadJurosPercTitulo").value,
                        pc_multa_titulo: dijit.byId("cadMultaPercTitulo").value,
                        pc_taxa_cartao: dijit.byId("pc_taxa_cartao").value,
                        nm_dias_cartao: dijit.byId("nm_dias_cartao").value,
                        vl_taxa_cartao: dijit.byId("vl_taxa_cartao").value,
                        cd_local_movto: dijit.byId("cbLocalMovtoTitulo").value,
                        cd_tipo_financeiro: dijit.byId("cbTipoFinan").value
                    })
                }).then(function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridTitulo';
                            var grid = dijit.byId(gridName);
                            apresentaMensagem('apresentadorMensagem', data);
                            data = data.retorno;

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];
                            removeObjSort(grid.itensSelecionados, "cd_titulo", itemAlterado.cd_titulo);
                            insertObjSort(grid.itensSelecionados, "cd_titulo", itemAlterado, false);
                            buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_titulos', 'selecionaTodos', ['pesquisaTitulos'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_titulo");
                            showCarregando();
                            dijit.byId("cadTitulo").hide();
                        } else {
                            apresentaMensagem('apresentadorMensagemTitulo', data);
                            showCarregando();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem('apresentadorMensagemTitulo', error);
                    showCarregando();
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function alterarBaixa(xhr, ref) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCadBaixa', null);

        if (!dijit.byId("formCadBaixa").validate()) {
            dijit.byId("tgCheque").set("open", true);
            return false;
        }
        showCarregando();
        var transacao = montaListaBaixa();
        xhr.post({
            url: Endereco() + "/api/escola/postUpdateTransacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(transacao)
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagem', data);

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
                    }

                buscarItensSelecionados(gridName, 'selecionado', 'cd_titulo', 'selecionaTodos', ['pesquisaTitulos'], 'todosItens', 2);
                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_titulo");
                grid.itemSelecionado = null;
                limparGridBaixas();
                if (hasValue(data) && hasValue(data.titulosBaixa) && hasValue(data.titulosBaixa[0])) {
                    grid.itemSelecionado = data.titulosBaixa[0];
                    buscarBaixasTitulos(data.titulosBaixa[0]);
                } else
                    hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemCadBaixa', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarEventosBotoesPrincipaisCadTransacao(xhr, on) {
    dijit.byId("incluirBaixa").on("click", function () {
        incluirBaixa(xhr, dojox.json.ref);
    });
    dijit.byId("alterarBaixa").on("click", function () {
        alterarBaixa(xhr, dojox.json.ref);
    });
    dijit.byId("deleteBaixa").on("click", function () {
        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarTransFinan(xhr, dojox.json.ref) });
    });
    dijit.byId("cancelarBaixa").on("click", function () {
        limparCamposBaixaCad();
        eventoEditarBaixaTitulo(dijit.byId("gridBaixa").itensSelecionados, xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, on);
    });
    dijit.byId("fecharBaixa").on("click", function () {
        dijit.byId("cadBaixaFinanceira").hide();
    });
    dijit.byId("limparBaixa").on("click", function () {
        var gridTitulo = dijit.byId('gridTitulo');
        var itensSelecionados = gridTitulo.itensSelecionados;
        showCarregando();
        limparCamposBaixaCad();
        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        simularBaixaTitulos(itensSelecionados, xhr, dojox.json.ref, Permissoes);
    });
}

function incluirBaixa(xhr, ref) {
    require([
       "dojo/ready",
       "dojo/date"
    ], function (ready, date) {
        ready(function () {
            try {
                var mensagensWeb = new Array();
                var dtBaixa = dijit.byId('dt_baixa').value;
                if (!ValidateRangeDate(dtBaixa, date, "apresentadorMensagemCadBaixa", msgErroDtaMin, msgErroDtaMax))
                return false;

                var gridBaixaCad = dijit.byId('gridBaixaCad');
                var itensGridBaixa = gridBaixaCad.store.objectStore.data;
                var bool_troca = false;
                var msg_aviso = "";
                apresentaMensagem('apresentadorMensagemCadBaixa', null);

                if (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value != BAIXACANCELAMENTO && dijit.byId("cbLiquidacao").value != BAIXAMOTIVOBOLSA && dijit.byId("cbLiquidacao").value != BAIXAMOTIVOBOLSAADITIVO && dijit.byId("cbLiquidacao").value != DESCONTOFOLHAPAGAMENTO)) {
                    if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CARTAO && (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value !== CARTAOCREDITO && dijit.byId("cbLiquidacao").value !== CARTAODEBITO))) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidCartaoDif);
                        apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                        return false;
                    } else if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CHEQUE && hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value !== CHEQUEVISTA && dijit.byId("cbLiquidacao").value !== CHEQUEPREDATADO)) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);
                        apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                        return false;
                    }
                }
                if (!dijit.byId("formCadBaixa").validate()) {
                    dijit.byId("tgCheque").set("open", true);
                    return false;
                }
                if (hasValue(itensGridBaixa, true) && itensGridBaixa.length <= 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTransSemTitulo);
                    apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                    return false;
                }
                if(hasValue(dijit.byId("cbLiquidacao").value) && ((dijit.byId("cbLiquidacao").value == CHEQUEVISTA || dijit.byId("cbLiquidacao").value == CHEQUEPREDATADO) &&
                    ((!dijit.byId("tgCheque").open && dojo.byId('nroCheque').value == "") || (dijit.byId("tgCheque").open && dojo.byId('nroPrimeiroChequeChequeBaixa').value == "" ))))
                {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Falta informar o número do cheque');
                    apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                    return false;
                }
                showCarregando();
                var transacao = montaListaBaixa();
                if (transacao.Baixas[0].Titulo.cd_tipo_financeiro != CARTAO && transacao.Baixas[0].Titulo.cd_tipo_financeiro != CHEQUE) 
                {
                    if (transacao.cd_tipo_liquidacao === CARTAOCREDITO || transacao.cd_tipo_liquidacao === CARTAODEBITO || transacao.cd_tipo_liquidacao === CHEQUEVISTA || transacao.cd_tipo_liquidacao === CHEQUEPREDATADO) 
                    {
                        bool_troca = true;
                        msg_aviso = transacao.cd_tipo_liquidacao === CARTAOCREDITO || transacao.cd_tipo_liquidacao === CARTAODEBITO ? "cartão" : "cheque"
                        transacao.cd_tipo_liquidacao = TROCA_FINANCEIRA;
                    }
                }
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
                        }
                        buscarItensSelecionados(gridName, 'selecionado', 'cd_titulo', 'selecionaTodos', ['pesquisaTitulos'], 'todosItens', 2);
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_titulo");
                        grid.itemSelecionado = null;
                        limparGridBaixas();
                        grid.itemSelecionado = data.titulosBaixa[0];
                        buscarBaixasTitulos(data.titulosBaixa[0]);
                        if (bool_troca)
                            caixaDialogo(DIALOGO_AVISO, 'Foi realizada uma troca financeira e gerado um titulo com o mesmo numero, em aberto e tipo ' + msg_aviso + '.', null);
                        //showCarregando();
                    }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }
                },
                function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemCadBaixa', error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function deletarTransFinan(xhr, ref) {
    try {
        if (dojo.byId('cd_tran_finan').value != 0)
            itensSelecionados = {
                cd_tran_finan: dojo.byId("cd_tran_finan").value
            };
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/postDeleteTransFinanceiraBaixa",
            headers: { "Accept": "applicatsion/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagem', data);
                limparGridBaixas();
                pesquisarTitulo(true);
                dijit.byId("pesquisaTitulos").set("disabled", false);
                dijit.byId("cadBaixaFinanceira").hide();
                dijit.byId("gridTitulo").itemSelecionado = null;
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            if (!hasValue(dojo.byId("cd_tran_finan").style.display))
                apresentaMensagem('apresentadorMensagemCadBaixa', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
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

function eventoNFServicoBaixa(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getverificarGeracaoNFSBaixa?cd_baixa=" + itensSelecionados[0].cd_baixa_titulo,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    window.location = Endereco() + '/Secretaria/movimentos?tipo=' + VENDASSERVICOORIGEMBAIXA + '&idOrigemNF=' + ORIGEMBAIXANF + '&cdBaixa=' + itensSelecionados[0].cd_baixa_titulo;
                } catch (e) {
                    postGerarLog(e);
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
                var retornoData = jQuery.parseJSON(data).retorno;

                if (!hasValue(retornoData) || retornoData.length <= 0) {
                    retornoData = [];
                }
                else {
                    retornoData = clearChildrenLenthZero(retornoData);
                }
                    
                var dataTree = {
                    identifier: 'id',
                    label: 'descricao',
                    items: retornoData
                };

                var store = new dojo.data.ItemFileWriteStore({ data: dataTree });

                var model = new dijit.tree.ForestStoreModel({
                    store: store, childrenAttrs: ['children']
                });

                var layout = [
                  { name: 'Usuário', field: 'descricao', width: '20%' },
                  { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
                  { name: 'Vl.Antigo', field: 'dc_valor_antigo', width: '20%', styles: "text-align: center;" },
                  { name: 'Vl.Novo', field: 'dc_valor_novo', width: '20%', styles: "text-align: center;" },
                  { name: 'Operação', field: 'dc_tipo_log', width: '20%', styles: "text-align: center;" },
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

function montarGridHistoricoTitulo(cd_titulo, xhr) {
    xhr.get({
        url: Endereco() + "/api/escola/getLogGeralTitulo?cd_titulo=" + cd_titulo,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagemTitulo", null);
            var retornoData = jQuery.parseJSON(data).retorno;
            if (!hasValue(retornoData) || retornoData.length <= 0) {
                retornoData = [];
            }
            else {
                retornoData = clearChildrenLenthZero(retornoData);
            }

            var dataTree = {
                identifier: 'id',
                label: 'descricao',
                items: retornoData
            };

            var store = new dojo.data.ItemFileWriteStore({ data: dataTree });

            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            var layout = [
              { name: 'Usuário', field: 'descricao', width: '20%' },
              { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
              { name: 'Vl.Antigo', field: 'dc_valor_antigo', width: '20%', styles: "text-align: center;" },
              { name: 'Vl.Novo', field: 'dc_valor_novo', width: '20%', styles: "text-align: center;" },
              { name: 'Operação', field: 'dc_tipo_log', width: '20%', styles: "text-align: center;" },
              { name: '', field: 'id', width: '0%', styles: "display: none;" }
            ];
            destroyCreateHistoricoTitulo();
            var gridHistoricoBaixaTitulo = new dojox.grid.LazyTreeGrid({
                id: 'gridHistoricoTitulo',
                treeModel: model,
                structure: layout,
                noDataMessage: msgNotRegEnc
            }, document.createElement('div'));

            dojo.byId("gridHistoricoTitulo").appendChild(gridHistoricoBaixaTitulo.domNode);
            gridHistoricoBaixaTitulo.canSort = function (col) { return false; };
            gridHistoricoBaixaTitulo.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemTitulo', error);
    });
}

function clearChildrenLenthZero(dataRetorno) {
    try {
        for (var i = 0; i < dataRetorno.length; i++) {
            if (dataRetorno[i].children.length > 0){
                for (var j = 0; j < dataRetorno[i].children.length; j++) {
                    if (dataRetorno[i].children[j].children != null && dataRetorno[i].children[j].children.length > 0) {
                        for (var m = 0; m < dataRetorno[i].children[j].children.length; m++)
                            delete dataRetorno[i].children[j].children[m].children;
                    } else delete dataRetorno[i].children[j].children;
                }
            }
        }
        return dataRetorno;
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

function destroyCreateHistoricoTitulo() {
    try {
        if (hasValue(dijit.byId("gridHistoricoTitulo"))) {
            dijit.byId("gridHistoricoTitulo").destroy();
            //$('<div>').attr('id', 'gridHistoricoBaixaTitulo').attr('style', 'height:100%;').attr('style', 'min-height:200px;').appendTo('#paiGridHistBaixaTitulo');
        }
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
            else
                document.getElementById("setValueOrigemTitulo").value = data[0].id_origem_titulo;
            gridBaixa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
            hideCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        hideCarregando();
        apresentaMensagem('apresentadorMensagem', error);
        
    });
}

function configLayoutPessoaBaixaFina(value, rowIndex, obj) {
    try {
        var natureza = dijit.byId("naturezaPesq").value;
        if (hasValue(natureza)) {
            if (natureza == TODAS) {
                dojo.byId("lblPessoaBaixaFinan").innerHTML = "Cliente/Fornecedor:";
                dojo.byId("idColPessoa").childNodes[0].innerHTML = "Cliente/Fornecedor";
                //dijit.byId("gridTitulo").getCell(8).name = "Cliente/Fornecedor";
                //obj.name = "Cliente/Fornecedor";
            }
            if (natureza == RECEBER) {
                dojo.byId("lblPessoaBaixaFinan").innerHTML = "Cliente:";
                dojo.byId("idColPessoa").childNodes[0].innerHTML = "Cliente";
                //dijit.byId("gridTitulo").getCell(8).name = "Cliente";
                //obj.name = "Cliente";
            }
            if (natureza == PAGAR) {
                dojo.byId("lblPessoaBaixaFinan").innerHTML = "Fornecedor:";
                dojo.byId("idColPessoa").childNodes[0].innerHTML = "Fornecedor";
                //dijit.byId("gridTitulo").getCell(8).nameL = "Fornecedor";
                //obj.name = "Fornecedor";
            }
            //dijit.byId("gridTitulo").buildRendering();
            //dijit.byId("gridTitulo").createViews();
            //dijit.byId("gridTitulo").defaultUpdate();
            //dijit.byId("gridTitulo").defaultUpdate();
        }
        return value;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarTipoDocumento(xhr) {
    xhr.get({
        url: Endereco() + "/api/financeiro/getTipoFinanceiroAtivo",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagem", null);
            itens = jQuery.parseJSON(data).retorno;
            if (itens != null) {
                loadSelect(itens, "pesqTipoDocumento", 'cd_tipo_financeiro', 'dc_tipo_financeiro', 0);
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
     function (error) {
         apresentaMensagem("apresentadorMensagem", error);
     });
};

function validaTitulosTipoFinanceiroCartao(titulos, gridTitulo, xhr, ref, on) {
    try {
        
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/escola/validaTitulosTipoFinanceiroCartao",
            headers: { "Accept": "applicatsion/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(titulos)
        }).then(function (data) {
            try {
                hideCarregando();
                mostrarCadastroBaixaFinanceira(true, gridTitulo, null, xhr, ref, on);
            }
            catch (e) {
                postGerarLog(e);
                hideCarregando();
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
            hideCarregando();            
        });
    }
    catch (e) {
        postGerarLog(e);
        hideCarregando();        
    }
}

function addDiasDataEmissaoTitulo(dataEmissao, dias) {
    //var novaData = new Date(data);
    dataEmissao.setDate(dataEmissao.getDate() + dias);
    return dataEmissao;
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
                apresentaMensagem("apresentadorMensagemTitulo", null);
                dijit.byId("nm_dias_cartao")._onChangeActive = false;
                dijit.byId("vl_taxa_cartao")._onChangeActive = false;
                dijit.byId("pc_taxa_cartao")._onChangeActive = false;

                dijit.byId("pc_taxa_cartao").set("value", data.pc_taxa_cartao);
                dijit.byId("nm_dias_cartao").set("value", data.nm_dias_cartao);
                dijit.byId('vl_taxa_cartao').set("value", data.vl_taxa_cartao);
                dojo.byId("dtaVencTitulo").value = data.dt_vcto;

                dijit.byId("vl_taxa_cartao")._onChangeActive = true;
                dijit.byId("nm_dias_cartao")._onChangeActive = true;
                dijit.byId("pc_taxa_cartao")._onChangeActive = true;
            } catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem("apresentadorMensagemTitulo", error);
            });

    } catch (e) {
        postGerarLog(e);
    }
}

function getLocalTipoDocumento(cd_tipo_financeiro) {
    try {
        dojo.xhr.get({
            url: Endereco() + 
                "/api/financeiro/getLocalMovtoGeralOuCartao?cd_tipo_financeiro=" + cd_tipo_financeiro,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                apresentaMensagem("apresentadorMensagemTitulo", null);
                if (cd_tipo_financeiro < 0) cd_tipo_financeiro = -1 * cd_tipo_financeiro;
                if (cd_tipo_financeiro != CARTAO)
                    loadSelectLocalMovimento(data, "cbLocalMovtoTitulo", 'cd_local_movto', 'nomeLocal', 'nm_tipo_local');
                else
                    loadSelectLocalMovimento(data, "cbLocalMovtoTitulo", 'cd_local_movto', 'no_local_movto', 'nm_tipo_local');
            } catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem("apresentadorMensagemTitulo", error);
            });

    } catch (e) {
        postGerarLog(e);
    }
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
