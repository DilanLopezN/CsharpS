//#region criando dropDowns, contantes

function montarIdLogin(Memory, on) {
    try{
        var dados = [{ name: "Sim", id: true },
                     { name: "Não", id: false }
                ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('idLogin').store = statusStore;
        dijit.byId('idLogin').set("value", true);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function montarIdMatricula(Memory, on) {
    try {
        var dados = [
            { name: "Sim", id: 1 },
            { name: "Não", id: 0 },
            { name: "Todas", id: 2 }
        ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('idMatricula').store = statusStore;
        dijit.byId('idMatricula').set("value", 2);
    }
    catch (e) {
        postGerarLog(e);
    }
};

//#endregion

//#region Monta relatório de estoque
function montarTelaLogin() {
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
                sugereDataCorrente();
                montarIdLogin(Memory, on);
                montarIdMatricula(Memory, on);

                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
            adicionarAtalhoPesquisa(['idLogin', 'idMatricula', 'dtAnalise'],'relatorio', ready);
    });
}
//#endregion

//#region  setDefaultAnoMes  - abrirItemFK -  pesquisarItemEstoqueFK - retornarItemFK - estoqueRpt -  emitirRelatorio

function emitirRelatorio() {
    try{
        if (!eval(MasterGeral())) {
            caixaDialogo(DIALOGO_AVISO, 'Opção disponível somente para usuários Administradores do Sistema', null);
            return false;
        }
        if (!dijit.byId("formRptEstoque").validate())
            return false;
        var idLogin = hasValue(dijit.byId("idLogin").value) ? dijit.byId("idLogin").value : false;
        var idMatricula = hasValue(dijit.byId("idMatricula").value) ? dijit.byId("idMatricula").value : 0;
        var dtAnalise = dojo.byId('dtAnalise').value;
        var hhAnalise = dojo.byId('hhAnalise').value;
        dojo.xhr.get({
            url: Endereco() + "/api/escola/getUrlRelatorioLoginEscola?dtAnalise=" + dtAnalise + "&hhAnalise=" + hhAnalise + "&idLogin=" + idLogin +
                "&idMatricula=" + idMatricula,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioLoginEscola?' + data, '1024px', '750px', 'popRelatorio');
        },
            function (error) {
                postGerarLog(error);
            });
    }
    catch (e) {
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

                dijit.byId('dtAnalise').attr("value", date);
                dijit.byId('hhAnalise').attr("value", "08:00");
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}
