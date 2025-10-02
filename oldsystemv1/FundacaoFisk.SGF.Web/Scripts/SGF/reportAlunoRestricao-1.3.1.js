//#region criando dropDowns, contantes

function montarTipoPeriodo(Memory, on) {
    try{
        var dados = [{ name: "Data de Entrada", id: "1" },
                     { name: "Data de Saida", id: "2" }
                ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('tipoPeriodo').store = statusStore;
        dijit.byId('tipoPeriodo').set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function loadOrgaos() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getAllOrgaoFinanceiro",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataOrgao) {
                try {
                    var dataOrgao = jQuery.parseJSON(dataOrgao);
                    var itemsCb = [];
                    var cbOrgao = dijit.byId("cbOrgao");

                    Array.forEach(dataOrgao, function (value, i) {
                        itemsCb.push({ id: value.cd_orgao_financeiro, name: value.dc_orgao_financeiro });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbOrgao.store = stateStore;
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    postGerarLog(error);
                });
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
                montarTipoPeriodo(Memory, on);
                loadOrgaos();

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
            adicionarAtalhoPesquisa(['cbOrgao', 'tipoPeriodo', 'dtInicial', 'dtFinal'],'relatorio', ready);
    });
}
//#endregion

//#region  setDefaultAnoMes  - abrirItemFK -  pesquisarItemEstoqueFK - retornarItemFK - estoqueRpt -  emitirRelatorio

function emitirRelatorio() {
    try{
        if (!dijit.byId("formRptEstoque").validate())
            return false;
        var cd_orgao = hasValue(dijit.byId("cbOrgao").value) ? dijit.byId("cbOrgao").value : 0;
        var tipodata = hasValue(dijit.byId("tipoPeriodo").value) ? dijit.byId("tipoPeriodo").value : 0;
        var dtInicio = dojo.byId('dtInicial').value;
        var dtFinal = dojo.byId('dtFinal').value;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getUrlRelatorioAlunoRestricao?cdOrgao=" + cd_orgao + "&tipodata=" + tipodata + 
                "&dtInicial=" + dtInicio + "&dtFinal=" + dtFinal,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioAlunoRestricao?' + data, '1024px', '750px', 'popRelatorio');
        },
            function (error) {
                postGerarLog(error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion