//#region  variáveis

var DESISTENCIA_PESQUISA = 7; DESISTENCIA_CAD = 8; LIQUIDAR_TITULOS = 1, DESISTENCIA_PES = 10, CANCELA_DESISTENCIA = 11;
var DESISTENCIA_CONSISTIR = 1; CANCELAMENTO = 2;
var TURMAS_ANDAMENTO = 1;
var SITUACAODESISTENTE = 2, SITUACAOAGUARDANDO = 9, SITUACAONAOREM = 10, SITUACAOATIVO = 1;
var LIQUIDACAO_CANCELAMENTO = 6, LIQUIDACAO_BOLSA = 100;
var TP_DESISTENCIA = 1, TP_CANCELAMENTO = 2;
var TIPO_DESI = new Array(
       { name: "Desistência", id: "1" },
       { name: "Cancelamento", id: "2" }
    );
var TIPONOVO = new Array(
       { name: "Desistência", id: "1" },
       { name: "Cancelamento", id: "2" });

var DESEJO_BAIXA = new Array(
      { name: "Deixar todos os títulos em aberto", id: "0" },
      { name: "Liquidar os títulos selecionados", id: "1" }
    );
var TIPOFINANCHEQUE = 4, CHEQUEPREDATADO_DESISTENCIA = 4, CHEQUEVISTA_DESISTENCIA = 10, EVENTO_GRID_CHEQUE = 0; isViewChequeTransacao = null;

function formatCheckBoxTitulo(value, rowIndex, obj) {
    try {
        var gridName = 'gridTituloDesistencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTitulos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_titulo", grid._by_idx[rowIndex].item.cd_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        setTimeout("configuraCheckBoxTitulosDesistencia(" + value + ", 'cd_titulo', 'selecionadoTitulo', " + rowIndex + ", '" + id + "', 'selecionaTodosTitulos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region  monta os drops
function returnMotivoDesistenciaCad(isCancelamento) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getMotivoDesistenciaByCancelamento?isCancelamento=" + isCancelamento,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            data = $.parseJSON(data);
            loadTipoDesistenciaCad(data.retorno);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}

function loadTipoDesistenciaCad(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbMotivo = dijit.byId('cbMotivoCad');
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_motivo_desistencia, name: value.dc_motivo_desistencia });
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

function loadTipo() {
    var tipo = true;
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbTipo = dijit.byId('cbTipoCad');
            Array.forEach(TIPO_DESI, function (value, i) {
                itemsCb.push({ id: value.id, name: value.name });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipo.store = stateStore;
            dijit.byId(cbTipo)._onChangeActive = false;
            dijit.byId(cbTipo).set('value', 1);
            dijit.byId(cbTipo)._onChangeActive = true;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTipoNovo() {
    var tipo = true;
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbTipo = dijit.byId('cbTipoCad');
            Array.forEach(TIPONOVO, function (value, i) {
                itemsCb.push({ id: value.id, name: value.name });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipo.store = stateStore;
            dijit.byId(cbTipo).set('value', 1);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadDesejoBaixa() {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        var itemsCb = [];
        var cbEscolha = dijit.byId('cbEscolhaCad');
        Array.forEach(DESEJO_BAIXA, function (value, i) {
            itemsCb.push({ id: value.id, name: value.name });
        });
        var stateStore = new Memory({
            data: itemsCb
        });
        cbEscolha.store = stateStore;
    });
}

function returnLocalMovimento(cd_aluno, cd_contrato, Permissoes) {
    require(["dojo/_base/xhr"], function (xhr) {
        if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getLocalMovtoAndTipoLiquidacaoGeral?cd_pessoa_titulo=" + cd_aluno + "&cd_contrato=" + cd_contrato,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    data = $.parseJSON(data);
                    if (data.titulos.length > 0) {
                        dijit.byId("tabContainerDesistencia_tablist_pnTitulo").domNode.style.visibility = '';
                        loadLocalMovimento(data.locaMovto);
                        loadTipoLiquidacao(data.tipoLiquidacao);
                        montarGridTitulos(data.titulos);
                    }
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        }
        else
            xhr.get({
                url: Endereco() + "/api/financeiro/getLocalMovtoAndTipoLiquidacao?cd_pessoa_responsavel=" + cd_aluno + "&cd_contrato=" + cd_contrato,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    data = $.parseJSON(data);
                    if (data.titulos.length > 0) {
                        dijit.byId("tabContainerDesistencia_tablist_pnTitulo").domNode.style.visibility = '';
                        loadLocalMovimento(data.locaMovto);
                        loadTipoLiquidacao(data.tipoLiquidacao);
                        montarGridTitulos(data.titulos);
                    }
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
    });
}

function loadLocalMovimento(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbMotivo = dijit.byId('des_local');
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
            var cbTipoLiquidacao = dijit.byId('cbLiquidacaoCad');
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

//#endregion

//#region  funções auxiliares - abrirCadastroDesistencia - limparDropsDowns  - montarGridTitulos

function abrirCadastroDesistencia() {
    try {
        loadBancoCadCheque(null);
        dijit.byId('cbTipoCad').set('disabled', false);
        IncluirAlterar(1, 'divAlterarDesistencia', 'divIncluirDesistencia', 'divExcluirDesistencia', 'apresentadorMensagemDesistencia', 'divCancelarDesistencia', 'divClearDesistencia');
        // *MMC* returnMotivoDesistenciaCad();
        dijit.byId("cadDesistencia").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadBancoCadCheque(funcao) {
    try {
        showCarregando();
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getAllBanco",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataBanco) {
            try {
                if (hasValue(dataBanco.retorno))
                    criarOuCarregarCompFiltering("bancoChequeDesistencia", dataBanco.retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');
                if (hasValue(funcao))
                    funcao.call();
                showCarregando();
            } catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemMat', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparDropsDowns() {
    try{
        dijit.byId('des_local').reset();
        dijit.byId('cbLiquidacaoCad').reset();
        dijit.byId('des_local').store.data = [];
        dijit.byId('cbLiquidacaoCad').store.data = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadastro() {
    try{
        apresentaMensagem("apresentadorMensagemCadDesistencia", '');
        clearForm('formDesistencia');
        dojo.byId('des_obs').value = '';
        dijit.byId('cbTipoCad')._onChangeActive = false;
        dijit.byId('cbTipoCad').set('value', TP_DESISTENCIA);
        dijit.byId('cbTipoCad')._onChangeActive = true;
        //dijit.byId('pesAlunoFKCad').set("disabled", true);
        dijit.byId('limparTurmaCad').set("disabled", true);
        dijit.byId('limparAlunoCad').set("disabled", true);
        dijit.byId("ckAlunoAtivo").set("disabled", false);
        dijit.byId("ckAlunoAtivo").set("checked", false);
        dijit.byId("tabContainerDesistencia_tablist_pnTitulo").domNode.style.visibility = 'hidden';

        dojo.byId("cdAlunoDesistencia").value = 0;
        dojo.byId("cdTurmaCad").value = 0;

        require(["dojo/store/Memory",  "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            dijit.byId("gridTituloDesistencia").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            dijit.byId("gridTituloDesistencia").itensSelecionados = null;
            dijit.byId("gridTituloDesistencia").update();
        });

        var tabs = dijit.byId("tabContainerDesistencia");
        var pane = dijit.byId("tabPrincipal");
        tabs.selectChild(pane);

        dijit.byId('cbEscolhaCad').reset();
        limparDropsDowns();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba
        dijit.byId("tabContainerDesistencia_tablist_pnTitulo").domNode.style.visibility = 'hidden';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarDesistenciaPartial(item, gridDesistencia, xhr, ready, Memory, FilteringSelect) {
    try {
        var isCancelamentoFilter = false;
        if (item != null)
            if (item.id_tipo_desistencia == CANCELAMENTO && item.id_cancelamento)
                isCancelamentoFilter = true;
            else
                isCancelamentoFilter = false;
        else
            if (gridDesistencia != null) {
                var grid = gridDesistencia.selection.getSelected()[0];
                if (hasValue(grid) && grid.id_tipo_desistencia == CANCELAMENTO && grid.id_cancelamento)
                    isCancelamentoFilter = true;
                else
                    isCancelamentoFilter = false;
            }
        loadTipo();
        xhr.get({
            url: Endereco() + "/api/coordenacao/getMotivoDesistenciaByCancelamento?isCancelamento=" + isCancelamentoFilter,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                data = $.parseJSON(data);
                loadTipoDesistenciaCad(data.retorno);
                selecionaTab('span#tabContainer_tablist_tabPrincipal.tabLabel');
                limparCadastro();
                keepValuesDesitencia(item, gridDesistencia, false, xhr, ready, Memory, FilteringSelect);
                IncluirAlterar(0, 'divAlterarDesistencia', 'divIncluirDesistencia', 'divExcluirDesistencia', 'apresentadorMensagemDesistencia', 'divCancelarDesistencia', 'divClearDesistencia');
                dijit.byId("cadDesistencia").show();
            }
            catch (er) {
                postGerarLog(er);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemDesistencia', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGridTitulos(dataTitulos) {
    obrigarOpcaoTitulo(dataTitulos)
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
    function (Memory, ObjectStore) {
        dijit.byId("gridTituloDesistencia").setStore(new ObjectStore({ objectStore: new Memory({ data: dataTitulos }) }));
        dijit.byId("gridTituloDesistencia").update();
    });
}

function obrigarOpcaoTitulo(titulos) {
    try{
        apresentaMensagem("apresentadorMensagemCadDesistencia", '');
        if (titulos != null && titulos.length > 0)
            dijit.byId("cbEscolhaCad").set("required", true);
        else
            dijit.byId("cbEscolhaCad").set("required", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCamposTitulos(titulos, windowUtils) {
    try{
        var validado = true;
        if (!dijit.byId("formDesistencia").validate()) {
            validado = false;
            var tabs = dijit.byId("tabContainerDesistencia");
            var pane = dijit.byId("tabPrincipal");
            tabs.selectChild(pane);
        }

        if (titulos != null && titulos.length > 0) {
            if (!hasValue(dojo.byId("cbEscolhaCad").value) || dijit.byId('cbEscolhaCad').get('value') == LIQUIDAR_TITULOS) {
                dijit.byId("cbEscolhaCad").set("required", true);
                if (dijit.byId("cbLiquidacaoCad").value == LIQUIDACAO_CANCELAMENTO)
                    dijit.byId('des_local').set('required', false);
                else
                    dijit.byId('des_local').set('required', true);
                dijit.byId('cbLiquidacaoCad').set('required', true);

                var desLocal = dijit.byId('des_local');
                var liquidacao = dijit.byId('cbLiquidacaoCad');
                var escolhaTitulo = dijit.byId("cbEscolhaCad");

                if (!dijit.byId("formLiquidarTitulos").validate())
                    validado = false;


                //if (!desLocal.validate()) {
                //    validado = false;
                //    mostrarMensagemCampoValidado(windowUtils, desLocal);
                //}
                //if (!liquidacao.validate()) {
                //    validado = false;
                //    mostrarMensagemCampoValidado(windowUtils, liquidacao);
                //}

                if (!escolhaTitulo.validate()) {
                    mostrarMensagemCampoValidado(windowUtils, escolhaTitulo);

                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDesistenciaOpcaoTitulos);
                    apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);

                    var tabs = dijit.byId("tabContainerDesistencia");
                    var pane = dijit.byId("pnTitulo");
                    tabs.selectChild(pane);

                    validado = false;
                }

            } else {
                dijit.byId("cbEscolhaCad").set("required", false);
                dijit.byId('des_local').set('required', false);
                dijit.byId('cbLiquidacaoCad').set('required', false);
                validado = true;
            }
        }
        else {
            dijit.byId("cbEscolhaCad").set("required", false);
            dijit.byId('des_local').set('required', false);
            dijit.byId('cbLiquidacaoCad').set('required', false);
            apresentaMensagem("apresentadorMensagemCadDesistencia", '');
            validado = true;
        }

        if (!dijit.byId("formDesistencia").validate())
            validado = false;

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region montarCadastroDesistencia
function montarCadastroDesistencia(funcao, Permissoes) {
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
    "dijit/Dialog"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, FilteringSelect) {
        ready(function () {        
            try{
                var gridTituloDesistencia = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodosTitulos' style='display:none'/>", field: "selecionadoTitulo", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTitulo },
                        { name: "Número", field: "nm_titulo", width: "10%", styles: "min-width:180px;text-align: center;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "8%", styles: "min-width:180px; text-align: center;" },
                        { name: "Tipo Doc.", field: "tipoDoc", width: "8%", styles: "text-align: center;" },
                        { name: "Emissão", field: "dt_emissao", width: "10%", styles: "min-width:180px;text-align: center;" },
                        { name: "Vencto.", field: "dt_vcto", width: "10%", styles: "min-width:180px;text-align: center;" },
                        { name: "Valor", field: "vlTitulo", width: "10%", styles: "min-width:180px; text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "10%", styles: "min-width:180px; text-align: right  ;" },
                        {
                            name: "Cheque",
                            field: "_item",
                            width: '10%',
                            styles: "text-align: center;",
                            formatter: function (item) {
                                var label = "Adicionar";
                        
                                if (hasValue(item.Cheque) && hasValue(item.Cheque.dt_bom_para))
                                    label = "Alterar";
                        
                                if (hasValue(dijit.byId(item.cd_titulo + "_" + item.nm_parcela_titulo))) {
                                    dijit.byId(item.cd_titulo + "_" + item.nm_parcela_titulo).destroy();
                                }
                        
                                var btn = new dijit.form.Button({
                                    label: label,
                                    id: item.cd_titulo + "_" + item.nm_parcela_titulo,
                                    disabled: desabilitarBotaoIncluirCheque(null, null, true),
                                    onClick: function () {
                                        try {
                                            dijit.byId('proChequeFK').set('title', 'Dados do cheque do Título: ' + item.nm_titulo + ' Parcela ' + item.nm_parcela_titulo);
                                            loadBancoViewCheque(item.Cheque.cd_banco, item, true);
                                        } catch (e) {
                        
                                        }
                                    }
                                });
                                setTimeout("alterarTamnhoBotaoDesistencia('" + btn.id + "')", 15);
                                return btn;
                            }
                        }
                    ],
                    canSort: true,
                    selectionMode: "single"
                }, "gridTituloDesistencia");
                gridTituloDesistencia.startup();
                gridTituloDesistencia.layout.setColumnVisibility(8, false);
                gridTituloDesistencia.on("RowClick", function (evt) {
                    EVENTO_GRID_CHEQUE = evt;
                }, true);

                //Botões de crud
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        incluirDesistencia(Permissoes);
                    }
                }, "incluirDesistencia");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        showCarregando();
                        keepValuesDesitencia(null, dijit.byId('gridDesistencia'), false, xhr, ready, Memory, FilteringSelect);
                    }
                }, "cancelarDesistencia");

                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { dijit.byId("cadDesistencia").hide(); } }, "fecharDesistencia");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        editarDesistencia(Permissoes);
                    }
                }, "alterarDesistencia");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarDesistencia();
                        });
                    }
                }, "deleteDesistencia");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparCadastro();
                    }
                }, "limparDesistencia");
                //Fim

                //botoes de fk
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', disabled: true,
                    onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                            montarGridPesquisaAluno(false, function () {
                                abrirAlunoCadFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                            });
                        }
                        else
                            abrirAlunoCadFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                    }
                }, "pesAlunoFKCad");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try{
                            dojo.byId("nome_aluno").value = "";
                            dojo.byId('cdAlunoCad').value = 0;
                            dojo.byId('cdAlunoDesistencia').value = 0;
                            dojo.byId("cdResponsavelCto").value = 0;
                            dijit.byId('limparAlunoCad').set("disabled", true);
                            dijit.byId("tabContainerDesistencia_tablist_pnTitulo").domNode.style.visibility = 'hidden';
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAlunoCad");

                //Botão de pesquisa da turma
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        apresentaMensagem("apresentadorMensagemCadDesistencia", '');
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                abrirTurmaFKDesistencia();
                            });
                        else
                            abrirTurmaFKDesistencia();

                    }
                }, "pesTurmaFKCadDesistencia");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        try{
                            dojo.byId("cdTurmaCad").value = 0;
                            dojo.byId("nome_turma").value = "";
                            dojo.byId("nome_aluno").value = "";
                            dojo.byId('cdAlunoCad').value = 0;
                            dojo.byId('cdAlunoDesistencia').value = 0;
                            dojo.byId("cdResponsavelCto").value = 0;
                            dijit.byId("ckAlunoAtivo").set("checked", false);
                            dijit.byId('limparTurmaCad').set("disabled", true);
                            dijit.byId('limparAlunoCad').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparTurmaCad");

                //Altera o tamanho dos botões
                var buttonFkArray = ['pesTurmaFKCadDesistencia', 'pesAlunoFKCad'];
                diminuirBotoes(buttonFkArray);

                if (hasValue(document.getElementById("limparAlunoCad"))) {
                    document.getElementById("limparAlunoCad").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAlunoCad").parentNode.style.width = '40px';
                }

                if (hasValue(document.getElementById("limparTurmaCad"))) {
                    document.getElementById("limparTurmaCad").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurmaCad").parentNode.style.width = '40px';
                }

                //eventtos
                dijit.byId("cbEscolhaCad").on("change", function (e) {
                    if (e == LIQUIDAR_TITULOS) {
                        try{
                            dojo.byId('localMovto').style.display = '';
                            dijit.byId('des_local').set('required', true);
                            dijit.byId('cbLiquidacaoCad').set('required', true);
                            var cd_aluno = dojo.byId("cdResponsavelCto").value;
                            var cd_contrato = dojo.byId("cdContrato").value;
                            apresentaMensagem("apresentadorMensagemCadDesistencia", "");
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    } else {
                        try{
                            dojo.byId('localMovto').style.display = 'none';
                            dijit.byId('des_local').set('required', false);
                            dijit.byId('cbLiquidacaoCad').set('required', false);
                            apresentaMensagem("apresentadorMensagemCadDesistencia", "");
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                dijit.byId("cbLiquidacaoCad").on("change", function (e) {

                    var compCbLocalCad = dijit.byId("des_local");
                    if (hasValue(dijit.byId('gridBaixaCad')))
                        dijit.byId('gridBaixaCad').update();
                    if (e == LIQUIDACAO_CANCELAMENTO || e == LIQUIDACAO_BOLSA) {
                        compCbLocalCad.reset();
                        compCbLocalCad.set("disabled", true);
                        compCbLocalCad.set("required", false);
                    } else {
                        validarTipoLiqChequeTransacaoDesistencia(e);
                        compCbLocalCad.set("disabled", false);
                        compCbLocalCad.set("required", true);
                    }
                });
                //loads para drops 
                loadTipo();
                loadDesejoBaixa();

                dijit.byId("cbTipoCad").on("change", function (e) {
                    dijit.byId('cbMotivoCad').reset();
                    if (parseInt(e) == CANCELAMENTO) returnMotivoDesistenciaCad(true);
                    if (parseInt(e) == DESISTENCIA_CONSISTIR) returnMotivoDesistenciaCad(false);
                    if (parseInt(e) == TP_CANCELAMENTO) {
                        dijit.byId("ckAlunoAtivo").set("checked", true);
                        dijit.byId("ckAlunoAtivo").set("disabled", true);
                    }
                    else {
                        dijit.byId("ckAlunoAtivo").set("checked", false);
                        dijit.byId("ckAlunoAtivo").set("disabled", false);
                    }

                });
                //funções particulares
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function configuraCheckBoxTitulosDesistencia(value, field, fieldTodos, rowIndex, id, idTodos, gridName, disabled) {
    require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
        ready(function () {
            var dojoId = dojo.byId(id);
            var grid = dijit.byId(gridName);

            if (hasValue(grid)) {
                if (id != idTodos || (hasValue(grid.pagination) && !grid.pagination.plugin._showAll)) {

                    // Se id for seleciona todos, verifica se todos estão marcados para marcá-lo:
                    if (id == idTodos) {
                        var j = 0;
                        var campo = dojo.byId(fieldTodos + '_Selected_' + j);
                        value = hasValue(campo);

                        while (hasValue(campo) && value) {
                            if (campo.type == 'text') {
                                setTimeout("configuraCheckBox(" + value + ", '" + field + "', '" + fieldTodos + "', " + rowIndex + ", '" + id + "', '" + idTodos + "', '" + gridName + "')", grid.rowsPerPage * 3);
                                return;
                            }
                            else {
                                value = value && campo.checked;
                                j += 1;
                                campo = dojo.byId(fieldTodos + '_Selected_' + j);
                            }
                        }
                    }

                    if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                        dijit.byId(id).destroy();
                    if (value == undefined)
                        value = false;
                    if (disabled == null || disabled == undefined) disabled = false;
                    if (hasValue(dojoId) && dojoId.type == 'text')
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            checked: value,
                            disabled: disabled,
                            onChange: function (b) { checkBoxChangeTitulosDesistencia(rowIndex, field, fieldTodos, idTodos, this, grid); }
                        }, id);
                }
                else if (hasValue(dojo.byId(idTodos)))
                    dojo.byId(idTodos).parentNode.removeChild(dojo.byId(idTodos));

            }
        })
    });
}

/* Funções para componentização de checkbox na grid paginada sob demanda: */
function checkBoxChangeTitulosDesistencia(rowIndex, field, fieldTodos, idTodos, obj, grid) {
    var itemTodos = dijit.byId(idTodos);

    if (rowIndex != -1 && hasValue(grid.getItem(rowIndex))) {
        if (!hasValue(grid.itensSelecionados))
            grid.itensSelecionados = [];

        if (obj.checked) {
            insertObjSort(grid.itensSelecionados, field, grid.getItem(rowIndex));
            verificaMarcacaoTodos(function () { marcaItem(itemTodos, true); }, fieldTodos, grid);

            //função
            desabilitarBotaoIncluirCheque(obj.checked, grid.getItem(rowIndex), false);
            validarTipoLiqChequeTransacaoDesistencia(TIPOFINANCHEQUE);
        }
        else {
            //função
            desabilitarBotaoIncluirCheque(obj.checked, grid.getItem(rowIndex), false);
            validarTipoLiqChequeTransacaoDesistencia(TIPOFINANCHEQUE);
            removeObjSort(grid.itensSelecionados, field, eval('grid.getItem(rowIndex).' + field));
            marcaItem(itemTodos, false);
        }
    }
    else
        // Checa todos:
        selecionaTodos(fieldTodos, itemTodos.checked);
}


//#endregion

//#region  FKS
function abrirTurmaFKDesistencia() {
    try{
        dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value = DESISTENCIA_CAD;
        dojo.byId("trAluno").style.display = "none";
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
        dojo.byId('tipoRetornoAlunoFK').value = tipoRetornoAlunoFK;
        dojo.byId("idOrigemCadastro").value = DESISTENCIA;
        dijit.byId("pesSituacaoFK").costumizado = true;
        loadSituacaoTurmaFKAbertas(dojo.store.Memory);
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId('pesProfessorFK').value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else
                dijit.byId('pesProfessorFK').set('disabled', false);

        }
        dijit.byId('pesSituacaoFK').set('value', TURMAS_ANDAMENTO);
        dijit.byId('pesSituacaoFK').set('disabled', false);
        dijit.byId('pesTurmasFilhasFK').set('checked', true);
        dijit.byId('pesTurmasFilhasFK').set('disabled', true);
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisarTurmaFKMudanca();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKDesistencia() {
    if (dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value == DESISTENCIA_CAD)
        retornarTurmaFKCadDesistencia();
    else
        retornarTurmaFKPesqDesistencia();
}

function retornarTurmaFKCadDesistencia() {
    try{
        if (hasValue(dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value) && dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value == DESISTENCIA_CAD) {
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
                if (gridPesquisaTurmaFK.itensSelecionados[0].cd_turma_ppt > 0)
                    pesquisaAlunoDesistePPTFilha(gridPesquisaTurmaFK.itensSelecionados[0].cd_turma, gridPesquisaTurmaFK.itensSelecionados[0].cd_turma_ppt);
                dojo.byId("nome_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dojo.byId("cdTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                var cdAluno = dojo.byId("cdResponsavelCto").value > 0 ? dojo.byId("cdResponsavelCto").value : 0;
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (gridPesquisaTurmaFK.itensSelecionados[0].cd_contrato > 0 && cdAluno > 0) {
                    showCarregando();
                    returnLocalMovimento(cdAluno, gridPesquisaTurmaFK.itensSelecionados[0].cd_contrato, Permissoes);
                }
                dijit.byId('limparTurmaCad').set("disabled", false);
                //dijit.byId("pesAlunoFKCad").set("disabled", false);
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

function pesquisaAlunoDesistFK() {
    var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
    var opcao = dijit.byId("cbTipoCad").value == TP_DESISTENCIA ? DESISTENCIA_PES : CANCELA_DESISTENCIA;
    var turma = hasValue(dojo.byId("cdTurmaCad").value) && dojo.byId("cdTurmaCad").value > 0 ? dojo.byId("cdTurmaCad").value : 0;

    myStore = dojo.store.Cache(
    dojo.store.JsonRest({
        target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                document.getElementById("inicioAlunoFK").checked + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + turma + "&opcao=" + opcao + "&origemFK=0",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        idProperty: "cd_aluno"
    }), dojo.store.Memory({ idProperty: "cd_aluno" }));
    dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
    var gridAluno = dijit.byId("gridPesquisaAluno");
    gridAluno.itensSelecionados = [];
    gridAluno.setStore(dataStore);
}

function pesquisaAlunoDesistePPTFilha(cdTurma, cdTurmaPPTPai) {
    var opcao = dijit.byId("cbTipoCad").value == TP_DESISTENCIA ? DESISTENCIA_PES : CANCELA_DESISTENCIA;
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/aluno/getAlunoPorTurmaPPTFilha?cdTurma=" + cdTurma + "&cdTurmaPai=" + cdTurmaPPTPai + "&opcao=" + opcao,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                showCarregando();
                var retorno = $.parseJSON(data).retorno;
                dojo.byId("cdAlunoCad").value = retorno.cd_aluno_turma;
                dojo.byId("cdAlunoDesistencia").value = retorno.cd_aluno;
                dojo.byId("nome_aluno").value = retorno.no_pessoa;
                dijit.byId('limparAlunoCad').set("disabled", false);
                dojo.byId("cdResponsavelCto").value = retorno.cd_pessoa_aluno;
                dojo.byId("cdContrato").value = retorno.cd_contrato;
                var cd_aluno = retorno.cd_pessoa_aluno;
                var cd_contrato = retorno.cd_contrato;
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if(cd_contrato > 0)
                    returnLocalMovimento(cd_aluno, cd_contrato, Permissoes);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemDesistencia', error);
        });
    });
}

function pesquisaTurmaDesistecdAluno(cdAluno) {
    var opcao = dijit.byId("cbTipoCad").value == TP_DESISTENCIA ? DESISTENCIA_PES : CANCELA_DESISTENCIA;
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.get({
            url: Endereco() + "/api/turma/getTurmaComAlunoDesistencia?cdAluno=" + cdAluno + "&opcao=" + opcao,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                var retorno = $.parseJSON(data).retorno;
                if (retorno.cd_turma > 0) {
                    dojo.byId("nome_turma").value = retorno.no_turma;
                    dojo.byId("cdTurmaCad").value = retorno.cd_turma;
                    dijit.byId('limparTurmaCad').set("disabled", false);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemDesistencia', error);
        });
    });
}

function abrirAlunoCadFK(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try{
        //Configuração retorno fk de aluno na pesquisa de aluno/desistencia
        dojo.byId('tipoRetornoAlunoFK').value = CADDESISTENCIA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisaAlunoDesistFK(xhr, ObjectStore, Memory, Cache, JsonRest);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoCadFK() {
    try{
        if (hasValue(dojo.byId("tipoRetornoAlunoFK").value) && dojo.byId("tipoRetornoAlunoFK").value == CADDESISTENCIA) {
            var valido = true;
            var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
            if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            }
            else {
                dojo.byId("cdAlunoCad").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno_turma;
                dojo.byId("cdAlunoDesistencia").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                dojo.byId("nome_aluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                dijit.byId('limparAlunoCad').set("disabled", false);
                dojo.byId("cdResponsavelCto").value = gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno;
                dojo.byId("cdContrato").value = gridPesquisaAluno.itensSelecionados[0].cd_contrato;
                var cd_aluno = gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno;
                var cd_contrato = gridPesquisaAluno.itensSelecionados[0].cd_contrato;
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (cd_contrato > 0) {
                    showCarregando();
                    returnLocalMovimento(cd_aluno, cd_contrato, Permissoes);
                }

            if (dijit.byId("cbTipoCad").value == TP_CANCELAMENTO)
                dijit.byId("ckAlunoAtivo").set("checked", true);
            else if (dijit.byId("cbTipoCad").value == TP_DESISTENCIA)
                dijit.byId("ckAlunoAtivo").set("checked", false);

                pesquisaTurmaDesistecdAluno(gridPesquisaAluno.itensSelecionados[0].cd_aluno);
            }
            if (!valido)
                return false;
            dijit.byId("proAluno").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaFKMudanca() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var opcao = dijit.byId("cbTipoCad").value == TP_DESISTENCIA ? DESISTENCIA_PES : CANCELA_DESISTENCIA;
            var cdAluno = dojo.byId("cdAlunoDesistencia").value > 0 ? dojo.byId("cdAlunoDesistencia").value : 0;
            var turmasFilhas = cdTipoTurma == 3 ? document.getElementById("pesTurmasFilhasFK").checked : false;
            /*--combo_escola_fk
            //some a combo de escola
            dojo.byId("trEscolaTurmaFiltroFk").style.display = "none";
            dojo.byId("lbEscolaTurmaFiltroFk").style.display = "none";
            require(['dojo/dom-style', 'dijit/registry'],
                function (domStyle, registry) {

                    domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', 'none');
                });*/

            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchComAluno?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value + "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor + "&prog=" + cdProg + "&turmasFilhas=" + turmasFilhas
                                            + "&cdAluno=" + cdAluno + "&dtInicial=&dtFinal=&cd_turma_PPT=null&cdTurmaOri=0&opcao=" + opcao + "&cd_escola_combo_fk=0",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];

            if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
                grid.layout.setColumnVisibility(2, true)
                grid.layout.setColumnVisibility(3, false)
                grid.turmasFilhas = true;
            }
            else {
                grid.layout.setColumnVisibility(2, false)
                grid.layout.setColumnVisibility(3, true)
                grid.turmasFilhas = false;
            }
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
//#endregion

//#region - keepValues - Crud 

function keepValuesDesitencia(value, grid, ehLink, xhr, ready, Memory, FilteringSelect) {
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
        dijit.byId('cbTipoCad')._onChangeActive = false;
        if (hasValue(value)) {
            if (hasValue(dojo.byId('cbTipoCad').value, true)) {
                hasValue(value) && hasValue(value.id_tipo_desistencia) ? dijit.byId("cbTipoCad").set("value", value.id_tipo_desistencia) : dijit.byId("cbTipoCad").reset();
            }

            dijit.byId('pesTurmaFKCadDesistencia').set('disabled', true);
            dijit.byId('cbTipoCad').set('disabled', true);
            dojo.byId("cdDesistencia").value = value.cd_desistencia;
            dojo.byId("cdTurmaCad").value = value.cd_turma;
            dojo.byId("cdAlunoCad").value = value.cd_aluno_turma;
            dojo.byId("cdAlunoDesistencia").value = value.cd_aluno;
            dojo.byId("nome_turma").value = value.no_turma;
            dojo.byId("nome_aluno").value = value.no_pessoa;
            dojo.byId("dtaDesistencia").value = value.dtaDesistencia != null ? dojo.byId("dtaDesistencia").value = value.dtaDesistencia : "";
            dojo.byId("cdAlunoTurma").value = value.cd_aluno_turma;
            dijit.byId("ckAlunoAtivo").set("checked", value.id_aluno_ativo);

            if (hasValue(dojo.byId('cbMotivoCad').value, true)) {
                hasValue(value.cd_motivo_desistencia) ? dijit.byId("cbMotivoCad").set("value", value.cd_motivo_desistencia) : dijit.byId("cbMotivoCad").reset();
            }

            if (hasValue(dojo.byId('des_obs').value, true)) {
                hasValue(value.tx_obs_desistencia) ? dijit.byId("des_obs").set("value", value.tx_obs_desistencia) : dijit.byId("des_obs").reset();
            }

            dojo.byId("cdResponsavelCto").value = value.cd_pessoa;
            dojo.byId("cdContrato").value = value.cd_contrato;
            dijit.byId('cbEscolhaCad').reset();
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        }
        if (hasValue(value) && value.cd_contrato > 0)
            returnLocalMovimento(value.cd_pessoa, value.cd_contrato, Permissoes);
        else
            showCarregando();
        dijit.byId('cbTipoCad')._onChangeActive = true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DesistenciaObj() {
    try{
        this.cd_desistencia = dojo.byId('cdDesistencia').value;
        this.cd_pessoa = dojo.byId('cdAlunoCad').value;
        this.cd_turma = dojo.byId('cdTurmaCad').value;
        this.cd_aluno_turma = dojo.byId('cdAlunoCad').value;
        this.id_tipo_desistencia = dijit.byId('cbTipoCad').get('value');
        this.dc_tipo = dojo.byId('cbTipoCad').value;
        this.dt_desistencia = dojo.date.locale.parse(dojo.byId("dtaDesistencia").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.cd_motivo_desistencia = dijit.byId('cbMotivoCad').get('value');
        this.tx_obs_desistencia = dijit.byId('des_obs').get('value');
        this.no_turma = dojo.byId('nome_turma').value;
        this.no_pessoa = dojo.byId('nome_aluno').value;
        this.dc_motivo_desistencia = dojo.byId('cbMotivoCad').value;
        this.cd_aluno = dojo.byId('cdAlunoDesistencia').value;
        this.cd_pessoa_responsavel_titulo = dojo.byId("cdResponsavelCto").value;
        this.cd_contrato = dojo.byId("cdContrato").value;
        this.id_cancelamento = dijit.byId('cbTipoCad').value == CANCELAMENTO ? true : false;
        this.id_aluno_ativo = dijit.byId("ckAlunoAtivo").checked;

        if (dijit.byId("cbEscolhaCad").get('value') > 0) {
            this.cd_tipo_liquidacao = dijit.byId('cbLiquidacaoCad').get('value');
            this.cd_local_movto = dijit.byId('des_local').get('value');
            this.titulos = montarTitulos();
        }
        if (document.getElementById("tgChequeDesistencia").style.display != "none") {
            this.chequeTransacao =
                {
                    Cheque:
                        {
                            no_emitente_cheque: dijit.byId("emissorChequeDesistencia").get('value'),
                            no_agencia_cheque: dijit.byId("nomeAgenciaChequeDesistencia").get('value'),

                            nm_agencia_cheque: dijit.byId("nroAgenciaChequeDesistencia").get('value'),
                            nm_digito_agencia_cheque: dijit.byId("dgAgenciaChequeDesistencia").get('value'),

                            nm_conta_corrente_cheque: dijit.byId("nroContaCorrenteChequeDesistencia").get('value'),
                            nm_digito_cc_cheque: dijit.byId("dgContaCorrenteChequeDesistencia").get('value'),

                            nm_primeiro_cheque: dijit.byId("nroPrimeiroChequeChequeDesistencia").get('value'),
                            cd_banco: dijit.byId("bancoChequeDesistencia").get('value'),
                        },
                    nm_cheque: dijit.byId("nroPrimeiroChequeChequeDesistencia").get('value'),
                    dt_bom_para: dijit.byId("dtChequeChequeDesistencia").get('value')
                }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirDesistencia(Permissoes) {
    try {
        apresentaMensagem("apresentadorMensagemCadDesistencia", '');

        var titulos = dijit.byId('gridTituloDesistencia').store.objectStore.data;
        var newDesistencia = new DesistenciaObj();

        if (newDesistencia.titulos == false)
            return false;

        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {

            if (!validarCamposTitulos(titulos, windows))
                return false;

            if (dijit.byId('cbLiquidacaoCad').value == CHEQUEPREDATADO_DESISTENCIA || dijit.byId('cbLiquidacaoCad').value == CHEQUEVISTA_DESISTENCIA) {
                if (!validarTipoLiqChequeTransacaoDesistencia(dijit.byId('cbLiquidacaoCad').value)) {
                    dijit.byId("tgChequeDesistencia").set("open", true);
                    return false;
                }
            }

            var requestApi = "/api/escola/postBaixarTituloAddDesistencia";

            if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes))
                requestApi = "/api/escola/postBaixarTituloAddDesistenciaGeral";
            showCarregando();
            xhr.post(Endereco() + requestApi, {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(newDesistencia)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    data = $.parseJSON(data);
                    var itemAlterado = data.retorno;
                    var gridName = 'gridDesistencia';
                    var tipoCad = dijit.byId("cbTipoCad").get('value');
                    var grid = dijit.byId(gridName);
                    if (hasValue(dojo.byId("cd_turma"))) {
                        var tipoTurma = dojo.byId("tipoTurmaCad").value;
                        var tipoVerif = dojo.byId("tipoVerif").value;
                        dojo.byId("cdTurmaCad").value = dojo.byId("cd_turma").value;
                        var gridAlunos = dijit.byId("gridAlunos");
                        var msg = 'apresentadorMensagem';
                        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL) {
                            gridAlunos = dijit.byId("gridAlunosPPT");
                            msg = 'apresentadorMensagemTurmaPPT';
                        }

                        if (hasValue(gridAlunos) && gridAlunos._by_idx.length > 0) {
                            apresentaMensagem(msg, data);

                            for (var i = 0; i <= gridAlunos._by_idx.length; i++)
                                if (gridAlunos._by_idx[i].item.cd_aluno == dojo.byId('cdAlunoDesistencia').value) {
                                    if (parseInt(tipoCad) == DESISTENCIA_CONSISTIR) {//desistência
                                        gridAlunos._by_idx[i].item.cd_situacao_aluno_turma = SITUACAODESISTENTE;
                                        gridAlunos._by_idx[i].item.situacaoAlunoTurma = "Desistente";
                                        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                                            gridAlunos._by_idx[i].item.situacaoAlunoTurmaFilhaPPT = "Desistente";
                                    }
                                    else {
                                        gridAlunos._by_idx[i].item.cd_situacao_aluno_turma = SITUACAOATIVO;
                                        gridAlunos._by_idx[i].item.situacaoAlunoTurma = "Ativo";
                                        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                                            gridAlunos._by_idx[i].item.situacaoAlunoTurmaFilhaPPT = "Ativo";
                                    }
                                    break;
                                }
                            gridAlunos.update();
                        }
                    }
                    else {
                        apresentaMensagem('apresentadorMensagem', data);

                        dijit.byId("cadDesistencia").hide();

                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        insertObjSort(grid.itensSelecionados, "cd_desistencia", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionado', 'cd_desistencia', 'selecionaTodos', ['pesquisarDesistencia', 'relatorioDesistencia'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_desistencia");
                        dijit.byId("cadDesistencia").hide();

                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_desistencia", itemAlterado.cd_desistencia);
                        insertObjSort(grid.itensSelecionados, "cd_desistencia", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionado', 'cd_desistencia', 'selecionaTodos', ['pesquisarDesistencia', 'relatorioDesistencia'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_desistencia");
                    }
                dijit.byId("cadDesistencia").hide();
               showCarregando();
                    //pesquisarDesistencia();
                } else
                    apresentaMensagem('apresentadorMensagemCadDesistencia', data);
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCadDesistencia', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarDesistencia(Permissoes) {
    try{
        apresentaMensagem("apresentadorMensagemCadDesistencia", '');

        var newDesistencia = new DesistenciaObj();

        if (newDesistencia.titulos == false)
            return false;

        var titulos = dijit.byId('gridTituloDesistencia').store.objectStore.data;

        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {

            if (!validarCamposTitulos(titulos, windows))
                return false;

            var requestApi = "/api/escola/postBaixarTituloEditDesistencia";

            if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes))
                requestApi = "/api/escola/postBaixarTituloEditDesistenciaGeral";
            showCarregando();
            xhr.post(Endereco() + requestApi, {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(newDesistencia)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        data = $.parseJSON(data);
                        var itemAlterado = data.retorno;
                        var gridName = 'gridDesistencia';
                        var grid = dijit.byId(gridName);
                        var tipoCad = dijit.byId("cbTipoCad").get('value');

                        if (hasValue(dijit.byId("gridAlunos")) && dijit.byId("gridAlunos")._by_idx.length > 0) {
                            apresentaMensagem('apresentadorMensagemTurma', data);
                            for (var i = 0; i <= dijit.byId("gridAlunos")._by_idx.length; i++)
                                if (dijit.byId("gridAlunos")._by_idx[i].item.cd_aluno == dojo.byId('cdAlunoDesistencia').value) {

                                    if (parseInt(tipoCad) == 1)//desistência
                                        dijit.byId("gridAlunos")._by_idx[i].item.situacaoAlunoTurma = "Desistente"
                                    else dijit.byId("gridAlunos")._by_idx[i].item.situacaoAlunoTurma = "Ativo";

                                    break;
                                }

                            dijit.byId("gridAlunos").update();
                        }
                        else
                            apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadDesistencia").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_desistencia", itemAlterado.cd_desistencia);
                        insertObjSort(grid.itensSelecionados, "cd_desistencia", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionado', 'cd_desistencia', 'selecionaTodos', ['pesquisarDesistencia', 'relatorioDesistencia'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        // pesquisarDesistencia();
                        setGridPagination(grid, itemAlterado, "cd_desistencia");
                    } else
                        apresentaMensagem('apresentadorMensagemCadDesistencia', data);
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCadDesistencia', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarDesistencia(itensSelecionados) {
    showCarregando();
    apresentaMensagem("apresentadorMensagemCadDesistencia", '');
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cdDesistencia').value != 0)
                    itensSelecionados = [{
                        cd_desistencia: dom.byId("cdDesistencia").value,
                        cd_turma: dom.byId("cdTurmaCad").value,
                        cd_aluno: dom.byId("cdAlunoDesistencia").value,
                        cd_aluno_turma: dom.byId("cdAlunoTurma").value,
                        dt_desistencia: dojo.date.locale.parse(dojo.byId("dtaDesistencia").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                        id_tipo_desistencia: dijit.byId("cbTipoCad").get("value")
                    }];
            xhr.post({
                url: Endereco() + "/api/escola/postDeleteDesistencia",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItens");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadDesistencia").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridDesistencia').itensSelecionados, "cd_desistencia", itensSelecionados[r].cd_desistencia);
                    pesquisarDesistencia(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarDesistencia").set('disabled', false);
                    dijit.byId("relatorioDesistencia").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";

                    showCarregando();
                }
                catch (er) {
                    showCarregando();
                    postGerarLog(er);
                }
            },
            function (error) {
                //apresentaMensagem(msg, error);
                if (!hasValue(dojo.byId("cadDesistencia").style.display == "")) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', error);
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCadDesistencia', error);
                }
            });
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

//#endregion

//#region montarTitulos
function montarTitulos() {
    try{
        var titulos = [];
        var gridTituloDesistencia = dijit.byId("gridTituloDesistencia");

        if (hasValue(gridTituloDesistencia))
            gridTituloDesistencia.store.save();

        if (hasValue(gridTituloDesistencia) && hasValue(gridTituloDesistencia.store.objectStore.data) && (gridTituloDesistencia.itensSelecionados != null) && (gridTituloDesistencia.itensSelecionados.length > 0))
            var data = gridTituloDesistencia.itensSelecionados;
        else {
            titulos = null;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);
            apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
            return false;
        }

        if (hasValue(gridTituloDesistencia) && hasValue(data) && data.length > 0) {
            $.each(data, function (idx, val) {
                titulos.push({
                    cd_titulo: val.cd_titulo,
                    cd_pessoa_empresa: val.cd_pessoa_empresa,
                    cd_pessoa_titulo: val.cd_pessoa_titulo,
                    cd_pessoa_responsavel: val.cd_pessoa_responsavel,
                    cd_local_movto: dijit.byId('des_local').get('value'),
                    dt_emissao_titulo: val.dt_emissao_titulo,
                    cd_origem_titulo: val.cd_origem_titulo,
                    dt_vcto_titulo: val.dt_vcto_titulo,
                    dh_cadastro_titulo: val.dh_cadastro_titulo,
                    vl_titulo: val.vl_titulo,
                    dt_liquidacao_titulo: val.dt_liquidacao_titulo,
                    dc_codigo_barra: val.dc_codigo_barra,
                    dc_tipo_titulo: val.dc_tipo_titulo,
                    dc_nosso_numero: val.dc_nosso_numero,
                    dc_num_documento_titulo: val.dc_num_documento_titulo,
                    vl_saldo_titulo: val.vl_saldo_titulo,
                    nm_titulo: val.nm_titulo,
                    nm_parcela_titulo: val.nm_parcela_titulo,
                    cd_tipo_financeiro: val.cd_tipo_financeiro,
                    id_status_titulo: val.id_status_titulo,
                    id_status_cnab: val.id_status_cnab,
                    id_origem_titulo: val.id_origem_titulo,
                    id_natureza_titulo: val.id_natureza_titulo,
                    vl_multa_titulo: val.vl_multa_titulo,
                    vl_juros_titulo: val.vl_juros_titulo,
                    vl_desconto_titulo: val.vl_desconto_titulo,
                    vl_liquidacao_titulo: val.vl_liquidacao_titulo,
                    vl_multa_liquidada: val.vl_multa_liquidada,
                    vl_juros_liquidado: val.vl_juros_liquidado,
                    vl_desconto_juros: val.vl_desconto_juros,
                    vl_desconto_multa: val.vl_desconto_multa,
                    pc_juros_titulo: val.pc_juros_titulo,
                    pc_multa_titulo: val.pc_multa_titulo,
                    cd_plano_conta_tit: val.cd_plano_conta_tit,
                    Cheque: val.Cheque
                });
            });
            return titulos;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function chamarPesquisasDesistencia() {
    var tipoOrigem = dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value;
    if (hasValue(tipoOrigem) && tipoOrigem == DESISTENCIA_CAD) {
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisarTurmaFKMudanca();
    }
    if (hasValue(tipoOrigem) && tipoOrigem == DESISTENCIA_PESQUISA) {
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisaTurmaAlunoDesistente(dojo.xhr, dojo.data.ObjectStore, dojo.store.Memory, dojo.store.Cache, dojo.store.JsonRest);
    }
}

function validarTipoLiqChequeTransacaoDesistencia(event) {
    var gridTitulos = dijit.byId("gridTituloDesistencia");
    apresentaMensagem("apresentadorMensagemCadDesistencia", "");
    var mensagensWeb = new Array();

    if (!hasValue(gridTitulos.itensSelecionados)) {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);
        apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
        dijit.byId("cbLiquidacaoCad").reset();
        return false;
    }

    document.getElementById("tgChequeDesistencia").style.display = "none";
    //setRequiredTipoCheque(false);

    var isCheque = [];
    var isChequeTransacao = [];
    setRequiredCamposChequeDesistencia(false);

    isCheque = jQuery.grep(gridTitulos.itensSelecionados, function (titulo) {
        if (titulo.cd_tipo_financeiro == TIPOFINANCHEQUE)
            return true;
    });

    isChequeTransacao = jQuery.grep(gridTitulos.itensSelecionados, function (titulo) {
        if (titulo.cd_tipo_financeiro != TIPOFINANCHEQUE)
            return true;
    });

    if (event == CHEQUEPREDATADO_DESISTENCIA || event == CHEQUEVISTA_DESISTENCIA) {

        // Retorna mensagem de erro informando os titulos que não são cheque.
        if (isCheque.length > 0 && isChequeTransacao.length > 0) {

            if (isViewChequeTransacao != null) {
                if (isViewChequeTransacao)
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoTransacao);
                else
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoCheque);
            } else
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);
            apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }

        // Libera o botão adicionar cheque para cada titulo selecionado do grid.
        if (isCheque.length > 0 && isChequeTransacao.length == 0 && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0)) {
            isViewChequeTransacao = false;
            apresentaMensagem("apresentadorMensagemCadDesistencia", "");
            gridTitulos.layout.setColumnVisibility(8, true);
            return true;
        }

        // Libera a opção de inserir um unico cheque para todos os titulos.
        if (isCheque.length == 0 && isChequeTransacao.length > 0 && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0)) {
            // show 
            isViewChequeTransacao = true;
            document.getElementById("tgChequeDesistencia").style.display = "";
            apresentaMensagem("apresentadorMensagemCadDesistencia", "");
            setRequiredCamposChequeDesistencia(true);
            return true;
        }
    } else {
        document.getElementById("tgChequeDesistencia").style.display = "none";
        dijit.byId('gridTituloDesistencia').layout.setColumnVisibility(8, false);
        isViewChequeTransacao = null;
        if (isCheque.length > 0 && isChequeTransacao.length > 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);
            apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }
        //Não deixar liquidar os títulos financeiros do tipo cheque com outro tipo de liquidação que não seja cheque pré-datado ou avista
        if (isCheque.length > 0 && isChequeTransacao.length == 0 && event != CHEQUEPREDATADO_DESISTENCIA) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);
            apresentaMensagem("apresentadorMensagemCadDesistencia", mensagensWeb);
            dijit.byId("cbLiquidacaoCad").reset();
            return false;
        }
    }
}

function alterarTamnhoBotaoDesistencia(id) {
    decreaseBtn(document.getElementById(id), '48px');
}

function desabilitarBotaoIncluirCheque(isChecked, tituloSelecionado, isCheckAoMontarGrid) {

    if (!hasValue(dijit.byId("cbLiquidacaoCad").value))
        dijit.byId('gridTituloDesistencia').layout.setColumnVisibility(8, false);

    if (isCheckAoMontarGrid) {
        if (hasValue(dijit.byId("gridTituloDesistencia").itensSelecionados)) {
            var grid = dijit.byId("gridTituloDesistencia").itensSelecionados;
            for (var i = 0; i < grid.length; i++) {
                if (grid[i].cd_tipo_financeiro == TIPOFINANCHEQUE && hasValue(dijit.byId(grid[i].cd_titulo + "_" + grid[i].nm_parcela_titulo))) {
                    dijit.byId(grid[i].cd_titulo + "_" + grid[i].nm_parcela_titulo).set("disabled", false);
                }
            }
            if (grid.length == dijit.byId("gridTituloDesistencia").store.objectStore.data.length)
                return false;
        }
        return true;
    } else {
        if (hasValue(tituloSelecionado)) {
            if (tituloSelecionado.cd_tipo_financeiro == TIPOFINANCHEQUE && isChecked && (hasValue(dijit.byId("cbLiquidacaoCad").value) && dijit.byId("cbLiquidacaoCad").value > 0)) {
                dijit.byId(tituloSelecionado.cd_titulo + "_" + tituloSelecionado.nm_parcela_titulo).set("disabled", false);
            } else {
                dijit.byId(tituloSelecionado.cd_titulo + "_" + tituloSelecionado.nm_parcela_titulo).set("disabled", true);
            }
        }
    }
}

function setRequiredCamposChequeDesistencia(isRequired) {
    dijit.byId("tgChequeDesistencia").set("open", false);
    dijit.byId("emissorChequeDesistencia").set("required", isRequired);
    dijit.byId("nroAgenciaChequeDesistencia").set("required", isRequired);
    dijit.byId("nomeAgenciaChequeDesistencia").set("required", isRequired);
    dijit.byId("dgAgenciaChequeDesistencia").set("required", isRequired);
    dijit.byId("dtChequeChequeDesistencia").set("required", isRequired);
    dijit.byId("bancoChequeDesistencia").set("required", isRequired);
    dijit.byId("nroContaCorrenteChequeDesistencia").set("required", isRequired);
    dijit.byId("dgContaCorrenteChequeDesistencia").set("required", isRequired);
    dijit.byId("nroPrimeiroChequeChequeDesistencia").set("required", isRequired);
}