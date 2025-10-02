var TODOS = 0, GERARBOLETOS = 1,CANCELAR_BOLETOS = 2,PEDIDO_BAIXA = 3, ABERTO = 1,FECHADO = 2, INICIAL = 0;
var CNABBOLETO = 9;
var CADASTRO = 1, EDICAO = 2;
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

//listaTitulosCnab
function montarCadastroRetornoCnab() {
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
    "dojo/_base/array",
    "dijit/Tooltip",
    "dojox/form/Uploader",
    "dojox/json/ref"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, array, Tooltip, Uploader,ref) {
        ready(function () {
            try{
                findIsLoadComponetePesquisaRetCnab();
                var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
                var myStore =
                    Cache(
                            JsonRest({
                                target: Endereco() + "/api/cnab/getRetornoCNABSearch?cd_carteira=" + parseInt(0) + "&cd_usuario=" + parseInt(0) + "&descRetorno=&status=" + parseInt(0) + "&dtInicial=&dtFinal=" + "&nossoNumero=" + "&cd_responsavel=" + parseInt(0) + "&cd_aluno=" + parseInt(0),
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({}));
                var gridRetornoCNAB = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    //store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosRetCnab' style='display:none'/>", field: "selecionadoRetCnab", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxRetCnab },
                        { name: "Carteira", field: "carteira_retorno_cnab", width: "40%", styles: "min-width:80px;" },
                        { name: "Retorno", field: "no_arquivo_retorno", width: "20%", styles: "min-width:80px;" },
                        { name: "Usuário", field: "usuarioRetornoCNAB", width: "15%", styles: "min-width:80px;" },
                        { name: "Data Cadastro", field: "dta_cadastro_cnab", width: "15%", styles: "min-width:80px;" },
                        { name: "Status", field: "statusRetornoCNAB", width: "10%", styles: "min-width:70px;" }
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
                }, "gridRetornoCNAB");
                gridRetornoCNAB.startup();
                gridRetornoCNAB.itensSelecionados = [];
                gridRetornoCNAB.pagination.plugin._paginator.plugin.connect(gridRetornoCNAB.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridRetornoCNAB, 'cd_retorno_cnab', 'selecionaTodosRetCnab');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridRetornoCNAB, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosRetCnab').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_retorno_cnab', 'selecionadoRetCnab', -1, 'selecionaTodosRetCnab', 'selecionaTodosRetCnab', 'gridRetornoCNAB')", gridRetornoCNAB.rowsPerPage * 3);
                    });
                });
                gridRetornoCNAB.canSort = function (col) { return Math.abs(col) != 1 };
                gridRetornoCNAB.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        keepValuesRetCnab(item, gridRetornoCNAB, false);
                        IncluirAlterar(0, 'divAlterarRetCnab', 'divIncluirRetCnab', 'divExcluirRetCnab', 'apresentadorMensagemRetornoCNAB', 'divCancelarRetCnab', 'divClearRetCnab');
                        dijit.byId("cadRetornoCNAB").show();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                //Ciração grade de títulos do cnab\
                var gridTitulosRetCnab = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosTituloCnab' style='display:none'/>", field: "selecionadoTituloCnab", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTituloCnab },
                        { name: "Tipo", field: "tipoRetornoCNAB", width: "12%", styles: "min-width:80px;" },
                        { name: "Nome", field: "nomePessoaTitulo", width: "22%", styles: "min-width:80px;" },
                        { name: "Responsável", field: "nomeResponsavel", width: "22%", styles: "min-width:80px;" },
                        { name: "Vencimento", field: "dt_vcto", width: "11%", styles: "min-width:70px;" },
                        { name: "Baixa", field: "dt_baixa", width: "10%", styles: "min-width:70px;" },
                        { name: "Valor", field: "vlBaixa", width: "9%", styles: "min-width:70px;text-align:right;" },
                        { name: "N.Número", field: "dc_nosso_numero", width: "10%", styles: "min-width:80px;" }
                    ],
                    //canSort: function(colIndex, field){
                    //    return colIndex !=0 && field != 'selecionadoCurso';
                    //},
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["13", "26", "39", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "13",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridTitulosRetCnab");
                gridTitulosRetCnab.canSort = function (col) { return Math.abs(col) != 1 };
                gridTitulosRetCnab.startup();
                gridTitulosRetCnab.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        gridTitulosRetCnab.itemSelecionado = item;
                        keepValuesTituloRetornoCnab(gridTitulosRetCnab);
                        dijit.byId("cadTituloRetornoCnab").show();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                //fim criação
                gridTitulosRetCnab.listaTitulosRetornoCNAB = new Array();
                //Criação dos botões da tela inicial (pesquisa) do cnab.
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarRetornoCnab(true); } }, "pesquisarCnabBoletos");
                decreaseBtn(document.getElementById("pesquisarCnabBoletos"), '32px');
                decreaseBtn(document.getElementById("uploader"), '18px');
            
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        try{
                            var pesqDtaInicial = dijit.byId("pesqDtaInicial");
                            var pesqDtaFinal = dijit.byId("pesqDtaFinal");
                    
                            var grid = dijit.byId("gridRetornoCNAB");
                            var cdCarteira = hasValue(dijit.byId("pesqCarteira").value) ? dijit.byId("pesqCarteira").value : 0;
                            var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
                            var cdStatus = hasValue(dijit.byId("pesqStatus").value) ? dijit.byId("pesqStatus").value : 0;
                            var cdResponsavelFiltro = (hasValue(dojo.byId("cdResponsavelFiltroFKCnab").value)) ? dojo.byId("cdResponsavelFiltroFKCnab").value : 0;
                            var cdAlunoFiltro = (hasValue(dojo.byId("cdAlunoFiltroFKCnab").value)) ? dojo.byId("cdAlunoFiltroFKCnab").value : 0;
                            xhr.get({
                                url: Endereco() + "/api/cnab/getUrlRelatorioRetornoCNAB?" + getStrGridParameters('gridRetornoCNAB') + "&cd_carteira=" + parseInt(cdCarteira) + "&cd_usuario=" + parseInt(cdUsuario) +
                                                       "&descRetorno=" + dijit.byId("retornoPesq").value + "&status=" + parseInt(cdStatus) + "&dtInicial=" + pesqDtaInicial + "&dtFinal=" + pesqDtaFinal +
                                                        "&cd_responsavel=" + cdResponsavelFiltro + "&cd_aluno=" + cdAlunoFiltro	,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioCnab");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                try{
                                    showCarregando();
                                    limparRetornoCnab();
                                    IncluirAlterar(1, 'divAlterarRetCnab', 'divIncluirRetCnab', 'divExcluirRetCnab', 'apresentadorMensagemRetornoCNAB', 'divCancelarRetCnab', 'divClearRetCnab');
                                    findIsLoadComponetesNovoRetCNAB();
                                    dojo.byId("tdBtnDown").style.display = "none";
                                    dojo.byId("tdUploader").style.display = "";
                                    dijit.byId("cadRetornoCNAB").show();
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            }
                }, "novoCnab");
                var dadosTipoRetorno = [
                    { name: "Título(s) baixado(s) ", id: "1" },
                    { name: "Título(s) confirmado(s)", id: "2" },
                    { name: "Pedido(s) de baixa(s) confirmado(s)", id: "3" },
                    { name: "Protesto(s)", id: "5" },
                    { name: "Titulo(s) com erro(s)", id: "4" }
                ]
                var dadosPesqStatus = [
                    { name: "Todos", id: "0" },
                    { name: "Aberto", id: "1" },
                    { name: "Fechado", id: "2" }
                ];
                criarOuCarregarCompFiltering("pesqTipoRetonroCad", dadosTipoRetorno, "",null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                criarOuCarregarCompFiltering("pesqStatus", dadosPesqStatus, "", TODOS, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
            
                //Criação dos botões da tela cadastro do cnab.
                criarOuCarregarCompFiltering("cadStatus", [{ name: "Aberto", id: "1" }, { name: "Fechado", id: "2" }], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                //Criação select do status do titulo cnab.

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { crudRetornoEArquivo(CADASTRO); } }, "incluirRetCnab");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        keepValuesRetCnab(null, gridRetornoCNAB , null);
                    }
                }, "cancelarRetCnab");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadRetornoCNAB").hide(); } }, "fecharRetCnab");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { crudRetornoEArquivo(EDICAO); } }, "alterarRetCnab");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarRetornoCnabs(gridRetornoCNAB.itensSelecionados); });
                    }
                }, "deleteRetCnab");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparRetornoCnab(); } }, "limparRetCnab");
                //Criação dos botões do modal de edição dos títulos cnab.
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTituloRetornoCnab").hide(); } }, "fecharRetCnabDet");

                //fim
                dijit.byId("pesqStatus").on("change", function (event) {
                    if (!hasValue(event) || event < TODOS)
                        dijit.byId("pesqStatus").set("value", TODOS);
                });
                dijit.byId("pesqTipoRetonroCad").on("change", function (event) {
                    pesquisarTitulosCnabGrade();
                });
                
                dijit.byId("tagTitulosCnab").on("show", function (event) {
                    dijit.byId("gridTitulosRetCnab").update();
                });
                dijit.byId("cadRetornoCNAB").on("show", function (event) {
                    dijit.byId("gridTitulosRetCnab").update();
                });
                criarAcaoRelacionadaCnabPesquisa(gridRetornoCNAB);
                criarBotoesFK(Memory);
                //FIm criação botões da tela inicial do cnab.
                dojo.byId("tagTitulosCnab_pane").style.paddingBottom = "0px";

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconDownload',
                    onClick: function () { }
                }, "btnDown");
                var buttonFkArray = ['uploader', 'btnDown'];
                new Tooltip({
                    connectId: ["uploader"],
                    label: "Upload",
                    position: ['above']
                });
                new Tooltip({
                    connectId: ["btnDown"],
                    label: "Download",
                    position: ['above']
                });
                dijit.byId('uploader').on("change", function (evt) {
                    try{
                        var mensagensWeb = new Array();
                        var files = dijit.byId("uploader")._files;
                        if (hasValue(files) && files.length > 0) {
                            if (dijit.byId("uploader")._files[0].name.length > 128) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanho);
                                apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
                            } else {
                                var nomeArquivo = files[0].name;
                                var extArquivo = nomeArquivo.substr(nomeArquivo.length - 4, nomeArquivo.length);
                                if (hasValue(dijit.byId("cadLocal").item) && dijit.byId("cadLocal").item.nm_banco === (EnumBanco.SICRED + "")) {
                                    if (hasValue(extArquivo) && extArquivo.toLowerCase() != ".txt" && extArquivo.toLowerCase() != ".ret" && extArquivo.toLowerCase() != ".dat" && extArquivo.toLowerCase() != ".crt" && extArquivo.toLowerCase() != ".rst") {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Extensão diferente de arquivo suportado pelo sistema, favor escolher um arquivo com a extensão '.txt' ou '.ret' ou '.dat' ou '.crt' ou '.rst'.");
                                        apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
                                        return false;
                                    }
                                } else {
                                    if (hasValue(extArquivo) && extArquivo.toLowerCase() != ".txt" && extArquivo.toLowerCase() != ".ret" && extArquivo.toLowerCase() != ".dat" && extArquivo.toLowerCase() != ".rst") {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Extensão diferente de arquivo suportado pelo sistema, favor escolher um arquivo com a extensão '.txt' ou '.ret' ou '.dat'  ou '.rst'.");
                                        apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
                                        return false;
                                    }
                                }
                                
                                //if (hasValue(files[0]) && files[0].size > 2097152) { // Aumentamos para 3MB devido ao chamado 275985
                                //if (hasValue(files[0]) && files[0].size > 3072000) { // Aumentamos para 6MB devido ao chamado 275915
                                if (hasValue(files[0]) && files[0].size > 6145000) {
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoRetornotExcedeuTamanho);
                                    apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
                                    return false;
                                }
                                apresentaMensagem('apresentadorMensagemRetornoCNAB', null);
                                dojo.byId("cadRemessa").value = files[0].name;
                            }
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                decreaseBtn(document.getElementById("btnDown"), '18px');
                dijit.byId('btnDown').on("click", function (evt) {
                    try{
                        var noRetorno = dojo.byId("cadRemessa").value;
                        var cdRetornoCnab = dojo.byId("cd_retorno_cnab").value;
                        if (hasValue(noRetorno)) {
                            var url = Endereco() + "/cnab/getArquivoRetorno?noRetorno=" + noRetorno + "&cdRetorno=" + cdRetornoCnab;
                            window.open(url);
                        } else {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgAvisoNoRetornoSemDocumento);
                            apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("cadLocal").on("change", function (event) {
                    try {
                        dijit.byId("uploader").reset();
                        dojo.byId("cadRemessa").value = "";
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("pesqDtaInicial").on("change", function (event) {
                    try{
                        apresentaMensagem("apresentadorMensagem", "");
                        var dataInicial = dijit.byId('pesqDtaInicial').get('value');
                        var dataFinal = dijit.byId('pesqDtaFinal').get('value');
                        if (!hasValue(dojo.byId('pesqDtaFinal').value))
                            dijit.byId('pesqDtaFinal').set("value", dataInicial);
                        if (dojo.date.compare(dataInicial, dataFinal, "date") == 1) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicalMaior);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return false;
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesqDtaFinal").on("change", function (event) {
                    try{
                        apresentaMensagem("apresentadorMensagem", "");
                        var dataInicial = dijit.byId('pesqDtaInicial').get('value');
                        var dataFinal = dijit.byId('pesqDtaFinal').get('value');
                        if (dojo.date.compare(dataInicial, dataFinal, "date") == 1) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicalMaior);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return false;
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323071', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['pesqCarteira', 'pesqUsuario', 'retornoPesq', 'pesqStatus', 'pesqDtaInicial', 'pesqDtaFinal'], 'pesquisarCnabBoletos', ready);

                //*** Cria a grade de Despesas **\\
                var gridDespesa = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Código", field: "cd_despesa", width: "12%", styles: "text-align:center;min-width:80px;" },
                        { name: "Descrição", field: "dc_despesa", width: "68%", styles: "min-width:80px;" },
                        { name: "Valor", field: "vl_despesa", width: "20%", styles: "text-align:right;min-width:80px;", formatter: formartValorDinheiro }
                    ],
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                }, "gridDespesa");
                gridDespesa.canSort = function (col) { return Math.abs(col) != 0 };
                gridDespesa.startup();

                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function criarAcaoRelacionadaCnabPesquisa(gridRetornoCNAB) {
    //*** Cria os botões do link de ações  e Todos os Itens**\\
    // Adiciona link de Todos os Itens:
    try{
        menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new dijit.MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(gridRetornoCNAB, 'todosItensCnab', ['pesquisarCnabBoletos', 'relatorioCnab']); pesquisarRetornoCnab(false); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new dijit.MenuItem({
            label: "Itens Selecionados",
            onClick: function () { buscarItensSelecionados('gridRetornoCNAB', 'selecionadoRetCnab', 'cd_retorno_cnab', 'selecionaTodosRetCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItensCnab'); }
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
            onClick: function () { eventoEditarCnab(dijit.byId("gridRetornoCNAB").itensSelecionados); }
        });
        menu.addChild(acaoEditar);

        var acaoExcluir = new dijit.MenuItem({
            label: "Excluir",
            onClick: function () { eventoRemoverCnab(dijit.byId("gridRetornoCNAB").itensSelecionados); }
        });
        menu.addChild(acaoExcluir);

        var acaoEditar = new dijit.MenuItem({
            label: "Relatório de Retorno Boletos",
            onClick: function () {
                imprimirTitulos(dijit.byId('gridRetornoCNAB').itensSelecionados);
            }
        });
        menu.addChild(acaoEditar);

        var acaoEditar = new dijit.MenuItem({
            label: "Processar Retorno",
            onClick: function () {
                processarRetorno(dijit.byId('gridRetornoCNAB').itensSelecionados);
            }
        });
        menu.addChild(acaoEditar);

        if (eval(MasterGeral())) {
            var açãoExcluirCNABRegistrado = new dijit.MenuItem({
                label: "Excluir Retorno Processado",
                onClick: function ()
                {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function () {
                        eventoExcluirRetornosProcessados(dijit.byId('gridRetornoCNAB').itensSelecionados);
                    });                    
                }
            });
            menu.addChild(açãoExcluirCNABRegistrado);
        }

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
            onClick: function () { eventoEditarTituloCnab(dijit.byId("gridTitulosRetCnab").itensSelecionados); }
        });
        menuTitulo.addChild(acaoEditar);

        var acaoExcluir = new dijit.MenuItem({
            label: "Ver Baixa",
            onClick: function () {
                try {
                    var itensSelecionados = dijit.byId("gridTitulosRetCnab").itensSelecionados;
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
                    else
                        if (itensSelecionados.length > 1) {
                            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
                        }
                        else {
                            if (itensSelecionados[0].id_tipo_retorno == 1) {
                                eventoEditarBaixaTitulo(itensSelecionados, dojo.xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect);
                                //botões de baixa
                                setarEventosBotoesPrincipaisCadTransacao(dojo.on);
                            }
                            else
                                caixaDialogo(DIALOGO_AVISO, msgErroVerBaixaTituloNaoBaixa, null);
                        }
                } catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menuTitulo.addChild(acaoExcluir);

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
    } catch (e) {
        postGerarLog(e);
    }
}

function processarRetorno(itensSelecionados) {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegProcessamento, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneProcessar, null);
        else {
            showCarregando();
            var mensagensWeb = new Array();
        
            if (mensagensWeb.length > 0)
                apresentaMensagem('apresentadorMensagem', mensagensWeb);

            dojo.xhr.post({
                url: Endereco() + "/relatorio/postProcessarRetornos",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson({
                    cd_retorno_cnab: itensSelecionados[0].cd_retorno_cnab,
                    LocalMovto: { nm_banco: itensSelecionados[0].LocalMovto.nm_banco, cd_local_movto: itensSelecionados[0].cd_local_movto },
                    no_arquivo_retorno: itensSelecionados[0].no_arquivo_retorno
                })
            }).then(function (data) {
                try {
                    if (hasValue(data.retorno)) {
                        dojo.xhr.get({
                            url: data.retorno,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "X-Requested-With": null }
                        }).then(function (data) {
                            try {
                                showCarregando();
                                var gridName = "gridRetornoCNAB";
                                var gridTitulosRetCnab = dijit.byId(gridName);

                                data = jQuery.parseJSON(data);
                                apresentaMensagem('apresentadorMensagem', data);
                                if (hasValue(data) && hasValue(data.retorno)) {
                                    if (data.retorno.id_status_cnab == 2) {
                                        var posicao = binaryObjSearch(gridTitulosRetCnab.itensSelecionados, 'cd_retorno_cnab', data.retorno.cd_retorno_cnab);
                                        if (hasValue(posicao, true)) {
                                            gridTitulosRetCnab.itensSelecionados[posicao].statusRetornoCNAB = "Fechado";
                                            //removeObjSort(gridTitulosRetCnab.itensSelecionados, "cd_retorno_cnab", data.retorno.cd_retorno_cnab);
                                            //insertObjSort(gridTitulosRetCnab.itensSelecionados, "cd_retorno_cnab", data.retorno);
                                            buscarItensSelecionados(gridName, 'selecionadoRetCnab', 'cd_retorno_cnab', 'selecionaTodosRetCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItensCnab');
                                        }
                                    }
                                }
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
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Ocorreu um erro, por favor tente novamente ou contacte o administrador do sistema.");
                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        showCarregando();
                    }
                } catch (e) {
                    showCarregando();
					e.message = "Retorno: " + data + e.message;
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
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
                url: Endereco() + "/api/cnab/getUrlRelatorioRetornoTitulosCNAB?cd_retorno_cnab=" + itensSelecionados[0].cd_retorno_cnab,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioRetornoTitulosCNAB?' + data, '765px', '771px', 'popRelatorio');
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function criarBotoesFK(Memory) {
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
                       
                } catch (e) {
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
                } catch (e) {
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
                            function () {
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
                                pesquisaPessoaFKTitulo();
                            });
                        });
                    else {
                        dojo.byId("selecionaRespFKCnab").value = CADASTRO;
                        abrirPessoaFK();
                    }
                } catch (e) {
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
                } catch (e) {
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


    } catch (e) {
        postGerarLog(e);
    }
}

//Configurar Fks
function abrirPessoaFK() {
    try{
        limparPesquisaPessoaFK();
        //dijit.byId("tipoPessoaFK").set("value", 1);
        //dijit.byId("tipoPessoaFK").set("disabled", true);
        pesquisaPessoaFKTitulo();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        dijit.byId("proPessoa").show();
    } catch (e) {
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
            try{
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&responsavel=false",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            } catch (e) {
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
    } catch (e) {
        postGerarLog(e);
    }
}

function getAllLocais(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_local_movto: data[i].value });
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function getTitulosGrade(data) {
    try{
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_titulo: data[i].cd_titulo });
        return retorno;
    } catch (e) {
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
        } else if (dojo.byId("selecionaAlunoFKCnab").value == EnumTipoConsultaAluno.CADASTRO) {
            dojo.byId("cdAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("descAlunoFKCnab").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limpaAlunoFKCnab').set("disabled", false);
            dijit.byId("proAluno").hide();

            if (hasValue(dojo.byId("selecionaAlunoFKCnab"))) {
                dojo.byId("selecionaAlunoFKCnab").value = 0;
            }
        } else if (dojo.byId("selecionaAlunoFiltroFKCnab").value == EnumTipoConsultaAluno.FILTRO) {
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
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try{
        limparPesquisaAlunoFK();
        dojo.byId('tipoRetornoAlunoFK').value = RETORNOCNAB;
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisarAlunoFK(true);
        dijit.byId("proAluno").show();
    } catch (e) {
        postGerarLog(e);
    }
}

//Fim
function formatCheckBoxRetCnab(value, rowIndex, obj) {
    try{
        var gridName = 'gridRetornoCNAB';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosRetCnab');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_retorno_cnab", grid._by_idx[rowIndex].item.cd_retorno_cnab);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_retorno_cnab', 'selecionadoRetCnab', -1, 'selecionaTodosRetCnab', 'selecionaTodosRetCnab', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_retorno_cnab', 'selecionadoRetCnab', " + rowIndex + ", '" + id + "', 'selecionaTodosRetCnab', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTituloCnab(value, rowIndex, obj) {
    try{
        var gridName = 'gridTitulosRetCnab';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTituloCnab');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo_retorno_cnab", grid._by_idx[rowIndex].item.cd_titulo_retorno_cnab);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_titulo_retorno_cnab', 'selecionadoTituloCnab', -1, 'selecionaTodosTituloCnab', 'selecionaTodosTituloCnab', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_titulo_retorno_cnab', 'selecionadoTituloCnab', " + rowIndex + ", '" + id + "', 'selecionaTodosTituloCnab', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function findIsLoadComponetePesquisaRetCnab() {
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesPesquisaRetCNAB",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            apresentaMensagem("apresentadorMensagem", null);
            if (data != null && data.retorno != null) {
                criarOuCarregarCompFiltering("pesqCarteira", data.retorno.carteirasCnab, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_localMvto', 'no_carteira_completa', FEMININO);
                criarOuCarregarCompFiltering("pesqUsuario", data.retorno.usuarios, "", data.retorno.cd_usuario, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_usuario', 'no_login', MASCULINO);
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function findIsLoadComponetesNovoRetCNAB() {
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesNovoCnab",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try{
            apresentaMensagem("apresentadorMensagemRetornoCNAB", null);
            if (dados.retorno != null && dados.retorno != null) {
                if (hasValue(dados.retorno.carteirasCnab))
                    LoadCarteirasCadastro(dados.retorno.carteirasCnab,null);
                showCarregando();
            }
            if (hasValue(dados.erro)) {
                apresentaMensagem('apresentadorMensagemRetornoCNAB', dados);
                showCarregando();
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemRetornoCNAB", error);
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
            var gridRetornoCNAB = dijit.byId('gridRetornoCNAB');
            apresentaMensagem('apresentadorMensagem', '');
            keepValuesRetCnab(null, gridRetornoCNAB, true);
            IncluirAlterar(0, 'divAlterarRetCnab', 'divIncluirRetCnab', 'divExcluirRetCnab', 'apresentadorMensagemRetornoCNAB', 'divCancelarRetCnab', 'divClearRetCnab');
            dijit.byId("cadRetornoCNAB").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesRetCnab(value, grid, ehLink) {
    try{
        grid.itemSelecionado = null;
        limparRetornoCnab();
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

        if (value.cd_retorno_cnab > 0) {
            showEditRetCnab(value.cd_retorno_cnab);
            grid.itemSelecionado = value;
        }
    } catch (e) {
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
            limparTituloRetornoCnab();
            var gridTitulosRetCnab = dijit.byId('gridTitulosRetCnab');
            gridTitulosRetCnab.itemSelecionado = gridTitulosRetCnab.itensSelecionados[0];
            apresentaMensagem('apresentadorMensagemTituloCnab', '');
            keepValuesTituloRetornoCnab(gridTitulosRetCnab);
            dijit.byId("cadTituloRetornoCnab").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparRetornoCnab() {
    try{
        //clearForm("formRetornoCNAB");
        apresentaMensagem("apresentadorMensagemRetornoCNAB", null);
        dojo.byId("cd_retorno_cnab").value = 0;
        dojo.byId("cadRemessa").value = "";

        //Tag Principal
        dijit.byId("cadStatus").set("value", ABERTO);
        dijit.byId("cadLocal").reset();
        dijit.byId("cadLocal").oldValue = 0;
        dijit.byId("uploader").reset();
        //dijit.byId("cadTipo").reset();
        //Filtros Especiais para aluno
        dojo.byId("cdAlunoFKCnab").value = 0;
        dojo.byId("descAlunoFKCnab").value = "";
        dojo.byId("cdResponsavelFKCnab").value = 0;
        dojo.byId("descResponsavelFKCnab").value = "";
        dijit.byId("nroContratoCnab").reset();

        dojo.byId("cdAlunoFKCnab").value = 0;
        dojo.byId("descAlunoFKCnab").value = "";
        dijit.byId("limpaAlunoFKCnab").set('disabled', true);
        dojo.byId("cdAlunoFKCnab").value = 0;
        dojo.byId("descAlunoFKCnab").value = "";
        dijit.byId("limpaAlunoFKCnab").set('disabled', true);

        dijit.byId("pesqTipoRetonroCad")._onChangeActive = false;
        dijit.byId("pesqTipoRetonroCad").set("value", "");
        dijit.byId("pesqTipoRetonroCad")._onChangeActive = true;
        //dojo.byId("cadRemessaUpload").value = "";
        //Titulos
        var gridTitulosRetCnab = dijit.byId("gridTitulosRetCnab");
        if (hasValue(gridTitulosRetCnab)) {
            gridTitulosRetCnab.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridTitulosRetCnab.itensSelecionados = [];
            gridTitulosRetCnab.listaTitulosRetornoCNAB = [];
        }

        dijit.byId("tagTitulosCnab").set("open", true);
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTitulosCnabGrade() {
    try{
        var grid = dijit.byId("gridTitulosRetCnab");
        var elementNroContrato = dojo.byId("nroContratoCnab");
        var elementAluno = dojo.byId("cdAlunoFKCnab");
        var elementPessoa = dojo.byId("cdResponsavelFKCnab");
        if (grid.listaTitulosRetornoCNAB != null && grid.listaTitulosRetornoCNAB.length > 0) {
            showCarregando();
            dojo.xhr.post({
                url: Endereco() + "/api/cnab/postPesquisaTituloCnabRet",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: dojox.json.ref.toJson({
                    nro_contrato: hasValue(elementNroContrato.value) ? elementNroContrato.value : 0,
                    cd_aluno: hasValue(elementAluno.value) ? elementAluno.value : 0,
                    cd_pessoa: hasValue(elementPessoa.value) ? elementPessoa.value : 0,
                    id_tipo_retorno: hasValue(dijit.byId("pesqTipoRetonroCad").value) && dijit.byId("pesqTipoRetonroCad").value > 0 ? dijit.byId("pesqTipoRetonroCad").value : 0,
                    titulosGradeRet: grid.listaTitulosRetornoCNAB
                })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data).retorno;
                    grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data }) }));
                } catch (e) {
                    postGerarLog(e);
                }
                showCarregando();
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemRetornoCNAB", error);
                showCarregando();
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//Metodos C.R.U.D Cnab

function showEditRetCnab(cdRetCnab) {
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getComponentesByRetCnabEdit?cd_retorno=" + cdRetCnab,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try{
            apresentaMensagem("apresentadorMensagemRetornoCNAB", null);
            if (dados.retorno != null && dados.retorno != null) {
                if (hasValue(dados.retorno.carteirasCnab))
                    LoadCarteirasCadastro(dados.retorno.carteirasCnab, dados.retorno.cd_carteira_cnab);
                criarOuCarregarCompFilteringBanco("cadLocal", dados.retorno.carteirasRetornoCNAB, "", dados.retorno.cd_carteira_cnab, dojo.ready, dojo.store.Memory,
                                                  dijit.form.FilteringSelect, 'cd_localMvto', 'no_carteira_completa');
                loadDataRetCnab(dados.retorno);
            }
            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemRetornoCNAB", error);
        showCarregando();
    });
}

function loadDataRetCnab(data) {
    try{
        var compCarteira = dijit.byId("cadLocal");
        //Tag Principal
        dojo.byId("cd_retorno_cnab").value = data.cd_retorno_cnab;
        compCarteira._onChangeActive = false;
        compCarteira.set("value", data.cd_local_movto);
        compCarteira.oldValue = data.cd_local_movto;
        compCarteira._onChangeActive = true;
        dijit.byId("cadStatus").set("value", data.id_status_cnab);

        dojo.byId("cadRemessa")._onChangeActive = false;

        dojo.byId("cadRemessa").value = data.no_arquivo_retorno;
        dojo.byId("cadRemessa")._onChangeActive = true;
        dojo.byId("cadLinhasRetorno").value = data.nm_linhas_retorno;
        dojo.byId("cadQtdTitulosRetorno").value = data.nm_titulos_gerados;
        if (data.id_status_cnab == FECHADO) {
            dojo.byId("tdBtnDown").style.display = "";
            dojo.byId("tdUploader").style.display = "none";
        }
        else {
            dojo.byId("tdBtnDown").style.display = "";
            dojo.byId("tdUploader").style.display = "";
        }
        //Titulos
        var gridTitulosRetCnab = dijit.byId("gridTitulosRetCnab");
        if (hasValue(data.TitulosRetornoCNAB)) {
            gridTitulosRetCnab.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.TitulosRetornoCNAB }) }));
            gridTitulosRetCnab.itensSelecionados = [];
            gridTitulosRetCnab.listaTitulosRetornoCNAB = data.TitulosRetornoCNAB;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarRetornoCnab(limparItens) {
    try{
        var pesqDtaInicial = dijit.byId("pesqDtaInicial");
        var pesqDtaFinal = dijit.byId("pesqDtaFinal");
    
        if (hasValue(dojo.byId("pesqDtaInicial").value) && hasValue(dojo.byId("pesqDtaFinal").value) && dojo.date.compare(pesqDtaInicial.get("value"), pesqDtaFinal.value) > 0) {
            var mensagensWeb = new Array();
            var mensagemErro = "";
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemErro);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            pesqDtaFinal._onChangeActive = false;
            pesqDtaFinal.reset();
            pesqDtaFinal._onChangeActive = true;
            return false;
        }
        var grid = dijit.byId("gridRetornoCNAB");
        var cdCarteira = hasValue(dijit.byId("pesqCarteira").value) ? dijit.byId("pesqCarteira").value : 0;
        var cdUsuario = hasValue(dijit.byId("pesqUsuario").value) ? dijit.byId("pesqUsuario").value : 0;
        var cdStatus = hasValue(dijit.byId("pesqStatus").value) ? dijit.byId("pesqStatus").value : 0;
        var nossoNum = hasValue(dijit.byId("pesNossoNumero").value) ? dijit.byId("pesNossoNumero").value : "";
        var cdResponsavelFiltro = (hasValue(dojo.byId("cdResponsavelFiltroFKCnab").value)) ? dojo.byId("cdResponsavelFiltroFKCnab").value : 0;
        var cdAlunoFiltro = (hasValue(dojo.byId("cdAlunoFiltroFKCnab").value)) ? dojo.byId("cdAlunoFiltroFKCnab").value : 0;
        var myStore =
            dojo.store.Cache(
                    dojo.store.JsonRest({
                        target: Endereco() + "/api/cnab/getRetornoCNABSearch?cd_carteira=" + parseInt(cdCarteira) + "&cd_usuario=" + parseInt(cdUsuario) + "&descRetorno=" + dijit.byId("retornoPesq").value +
                                   "&status=" + parseInt(cdStatus) + "&dtInicial=" + dojo.byId("pesqDtaInicial").value + "&dtFinal=" + dojo.byId("pesqDtaFinal").value + "&nossoNumero=" + nossoNum + 
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
    } catch (e) {
        postGerarLog(e);
    }
}

function validarCnab() {
    try{
        var validado = true;
        if (!dijit.byId("formRetornoCNAB").validate()) {
            validado = false;
        }
        return validado;
    } catch (e) {
        postGerarLog(e);
    }
}

function salvarRetornoCNAB(retornoTemporario) {
    try{
        var validado = true;
        var retornoCNAB = null;
        if (!validarCnab())
            return false;

        showCarregando();
        retornoCNAB = mountDataRetCNABForPost(retornoTemporario);
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postInsertRetornoCNAB",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(retornoCNAB)
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridRetornoCNAB';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_retorno_cnab", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoRetCnab', 'cd_retorno_cnab', 'selecionaTodosRetCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItensCnab');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_retorno_cnab");
                    showCarregando();
                    dijit.byId("cadRetornoCNAB").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemRetornoCNAB', data);
                    showCarregando();
                }
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemRetornoCNAB', error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarRetCnab(retornoTemporario) {
    try{
        var cnab = null;
        if (!validarCnab())
            return false;
        showCarregando();
        cnab = mountDataRetCNABForPost(retornoTemporario);
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postUpdateRetornoCNAB",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(cnab)
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridRetornoCNAB';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_retorno_cnab", dojo.byId("cd_retorno_cnab").value);
                    insertObjSort(grid.itensSelecionados, "cd_retorno_cnab", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoRetCnab', 'cd_retorno_cnab', 'selecionaTodosRetCnab', ['pesquisarCnabBoletos', 'relatorioCnab'], 'todosItensCnab');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_retorno_cnab");
                    dijit.byId("cadRetornoCNAB").hide();
                }
                else
                    apresentaMensagem('apresentadorMensagemRetornoCNAB', data);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemRetornoCNAB', error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
    }
};

function mountDataRetCNABForPost(retornoTemporario) {
    try{
        var cd_carteira = 0;

        if (dijit.byId("cadLocal").value > 0) {
            var storeCarteira = dijit.byId("cadLocal").store.data;
            quickSortObj(storeCarteira, 'id');
            var posicao = binaryObjSearch(storeCarteira, 'id', parseInt(dijit.byId("cadLocal").value));
            cd_carteira = storeCarteira[posicao].cd_carteira_cnab;
        }

        var retorno = {
            cd_retorno_cnab: hasValue(dojo.byId("cd_retorno_cnab").value) ? dojo.byId("cd_retorno_cnab").value : 0,
            cd_local_movto: dijit.byId("cadLocal").value,
            id_status_cnab: dijit.byId("cadStatus").get("value"),
            no_arquivo_retorno: dojo.byId("cadRemessa").value,
            cd_carteira_retorno_cnab: cd_carteira,
            retornoTemporario: retornoTemporario,
            nm_banco: hasValue(dijit.byId("cadLocal").item)? dijit.byId("cadLocal").item.nm_banco: 0
        }
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverCnab(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else {
            if (itensSelecionados.length > 1) //É preciso selecionar apenas um registro pois é complicado manter a transação de arquivos junto a vários registros.
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            else
                caixaDialogo(DIALOGO_CONFIRMAR, '', function () {
                    deletarRetornoCnabs(itensSelecionados);
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function deletarRetornoCnabs(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId("cd_retorno_cnab").value != 0)
                itensSelecionados = [{
                    cd_retorno_cnab: dojo.byId("cd_retorno_cnab").value
                }];
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/cnab/postDeleteRetornoCnab",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridRetornoCNAB').itensSelecionados, "cd_retorno_cnab", itensSelecionados[r].cd_retorno_cnab);
                pesquisarRetornoCnab(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarCnabBoletos").set('disabled', false);
                dijit.byId("relatorioCnab").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadRetornoCNAB").hide();
                showCarregando();

            } catch (e) {
                showCarregando();

                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();

            if (!hasValue(dojo.byId("cadRetornoCNAB").style.display))
                apresentaMensagem('apresentadorMensagemRetornoCNAB', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        showCarregando();

        postGerarLog(e);
    }
}

function LoadCarteirasCadastro(data, value) {
    try{
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

        if (value != null && hasValue(value, true)) {
            dijit.byId("cadLocal").set("value", value);
            dijit.byId("cadLocal").oldValue = value;
        }
    } catch (e) {
        postGerarLog(e);
    }
};

function crudRetornoEArquivo(tipoOperacao) {
    try{
        if (!validarCnab()) {
            return false;
        }
        var files = dijit.byId("uploader")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("file" + i, files[i]);
                }
                if (hasValue(dijit.byId("cadLocal").item)) {
                    data.append("nm_banco", dijit.byId("cadLocal").item.nm_banco);
                } else {
                    data.append("nm_banco", 0);
                }
                
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/cnab/UploadRetorno",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (results) {
                        if (tipoOperacao == CADASTRO)
                            salvarRetornoCNAB(results);
                        else
                            alterarRetCnab(results);
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemRetornoCNAB', error);
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemRetornoCNAB', mensagensWeb);
            }
        } else {
            if (tipoOperacao == CADASTRO)
                salvarRetornoCNAB("");
            else
                alterarRetCnab("");
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparTituloRetornoCnab() {
    try{
        clearForm("formTituloRetornoCnab");
        apresentaMensagem("apresentadorMensagemTituloRetornoCnab", null);
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesTituloRetornoCnab(grid) {
    try{
        limparTituloRetornoCnab();
        value = grid.itemSelecionado;
        showEditTituloRetornoCnab(value);
    } catch (e) {
        postGerarLog(e);
    }
}

function showEditTituloRetornoCnab(value) {
    showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/cnab/getTituloRetornoCnabEdit?cd_titulo_retorno_cnab=" + value.cd_titulo_retorno_cnab,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try{
            apresentaMensagem("apresentadorMensagemTituloCnab", null);
            if (dados.retorno != null && dados.retorno != null)
                loadDataTituloRetornoCnab(dados.retorno, value);
            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemTituloCnab", error);
        showCarregando();
    });
}

function loadDataTituloRetornoCnab(itemTituloCnabBase, itemTituloCnabView) {
    //Tag Titulo
    try{
        dojo.byId("responsavelTituloCnabReg").value = itemTituloCnabView.nomeResponsavel;
        dojo.byId("responsavelTituloCnabReg").value = itemTituloCnabView.nomeResponsavel;

        dojo.byId("codigoTituloCnabReg").value = itemTituloCnabView.cd_titulo;
        dojo.byId("dtaEmissaoTituloCnabReg").value = itemTituloCnabView.dt_emissao;
        dojo.byId("dtaVencTituloCnabReg").value = itemTituloCnabView.dt_vcto;
        dojo.byId("localMvtoTituloEdit").value = itemTituloCnabView.descLocalMovtoTitulo;
        

        if (hasValue(itemTituloCnabView.Titulo)) {
            dojo.byId("vlTituloCnabReg").value = itemTituloCnabView.Titulo.vlTitulo;
            dojo.byId("statusTituloCanbReg").value = itemTituloCnabView.Titulo.statusCnabTitulo;
            dojo.byId("nossoNroTituloCnabReg").value = itemTituloCnabView.Titulo.dc_nosso_numero;
            
        }

        if (itemTituloCnabBase != null) {
            dojo.byId("descObsTituloCnabDet").value = itemTituloCnabBase.tx_mensagem_retorno;
            dojo.byId("pessoaTituloCnabReg").value = itemTituloCnabBase.nomePessoaTitulo;
            if (hasValue(itemTituloCnabView.Titulo)) {
                dojo.byId("nrParcelaTituloCnabReg").value = itemTituloCnabBase.Titulo.nm_parcela_titulo;
                dojo.byId("numTituloCnab").value = itemTituloCnabBase.Titulo.nm_titulo;
            }
            //tag título cnab
            dojo.byId("dataTipo").value = itemTituloCnabBase.dt_banco;
            dojo.byId("jurosTituloCnabDet").value = itemTituloCnabBase.vlJuros;
            dojo.byId("edMulta").value = itemTituloCnabBase.vlMulta;
            dojo.byId("descTituloCnabDet").value = itemTituloCnabBase.vlDesconto;
            dijit.byId('vlBaixa').set('value', itemTituloCnabBase.vl_baixa_retorno);
            dojo.byId('tipoRetorno').value = itemTituloCnabBase.tipoRetornoCNAB;
            dojo.byId("nossoNroTituloCnabDet").value = itemTituloCnabBase.dc_nosso_numero;

            // Despesa CNAB
            var gridDespesa = dijit.byId("gridDespesa");
            gridDespesa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemTituloCnabBase.DespesaTituloCnab }) }));
            gridDespesa.startup();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function setarEventosBotoesPrincipaisCadTransacao(on) {
    try{
        dojo.byId("divIncluirBaixa").style.display = "none";
        dojo.byId("divAlterarBaixa").style.display = "none";
        dojo.byId("divExcluirBaixa").style.display = "none";
        dojo.byId("divCancelarBaixa").style.display = "none";
        dojo.byId("divLimparBaixa").style.display = "none";

        dijit.byId("fecharBaixa").on("click", function () {
            dijit.byId("cadBaixaFinanceira").hide();
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoExcluirRetornosProcessados(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelecioneRegExcluir, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelecioneApenasUmRegistro, null);
        else {
            showCarregando();
            var cnabs = [];
            for (var i = 0; i < itensSelecionados.length; i++) {
                cnabs.push(itensSelecionados[i].cd_retorno_cnab);
            }
            dojo.xhr.post({
                url: Endereco() + "/api/cnab/deleteCnabRetornosProcessados",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson(cnabs)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItens_label");
                    apresentaMensagem('apresentadorMensagem', data);
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridRetornoCNAB').itensSelecionados, "cd_retorno_cnab", itensSelecionados[r].cd_retorno_cnab);
                    pesquisarRetornoCnab(false);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCnabBoletos").set('disabled', false);
                    dijit.byId("relatorioCnab").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    dijit.byId("cadRetornoCNAB").hide();

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
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}