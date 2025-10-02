//#region Monta relatório
var TODOS = 0;
function montarRelatorioPagamentoProfessores() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/data/ObjectStore",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/on",
    "dijit/form/FilteringSelect",
    ], function (ready, xhr, ObjectStore, Memory, Button, on, FilteringSelect) {
        ready(function () {
            try {
                sugerirDatas();
                criarOuCarregarCompFiltering("tipo_relatorio", [{ name: "Aulas Normais", id: "1" }, { name: "Aulas Substituição  ", id: "2" }, {name : "Atividade Extra", id: "3" }], "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name', MASCULINO);
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");
                findIsLoadComponetesRelatorio(xhr, ready, Memory, FilteringSelect);
                dijit.byId("tipo_relatorio").on("change", function (cd_tipo) {
                    try {
                        if (!hasValue(cd_tipo) || cd_tipo < 0)
                            dijit.byId("tipo_relatorio").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesqProf").on("change", function (cd_prof) {
                    try {
                        if (!hasValue(cd_prof) || cd_prof < 0)
                            dijit.byId("pesqProf").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId('dtInicial').on("change", function (data) {
                    apresentaMensagem('apresentadorMensagem', null);
                });
                dijit.byId('dtFinal').on("change", function (data) {
                    apresentaMensagem('apresentadorMensagem', null);
                });
            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
}

function findIsLoadComponetesRelatorio(xhr, ready, Memory, FilteringSelect) {
    xhr.get({
        url: Endereco() + "/api/coordenacao/getComponentesRelatorioPagamentoProfessores",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var data = jQuery.parseJSON(data).retorno;
            if (data.professores != null && data.professores.length > 0)
                criarOuCarregarCompFiltering("pesqProf", data.professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_fantasia', MASCULINO);
            showCarregando();
        }
        catch (e) {
            showCarregando
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagemMovto", error);
    });
}

function testaPeriodo(dataInicial, dataFinal) {
    try {
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


function emitirRelatorio() {
    try {
        var validado = true;
        apresentaMensagem('apresentadorMensagem', null);
        var dataInicial = dojo.byId("dtInicial").value;
        var dataFinal = dojo.byId("dtFinal").value;
        if (!dijit.byId("formRptPgtoProfessor").validate())
            validado = false;
        if (!testaPeriodo(dataInicial, dataFinal))
            validado = false;
      
        if (!validado)
            return false;
        dojo.xhr.get({
            url: Endereco() + "/api/coordenacao/GeturlrelatorioPagamentoProfessores?cd_tipo_relatorio=" + dijit.byId("tipo_relatorio").value + "&cdProf=" + dijit.byId("pesqProf").value +
                "&dtInicial=" + dojo.byId('dtInicial').value + "&dtFinal=" + dojo.byId('dtFinal').value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioPagamentoProfessores?' + data, '1000px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function sugerirDatas() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataInicialEFinalMes",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            if (hasValue(dataCorrente) && hasValue(dataCorrente.dt_ini_mes)) {
                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicial').attr("value", dataCorrente.dt_ini_mes);
                dijit.byId('dtInicial').oldValue = dijit.byId('dtInicial').value;
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", dataCorrente.dt_fim_mes);
                dijit.byId('dtFinal').oldValue = dijit.byId('dtFinal').value;
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}