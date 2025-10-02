var TODOS = 0;

function montarMetodosSaldoFinanceira() {
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dijit/Dialog",
    "dijit/form/DateTextBox",
    ], function (ready, xhr, Memory, on, Button) {
        ready(function () {
            try {
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { } }, "relatorioSaldoFinan");
                var statusStore = new Memory({
                    data: [
                    { name: "Todos", id: 0 },
                    { name: "Caixas", id: 3 },
                    { name: "Bancos", id: 2 }
                    ]
                });
                dijit.byId("categoria").store = statusStore;
                dijit.byId("categoria").set("value", TODOS);
                dijit.byId("categoria").on("change", function (e) {
                    if (isNaN(e))
                        compQtd.set('value', TODOS);
                });
                dijit.byId("relatorioSaldoFinan").on("click", function (e) {
                    emitirRelatorio();
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323082', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['dataBase', 'categoria'], 'relatorioSaldoFinan', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function emitirRelatorio() {
    try {
        var dataBase = dojo.byId('dataBase').value;
        var categoria = dijit.byId('categoria').get('value');

        if (!dijit.byId("formPesqSaldoFinan").validate())
            return false;

        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getUrlRelatorioSaldoFinanceiro?dta_base=" + dataBase + "&tipo=" + categoria + "&tipoLiquidacao=" + document.getElementById("tipoLiquidacao").checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioSaldoFinanceiro?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
