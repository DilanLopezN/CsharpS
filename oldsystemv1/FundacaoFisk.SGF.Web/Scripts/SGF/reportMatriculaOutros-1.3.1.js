//#region Monta relatório

function montarMetodosReportMatriculaOutros() {
    require([
    "dojo/ready",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/on",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, Memory, Button, JsonRest, ObjectStore, Cache, on, registry, ItemFileReadStore) {
        ready(function () {
            try {
                findIsLoadComponetesPesquisaReportMatricula();
                sugereDataCorrente();
                //montarStatus("statusAluno");
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                dijit.byId("pesProdutoFiltro").on("change", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("ckEscolas").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("dtInicial").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                dijit.byId("dtFinal").on("click", function (e) {
                    apresentaMensagem('apresentadorMensagem', null);
                });

                adicionarAtalhoPesquisa(['dtInicial', 'dtFinal'], 'relatorio', ready);
            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
}

function findIsLoadComponetesPesquisaReportMatricula() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/componentesPesquisaReportMatricula",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        showCarregando();
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno)) {
                        no_codigo = 'cd_produto'
                        no_nome = 'no_produto'
                        storeData = null;
                        storeData = [];
                        storeData.push({ id: 0, name: "Todos" });
                        value = 0;

                        if (data.retorno.length > 0) {

                            $.each(data.retorno, function (index, value) {
                                var valor = eval("value." + no_codigo)
                                if (valor != 4 && (hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : "") != "Profissionalizante") 
                                storeData.push({
                                    id: valor,
                                    name: hasValue(eval("value." + no_nome)) ? eval("value." + no_nome) : ""
                                });
                            });
                        }

                        var statusStore = new dojo.store.Memory({
                            data: storeData
                        });
                        var dijitElement = dijit.byId('pesProdutoFiltro');
                        dijitElement.store = statusStore;

                        if (hasValue(dijitElement, true) && value != null && hasValue(value, true))
                            dijitElement.set("value", value);
                }
            }
            hideCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function testaPeriodo(dataInicial, dataFinal) {
    try{
        var retorno = true;
        if (dojo.date.compare(dataInicial, dataFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function emitirRelatorio() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        var dataInicial = dojo.byId("dtInicial").value;
        var dataFinal = dojo.byId("dtFinal").value;
    
        if (!hasValue(dataFinal) && hasValue(dataInicial))
            dijit.byId('dtFinal').set('value', dataInicial);

        if (!testaPeriodo(dataInicial, dataFinal))
            return false;
        if (!dijit.byId('formRptMatricula').validate())
            return false;

        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getUrlRelatorioMatOut?qt_max=" + dijit.byId("qtdMax").value + 
                "&dtIni=" + dataInicial + "&dtFim=" + dataFinal +
                "&cd_produto=" + dijit.byId("pesProdutoFiltro").value + "&no_produto=" + dojo.byId("pesProdutoFiltro").value +
                "&todasescolas=" + dijit.byId("ckEscolas").checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                abrePopUp(Endereco() + '/Relatorio/RelatorioMatriculaOutros?' + data, '1024px', '750px', 'popRelatorio');
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }

}

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

