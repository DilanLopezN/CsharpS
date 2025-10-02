var TODOS = 0;

function montarMetodosRelatorioPercentualTerminoEstagio() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button"
    ], function (ready, xhr, Memory, on, Button, window) {
        ready(function () {
            try {
                //montarTipos("tipoListagem", Memory);
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");
                //if (hasValue(dijit.byId("menuManual"))) {
                //dijit.byId("menuManual").on("click", function (e) {
                //    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323076', '765px', '771px');
                //});}
                //dijit.byId("tipoListagem").on("change", function (cd_tipo) {
                //    try {
                //        if (!hasValue(cd_tipo) || cd_tipo < 0)
                //            dijit.byId("tipoListagem").set("value", TODOS);
                //        else {
                //            if (hasValue(dijit.byId("tipoListagem").oldValue) && (dijit.byId("tipoListagem").oldValue != CLIENTES && dijit.byId("tipoListagem").oldValue != FUNCIONARIOS))
                //                apresentaMensagem('apresentadorMensagem', null);
                //            dijit.byId("tipoListagem").oldValue = cd_tipo;
                //            verificarTipoFiltros();
                //        }
                //    }
                //    catch (e) {
                //        postGerarLog(e);
                //    }
                //});
                sugerirDatas();
                populaProfessor('pesProf');
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function populaProfessor(field) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/professor/getAllProfessorTurma",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                if (data != null && hasValue(jQuery.parseJSON(data).retorno))
                    criarOuCarregarCompFiltering(field, jQuery.parseJSON(data).retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_fantasia', MASCULINO);
            });
        });
    }
    catch (e) {
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
        if (!dijit.byId("formRptPTerminoEst").validate())
            validado = false;
        if (!testaPeriodo(dataInicial, dataFinal))
            validado = false;

        if (!validado)
            return false;

        var cdProf = hasValue(dijit.byId("pesProf").value) ? dijit.byId("pesProf").value : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/coordenacao/GeturlrelatorioPercTerminoEstagio?&cdProf=" + cdProf +
                "&dtInicial=" + dojo.byId('dtInicial').value + "&dtFinal=" + dojo.byId('dtFinal').value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioPercentualTerminoEstagio?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
