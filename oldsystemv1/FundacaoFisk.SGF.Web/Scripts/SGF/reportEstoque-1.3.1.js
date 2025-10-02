//#region criando dropDowns, contantes

var MOVIMENTOS = 0;
var ITENS = 1;
var SALDO_REAL_BIBLIOTECA = 2;
var ANALITICO = 0;
var SINTETICO = 1;
var TIPOMOVIMENTO = null;

function montarMovimentos(Memory, on, ready) {
    ready(function () {
        try{
            var dados = [
                { name: "Movimentos", id: "0" },
                { name: "Itens", id: "1" },
                { name: "Saldo Real Biblioteca", id: "2" }
            ]
            var statusStore = new Memory({
                data: dados
            });
            dijit.byId('pesRelatorio').store = statusStore;
            dijit.byId('pesRelatorio').set("value", MOVIMENTOS);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};

function montarTipoRelatorio(Memory, on) {
    try{
        var dados = [{ name: "Analítico", id: "0" },
                     { name: "Sintético", id: "1" }]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('tipoRelatorio').store = statusStore;
        dijit.byId('tipoRelatorio').set("value", ANALITICO);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function recoverGrupoEstoque(fieldGrupo) {
    try{
        require(["dojo/_base/xhr"], function ( xhr) {
            xhr.get({
                url: Endereco() + "/api/financeiro/GetAllGrupoEstoqueAtivo",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataItem) {
                var data = $.parseJSON(dataItem);
                loadGrupoEstoque(data.retorno, fieldGrupo);
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGrupoEstoque(items, linkGrupo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbGrupo = dijit.byId(linkGrupo);

            itemsCb.push({
                id: 0,
                name: "Todos"
            });

            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbGrupo.store = stateStore;

            cbGrupo.set("value", 0);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaTipoItemRPT(xhr, Memory, FilteringSelect, array) {
    try{
        xhr.get({
            url: Endereco() + "/api/financeiro/getTipoItemMovimentaEstoque",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataTipoItem) {
            loadTipoItemRPT(jQuery.parseJSON(dataTipoItem).retorno, Memory, FilteringSelect, array);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTipoItemRPT(items, Memory, FilteringSelect, Array) {
    try{
        var itemsCb = [];
        var cbTipoItem = dijit.byId('tipoItem');
        var comCdTipoItem = dijit.byId('tipoItem');

        itemsCb.push({
            id: 0,
            name: "Todos"
        });

        Array.forEach(items, function (value, i) {
            itemsCb.push({ id: value.cd_tipo_item, name: value.dc_tipo_item });
        });
        var stateStore = new Memory({
            data: itemsCb
        });
        cbTipoItem.store = stateStore;

        cbTipoItem.set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaAnoMes(xhr, Memory, FilteringSelect, array) {
    try{
        xhr.get({
            url: Endereco() + "/api/financeiro/getFechamentoAnoMes",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataTipoItem) {
            loadAnoMes(jQuery.parseJSON(dataTipoItem).retorno, Memory, FilteringSelect, array);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadAnoMes(items, Memory, FilteringSelect, Array) {
    try{
        var itemsCb = [];
        var cbContagem = dijit.byId('cbContagem');

        Array.forEach(items, function (value, i) {
            itemsCb.push({
                id: value.cd_fechamento,
                name: value.desAnoMes,
                ano: value.nm_ano_fechamento,
                mes: value.nm_mes_fechamento
            });
        });

        var stateStore = new Memory({
            data: itemsCb
        });

        cbContagem.store = stateStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    dijit.byId("dtInicial")._onChangeActive = false;
    dijit.byId("dtFinal")._onChangeActive = false;

    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), date.getMonth(), 01));
                dojo.byId('dtInicialOld').value = dojo.byId('dtInicial').value;

                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
                dojo.byId('dtFinalOld').value = dojo.byId('dtFinal').value;

                dijit.byId("dtInicial")._onChangeActive = true;
                dijit.byId("dtFinal")._onChangeActive = true;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region Monta relatório de estoque
function montarMetodosEstoque() {
    require([
        "dojo/ready",
        "dojo/_base/array",
        "dojo/_base/xhr",
        "dojo/store/Memory",
        "dojo/on",
        "dijit/form/Button",
        "dijit/form/FilteringSelect",
        "dojo/data/ObjectStore",
        "dojo/date"
    ], function (ready, array, xhr, Memory, on, Button, FilteringSelect, ObjectStore, date) {
        ready(function () {
            try {
                //montarTipoItens(Memory, on);
                montarMovimentos(Memory, on, ready);
                montarTipoRelatorio(Memory, on);
                montarMeses('pesMes', Memory, on);
                recoverGrupoEstoque('pesGrupo');
                populaTipoItemRPT(xhr, Memory, FilteringSelect, array);
                sugereDataCorrente();


                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        var tipoRelatorioImp = dijit.byId('pesRelatorio').get('value');
                        emitirRelatorio(tipoRelatorioImp, date);
                    }
                }, "relatorio");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("pesquisarItemFK")))
                                montargridPesquisaItem(function () {
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemEstoqueFK();
                                    });
                                    dijit.byId('grupoPes').on("change", function (e) {
                                        dijit.byId("pesGrupo").set("value", e);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) {
                                        dijit.byId("fkItem").hide();
                                    });
                                    abrirItemFK();
                                }, true, true);
                            else
                                abrirItemFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesItem");

                var pesItem = document.getElementById('pesItem');
                if (hasValue(pesItem)) {
                    pesItem.parentNode.style.minWidth = '18px';
                    pesItem.parentNode.style.width = '18px';
                }

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dijit.byId("limparItemFK").set('disabled', true);
                        dojo.byId("cd_item").value = 0;
                        dijit.byId("noItem").set("value", "");
                    },
                }, "limparItemFK");

                if (hasValue(document.getElementById("limparItemFK"))) {
                    document.getElementById("limparItemFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparItemFK").parentNode.style.width = '40px';
                }

                dijit.byId('pesRelatorio').on("change", function (e) {
                    try{
                        if (e == MOVIMENTOS) {

                            dijit.byId("cbContagem")._onChangeActive = false;
                            dojo.byId('paneDataInicial').style.display = 'block';

                            dojo.byId('paMostrarDetalhes').style.display = '';
                            displayBotoesPanelOpcaoItem('none');                            
                            dojo.byId('tdChkItensMovimento').style.display = '';
                            dojo.byId('tdBlank').style.display = '';

                            dijit.byId("ckContagem").set("disabled", true);
                            dijit.byId("ckContados").set("disabled", true);
                            dijit.byId("ckContagem").set("checked", false);
                            dijit.byId("ckContados").set("checked", false);
                            dijit.byId("ckSemCC").set("checked", false);
                            dijit.byId("tipoRelatorio").set("disabled", false);
                            dijit.byId('cbContagem').set("disabled", true);
                            dijit.byId("tipoItem").set("disabled", false);
                            dijit.byId("tipoItem").set("value", 0);
                            dijit.byId('dtInicial').required = true;
                            dijit.byId('dtFinal').required = true;
                            dijit.byId("cbContagem").reset();
                            dijit.byId("cbContagem")._onChangeActive = true;
                        }
                        if (e == ITENS) {
                            dijit.byId("chkItensMovimento").set("value", false);

                            isAtivarChangeRadios(false)
                            dojo.byId('paneDataInicial').style.display = 'none';

                            dojo.byId('paMostrarDetalhes').style.display = 'block';
                            displayBotoesPanelOpcaoItem('');
                            dojo.byId('tdChkItensMovimento').style.display = 'none';
                            dojo.byId('tdBlank').style.display = 'none';

                            dijit.byId("ckContagem").set("disabled", false);
                            dijit.byId("ckContados").set("disabled", false);
                            dijit.byId("ckContados").set("checked", false);
                            dijit.byId("ckSemCC").set("checked", true);
                            dijit.byId("tipoRelatorio").set("disabled", true);
                            dijit.byId("tipoItem").set("disabled", false);
                            dijit.byId("tipoItem").set("value", 0);
                            dijit.byId('dtInicial').required = false;
                            dijit.byId('dtFinal').required = false;
                            dijit.byId("cbContagem")._onChangeActive = false;
                            dijit.byId('cbContagem').set("disabled", true);
                            dijit.byId("cbContagem")._onChangeActive = true;
                            isAtivarChangeRadios(true)
                            setDefaultAnoMes();

                        }
                        if (e == SALDO_REAL_BIBLIOTECA) {
                            dijit.byId("chkItensMovimento").set("value", false);

                            dijit.byId("cbContagem")._onChangeActive = false;
                            dojo.byId('paneDataInicial').style.display = 'none';
                            dojo.byId('paMostrarDetalhes').style.display = 'none';
                            dijit.byId("ckContagem").set("disabled", true);
                            dijit.byId("ckContados").set("disabled", true);
                            dijit.byId("ckContagem").set("checked", false);
                            dijit.byId("ckContados").set("checked", false);
                            dijit.byId("ckSemCC").set("checked", false);
                            dijit.byId("tipoRelatorio").set("disabled", true);
                            dijit.byId('cbContagem').set("disabled", true);
                            dijit.byId("tipoItem").set("value", 3);
                            dijit.byId("tipoItem").set("disabled", true);
                            dijit.byId('dtInicial').required = false;
                            dijit.byId('dtFinal').required = false;
                            dijit.byId("cbContagem").reset()
                            dijit.byId("cbContagem")._onChangeActive = true;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('ckContados').on("change", function (e) {
                    try{
                        dijit.byId("cbContagem")._onChangeActive = false;
                        dijit.byId('cbContagem').reset();
                        if (e == true) {
                            populaAnoMes(xhr, Memory, FilteringSelect, array);
                            dijit.byId('cbContagem').set("disabled", false);
                            dijit.byId("ckSemCC").set("checked", false);
                            dijit.byId("ckContagem").set("checked", false);
                            dijit.byId("ckSemCC").set("checked", false);
                        }
                        else
                            dijit.byId('cbContagem').set("disabled", true);
                        dijit.byId("cbContagem")._onChangeActive = true;
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('ckContagem').on("change", function (e) {
                    try{
                        dijit.byId("cbContagem")._onChangeActive = false;
                        dijit.byId("ckSemCC")._onChangeActive = false;
                        dijit.byId('cbContagem').reset();
                        if (e == true) {
                            dijit.byId("ckContados").set("checked", false);
                            dijit.byId("ckSemCC").set("checked", false);
                            dijit.byId('cbContagem').set("disabled", true);
                            dijit.byId("ckSemCC").set("checked", false);
                        }
                        dijit.byId("cbContagem")._onChangeActive = true;
                        dijit.byId("ckSemCC")._onChangeActive = true;
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });


                dijit.byId('ckSemCC').on("change", function (e) {
                    try{
                        dijit.byId("cbContagem")._onChangeActive = false;
                        dijit.byId("ckContagem")._onChangeActive = false;
                        dijit.byId("ckContados")._onChangeActive = false;
                        dijit.byId('cbContagem').reset();

                        if (e == true) {
                            populaAnoMes(xhr, Memory, FilteringSelect, array);
                            dijit.byId("ckContagem").set("checked", false);
                            dijit.byId("ckContados").set("checked", false);
                            dijit.byId('cbContagem').set("disabled", true);
                        }
                        else
                            dijit.byId('cbContagem').set("disabled", false);

                        dijit.byId("cbContagem")._onChangeActive = true;
                        dijit.byId("ckContagem")._onChangeActive = true;
                        dijit.byId("ckContados")._onChangeActive = true;
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('cbContagem').on("change", function (e) {
                    try{
                        dijit.byId("edAno")._onChangeActive = false;
                        dijit.byId("pesMes")._onChangeActive = false;

                        var contagem = dijit.byId('cbContagem').store.data;
                        var data = new Date();
                        var ano = data.getFullYear();
                        var mes = data.getMonth() + 1;

                        for (var i = 0; i < contagem.length; i++) {
                            if (e == contagem[i].id) {
                                ano = contagem[i].ano;
                                mes = contagem[i].mes;
                                break;
                            }
                        }
                        dojo.byId('edAno').value = ano;
                        dijit.byId('pesMes').set('value', mes);

                        dijit.byId("edAno")._onChangeActive = true;
                        dijit.byId("pesMes")._onChangeActive = true;
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('dtInicial').on("change", function (e) {
                    try{
                        var dtFinal = dijit.byId('dtFinal').get('value');

                        var retorno = testaPeriodo(e, dtFinal, date);

                        if (e == null) {
                            var data = dojo.date.locale.parse(dojo.byId("dtInicialOld").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                            dijit.byId('dtInicial').set('value', data);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('dtFinal').on("change", function (e) {
                    try{
                        var dtInicial = dijit.byId('dtInicial').get('value');

                        var retorno = testaPeriodo(dtInicial, e, date);
                        if (e == null) {
                            var dataF = dojo.date.locale.parse(dojo.byId("dtFinalOld").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                            dijit.byId('dtFinal').set('value', dataF);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('tipo').on("change", function (e) {
                    dijit.byId("tipoItem").set("value", 0);
                });

                dijit.byId('edAno').on("change", function (e) {
                    dijit.byId("cbContagem")._onChangeActive = false;
                    dijit.byId('cbContagem').reset();
                    dijit.byId("cbContagem")._onChangeActive = true;
                });

                dijit.byId('pesMes').on("change", function (e) {
                    dijit.byId("cbContagem")._onChangeActive = false;
                    dijit.byId('cbContagem').reset();
                    dijit.byId("cbContagem")._onChangeActive = true;
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        adicionarAtalhoPesquisa(['pesRelatorio', 'tipoItem', 'tipoRelatorio', 'pesGrupo', 'dtInicial', 'dtFinal'], 'relatorio', ready);
        if (hasValue(dijit.byId("menuManual"))) {
            dijit.byId("menuManual").on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323084', '765px', '771px');
                });
        }
    });
}
//#endregion

//#region  setDefaultAnoMes  - abrirItemFK -  pesquisarItemEstoqueFK - retornarItemFK - estoqueRpt -  emitirRelatorio

function displayBotoesPanelOpcaoItem(display) {

    dojo.byId('tdCkSemCC').style.display = display;
    dojo.byId('tdCkContagem').style.display = display;
    dojo.byId('tdCkContados').style.display = display;

    dojo.byId('tdLblCbContagem').style.display = display;
    dojo.byId('tdCbContagem').style.display = display;

    dojo.byId('tdLbledAno').style.display = display;
    dojo.byId('tdEdAno').style.display = display;

    dojo.byId('tdLblPesMes').style.display = display;
    dojo.byId('tdPesMes').style.display = display;
}

function setDefaultAnoMes() {
    try{
        var data = new Date();
        var anoD = data.getFullYear();
        var mesD = data.getMonth() + 1;
        dojo.byId('edAno').value = anoD;
        dijit.byId('pesMes').set('value', mesD);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function isAtivarChangeRadios(isAtivar) {
    try{
        if (isAtivar) {
            dijit.byId("ckContados")._onChangeActive = true;
            dijit.byId("ckContagem")._onChangeActive = true;
            dijit.byId("ckSemCC")._onChangeActive = true;
            dijit.byId("cbContagem")._onChangeActive = true;
        }
        else {
            dijit.byId("ckContados")._onChangeActive = false;
            dijit.byId("ckContagem")._onChangeActive = false;
            dijit.byId("ckSemCC")._onChangeActive = false;
            dijit.byId("cbContagem")._onChangeActive = false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFK() {
    try {
        dijit.byId("grupoPes")._onChangeActive = false;
        dijit.byId("tipo")._onChangeActive = false;
        //dijit.byId('comEstoque').set('disabled', true);
        limparPesquisaCursoFK(false);

        var tipoRpt = dijit.byId('pesRelatorio').get('value');
        if (tipoRpt == SALDO_REAL_BIBLIOTECA)
            dijit.byId("tipo").set("disabled", true);
        else
            dijit.byId("tipo").set("disabled", false);

        showP('comEstoqueTitulo', true);
        showP('comEstoqueCampo', true);
        dijit.byId("fkItem").show();
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("statusItemFK").set("disabled", true);

        var tipoItem = dijit.byId("tipoItem").get('value');
        var grupoItem = dijit.byId("pesGrupo").get('value');

        setTimeout(function () {
            try{
                dijit.byId("tipo").set('value', tipoItem);
                dijit.byId("grupoPes").set('value', grupoItem);
                dijit.byId("grupoPes")._onChangeActive = true;
                dijit.byId("tipo")._onChangeActive = true;
                pesquisarItemEstoqueFK();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, 100);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemEstoqueFK() {
    try{
        dijit.byId("grupoPes")._onChangeActive = false;
        dijit.byId("tipo")._onChangeActive = false;
        //dijit.byId('comEstoque').set('checked', true);
        var desItem = encodeURIComponent(document.getElementById("pesquisaItemServico").value);
        var inicio = document.getElementById("inicioItemServico").checked;
        var statusItem = retornaStatus("statusItemServico");
        var tipoItem = hasValue(dijit.byId('tipo').get('value')) ? dijit.byId('tipo').get('value') : 0;
        var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : 0;
        var ckEstoque = document.getElementById("comEstoque").checked;
        dijit.byId("grupoPes")._onChangeActive = true;
        dijit.byId("tipo")._onChangeActive = true;

        require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/financeiro/getItemFKEstoque?desc=" + desItem + "&inicio=" + inicio + "&status=" + statusItem + "&tipoItemInt=" + tipoItem + "&grupoItem=" + grupoItem + "&comEstoque=" + ckEstoque,
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                       idProperty: "cd_item"
                   }
                ), Memory({ idProperty: "cd_item" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaItem");
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

function retornarItemFK() {
    try{
        var gridPesquisaItem = dijit.byId("gridPesquisaItem");
        if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesquisaItem.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {
            dojo.byId("cd_item").value = gridPesquisaItem.itensSelecionados[0].cd_item;
            dojo.byId("noItem").value = gridPesquisaItem.itensSelecionados[0].no_item;
            dijit.byId('limparItemFK').set('disabled', false);
            dijit.byId("fkItem").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function estoqueRpt() {
    try{
        this.relatorio = dijit.byId('pesRelatorio').get('value');
        this.tipoItem = dijit.byId('tipoItem').get('value');
        this.grupoItem = dijit.byId('pesGrupo').get('value');
        this.tipoRpt = dijit.byId('tipoRelatorio').get('value');  // analítico // sintético
        this.item = dojo.byId('cd_item').value;
        this.dtaInicio = dojo.date.locale.parse(dojo.byId("dtInicial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dtaFim = dojo.date.locale.parse(dojo.byId("dtFinal").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.isColunaContagem = dijit.byId('ckContagem').checked;
        this.isColunaContatos = dijit.byId('ckContados').checked;
        this.anoMesContagem = dijit.byId('cbContagem').get('value');
        this.ano = dojo.byId('edAno').value;
        this.mes = dijit.byId('pesMes').get('value');
        this.dc_item = !hasValue(dojo.byId('noItem').value) ? "Todos" : dojo.byId('noItem').value;
        this.dc_grupo = !hasValue(dojo.byId('pesGrupo').value) ? "Todos" : dojo.byId('pesGrupo').value;
        this.isSemColunaC = dijit.byId('ckSemCC').checked;
        this.isApenasItensMovimento = dijit.byId('chkItensMovimento').checked;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function emitirRelatorio(value, date) {
    try{
        if (!dijit.byId("formRptEstoque").validate())
            return false;
        if (!testaPeriodo(dijit.byId('dtInicial').value, dijit.byId('dtFinal').value, date))
            return false;
        var estoqueUI = new estoqueRpt();

        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post(Endereco() + "/api/financeiro/getUrlRelatorioEstoque", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(estoqueUI)
            }).then(function (rptEstoque) {
                try{
                    if (!hasValue(rptEstoque.erro)) {
                        switch (parseInt(value)) {
                            case MOVIMENTOS:
                                abrePopUp(Endereco() + '/Relatorio/RelatorioEstoque?' + rptEstoque, '1024px', '750px', 'popRelatorio');
                                break
                            case ITENS:
                                abrePopUp(Endereco() + '/Relatorio/RelatorioEstoque?' + rptEstoque, '1024px', '750px', 'popRelatorio');//RelatorioItem
                                break;
                            case SALDO_REAL_BIBLIOTECA:
                                abrePopUp(Endereco() + '/Relatorio/RelatorioEstoque?' + rptEstoque, '1024px', '750px', 'popRelatorio');
                                break;
                            default: alert('Nenhum relatório selecionado');
                                break;
                        }
                    } else
                        apresentaMensagem('apresentadorMensagem', rptEstoque);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function testaPeriodo(dataInicial, dataFinal, date) {
    try{
        var retorno = true;
        if (date.compare(dataInicial, dataFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion