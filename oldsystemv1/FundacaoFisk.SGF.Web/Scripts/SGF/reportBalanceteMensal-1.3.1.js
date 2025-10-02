function montarBalanceteMensal()
{
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/on",
    "dijit/form/Button"
    ], function (ready, xhr, on, Button) {
        ready(function () {
            try{
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");

                // Sugere o nível a analisar:
                xhr.get({
                    url: Endereco() + "/api/escola/getParametroNiveisPlanoContas",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try{
                        if (hasValue(data.retorno)) {
                            var cb_nivel = dijit.byId("nivel");
                            var nivel = parseInt(data.retorno);

                            if (nivel == 1 || nivel == 2) {
                                cb_nivel.constraints.max = nivel;
                                cb_nivel.set('value', nivel);
                                dojo.byId('nivel_escola').value = nivel;
                            }
                            else
                                dijit.byId('relatorio').set('disabled', true)
                        }
                        else
                            apresentaMensagem('apresentadorMensagem', data);
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323081', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['mes', 'ano', 'nivel'], 'relatorio', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function emitirRelatorio() {
    try{
        apresentaMensagem('apresentadorMensagem', null);

        if (!dijit.byId("formBalancete").validate())
        return false;

        var mes = dijit.byId("mes").value;
        var ano = dijit.byId("ano").value;
        var nivel = dojo.byId("nivel_escola").value;
        var nivel_analisar = dijit.byId("nivel").value;
        var mostrar_contas = dijit.byId("mostrar_contas").checked;

        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getUrlRelatorioBalanceteMensal?mes=" + mes + "&ano=" + ano + "&nivel=" + nivel + "&nivel_analisar=" + nivel_analisar +
                                "&mostrar_contas=" + mostrar_contas,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioBalanceteMensal?' + data, '1024px', '750px', 'popRelatorio');
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