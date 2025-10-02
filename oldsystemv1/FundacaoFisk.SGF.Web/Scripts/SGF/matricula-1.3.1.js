//#region declaração de constantes
var ABRINDOTELA = 1;
var VENDASPRODUTO = 2, ORIGEMCHAMADONF = 22;
var TIPONUMEROMATRICULA = 0, NUMEROMATRICULAIGUALCONTRATO = 1;
function montarCadastroMatricula(permissoes) {
    //Criação da Grade de sala
    require([
    "dojo/_base/xhr",
    "dojo/dom",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojox/grid/enhanced/plugins/NestedSorting",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/query",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojox/json/ref",
    "dijit/Dialog",
    "dojo/domReady!"
    ], function (xhr, dom, EnhancedGrid, Pagination, NestedSorting, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, on, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, ref) {
        ready(function () {
            try {
                xhr.get({
                    url: Endereco() + "/api/escola/getTipoNumeroContrato",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    if (hasValue(data) && hasValue(data.retorno) && data.retorno > 0)
                        TIPONUMEROMATRICULA = data.retorno;
                    dojo.byId("tdCkRenegociar").style.display = "none";
                    dojo.byId("tdCkRenegociarLabel").style.display = "none";
                    componentesPesquisaMatricula();
                    if (hasValue(permissoes))
                        document.getElementById("setValuePermissoes").value = permissoes;
                    dojo.byId("apresentadorMensagemFks").value = "apresentadorMensagemFks";
                    query("body").addClass("claro");
                    montarTipoPesquisa(ready, Memory, FilteringSelect);
                    loadSituacaoPesquisa(ready, Memory, FilteringSelect);
                    montarTipoVinculado(ready, Memory, FilteringSelect);
                    loadTipoContrato(Memory, dijit.byId('tipoContrato'));
                    montarStatus("statusAluno");

                    var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/secretaria/getMatriculaSearch?descAluno=&descTurma=&inicio=false&semTurma=false&situacaoTurma=1" +
                                            "&nmContrato=0&tipo=0&dtaInicio=null&dtaFim=null&filtraMat=false&filtraDtaInicio=false&filtraDtaFim=false" +
                                            "&renegocia=false&transf=false&retornoEsc=false&cdNomeContrato=null&nm_matricula=0&cdAnoEscolar=null" + 
                                            "&cdContratoAnterior=null&tipoC=4&status=1&vinculado=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }), Memory({}));


                    var gridMatricula = new EnhancedGrid({
                        store: ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                        structure: [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMatricula },
                            { name: "Contrato", field: "nm_contrato", width: "6%", styles: "min-width:80px;" },
                            { name: "Matrícula", field: "nm_matricula_contrato", width: "7%", styles: "min-width:80px;" },
                            { name: "Aluno", field: "no_pessoa", width: "15%", styles: "min-width:80px;" },
                            { name: "Turma", field: "no_turma", width: "16%", styles: "min-width:80px;" },
                            { name: "Data Matricula", field: "dtMatriculaContrato", width: "8%", styles: "min-width:80px;" },
                            { name: "Data Inicial", field: "dtInicialContrato", width: "7%", styles: "min-width:80px;" },
                            { name: "Data Final", field: "dtFinalContrato", width: "7%", styles: "min-width:80px;" },
                            { name: "Tipo", field: "dc_tipo_matricula", width: "8%", styles: "min-width:60px;" },
                            { name: "Tipo de Desconto", field: "desc_descontos_contrato", width: "15%", styles: "min-width:60px;" },
                            { name: "Situação", field: "dc_situacao_turma", width: "7%", styles: "min-width:60px;" }
                        ],
                        noDataMessage: msgNotRegEncFiltro,
                        plugins: {
                            nestedSorting: true,
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                /*page step to be displayed*/
                                maxPageStep: 4,
                                /*position of the pagination bar*/
                                position: "button"
                            }
                        }
                    }, "gridMatricula");

                    gridMatricula.pagination.plugin._paginator.plugin.connect(gridMatricula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                        verificaMostrarTodos(evt, gridMatricula, 'cd_contrato', 'selecionaTodos');
                    });
                    require(["dojo/aspect"], function (aspect) {
                        aspect.after(gridMatricula, "_onFetchComplete", function () {
                            // Configura o check de todos:
                            if (dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionaMatricula', -1, 'selecionaTodos', 'selecionaTodos', 'gridMatricula')", gridMatricula.rowsPerPage * 3);
                        });
                    });

                    gridMatricula.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 10; };
                    gridMatricula.startup();
                    gridMatricula.itensSelecionados = new Array();
                    gridMatricula.on("RowDblClick", function (evt) {
                        try {
                            apresentaMensagem('apresentadorMensagemMat', null);
                            //showCarregando();
                            var idx = evt.rowIndex,
                                      item = this.getItem(idx),
                                      store = this.store;
                            if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                                montarCadastroMatriculaPartial(function () {
                                    dijit.byId("alterarMatricula").on("click", function () {
                                        //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                                        alterarMatricula();
                                    });
                                    setarTabCadMatricula(true);
                                    dijit.byId('tabContainerMatricula').resize();
                                    hideTagMatriculaTurma();
                                    dijit.byId("cadMatricula").show();
                                    dijit.byId('tabContainerMatricula').resize();
                                    IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');

                                    gridMatricula.itemSelecionado = item;
                                    habilitarOnChange("ckAula", false);
                                    keepValuesMatricula(item, gridMatricula, true, xhr, ready, Memory, FilteringSelect, ObjectStore);
                                    habilitarOnChange("ckAula", true);
                                }, Permissoes);
                            }
                            else {
                                dijit.byId('dtaInicioMat').set('disabled', false);
                                dijit.byId('dtaFinalMat').set('disabled', false);
                                dijit.byId('tipoMatricula').set('disabled', false);
                                dijit.byId('vl_pre_matricula').set('disabled', false);
                                dijit.byId('ckTransfCad').set('disabled', false);
                                dijit.byId('id_venda_pacote').set('disabled', false);
                                //dijit.byId('ckManual').set('disabled', false);
                                dijit.byId('ckCertificado').set('disabled', false);
                                dojo.byId('id_motivo_aditamento').value = false;

                                //habilita a ação relacionada Editar da grid dos titulos
                                require([
                                        "dojo/on",
                                        "dijit/registry",
                                        "dojo/ready",
                                        "dojo/require"
                                    ],
                                    function (on, registry, ready) {
                                        ready(function () {

                                            registry.byId("acoesRelacionadasTit").dropDown._getFirst().set("disabled", false);

                                        });
                                    });

                                

                                setarTabCadMatricula(true);
                                dijit.byId('tabContainerMatricula').resize();
                                hideTagMatriculaTurma();
                                dijit.byId("cadMatricula").show();
                                dijit.byId('tabContainerMatricula').resize();
                                IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');
                                var idx = evt.rowIndex,
                                        item = this.getItem(idx),
                                        store = this.store;
                                gridMatricula.itemSelecionado = item;
                                habilitarOnChange("ckAula", false);
                                keepValuesMatricula(item, gridMatricula, true, xhr, ready, Memory, FilteringSelect, ObjectStore);
                                habilitarOnChange("ckAula", true);
                                dijit.byId('btnNovoAdto').set("disabled", false);
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }, true);
                    if (TIPONUMEROMATRICULA == NUMEROMATRICULAIGUALCONTRATO) {
                        dojo.byId("tdLblMatricula").style.display = "none";
                        dojo.byId("tdCampMatricula").style.display = "none";
                        gridMatricula.layout.setColumnVisibility(2, false);
                    }
                    new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarMatricula(true); } }, "pesquisarMatricula");
                    decreaseBtn(document.getElementById("pesquisarMatricula"), '32px');
                    new Button({
                        label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                            try {
                                var nmContrato = hasValue(document.getElementById("_contrato").value) ? document.getElementById("_contrato").value.replace(".", "") : 0;
                                var nm_matricula = hasValue(document.getElementById("nm_matricula_pesq").value) ? document.getElementById("nm_matricula_pesq").value.replace(".", "") : 0;
                                var pesqAnoEscolar = hasValue(dijit.byId("pesqAnoEscolar").value) ? dijit.byId("pesqAnoEscolar").value : null;
                                if (document.getElementById("ckVirada").checked) {
                                    var cd_contrato_anterior = 1;
                                } else {
                                    var cd_contrato_anterior = null;
                                }
                                xhr.get({
                                    url: Endereco() + "/api/secretaria/GeturlrelatorioMatricula?" + getStrGridParameters('gridMatricula') + "descAluno=" + encodeURIComponent(document.getElementById("_aluno").value) + "&descTurma=" + encodeURIComponent(document.getElementById("_turma").value)
                                        + "&inicio=" + document.getElementById("inicioAlunoTurma").checked + "&semTurma=" + document.getElementById("ckSemTurma").checked +
                                        "&situacaoTurma=" + retornaStatus("situacao") + "&nmContrato=" + nmContrato +
                                        "&tipo=" + retornaStatus("_tipoPes") + "&dtaInicio=" + dojo.byId("dtaInicial").value + "&dtaFim=" + dojo.byId("dtaFinal").value +
                                        "&filtraMat=" + document.getElementById("ckDtaMatricula").checked + "&filtraDtaInicio=" + document.getElementById("ckDtaInicial").checked +
                                        "&filtraDtaFim=" + document.getElementById("ckDtaFinal").checked + "&renegocia=" + dijit.byId("ckRenegociar").checked +
                                        "&transf=" + dijit.byId("ckTransferir").checked + "&retornoEsc=" + dijit.byId("ckRetorno").checked + "&cdNomeContrato=" + dijit.byId("cd_nome_contrato_pesq").value +
                                        "&nm_matricula=" + nm_matricula + "&cd_ano_escolar=" + pesqAnoEscolar + "&cdContratoAnterior=" + cd_contrato_anterior + "&tipoC=" + dijit.byId('tipoContrato').value +
                                        "&status=" + dijit.byId("statusAluno").value + "&vinculado=" + dijit.byId("_tipoVinculado").value,
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagemAluno', error);
                                });
                            } catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "relatorioMatricula");
                    new Button({
                        label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                try {
                                    showCarregando();
                                    var dadosAluno = {
                                        cd_aluno: 0,
                                        nom_aluno: "",
                                        cd_pessoa_aluno: 0,
                                        cd_pessoa_responsavel: 0,
                                        no_pessoa_responsavel: "",
                                        pc_bolsa: 0,
                                        pc_bolsa_material: 0,
                                        dt_inicio_bolsa: null
                                    }
                                    criarOuCarregarCompFiltering("notasMaterialDidatico", [], "", null, ready, Memory, FilteringSelect, 'cd_movimento', 'nm_movimento');
                                    dijit.byId("notasMaterialDidatico").set("disabled", true);
                                    if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                                        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                                        montarCadastroMatriculaPartial(
                                            function () {

                                                dijit.byId("alterarMatricula").on("click", function () {
                                                    //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                                                    alterarMatricula();
                                                });
                                                criaNovaMatricula(xhr, Memory, dadosAluno, 3, ObjectStore, null);
                                                dijit.byId("fkAluno").set("disabled", false);

                                            }, Permissoes);
                                    }
                                    else {
                                        criaNovaMatricula(xhr, Memory, dadosAluno, 3, ObjectStore, null);
                                        dijit.byId("fkAluno").set("disabled", false);
                                    }
                                    hideCarregando();
                                } catch (e) {
                                    hideCarregando();
                                    postGerarLog(e);
                                }
                            }
                    }, "novaMatricula");

                    new Button({
                        label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                            enviarEmailAlunoRaf(Memory, ObjectStore);

                        }
                    }, "enviarEmailRafAluno");

                    new Button({
                        label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                            dijit.byId('dialogEnviarEmailAlunoRaf').hide();
                        }
                    }, "fecharDialogEnviarEmailRafAluno");

                    //Metodo para a criação do dropDown de link
                    //if (dojo.byId('selecionaTodos').type == 'text')
                    //    setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridPessoa')", gridMatricula.rowsPerPage * 3);

                    // Adiciona link de ações:
                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () {
                            eventoEditar(gridMatricula.itensSelecionados, xhr, ready, Memory, FilteringSelect, ObjectStore);
                        }
                    });
                    menu.addChild(acaoEditar);

                    var acaoRemover = new MenuItem({
                        label: "Excluir",
                        onClick: function () {
                            try {
                                var gridMatricula = dijit.byId('gridMatricula');
                                var itensSelecionados = gridMatricula.itensSelecionados;
                                if (itensSelecionados.length <= 0)
                                    caixaDialogo(DIALOGO_ERRO, msgNoSelectRegExcluirMatricula, null);
                                if (itensSelecionados.length > 1)
                                    caixaDialogo(DIALOGO_ERRO, msgSelectRegExcluirMatricula, null);
                                else
                                    eventoRemover(gridMatricula.itensSelecionados, 'DeletarMatricula(itensSelecionados)');
                                    //DeletarMatricula(itensSelecionados);
                            } catch (e) {
                                postGerarLog(e);
                            }
                         }                        
                            //eventoRemover(gridMatricula.itensSelecionados, 'DeletarMatricula(itensSelecionados)');
                        //}
                    });
                    menu.addChild(acaoRemover);

                    var acaoContrato = new MenuItem({
                        label: "Contrato",
                        onClick: function () {
                            try {
                                var gridMatricula = dijit.byId('gridMatricula');
                                var itensSelecionados = gridMatricula.itensSelecionados;

                                if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                                    caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdateContrato, null);
                                else if (itensSelecionados.length > 1)
                                    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdateContrato, null);
                                else
                                    imprimirContrato(itensSelecionados[0].cd_contrato);
                            } catch (e) {
                                postGerarLog(e);
                            }
                        }
                    });
                    menu.addChild(acaoContrato);

                    var acaoCNAB = new MenuItem({
                        label: "Cnab + Boletos",
                        onClick: function () {
                            redirecionarCNABBoletos(0);
                        }
                    });
                    menu.addChild(acaoCNAB);

                    var acaoBoleto = new MenuItem({
                        label: "Boletos",
                        onClick: function () {
                            redirecionarCNABBoletos(1);
                        }
                    });
                    menu.addChild(acaoBoleto);

                    var acaoMaterial = new MenuItem({
                        label: "Venda de Material",
                        onClick: function () {
                            var itensSelecionados = gridMatricula.itensSelecionados;
                            redirecionarMaterialDidatico(xhr, false);
                        }
                    });
                    menu.addChild(acaoMaterial);

                    var acaoMaterialF = new MenuItem({
                        label: "Venda de Material Futura",
                        onClick: function () {
                            var itensSelecionados = gridMatricula.itensSelecionados;
                            redirecionarMaterialDidatico(xhr, true);
                        }
                    });
                    menu.addChild(acaoMaterialF);

                    new Button({
                        label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                            var gridMatricula = dijit.byId('gridMatricula');
                            var itensSelecionados = gridMatricula.itensSelecionados;
                            emitirRelatorioCarne(itensSelecionados[0].cd_contrato, permissoes, xhr);
                        }
                    }, "visulizarCarne");

                    new Button({
                        label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                            dijit.byId('dialogGerarCarne').hide();
                        }
                    }, "fecharDialogCarne");

                    var acaoCarne = new MenuItem({
                        label: "Carnê",
                        onClick: function () {
                            try {
                                var gridMatricula = dijit.byId('gridMatricula');
                                var itensSelecionados = gridMatricula.itensSelecionados;
                                if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                                    caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdateCarne, null);
                                else if (itensSelecionados.length > 1)
                                    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdateCarne, null);
                                else {
                                    limparInputDialogCarne();
                                    dijit.byId('dialogGerarCarne').show();
                                }
                                    //emitirRelatorioCarne(itensSelecionados[0].cd_contrato, permissoes, xhr);
                            } catch (e) {
                                postGerarLog(e);
                            }
                        }
                    });
                    menu.addChild(acaoCarne);

                    var acaoReciboConfirmacao = new MenuItem({
                        label: "Recibo Confirmacao",
                        onClick: function () {
                            var gridMatricula = dijit.byId('gridMatricula');
                            if (!hasValue(gridMatricula.itensSelecionados) || gridMatricula.itensSelecionados.length <= 0)
                                caixaDialogo(DIALOGO_AVISO, 'Selecione alguma matricula para emitir o recibo de confirmação.', null);
                            else if (gridMatricula.itensSelecionados.length > 1)
                                caixaDialogo(DIALOGO_ERRO, 'Selecione somente uma matricula para emitir o recibo de confirmação.', null);
                            else {
                                apresentaMensagem('apresentadorMensagem', null);
                                var cd_contrato = gridMatricula.itensSelecionados[0].cd_contrato;
                                xhr.get({
                                    url: Endereco() + "/api/coordenacao/getUrlRelatorioRecibo?cd_contrato=" + cd_contrato,
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

                    menu.addChild(acaoReciboConfirmacao);

                    var acaoEnviarEmailAlunoRaf = new MenuItem({
                        label: "Enviar Email Aluno RAF",
                        onClick: function () {
                            openDialogEnviarEmailAlunoRaf(Memory, ObjectStore);
                        }
                    });
                    menu.addChild(acaoEnviarEmailAlunoRaf);

                    //var acaoProspectGerado = new MenuItem({
                    //    label: "Enviar Promoção Aluno",
                    //    onClick: function () {
                    //        try {
                    //            var gridMatricula = dijit.byId('gridMatricula');
                    //            if (!hasValue(gridMatricula.itensSelecionados) || gridMatricula.itensSelecionados.length <= 0)
                    //                caixaDialogo(DIALOGO_AVISO, 'Selecione alguma matrícula para enviar a promoção.', null);
                    //            else if (gridMatricula.itensSelecionados.length > 1)
                    //                caixaDialogo(DIALOGO_ERRO, 'Selecione somente uma matrícula para enviar a promoção.', null);
                    //            else {
                    //                var dataHoje = new Date(2023, 0, 1);
                    //                if (gridMatricula.itensSelecionados[0]['dtMatriculaContrato'] == null ||
                    //                    gridMatricula.itensSelecionados[0]['dtMatriculaContrato'] == undefined || 
                    //                    gridMatricula.itensSelecionados[0]['dtMatriculaContrato'] == '') {
                    //                    caixaDialogo(DIALOGO_ERRO, 'Data de matrícula nula ou vazia.', null);
                    //                }

                    //                var dataContratoAluno = new Date(gridMatricula.itensSelecionados[0].dt_matricula_contrato);
                                    
                    //                if (gridMatricula.itensSelecionados[0]['cd_aluno'] == null ||
                    //                    gridMatricula.itensSelecionados[0]['cd_aluno'] == undefined) {
                    //                    caixaDialogo(DIALOGO_ERRO, 'Código do aluno não encontrado.', null);
                    //                }
                    //                if (gridMatricula.itensSelecionados[0]['id_tipo_matricula'] == null ||
                    //                    gridMatricula.itensSelecionados[0]['id_tipo_matricula'] == undefined) {
                    //                    caixaDialogo(DIALOGO_ERRO, 'Tipo de matrícula não encontrado.', null);
                    //                }
                    //                if (gridMatricula.itensSelecionados[0].id_tipo_matricula != 1 &&
                    //                    gridMatricula.itensSelecionados[0].id_tipo_matricula != 2) {
                    //                    caixaDialogo(DIALOGO_ERRO, 'Para enviar a promoção o tipo de matrícula selecionado deve ser tipo matrícula ou rematricula.', null);
                    //                }
                    //                if (dojo.date.compare(dataContratoAluno, dataHoje) < 0) {
                    //                    caixaDialogo(DIALOGO_ERRO, 'Selecione somente um contrato com data de matrícula a partir de Janeiro de 2023.', null);
                    //                }
                    //                else {
                    //                    apresentaMensagem('apresentadorMensagem', null);
                    //                    var cdAluno = gridMatricula.itensSelecionados[0].cd_aluno;
                    //                    var cdContrato = gridMatricula.itensSelecionados[0].cd_contrato;
                    //                    var idTipoMatricula = gridMatricula.itensSelecionados[0].id_tipo_matricula;
                    //                    xhr.get({
                    //                        url: Endereco() + "/api/Escola/getSendPromocaoAlunoMatricula?cd_aluno=" + cdAluno + "&cd_contrato=" + cdContrato + "&id_tipo_matricula=" + idTipoMatricula,
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
                    dom.byId("linkAcoes").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });

                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function () {
                            buscarTodosItens(gridMatricula, 'todosItens', ['pesquisarMatricula', 'relatorioMatricula']);
                            pesquisarMatricula(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function () {
                            buscarItensSelecionados('gridMatricula', 'selecionadoMatricula', 'cd_contrato', 'selecionaTodosMatricula', ['pesquisarMatricula', 'relatorioMatricula'], 'todosItens');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItens",
                        dropDown: menu,
                        id: "todosItens"
                    });
                    dom.byId("linkSelecionadosMatricula").appendChild(button.domNode);
                    if (hasValue(dijit.byId("menuManual"))) {
                        dijit.byId("menuManual").on("click",
                            function(e) {
                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323044', '765px', '771px');
                            });
                    }

                    dijit.byId("ckDtaMatricula").on("change", function (e) {
                        if (!e && !dijit.byId("ckDtaFinal").checked && !dijit.byId("ckDtaInicial").checked)
                            dijit.byId("ckDtaInicial").set("checked", true);
                    });

                    dijit.byId("ckDtaInicial").on("change", function (e) {
                        if (!e && !dijit.byId("ckDtaFinal").checked && !dijit.byId("ckDtaMatricula").checked)
                            dijit.byId("ckDtaInicial").set("checked", true);
                    });
                    dijit.byId("ckDtaFinal").on("change", function (e) {
                        if (!e && !dijit.byId("ckDtaInicial").checked && !dijit.byId("ckDtaMatricula").checked)
                            dijit.byId("ckDtaInicial").set("checked", true);
                    });
                    adicionarAtalhoPesquisa(['_aluno', '_turma', 'situacao', '_contrato', '_tipocontrato', 'dtaInicial', 'dtaFinal', 'statusAluno'], 'pesquisarMatricula', ready);
                    showCarregando();
                },
        function (error) {
            apresentaMensagem("apresentadorMensagemItem", error);
        });
            } catch (e) {
                postGerarLog(e);
            }
        })

    });
};

function montarTipoPesquisa(ready, Memory, filteringSelect) {
    try{
        var statusTipo = new Memory({
            data: [
            { name: "Todos", id: 0 },
            { name: "Matrícula", id: 1 },
            { name: "Rematrícula", id: 2 }
            ]
        });

        var _tipo = new filteringSelect({
            id: "_tipoPes",
            name: "_tipoPes",
            store: statusTipo,
            searchAttr: "name",
            value: 0,
            style: "width: 100%;"
        }, "_tipo");
    } catch (e) {
        postGerarLog(e);
    }
}

function montarTipoVinculado(ready, Memory, filteringSelect) {
    try {
        var statusTipo = new Memory({
            data: [
                { name: "Todos", id: 0 },
                { name: "Sim", id: 1 },
                { name: "Não", id: 2 },
                { name: "Não c/NF", id: 3 }
            ]
        });

        var _tipo = new filteringSelect({
            id: "_tipoVinculado",
            name: "_tipoVinculado",
            store: statusTipo,
            searchAttr: "name",
            value: 0,
            style: "width: 90px;"
        }, "_tipoVinculado");
    } catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoPesquisa(ready, Memory, filteringSelect) {
    try{
        var statusStore = new Memory({
            data: [
               { name: "Todos", id: 0 },
               { name: "Normal", id: 1 },
               { name: "Encerrada", id: 2 },
               { name: "Transferido", id: 3 },
               { name: "Movido/Desistente", id: 4 }
            ]
        });

        var situacao = new filteringSelect({
            id: "situacao",
            name: "situacao",
            store: statusStore,
            value: 1,
            searchAttr: "name",
            style: "width:90px;"
        }, "situacao");
    } catch (e) {
        postGerarLog(e);
    }
}

function loadTipoContrato(Memory, id) {
    try {
        var stateStore = new Memory({
            data: [
               { name: "Todos", id: 4 },
               { name: "Normal", id: 0 },
               { name: "Múltiplo", id: 1 },
               { name: "Profissionalizante", id: 2 },
               { name: "Informatica", id: 3 }
            ]
        });
        id.store = stateStore;
        id._onChangeActive = false;
        id.set("value", 4);
        id._onChangeActive = true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxMatricula(value, rowIndex, obj) {
    try{
        var gridName = 'gridMatricula';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_contrato", grid._by_idx[rowIndex].item.cd_contrato);

            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_contrato', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function atualizaGrid(gridName) {
    dijit.byId(gridName).update();
}

function eventoEditar(itensSelecionados, xhr, ready, Memory, FilteringSelect, ObjectStore) {
    try{
        apresentaMensagem('apresentadorMensagemMat', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            //showCarregando();
            var gridMatricula = dijit.byId('gridMatricula');
            apresentaMensagem('apresentadorMensagemMat', null);
            if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                montarCadastroMatriculaPartial(function () {
                    dijit.byId("alterarMatricula").on("click", function () {
                        //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                        alterarMatricula();
                    });
                    limparCadMatricula(xhr, ObjectStore, Memory, false);
                    setarTabCadMatricula(true);
                    dijit.byId('tabContainerMatricula').resize();
                    hideTagMatriculaTurma();
                    dijit.byId("cadMatricula").show();
                    dijit.byId('tabContainerMatricula').resize();
                    IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');

                    gridMatricula.itemSelecionado = itensSelecionados[0];
                    habilitarOnChange("ckAula", false);
                    keepValuesMatricula(null, dijit.byId('gridMatricula'), true, xhr, ready, Memory, FilteringSelect, ObjectStore);
                    habilitarOnChange("ckAula", true);
                }, Permissoes);
            }

            else {
                dijit.byId('dtaInicioMat').set('disabled', false);
                dijit.byId('dtaFinalMat').set('disabled', false);
                dijit.byId('tipoMatricula').set('disabled', false);
                dijit.byId('vl_pre_matricula').set('disabled', false);
                dijit.byId('ckTransfCad').set('disabled', false);
                dijit.byId('id_venda_pacote').set('disabled', false);
                //dijit.byId('ckManual').set('disabled', false);
                dijit.byId('ckCertificado').set('disabled', false);
                dojo.byId('id_motivo_aditamento').value = false;

                //habilita a ação relacionada Editar da grid dos titulos
                require([
                        "dojo/on",
                        "dijit/registry",
                        "dojo/ready",
                        "dojo/require"
                    ],
                    function (on, registry, ready) {
                        ready(function () {

                            registry.byId("acoesRelacionadasTit").dropDown._getFirst().set("disabled", false);

                        });
                    });


                limparCadMatricula(xhr, ObjectStore, Memory, false);
                setarTabCadMatricula(true);
                dijit.byId('tabContainerMatricula').resize();
                hideTagMatriculaTurma();
                dijit.byId("cadMatricula").show();
                dijit.byId('tabContainerMatricula').resize();
                IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');

                gridMatricula.itemSelecionado = itensSelecionados[0];
                habilitarOnChange("ckAula", false);
                keepValuesMatricula(null, dijit.byId('gridMatricula'), true, xhr, ready, Memory, FilteringSelect, ObjectStore);
                habilitarOnChange("ckAula", true);
            }

        }
    } catch (e) {
        postGerarLog(e);
    }
}

function montaObjetoMatricula(item) {
    try{
        var objeto =
            {
                cd_contrato: item.cd_contrato,
                nm_contrato: item.nm_contrato,
                cd_aluno: item.cd_aluno,
                no_aluno: item.no_aluno,
                cd_turma: item.cd_turma,
                no_turma: item.no_turma,
                dt_matricula: item.dt_matricula,
                dt_inicial: item.dt_inicial,
                dt_final: item.dt_final,
                dc_tipo_contrato: item.dc_tipo_contrato,
                dc_situacao: item.dc_situacao
            };
        return objeto;
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarMatricula(limparItens) {
    if (document.getElementById("ckVirada").checked) {
        var cd_contrato_anterior = 1;
    } else {
        var cd_contrato_anterior = null;
    }
    try{
        var nmContrato = hasValue(document.getElementById("_contrato").value) ? document.getElementById("_contrato").value.replace(".", "") : 0;
        var nm_matricula = hasValue(document.getElementById("nm_matricula_pesq").value) ? document.getElementById("nm_matricula_pesq").value.replace(".", "") : 0;
        var myStore =
             dojo.store.Cache(
                dojo.store.JsonRest({
                    target: Endereco() + "/api/secretaria/getMatriculaSearch?descAluno=" + encodeURIComponent(document.getElementById("_aluno").value) + "&descTurma=" +
                            encodeURIComponent(document.getElementById("_turma").value)
                                + "&inicio=" + document.getElementById("inicioAlunoTurma").checked + "&semTurma=" + document.getElementById("ckSemTurma").checked +
                                "&situacaoTurma=" + retornaStatus("situacao") + "&nmContrato=" + nmContrato +
                                "&tipo=" + retornaStatus("_tipoPes") + "&dtaInicio=" + dojo.byId("dtaInicial").value + "&dtaFim=" + dojo.byId("dtaFinal").value +
                                "&filtraMat=" + document.getElementById("ckDtaMatricula").checked + "&filtraDtaInicio=" + document.getElementById("ckDtaInicial").checked +
                                "&filtraDtaFim=" + document.getElementById("ckDtaFinal").checked + "&renegocia=" + document.getElementById("ckRenegociar").checked +
                                "&transf=" + document.getElementById("ckTransferir").checked + "&retornoEsc=" + document.getElementById("ckRetorno").checked +
                                "&cdNomeContrato=" + dijit.byId("cd_nome_contrato_pesq").value + "&nm_matricula=" + nm_matricula +
                                "&cdAnoEscolar=" + dijit.byId("pesqAnoEscolar").value + "&cdContratoAnterior=" + cd_contrato_anterior + "&tipoC=" + dijit.byId('tipoContrato').value +
                                "&status=" + dijit.byId("statusAluno").value + "&vinculado=" + dijit.byId("_tipoVinculado").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridMatricula");

        if (limparItens)
            grid.itensSelecionados = [];
        grid.noDataMessage = msgNotRegEnc;
        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function redirecionarCNABBoletos(tipo) {
    try{
        var gridMatricula = dijit.byId('gridMatricula');

        if (!hasValue(gridMatricula.itensSelecionados) || (gridMatricula.itensSelecionados.length <= 0))
            caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
        else
            if (hasValue(gridMatricula.itensSelecionados) && (gridMatricula.itensSelecionados.length > 1))
                caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            else
                window.location = Endereco() + '/Financeiro/CnabBoleto?cd_contrato=' + gridMatricula.itensSelecionados[0].cd_contrato + '&tipo=' + tipo;
    } catch (e) {
        postGerarLog(e);
    }
}

//Emitir carnê

function emitirRelatorioCarne(cdContrato, Permissoes, xhr) {

    var url = Endereco() + "/relatorio/GerarCarne?cd_contrato=" + cdContrato +
        "&parcIniCarne=" + (hasValue(dojo.byId("parcIniCarne").value) ? dojo.byId("parcIniCarne").value : 0) +
        "&parcFimCarne=" + (hasValue(dojo.byId("parcFimCarne").value) ? dojo.byId("parcFimCarne").value : 0) +
        "&imprimirCapaCarne=" + dijit.byId("imprimirCapaCarne").checked;
    
        xhr.get({
            url: url,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
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

function redirecionarMaterialDidatico(xhr, id_futura) {
    try {
        var gridMatricula = dijit.byId('gridMatricula');
        
        if (!hasValue(gridMatricula.itensSelecionados) || (gridMatricula.itensSelecionados.length <= 0))
            caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
        else
            if (hasValue(gridMatricula.itensSelecionados) && (gridMatricula.itensSelecionados.length > 1))
                caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            else {
                xhr.get({
                    url: Endereco() + "/api/escola/getValidaMatriculaHasNFMaterial?cd_contrato=" + gridMatricula.itensSelecionados[0].cd_contrato + "&id_futura=" + id_futura,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        if (hasValue(data) && hasValue(data['retorno'])) {
                            if (data.retorno.status == true) {
                                showCarregando();
                                window.location = Endereco() + '/Secretaria/Movimentos?tipo=' + VENDASPRODUTO + '&idOrigemNF=' + ORIGEMCHAMADONF +
                                    '&cdContrato=' + gridMatricula.itensSelecionados[0].cd_contrato + '&id_material_didatico=1&id_futura=' + id_futura;
                                hideCarregando();
                            } else {
                                caixaDialogo(DIALOGO_ERRO, msgErroMatriculaNaoPossuiVendaMaterial, null);
                            }
                            
                        }
                            
                        }
                    catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                        }
                    },
                    function (error) {
                        hideCarregando();
                        apresentaMensagem('apresentadorMensagem', error);
                    });

                
            }
    } catch (e) {
        postGerarLog(e);
    }
}

function componentesPesquisaMatricula() {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/componentesPesquisaMatricula",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (hasValue(data.retorno)){
                if (hasValue(data.retorno.nomesContrato)) {
                    data.retorno.nomesContrato.push({ cd_nome_contrato: 1000000, no_contrato: "Nenhum" });
                    criarOuCarregarCompFiltering("cd_nome_contrato_pesq", data.retorno.nomesContrato, "", 0, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_nome_contrato', 'no_contrato', MASCULINO);
                }
                if (hasValue(data.retorno.anosEscolares))
                    criarOuCarregarCompFiltering("pesqAnoEscolar", data.retorno.anosEscolares, "", 0, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_ano_escolar', 'dc_ano_escolar', MASCULINO);
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemMat', error);
    });
}

function limparInputDialogCarne() {
    dijit.byId("parcIniCarne").set("value", null);
    dijit.byId("parcFimCarne").set("value", null);
    dijit.byId("imprimirCapaCarne").set("checked", true);
}



function openDialogEnviarEmailAlunoRaf(Memory, ObjectStore) {
    try {
        apresentaMensagem('apresentadorMensagemEnviarEmailAlunoRaf', null);
        var grid = dijit.byId("gridMatricula");
        if (!hasValue(grid.itensSelecionados) || grid.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione alguma matrícula para enviar email para o aluno raf.', null);
        else if (grid.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas uma matrícula para enviar email para o aluno raf.', null);
        else {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/Aluno/VerificaExistAlunoraf?cd_aluno=" + grid.itensSelecionados[0].cd_aluno ,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try {
                  
                    if (hasValue(dados)) {
                        var retorno = jQuery.parseJSON(dados).retorno;
                        if (retorno != null && retorno != undefined &&
                            retorno['pessoaFisica'] != null && retorno['pessoaFisica'] != undefined &&
                            retorno.pessoaFisica['PessoaRaf'] != null &&
                            retorno.pessoaFisica['PessoaRaf'] != undefined &&
                            retorno.pessoaFisica['PessoaRaf'].length > 0
                                ) {
                            if (dojo.byId("email_aluno_envio_raf") != null &&
                                dojo.byId("email_aluno_envio_raf") != undefined &&
                                retorno.pessoaFisica.TelefonePessoa != null &&
                                retorno.pessoaFisica.TelefonePessoa != undefined &&
                                retorno.pessoaFisica.TelefonePessoa.length > 0 &&
                                hasValue(retorno.pessoaFisica.TelefonePessoa[0].dc_fone_mail)) {
                                dojo.byId("email_aluno_envio_raf").value =
                                    retorno.pessoaFisica.TelefonePessoa[0].dc_fone_mail;
                            } else {
                                caixaDialogo(DIALOGO_ERRO, "Não foi possível carregar o email deste aluno para realizar o envio. Tente novamente o contate o adminstrador do sistema", null);
                            }

                            loadAlunoRafMatricula(retorno.pessoaFisica.PessoaRaf);
                            dijit.byId('dialogEnviarEmailAlunoRaf').show();
                        } else {
                            caixaDialogo(DIALOGO_ERRO, "RAF não encontrado para este aluno.", null);
                            
                        }
                    } else {
                        caixaDialogo(DIALOGO_ERRO, "RAF não encontrado para este aluno.", null);
                    }

                    hideCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
                function (error) {
                    hideCarregando();
                console.log(error);
                caixaDialogo(DIALOGO_ERRO, "Raf não encontrado para este aluno.", null);
            });

           
        }

    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function enviarEmailAlunoRaf(Memory, ObjectStore) {

    apresentaMensagem('apresentadorMensagemEnviarEmailAlunoRaf', null);

    if ((dojo.byId("email_aluno_envio_raf") == null ||
            dojo.byId("email_aluno_envio_raf") == undefined) ||
        (dojo.byId("email_aluno_envio_raf") != null &&
            dojo.byId("email_aluno_envio_raf") != undefined &&
            !hasValue(dojo.byId("email_aluno_envio_raf").value))
    ) {
        caixaDialogo(DIALOGO_AVISO, 'Não foi possível carregar o email deste aluno para realizar o envio.', null);
    } else {
        var emailRafAluno = dojo.byId("email_aluno_envio_raf").value;
        var nmRaf = dijit.byId("nmRafEmail").value;
        showCarregando();
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getUrlEnviarEmailAlunoRaf?emailRafAluno=" + emailRafAluno + "&nmRaf=" + nmRaf,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dados) {
            try {
                if (hasValue(dados)) {
                    
                        try {
                            
                            caixaDialogo(DIALOGO_AVISO, "Email enviado com sucesso.", null);
                            dijit.byId("enviarEmailRafAluno").set("disabled", true);
                            hideCarregando();
                        } catch (e) {
                            hideCarregando();
                            postGerarLog(e);
                        }
                    
                } else {
                    hideCarregando();
                    caixaDialogo(DIALOGO_AVISO, "Não foi possível configurar os parametros (email ou nmRaf) para enviar o email. Tente novamente ou contate o adminstrador do sistema.", null);
                }

            }
            catch (e) {
                hideCarregando();
                caixaDialogo(DIALOGO_AVISO, "Houve um problema ao realizar a consulta que configura os parametros do envio do email. Tente novamente ou contate o adminstrador do sistema.", null);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagemEnviarEmailAlunoRaf', error);
            });
    }
        
    

    
}


function loadAlunoRafMatricula(PessoaRaf) {
    try {
        if (hasValue(PessoaRaf) && PessoaRaf != null && PessoaRaf.length > 0) {
            

            

            if (dojo.byId("cd_pessoa_raf_email") != null && dojo.byId("cd_pessoa_raf_email") != undefined) {
                dojo.byId("cd_pessoa_raf_email").value = PessoaRaf[0].cd_pessoa_raf;
            }

            if (dojo.byId("raf_cd_pessoa_email") != null && dojo.byId("raf_cd_pessoa_email") != undefined) {
                dojo.byId("raf_cd_pessoa_email").value = PessoaRaf[0].cd_pessoa;
            }

            if (hasValue(dijit.byId("nmRafEmail"))) {
                dijit.byId("nmRafEmail").set("value", PessoaRaf[0].nm_raf);
                dijit.byId("nmRafEmail").set("disabled", true);
            }

            if (hasValue(dijit.byId("ckLiberadoRafEmail"))) {
                dijit.byId("ckLiberadoRafEmail").set("value", PessoaRaf[0].id_raf_liberado);
                
                dijit.byId("ckLiberadoRafEmail").set("disabled", true);
                

            }

            if (hasValue(dijit.byId("ckBloqueadoRafEmail"))) {
                dijit.byId("ckBloqueadoRafEmail").set("value", PessoaRaf[0].id_bloqueado);
                dijit.byId("ckBloqueadoRafEmail").set("disabled", true);
            }

            if (hasValue(dijit.byId("ckTrocarSenhaRafEmail"))) {
                dijit.byId("ckTrocarSenhaRafEmail").set("value", PessoaRaf[0].id_trocar_senha);
                dijit.byId("ckTrocarSenhaRafEmail").set("disabled", true);
            }

            if (dijit.byId("nmTentativasRafEmail") != null && dijit.byId("nmTentativasRafEmail") != undefined) {
                dijit.byId("nmTentativasRafEmail").set("value", PessoaRaf[0].nm_tentativa);
                dijit.byId("nmTentativasRafEmail").set("disabled", true);
            }

            if (hasValue(dojo.byId("dt_expiracao_senhaRafEmail"))) {
                dijit.byId("dt_expiracao_senhaRafEmail")._onChangeActive = false;
                dojo.byId("dt_expiracao_senhaRafEmail").value = PessoaRaf[0].dta_expiracao_senha;
                dijit.byId("dt_expiracao_senhaRafEmail")._onChangeActive = true;
                dijit.byId("dt_expiracao_senhaRafEmail").set("disabled", true);
            }

            if (hasValue(dijit.byId("nmRafEmail"))) {
                if (PessoaRaf[0].hasPassCreated == true) {
                    dijit.byId("enviarEmailRafAluno").set("disabled", true);
                } else {
                    dijit.byId("enviarEmailRafAluno").set("disabled", false);
                }

            }




        } 

    } catch (e) {
        postGerarLog(e);
    }
}

